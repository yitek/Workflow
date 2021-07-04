using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Yitec.Workflow.Expressions;
using Yitec.Workflow.Tokens;

namespace Yitec.Workflow
{
    public class Transition :ObjectToken
    {
        public Transition(State from ,string toName, ObjectToken data) :base(data){
            this.Name = this["name"];
            this.From = from;
            
            if (string.IsNullOrWhiteSpace(toName)) throw new DiagramException("转换必须定义目标状态属性,请在流程图中给相关转换指定to属性");
            this._toName = toName.Trim();
        }
        
        public string Name { get;private set; }

        Expression _Predicate;
        public Expression Predicate { 
            get {
                if (this._Predicate == null) {
                    Token token = this["predicate"];
                    this._Predicate = Expression.Parse(token);
                    
                }
                return this._Predicate == Expression.PlaceHolder ? null : this._Predicate;
            }
        }

        public State From { get;private set; }


        private string _toName;
        private State _to;
        public async Task<State> ToAsync()
        {
            if (_to == null) {
                var diagram =await this.From.OwnDiagramAsync();
                this._to  = diagram.State(this._toName);
                if (this._to == null) {
                    throw new DiagramException($"转换{{from={this.From.Name},to={this._toName},name={this.Name}}}无法在流程图中找到目的状态,请检查流程图中相关转换的to属性是否正确");
                }
            }
            return this._to;
            

        }



    }
}
