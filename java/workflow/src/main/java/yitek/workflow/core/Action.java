package yitek.workflow.core;

public interface Action {
	Boolean entry(Activity activity,Dealer dealer,StringMap inputs,DiagramBuilder subDiagramBuilder,FlowContext ctx)throws Exception;	
	Object deal(Activity activity,Dealer dealer,StringMap params,FlowContext ctx)throws Exception;
	Boolean exit(Activity activitiy,Dealer dealer,FlowContext ctx)throws Exception;

	void undo(Activity activity,FlowContext ctx) throws Exception;
}
