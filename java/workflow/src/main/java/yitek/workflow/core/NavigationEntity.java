package yitek.workflow.core;

import java.util.Date;
import java.util.UUID;

public class NavigationEntity {
	UUID _id;
	public UUID id() {return this._id;}
	public NavigationEntity id(UUID value){ this._id = value;return this;}

	UUID _fromId;
	public UUID fromId() {return this._fromId;}
	public NavigationEntity fromId(UUID value){ this._fromId = value;return this;}

	UUID _toId;
	public UUID toId() {return this._toId;}
	public NavigationEntity toId(UUID value){ this._toId = value;return this;}

	UUID _ownerId;
	public UUID ownerId() {return this._ownerId;}
	
	public NavigationEntity ownerId(UUID value){ this._ownerId = value;return this;}

	UUID _flowId;
	public UUID flowId() {return this._flowId;}
	public NavigationEntity flowId(UUID value){ this._flowId = value;return this;}

	String _name;
	public String name() {return this._name;}
	public NavigationEntity name(String value){ this._name = value;return this;}

	String _transition;
	public String transition() {return this._transition;}
	public NavigationEntity transition(String value){ this._transition = value;return this;}

	Date _createTime;
	public Date createTime() {return this._createTime;}
	public NavigationEntity createTime(Date value){ this._createTime = value;return this;}

	String _creatorId;
	public String creatorId() {return this._creatorId;}
	public NavigationEntity creatorId(String value){ this._creatorId = value;return this;}

	String _creatorName;
	public String creatorName() {return this._creatorName;}
	public NavigationEntity creatorName(String value){ this._creatorName = value;return this;}
}
