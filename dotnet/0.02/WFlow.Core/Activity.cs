using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

using WFlow.Graphs;
using WFlow.Entities;
using WFlow.Utils;
using WFlow.Repositories;

namespace WFlow
{
    public class Activity : IActivity
    {
        public Activity(Engine engine, INode node)
        {
            this.Engine = engine;
            this.graph = node;
        }

        public Activity(Activity parent,INode node, ProcessContext context = null)
        {
        }
        /// <summary>
        /// 根据id创建活动对象
        /// </summary>
        /// <param name="id"></param>
        /// <param name="engine"></param>
        public Activity(Engine engine, Guid id,ProcessContext context=null)
        {
            this.Engine = engine;
            this.FlowRepository = engine.ResolveFlowRepository(context);
            this.id = id;
        }

        public Activity(Engine engine, ActivityEntity entity, ProcessContext context =null)
        {
            this.Engine = engine;
            this.FlowRepository = engine.ResolveFlowRepository(context);
            this.entity = entity;
            this.id = entity.Id;
            

        }

        public Activity(Activity parent, ActivityEntity entity)
        {
            this.parent = parent;
            this.Engine = parent.Engine;
            this.FlowRepository = parent.FlowRepository;
            this.entity = entity;
            parent.AddChild(this);
        }

        public Activity(Engine engine, Activity parent, INode node, string version,object inputs, IUser creator, IUser owner,ProcessContext processContext )
        {
            this.Engine = engine;
            this.graph = node;
            this.parent = parent;
            

            #region init entity
            var entity = new ActivityEntity()
            {
                Id = Guid.NewGuid(),
                NodeName = node.Name,
                NodePath = parent == null ? node.Name : parent.NodePath + "/" + node.Name,
                Status = ActivityStates.Creating,
                Version = version ?? parent?.Version,
                Domain = engine.Name,
                ActionType = node.InstanceType ?? engine.DefaultActionType,
                Graph = JSON.Stringify(node),
                HasChildren = node.Nodes != null && node.Nodes.Count > 0,
                Inputs = JSON.Stringify(inputs),
                CreateTime = DateTime.Now,
                CreatorId = creator.Id,
                CreatorName = creator.Name
            };

            this.entity = entity;
            this.Owner = owner;
            #endregion

            if (parent != null)
            {
                this.FlowRepository = parent.FlowRepository;
                this.AddChild(this);
            }
            else this.FlowRepository = engine.ResolveFlowRepository(processContext);

            #region init states
            var states = new JObject();
            var metas = (node as Node).Metas;
            if (metas != null)
            {
                foreach (var pair in metas)
                {
                    states.Add(pair.Key, pair.Value.DeepClone());
                }
            }
            if (parent != null && node.Imports != null)
            {
                var pStates = parent.States as JObject;
                foreach (var pair in node.Imports)
                {
                    var value = JSON.GetPathValue(pStates, pair.Value);
                    JSON.SetPathValue(states, pair.Key, value);
                }

            }
            
            this.entity.States = states.ToString();
            this.states = states;
            #endregion
        }






        /// <summary>
        /// 流程引擎
        /// 上面有数据库操作对象，可以用来保存数据
        /// 还有一些配置信息
        /// </summary>
        public Engine Engine { get; private set; }
        
        public IFlowRepository FlowRepository {
            get;private set;
        }

        #region ID,名称与实体
        Guid? id;
        public Guid Id
        {
            get
            {
                if (id == null)
                {
                    if (this.entity != null) id = this.entity.Id;
                }
                return id ?? Guid.Empty;
            }
        }

        ActivityEntity entity;

        public ActivityEntity Entity
        {
            get
            {
                if (this.entity == null)
                {
                    if (this.id != null)
                    {
                        this.entity = this.FlowRepository.GetActivityRuntimeById(this.id.Value);
                    }
                }

                return this.entity;
            }
        }

        public string Version
        {
            get { return this.Entity.Version; }
        }
        public string NodePath => this.Entity.NodePath;

        public string NodeName => this.Entity.NodeName;

        public Guid FlowId => this.Entity.FlowId;
        #endregion

        #region 树形结构与链表结构

        public Guid? ParentId => this.Entity.ParentId;
        Activity parent;
        public IActivity Parent
        {
            get
            {
                if (this.parent == null)
                {
                    if (this.Entity.ParentId != null)
                    {
                        if (this.entity.ParentActivity != null)
                        {
                            this.parent = new Activity(this.Engine, this.entity.ParentActivity);

                        }
                        else
                        {
                            this.parent = new Activity(this.Engine, this.entity.ParentId.Value);
                        }
                    }
                    this.parent.AddChild(this);
                }
                return parent;
            }
            
        }

        List<IActivity> children;
        List<Activity> createdChildren;
        public IReadOnlyList<IActivity> Children
        {
            get
            {
                if (this.children == null)
                {
                    var childEntities = this.FlowRepository.ListActivitiesRuntimeByParentId(this.Id);
                    var children = this.children = new List<IActivity>();
                    var createdChildren = this.createdChildren;
                    this.createdChildren = null;
                    foreach (var childEntity in childEntities)
                    {
                        var existed = createdChildren.FirstOrDefault(a => a.Id == childEntity.Id);
                        if (existed != null)
                        {
                            if (existed.entity == null) existed.entity = childEntity;
                            if (existed.parent == null) existed.parent = this;
                            children.Add(existed);
                        }
                        else
                        {
                            existed = new Activity(this, childEntity);

                        }
                        
                    }
                }
                return this.children;
            }
        }
        internal Activity FindUndealedChild(string name) {
            Activity child;
            if (this.createdChildren != null) {
                child = this.createdChildren.FirstOrDefault(child=>child.NodeName ==name && (child.Status== ActivityStates.Creating || child.Status== ActivityStates.Created));
                if (child != null) return child;
            }
            return this.Children.FirstOrDefault(child => child.NodeName == name && (child.Status == ActivityStates.Creating || child.Status == ActivityStates.Created)) as Activity;
        }
        void AddChild(Activity child)
        {
            if (this.children == null)
            {
                if (this.createdChildren == null) this.createdChildren = new List<Activity>();
                else
                {
                    if (Engine.Development)
                    {
                        if (this.createdChildren.Any(c => c.Id == child.Id)) throw new Exception("重复添加了相同Id的子活动对象");
                    }
                }
                this.createdChildren.Add(child);
            }
            else
            {
                if (Engine.Development)
                {
                    if (this.children.Any(c => c.Id == child.Id)) throw new Exception("重复添加了相同Id的子活动对象");
                }
                this.children.Add(child);
            }
        }

        Dictionary<string, Guid?> nexts;
        bool nextsChanged;

        public IReadOnlyDictionary<string, Guid?> Nexts
        {
            get
            {
                if (this.nexts == null)
                {
                    this.nexts = JSON.Parse<Dictionary<string, Guid?>>(entity.Nexts);
                }
                return nexts;
            }
        }

        internal void AddNexts(string name, Guid? nextActivityId = null)
        {
            if (this.nexts == null)
            {
                this.nexts = JSON.Parse<Dictionary<string, Guid?>>(entity.Nexts);
                if (this.nexts == null) this.nexts = new Dictionary<string, Guid?>();
            }
            this.nexts.Add(name, nextActivityId);
            nextsChanged = true;

        }
        #endregion

        #region 图
        INode graph;

        public INode Graph
        {
            get
            {
                if (graph == null)
                {
                    this.graph = JSON.Parse<Node>(this.Entity.Graph);
                }
                return graph;
            }
        }
        #endregion

        #region 属性:状态

        bool statusChanged;
        public ActivityStates Status
        {
            get { return this.Entity.Status; }
            private set { this.Entity.Status = value; this.statusChanged = true; }
        }

        public bool IsFulfilled {
            get {
                var status = this.Status;
                return status == ActivityStates.Done || status == ActivityStates.Error || status == ActivityStates.Canceled;
            }
        }

        private bool statesChanged;

        JObject states;
        public JObject States
        {
            get
            {
                if (this.states == null)
                {
                    if (!string.IsNullOrWhiteSpace(this.Entity.States))
                    {
                        this.states = JObject.Parse(this.Entity.States);
                    }
                }
                return this.states;
            }
        }

        public T State<T>(string key)
        {
            if (this.states == null)
            {
                if (!string.IsNullOrWhiteSpace(this.Entity.States))
                {
                    this.states = JSON.Parse<JObject>(this.Entity.States);
                }
            }
            return JSON.GetPathValue<T>(this.states,key);
        }

        public object State(string path) {
            if (this.states == null)
            {
                if (!string.IsNullOrWhiteSpace(this.Entity.States))
                {
                    this.states = JObject.Parse(this.Entity.States);
                }
            }
            return JSON.GetPathValue(this.states,path);
        }
        public Activity State(string key, object value)
        {
            return this.State(key, JToken.FromObject(value));
        }

        public Activity State(string key, JToken value)
        {
            if (this.states == null)
            {
                if (!string.IsNullOrWhiteSpace(this.Entity.States))
                {
                    this.states = JObject.Parse(this.Entity.States);
                }
                if (this.states == null) this.states = new JObject();
            }
            JSON.SetPathValue(this.states,key,value);
            this.statesChanged = true;
            return this;
        }

        public string this[string key]
        {
            get
            {
                if (this.states == null) return null;

                this.states.TryGetValue(key, out JToken value);
                return value?.ToString();
            }
            protected set
            {
                this.State(key, value);
            }
        }

        object results;
        public object Results
        {
            set
            {
                this.results = value;
            }
            get { return this.results; }
        }


        #endregion



        #region 记录
        #region 新建 Created
        public DateTime CreateTime { get { return this.Entity.CreateTime; } }
        IUser creator;

        public IUser Creator
        {
            get
            {
                if (this.creator == null)
                {
                    this.creator = User.Creator(this.Entity);
                }
                return this.creator;
            }
        }
        #endregion

        #region 处理
        public DateTime? DealTime { get { return this.Entity.DealTime; } private set { this.Entity.DealTime = value; } }


        IUser dealer;
        public IUser Dealer
        {
            get
            {
                if (this.dealer == null) this.dealer = User.Dealer(this.Entity);
                return this.dealer;
            }
            private set
            {
                if (this.dealer != value) this.dealer = User.Dealer(this.Entity, value);
            }
        }

        IUser owner;
        public IUser Owner
        {
            get
            {
                if (this.owner == null) this.owner = User.Owner(this.Entity);
                return this.owner;
            }
            private set
            {
                if (this.owner != value) this.owner = User.Owner(this.Entity,value);
            }
        }


        public DateTime? DoneTime { get { return this.Entity.DoneTime; } private set { this.Entity.DoneTime = value; } }
        #endregion

        public DateTime? CloseTime { get { return this.Entity.CloseTime; } private set { this.Entity.CloseTime = value; } }


        IUser closer;
        public IUser Closer
        {
            get
            {
                if (this.closer == null) this.closer = User.Closer(this.Entity);
                return this.owner;
            }
            private set
            {
                if (this.closer != value) this.closer = User.Closer(this.Entity, value);
            }
        }
        #endregion




        #region 执行
        public IAction Action { get; private set; }

        public ActivityStates Deal(object inputs, IUser dealer, object context)
        {
            //创建上下文
            var processContext = new ProcessContext(this, inputs, dealer, context);
            return this.Deal(inputs,processContext);
        }

        internal ActivityStates Deal(object inputs, ProcessContext processContext)
        {
            //终态，直接返回
            if (this.IsFulfilled) return this.Status;

            // 用户检查
            if (this.Status != ActivityStates.Created
                && processContext.Dealer != null
                && this.Owner != null
                && this.owner.Id != processContext.Dealer.Id
                ) return this.Status;

            // 合并Inputs到状态中
            this.CombineInputsToStates(inputs);

            // 处理者
            this.Dealer = processContext.Dealer;

            // 加载活动
            this.Action = this.Engine.LoadAction(this.Graph.InstanceType, processContext)??new Action();
            if (this.Predicate(inputs, processContext))
            {
                var trans = this.FlowRepository.CreateTransaction(processContext.UserContext);
                try {
                    trans.TryBegin();
                    // 执行得到执行结果
                    var executeResult = this.Execute(inputs, processContext);
                    // 试图结束当前活动
                    this.Postdicate(processContext, trans);
                    // 根据执行结果看是否执行下一步
                    this.ResolveNexts(executeResult, processContext, trans);
                    
                } catch (Exception ex) {
                    trans.TryRollback();
                    throw ex;
                }
                this.Todo(processContext);
            }
            return this.Status;
        }
        /// <summary>
        /// 合并inputs数据到内部状态
        /// </summary>
        /// <param name="inputs"></param>
        void CombineInputsToStates(object inputs)
        {
            if (inputs == null) return;
            var jInput = JObject.FromObject(inputs);
            var states = this.States;
            foreach (var pair in jInput)
            {
                states[pair.Key] = pair.Value;
                this.statesChanged = true;
            }
        }
        
        bool Predicate(object inputs, ProcessContext context)
        {
            // 已创建，但尚未开始的可以做执行检查
            if (this.Status != ActivityStates.Creating && this.Status != ActivityStates.Created)
            {
                return true;
            }
            // 创建事务
            var trans = this.FlowRepository.CreateTransaction(context.UserContext);
            try {
                // 开始事务
                trans.TryBegin();
                // 执行Action的predicate获取到owner
                var owner = this.Action.Predicate(this, inputs, context);
                // 返回null，表示活动的进入检查不成功，返回false
                if (owner == null)
                {
                    // 活动状态变更为处理中
                    this.Status = ActivityStates.Created;
                    this.SaveChanges(trans);
                    trans.TryCommitAsync();
                    return false;
                }
                // 指定了有效所有者才会赋值
                if (!string.IsNullOrEmpty(owner.Id)) this.Owner = owner;
                // 活动对象的状态变更为处理中
                this.Status = ActivityStates.Dealing;
                this.SaveChanges(trans);
                trans.TryCommit();
                return true;
            } catch (Exception ex) {
                trans.TryRollback();
                throw ex;
            }
            
        }
        object Execute(object inputs,ProcessContext protecssContext)
        {
            //正在处理/已处理还未完成的活动可以被执行
            if (this.Status != ActivityStates.Dealing && this.Status != ActivityStates.Dealed) return null;

            var executeResults = this.results = this.Action.Execute(this,inputs, protecssContext);
            if (executeResults == null) return null;
            var jobj=  JObject.FromObject(executeResults);
            this.results = jobj;
            foreach (var pair in jobj)
            {
                this.State(pair.Key, pair.Value);
            }
            this.Status = ActivityStates.Dealed;
            return executeResults;
        }

        Navigations ResolveNexts(object executeResults, ProcessContext processConext,ITransaction trans)
        {
            // 已处理，且状态有变化的才能开始寻找下一步
            if (this.Status != ActivityStates.Dealed || !this.statesChanged || executeResults == null) return null;
            if (this.Graph.Nexts == null || this.Graph.Nexts.Count == 0) return null;
            var navs = new Navigations(this, executeResults);
            foreach (var assoc in this.Graph.Nexts) {
                navs.AddIfPass(assoc,processConext);
            }
            return navs.Save(trans);
        }



        void Postdicate(ProcessContext processContext,ITransaction trans=null)
        {
            // 已处理，且状态有变化的才能执行结束检查
            if (this.Status != ActivityStates.Dealed || !this.statesChanged || results == null) return;
            if (!this.Action.Postdicate(this, processContext)) return;
            this.Status = ActivityStates.Done;
            this.Entity.DoneTime = DateTime.Now;
            if (this.Graph.Exports != null && this.Graph.Exports.Count > 0)
            {
                var parent = this.Parent as Activity;
                var parentStates = parent.States as JObject;
                foreach (var pair in this.Graph.Exports)
                {
                    var value = JSON.GetPathValue(this.states, pair.Key);
                    JSON.SetPathValue(parentStates, pair.Value, value);
                    parent.statesChanged = true;
                }
                parent.SaveChanges(trans);
                this.Todo(parent,null);
            }
            this.SaveChanges(trans);
        }

        



        #endregion

        Queue<KeyValuePair<Activity,object>> todos;

        public void Todo(Activity activity,object inputs)
        {
            if (this.todos == null) this.todos = new Queue<KeyValuePair<Activity, object>>();
            for (int i = 0, j = this.todos.Count; i < j; i++) {
                var pair = this.todos.Dequeue();
                if (pair.Key.Id != activity.Id) this.todos.Enqueue(pair);
                else if (pair.Key != activity) throw new Exception("相同Id的Activity有不同的对象实例");
            }
            this.todos.Enqueue(new KeyValuePair<Activity, object>(activity,inputs));
        }

        void Todo(ProcessContext processContext)
        {
            while (this.todos != null) {
                var todos = this.todos;
                this.todos = null;
                foreach (var pair in todos) {
                    pair.Key.Deal(pair.Value, processContext);
                }
            }
        }

        public override string ToString()
        {
            return this.NodePath + "#" + this.Id;
        }




        void SaveChanges(ITransaction trans=null) {
            if (this.statesChanged) {
                this.Entity.States = this.states.ToString();
                var values = new Dictionary<string, string>();
                if (this.results != null)
                {
                    values.Add("results", this.Entity.Results = this.results.ToString());
                }
                if (this.nextsChanged)
                {
                    values.Add("nexts", this.Entity.Nexts = JSON.Stringify(this.nexts));
                }
                if (values.Count == 0) this.FlowRepository.SaveActivityStates(this.entity, trans);
                else this.FlowRepository.SaveActivityStatesAndValues(this.entity,values,trans);
            } else {
                if (this.results != null || this.nextsChanged || this.statusChanged) {
                    throw new Exception("state未变，但results/nexts/status发生了变化，这在逻辑上是错误的");
                }

            }
            
            
        }
    }
}
