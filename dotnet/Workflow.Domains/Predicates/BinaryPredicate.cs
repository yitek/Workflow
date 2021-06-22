using System;
using System.Collections.Generic;
using System.Text;

namespace Workflow.Domains.Predicates
{
    public class BinaryPredicate:Predicate
    {
        public Predicate Left { get; private set; }
        public Predicate Right { get; private set; }
    }
}
