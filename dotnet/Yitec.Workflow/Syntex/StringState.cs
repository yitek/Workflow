//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Yitec.Workflow.Syntex
//{
//    public class StringState: DFState
//    {
//        public StringState(char[] quotes) {
//            this.Quotes = quotes?? new char[] { '"','\''};
//        }

//        public override void Reset()
//        {
//            this.Quote = '0';
//            base.Reset();
//        }
//        public char[] Quotes { get; private set; }

//        public char Quote { get; private set; }

//        bool _MeaningTransfering;
//        public override IReadOnlyList<DFState> Transfer(char ch)
//        {
//            if (this.Quote == '0') {
//                for (var i = 0; i < this.Quotes.Length; i++) {
//                    if (ch == this.Quotes[i]) {
//                        this.Quote = ch;
//                        this.IsMatched = MatchResults.Unknown;
//                        return this.SelfStates;
//                    }
//                }
//                return null;
//            }
//            if (ch == '\\')
//            {
//                if (this._MeaningTransfering)
//                {
//                    this.Buffer.Append(ch);
//                    this._MeaningTransfering = false;

//                }
//                else this._MeaningTransfering = true;
//                return this.SelfStates;
//            }
//            else if (ch == this.Quote)
//            {
//                if (this._MeaningTransfering)
//                {
//                    this.Buffer.Append(ch);
//                    this._MeaningTransfering = false;
//                    return this.SelfStates;
//                }
//                else {
//                    this.IsMatched = MatchResults.Matched;
//                    return this.NextStates;
//                }
//            }
//            else {
//                this.Buffer.Append(ch);
//                return this.SelfStates;
//            }
//        }

//        public override DFState Clone()
//        {
//            return new StringState(this.Quotes);
//        }
//    }
//}
