package yitek.workflow.core.std;

import com.alibaba.fastjson.*;

import yitek.workflow.core.StringMap;

public class Transition {
	StringMap _data;
	public Transition(State from,String to,StringMap data) throws Exception{
		this._from = from;
		this._toName = to;
		this._data = data;
	}

	String _name;
	public String name(){
		if(this._name==null){
			this._name = this._data.getString("@name");
		}
		return this._name;
	}
	//public Transition setName(String value) { this._name= value; return this;}

	

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
		
		return this._to==State.empty()?null:this._to;
	}
	//public Transition setTo(String value) { this._to= value; return this;}

	Predicate _predicate;
;
	public Predicate predicate(){
		if(_predicate==null){
			Object value = StringMap.resolve(this._data,"@predicate");
			if(value==null) this._predicate = Predicate.empty();
			else this._predicate = new Predicate(value);
		}
		return this._predicate;
	}

	public Object get(String key){
		return this._data.get(key);
	}
	public String getString(String key){
		return this._data.getString(key);
	}

	public Object getString(String key,String dft){
		return this._data.getString(key,dft);
	}
	
	public StringMap jsonObject() throws Exception{
		return this._data;
	}

	public String jsonString() throws Exception{
		return JSON.toJSONString(this._data);
	}
}
