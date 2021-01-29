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
       

        public List<Association> Nexts { get; set; }

        IReadOnlyList<IAssociation> nexts;
        IReadOnlyList<IAssociation> INode.Nexts
        {
            get {
                if (this.nexts == null) this.nexts = this.Nexts.Cast<IAssociation>().ToList();
                return this.nexts;
            }
        }
                
        IReadOnlyList<INode> nodes;
        IReadOnlyList<INode> INode.Nodes {
            get
            {
                if (this.nodes == null) this.nodes = this.Nodes.Cast<INode>().ToList();
                return this.nodes;
            }
        }

        IReadOnlyDictionary<string, string> INode.Imports => this.Imports;

        IReadOnlyDictionary<string, string> INode.Exports => this.Exports;
    }
}
