using WFlow.Repositories;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using WFlow.Graphs;
using WFlow.Utils;

namespace WFlow
{
    public class Engine
    {
        readonly static ConcurrentDictionary<string, Engine> flows = new ConcurrentDictionary<string, Engine>();
        public Engine(string name,IFlowRepository flowRepository,IGraphRepository graphRepository=null) {
            this.FlowRepository = flowRepository;
            this.GraphRepository = graphRepository?? WFlow.Repositories.GraphRepository.Default;
            this.Name = name;
        }
        public static Engine GetOrAdd(string name, Func<string, Engine> creator = null) {
            return flows.GetOrAdd(name, creator);
        }

        public virtual IFlowRepository ResolveFlowRepository(ProcessContext processContext) {
            return this.FlowRepository;
        }

        public INavigator ResolveNavigator(Activity activity,IAssociation assoc,ProcessContext processContext)
        {
            return activity.Action;
        }



        public string Name { get; set; }

        string defaultVersion;
        public string DefaultVersion { get { return defaultVersion ?? "default"; } set { this.defaultVersion = value; } }

        
        string defaultActionType;
        public string DefaultActionType { get { return this.defaultActionType ?? "WFlow.Action"; } set { this.defaultActionType = value; } }

        string defaultNavigatorType;
        public string DefaultNavigatorType { get { return defaultNavigatorType ?? "WF.Navigation"; } set { defaultNavigatorType = value; } }

        string defaultNextDealerIdPath;
        public string DefaultNextDealerIdPath { get { return defaultNextDealerIdPath ?? "nextDealerId"; } set { defaultNextDealerIdPath = value; } }

        string defaultNextDealerNamePath;
        public string DefaultNextDealerNamePath { get { return defaultNextDealerNamePath ?? "nextDealerId"; } set { defaultNextDealerNamePath = value; } }


        IErrorLogger errorLogger;
        public IErrorLogger ErrorLogger { 
            get {
                if (errorLogger == null) {
                    lock (this) {
                        if (this.errorLogger == null) this.errorLogger = WFlow.Utils.ErrorLogger.Default;
                    }
                }
                return this.errorLogger;
            }
            set {
                lock (this) this.errorLogger = value;
            }
        }

        public IFlowRepository FlowRepository {
            get;private set;
        }

        public IGraphRepository GraphRepository { get; private set; }

        public Node FetchGraph(string name, string version = null) {
            //var graph = this.GraphRepository.Fetch(name,version??this.defaultVersion);
            //if (string.IsNullOrWhiteSpace(graph.InstanceType)) graph.InstanceType = this.DefaultFlowInstanceType;
            //return graph;
            return null;
        }


        public virtual IAction LoadAction(string actionTypeName, ProcessContext context) {
            var actionType = LoadActionType(actionTypeName);
            if (actionType == null) return null;
            return Activator.CreateInstance(actionType) as IAction;
        }
         
        Type LoadActionType(string actionTypeName,string dllPath=null) {
            var actionType = TypeResolver.Resolve(actionTypeName,dllPath);
            if (!typeof(IAction).IsAssignableFrom(actionType)) throw new Exception(actionTypeName + "不是IAction");
            return actionType;
        }




        public static bool Development = false;
    }
}
