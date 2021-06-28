package yitek.workflow.core;


public class BasAction implements Action {
	public Boolean entry(Activity activity ,Dealer dealer,StringMap inputs,DiagramBuilder builder,FlowContext ctt) throws Exception{
		return true;
	}	
	public Object deal(Activity activity,Dealer dealer,StringMap params,FlowContext ctx) throws Exception{
		return params;
	}
	public Boolean exit(Activity activity,Dealer dealer,FlowContext ctx) throws Exception{
		return true;
	}

	public void undo(Activity activity,FlowContext ctx) throws Exception{

	}
}
