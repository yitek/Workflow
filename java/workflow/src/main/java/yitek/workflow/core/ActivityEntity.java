package yitek.workflow.core;

import java.nio.ByteBuffer;
import java.util.*;

import com.alibaba.fastjson.JSON;

import yitek.workflow.core.std.*;

public class ActivityEntity  {
	public ActivityEntity(){}
	private ActivityEntity(String name,Dealer dealer){
		this._id = UUID.randomUUID();
		this._status = ActivityStates.created;
		this._name = name;
		this._creatorId = dealer.id();
		this._creatorName = dealer.name();
		this._createTime = new Date();
		this._startType= StartTypes.notStart;
	}
	// flow的创建
	public ActivityEntity(String name,String version, Map<String,Object> diagramState,Dealer dealer){
		this(name,dealer);
		this._flowId = this._superId =  this._id;
		this._pathname = this._name;
		this._version = version;
		this._state = JSON.toJSONString(diagramState);
		this._startType= StartTypes.flowStart;
	}
	// start的创建
	public ActivityEntity(String name,Map<String,Object> state,Dealer dealer,ActivityEntity superEntity){
		this(name,dealer);
		this._pathname = superEntity._pathname + "." + name;
		this.state(JSON.toJSONString(state));
		this._superId = superEntity._id;
		this._flowId = superEntity._flowId;
		this._version = superEntity._version;
		this._startType= StartTypes.diagramStart;
	}
	// 下一步
	public ActivityEntity(Dealer dealer,Transition transition,ActivityEntity from) throws Exception{
		this(transition.to().name(),dealer);
		this._pathname = from._pathname + "." + this._name;
		this.state(transition.to().jsonString());
		this._transitionName = transition.name();
		this._fromId = from._id;
		this._superId = from._superId;
		this._flowId = from._flowId;
		this._version = from._version;
		this._businessId = from._businessId;
		this._billId = from._billId;
		this._taskId =from._taskId;
		this._startType= StartTypes.notStart;
	}

	UUID _id;
	public UUID id() {return this._id;}
	public ActivityEntity id(UUID value){ this._id = value;return this;}

	UUID _fromId;
	public UUID fromId() {return this._fromId;}
	public ActivityEntity fromId(UUID value){ this._fromId = value;return this;}

	UUID _superId;
	public UUID superId() {return this._superId;}
	
	public ActivityEntity superId(UUID value){ this._superId = value;return this;}

	UUID _flowId;
	public UUID flowId() {return this._flowId;}
	public ActivityEntity flowId(UUID value){ this._flowId = value;return this;}

	String _name;
	public String name() {return this._name;}
	public ActivityEntity name(String value){ this._name = value;return this;}

	String _version;
	public String version() {return this._version;}
	public ActivityEntity version(String value){ this._version = value;return this;}

	String _pathname;
	public String pathname() {return this._pathname;}
	public ActivityEntity pathname(String value){ this._pathname = value;return this;}

	String _transitionName;
	public String transitionName() {return this._transitionName;}
	public ActivityEntity transitionName(String value){ this._transitionName = value;return this;}

	String _actionName;
	public String actionName() {return this._actionName;}
	public ActivityEntity actionName(String value){ this._actionName = value;return this;}

	String _billStatus;
	public String billStatus() {return this._billStatus;}
	public ActivityEntity billStatus(String value){ this._billStatus = value;return this;}
	
	ActivityStates _status;
	public ActivityStates activityStatus() {return this._status;}
	public ActivityEntity activityStatus(ActivityStates value){ this._status = value;return this;}

	Date _createTime;
	public Date createTime() {return this._createTime;}
	public ActivityEntity createTime(Date value){ this._createTime = value;return this;}

	String _creatorId;
	public String creatorId() {return this._creatorId;}
	public ActivityEntity creatorId(String value){ this._creatorId = value;return this;}

	String _creatorName;
	public String creatorName() {return this._creatorName;}
	public ActivityEntity creatorName(String value){ this._creatorName = value;return this;}

	Date _dealTime;
	public Date dealTime() {return this._dealTime;}
	public ActivityEntity dealTime(Date value){ this._dealTime = value;return this;}

	String _dealerId;
	public String dealerId() {return this._dealerId;}
	public ActivityEntity dealerId(String value){ this._dealerId = value;return this;}

	String _dealerName;
	public String dealerName() {return this._dealerName;}
	public ActivityEntity dealerName(String value){ this._dealerName = value;return this;}

	Date _doneTime;
	public Date doneTime() {return this._doneTime;}
	public ActivityEntity doneTime(Date value){ this._dealTime = value;return this;}

	boolean _isAuto;
	public boolean isAuto() {return this._isAuto;}
	public ActivityEntity isAuto(boolean value){ this._isAuto = value;return this;}

	String _state;
	public String state() {return this._state;}
	public ActivityEntity state(String value){ this._state = value;return this;}

	String _activityType;
	public String activityType() {return this._activityType;}
	public ActivityEntity activityType(String value){ this._activityType = value;return this;}

	String _inputs;
	public String inputs() {return this._inputs;}
	public ActivityEntity inputs(String value){ this._inputs = value;return this;}

	String _params;
	public String params() {return this._params;}
	public ActivityEntity params(String value){ this._params = value;return this;}

	
	String _variables;
	public String variables() {return this._variables;}
	public ActivityEntity variables(String value){ this._variables = value;return this;}

	String _results;
	public String results() {return this._results;}
	public ActivityEntity results(String value){ this._results = value;return this;}

	String _billId;
	public String billId() {return this._billId;}
	public ActivityEntity billId(String value){ this._billId = value;return this;}

	String _businessId;
	public String businessId() {return this._businessId;}
	public ActivityEntity businessId(String value){ this._businessId = value;return this;}
	String _taskId;
	public String taskId() {return this._taskId;}
	public ActivityEntity taskId(String value){ this._taskId = value;return this;}

	Boolean _suspended;
	public Boolean suspended(){ return this._suspended;}
	public ActivityEntity suspended(Boolean value){this._suspended=value;return this;}

	int _subCount;
	public Integer subCount(){return this._subCount; }
	public ActivityEntity subCount(Integer value){this._subCount=value;return this;}

	StartTypes _startType;
	public StartTypes startType(){ return this._startType;}
	public ActivityEntity startType(StartTypes value){this._startType=value;return this;}


	List<ActivityEntity> _subordinates;
	public List<ActivityEntity> subordinates() {return this._subordinates;}
	public ActivityEntity subordinates(List<ActivityEntity> value){ this._subordinates = value;return this;}

	public static UUID Bytes2UUID(byte[] bytes) {
		return UUID.fromString(new String(bytes));
	}

	public static UUID Bytes2UUID1(byte[] bytes) {
		ByteBuffer bb = ByteBuffer.wrap(bytes);
		long firstLong = bb.getLong();
		long secondLong = bb.getLong();
		return new UUID(firstLong, secondLong);
	}

	public static byte[] UUID2Bytes(UUID uuid){
		return uuid.toString().getBytes();
	}
	
	
	public static byte[] UUID2Bytes1(UUID uuid) {
		ByteBuffer bb = ByteBuffer.wrap(new byte[16]);
		bb.putLong(uuid.getMostSignificantBits());
		bb.putLong(uuid.getLeastSignificantBits());
		return bb.array();
	}
}
