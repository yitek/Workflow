package yitek.workflow.core;

import java.util.*;
import java.util.stream.Stream;

import com.alibaba.fastjson.JSON;
import com.fasterxml.jackson.annotation.JsonTypeInfo.Id;

import org.springframework.beans.factory.annotation.*;
import org.springframework.stereotype.*;

import yitek.workflow.core.std.*;

// 工作流引擎
// 对工作流的的操作都是从这里入口的
@Service
public class FlowEngine {
	public FlowEngine(){}

	@Autowired
	STDRepository stdRepository;

	public STDRepository getSTDRepository(){
		if(this.stdRepository==null) this.stdRepository = new FileSTDRepository();
		return this.stdRepository;
	}
	public FlowEngine setSTDRepository(STDRepository value){ this.stdRepository=value;return this;}

	@Autowired
	ActivityRepository acitivityRepository;
	public ActivityRepository getActivityRepository(){
		return this.acitivityRepository;
	}
	public FlowEngine setActivityRepository(ActivityRepository value){ this.acitivityRepository=value;return this;}

	public void startFlow(String stdName,Dealer dealer,Object params , TriggleContext ctx)throws Exception{
		State state = this.getSTDRepository().GetStateByName(stdName);
		System.out.println(JSON.toJSONString(state));
	}

	public void Triggle(UUID activityId,Dealer dealer,Object params , TriggleContext ctx)throws Exception{
		List<ActivityEntity> entities = this.getActivityRepository().ListLivedActivitiesById(activityId);
		HashMap<UUID,Activity> acts = new HashMap<UUID,Activity>();
		Activity curr=null;
		for(ActivityEntity entity:entities){
			String activityType = entity.getActivityType();
			if(activityType==null || activityType.equals("")) activityType="yitek.workflow.core.Activity";
			Activity acti = (Activity)ctx.resolveInstance(activityType);
			acti.entity = entity;
			acts.put(entity.getId(), acti);
			if(entity.id==activityId){
				curr = acti;
			}
		}
		for(Activity acti : acts.values()){
			UUID pid = acti.entity.parentId;
			if(pid!=null) {
				Activity parent = acts.get(pid);
				acti.setParent(parent);
			}
		}
		
	}
	public void Triggle(Activity activity,Dealer dealer,Object params , TriggleContext ctx)throws Exception{
		if(activity.Triggle(dealer, params, ctx)){
			this.acitivityRepository.SaveActivity(activity.entity);
		}
		if(activity.getStatus()==ActivityStates.dealed){
			Map<String,Transition> trans = activity.state.getTransitions();
			if(trans!=null && trans.size()>0){
				for(Map.Entry<String,Transition> entry : trans.entrySet()){
					String tranResult = entry.getValue().getPredicate().Eval(null, null)
				}
			}
		}

	}
}
