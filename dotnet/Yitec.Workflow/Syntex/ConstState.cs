using System;
using System.Collections.Generic;
using System.Text;

namespace Yitec.Workflow.Syntex
{
    public class ConstState : DFState
    {
       

        public string Chars { get; private set; }

        public ConstState(string chars, bool isNegotive = false)
        {
            this.Chars = chars; 
        }

        private int _at;

        public override void Reset()
        {
            base.Reset();
            this._at = 0;
        }

        public override DFState Clone(bool withHandler = false)
        {
            return new ConstState(this.Chars) { _OnResult = withHandler ? this._OnResult : null };
        }

        public override MatchResults Transfer(char ch)
        {
            if (this.IsMatched == MatchResults.NotMatched || this.IsMatched == MatchResults.Matched) {
                throw new Exception("已经有终值了，需要reset");
            }
            if (this.Chars[this._at++] != ch) {
                return this.IsMatched = MatchResults.NotMatched;
            }
            if (this._at == this.Chars.Length) {
                this.Buffer.Append(this.Chars);
                return this.IsMatched = MatchResults.Matched;
            }
            return this.IsMatched = MatchResults.Unknown;
        }
    }
}
