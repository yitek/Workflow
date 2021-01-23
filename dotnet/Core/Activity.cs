using Flow.Graphs;
using Flow.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Flow
{
    public class Activity
    {
        #region 构造函数
        protected Activity() { }
        /// <summary>
        /// 新建(存储中没有的Activity)调用的构造函数
        /// 该构造函数不公开
        /// </summary>
        /// <param name="node"></param>
        /// <param name="nav"></param>
        /// <param name="version"></param>
        /// <param name="repo"></param>
        /// <param name="dealer"></param>
        /// <param name="creator"></param>
        private static void InitActivity(Activity activity , Node node,NextInfo nextInfo, string version=null, IActivityRepository repo=null,IUser dealer=null, IUser creator = null) {
            Activity parent = activity.parent =nextInfo?.FromActivity?.Parent;
            
            Guid id = Guid.NewGuid();
            activity.id = id;
            activity.repository = repo ?? parent.Repository;
            activity.entity = new ActivityEntity()
            {
                Id= id,
                NodeName = node.Name,
                Fullname = parent==null?node.Name : parent.Fullname + "/" + node.Name,
                Status = ActivityStates.Initializing,
                Version = version?? parent.Entity.Version,
                ParentId = parent?.Id,
                FlowId = parent!=null?parent.FlowId:id,
                Graph = JSON.Stringify(node),
                CreateTime = DateTime.Now,
            };
            
            
            if (dealer != null)
            {
                activity.entity.DealerId = dealer.Id;
                activity.entity.DealerName = dealer.Name;
            }
            else if(nextInfo != null && nextInfo.Dealer!=null) {
                activity.entity.DealerId = nextInfo.Dealer.Id;
                activity.entity.DealerName = nextInfo.Dealer.Name;
            }
            if (creator != null)
            {
                activity.entity.CreatorId = creator.Id;
                activity.entity.CreatorName = creator.Name;
            }
            else if (nextInfo != null && nextInfo.FromActivity != null && nextInfo.FromActivity.Dealer!=null)
            {
                activity.entity.CreatorId = nextInfo.FromActivity.Dealer.Id;
                activity.entity.CreatorName = nextInfo.FromActivity.Dealer.Name;
            }

            if (nextInfo != null)
            {
                activity.entity.AssociationName = nextInfo.Association.Name;
                activity.entity.PrevActivityId = nextInfo.FromActivity.Id;
            }

            if (activity.Graph.InParameters != null && activity.Parent != null)
            {
                var states = new Dictionary<string, string>();
                foreach (var pair in activity.Graph.InParameters)
                {
                    string value = parent[pair.Key];
                    states.Add(pair.Value, value);
                }
            }
        }
        static Activity CreateActivityInstance(ActivityEntity entity, IActivityRepository repo) {
            var activity = CreateActivityInstance(entity.InstanceType);
            activity.entity = entity;
            activity.id = entity.Id;
            activity.repository = repo;
            return activity;
        }



        static Activity CreateActivityInstance(string typename)
        {
            Activity nextActivity;
            if (!string.IsNullOrWhiteSpace(typename))
            {
                var ns = typename.Split(',');
                if (ns.Length == 2)
                {
                    nextActivity = (Activity)Activator.CreateInstance(ns[0], ns[1]).Unwrap();
                }
                else throw new NotImplementedException();
            }
            else
            {
                nextActivity = new Activity();
            }
            return nextActivity;
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

        #endregion

        #region 属性:存储与数据
        IActivityRepository repository;
        public IActivityRepository Repository {
            get {
                if (this.repository == null && this.Parent!=null) repository = this.Parent.repository;
                return this.repository;
            }
        }

        private Guid? id;

        public Guid Id {
            get {
                if (id == null) id = this.Entity.Id;
                return id.Value;
            }
            set {
                id = value;
            }
        }
        private ActivityEntity entity;
        private ActivityEntity Entity { 
            get {
                if (this.entity == null) {
                    this.entity = this.Repository.GetById(this.id.Value);
                }
                return this.entity;
            } 
        }

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
        public string Fullname { get { return this.Entity.Fullname; } }

        public string Name {
            get {
                return this.Entity.NodeName;
            }
        }

        public Guid FlowId {
            get { return this.Entity.FlowId; }
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

        Dictionary<string, string> states;
        private IDictionary<string, string> States {
            get {
                if (this.states == null) {
                    if (!string.IsNullOrWhiteSpace(this.Entity.States)) {
                        this.states = JSON.Parse<Dictionary<string, string>>(this.Entity.States);
                    }
                }
                return this.states;
            }
        }

        public string this[string key]
        {
            get
            {
                if (this.states == null) return null;

                this.states.TryGetValue(key, out string value);
                return value;
            }
            protected set
            {
                if (this.states == null)
                {
                    this.states = JSON.Parse<Dictionary<string, string>>(this.Entity.States)?? new Dictionary<string, string>();

                }
                if (this.states.ContainsKey(key)) states[key] = value;
                else this.states.Add(key, value);
                this.statesChanged = true;
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


        

        List<Activity> children;

        bool hasFullChildren;

        public IReadOnlyList<Activity> Children {
            get {
                if (!hasFullChildren) {
                    if (this.Graph.Nodes == null || this.Graph.Nodes.Count==0) {
                        hasFullChildren = true;
                        return null;
                    }
                    var s = this.Repository.ListByParentId(this.Entity.Id);
                    var children = new List<Activity>();
                    foreach (var entity in s) {
                        if (this.children != null && this.children.Any(p => p.Id == entity.Id)) continue;
                        var acti = CreateActivityInstance(entity,this.Repository);
                       
                        children.Add(acti);
                    }
                    if (this.children == null) this.children = children;
                    else this.children.AddRange(children);
                    hasFullChildren = true;
                }
                return this.children;
            }
            private set {
                this.children = value as List<Activity>;
            }
            
        }

        Activity parent;
        public Activity Parent {
            get {
                if (this.parent == null) {
                    if (this.Entity.ParentId != null) {
                        var entity = this.Repository.GetById(this.Entity.ParentId.Value);
                        var parent = CreateActivityInstance(entity.InstanceType);
                        parent.repository = this.repository;
                        parent.children = new List<Activity>() {this };
                    }
                }
                return this.parent;
            }
            private set {
                if (this.parent != null) throw new Exception("已经有了parent的活动，不可以再设Parent");
                if (this.Entity.ParentId != null && this.Entity.ParentId.Value != value.Id) {
                    throw new Exception(string.Format("设置的Parent的Id[{0}]跟Entity.ParentId[{1}]中的不一致",value.Id.ToString(),this.Entity.ParentId.Value.ToString()));
                }
                this.parent = value;
                this.Entity.ParentId = value.Id;
            }
        }

       

        
        #endregion

        


        public ActivityStates Process(IUser dealer, object inputs, FlowContext context =null)
        {
            

            #region 处理前检查，已经完成的就直接返回;
            // 处于已处理状态，直接返回
            if (this.Status ==  ActivityStates.Closed || this.Status== ActivityStates.Abort) {
                return this.Status;
            }
            
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
            ITransaction trans = context?.Transaction;
            if (trans == null) trans = context.Transaction = this.Repository.CreateTransaction();
            
            trans.TryBegin();
            IList<NextInfo> nextInfos = null;
            try {
                if (this.Status == ActivityStates.Initialized || this.Status == ActivityStates.Executed) {
                    // 开始执行节点代码
                    var results = this.Execute(dealer, inputs, this.states,context);
                    this.Entity.DealTime = DateTime.Now;
                    // 试图结束当前节点
                    nextInfos = this.Done(results,trans,context);
                }
                

                if (this.Status == ActivityStates.Done) this.StepToNexts(dealer, nextInfos, context);

                this.SaveChanges(trans);

                this.DelayExecute(dealer, inputs,  context);
                
                trans.TryCommit();
                
            } catch (Exception ex) {
                trans.TryRollback();
                throw ex;
            }
            #endregion


            return this.Status;
        }

        public virtual bool Predicate(FlowContext context = null)
        {
            return true;
        }

        #region Execute

        protected virtual IReadOnlyDictionary<string,string> Execute(IUser dealer, object inputs,IReadOnlyDictionary<string,string> states,FlowContext context) {
            if (this.Graph.Nodes == null)
            {
                var inputDic = JSON.ToDict(inputs, "nextDealerId", "nextDealerName", "isDone");
                return inputDic;
            }
            else {
                var children = this.Children;
                if (children.Count == 0)
                {
                    this.StartActivity(dealer, context);
                }
                else {
                    var actived = children.Where(p => p.Status != ActivityStates.Abort && p.Status != ActivityStates.Closed);
                    if (actived.Count() > 0)
                    {
                        foreach (var activity in actived)
                        {
                            activity.Process(dealer, inputs, context);
                        }
                    }
                    else {
                        return new Dictionary<string, string>();
                    }
                    
                }
                return null;
            }
        }

        private void StartActivity(IUser dealer,  FlowContext context)
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
            foreach (var startName in starts)
            {
                var startNode = this.Graph.Nodes.FirstOrDefault(p => p.Name == startName);
                if (startNode == null) throw new InvalidGraphException("活动{0}无法找到起始节点{1}", this.ToString(), startName);
                var startActivity = CreateActivityInstance(startNode.InstanceType);
                InitActivity(startActivity, startNode, null, this.Entity.Version, this.Repository, dealer, dealer);
                startActivity.Parent = this;
                this.children.Add(startActivity);
                this.Repository.Insert(startActivity.Entity, context.Transaction);
                nextActivities.Add(startActivity);
            }
            this.DelayExecute((dealer,inputs,context)=> {
                foreach (var nextActivity in nextActivities)
                {
                    nextActivity.Process(dealer,inputs,context);
                }
            });
            
        }
        #endregion





        IList<NextInfo> Done(IReadOnlyDictionary<string, string> results,ITransaction trans,FlowContext context) {
            //没有返回任何结果 ，不做进一步尝试
            if (results == null)
            {
                this.Status = ActivityStates.Executed;
                return null;
            }
            // 记录结果并将结果整合进states
            foreach (var pair in results)
            {
                this[pair.Key] = pair.Value;
            }
            this.Results = results;

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
        IList<NextInfo> ResolveNexts(FlowContext context)
        {
            if (this.Parent == null) return null;
            var pNode = this.Parent.Graph;
            if (pNode.Associations == null || pNode.Associations.Count == 0) return null;
            var assocsFromMe = pNode.Associations.Where(p => p.From == this.Graph.Name);
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
                        throw new InvalidGraphException("节点{0}寻找{1}关联的To节点{2}失败，未找到该节点", this.Fullname, nav.Association.Name, nav.Association.To);
                    }
                    nexts.Add(new NextInfo(nav, toNode, navResult));
                }
            }
            return nexts;
        }

        void ExportStates(ITransaction trans) {
            var p = this.Parent;
            if (p == null || this.Graph.OutParameters == null) return;
            foreach (var pair in this.Graph.OutParameters) {
                string value = this[pair.Value];
                p[pair.Key] = value;
            }
            p.SaveChanges(trans);
        }

        void StepToNexts(IUser dealer,IList<NextInfo> nextInfos,FlowContext context) {
            
            if (nextInfos.Count>0)
            {
                List<ActivityEntity> nextEntities = new List<ActivityEntity>();
                foreach (var nextInfo in nextInfos)
                {
                    var nextActivity = CreateActivityInstance(nextInfo.ToNode.InstanceType);
                    InitActivity(nextActivity, nextInfo.ToNode, nextInfo);
                    nextInfo.Activity = nextActivity;
                    nextEntities.Add(nextActivity.Entity);
                }
                this.Repository.Inserts(nextEntities,context.Transaction);

                this.DelayExecute((dealer,inputs,context)=> {
                    foreach (var nextInfo in nextInfos)
                    {
                        nextInfo.Activity.Process(dealer, null, context);
                    }
                });
                this.Status = ActivityStates.Closed;

                
            }
            else {
                // 没有下一步了
                if (this.Parent != null) {
                    this.DelayExecute((dealer,inputs,context)=> {
                        this.Parent.Process(dealer, null, context);
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
                        this.Repository.SaveResults(this.Entity,trans);
                    }
                    else {
                        this.Repository.SaveStatusAndStates(this.Entity,trans);
                    }
                } else {
                    this.Repository.SaveStates(this.Entity,trans);
                }
            } else {
                if (this.statusChanged)
                {
                    if (this.results != null) {
                        this.Entity.Results = JSON.Stringify(this.results);
                        this.Repository.SaveStatusAndResults(this.Entity,trans);
                    } else {
                        this.Repository.SaveStatus(this.Entity, trans);
                    }
                }
            }
        }

        Queue<Action<IUser, object, FlowContext>> nextTicks;

        void DelayExecute(Action<IUser, object, FlowContext> handler) {
            if (this.nextTicks == null) this.nextTicks = new Queue<Action<IUser, object, FlowContext>>();
            this.nextTicks.Enqueue(handler);
        }

        void DelayExecute(IUser dealer, object inputs, FlowContext context)
        {
            if (this.nextTicks == null) return;
            Action<IUser, object, FlowContext> handler = this.nextTicks.Dequeue();
            while (handler != null) {
                handler(dealer,inputs,context);
                handler = this.nextTicks.Dequeue();
            }
        }

        public override string ToString()
        {
            return this.Fullname + "#" + this.Id;
        }


        

        public static Activity StartFlow(string fullname,string version,IUser dealer,object inputs ,FlowContext context) {
            var node = FlowFactory.Fetch(fullname,version);

            var repo = context.Repository;
            var activity = CreateActivityInstance(node.InstanceType);
            InitActivity(activity, node, null,version, context.Repository,null,dealer);
            activity.entity.FlowId = activity.Id;
            
            repo.Insert(activity.entity);
            activity.Process(dealer,inputs,context);
            return activity;
        }

        public static Activity Deal(Guid id, IUser dealer, object inputs, FlowContext context) {
            var repo = context.Repository;
            var entity = repo.GetById(id);
            var activity = CreateActivityInstance(entity,repo);
            activity.Process(dealer, inputs, context);
            return activity;

        }
        public static IFlowFactory FlowFactory = Flow.FlowFactory.Default;

    }
}
