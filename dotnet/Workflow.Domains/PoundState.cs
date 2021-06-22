using System;
using System.Collections.Generic;
using System.Text;

namespace Workflow.Domains
{
    /// <summary>
    /// 表示有子状态的复合状态对象
    /// </summary>
    public class PoundState : State
    {
        /// <summary>
        /// 该复合状态的所有子状态
        /// </summary>
        public IEnumerable<State> States { get; private set; }
    }
}