using System;
using System.Collections.Generic;
using System.Text;

namespace Yitec.Workflow.Tokens
{
    public sealed class BooleanToken :Token
    {
        public BooleanToken(bool value) {
            this.Value = value;
        }
        public bool Value { get; private set; }

        public override Token this[string name] {
            get { return Token.Empty; }
            protected internal set { } 
        }

        public override string ToString()

        {
            return (this.Value) ? TrueString : FalseString;
        }

       

        readonly static string TrueString = "<TRUE>";

        readonly static string FalseString = "<FALSE>";

        public static implicit operator bool(BooleanToken value) => value==null?false:value.Value;

        public static explicit operator string(BooleanToken value) {
            return value == null ? string.Empty : (value.Value?TrueString:string.Empty);
        }

       

  

    }
}
