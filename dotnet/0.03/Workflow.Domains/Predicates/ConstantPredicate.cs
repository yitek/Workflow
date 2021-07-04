using System;
using System.Collections.Generic;
using System.Text;

namespace Workflow.Domains.Predicates
{
    public class ConstantPredicate :Predicate
    {
        public string ConstantValue { get; set; }
    }
}
