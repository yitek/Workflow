using System;
using System.Collections.Generic;
using System.Text;

namespace Workflow.Domains
{
    public enum ActivityStates
    {
        Initializing=0,
        Padding,
        Dealing,
        Dealed,
        Completed,
        Error = -1
    }
}