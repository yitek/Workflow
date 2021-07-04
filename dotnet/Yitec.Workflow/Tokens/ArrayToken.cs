using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Yitec.Workflow.Tokens
{
    public sealed class ArrayToken :Token,IEnumerable<Token>
    {
        public ArrayToken(IList<Token> raw) {
            this.List = new List<Token>();
        }
        readonly static Regex NumberRegx = new Regex("^\\d+$");
        internal IList<Token> List;
        public override Token this[string name] {
            get
            {
                int index = 0;
                if (int.TryParse(name, out index)) {
                    return this.List[index];
                }
                return name == "length" ? new StringToken(this.List.Count.ToString()) : Token.Empty;
            }
            protected internal set {
                int index = 0;
                if (int.TryParse(name, out index))
                {
                    if (index < 0)
                    {
                        this.List.Add(value);
                    }
                    else this.List[index] = value;
                }
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach(var item in this.List) {
                if (sb.Length > 0) sb.Append(',');
                sb.Append(item.ToString());
            }
            return sb.ToString();
        }




        public static implicit operator bool(ArrayToken value)
        {
            return value.List != null && value.List.Count>0;
        }

        public static explicit operator string(ArrayToken value)
        {
            return value.List != null?value.ToString():null;
        }

        public IEnumerator<Token> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
