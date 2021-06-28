package yitek.workflow.core.std;

import java.util.*;
import com.alibaba.fastjson.*;

import yitek.workflow.core.*;

public class Diagram {
	JSONObject data;
	protected Diagram(){}
	static Diagram _empty = new Diagram();
	public static Diagram empty(){return _empty;}


	public Diagram(JSONObject data,State superState){
		this.data = data;
		this._superState = superState;
	}

	State _superState;
	public State superState(){
		return this._superState;
	}
	
	Map<String,List<String>> _imports;
	public Map<String,List<String>> imports(){
		if(_imports==null){
			Object value = data.get("imports");
			Map<String,List<String>> existed = null;
			if(this._superState!=null && this._superState._ownDiagram!=null) existed = this._superState._ownDiagram.imports();
			this._imports = State.mergeImExport(existed, value);
			this._imports = Collections.unmodifiableMap(this._imports);
		}
		return _imports;
	}
	public List<String> imports(String name){
		return StringMap.resolve(this.imports(), name);
	}
	Map<String,List<String>> _exports;
	public Map<String,List<String>> exports(){
		if(_exports==null){
			Object value = data.get("exports");
			Map<String,List<String>> existed = null;
			if(this._superState!=null && this._superState._ownDiagram!=null) existed = this._superState._ownDiagram.imports();
			this._exports = State.mergeImExport(existed, value);
			this._exports = Collections.unmodifiableMap(this._exports);
		}
		return _exports;
	}
	public List<String> exports(String name){
		return StringMap.resolve(this.exports(), name);
	}
	StringMap _variables;
	public StringMap variables(){
		if(this._variables==null){
			StringMap vars = new StringMap();
			if(this._superState!=null ){
				Map<String,Object> superVars = this._superState.variables();
				for(Map.Entry<String,Object> entry: superVars.entrySet()){
					vars.put(entry.getKey(), entry.getValue());
				}
			}
			Object meVals = this.data.get("variables");
			if(meVals instanceof JSONObject){
				for(Map.Entry<String,Object> entry: ((JSONObject)meVals).entrySet()){
					vars.put(entry.getKey(), entry.getValue());
				}
			}
			this._variables = vars.readonly(true);
		}
		return this._variables;
	}
	public Object variables(String name){
		return this.variables().get(name);
	}
	
	List<State> _starts;
	public List<State> starts() throws Exception{
		if(_starts==null){
			this._starts = new ArrayList<State>();
			Object startNames = data.get("starts");
			if(startNames!=null && startNames instanceof JSONArray) {
				for(Object name : ((JSONArray)startNames)){
					State state = this.states(name.toString());
					if(state!=null) this._starts.add(state);
				}
			}else {
				Object startName = data.get("start");
				if(startName!=null){
					State state = this.states(startName.toString());
					if(state!=null) this._starts.add(state);
				}
			}
			if(this._starts.size()==0){
				throw new Exception("没有找到开始节点");
			}
			this._starts = Collections.unmodifiableList(this._starts);
		}
		return this._starts;

	}
	String _actionType;
	public String actionType(){
		if(_actionType==null){
			_actionType = this.data.getString("actionType");
			if(_actionType==null) {
				if(this.superState()!=null){
					_actionType = this.superState().actionType();
				}else _actionType = "";
			}
		}
		return this._actionType;
	}

	Map<String,State> _states;
	public Map<String,State> states(){
		if(_states==null){
			this._states = new HashMap<String,State>();
			Object value = data.get("states");
			if(value instanceof JSONObject) {
				
				for(Map.Entry<String,Object> entry : ((JSONObject)value).entrySet()){
					String name = entry.getKey();
					this._states.put(name,new State(name,(JSONObject)entry.getValue(),this));
				}
			}
			this._states = Collections.unmodifiableMap(this._states);
		}
		return _states;
	}
	public State states(String name){
		 return StringMap.resolve(this.states(), name);
	}
	
	public StringMap jsonObject() throws Exception{
		StringMap obj = new StringMap();
		if(!this.actionType().equals(""))obj.put("actionType",this.actionType());
		if(this.imports()!=null)obj.put("imports",this.imports() );
		if(this.exports()!=null) obj.put("exports", this.exports());
		if(this.starts()!=null) {
			List<String> startNames = new ArrayList<String>();
			for(State state :this.starts()) startNames.add(state.name());
			obj.put("starts", startNames);
		}
		if(this.states()!=null) {
			StringMap states = new StringMap();
			for(Map.Entry<String,State> entry:this.states().entrySet()){
				states.put(entry.getKey(), entry.getValue().jsonObject());
			}
		}
		return obj;
	}

	public String jsonString() throws Exception{
		return JSON.toJSONString(this.jsonObject());
	}
}
