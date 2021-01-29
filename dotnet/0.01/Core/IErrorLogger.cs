using System;
using System.Collections.Generic;
using System.Text;

namespace WFlow
{
    public interface IErrorLogger
    {
        void Log(Engine flow,Activity activity,ActivityEntity entity, Exception ex);
    }
}
