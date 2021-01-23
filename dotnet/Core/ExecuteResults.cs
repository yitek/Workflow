using System;
using System.Collections.Generic;
using System.Text;

namespace Flow
{
    public class ExecuteResults:Dictionary<string,string>
    {
        public ExecuteResults(bool isDone) {
            this.IsDone = IsDone;
        }
       public bool IsDone { get;private set; }
    }
}
