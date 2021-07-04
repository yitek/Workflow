using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Yitec.Workflow.Expressions
{
    /// <summary>
    /// 成员访问表达式
    /// </summary>
    public class MemberExpression : Expression
    {
        public string Code { get; private set; }
        public MemberExpression(string text) {
            this.Code = text;
        }

        void Parse() {
            var code = this.Code;
            var at = code.IndexOf('/');
            if (at >= 0)
            {
                this._Target = this.Code.Substring(0, at);
                code = code.Substring(at + 1);
            }
            else this._Target = string.Empty;
            this._Members = new List<string>(code.Split('.'));
        }
        string _Target;
        public string Target { get { if (this._Target == null) Parse();return this._Target; } }

        List<string> _Members;
        public IReadOnlyList<String> Members { get { if (this._Target == null) Parse();return this._Members; } }

        public async override Task<Token> EvalAsync(IEvalContext ctx)
        {
            Token value = await ctx.ResolveTargetAsync(this.Target);
            if (value == null) return Token.Empty;
            foreach (var member in Members) {
                value = value[member];
                if (value == null) return Token.Empty;
            }
            return value;
        }
    }
}
