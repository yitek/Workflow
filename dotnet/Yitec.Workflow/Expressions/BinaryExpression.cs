using System;
using System.Collections.Generic;
using System.Text;

namespace Yitec.Workflow.Expressions
{
    public abstract class BinaryExpression : Expression
    {
        public Expression Left { get; internal set; }
        public Expression Right { get; internal set; }
    }
}
