using System;
using System.Collections.Generic;
using System.Text;

namespace Flow.Graphs
{
    /// <summary>
    /// 图像上面的节点
    /// </summary>
    public class Node : Element
    {
        /// <summary>
        /// 如果不是最终节点，指明
        /// 该节点的开始节点名称
        /// </summary>
        public string Start { get; set; }
        public string[] Starts { get; set; }

        /// <summary>
        /// 从父活动的状态中引入本节点的状态
        /// parent-state-key:this-state-key
        /// </summary>

        public IDictionary<string, string> InParameters { get; set; }
        /// <summary>
        /// 该活动完成后，导出到父节点的状态
        /// parent-state-key:this-state-key
        /// </summary>

        public IDictionary<string, string> OutParameters { get; set; }
        
        /// <summary>
        /// 子节点
        /// </summary>
        public IList<Node> Nodes { get; set; }
        /// <summary>
        /// 各子节点之间的线条
        /// </summary>
        public IList<Association> Associations { get; set; }
    }
}
