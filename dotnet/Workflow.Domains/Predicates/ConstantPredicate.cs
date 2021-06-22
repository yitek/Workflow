using System;
using System.Collections.Generic;
using System.Text;

namespace Workflow.Domains.Predicates
{
    public class ConstantPedicate :Predicate
    {
        public string ConstantValue { get; set; }
    }
}
