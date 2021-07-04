using System;
using System.Collections.Generic;
using System.Text;

namespace Workflow.Domains.Predicates
{
    public class StatusPredicate:Predicate
    {
        public string StateName { get; private set; }
    }
}
