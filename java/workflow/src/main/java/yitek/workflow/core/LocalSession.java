package yitek.workflow.core;

import java.util.*;
import java.util.function.Function;

import com.alibaba.fastjson.JSON;

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

	public <P,R> R transactional(P arg,Function<P,R> fn){
		return fn.apply(arg);
	}

	public Activity startFlow(String stdName,String version,Dealer dealer,Object inputs)throws Exception{
		return startFlow(stdName,version,dealer,inputs,null);
		
	}
	public Activity startFlow(String stdName,String version,Dealer dealer,Object inputs,Object bill)throws Exception{
		return startFlow(stdName, version, dealer, inputs,bill,null);
	}
	public Activity startFlow(String stdName,String version,Dealer dealer,Object inputs,Object bill,List<String> starts)throws Exception{
		try{
			if(version==null)version="";
			Diagram diagram = this.diagramRepository().getDiagramByName(stdName,version);
			Activity activity = new Activity(this,dealer,stdName,version,diagram,inputs,starts);
			activity.bill(bill);
			this.activityRepository().createActivity(activity._entity);
			return this.active(activity, dealer, inputs);
		}finally{
			this.activityRepository().dispose();
		}
		
	}
	Activity active(Activity activity,Dealer dealer, Object params) throws Exception{
		try{
			StringMap paramsMap = null;
			if(activity.activityStatus()== ActivityStates.created){
				if(!activity.entry(dealer, activity.inputs())) return activity;
				boolean auto = activity.state().auto()!=null && activity.state().auto();
				if(!auto && activity.state().subDiagram()==null){
					return activity;
				}
				activity._entity.isAuto(true);
			}
			if(activity.activityStatus()== ActivityStates.entried || activity.activityStatus()== ActivityStates.dealed){
				if(params != null) paramsMap = new StringMap(params);
				if(!activity.deal(dealer, paramsMap)) return activity;
			}
			if(activity.activityStatus()== ActivityStates.dealed){
				activity.transfer(dealer);
			}
			return activity;
		}finally{
			this.activityRepository().dispose();
		}
		
		
	}

	public Activity active(UUID activityId,Dealer dealer,Object params,Object bill,Object task)throws Exception{
		try{
			List<ActivityEntity> entities = this.activityRepository().listLivedActivitiesById(activityId);
			if(entities==null || entities.size()==0) throw new Exception("该Activity处于不能激活状态(错误，创建或已结束)");
			HashMap<UUID,Activity> acts = new HashMap<>();
			Activity curr=null;
			for(ActivityEntity entity:entities){
				
				Activity acti = new Activity(this,entity);
				acts.put(entity.id(), acti);
				if(entity.id().equals(activityId)){
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
			if(curr==null) throw new Exception("无法找到活动");
			curr.bill(bill).task(task);
			active(curr, dealer, params);
			return curr;
		}finally{
			this.activityRepository().dispose();
		}
		
	}

	public Activity active(UUID activityId,Dealer dealer,Object params)throws Exception{
		return this.active(activityId,dealer,params,null,null);
	}
	
	public Activity activity(UUID activityId) throws Exception{
		ActivityEntity entity = this.activityRepository().getActivityById(activityId);
		return new Activity(this,entity);
	}

	public Activity activityByTaskId(String taskId) throws Exception{
		ActivityEntity entity = this.activityRepository().getActivityByTaskId(taskId);
		return new Activity(this,entity);
	}

	public Boolean recall(UUID activityId,Dealer dealer) throws Exception {
		ActivityEntity entity = this.activityRepository().getActivityById(activityId);
		Activity activity =  new Activity(this,entity);
		activity.recall(dealer);
		return true;
	}
}
