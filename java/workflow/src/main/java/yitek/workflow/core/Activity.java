package yitek.workflow.core;

import java.util.*;

import com.alibaba.fastjson.JSON;
import com.alibaba.fastjson.JSONObject;

import yitek.workflow.core.std.*;

public class Activity {
	public Activity(){}
	UUID id;
	public UUID getId(){
		return this.entity.id;
	}
	ActivityEntity entity;

	Activity parent;
	public Activity getParent(){
		return this.parent;
	}
	Activity setParent(Activity value){
		this.parent = value; 
		if(value.subsidaries==null)value.subsidaries = new ArrayList<Activity>();
		value.subsidaries.add(value);
		return this;
	}
	List<Activity> subsidaries;
	public List<Activity> getSubsidaries(){
		return this.subsidaries==null?(this.subsidaries = new ArrayList<Activity>()):this.subsidaries;
	}
	

	public ActivityStates getStatus(){
		return this.entity.getStatus();
	}
	private Activity setStatus(ActivityStates value){this.entity.status = value;return this;}
	
	State state;
	public State getState(){
		if(state==null){
			JSONObject json = JSON.parseObject(this.entity.std);
			this.state = new State(this.entity.name,json);
		}
		return this.state;
	}

	JSONObject results;
	public JSONObject getResults(){
		if(this.results==null){
			this.results = JSON.parseObject(this.entity.results);
		}
		return this.results;
	}
	

	protected State Entry(Dealer dealer,JSONObject params,JSONObject inputs,TriggleContext ctx){
		return null;
	}

	protected Object Deal(Dealer dealer,JSONObject params,TriggleContext ctx){
		return params;
	}

	protected void Exit(Dealer dealer,TriggleContext ctx){

	}
	public Boolean Triggle(Dealer dealer,Object params,TriggleContext ctx){
		if(this.getStatus()== ActivityStates.entried || this.getStatus()== ActivityStates.dealed){
			Object rs = this.Deal(dealer, params, ctx);
			if(rs!=null){
				JSONObject results = this.results  = (JSONObject)JSON.toJSON(rs);
				JSONObject variables = JSON.parseObject(this.entity.variables);
				for(Map.Entry<String,Object> entry : results.entrySet()){
					variables.put(entry.getKey(),entry.getValue());
				}
				this.entity.variables = JSON.toJSONString(variables);
				this.entity.results = JSON.toJSONString(results);
				this.setStatus(ActivityStates.dealed);
			}
			return true;
		}
		return false;
	}
		
}
