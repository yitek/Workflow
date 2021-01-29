using System;
using System.Collections.Generic;
using System.Linq;
using WFlow.Entities;
using WFlow.Graphs;
using WFlow.Repositories;
using WFlow.Utils;

namespace WFlow
{
    public class Navigations
    {
        public IList<ActivityEntity> ActivityEntities { get; private set; }

        public IList<NavigationEntity> NavigationEntities { get; private set; }

        public IList<Activity> ToActivities { get; private set; }

        public Activity FromActivity { get; private set; }

        public object ExecuteResults { get; private set; }

        

        public Navigations(Activity fromActivity,object executeResults) {
            this.FromActivity = fromActivity;
            this.ExecuteResults = executeResults;
            this.ToActivities = new List<Activity>();
            this.ActivityEntities = new List<ActivityEntity>();
            this.NavigationEntities = new List<NavigationEntity>();
        }

        public bool AddIfPass(IAssociation assoc,ProcessContext processConext) {
            // 该连线已经有了
            if (this.FromActivity.Nexts.ContainsKey(assoc.Name)) return false;
            // 获取到导航器
            var navigator = this.FromActivity.Engine.ResolveNavigator(this.FromActivity, assoc, processConext);
            // 用导航器得到导航结果
            NavigateResults navResults = navigator.Navigate(this.FromActivity, assoc, this.ExecuteResults, processConext);
            //检查没通过，不会走该线路
            if (navResults == null) return false;

            var nextActivity = this.ResolveNextActivity(assoc, navResults, processConext);

            this.CreateNavigation(nextActivity, assoc, navResults, processConext);

            return true;
        }

        

        Activity ResolveNextActivity(IAssociation assoc, NavigateResults navResults, ProcessContext processContext)
        {
            var parent = this.FromActivity.Parent as Activity;
            if (parent.Graph.Nodes == null) throw new InvalidGraphException("父级节点{0}的图形不正确，没有nodes", parent.Id.ToString());
            var node = parent.Graph.Nodes.FirstOrDefault(node => node.Name == assoc.To);
            if (node == null)
            {
                throw new InvalidGraphException("未在父级节点{0}找到名称为{1}的节点", parent.Id.ToString(), assoc.To);
            }

            var nextActivity = parent.FindUndealedChild(node.Name);
            if (nextActivity != null) return nextActivity;

            nextActivity = new Activity(this.FromActivity.Engine, parent, node,this.FromActivity.Version,navResults.NextInputs, processContext.Dealer,navResults.NextDealer,processContext);
            this.FromActivity.Todo(nextActivity,navResults.NextInputs);
            this.ToActivities.Add(nextActivity);
            this.ActivityEntities.Add(nextActivity.Entity);
            return nextActivity;
        }

        NavigationEntity CreateNavigation(Activity toActivity, IAssociation assoc, NavigateResults navResults,ProcessContext processContext) {
            var entity = new NavigationEntity()
            {
                FromActivityId = this.FromActivity.Id,
                ToActivityId = toActivity.Id,
                ParentActivityId = this.FromActivity.ParentId.Value,
                FlowId = toActivity.FlowId,
                NavigatorType = assoc.InstanceType,
                NextDealerId = navResults.NextDealer?.Id,
                NextDealerName = navResults.NextDealer?.Name,
                NextInputs = JSON.Stringify(navResults.NextInputs),
                CreateTime = DateTime.Now,
                CreatorId = processContext.Dealer.Id,
                CreatorName = processContext.Dealer.Name

            };
            this.NavigationEntities.Add(entity);
            this.FromActivity.AddNexts(assoc.Name,toActivity.Id);
            return entity;
        }

        public Navigations Save(ITransaction trans=null) {
            var flowRepo = this.FromActivity.FlowRepository;
            flowRepo.InsertActivities(this.ActivityEntities,trans);
            flowRepo.InsertNavigations(this.NavigationEntities,trans);
            return this;
        }

        public bool IsValid => this.ToActivities.Count > 0;
    }
}
