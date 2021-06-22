using System;
using System.Collections.Generic;
using System.Text;

namespace Workflow.Domains.Predicates
{
    public class OperandPredicate:Predicate
    {
        public Predicate Operand { get; set; }

    }
}
