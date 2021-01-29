using System;
using System.Collections.Generic;
using System.Linq;

namespace WFlow.Graphs
{
    /// <summary>
    /// 图像上面的节点
    /// </summary>
    public class Node : Element, INode
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

        public Dictionary<string, string> Imports { get; set; }
        /// <summary>
        /// 该活动完成后，导出到父节点的状态
        /// parent-state-key:this-state-key
        /// </summary>

        public Dictionary<string, string> Exports { get; set; }



        /// <summary>
        /// 子节点
        /// </summary>
        public List<Node> Nodes { get; set; }
        /// <summary>
        /// 各子节点之间的线条
        /// </summary>
        public List<Association> Associations { get; set; }

        public List<Arrow> Nexts { get; set; }

        public static List<Node> FindStarts(Node node)
        {
            if (node.Nodes == null || node.Nodes.Count == 0) return null;
            if (node.Associations == null || node.Associations.Count == 0) return node.Nodes;
            var tos = node.Associations.Select(assoc => assoc.To);
            return node.Nodes.Where(node => !tos.Contains(node.Name)).ToList();
        }
    }
}
