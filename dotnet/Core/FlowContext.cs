using Flow.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flow
{
    public class FlowContext
    {
        public IActivityRepository Repository { get; set; }
        public ITransaction Transaction { get; set; }

        public IErrorLogger ErrorLogger { get; set; }
    }
}
