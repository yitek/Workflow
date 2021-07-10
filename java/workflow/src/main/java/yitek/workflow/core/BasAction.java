package yitek.workflow.core;

import java.util.List;

import yitek.workflow.core.std.Transition;

public class BasAction implements Action {
	public Boolean entry(Activity activity ,Dealer dealer,StringMap inputs,DiagramBuilder builder,FlowContext ctt) throws Exception{
		return true;
	}	
	public Object deal(Activity activity,Dealer dealer,StringMap params,DiagramBuilder subDiagramBuilder,FlowContext ctx) throws Exception{
		return params;
	}
	public Object transfer(Activity activity,Dealer dealer,Transition transition,FlowContext ctx)throws Exception{
		return new Object();
	}
	public Boolean exit(Activity activity,Dealer dealer,List<Activity> nextActEntities,FlowContext ctx) throws Exception{
		return true;
	}

	public void undo(Activity activity,Dealer dealer,FlowContext ctx) throws Exception{

	}
	public void reenter(Activity activity,Dealer dealer,FlowContext ctx)throws Exception{}

	private static Action _empty = new BasAction();
	public static Action empty(){
		return _empty;
	}
}
