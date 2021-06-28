package yitek.workflow.core.std;
import java.util.*;
import com.alibaba.fastjson.*;

import yitek.workflow.core.StringMap;



public class State {
	JSONObject _data;
	//@SuppressWarnings("unchecked")
	public State(String name,JSONObject data,Diagram ownDiagram){
		this._name = name; 
		this._data = data;
		this._ownDiagram = ownDiagram;
	}
	private State(String name,JSONObject data){
		this(name,data,null);
	}
	public static State createFlow(String name,Diagram subDiagram) throws Exception{
		JSONObject stateData = new JSONObject();
		stateData.put("name", name);
		stateData.put("subDiagram", subDiagram.jsonObject());
		State state = new State(name,stateData);
		state._sub = subDiagram;
		return state;
	}

	private State(){}
	static State _empty = new State();
	public static State empty(){
		return _empty;
	}

	Diagram _ownDiagram;
	public Diagram ownDiagram(){
		return this._ownDiagram;
	}

	String _name;
	public String name(){
		if(_name==null){
			Object value;
			value = _data.get("name");
			if(value!=null) this._name = value.toString();
			else this._name = "";
		}
		return this._name;
	}
	String _actionType;
	public String actionType(){
		if(_actionType==null){
			Object value = _data.get("actionType");
			if(value!=null) this._actionType = value.toString();
			else {
				if(this.ownDiagram()!=null) this._actionType = this.ownDiagram().actionType();
				else this._actionType = "";
			}
		}
		return this._actionType;
	}
	
	//public State setName(String value) { this.name= value; return this;}



	StringMap _variables;
	public StringMap variables(){
		if(this._variables==null){
			StringMap vars = new StringMap();
			if(this._ownDiagram!=null){
				Map<String,Object> superVars = this._ownDiagram.variables();
				for(Map.Entry<String,Object> entry: superVars.entrySet()){
					vars.put(entry.getKey(), entry.getValue());
				}
			}
			Object myVariableObj = this._data.get("variables");
			if(myVariableObj instanceof Map){
				for(Map.Entry<String,Object> entry: ((JSONObject)myVariableObj).entrySet()){
					vars.put(entry.getKey(), entry.getValue());
				}
			}
			this._variables =vars.readonly(true);
		}
		return this._variables;
	}
	public Object variables(String name){
		return this.variables().get(name);
	}
	
	Map<String,List<String>> _imports;
	public Map<String,List<String>> imports(){
		if(_imports==null){
			Object value = _data.get("imports");
			Map<String,List<String>> existed = null;
			if(this._ownDiagram!=null) existed = this._ownDiagram.imports();
			this._imports = State.mergeImExport(existed, value);
		}
		return _imports;
	}
	public List<String> imports(String name){
		return StringMap.resolve(this.imports(), name);
	}
	Map<String,List<String>> _exports;
	public Map<String,List<String>> exports(){
		if(_exports==null){
			Object value = _data.get("exports");
			Map<String,List<String>> existed = null;
			if(this._ownDiagram!=null) existed = this._ownDiagram.exports();
			this._exports = State.mergeImExport(existed, value);
		}
		return _exports;
	}
	public List<String> exports(String name){
		return StringMap.resolve(this.exports(), name);
	}

	Map<String,Transition> _transitions;
	public Map<String,Transition> transitions() throws Exception{
		if(_transitions==null){
			this._transitions = new HashMap<>();
			Object value = _data.get("transitions");
			if(value instanceof JSONObject) {
				
				for(Map.Entry<String,Object> entry : ((JSONObject)value).entrySet()){
					String to = entry.getKey();
					this._transitions.put(to,new Transition(this,to,entry.getValue()));
				}
			}
		}
		return _transitions;
	}
	public List<String> transitions(String name){
		return StringMap.resolve(this.exports(), name);
	}

	Diagram _sub;
	public Diagram subDiagram(){
		if(this._sub==null){
			Object value = _data.get("subDiagram");
			if(value instanceof JSONObject) {
				this._sub =new Diagram((JSONObject)value,this);
			}else if(value instanceof  String){
				this._sub = new ReferenceDiagram(value.toString());
			}else this._sub = Diagram.empty();
		}
		return this._sub==Diagram.empty()?null:this._sub;
	}

	public static JSON resolveMemberJSON(String memberExpr,JSONObject scope) throws Exception{
		if(memberExpr==null) return null;
		JSON target=scope;
		String[] pathNames = memberExpr.split("\\.");
		
		for(int i = 1;i<pathNames.length;i++){
			String pathname = pathNames[i];
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

	static void uniqueMergeStringList(List<String> list1,List<String> list2){
		for(String val : list2){
			boolean hasIt=false;
			for(String exi : list1) {
				if(exi.equals(val)) {hasIt=true;break;}
			}
			if(!hasIt) list1.add(val);
		}
	}
	public static Map<String,List<String>> mergeImExport(Map<String,List<String>> superMap, Object value){
		Map<String,List<String>> map = new HashMap<>();
		//将上级的拷贝到自己里面来
		if(superMap!=null){
			for(Map.Entry<String,List<String>> entry:superMap.entrySet()){
				List<String> exprs = new ArrayList<>(entry.getValue());
				map.put(entry.getKey(), exprs);
			}
		}
		if(value instanceof JSONObject) {
			for(Map.Entry<String,Object> entry : ((JSONObject)value).entrySet()){
				List<String> existed = map.get(entry.getKey());
				List<String> vals =null;
				if(entry.getValue() instanceof String){
					vals = Arrays.asList(entry.getValue().toString().split(","));
				}else if(entry.getValue() instanceof JSONArray){
					vals = ((JSONArray)entry.getValue()).toJavaList(String.class);
				}
				if(existed==null) {
					map.put(entry.getKey(), vals);
				}else{
					if(vals!=null)uniqueMergeStringList(existed, vals);
				}
			}
			
		}
		for(Map.Entry<String,List<String>> entry : map.entrySet()){
			map.replace(entry.getKey(), Collections.unmodifiableList(entry.getValue()));
		}
		return map;
	}

	public StringMap jsonObject() throws Exception{
		StringMap rs = new StringMap();
		rs.put("name", this.name());
		if(!this.actionType().equals(""))rs.put("actionType",this.actionType());
		rs.put("variables",this.variables());
		rs.put("imports",this._data.get("imports"));
		rs.put("exports",this._data.get("exports"));
		if(this.subDiagram()!=null){
			if(this._sub instanceof ReferenceDiagram){
				rs.put("subDiagram", ((ReferenceDiagram)this._sub).reference());
			}else {
				rs.put("subDiagram", this.subDiagram().jsonObject());
			}
		}
		return rs;
	}

	public String jsonString() throws Exception{
		Object obj = this.jsonObject();
		return JSON.toJSONString(obj);
	}
	
	public static void resetSubDiagram(State state,Diagram subDiagram){
		state._ownDiagram = subDiagram;
	}
}
