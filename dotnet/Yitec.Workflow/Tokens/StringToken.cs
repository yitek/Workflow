using System;
using System.Collections.Generic;
using System.Text;

namespace Yitec.Workflow.Tokens
{
    public sealed class StringToken: Token
    {
        public StringToken(string value) {
            this.Value = value == null ? string.Empty : value;
        }

        public string Value { get; private set; }
        public override Token this[string name] {
            get {
                int index = 0;
                if (int.TryParse(name, out index)) {
                    if(this.Value.Length>index) return new StringToken(this.Value.Substring(index,1));
                    return Token.Empty;
                }
                return Token.Empty;
            }
            protected internal set{ }
        }

        public override string ToString()
        {
            return this.Value;
        }


        public static explicit operator BooleanToken(StringToken value)
        {
            return value == null || value.Value == string.Empty ? BooleanToken.False : BooleanToken.True;
        }
        public static explicit operator string(StringToken value)
        {
            return value == null ? string.Empty : value.Value;
        }

        public static implicit operator bool(StringToken value)
        {
            return value != null && value.Value != string.Empty;
        }

        
       
    }
}
