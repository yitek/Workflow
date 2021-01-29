using WFlow.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace WFlow
{
    public class ProcessContext
    {
        public ProcessContext(Activity activity,object inputs, IUser dealer, object userContext) {
            this.Dealer = dealer;
            this.Inputs = inputs;
            this.activity = activity;
            this.UserContext = userContext;
        }
        public IUser Dealer { get; private set; }

        Activity activity;
        public IActivity Activity => activity;

        public IFlowRepository FlowRepository => activity.FlowRepository;

        public Engine Engine => activity.Engine;

        public ITransaction FlowTransaction { get; internal set; }
        public object Inputs { get; private set; }

        public object UserContext { get; private set; }

        public IDictionary<string, IUser> StartOwners { get; set; }

        public IUser StartOwner { get; set; }

        public ProcessContext Todo(Activity activity, object inputs) {
            activity.Todo(activity,inputs);
            return this;
        }

    }
}
