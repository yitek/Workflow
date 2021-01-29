using System;
using System.Collections.Generic;
using System.Text;

namespace WFlow.Graphs
{
    /// <summary>
    /// 流程图上的线条
    /// </summary>
    public class Association: Element
    {
        /// <summary>
        /// 从哪个节点
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// 到哪个节点
        /// </summary>
        public string To { get; set; }

        public string UserIdKey { get; set; }

        public string UserNameKey { get;set; }

        
    }
}
