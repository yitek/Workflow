package yitek.workflow.core;

import java.util.UUID;

import com.alibaba.fastjson.JSONObject;

public class BusinessInfoBuilder implements BusinessInfo {
	private Activity _activity;
	private BusinessInfo _superInfo;
	private BusinessInfo _rootInfo;

	public BusinessInfoBuilder(Activity activity){
		this._activity = activity;
	}

	public String billId(){ return this._activity._entity.billId();}
	public BusinessInfo billId(String value){this._activity._entity.billId(value);return this;}
	public String businessId(){ return this._activity._entity.businessId();}
	public BusinessInfo businessId(String value){this._activity._entity.businessId(value);return this;}
	public String taskId(){ return this._activity._entity.taskId();}
	public BusinessInfo taskId(String value){this._activity._entity.taskId(value);return this;}

	public UUID activityId(){
		return this._activity.id();
	}
	public BusinessInfo superBusiness(){
		if(this._superInfo==null && this._activity.superActivity()!=null){
			this._superInfo = new BusinessInfoBuilder(this._activity.superActivity());
		}
		return this._superInfo;
	}
	public BusinessInfo rootBusiness(){
		if(this._rootInfo==null && this._activity.flow()!=null){
			this._rootInfo = new BusinessInfoBuilder(this._activity.flow());
		}
		return this._superInfo;
	}
	public JSONObject variables(){
		return this._activity.variables();
	}
}
