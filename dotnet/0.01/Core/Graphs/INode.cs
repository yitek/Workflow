using System.Collections.Generic;

namespace WFlow.Graphs
{
    public interface INode:IElement
    {
        List<Association> Associations { get; set; }
        
        IList<IArrow> Nexts { get;  }
        IList<INode> Nodes { get;  }
        IReadOnlyDictionary<string, string> Imports { get; }
        IReadOnlyDictionary<string, string> Exports { get; }
        string Start { get;  }
        string[] Starts { get; }
    }
}