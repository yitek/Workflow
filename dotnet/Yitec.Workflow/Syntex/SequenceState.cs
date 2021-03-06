using System;
using System.Collections.Generic;
using System.Text;

namespace Yitec.Workflow.Syntex
{
    public class SequenceState : DFState
    {
        List<DFState> _States;
        public SequenceState() {
            this._States = new List<DFState>();
        }
        public SequenceState(params DFState[] pars)
        {
            this._States = new List<DFState>(pars);
        }
        public SequenceState(DFState left,DFState right)
        {
            this._States = new List<DFState>() { left, right };
        }

        internal void AppendState(DFState state) {
            this._States.Add(state);
        }

        public override void Reset()
        {
            base.Reset();
            foreach (var state in this._States) {
                state.Reset();
            }
        }
        public override DFState Clone(bool withHandler=false)
        {
            List<DFState> states = new List<DFState>();
            foreach (var state in this._States) {
                states.Add(state.Clone(withHandler));
            }
            return new SequenceState() { _States = states};
        }

        public override MatchResults Transfer(char ch)
        {
            if (this.IsMatched == MatchResults.NotMatched || this.IsMatched == MatchResults.Matched)
            {
                throw new Exception("已经有终值了，需要reset");
            }
            var c = 0;
            foreach (var state in this._States) {
                if (state.IsMatched == MatchResults.Matched) { c++;continue; }
                var rs = state.Transfer(ch);
                if (rs == MatchResults.NotMatched)
                {
                    return this.IsMatched = MatchResults.NotMatched;
                }
                else if (rs == MatchResults.Matched) {
                    c++;
                    this.Buffer.Append(state.GetResult());
                }
            }
            if (c == this._States.Count) {
         
                return this.IsMatched = MatchResults.Matched;
            }return this.IsMatched = MatchResults.Unknown;
        }

        

    }
}
