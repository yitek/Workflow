using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Yitec.Workflow
{
    public interface IEvalContext
    {
        Task<Token> ResolveTargetAsync(string name);
    }
}
