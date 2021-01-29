using WFlow.Repositories;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using WFlow.Graphs;

namespace WFlow
{
    public class Engine
    {
        readonly static ConcurrentDictionary<string, Engine> flows = new ConcurrentDictionary<string, Engine>();
        public Engine(string name,IFlowRepository FlowRepository) {
            this.FlowRepository = FlowRepository;
            this.Name = name;
        }
        public static Engine GetOrAdd(string name, Func<string, Engine> creator = null) {
            return flows.GetOrAdd(name, creator);
        }

        public IFlowRepository FlowRepository { get; set; }

        IGraphRepository graphRepository;
        public IGraphRepository GraphRepository
        {
            get
            {
                if (this.graphRepository == null)
                {
                    lock (this) {
                        if (this.graphRepository == null) {
                            var baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wf_defs");
                            if (!string.IsNullOrWhiteSpace(this.Name))
                            {
                                baseDir = Path.Combine(baseDir, this.Name);
                            }
                            this.graphRepository = new GraphRepository(baseDir);
                        }
                    }
                   
                }
                return this.graphRepository;
            }
            set { lock(this)this.graphRepository = value; }
        }

        public string Name { get; set; }

        string defaultVersion;
        public string DefaultVersion { get { return defaultVersion ?? "default"; } set { defaultVersion = value; } }

        string flowInstanceType;
        public string DefaultFlowInstanceType { get { return flowInstanceType ?? "WFlow.Flow"; } set { flowInstanceType = value; } }

        string defaultActivityInstanceType;
        public string DefaultActivityInstanceType { get { return defaultActivityInstanceType ?? "WF.Activity"; } set { defaultActivityInstanceType = value; } }

        string defaultNavigationInstanceType;
        public string DefaultNavigationInstanceType { get { return defaultNavigationInstanceType ?? "WF.Navigation"; } set { defaultNavigationInstanceType = value; } }



        IErrorLogger errorLogger;
        public IErrorLogger ErrorLogger { 
            get {
                if (errorLogger == null) {
                    lock (this) {
                        if (this.errorLogger == null) this.errorLogger = WFlow.ErrorLogger.Default;
                    }
                }
                return this.errorLogger;
            }
            set {
                lock (this) this.errorLogger = value;
            }
        }

        public Node FetchGraph(string name, string version = null) {
            var graph = this.GraphRepository.Fetch(name,version??this.defaultVersion);
            if (string.IsNullOrWhiteSpace(graph.InstanceType)) graph.InstanceType = this.DefaultFlowInstanceType;
            return graph;
        }


        public Activity StartFlow(string activitname, IUser dealer, object inputs, Context context = null)
        {
            if (context == null) context = new Context();
            context.Engine = this;
            context.Dealer = dealer;
            context.Inputs = inputs;

            if (Engine.Development) return Activity.InternalStartFlow(activitname, context);
            try
            {
                return Activity.InternalStartFlow(activitname, context);
            }
            catch (Exception ex)
            {
                context.Engine.ErrorLogger.Log(this, context.Activity, context.Activity?.Entity, ex);
                throw ex;
            }
        }

        public Activity DealActivity(Guid id, IUser dealer, object inputs, Context context = null)
        {
            if (context == null) context = new Context();
            context.Engine = this;
            context.Dealer = dealer;
            context.Inputs = inputs;
            if(Engine.Development) return Activity.InternalDealActivity(id, context);
            try {
                return Activity.InternalDealActivity(id, context);
            } catch (Exception ex) {
                this.ErrorLogger.Log(this, context.Activity, context.Activity?.Entity, ex);
                throw ex;
            }
        }

        public static bool Development = false;
    }
}
