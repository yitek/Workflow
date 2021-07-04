using System;
using System.Collections.Generic;

using System.Text;
using System.Threading.Tasks;
using Yitec.Workflow.Expressions;
using Yitec.Workflow.Tokens;

namespace Yitec.Workflow
{
    public class State:WorkflowConfig
    {
        public State(string name, ObjectToken proto,Func<Task<Diagram>> diagramGetter) : base(name,proto){
            this._DiagramGetter = diagramGetter;
        }
        public State(string name, ObjectToken proto, Diagram diagram) : this(name, proto,async ()=>diagram){}
        public State(ObjectToken proto, Func<Task<Diagram>> diagramGetter) : this(proto["name"], proto,diagramGetter){}

        public State(ObjectToken proto, Diagram ownDiagram):this(proto["name"],proto,async ()=>ownDiagram) {}
       

        Func<Task<Diagram>> _DiagramGetter;

        public async Task<Diagram> OwnDiagramAsync() {
            return await this._DiagramGetter();
        }

        Diagram _SubDiagram;
        public Diagram SubDiagram { 
            get {
                if (_SubDiagram == null) {
                    var token = this["subDiagram"] as ObjectToken;
                    if (token == Token.Empty)
                    {
                        this._SubDiagram = Diagram.PlaceHolder;
                    }
                    else {
                        this._SubDiagram = new Diagram(token, this);
                    }
                }
                return this._SubDiagram == Diagram.PlaceHolder ? null : this._SubDiagram;
            }
        }


    }
}
