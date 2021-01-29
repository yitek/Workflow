using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using WFlow.Graphs;

namespace WFlow
{
    public class Action:IReadOnlyState
    {
        public Action(Engine engine,Node node) {
            this.Engine = engine;
        }
        /// <summary>
        /// 根据id创建活动对象
        /// </summary>
        /// <param name="id"></param>
        /// <param name="engine"></param>
        public Action(Engine engine,Guid id) {
            this.Engine = engine;
            this.id = id;
        }

        public Action(Engine engine,ActivityEntity entity) {
            this.Engine = engine;
            this.entity = entity;
            this.id = entity.Id;

        }

        public Action(Action parent, ActivityEntity entity) {
            this.parent = parent;
            this.Engine = parent.Engine;
            this.entity = entity;
        }

        public Action(Engine engine,Action parent, Node node,string version, object inputs, IUser creator,IUser dealer=null)
        {
            this.Engine = engine;
            this.graph = node;
            this.parent = parent;

            #region init entity
            var entity = new ActivityEntity()
            {
                Id = Guid.NewGuid(),
                NodeName = node.Name,
                NodePath = parent==null?node.Name:parent.NodePath + "/" + node.Name,
                Status = ActivityStates.Created,
                Version = version??parent?.Version,
                Domain = engine.Name,
                ActivityType = node.InstanceType ?? engine.DefaultActivityInstanceType,
                Graph = JSON.Stringify(node),
                HasChildren = node.Nodes != null && node.Nodes.Count > 0,
                Inputs = JSON.Stringify(inputs),
                CreateTime = DateTime.Now,
                CreatorId = creator.Id,
                CreatorName = creator.Name
            };

            this.entity = entity;
            if (dealer != null) {
                this.dealer = dealer;
                entity.DealerId = dealer.Id;
                entity.DealerName = dealer.Name;
            }
            #endregion
            #region init states
            var states = new JObject();
            if (node.Meta != null)
            {
                foreach (var pair in node.Meta)
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
            if (inputs != null)
            {
                var inputObj = JObject.FromObject(inputs);
                foreach (var pair in inputObj)
                {
                    states[pair.Key] = pair.Value;
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

        #region ID,名称与实体
        Guid? id;
        public Guid Id {
            get {
                if (id == null) { 
                    if(this.entity!=null) id = this.entity.Id;
                }
                return id.HasValue ? id.Value : Guid.Empty ;
            }
        }

        ActivityEntity entity;

        public ActivityEntity Entity {
            get {
                if (this.entity == null) {
                    if (this.id != null) {
                        this.entity = this.Engine.FlowRepository.GetById(this.id.Value);
                    }
                }

                return this.entity;
            }
        }

        public string Version
        {
            get { return this.Entity.Version; }
        }
        public string NodePath { get { return this.Entity.NodePath; } }

        public string Name
        {
            get
            {
                return this.Entity.NodeName;
            }
        }
        #endregion

        #region 树形结构与链表结构
        Action parent;
        public Action Parent {
            get {
                if (this.parent == null) {
                    if (this.Entity.ParentId != null) {
                        if (this.entity.ParentActivity != null)
                        {
                            this.parent = new Action(this.Engine, this.entity.ParentActivity);
                            
                        }
                        else {
                            this.parent = new Action(this.Engine,this.entity.ParentId.Value);
                        }
                    }
                    this.parent.AddChild(this);
                }
                return parent;
            }
            private set {
                this.parent = value;
            }
        }

        List<Action> children;
        List<Action> createdChildren;
        public IReadOnlyList<Action> Children {
            get {
                if (this.children == null) {
                    var childEntities = this.Engine.FlowRepository.ListByParentId(this.Id);
                    var children = new List<Action>();
                    foreach (var childEntity in childEntities) {
                        var existed = this.createdChildren.FirstOrDefault(a => a.Id == childEntity.Id);
                        if (existed != null)
                        {
                            if (existed.entity == null) existed.entity = childEntity;
                            if (existed.parent == null) existed.parent = this;
                            
                        }
                        else {
                            existed = new Action(this, childEntity);
                            
                        }
                        children.Add(existed);
                    }
                    this.children = children;
                    this.createdChildren = null;
                }
                return this.children;
            }
        }

        void AddChild(Action action) {
            if (this.children == null)
            {
                if (this.createdChildren == null) this.createdChildren = new List<Action>();
                else {
                    if (Engine.Development)
                    {
                        if (this.createdChildren.Any(c => c.Id == action.Id)) throw new Exception("重复添加了相同Id的子活动对象");
                    }
                }
                this.createdChildren.Add(action);
            }
            else {
                if (Engine.Development) {
                    if (this.children.Any(c => c.Id == action.Id)) throw new Exception("重复添加了相同Id的子活动对象");
                }
                this.children.Add(action);
            }
        }

        Dictionary<string, Guid?> nexts;
        bool nextsChanged;

        public IReadOnlyDictionary<string, Guid?> Nexts {
            get {
                if (this.nexts == null) {
                    this.nexts = JSON.Parse<Dictionary<string, Guid?>>(entity.Nexts);
                }
                return nexts;
            }
        }

        void AddNexts(string name, Guid? nextActivityId=null) {
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
        Node graph;

        public Node Graph {
            get {
                if (graph == null) {
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

        private bool statesChanged;

        JObject states;
        public object States
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
                    this.states = JObject.Parse(this.Entity.States);
                }
            }
            this.states.TryGetValue(key, out JToken value);
            return value.ToObject<T>();
        }
        public Action State(string key, object value)
        {
            return this.State(key,JToken.FromObject(value));
        }

        public Action State(string key, JToken value)
        {
            if (this.states == null)
            {
                if (!string.IsNullOrWhiteSpace(this.Entity.States))
                {
                    this.states = JObject.Parse(this.Entity.States);
                }
                if (this.states == null) this.states = new JObject();
            }
            this.states[key] = value;
            this.statesChanged = true;
            return this;
        }

        public string this[string key]
        {
            get
            {
                if (this.states == null) return null;

                this.states.TryGetValue(key, out JToken value);
                return value == null ? null : value.ToString();
            }
            protected set
            {
                this.State(key, value);
            }
        }

        object results;
        public object Results {
            set {
                this.results = value;
            }
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
                    this.creator = User.GetCreator(this.Entity);
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
                if (this.dealer == null && this.Entity.DealerId != null)
                {
                    this.dealer = User.GetDealer(this.Entity);
                }
                return this.dealer;
            }
        }

        
        public DateTime? DoneTime { get { return this.Entity.DoneTime; } private set { this.Entity.DoneTime = value; } }
        #endregion

        public DateTime? CloseTime { get { return this.Entity.CloseTime; } private set { this.Entity.CloseTime = value; } }

        
        IUser closer;
        public IUser Canceler
        {
            get
            {
                if (this.closer == null && this.Entity.CloserId != null)
                {
                    this.closer = User.GetCloser(this.Entity);
                }
                return this.dealer;
            }
        }
        #endregion

        #region Activity
        IActivity GetActivity(Context context) { return null; }
        #endregion

        #region 执行

        public ActivityStates Deal(object inputs, IUser dealer, Context context)
        {
            //终态，直接返回
            if (this.Status == ActivityStates.Canceled || this.Status == ActivityStates.Error || this.Status == ActivityStates.Done) return this.Status;

            var activity = this.GetActivity(context);
            this.InternalDeal(activity,inputs,dealer,context);
        }

        public ActivityStates InternalDeal(IActivity activity,object inputs, IUser dealer, Context context) {

            //检查
            if (this.Predicate(activity, inputs, dealer, context)) { 
                //执行
                var executeResults = this.Execute(activity, inputs, dealer, context);
                if (executeResults != null) {
                    this.Navigate(activity, executeResults, dealer, context);
                    this.Postdicate(activity, this.results, context);
                }
                
            }
            
            
            return this.Status;
        }
        bool Predicate(IActivity activity, object inputs,IUser dealer, Context context) {
            // 已创建，但尚未开始的可以做执行检查
            if (this.Status != ActivityStates.Created) return true;

            if (activity.Predicate(inputs, dealer, this, context))
            {
                this.Status = ActivityStates.Dealing;
                return true;

            }
            else return false;
        }
        object Execute(IActivity activity, object inputs, IUser dealer, Context context) {
            //正在处理/已处理还未完成的活动可以被执行
            if (this.Status != ActivityStates.Dealing && this.Status != ActivityStates.Dealed) return null;

            var executeResults = activity.Execute(inputs,dealer,this,context);
            if (executeResults == null) return null;
            var jobj = JObject.FromObject(executeResults);
            foreach (var pair in jobj) {
                this.State(pair.Key,pair.Value);
            }
            this.Status = ActivityStates.Dealed;
            this.SaveChanges();
            return executeResults;
        }

        List<Action> Navigate(IActivity activity, object results, IUser dealer, Context context) {
            // 已处理，且状态有变化的才能开始寻找下一步
            if (this.Status != ActivityStates.Dealed || !this.statesChanged || results==null) return;
            if (this.Graph.Nexts == null || this.Graph.Nexts.Count == 0) return;
            var nextActions = new List<Action>();
            var nextEntities = new List<ActivityEntity>();
            var navEntities = new List<NavigationEntity>();
            foreach (var arrow in this.Graph.Nexts) {
                // 每条线路只能进入一次
                if (this.Nexts.ContainsKey(arrow.Name)) continue;
                var navResults = activity.Navigate(arrow,this,context);
                //检查没通过，不会走该线路
                if (navResults==null) continue;
                // 执行导航
                var nextAction = this.DoNavigate(arrow,navResults,context);
                

                nextEntities.Add(nextAction.entity);
                nextActions.Add(nextAction);
                navEntities.Add(new NavigationEntity()
                {
                    FromActionId = this.Id,
                    ToActionId = nextAction.Id,
                    OwnActionId = parent.Id,
                    FlowId = parent.entity.FlowId,
                    NavigationType = arrow.InstanceType,
                    NextDealerId = navResults.NextDealer?.Id,
                    NextDealerName = navResults.NextDealer?.Name,
                    NextInputs = JSON.Stringify(navResults.NextInputs),
                    CreateTime = DateTime.Now,
                    CreatorId = context.Dealer.Id,
                    CreatorName = context.Dealer.Name

                });
                
                this.AddNexts(arrow.Name, nextAction.entity.Id);
            }
            if (nextEntities.Count > 0) {
                this.Engine.FlowRepository.Inserts(nextEntities);
                this.Engine.FlowRepository.InsertNavigations(navEntities);
            }
            this.SaveChanges();
            return nextActions;
        }

        void Postdicate(IActivity activity, object results, Context context) {
            // 已处理，且状态有变化的才能执行结束检查
            if (this.Status != ActivityStates.Dealed || !this.statesChanged || results == null) return;
            if (!activity.Postdicate(results, this, context)) return;
            this.Status = ActivityStates.Done;
            if (this.Graph.Exports!=null && this.Graph.Exports.Count>0) {
                var parent = this.Parent;
                var parentStates = parent.States as JObject;
                foreach (var pair in this.Graph.Exports) {
                    var value = JSON.GetPathValue(this.states, pair.Key);
                    JSON.SetPathValue(parentStates,pair.Value,value);
                    parent.statesChanged = true;
                }
                parent.SaveChanges();
                this.DelayExecute((context)=> {
                    parent.Deal(null,context.Dealer,context);
                });
            }
            this.SaveChanges();
        }

        Action DoNavigate(Arrow arrow,NavigateResults navResults,Context context) {
            var parent = this.Parent;
            if (parent.Graph.Nodes == null) throw new InvalidGraphException("父级节点{0}的图形不正确，没有nodes", parent.Id.ToString());
            var node = parent.Graph.Nodes.FirstOrDefault(node => node.Name == arrow.To);
            if (node == null)
            {
                throw new InvalidGraphException("未在父级节点{0}找到名称为{1}的节点", parent.Id.ToString(), arrow.To);
            }
            Action nextAction = new Action(this.Engine,parent,node,null,navResults.NextInputs?? this.results,context.Dealer,navResults.NextDealer);
            parent.AddChild(nextAction);
            this.DelayExecute((context) => {
                nextAction.Deal(null, context.Dealer, context);
            });
            return nextAction;
        }

        

        #endregion

        Queue<Action<Context>> nextTicks;

        void DelayExecute(Action<Context> handler)
        {
            if (this.nextTicks == null) this.nextTicks = new Queue<Action<Context>>();
            this.nextTicks.Enqueue(handler);
        }

        void DelayExecute(Context context)
        {
            if (this.nextTicks == null) return;
            Action<Context> handler = this.nextTicks.Dequeue();
            while (this.nextTicks.Count > 0)
            {
                handler(context);

                handler = this.nextTicks.Dequeue();
            }
        }

        public override string ToString()
        {
            return this.NodePath + "#" + this.Id;
        }




        void SaveChanges() { }
    }
}
