package yitek.workflow.core;

import java.util.*;

import yitek.workflow.core.std.*;


public class Navigation {
	private LocalSession _session;
	public Navigation(LocalSession session,Dealer dealer,Activity from, Transition tran) throws Exception{
		this._session = session;
		this._from = from;
		Activity to = this._to = new Activity(tran.to(),dealer,from.superActivity());
		this._entity = new NavigationEntity(){{
			id(UUID.randomUUID());
			name(tran.name());
			fromId(from.id());
			toId(to.id());
			ownerId(from.superActivity().id());
			flowId(from.flowId());
			transition(tran.jsonString());
			createTime(new Date());
			creatorId(dealer.id());
			creatorName(dealer.name());
			
		}};
	}
	public Navigation(NavigationEntity entity){
		this._entity = entity;
	}
	NavigationEntity _entity;
	Activity _from;
	public Activity from() throws Exception{
		if(this._from==null && this._entity!=null && this._entity.fromId()!=null){
			this._from = this._session.activity(this._entity.fromId());
		}
		return this._from;
	}

	Activity _to;
	public Activity to() throws Exception{
		if(this._to==null && this._entity!=null && this._entity.toId()!=null){
			this._to = this._session.activity(this._entity.toId());
		}
		return this._from;
	}
	StringMap _data;
	StringMap variables() throws Exception{
		if(this._data==null && this._entity!=null && this._entity.transition()!=null){
			this._data = new StringMap(this._entity.transition());			
		}
		return this._data;
	}

	public Object get(String key) throws Exception{
		return this.variables().get(key);
	}
	public Object getString(String key) throws Exception{
		return this.variables().getString(key);
	}

	public Object getString(String key,String dft) throws Exception{
		return this.variables().getString(key,dft);
	}
}
