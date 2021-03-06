package yitek.workflow.core.std;

import com.alibaba.fastjson.*;
import yitek.workflow.core.*;
// 状态转换判断
/*{"op":"eq",}*/
public class Predicate {
	PredicateOperations _op;
	public PredicateOperations op(){return this._op;}

	static Predicate _empty = new Predicate();
	public static Predicate empty(){return _empty;}

	String _value;
	public String value(){return this._value;}
	
	Predicate _left;
	public Predicate left(){return this._left;}

	Predicate _right;
	public Predicate right(){return this._right;}
	private Predicate(){}
	public Predicate(String value){
		if(value!=null && value.indexOf('/')>=0){
			this._op = PredicateOperations.member;
			this._value = value.toString();
		}
		this._op = PredicateOperations.constant;
		this._value = value==null||value.equals("<FALSE>")?"":value;
	}
	public Predicate(Object value){
		if(value instanceof JSONArray){
			JSONArray opcodes = (JSONArray)value;
			String opText = opcodes.getString(0);
			Object arg1=null;
			Object arg2=null;
			if(opcodes.size()==2){
				//opText = "eq";
				arg1 = opText;
				arg2 = opcodes.get(1);
				opText = "eq";
			}else {
				arg1 = opcodes.get(1);
				arg2 = opcodes.get(2);
			}
			PredicateOperations op = PredicateOperations.valueOf(opText);
			this._op = op;
			this._left = new Predicate(arg1);
			this._right = new Predicate(arg2);
		}else{

			this._value = value==null||value.equals("<FALSE>")?"":value.toString();
			if(_value.indexOf('/')>=0){
				this._op = PredicateOperations.member;
			}else this._op = PredicateOperations.constant;

		}
	}

	public String eval(Activity activity ,Activity from,FlowContext ctx) throws Exception{
		String left = null;
		String right =null;
		switch(this._op){
			case constant: return this._value;
			case member: return activity.resolveVariableString(this._value);
			case eq:
				left = this._left.eval(activity,from,ctx);
				right = this._right.eval(activity,from,ctx);
				return left.equals(right)?"<TRUE>":"<FALSE>";
			case neq:
				left = this._left.eval(activity,from,ctx);
				right = this._right.eval(activity,from,ctx);
				return !left.equals(right)?"<TRUE>":"<FALSE>";
			case gt:
				left = this._left.eval(activity,from,ctx);
				right = this._right.eval(activity,from,ctx);
				return left.compareTo(right)==1?"<TRUE>":"<FALSE>";
			case gte:
				left = this._left.eval(activity,from,ctx);
				right = this._right.eval(activity,from,ctx);
				return left.compareTo(right)!=-1?"<TRUE>":"<FALSE>";
			case lt:
				left = this._left.eval(activity,from,ctx);
				right = this._right.eval(activity,from,ctx);
				return left.compareTo(right)==-1?"<TRUE>":"<FALSE>";
			case lte:
				left = this._left.eval(activity,from,ctx);
				right = this._right.eval(activity,from,ctx);
				return left.compareTo(right)!=1?"<TRUE>":"<FALSE>";
			case or:
				left = this._left.eval(activity,from,ctx);
				right = this._right.eval(activity,from,ctx);
				return !left.equals("") || !right.equals("")?"<TRUE>":"";
			case and:
				left = this._left.eval(activity,from,ctx);
				right = this._right.eval(activity,from,ctx);
				return !left.equals("") && !right.equals("")?"<TRUE>":"";
			default:
				return "";
		}
	}
	
	
	
}
