package yitek.workflow.core.std;
import com.alibaba.fastjson.*;

public class Transition {
	public Transition(){}
	
	public Transition(State from,String to,Object data) throws Exception{
		this._from = from;
		Diagram diagram = from.ownDiagram();
		this._to = diagram.states().get(to);
		if(this._to==null) throw new Exception("未能找到"+to+"的节点");
		if(data instanceof JSONArray) {
			this._predicate = new Predicate((JSON)data);
		}else if(data instanceof JSONObject){
			Object value = null;
			JSONObject tdata =(JSONObject) data; 
			value = tdata.get("name");
			if(value!=null) this._name = value.toString();
			value = tdata.get("predicate");
			if(value!=null) this._predicateData = (JSON)value;
		}else throw new Exception("不正确的Transition");
	}

	String _name;
	public String name(){return this._name;}
	//public Transition setName(String value) { this._name= value; return this;}

	State _from;
	public State from(){return this._from;}
	//public Transition setFrom(String value) { this._from= value; return this;}
	State _to;
	public State to(){return this._to;}
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
		if(this.to()!=null){
			ret.put("to", this.to().name());
		}
		if(this._predicateData!=null){
			ret.put("predicate",this._predicateData);
		}
		return ret;
	}

	public String jsonString(){
		return this.jsonObject().toString();
	}
}
