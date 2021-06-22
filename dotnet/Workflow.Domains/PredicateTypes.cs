using System;
using System.Collections.Generic;
using System.Text;

namespace Workflow.Domains
{
    public enum PredicateTypes
    {
        Constant,
        Status,
        Not,
        Exists,
        Equal,
        GreatThan,
        GreatThanOrEqual,
        LessThan,
        LessThanOrEqual,
        AndAlso,
        OrElse
    }
}