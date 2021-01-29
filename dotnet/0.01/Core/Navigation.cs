using WFlow.Graphs;
using System;
using System.Collections.Generic;
using System.Text;

namespace WFlow
{
    public class Navigation
    {
        public Activity FromActivity { get; set; }
        public Association Association { get;private set; }
        
        public Activity ToActivity { get; set; }

        internal void Init(Activity fromActivity, Association assoc) {
            this.Association = assoc;
            this.FromActivity = fromActivity;
        }

        public virtual IReadOnlyDictionary<string,string> Navigate(IReadOnlyState states,Context context=null) {
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
