using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Yitec.Workflow.Tokens;

namespace Yitec.Workflow
{
    public abstract class Token
    {
        public abstract Token this[string name] {
            get;
            protected internal set;
        }

        public string GetString(string name) {
            return this[name];
        }

        public bool GetBoolean(string name)
        {
            return this[name];
        }

        public Guid GetGuid(string name)
        {
            Guid value = Guid.Empty;
            Guid.TryParse(this[name], out value);
            return value;
        }

        public int GetInt(string name)
        {
            int value = 0;
            int.TryParse(this[name], out value);
            return value;
        }

        public static Token Wrap(object value) {
            if (value is Token) return value as Token;
            if (value is IList<Token>) return new ArrayToken(value as IList<Token>);

            return new StringToken(value.ToString());
        }
        



        public static readonly BooleanToken True = new BooleanToken(true);
        public static readonly BooleanToken False = new BooleanToken(false);
        public static readonly StringToken Empty = new StringToken(string.Empty);

        public static implicit operator bool(Token token) {
            if (token == null) return false;
            if (token is BooleanToken) return ((BooleanToken)token).Value;
            if (token is StringToken) return ((StringToken)token).Value != string.Empty;
            return true;
        }

        public static implicit operator string(Token token)
        {
            if (token == null) return string.Empty;
            return token.ToString();
        }

        public override bool Equals(object right)
        {
            if(right is Token)  return Equal(this,right as Token);
            return base.Equals(right);
        }
        public override int GetHashCode()
        {
            if (this is StringToken) return (this as StringToken).Value.GetHashCode();
            if (this is BooleanToken) return (this as BooleanToken).Value ? 1 : 0;
            if (this is ObjectToken) return (this as ObjectToken).Map == null ? 0 : (this as ObjectToken).Map.GetHashCode();
            if (this is ArrayToken) return (this as ArrayToken).List == null ? 0 : (this as ArrayToken).List.GetHashCode();

            return base.GetHashCode();
        }
        static bool Equal(Token left, Token right)
        {
            var leftStr = (string)left;
            var rightStr = (string)right;
            return leftStr == right;
        }
        public static bool operator ==(Token left, Token right) {
            return Equal(left,right);
        }
        public static bool operator !=(Token left, Token right)
        {
            var leftStr = (string)left;
            var rightStr = (string)right;
            return leftStr != rightStr;
        }

        public static bool operator>(Token left, Token right) {
            var leftStr = (string)left;
            var rightStr = (string)right;
            return leftStr.CompareTo(rightStr)>0;
        }

        public static bool operator <(Token left, Token right)
        {
            var leftStr = (string)left;
            var rightStr = (string)right;
            return leftStr.CompareTo(rightStr) < 0;
        }

        public static bool operator >=(Token left, Token right)
        {
            var leftStr = (string)left;
            var rightStr = (string)right;
            return leftStr.CompareTo(rightStr) >= 0;
        }

        public static bool operator <=(Token left, Token right)
        {
            var leftStr = (string)left;
            var rightStr = (string)right;
            return leftStr.CompareTo(rightStr) <= 0;
        }

        /*
        name: start
        imports:
            userId:
                ./abc
                ../abc

         */
        public static async Task<ObjectToken> ParseAsync(TextReader stream)
        {
            int deep = 0;
            IDictionary<string, Token> currentMap = null;
            List<Token> currentArr = null;
            var root = new ObjectToken(currentMap = new Dictionary<string, Token>());


            Stack<IDictionary<string, Token>> stack = new Stack<IDictionary<string, Token>>();
            var lineAt = 1;
            var charAt = 0;
            var buffer = new char[1024];
            var readed = 0;
            var level = 0;
            var lastToken = '\n';
            var sb = new StringBuilder();
            string key = null;
            string value = null;
            string lastName = null;
            while ((readed = await stream.ReadAsync(buffer, 0, 1024)) != 0)
            {
                for (int i = 0; i < readed; i++)
                {
                    char ch = buffer[i];
                    charAt++;
                    if (ch == '\t')
                    {
                        if (lastToken == '\n')
                        {
                            level++;
                            continue;
                        }
                        else throw new DiagramException($"意外的\\t符号,row={lineAt},col={charAt}");
                    }
                    else if (ch == ':')
                    {
                        lastName = key;
                        key = sb.ToString().Trim();
                        if (currentArr != null) throw new DiagramException($"意外的:符号，当前正在定义数组，无法指定属性,row={lineAt},col={charAt}");
                       
                        sb = new StringBuilder();
                        lastToken = ch;
                    }
                    else if (ch == '\n')
                    {
                        lineAt++; lastToken = '\n';
                        value = sb.ToString().Trim();
                        sb = new StringBuilder();

                        if (level > deep)
                        {
                            if (level > deep + 1) throw new DiagramException($"多余的tab,row={lineAt}");
                            deep++;
                            //将当前map压入堆栈
                            stack.Push(currentMap);
                            Token ct = null;
                            if (key == null)
                            {
                                var nextArr = new List<Token>();
                                ct = new ArrayToken(nextArr);
                                currentMap[lastName] = ct;
                                currentArr = nextArr;
                                currentMap = null;

                            }
                            else
                            {
                                var nextMap = new Dictionary<string, Token>();
                                ct = new ObjectToken(nextMap);
                                currentMap[lastName] = ct;
                                currentMap = nextMap;
                                currentArr = null;
                            }
                           
                            lastName = null;
                            
                        }
                        else if (deep == level) { }
                        else
                        {
                            
                            for (var j = deep; j > level; j--)
                            {
                                currentMap = stack.Pop();
                            }
                            
                            if (currentMap == null)
                            {
                                throw new DiagramException($"内部算法错误");
                            }

                        }
                        if (currentArr != null)
                        {
                            currentArr.Add(new StringToken(value));
                        }
                        else
                        {
                            if (currentMap.ContainsKey(key)) throw new DiagramException($"重复的属性定义${key},row={lineAt},col={charAt}");

                            currentMap.Add(key, new StringToken(value));
                        }

                        level = 0;
                    }
                    else sb.Append(ch);

                }
            }
            return root;

        }

        public static async Task<ObjectToken> ParseAsync(string content) {
            using (var mstream = new StringReader(content)) {
                var token = await ParseAsync(mstream);
                return token;
            }
        }

        public static async Task<ObjectToken> ParseFromFileAsync(string file)
        {
            using (var mstream = new StreamReader(file))
            {
                return await ParseAsync(mstream);
            }
        }

    }
}
