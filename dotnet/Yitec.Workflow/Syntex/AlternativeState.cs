using System;
using System.Collections.Generic;
using System.Text;

namespace Yitec.Workflow.Syntex
{
    public class AlternativeState : DFState
    {
        List<DFState> _States;
        public AlternativeState() {
            this._States = new List<DFState>();
        }
        public AlternativeState(DFState left,DFState right)
        {
            this._States = new List<DFState>() { left, right };
        }

        internal void JoinState(DFState state) {
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
            return new AlternativeState() { _States = states};
        }

        public override MatchResults Transfer(char ch)
        {
            if (this.IsMatched == MatchResults.NotMatched || this.IsMatched == MatchResults.Matched)
            {
                throw new Exception("已经有终值了，需要reset");
            }
            foreach (var state in this._States) {
                if (state.IsMatched == MatchResults.NotMatched) continue;
                if (state.Transfer(ch) == MatchResults.Matched) {
                    this.Buffer.Append(state.GetResult());
                    return this.IsMatched = MatchResults.Matched;
                }
            }
            return this.IsMatched = MatchResults.Unknown;
        }
    }
}
