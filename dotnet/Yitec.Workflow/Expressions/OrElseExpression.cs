using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Yitec.Workflow.Tokens;

namespace Yitec.Workflow.Expressions
{
    public class OrElseExpression : BinaryExpression
    {

        public override async Task<Token> EvalAsync(IEvalContext ctx)
        {
            var leftValue = await this.Left.EvalAsync(ctx);
            if (leftValue) return BooleanToken.True;
            var rightValue = await this.Right.EvalAsync(ctx);
            if (rightValue) return BooleanToken.True;
            return BooleanToken.False;
        }
    }
}
