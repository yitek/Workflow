using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WFlow.Entities;
using WFlow.Graphs;
using WFlow.Utils;

namespace WFlow
{
    public class Action :Navigator, IAction
    {
        public virtual IUser Predicate(IActivity activity, object inputs,  ProcessContext processContext)
        {
            return activity.Owner ?? User.Anyone;
        }

        public virtual async Task<IUser> PredicateAsync(IActivity activity, object inputs,  ProcessContext processContext)
        {
            return activity.Owner ?? User.Anyone;
        }

        public virtual object Execute(IActivity activity, object inputs,  ProcessContext processContext)
        {
            
            
            string[] starts = activity.Graph.Starts;
            if (starts == null || starts.Length == 0)
            {
                if (activity.Graph.Start != null) starts = new string[] { activity.Graph.Start };
            }
            var nodes = activity.Graph.Nodes;
            if (starts == null || starts.Length == 0)
            {
                if (nodes != null && nodes.Count != 0) throw new InvalidGraphException("活动{0}未配置start/starts,却没有配置nodes", this.ToString());
            }

            if (nodes == null && nodes.Count == 0) throw new InvalidGraphException("活动{0}配置了start/starts,却没有配置nodes", this.ToString());
            var nextEntities = new List<ActivityEntity>();
            foreach (var startName in starts)
            {
                var startNode = activity.Graph.Nodes.FirstOrDefault(p => p.Name == startName);
                if (startNode == null) throw new InvalidGraphException("活动{0}无法找到起始节点{1}", this.ToString(), startName);
                IUser startOwner=null;
                if (processContext.StartOwners != null) {
                    processContext.StartOwners.TryGetValue(startName,out startOwner);
                }
                if (startOwner == null) startOwner = processContext.StartOwner;
                if (startOwner == null) throw new ArgumentException("未指定开始节点的所有者，请给owner/owners参数赋值");
                var startActivity = new Activity(processContext.Engine,activity as Activity,startNode,activity.Version,inputs,processContext.Dealer,startOwner,processContext);
                
                nextEntities.Add(startActivity.Entity);
                processContext.Todo(startActivity,inputs);
            }
            processContext.FlowRepository.InsertActivities(nextEntities, processContext.FlowTransaction);
            
            return null;
        }

        public virtual Task<object> ExecuteAsync(IActivity activity, object inputs,  ProcessContext processContext)
        {
            throw new NotImplementedException();
        }

        

        public virtual bool Postdicate(IActivity activity, ProcessContext processContext)
        {
            var nodes = activity.Graph.Nodes;
            //如果是活动容器，要首先检查是否还有活着的子活动
            if (nodes != null 
                && nodes.Count != 0 
                && activity.Children.Any(p => !p.IsFulfilled)) return false;

            if (!string.IsNullOrEmpty(activity.Graph.Key)) {
                var value = activity.State<string>(activity.Graph.Key);
                return value == activity.Graph.Value;
            }
            return true;
        
        }

        public virtual Task<bool> PostdicateAsync(IActivity activity, ProcessContext processContext)
        {
            throw new NotImplementedException();
        }

        
    }
}
