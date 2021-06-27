package yitek.workflow.core.std;

import java.util.*;
import com.alibaba.fastjson.*;

public class Diagram {
	JSONObject data;
	Diagram(){}
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
			if(this._superState!=null && this._superState._diagram!=null) existed = this._superState._diagram.imports();
			this._imports = State.mergeImExport(existed, value);
		}
		return _imports;
	}
	Map<String,List<String>> _exports;
	public Map<String,List<String>> exports(){
		if(_exports==null){
			Object value = data.get("exports");
			Map<String,List<String>> existed = null;
			if(this._superState!=null && this._superState._diagram!=null) existed = this._superState._diagram.imports();
			this._exports = State.mergeImExport(existed, value);
		}
		return _exports;
	}
	Map<String,Object> _variables;
	public Map<String,Object> variables(){
		if(this._variables==null){
			Map<String,Object> vars = new HashMap<String,Object>();
			if(this._superState!=null && this._superState._diagram!=null){
				Map<String,Object> superVars = this._superState._diagram.variables();
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
			this._variables = Collections.unmodifiableMap(vars);
		}
		return this._variables;
	}
	String _start;
	public String start(){
		if(_start==null){
			Object value = data.get("start");
			if(value!=null) this._start = value.toString();
			else this._start = "";
		}
		return this._start;
	}
	String[] _starts;
	public String[] starts(){
		if(_starts==null){
			Object value = data.get("starts");
			if(value!=null){
				this._starts = ((JSONArray)value).toArray(this._starts);
			}
			else this._starts = new String[0];
		}
		return this._starts;

	}

	Map<String,State> _states;
	public Map<String,State> states(){
		if(_states==null){
			this._states = new HashMap<String,State>();
			Object value = data.get("states");
			if(value instanceof JSONObject) {
				
				for(Map.Entry<String,Object> entry : ((JSONObject)value).entrySet()){
					String name = entry.getKey();
					this._states.put(name,new State(name,(JSONObject)entry.getValue()));
				}
			}
		}
		return _states;
	}
	
	public JSONObject jsonObject(){
		JSONObject obj = new JSONObject();
		if(this.imports()!=null)obj.put("imports",this.imports() );
		if(this.exports()!=null) obj.put("exports", this.exports());
		if(this.start()!=null) obj.put("start", this.start());
		if(this.starts()!=null) obj.put("starts",this.starts());
		if(this.states()!=null) {
			Map<String, JSONObject> states = new HashMap<String,JSONObject>();
			for(Map.Entry<String,State> entry:this.states().entrySet()){
				states.put(entry.getKey(), entry.getValue().jsonObject());
			}
		}
		return obj;
	}

	public String jsonString(){
		return JSON.toJSONString(this.jsonObject());
	}
}
