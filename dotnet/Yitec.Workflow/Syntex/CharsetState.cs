using System;
using System.Collections.Generic;
using System.Text;

namespace Yitec.Workflow.Syntex
{
    public class CharsetState:DFState
    {
        public char MinChar { get; private set; }
        public char MaxChar { get; private set; }

        public string Chars { get; private set; }

        public bool IsNegotive { get; private set; }

        public CharsetState(string chars,bool isNegotive=false) {
            this.Chars = chars;this.IsNegotive = IsNegotive;
        }

        public CharsetState(char minChar,char maxChar, bool isNegotive = false)
        {
            if (maxChar < minChar) throw new Exception("minChar必须比maxChar小");
            this.MinChar = minChar;
            this.MaxChar = maxChar;
            this.IsNegotive = IsNegotive;
        }

        public override void Reset()
        {
            base.Reset();
        }

        public override DFState Clone(bool withHandler=false)
        {
            if (this.Chars != null) return new CharsetState(this.Chars, this.IsNegotive) { _OnResult=withHandler?this._OnResult:null };
            else return new CharsetState(this.MinChar,this.MaxChar,this.IsNegotive) { _OnResult = withHandler ? this._OnResult : null };
        }

        public override MatchResults Transfer(char ch)
        {
            if (this.IsMatched == MatchResults.NotMatched || this.IsMatched == MatchResults.Matched)
            {
                throw new Exception("已经有终值了，需要reset");
            }
            bool matched = false;
            if (this.Chars == null)
            {
                matched = ch >= this.MinChar && ch <= this.MaxChar;
            }
            else matched = this.Chars.Contains(ch);

            if (this.IsNegotive ? !matched : matched)
            {
                this.Buffer.Append(ch);
                this.IsMatched = MatchResults.Matched;
                
            }
            else {
                this.IsMatched = MatchResults.NotMatched;
            }
            return this.IsMatched;
        }
    }
}
