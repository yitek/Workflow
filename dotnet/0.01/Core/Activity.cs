using WFlow.Graphs;
using WFlow.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace WFlow
{
    public class Activity : IReadOnlyState
    {
        

        #region 构造函数

        

        static Activity CreateActivityInstance(string typename, Engine engine)
        {
            Activity creatingAcitivity;
            if (!string.IsNullOrWhiteSpace(typename))
            {
                var ns = typename.Split(',');
                if (ns.Length == 2)
                {
                    creatingAcitivity = (Activity)Activator.CreateInstance(ns[0], ns[1]).Unwrap();
                }
                else throw new NotImplementedException();
            }
            else
            {
                creatingAcitivity = new Activity();
            }
            creatingAcitivity.Engine = engine;
            return creatingAcitivity;
        }


        static Activity CreateActivityInstance(ActivityEntity entity,Engine engine)
        {
            var creatingActivity = CreateActivityInstance(entity.InstanceType ?? engine.DefaultActivityInstanceType, engine);
            creatingActivity.Entity = entity;
            return creatingActivity;
        }
        static Activity CreateActivityInstance(ActivityEntity entity, Activity parent) {
            var creatingActivity = CreateActivityInstance(entity,parent.Engine);
            creatingActivity.parent = parent;
            return creatingActivity;
            
        }
        static Activity CreateActivityInstance(Node node, string version, IUser creator, Engine engine)
        {
            Guid id = Guid.NewGuid();

            var entity = new ActivityEntity()
            {
                Id = id,
                NodeName = node.Name,
                Status = ActivityStates.Initializing,
                Version = version,
                Domain = engine.Name,
                InstanceType = node.InstanceType ?? engine.DefaultActivityInstanceType,
                Graph = JSON.Stringify(node),
                HasChildren = node.Nodes!=null && node.Nodes.Count>0,
                CreateTime = DateTime.Now,
                CreatorId = creator.Id,
                CreatorName = creator.Name
            };
            return CreateActivityInstance(entity, engine);
        }
        static Activity CreateActivityInstance(Node node,IUser creator, Activity parent)
        {
            var activity = CreateActivityInstance(node,parent.Version,creator,parent.Engine);
            activity.Entity.NodePath = parent.NodePath + "/" + node.Name;
            activity.parent = parent;
            activity.Entity.ParentId = parent.Id;
            activity.flow = parent.flow;
            activity.Entity.FlowId = parent.FlowId;
            return activity;
        }
        static Activity CreateActivityInstance(NextInfo nextInfo) {
            var activity = CreateActivityInstance(nextInfo.ToNode,nextInfo.FromActivity.Dealer ,nextInfo.FromActivity.Parent);
            if (nextInfo.NextDealer!=null) {
                activity.Entity.DealerId = nextInfo.NextDealer.Id;
                activity.Entity.DealerName = nextInfo.NextDealer.Name;
            }
            
            return activity;
        }

        static Navigation CreateNavigationInstance(string typename)
        {
            Navigation nav;
            if (!string.IsNullOrWhiteSpace(typename))
            {
                var ns = typename.Split(',');
                if (ns.Length == 2)
                {
                    nav = (Navigation)Activator.CreateInstance(ns[0], ns[1]).Unwrap();
                }
                else throw new NotImplementedException();
            }
            else
            {
                nav = new Navigation();
            }
            return nav;
        }

        static void ImportStates(Activity activity,object inputs,Context context) {
            var node = activity.Graph;
            activity.Entity.Inputs = JSON.Stringify(inputs);
            if (node.Imports != null && node.Imports.Count > 0 && context.Inputs != null)
            {
                var inputDic = JSON.ToDict(inputs);
                if (inputDic != null && inputDic.Count > 0)
                {
                    var states = new Dictionary<string, string>();
                    foreach (var pair in node.Imports)
                    {
                        inputDic.TryGetValue(pair.Key, out string stat);
                        states.Add(pair.Value, stat);
                    }
                    activity.Entity.States = JSON.Stringify(states);
                }
            }
        }

        #endregion

        #region 属性:存储与流程图数据
       

        public Guid Id {
            get {
                return this.Entity.Id;
            }
        }
        
        internal ActivityEntity Entity { get; private set; }

        Node graph;
        private Node Graph
        {
            get
            {
                if (this.graph == null)
                {
                    this.graph = JSON.Parse<Node>(this.Entity.Graph);
                }
                return graph;
            }
        }

        public string Version
        {
            get { return this.Entity.Version; }
        }
        public string NodePath { get { return this.Entity.NodePath; } }

        public string Name {
            get {
                return this.Entity.NodeName;
            }
        }
        

       


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

        public DateTime CreateTime { get { return this.Entity.CreateTime; } }

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

        public DateTime? ExecuteTime { get { return this.Entity.DealTime; }private set { this.Entity.DealTime = value; } }

        public DateTime? DoneTime { get { return this.Entity.DoneTime; } private set { this.Entity.DoneTime = value; } }
        #endregion

        #region 属性:状态

        private bool statesChanged;

        JObject states;
        public object States {
            get {
                if (this.states == null) {
                    if (!string.IsNullOrWhiteSpace(this.Entity.States)) {
                        this.states = JObject.Parse(this.Entity.States);
                    }
                }
                return this.states;
            }
        }

        public T State<T>(string key) {
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
        public Activity State(string key, object value) {
            if (this.states == null)
            {
                if (!string.IsNullOrWhiteSpace(this.Entity.States))
                {
                    this.states = JObject.Parse(this.Entity.States);
                }
                if (this.states == null) this.states = new JObject();
            }
            this.states[key] = JToken.FromObject(value);
            this.statesChanged = true;
            return this;
        }

        public string this[string key]
        {
            get
            {
                if (this.states == null) return null;

                this.states.TryGetValue(key, out JToken value);
                return value==null?null:value.ToString();
            }
            protected set
            {
                this.State(key, value);
            }
        }
        bool statusChanged;
        public ActivityStates Status {
            get { return this.Entity.Status; }
            private set { this.Entity.Status = value; this.statusChanged = true; }
        }

        IReadOnlyDictionary<string, string> results;
        
        public IReadOnlyDictionary<string, string> Results {
            get {
                if (this.results == null && !string.IsNullOrWhiteSpace(this.Entity.Results)) {
                    this.results = JSON.Parse<Dictionary<string, string>>(this.Entity.Results);
                }
                return this.results;
            }
            private set{
                this.results = value;
            }
        }
        #endregion

        #region 结构

        public string Domain { get { return this.Entity.Domain; } }

        public Engine Engine { get; private set; }
        public Guid FlowId
        {
            get { return this.Entity.FlowId; }
        }

        Activity flow;
        
        public Activity Flow {
            get {
                if (this.flow == null) {
                    if (this.Entity.FlowId == this.Id)
                    {
                        return this.flow = this;
                    }
                    if (this.parent != null) return this.flow = this.parent.Flow;
                    var flowEntity = this.Engine.FlowRepository.GetById(this.Entity.FlowId);
                    this.flow = CreateActivityInstance(flowEntity,this.Engine);
                    
                }
                return this.flow;
            }
            
        }

        Activity parent;

        public Activity Parent
        {
            get
            {
                if (this.parent == null)
                {
                    if (this.Entity.ParentId != null)
                    {
                        
                        var parentEntity = this.Engine.FlowRepository.GetById(this.Entity.ParentId.Value);
                        this.parent = CreateActivityInstance(parentEntity,this.Engine);

                    }
                }
                return this.parent;
            }

        }

        public bool HasChildren {
            get { return this.Entity.HasChildren; }
        }

        List<Activity> children;

        bool hasFullChildren;

        public IReadOnlyList<Activity> Children
        {
            get
            {
                if (!hasFullChildren)
                {
                    if (this.Graph.Nodes == null || this.Graph.Nodes.Count == 0)
                    {
                        hasFullChildren = true;
                        return null;
                    }
                    var s = this.Engine.FlowRepository.ListByParentId(this.Entity.Id);
                    var children = new List<Activity>();
                    foreach (var entity in s)
                    {
                        if (this.children != null && this.children.Any(p => p.Id == entity.Id)) continue;
                        var acti = CreateActivityInstance(entity,this);
                        acti.Entity = entity;
                        acti.parent = this;
                        children.Add(acti);
                    }
                    if (this.children == null) this.children = children;
                    else this.children.AddRange(children);
                    hasFullChildren = true;
                }
                return this.children;
            }
            private set
            {
                this.children = value as List<Activity>;
            }

        }



        #endregion

        public ActivityStates Process(object parameterObj,IUser dealer, Context context) {
            if (WFlow.Engine.Development) {
                var status = this.InternalProcess(parameterObj,dealer, context);
                this.DelayExecute(context);
                return status;
            }

            if (context.Transaction == null) context.Transaction = context.Engine.FlowRepository.CreateTransaction();
            try {
                var status = this.InternalProcess(parameterObj,dealer,context);
                context.Transaction.TryCommit();
                this.DelayExecute(context);
                return status;
            } catch (Exception ex) {
                context.Transaction.TryRollback();
                throw ex;
            }
        }



        public ActivityStates InternalProcess( object parameterObj, IUser dealer, Context context)
        {
            #region 处理前检查，已经完成的就直接返回;
            // 处于已处理状态，直接返回
            if (this.Status ==  ActivityStates.Completed || this.Status== ActivityStates.Canceled) {
                return this.Status;
            }
            //指定了处理人，但当前处理人不匹配，什么都不做
            if (this.Dealer != null && this.Dealer.Id != dealer.Id)
            {
                return this.Status;
            }
            #endregion 

            #region predicate阶段
            if (this.Status == ActivityStates.Initializing) {
                if (this.Predicate(context)) {
                    //执行前检查通过，导入状态
                    this.Status = ActivityStates.Initialized;
                    
                }
                else return this.Status;
            }

            #endregion


            #region execute
            
            IList<NextInfo> nextInfos = null;
            object changedStatesObj=null;

            if (this.Status == ActivityStates.Initialized || this.Status == ActivityStates.Dealed)
            {
                // 开始执行节点代码
                changedStatesObj = this.Execute(dealer, parameterObj, this, context);
                this.Entity.DealTime = DateTime.Now;
                if (changedStatesObj != null) {
                    
                }
                this.Status = ActivityStates.Dealed;
            }

            if (changedStatesObj != null && this.statesChanged) { 
                
            }

            


            if (this.Status == ActivityStates.Done) this.StepToNexts(nextInfos, context);

            this.SaveChanges(trans);

            

            #endregion


            return this.Status;
        }

        public virtual bool Predicate(Context context = null)
        {
            return true;
        }

        #region Execute

        protected virtual object Execute(IUser dealer, object parameterObj,IReadOnlyState states,Context context) {
            if (!this.hasFullChildren)
            {
                var inputDic = JSON.ToDict(inputs, "nextDealerId", "nextDealerName", "isDone");
                return inputDic;
            }
            else {
                var children = this.Children;
                if (children.Count == 0)
                {
                    this.StartChildActivity(context);
                }
                else {
                    var actived = children.Where(p => p.Status != ActivityStates.Canceled && p.Status != ActivityStates.Completed);
                    if (actived.Count() > 0)
                    {
                        foreach (var activity in actived)
                        {
                            activity.Process(context);
                        }
                    }
                    else {
                        return new Dictionary<string, string>();
                    }
                    
                }
                return null;
            }
        }
        public bool Postdicate() { return true; }

        private void StartChildActivity( Context context)
        {
            string[] starts = this.Graph.Starts;
            if (starts == null || starts.Length == 0)
            {
                if (this.Graph.Start != null) starts = new string[] { this.Graph.Start };
            }
            if (starts == null || starts.Length == 0)
            {
                throw new InvalidGraphException("活动{0}配置了nodes，却没有配置start/starts", this.ToString());
            }
            var nextActivities = new List<Activity>();
            var nextEntities = new List<ActivityEntity>();
            foreach (var startName in starts)
            {
                var startNode = this.Graph.Nodes.FirstOrDefault(p => p.Name == startName);
                if (startNode == null) throw new InvalidGraphException("活动{0}无法找到起始节点{1}", this.ToString(), startName);
                var startActivity = CreateActivityInstance(startNode,this.Dealer,this);
                this.children.Add(startActivity);
                nextEntities.Add(startActivity.Entity);
                nextActivities.Add(startActivity);
            }
            this.Engine.FlowRepository.Inserts(nextEntities,context.Transaction);
            this.DelayExecute((context)=> {
                foreach (var nextActivity in nextActivities)
                {
                    nextActivity.Process(context);
                }
            });
            
        }

        
        #endregion





        void Done(object executeResults,ITransaction trans,Context context) {
            //没有返回任何结果 ，不做进一步尝试
            if (executeResults == null) return;
           
            var changedStates = JObject.FromObject(executeResults);
            foreach (var pair in changedStates)
            {
                if (this.states == null)
                {
                    if (!string.IsNullOrWhiteSpace(this.Entity.States))
                    {
                        this.states = JObject.Parse(this.Entity.States);
                    }

                }
                if (this.states == null) this.states = new JObject();
                this.states[pair.Key] = pair.Value;
                this.statesChanged = true;
            }

            // 状态转换后，找到下一步的连线
            var navs = this.ResolveNexts(context);
            if (navs == null)
            {
                // 图中就没有下一步
                this.DoneTime = DateTime.Now;
                this.ExportStates(trans);
                // 指示后面执行
                return new List<NextInfo>();
            }
            else if (navs.Count == 0)
            {
                // 当前状态不足以找到nexts，
                // 节点处于中间状态
                // 返回null，指示deal函数，不做StepToNexts操作
                return null;
            }
            else
            {
                
                this.DoneTime = DateTime.Now;
                this.ExportStates(trans);
                return navs;
                
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns>null 表示图中没有下一步</returns>
        IList<NextInfo> ResolveNexts(Context context)
        {
            // 该Activity是一个顶层的Flow,没有下一步
            if (this.Parent == null) return null;
            var pNode = this.Flow.Graph;
            if (pNode.Associations == null || pNode.Associations.Count == 0) return null;
            var assocsFromMe = pNode.Associations.Where(p => p.From == this.Name);
            // 没有找到下一步，不执行stepToNexts
            if (assocsFromMe.Count() == 0) return null;
            var nexts = new List<NextInfo>();
            foreach (var assoc in assocsFromMe)
            {
                var nav = CreateNavigationInstance(assoc.InstanceType);
                var navResult = nav.Navigate(this.states, context);
                if (navResult != null)
                {
                    var toNode = pNode.Nodes.FirstOrDefault(p => p.Name == assoc.To);
                    if (toNode == null)
                    {
                        throw new InvalidGraphException("节点{0}寻找{1}关联的To节点{2}失败，未找到该节点", this.NodePath, nav.Association.Name, nav.Association.To);
                    }
                    nexts.Add(new NextInfo(nav, toNode, navResult));
                }
            }
            return nexts;
        }

        void ExportStates(ITransaction trans) {
            var p = this.Flow;
            if (p == null || this.Graph.Exports == null) return;
            foreach (var pair in this.Graph.Exports) {
                string value = this[pair.Value];
                p[pair.Key] = value;
            }
            p.SaveChanges(trans);
        }

        void StepToNexts(IList<NextInfo> nextInfos,Context context) {
            
            if (nextInfos.Count>0)
            {
                List<ActivityEntity> nextEntities = new List<ActivityEntity>();
                foreach (var nextInfo in nextInfos)
                {
                    var nextActivity = CreateActivityInstance(nextInfo);
                    
                    nextInfo.NextActivity = nextActivity;
                    nextEntities.Add(nextActivity.Entity);
                }
                this.Engine.FlowRepository.Inserts(nextEntities,context.Transaction);

                this.DelayExecute((context)=> {
                    foreach (var nextInfo in nextInfos)
                    {
                        nextInfo.NextActivity.Process(context);
                    }
                });
                this.Status = ActivityStates.Completed;

                
            }
            else {
                // 没有下一步了
                if (this.Flow != null) {
                    this.DelayExecute((context)=> {
                        this.Flow.Process(context);
                    });
                }
            }
        }

        void SaveChanges(ITransaction trans) {
            if (this.statesChanged) {
                this.Entity.States = JSON.Stringify(this.States);
                if (this.statusChanged) {
                    if (this.Results != null)
                    {
                        this.Entity.Results = JSON.Stringify(this.Results);
                        this.Engine.FlowRepository.SaveResults(this.Entity,trans);
                    }
                    else {
                        this.Engine.FlowRepository.SaveStatusAndStates(this.Entity,trans);
                    }
                } else {
                    this.Engine.FlowRepository.SaveStates(this.Entity,trans);
                }
            } else {
                if (this.statusChanged)
                {
                    if (this.results != null) {
                        this.Entity.Results = JSON.Stringify(this.results);
                        this.Engine.FlowRepository.SaveStatusAndResults(this.Entity,trans);
                    } else {
                        this.Engine.FlowRepository.SaveStatus(this.Entity, trans);
                    }
                }
            }
        }

        Queue<Action<Context>> nextTicks;

        void DelayExecute(Action<Context> handler) {
            if (this.nextTicks == null) this.nextTicks = new Queue<Action<Context>>();
            this.nextTicks.Enqueue(handler);
        }

        void DelayExecute(Context context)
        {
            if (this.nextTicks == null) return;
            Action< Context> handler = this.nextTicks.Dequeue();
            while (this.nextTicks.Count>0) {
                handler(context);
               
                handler = this.nextTicks.Dequeue();
            }
        }

        public override string ToString()
        {
            return this.NodePath + "#" + this.Id;
        }


        

        

        internal static Activity InternalStartFlow(string fullname, Context context)
        {
            if (string.IsNullOrWhiteSpace(context.Version)) context.Version = context.Engine.DefaultVersion;
            var node = context.Engine.FetchGraph(fullname, context.Version);
            var repo = context.Engine.FlowRepository;
            var activity = context.Activity = CreateActivityInstance(node,context.Version,context.Dealer,context.Engine);
            activity.Entity.FlowId = context.Activity.Id;
            
            activity.flow = activity;

            ImportStates(activity,context.Inputs,context);
            repo.Insert(context.Activity.Entity);
            context.Activity.Process(context);
            return context.Activity;
        }

        
        internal static Activity InternalDealActivity(Guid id,Context context) {
            
            var repo = context.Engine.FlowRepository;
            var entity = repo.GetWithParentsById(id);
            var current = context.Activity = CreateActivityInstance(entity,context.Engine);
            if (entity.ParentActivity != null) {
                current.parent = CreateActivityInstance(entity.ParentActivity,context.Engine);
                current.parent.children = new List<Activity>() { current };
            }
            if(entity.FlowId != entity.Id){
                if (entity.ParentId != null && entity.ParentId.Value == entity.FlowId)
                {
                    current.flow = current.parent;
                }
                else {
                    current.flow = CreateActivityInstance(entity.FlowActivity,context.Engine);
                }
            } 
            context.Activity.Process( context);
            return context.Activity;
        }
        

    }
}
