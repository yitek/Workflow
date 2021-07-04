using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Yitec.Workflow.Tokens;

namespace Yitec.Workflow.Expressions
{
    public class ValueExpression:Expression
    {
        public string Text { get; set; }
        public ValueExpression(string text) {
            this.Text = text;

        }
        List<Expression> _Expressions;
        public IReadOnlyList<Expression> Expressions {
            get {
                if (_Expressions == null) this._Expressions = Parse(this.Text);
                return this._Expressions;
            }
        }

        public async override Task<Token> EvalAsync(IEvalContext ctx)
        {
            var strs = this.Expressions;
            if (strs.Count == 0) return Token.Empty;
            if (strs.Count == 1) return await strs[0].EvalAsync(ctx);
            StringBuilder sb = new StringBuilder();
            foreach (var expr in strs) sb.Append((string)(await expr.EvalAsync(ctx)));
            return new StringToken(sb.ToString());
        }

        public static List<Expression> Parse(string valueText) {
            var strs = new List<Expression>();
            var tokenAt = 0;
            while (true) {
                var tokenAt1 = valueText.IndexOf("$", tokenAt);
                if (tokenAt1 < 0) break;
                var constText = valueText.Substring(tokenAt , tokenAt1-tokenAt);
                if (constText != string.Empty) {
                    strs.Add(new ConstantExpression(constText));
                }
                if (++tokenAt == valueText.Length) break;
                if (valueText[tokenAt] != '{') continue;
                var lastAt = valueText.IndexOf("}", ++tokenAt);
                if (lastAt < 0) break;
                var memberExpr = valueText.Substring(tokenAt, lastAt - tokenAt);
                strs.Add(new MemberExpression(memberExpr));
                tokenAt = lastAt;
            }
            var constText1 = valueText.Substring(tokenAt);
            if (constText1 != string.Empty)
            {
                strs.Add(new ConstantExpression(constText1));
            }
            return strs;
        }

        public void BuildJson(StringBuilder sb) {
            sb.Append("\"").Append(this.Text).Append("\"");
        }
    }
}
