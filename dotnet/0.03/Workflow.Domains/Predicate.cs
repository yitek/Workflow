using System;
using System.Collections.Generic;
using System.Text;

namespace Workflow.Domains
{
    /// <summary>
    /// 迁移判断对象
    /// </summary>
    public abstract class Predicate
    {
        public PredicateTypes Type { get; set; }
    }
}