using System;
using System.Collections.Generic;
using System.Text;

namespace Workflow.Domains
{
    /// <summary>
    /// 状态对象
    /// 表示一个特定的状态
    /// </summary>
    public class State
    {
        public string Name { get; set; }
        public State SuperState { get; set; }
        /// <summary>
        /// 状态迁移对象，从该对象出发的所有迁移对象
        /// </summary>
        public IEnumerable<Transiction> Transactions { get; private set; }

        IReadOnlyDictionary<string,string> Imports { get; set; }
        IReadOnlyDictionary<string, string> Exports { get; set; }


    }
}