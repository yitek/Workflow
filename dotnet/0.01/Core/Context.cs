using WFlow.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace WFlow
{
    public class Context
    {
        public IUser Dealer { get; protected internal set; }

        public Engine Engine { get; internal set; }

        public IReadOnlyList<Activity> ActivedActivities { get; protected internal set; }

        public Activity Activity { get; protected internal set; }
        public ITransaction Transaction { get; set; }

        public string Version { get; internal set; }

        public object Inputs { get; protected internal set; }
    }
}
