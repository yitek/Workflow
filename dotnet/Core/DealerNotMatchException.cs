using System;
using System.Collections.Generic;
using System.Text;

namespace Flow
{
    public class DealerNotMatchException:Exception
    {
        public DealerNotMatchException(string message = null) : base(message) { }
    }
}
