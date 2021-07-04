using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Yitec.Workflow.Tokens;

namespace Yitec.Workflow.Expressions
{
    public class ConstantExpression: Expression
    {
        public ConstantExpression(string value) {
            this.Value = value;
        }

        public string Value { get; private set; }

        public async override Task<Token> EvalAsync(IEvalContext ctx)
        {
            return new StringToken(this.Value);
        }
    }
}
