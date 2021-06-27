package yitek.workflow.core;



public class Dealer {
	public Dealer(){}
	public Dealer(String id,String name){
		this._id = id;
		this._name = name;
	}
	String _id;
	public String id(){ return this._id;}
	public Dealer id(String value) { this._id= value; return this;}
	String _name;
	public String name(){return this._name;}
	public Dealer name(String value) { this._name= value; return this;}
}
