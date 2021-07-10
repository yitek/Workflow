package yitek.workflow.core;

import java.util.List;

import yitek.workflow.core.std.Transition;

public interface Action {
	Boolean entry(Activity activity,Dealer dealer,StringMap inputs,DiagramBuilder subDiagramBuilder,FlowContext ctx)throws Exception;	
	Object deal(Activity activity,Dealer dealer,StringMap params,DiagramBuilder subDiagramBuilder,FlowContext ctx)throws Exception;
	Object transfer(Activity activity,Dealer dealer,Transition transition,FlowContext ctx)throws Exception;;
	Boolean exit(Activity activitiy,Dealer dealer, List<Activity> nextActEntities,FlowContext ctx)throws Exception;

	void undo(Activity activity,Dealer dealer,FlowContext ctx) throws Exception;
	void reenter(Activity activity,Dealer dealer,FlowContext ctx) throws Exception;
}
