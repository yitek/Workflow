package yitek.workflow.core.std;


import com.alibaba.fastjson.*;
import com.alibaba.fastjson.annotation.JSONField;

public class Transition {
	public Transition(){}
	public Transition(String from,String to){
		this.from = from;
		this.to = to;
	}
	public Transition(String from,String to,Object data){
		this.from = from;
		this.to = to;
		if(data instanceof JSONArray) {
			this.predicate = new Predicate((JSON)data);
		}else{
			Object value = null;
			JSONObject tdata =(JSONObject) data; 
			value = tdata.get("name");
			if(value!=null) this.name = value.toString();
			value = tdata.get("predicate");
			if(value!=null) this.predicateData = (JSON)value;
		}
	}
	String name;
	public String getName(){return this.name;}
	public Transition setName(String value) { this.name= value; return this;}

	String from;
	public String getFrom(){return this.from;}
	public Transition setFrom(String value) { this.from= value; return this;}
	String to;
	public String getTo(){return this.to;}
	public Transition setTo(String value) { this.to= value; return this;}
	@JSONField(serialize = false)
	Predicate predicate;

	@JSONField(serialize = false)
	JSON predicateData;
	@JSONField
	public Predicate getPredicate(){
		if(predicate==null && predicateData!=null){
			this.predicate = new Predicate(this.predicateData);
		}
		return this.predicate;
	}
	
	
}
