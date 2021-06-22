package yitek.workflow.core.std;

import java.util.*;

import com.alibaba.fastjson.*;
import com.alibaba.fastjson.annotation.JSONField;

public class State {
	@JSONField(serialize = false)
	JSONObject data;
	//@SuppressWarnings("unchecked")
	public State(String name,JSONObject data){
		this.name = name; 
		this.data = data;
	}
	public State(JSONObject data){
		this(null,data);
	}
	String name;
	public String getName(){
		if(name==null){
			Object value = null;
			value = data.get("name");
			if(value!=null) this.name = value.toString();
			else this.name = "";
		}
		return this.name;
	}
	//public State setName(String value) { this.name= value; return this;}

	String activityType;
	public String getActivityType(){
		if(name==null){
			Object value = data.get("activityType");
			if(value!=null) this.activityType = value.toString();
			else this.activityType = "";
		}
		return this.name;

	}
	//public State setActivityType(String value) { this.activityType= value; return this;}
	
	public JSONObject variables;
	public JSONObject getVariables(){
		if(variables==null){
			Object value = data.get("variables");
			if(value instanceof JSONObject) this.variables = (JSONObject)value;
			else this.variables = new JSONObject();
		}
		return variables;
	}
	//public State setVariables(JSONObject value) { this.variables= value; return this;}
	
	Map<String,String> imports;
	public Map<String,String> getImports(){
		if(imports==null){
			this.imports = new HashMap<String,String>();
			Object value = data.get("imports");
			if(value instanceof JSONObject) {
				
				for(Map.Entry<String,Object> entry : ((JSONObject)value).entrySet()){
					this.imports.put(entry.getKey(),entry.getValue().toString());
				}
			}
		}
		return imports;
	}
	Map<String,String> exports;
	public Map<String,String> getExports(){
		if(imports==null){
			this.exports = new HashMap<String,String>();
			Object value = data.get("exports");
			if(value instanceof JSONObject) {
				
				for(Map.Entry<String,Object> entry : ((JSONObject)value).entrySet()){
					this.exports.put(entry.getKey(),entry.getValue().toString());
				}
			}
		}
		return exports;
	}

	Map<String,Transition> transitions;
	public Map<String,Transition> getTransitions(){
		if(transitions==null){
			this.transitions = new HashMap<String,Transition>();
			Object value = data.get("transitions");
			if(value instanceof JSONObject) {
				
				for(Map.Entry<String,Object> entry : ((JSONObject)value).entrySet()){
					String to = entry.getKey();
					this.transitions.put(to,new Transition(this.name,to,entry.getValue()));
				}
			}
		}
		return transitions;
	}

	String start;
	public String getStart(){
		if(start==null){
			Object value = data.get("start");
			if(value!=null) this.start = value.toString();
			else this.start = "";
		}
		return this.start;

	}
	String[] starts;
	public String[] getStarts(){
		if(starts==null){
			Object value = data.get("starts");
			if(value!=null){
				this.starts = ((JSONArray)value).toArray(this.starts);
			}
			else this.starts = new String[0];
		}
		return this.starts;

	}

	Map<String,State> states;
	public Map<String,State> getStates(){
		if(states==null){
			this.states = new HashMap<String,State>();
			Object value = data.get("states");
			if(value instanceof JSONObject) {
				
				for(Map.Entry<String,Object> entry : ((JSONObject)value).entrySet()){
					String name = entry.getKey();
					this.states.put(name,new State(name,(JSONObject)entry.getValue()));
				}
			}
		}
		return states;
	}

	

	
	

	public static JSON ResolveMemberJSON(String memberExpr,JSONObject stateVariables,JSONObject flowVariables) throws Exception{
		if(memberExpr==null) return null;
		JSON target=null;
		String[] pathnames = memberExpr.split(".");
		int start = 0;
		String pathname = pathnames[start];
		if(pathname=="$"){
			target = flowVariables;
		}else if(pathname==""){
			target =  stateVariables;
		}else throw new Exception("不正确的表达式:" + memberExpr);
		for(int i = 1;i<pathnames.length;i++){
			pathname = pathnames[i];
			if(target instanceof JSONObject){
				Object tgt = ((JSONObject)target).get(pathname);
				if(tgt==null) return  null;
				if(tgt instanceof JSON){
					target = (JSON) tgt;
				}else return  null;
				
			}
		}
		return target;
	}

}
