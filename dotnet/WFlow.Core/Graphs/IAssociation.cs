using System.Collections.Generic;

namespace WFlow.Graphs
{
    public interface IAssociation:IElement
    {
        
        string To { get;  }
        string From { get; }
        IReadOnlyDictionary<string, string> NextInputsMaps { get; }

        string NextDealerIdPath { get; set; }

        string NextDealerNamePath { get; set; }
    }
}