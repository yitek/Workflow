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

	//不必要的字段
	//即使要在同一个数据库里放许多租户
	//租户与账号也不是同一个东西，租户信息应该放在其他地方
	// 账号应该是全局的
	Long _orgId;
	public Long orgId(){return this._orgId;}
	public Dealer orgId(Long value) { this._orgId= value; return this;}
}
