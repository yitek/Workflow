using Flow.Graphs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flow
{
    public class Navigation
    {
        public Activity FromActivity { get; set; }
        public Association Association { get;private set; }
        

        internal void Init(Activity fromActivity, Association assoc) {
            this.Association = assoc;
            this.FromActivity = fromActivity;
        }

        public virtual IReadOnlyDictionary<string,string> Navigate(IReadOnlyDictionary<string,string> states,FlowContext context=null) {
            var assoc = this.Association;
            if (string.IsNullOrWhiteSpace(assoc.Key)) {
                throw new Exception("必须要有key");
            }
            
            if (!states.TryGetValue(assoc.Key,out string value)) {
                return null;
            }
            if (value != this.Association.Value) return null;
            return new Dictionary<string,string>();
        }
    }
}
