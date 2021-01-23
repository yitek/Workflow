using System;
using System.Collections.Generic;
using System.Text;

namespace Flow
{
    public class InvalidGraphException:Exception
    {
        public InvalidGraphException(string message, params string[] args) 
        :base(string.Format(message,args))
        { }
    }
}
