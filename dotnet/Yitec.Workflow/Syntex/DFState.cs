using System;
using System.Collections.Generic;
using System.Text;

namespace Yitec.Workflow.Syntex
{
    public abstract class DFState
    {
        public enum MatchResults { 
            Init,
            Unknown,
            Matched,
            NotMatched
        }
        protected DFState() {
            this.Buffer = new StringBuilder();
            this.IsMatched = MatchResults.Init;
        }
        
        protected StringBuilder Buffer { get; private set; }
        public abstract MatchResults Transfer(char ch);

       

        public abstract DFState Clone(bool withHandler=false);

        public MatchResults IsMatched {
            get;protected set;
        }

        protected Action<DFState> _OnResult;

        public string GetResult() {
            return this.Buffer.ToString();
        }

        public virtual void Reset() {
            this.Buffer.Clear();
            this.IsMatched = MatchResults.Init;
        }

        
        

        public DFState OnStateChange(Action<DFState> evtHandler) {
            this._OnResult = evtHandler;
            return this;
        }

        public DFState Limits(uint length)
        {
            return new LimitState(this, length);
        }

        public DFState Limits(uint min, uint? max = null)
        {
            return new LimitState(this,min,max);
        }

        public static implicit operator DFState(string text) {
            return new ConstState(text);
        }

        public static DFState operator|(DFState left, DFState right) {
            return new AlternativeState(left ,right);
        }
        public static DFState operator &(DFState left, DFState right)
        {
            return new AlternativeState(left, right);
        }
    }
}
