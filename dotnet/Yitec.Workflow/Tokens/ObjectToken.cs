using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Yitec.Workflow.Tokens
{
    public class ObjectToken : Token, IEnumerable<KeyValuePair<string, Token>>
    {
        internal ObjectToken() { }
        public ObjectToken(IDictionary<string, Token> raw) {
            this.Map = raw;
        }
       
        public ObjectToken(ObjectToken other)
        {
            this.Map = other.Map;
        }
        internal IDictionary<string,Token> Map { get; private set; }

        public override Token this[string name] {
            get {
                Token token = null;
                if (this.Map.TryGetValue(name, out token)) return token;
                return Token.Empty;
            }
            protected internal set{
                this.Map[name] = value;
            }
        }

        public static implicit operator bool(ObjectToken value)
        {
            return value.Map != null;
        }

        public static explicit operator string(ObjectToken value)
        {
            return value.Map != null ? value.ToString() : string.Empty;
        }

        public IEnumerator<KeyValuePair<string, Token>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
