package yitek.workflow.core.std;
import java.util.*;
import com.alibaba.fastjson.*;

import yitek.workflow.core.StringMap;



public class State {
	StringMap _data;
	//@SuppressWarnings("unchecked")
	public State(String name,StringMap data,Diagram ownDiagram){
		this._name = name; 
		this._data = data;
		this._ownDiagram = ownDiagram;
	}
	private State(String name,StringMap data){
		this(name,data,null);
	}
	public static State createFlow(String name,Diagram subDiagram) throws Exception{
		StringMap stateData = new StringMap();
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

	private Diagram _ownDiagram;
	public Diagram ownDiagram(){
		return this._ownDiagram;
	}

	private String _name;
	public String name(){
		if(_name==null){
			this._name = this._data.getString("@name",""); 
		}
		return this._name;
	}
	private String _actionName;
	public String actionName(){
		if(_actionName==null){
			this._actionName = this._data.getString("@actionName","");
			if(this._actionName.equals("")) {
				if(this.ownDiagram()!=null) this._actionName = this.ownDiagram().actionName();
				else this._actionName = "";
			}
		}
		return this._actionName;
	}
	
	//public State setName(String value) { this.name= value; return this;}



	private StringMap _variables;
	List<String> _internals;
	public List<String> internals(){
		return this._internals;
	}
	public StringMap variables() throws Exception{
		if(this._variables==null){
			this._variables = new StringMap();
			if(this._ownDiagram!=null){
				Map<String,Object> superVars = this._ownDiagram.variables();
				for(Map.Entry<String,Object> entry: superVars.entrySet()){
					this._variables.put(entry.getKey(),entry.getValue());
				}
			}
			for(Map.Entry<String,Object> entry: _data.entrySet()){
				String key = entry.getKey();
				if(key!=null && key.length()>0){
					char prefix = key.charAt(0);
					if(prefix=='@')continue;
					if(prefix=='!'){
						key = key.substring(1);
						if(this._internals==null) this._internals = new ArrayList<>();
						this._internals.add(key);
					}
					this._variables.put(key, entry.getValue());
				}
				
			}
			this._variables =this._variables.readonly(true);
			if(this._internals!=null) this._internals = Collections.unmodifiableList(this._internals);
		}
		return this._variables;
	}
	public Object variables(String name) throws Exception{
		return this.variables().get(name);
	}
	
	private Map<String,List<String>> _imports;
	public Map<String,List<String>> imports(){
		if(_imports==null){
			Object value = _data.get("@imports");
			Map<String,List<String>> existed = null;
			if(this._ownDiagram!=null) existed = this._ownDiagram.imports();
			this._imports = State.mergeImExport(existed, value);
			this._data.put("@imports", this._imports);
		}
		return _imports;
	}
	
	public List<String> imports(String name){
		return StringMap.resolve(this.imports(), name);
	}
	private Map<String,List<String>> _exports;
	public Map<String,List<String>> exports(){
		if(_exports==null){
			Object value = _data.get("@exports");
			Map<String,List<String>> existed = null;
			if(this._ownDiagram!=null) existed = this._ownDiagram.exports();
			this._exports = State.mergeImExport(existed, value);
			this._data.put("@exports", this._imports);
		}
		return _exports;
	}

	private String _auto;
	public Boolean auto(){
		if(_auto==null){
			_auto = this._data.getString("@auto");
			if(_auto==null) {
				if(this.ownDiagram()!=null){
					Boolean auto = this.ownDiagram().auto();
					if(auto!=null) _auto = auto?"true":"false"; 
					else _auto = "";
				} else _auto = "";
			}
			if(!_auto.equals(""))this._data.put("@auto",this._auto);
		}
		return this._auto.equals("")?null:_auto.equals("true");
	}

	public List<String> exports(String name){
		return StringMap.resolve(this.exports(), name);
	}

	private List<String> _params;
	public List<String> params(){
		if(_imports==null){
			Object value = _data.get("@params");
			if(value!=null){
				List<String> existed = null;
				if(this._ownDiagram!=null) existed = this._ownDiagram.params();
				if(existed==null) existed = new ArrayList<>();
				else existed = new ArrayList<>(existed);

				List<String> myParams=null;
				if(value instanceof List) myParams = (List<String>)value;
				State.uniqueMergeStringList(existed, myParams);
				this._params = Collections.unmodifiableList(existed);
			}
			
			this._data.put("@params", this._params);
		}
		return _params;
	}
	

	private Map<String,Transition> _transitions;
	public Map<String,Transition> transitions() throws Exception{
		if(_transitions==null){
			this._transitions = new HashMap<>();
			Object value = _data.get("@transitions");
			if(value instanceof JSONObject) {
				
				for(Map.Entry<String,Object> entry : ((JSONObject)value).entrySet()){
					String to = entry.getKey();
					Object tranData = entry.getValue();
					if(tranData!=null){
						this._transitions.put(to,new Transition(this,to,new StringMap(tranData)));
					}
					
				}
			}
		}
		return _transitions;
	}
	public List<String> transitions(String name){
		return StringMap.resolve(this.exports(), name);
	}

	private Diagram _sub;
	public Diagram subDiagram(){
		if(this._sub==null){
			Object value = _data.get("subDiagram");
			if(value instanceof JSONObject) {
				this._sub =new Diagram(new StringMap(value),this);
			}else if(value instanceof  String){
				this._sub = new ReferenceDiagram(value.toString());
			}else this._sub = Diagram.empty();
		}
		return this._sub==Diagram.empty()?null:this._sub;
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
		return this._data;
	}

	public String jsonString() throws Exception{
		Object obj = this.jsonObject();
		return JSON.toJSONString(obj);
	}
	
	public static void resetSubDiagram(State state,Diagram subDiagram){
		state._ownDiagram = subDiagram;
	}
}
