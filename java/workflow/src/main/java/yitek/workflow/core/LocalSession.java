package yitek.workflow.core;

import java.util.*;
import java.util.function.Function;

import com.alibaba.fastjson.*;
import yitek.workflow.core.std.*;

// 工作流引擎
// 对工作流的的操作都是从这里入口的

public abstract class LocalSession  implements Session{
	public LocalSession(){}
	
	private DiagramRepository _diagramRepository;

	public DiagramRepository diagramRepository(){
		if(this._diagramRepository==null) this._diagramRepository = new FileDiagramRepository();
		return this._diagramRepository;
	}
	protected LocalSession diagramRepository(DiagramRepository value){ this._diagramRepository=value;return this;}

	private ActivityRepository _activityRepository;
	public ActivityRepository activityRepository(){
		return this._activityRepository;
	}
	protected LocalSession activityRepository(ActivityRepository value){ this._activityRepository=value;return this;}

	// 从依赖注入中获取实例
	public Object resolveInstance(String name){ return null;}
	public Action resolveAction(String actionType){ return null;}

	public <P,R> R transactional(P arg,Function<P,R> fn) throws Exception{
		return fn.apply(arg);
	}

	public void startFlow(String stdName,String version,Dealer dealer,Object params)throws Exception{
		if(version==null)version="";
		Diagram diagram = this.diagramRepository().getDiagramByName(stdName,version);
		Activity activity = new Activity(this,dealer,stdName,version,diagram);
		this.activityRepository().createActivity(activity._entity);
		this.active(activity, dealer, params);
	}
	void active(Activity activity,Dealer dealer, Object params) throws Exception{
		StringMap paramsMap = null;
		if(params instanceof Map) paramsMap = new StringMap(params);
		else {
			Object paramsObj = JSONObject.toJSON(params);
			paramsMap = new StringMap(paramsObj);
		}
		if(activity.status()== ActivityStates.created){
			if(!activity.entry(dealer, paramsMap)) return;
		}
		if(activity.status()== ActivityStates.entried || activity.status()== ActivityStates.dealed){
			if(!activity.deal(dealer, paramsMap)) return;
		}
		if(activity.status()== ActivityStates.dealed){
			activity.transfer(dealer);
		}
	}

	public void active(UUID activityId,Dealer dealer,Object params)throws Exception{
		List<ActivityEntity> entities = this.activityRepository().listLivedActivitiesById(activityId);
		HashMap<UUID,Activity> acts = new HashMap<UUID,Activity>();
		Activity curr=null;
		for(ActivityEntity entity:entities){
			
			Activity acti = new Activity(this,entity);
			acts.put(entity.id(), acti);
			if(entity.id()==activityId){
				curr = acti;
			}
		}
		for(Activity acti : acts.values()){
			UUID pid = acti._entity.superId();
			if(pid!=null) {
				Activity parent = acts.get(pid);
				acti.superActivity(parent);
			}
		}
		active(curr, dealer, params);
		
	}
	
	public Activity activity(UUID activityId) throws Exception{
		ActivityEntity entity = this.activityRepository().getActivityById(activityId);
		Activity activity = new Activity(this,entity);
		return activity;
	}

	public Activity activityByTaskId(String taskId) throws Exception{
		ActivityEntity entity = this.activityRepository().getActivityByTaskId(taskId);
		Activity activity = new Activity(this,entity);
		return activity;
	}

	public Boolean recall(UUID activityId,Dealer dealer) throws Exception{
		return this.transactional(dealer,(opper)->{
			try{
				ActivityEntity curr = this.activityRepository().getActivityById(activityId);
				List<ActivityEntity> undoActivities = Activity.undoableActivities(curr,opper, this);
				if(undoActivities==null) return false;
				return this.activityRepository().removeNextActivities(activityId);
			}catch(Exception ex){
				return false;
			}
			
		});
		
	}
}
