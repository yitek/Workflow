using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Yitec.Workflow.Expressions;
using Yitec.Workflow.Tokens;

namespace Yitec.Workflow
{
    public class WorkflowConfig :ObjectToken
    {
        protected WorkflowConfig(string name) { this.Name = name; }
        protected WorkflowConfig(string name, ObjectToken proto) : base(proto)
        {
            this.Name = name;
        }

        protected WorkflowConfig(ObjectToken proto) : this(proto["name"], proto) { }
        
        public string Name { get; private set; }


        static Dictionary<string, ValueExpression> loadKeyValues(ObjectToken self, string name)
        {
            var values = new Dictionary<string, ValueExpression>();
            var varConfigs = self[name];
            if (varConfigs != null && varConfigs != Token.Empty)
            {
                foreach (KeyValuePair<string, Token> pair in varConfigs as ObjectToken)
                {
                    values.Add(pair.Key, new ValueExpression(pair.Value));
                }
            }
            return values;
        }

        Dictionary<string, ValueExpression> _Imports;
        public IReadOnlyDictionary<string, ValueExpression> Imports
        {
            get
            {
                if (_Imports == null)
                {
                    _Imports = loadKeyValues(this, "imports");
                }
                return this._Imports;
            }
            internal set {
                _Imports = value as Dictionary<string, ValueExpression>;
            }
        }

        Dictionary<string, ValueExpression> _Exports;
        public IReadOnlyDictionary<string, ValueExpression> Exports
        {
            get
            {
                if (_Exports == null)
                {
                    _Exports = loadKeyValues(this, "imports");
                }
                return this._Exports;
            }
            internal set
            {
                _Exports = value as Dictionary<string, ValueExpression>;
            }
        }

        Dictionary<string, ValueExpression> _Variables;
        public IReadOnlyDictionary<string, ValueExpression> Variables
        {
            get
            {
                if (_Variables == null)
                {
                    this._Variables = loadKeyValues(this, "variables");
                }
                return this._Variables;
            }
            internal set
            {
                _Variables = value as Dictionary<string, ValueExpression>;
            }
        }
    }
}
