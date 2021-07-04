using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yitec.Workflow.Tokens;

namespace Yitec.Workflow
{
    public class Diagram : WorkflowConfig
    {
        public Diagram(ObjectToken proto,State ownState) : base(proto) {
            this.OwnState = ownState;
        }
        private Diagram() :base("<Yitec.Workflow.Diagram.PlaceHolder>"){}

        public State OwnState { get; private set; }

        public static readonly Diagram PlaceHolder = new Diagram();

        List<State> _States;

        public IReadOnlyList<State> States() {
            if (this._States == null) {
                var states = this["states"] as ObjectToken;
                this._States = new List<State>();
                if (states != null)
                {
                    foreach (var pair in states)
                    {
                        this._States.Add(new State(pair.Key,pair.Value as ObjectToken,this));
                    }
                }
            }
            return this._States;
        }

        public State State(string name) {
            return this.States().FirstOrDefault(state=>state.Name==name);
        }


    }
}
