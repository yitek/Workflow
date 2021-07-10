using System;
using System.Collections.Generic;
using System.Text;

namespace Yitec.Workflow.Syntex
{
    public class YMLSyntex
    {
        void onLevel(DFState state) { }
        void onKey(DFState state) { }

        void onItem(DFState state) { 
        }
        void onLine(DFState state) { 
        }
        public DFState BuildStates() {
            var levelState = new CharsetState("\t").Limits(0).OnStateChange(this.onLevel);
            var keyState = new CharsetState(":",true).Limits(0).OnStateChange(this.onKey);
            var itemState = new CharsetState("\n,",true).Limits(0).OnStateChange(this.onItem);
            var moreItemState = new SequenceState(",",itemState.Clone(true)).Limits(0);
            var valueState = new SequenceState(itemState, moreItemState);
            var lnState = new CharsetState("\n").OnStateChange(this.onLine);
            var pairState = new SequenceState(levelState,keyState,valueState,lnState);
            var commentState = new SequenceState(new CharsetState(" \t").Limits(0),"#", new CharsetState("\n",true).Limits(0),"\n");
            var lineState = new AlternativeState(pairState,commentState);
            return lineState.Limits(0);
            
        }
    }
}
