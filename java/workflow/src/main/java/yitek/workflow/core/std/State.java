package yitek.workflow.core.std;
import java.util.*;
import com.alibaba.fastjson.*;

public class State {
	JSONObject _data;
	//@SuppressWarnings("unchecked")
	public State(String name,JSONObject data,Diagram diagram){
		this._name = name; 
		this._data = data;
		this._diagram = diagram;
	}
	public State(String name,JSONObject data){
		this(name,data,null);
	}
	public State(JSONObject data){
		this(null,data,null);
	}

	Diagram _diagram;
	public Diagram ownDiagram(){
		return this._diagram;
	}

	String _name;
	public String name(){
		if(_name==null){
			Object value = null;
			value = _data.get("name");
			if(value!=null) this._name = value.toString();
			else this._name = "";
		}
		return this._name;
	}
	String _actionType;
	public String actionType(){
		if(_actionType==null){
			Object value = null;
			value = _data.get("actionType");
			if(value!=null) this._actionType = value.toString();
			else this._actionType = "";
		}
		return this._actionType;
	}
	//public State setName(String value) { this.name= value; return this;}



	Map<String,Object> _variables;
	public Map<String,Object> variables(){
		if(this._variables==null){
			Map<String,Object> vars = new HashMap<String,Object>();
			if(this._diagram!=null){
				Map<String,Object> superVars = this._diagram.variables();
				for(Map.Entry<String,Object> entry: superVars.entrySet()){
					vars.put(entry.getKey(), entry.getValue());
				}
			}
			Object meVals = this._data.get("variables");
			if(meVals instanceof JSONObject){
				for(Map.Entry<String,Object> entry: ((JSONObject)meVals).entrySet()){
					vars.put(entry.getKey(), entry.getValue());
				}
			}
			this._variables = Collections.unmodifiableMap(vars);
		}
		return this._variables;
	}
	
	Map<String,List<String>> _imports;
	public Map<String,List<String>> imports(){
		if(_imports==null){
			Object value = _data.get("imports");
			Map<String,List<String>> existed = null;
			if(this._diagram!=null) existed = this._diagram.imports();
			this._imports = State.mergeImExport(existed, value);
		}
		return _imports;
	}
	Map<String,List<String>> _exports;
	public Map<String,List<String>> exports(){
		if(_exports==null){
			Object value = _data.get("exports");
			Map<String,List<String>> existed = null;
			if(this._diagram!=null) existed = this._diagram.exports();
			this._exports = State.mergeImExport(existed, value);
		}
		return _exports;
	}

	Map<String,Transition> _transitions;
	public Map<String,Transition> transitions() throws Exception{
		if(_transitions==null){
			this._transitions = new HashMap<String,Transition>();
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

	Diagram _sub;
	public Diagram subDiagram(){
		if(this._sub==null){
			Object value = _data.get("imports");
			if(value instanceof JSONObject) {
				this._sub =new Diagram((JSONObject)value,this);
			}else this._sub = Diagram.empty();
		}
		return this._sub==Diagram.empty()?null:this._sub;
	}

	public static JSON resolveMemberJSON(String memberExpr,JSONObject scope) throws Exception{
		if(memberExpr==null) return null;
		JSON target=scope;
		String[] pathnames = memberExpr.split(".");
		
		for(int i = 1;i<pathnames.length;i++){
			String pathname = pathnames[i];
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
			Boolean hasIt=false;
			for(String exi : list1) {
				if(exi.equals(val)) {hasIt=true;break;}
			}
			if(!hasIt) list1.add(val);
		}
	}
	public static Map<String,List<String>> mergeImExport(Map<String,List<String>> superMap, Object value){
		Map<String,List<String>> map = new HashMap<String,List<String>>();
		//将上级的拷贝到自己里面来
		if(superMap!=null){
			for(Map.Entry<String,List<String>> entry:superMap.entrySet()){
				List<String> vals = new ArrayList<String>(entry.getValue());
				map.put(entry.getKey(), vals);
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
					uniqueMergeStringList(existed, vals);
				}
			}
			
		}
		for(Map.Entry<String,List<String>> entry : map.entrySet()){
			map.replace(entry.getKey(), Collections.unmodifiableList(entry.getValue()));
		}
		return Collections.unmodifiableMap(map);
	}

	public JSONObject jsonObject(){
		JSONObject rs = new JSONObject();
		rs.put("name", this.name());
		if(!this.actionType().equals(""))rs.put("actionType",this.actionType());
		rs.put("variables",this.variables());
		rs.put("imports",this._data.get("imports"));
		rs.put("exports",this._data.get("exports"));
		if(this.subDiagram()!=null){
			rs.put("sub", this.subDiagram().jsonObject());
		}
		return rs;
	}

	public String jsonString(){
		JSONObject obj = this.jsonObject();
		return JSON.toJSONString(obj);
	}

	public static Object cloneJSON(Object obj){
		if(obj instanceof JSONArray){
			JSONArray rs = new JSONArray();
			for(Object item : (JSONArray)obj){
				rs.add(cloneJSON(item));
			}
			return rs;
		}else if(obj instanceof JSONObject){
			JSONObject rs = new JSONObject();
			for(Map.Entry<String,Object> pair : ((JSONObject)obj).entrySet()){
				rs.put(pair.getKey(), cloneJSON(pair.getValue()));
			}
			return rs;
		}
		else return obj;
	}
	public static void resetSubDiagram(State state,Diagram subDiagram){
		state._diagram = subDiagram;
	}
}
