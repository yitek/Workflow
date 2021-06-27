package yitek.workflow.core;

import java.util.UUID;

import com.alibaba.fastjson.JSONObject;

public interface BusinessInfo {
	UUID activityId();
	BusinessInfo superBusiness();
	BusinessInfo rootBusiness();
	JSONObject variables();
	String businessId();
	BusinessInfo businessId(String value);
	String taskId();
	BusinessInfo taskId(String value);
	String billId();
	BusinessInfo billId(String value);
}
