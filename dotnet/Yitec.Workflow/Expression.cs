using System.Threading.Tasks;
using System;
using System.Text;
using Yitec.Workflow.Expressions;

namespace Yitec.Workflow
{
    public abstract class Expression
    {
        public abstract Task<Token> EvalAsync(IEvalContext ctx);

        

        public static bool IsTrue(Token left) {
            if (left == null || left.ToString() == "") return false;
            return true;
        }

        public static Expression Parse(Token token) {
            if (token == null || token == string.Empty) return PlaceHolder;
            throw new NotImplementedException("Not implement");
        }

        public readonly static ConstantExpression PlaceHolder = new ConstantExpression("<Yitec.Workflow.Expression.PlaceHolder>");

        //public abstract void BuildJson(StringBuilder sb);
    }
}
