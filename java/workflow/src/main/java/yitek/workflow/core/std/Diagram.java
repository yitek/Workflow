package yitek.workflow.core.std;

import java.util.*;
import com.alibaba.fastjson.*;

import yitek.workflow.core.*;

public class Diagram {
	private StringMap _data;
	protected Diagram(){}
	static Diagram _empty = new Diagram();
	public static Diagram empty(){return _empty;}


	public Diagram(StringMap data,State superState){
		this._data = data;
		this._superState = superState;
	}

	private State _superState;
	public State superState(){
		return this._superState;
	}
	
	private Map<String,List<String>> _imports;
	public Map<String,List<String>> imports(){
		if(_imports==null){
			Object value = _data.get("@imports");
			Map<String,List<String>> existed = null;
			if(this._superState!=null && this._superState.ownDiagram()!=null) existed = this._superState.ownDiagram().imports();
			this._imports = State.mergeImExport(existed, value);
			this._imports = Collections.unmodifiableMap(this._imports);
			if(this._imports.size()>0) this._data.put("@imports", this._imports);
			else this._data.remove("@imports");
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
			if(this._superState!=null && this._superState.ownDiagram()!=null) existed = this._superState.ownDiagram().exports();
			this._exports = State.mergeImExport(existed, value);
			this._exports = Collections.unmodifiableMap(this._exports);
			if(this.imports().size()>0) this._data.put("@exports", this._imports);
			else this._data.remove("@exports");
		}
		return _exports;
	}
	public List<String> exports(String name){
		return StringMap.resolve(this.exports(), name);
	}
	
	private StringMap _variables;
	StringMap variables() throws Exception{
		if(this._variables==null){
			this._variables = new StringMap();
			if(this._superState!=null && this._superState.ownDiagram()!=null && this._superState.ownDiagram().variables()!=null){
				Map<String,Object> superVars = this._superState.ownDiagram().variables();
				for(Map.Entry<String,Object> entry: superVars.entrySet()){
					String key = entry.getKey();
					if(key!=null && key.length()>0 && key.charAt(0)!='@' )
					this._variables.put(key, entry.getValue());
				}
			}
			Object thisVariablesObj = this._data.get("@variables");
			if(thisVariablesObj!=null && thisVariablesObj instanceof Object){
				StringMap thisVariables = new StringMap(thisVariablesObj);
				for(Map.Entry<String,Object> entry: thisVariables.entrySet()){
					String key = entry.getKey();
					if(key!=null && key.length()>0 && key.charAt(0)!='@' )
					this._variables.put(key, entry.getValue());
				}
			}
			
			this._variables =this._variables.readonly(true);
		}
		return this._variables;
	}
	public Object variables(String name) throws Exception{
		return this.variables().get(name);
	}

	
	private List<String> _params;
	public List<String> params(){
		if(_params==null){
			Object value = _data.get("@params");
			if(value!=null){
				List<String> existed = null;
				if(this.superState()!=null && this._superState.ownDiagram()!=null) existed = this._superState.ownDiagram().params();
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
	
	private List<State> _starts;
	public List<State> starts() throws Exception{
		if(_starts==null){
			this._starts = new ArrayList<State>();
			Object startNames = _data.get("@starts");
			if(startNames!=null && startNames instanceof JSONArray) {
				for(Object name : ((JSONArray)startNames)){
					State state = this.states(name.toString());
					if(state!=null) this._starts.add(state);
				}
			}else {
				Object startName = _data.get("@start");
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
	private String _actionName;
	public String actionName(){
		if(_actionName==null){
			_actionName = this._data.getString("@actionName");
			if(_actionName==null) {
				if(this.superState()!=null && this.superState().ownDiagram()!=null){
					_actionName = this.superState().ownDiagram().actionName();
				}else _actionName = "";
			}
			this._data.put("@actionName",this._actionName);
		}
		return this._actionName;
	}

	private String _auto;
	public Boolean auto(){
		if(_auto==null){
			_auto = this._data.getString("@auto");
			if(_auto==null) {
				if(this.superState()!=null && this.superState().ownDiagram()!=null){
					Boolean auto = this.superState().ownDiagram().auto();
					if(auto!=null) _auto = auto?"true":"false"; 
					else _auto = "";
				} else _auto = "";
			}
			if(!_auto.equals(""))this._data.put("@auto",this._auto);
		}
		return this._auto.equals("")?null:_auto.equals("true");
	}

	private Map<String,State> _states;
	public Map<String,State> states(){
		if(_states==null){
			this._states = new HashMap<String,State>();
			Object value = _data.get("@states");
			if(value instanceof JSONObject) {
				
				for(Map.Entry<String,Object> entry : ((JSONObject)value).entrySet()){
					String name = entry.getKey();
					Object stateObj = entry.getValue();
					if(stateObj!=null){
						this._states.put(name,new State(name,new StringMap(stateObj),this));
					}
					
				}
			}
			this._states = Collections.unmodifiableMap(this._states);
		}
		return _states;
	}
	public State states(String name){
		 return StringMap.resolve(this.states(), name);
	}

	public State findStateByBillStatus(String billStatus){
		if(billStatus==null) return null;
		for(Map.Entry<String,State> entry: this.states().entrySet()){
			State state = entry.getValue();
			if(state.billStatus().equals(billStatus)) return state;
		}
		return null;
	}
	
	public StringMap jsonObject() throws Exception{
		
		return this._data;
	}

	public String jsonString() throws Exception{
		return JSON.toJSONString(this._data);
	}
}
