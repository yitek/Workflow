using System.Collections.Generic;

namespace WFlow.Graphs
{
    public interface INode:IElement
    {
        
        
        IReadOnlyList<IAssociation> Nexts { get;  }
        IReadOnlyList<INode> Nodes { get;  }
        IReadOnlyDictionary<string, string> Imports { get; }
        IReadOnlyDictionary<string, string> Exports { get; }
        string Start { get;  }
        string[] Starts { get; }
    }
}