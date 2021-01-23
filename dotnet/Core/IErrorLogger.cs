using System;
using System.Collections.Generic;
using System.Text;

namespace Flow
{
    public interface IErrorLogger
    {
        void Log(Activity activity, Exception ex);
    }
}
