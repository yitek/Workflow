package yitek.workflow.core;

import java.util.*;
import com.alibaba.fastjson.*;

import org.springframework.beans.factory.annotation.*;
import org.springframework.stereotype.*;

import yitek.workflow.core.std.*;

// 工作流引擎
// 对工作流的的操作都是从这里入口的
@Service
public class LocalFlowEngine  implements FlowEngine{
	public LocalFlowEngine(){}
	public LocalFlowEngine(ActivityRepository activityRepository){
		this._acitivityRepository = activityRepository;
	}

	@Autowired
	DiagramRepository _diagramRepository;

	public DiagramRepository diagramRepository(){
		if(this._diagramRepository==null) this._diagramRepository = new FileDiagramRepository();
		return this._diagramRepository;
	}
	public LocalFlowEngine diagramRepository(DiagramRepository value){ this._diagramRepository=value;return this;}

	@Autowired
	ActivityRepository _acitivityRepository;
	public ActivityRepository activityRepository(){
		return this._acitivityRepository;
	}
	public LocalFlowEngine activityRepository(ActivityRepository value){ this._acitivityRepository=value;return this;}

	public void startFlow(String stdName,String version,Dealer dealer,Object params , TriggleContext ctx)throws Exception{
		Diagram diagram = this.diagramRepository().getDiagramByName(stdName,version);
		Activity activity = new Activity(this,dealer,stdName,version,diagram);
		this._acitivityRepository.createActivity(activity._entity, ctx);
		this.triggle(activity, dealer, params, ctx);
		//System.out.println(JSON.toJSONString(state));
	}
	public void triggle(Activity activity,Dealer dealer, Object params,TriggleContext ctx) throws Exception{
		JSONObject paramsJSON = null;
		if(params instanceof JSONObject) paramsJSON = (JSONObject)params;
		else {
			Object paramsObj = JSONObject.toJSON(params);
			if(paramsObj instanceof JSONObject) paramsJSON = (JSONObject)paramsObj;
		}
		if(activity.status()== ActivityStates.created){
			if(!activity.entry(dealer, paramsJSON, activity, ctx)) return;
		}
		if(activity.status()== ActivityStates.entried || activity.status()== ActivityStates.dealed){
			if(!activity.deal(dealer, paramsJSON, ctx)) return;
		}
		if(activity.status()== ActivityStates.dealed){
			activity.transfer(dealer, ctx);
		}
	}

	public void triggle(UUID activityId,Dealer dealer,Object params , TriggleContext ctx)throws Exception{
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
		triggle(curr, dealer, params, ctx);
		
	}
}
