using System;
using System.Collections.Generic;
using System.Text;

namespace WFlow
{
    public class NavigateResults
    {
        public NavigateResults(IUser nextDealer, object nextInputs) {
            this.NextInputs = nextInputs;
            this.NextDealer = nextDealer;
        }
        public object NextInputs { get; private set; }
        public IUser NextDealer { get; private set; }
    }
}
