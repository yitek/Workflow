using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Yitec.Workflow.Tokens;

namespace Yitec.Workflow.Expressions
{
    /// <summary>
    /// 字符串拼接表达式
    /// </summary>
    public class ConcatExpression : Expression
    {
        public IReadOnlyList<Expression> Expressions { get; private set; }

        public async override Task<Token> EvalAsync(IEvalContext ctx)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var expr in this.Expressions) sb.Append((string)await expr.EvalAsync(ctx));
            return new StringToken(sb.ToString());
        }
    }
}
