using System;
using System.Collections.Generic;
using System.Linq;

namespace WFlow.Graphs
{
    public class Association : Element, IAssociation
    {
        public string To { get; set; }
        public string From { get; set; }
        public Dictionary<string, string> NextInputsMaps { get; set; }

        public string NextDealerIdPath{get;set;}

        public string NextDealerNamePath { get; set; }

        IReadOnlyDictionary<string, string> IAssociation.NextInputsMaps => this.NextInputsMaps;
    }
}
