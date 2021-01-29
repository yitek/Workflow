using System.Collections.Generic;

namespace WFlow.Graphs
{
    public interface IElement
    {
        string Name { get; }
        string DisplayName { get;}
        string InstanceType { get;  }

        string Key { get; }

        string Value { get; }
        
        
    }
}