package yitek.workflow.core.std;

import com.alibaba.fastjson.*;


// 状态转换判断
/*{"op":"eq",}*/
public class Predicate {
	PredicateOperations op;
	public PredicateOperations getOp(){return this.op;}
	String value;
	public String getValue(){return this.value;}
	
	Predicate left;
	public Predicate getLeft(){return this.left;}

	Predicate right;
	public Predicate getRight(){return this.right;}

	public Predicate(String value){
		if(value!=null && (value.startsWith("$.") || value.startsWith("."))){
			this.op = PredicateOperations.member;
			this.value = value.toString();
		}
		this.op = PredicateOperations.constant;
		this.value = value==null||value.equals("<FALSE>")?"":value;
	}
	public Predicate(JSON value){
		if(value instanceof JSONArray){
			JSONArray opcodes = (JSONArray)value;
			String opText = opcodes.getString(0);
			Object arg1=null;
			Object arg2=null;
			if(opcodes.size()==2){
				opText = "eq";
				arg1 = opText;
				arg2 = opcodes.get(1);
			}else {
				arg1 = opcodes.get(1);
				arg2 = opcodes.get(2);
			}
			PredicateOperations op = PredicateOperations.valueOf(opText);
			this.op = op;
			this.left = new Predicate((JSON)arg1);
			this.right = new Predicate((JSON)arg2);
		}else{
			this.op = PredicateOperations.constant;
			this.value = value==null?"":value.toString();
		}
	}

	public String Eval(JSONObject stateVariables,JSONObject flowVariables) throws Exception{
		String left = null;
		String right =null;
		switch(this.op){
			case constant: return this.value;
			case member: return ResolveMemberValue(this.value,stateVariables,flowVariables);
			case eq:
				left = this.left.Eval(stateVariables, flowVariables);
				right = this.right.Eval( stateVariables, flowVariables);
				return left.equals(right)?"<TRUE>":"<FALSE>";
			case neq:
				left = this.left.Eval(stateVariables, flowVariables);
				right = this.right.Eval(stateVariables, flowVariables);
				return !left.equals(right)?"<TRUE>":"<FALSE>";
			case gt:
				left = this.left.Eval(stateVariables, flowVariables);
				right = this.right.Eval(stateVariables, flowVariables);
				return left.compareTo(right)==1?"<TRUE>":"<FALSE>";
			case gte:
				left = this.left.Eval(stateVariables, flowVariables);
				right = this.right.Eval(stateVariables, flowVariables);
				return left.compareTo(right)!=-1?"<TRUE>":"<FALSE>";
			case lt:
				left = this.left.Eval(stateVariables, flowVariables);
				right = this.right.Eval(stateVariables, flowVariables);
				return left.compareTo(right)==-1?"<TRUE>":"<FALSE>";
			case lte:
				left = this.left.Eval(stateVariables, flowVariables);
				right = this.right.Eval(stateVariables, flowVariables);
				return left.compareTo(right)!=1?"<TRUE>":"<FALSE>";
			case or:
				left = this.left.Eval(stateVariables, flowVariables);
				right = this.right.Eval(stateVariables, flowVariables);
				return !left.equals("") || !right.equals("")?"<TRUE>":"";
			case and:
				left = this.left.Eval(stateVariables, flowVariables);
				right = this.right.Eval(stateVariables, flowVariables);
				return !left.equals("") && !right.equals("")?"<TRUE>":"";
			default:
				return "";
		}
	}
	static String ResolveMemberValue(String memberExpr,JSONObject stateVariables,JSONObject flowVariables) throws Exception{
		JSON rs = State.ResolveMemberJSON(memberExpr, stateVariables, flowVariables);
		if(rs==null) return "";
		return rs.toString();
	}
	
	
}
