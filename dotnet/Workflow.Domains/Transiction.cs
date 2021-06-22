using System;
using System.Collections.Generic;
using System.Text;

namespace Workflow.Domains
{
    /// <summary>
    /// 状态迁移对象
    /// 表示状态迁移
    /// </summary>
    public class Transiction
    {
        public string Name { get; set; }
        /// <summary>
        /// 迁移发起状态
        /// </summary>
        public State From { get; set; }
        /// <summary>
        /// 迁移目标状态
        /// </summary>

        public State To { get; set; }

        public Predicate Predicate { get; set; }
    }
}