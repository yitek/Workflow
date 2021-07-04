using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Yitec.Workflow.Tokens;

namespace Yitec.Workflow.Expressions
{
    public class GreaterThanOrEqualExpression : BinaryExpression
    {
        public async override Task<Token> EvalAsync(IEvalContext ctx)
        {
            if (this.Left == this.Right) return Token.True;
            var leftToken = await this.Left.EvalAsync(ctx);
            var rightToken = await this.Right.EvalAsync(ctx);
            return new BooleanToken(leftToken >= rightToken);
        }
    }
}
