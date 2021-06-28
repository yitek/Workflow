package yitek.workflow.core.std;
import com.alibaba.fastjson.*;

public class Transition {
	public Transition(State from,String to,Object data) throws Exception{
		this._from = from;
		
		this._toName = to;
		if(data instanceof JSONArray) {
			this._predicate = new Predicate(data);
		}else if(data instanceof JSONObject){
			Object value;
			JSONObject jsonData =(JSONObject) data;
			value = jsonData.get("name");
			if(value!=null) this._name = value.toString();
			value = jsonData.get("triggleUrl");
			if(value!=null) this._triggleUrl = value.toString();
			value = jsonData.get("predicate");
			if(value!=null) this._predicateData = (JSON)value;
		}else throw new Exception("不正确的Transition");
	}

	String _name;
	public String name(){return this._name;}
	//public Transition setName(String value) { this._name= value; return this;}

	String _triggleUrl;
	public String triggleUrl(){
		return this._triggleUrl;
	}

	State _from;
	public State from(){return this._from;}
	//public Transition setFrom(String value) { this._from= value; return this;}
	State _to;
	String _toName;
	public State to() throws Exception{
		if(_to==null){
			if(_toName.equals("") || _toName.equals("<END>")){
				this._to = State.empty();
			}else {
				Diagram diagram = this._from.ownDiagram();
				this._to = diagram.states(this._toName);
				if(this._to==null) throw new Exception("未能找到"+_toName+"的节点");
			}

		}
		
		return this._to;
	}
	//public Transition setTo(String value) { this._to= value; return this;}

	Predicate _predicate;


	JSON _predicateData;
	public Predicate predicate(){
		if(_predicate==null && _predicateData!=null){
			this._predicate = new Predicate(this._predicateData);
		}
		return this._predicate;
	}
	
	public JSONObject  jsonObject(){
		JSONObject ret = new JSONObject();
		if(this.name()!=null && !this.name().equals("")){
			ret.put("name", this.name());
		}
		if(this.from()!=null){
			ret.put("from", this.from().name());
		}
		if(this._toName!=null){
			ret.put("to", this._toName);
		}
		if(this.triggleUrl()!=null){
			ret.put("triggleUrl", this.triggleUrl());
		}
		if(this._predicateData!=null){
			ret.put("predicate",this._predicateData);
		}
		return ret;
	}

	public String jsonString(){
		return JSON.toJSONString(this.jsonObject());
	}
}
