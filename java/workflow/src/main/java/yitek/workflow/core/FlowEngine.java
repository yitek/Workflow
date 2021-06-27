package yitek.workflow.core;

import java.util.UUID;

public interface FlowEngine {
	void startFlow(String stdName,String version,Dealer dealer,Object params , TriggleContext ctx)throws Exception;
	void triggle(UUID activityId,Dealer dealer,Object params , TriggleContext ctx)throws Exception;
}
