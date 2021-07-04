using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Yitec.Workflow.Tokens;

namespace Yitec.Workflow.Expressions
{
    public class AndAlsoExpression : BinaryExpression
    {


        public override async Task<Token> EvalAsync(IEvalContext ctx)
        {
            var leftValue = await this.Left.EvalAsync(ctx);
            if (!leftValue) return BooleanToken.False;
            var rightValue = await this.Right.EvalAsync(ctx);
            if (!rightValue) return BooleanToken.False;
            return BooleanToken.True;
        }
    }
}
