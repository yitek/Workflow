package testSite.models;

import com.alibaba.fastjson.JSONObject;

import lombok.Data;

import java.util.UUID;

@Data
public class TaskActionParams {
	public TaskActionParams(){}
	int taskId;
	UUID activityId;
	String actionName;
	String billStatus;
	String dealerId;
	String dealerName;
	JSONObject taskDetails;
}
