package yitek.workflow.core;

import java.util.*;

public interface Session extends FlowContext {
	ActivityRepository activityRepository();
	
	// 从依赖注入中获取实例
	Object resolveInstance(String name);
	Action resolveAction(String actionType);
	Activity startFlow(String stdName,String version,Dealer dealer,Object inputs,Object bill,List<String> starts)throws Exception;
	Activity startFlow(String stdName,String version,Dealer dealer,Object params )throws Exception;
	Activity startFlow(String stdName,String version,Dealer dealer,Object inputs,Object bill)throws Exception;
	Activity active(UUID activityId,Dealer dealer,Object params)throws Exception;
	Activity active(UUID activityId,Dealer dealer,Object params,Object bill,Object task)throws Exception;

	Boolean recall(UUID activityId,Dealer dealer)throws Exception;

	Activity activity(UUID activityId) throws Exception;

	Activity activityByTaskId(String taskId) throws Exception;
}
