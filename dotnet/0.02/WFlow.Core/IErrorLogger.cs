using System;
using System.Collections.Generic;
using System.Text;

using WFlow.Entities;

namespace WFlow
{
    public interface IErrorLogger
    {
        void Log(Engine flow,Activity activity,ActivityEntity entity, Exception ex);
    }
}
