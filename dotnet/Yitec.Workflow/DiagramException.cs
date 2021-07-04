using System;
using System.Collections.Generic;
using System.Text;

namespace Yitec.Workflow
{
    public class DiagramException :Exception
    {
        public DiagramException(string message, params object[] pars) : base(String.Format(message,pars)) { }
    }
}
