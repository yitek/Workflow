using System;
using System.Collections.Generic;
using System.Text;

namespace Yitec.Workflow.Syntex
{
    public class LimitState:DFState
    {
        public uint Min { get; private set; }

        public uint? Max { get; private set; }

        public DFState InnerState { get; private set; }
        private LimitState() { }

        public LimitState(DFState innerState,uint length) {
            this.Min = length;
            this.Max = length;
            this.InnerState = innerState;
        }
        public LimitState(DFState innerState,uint min, uint? max) {
            this.InnerState = innerState;
            if (max != null && max.Value < min) throw new Exception("max必须比min大");
            this.Min = min;
            this.Max = max;
        }

        public override void Reset()
        {
            base.Reset();
            this.Count = 0;
        }

        public override DFState Clone(bool withHandler=false)
        {
            return new LimitState()
            {
                Min = this.Min,
                Max = this.Max,
                InnerState = this.InnerState.Clone(withHandler),
                _OnResult = withHandler ? this._OnResult : null
            };
        }
        public int Count { get; private set; }
        public override MatchResults Transfer(char ch)
        {
            if (this.IsMatched == MatchResults.NotMatched || this.IsMatched == MatchResults.Matched)
            {
                throw new Exception("已经有终值了，需要reset");
            }
            this.InnerState.Transfer(ch);
            
            if (this.InnerState.IsMatched == MatchResults.Matched)
            {
                this.Count++;
                if (this.Max != null && this.Count == this.Max.Value)
                {
                    this.Buffer.Append(this.InnerState.GetResult());
                    return this.IsMatched = MatchResults.Matched;
                    
                }
                else
                {
                    
                    this.InnerState.Reset();
                    return this.IsMatched = MatchResults.Unknown;
                }
            }
            else if (this.InnerState.IsMatched == MatchResults.Unknown)
            {
                return this.IsMatched = MatchResults.Unknown;
                
            }
            else if (this.InnerState.IsMatched == MatchResults.NotMatched) {
                if (this.Count < this.Min)
                {
                    return this.IsMatched = MatchResults.NotMatched;
                }
                else
                {
                    return this.IsMatched = MatchResults.Matched;
                   
                }
                

            }
            throw new Exception("不正确的配置");

        }

    }
}
