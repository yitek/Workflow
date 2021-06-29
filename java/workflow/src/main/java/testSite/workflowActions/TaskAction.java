package testSite.workflowActions;

import com.alibaba.fastjson.JSONObject;

import org.springframework.beans.factory.annotation.*;
import org.springframework.stereotype.*;

import testSite.models.*;
import testSite.repositories.*;
import yitek.workflow.core.*;


@Service
public class TaskAction extends BasAction {
	@Autowired
	TaskRepository _taskRepository;
	public TaskRepository taskRepository(){return _taskRepository;}
	TaskAction taskRepository(TaskRepository value){this._taskRepository=value;return this;}

	protected Dealer resolveTaskDealer(Activity activity ,Dealer dealer,StringMap inputs,FlowContext ctx){
		Object taskDealerIdObject = null;
		if(inputs!=null){
			taskDealerIdObject = inputs.get("nextDealerId");
			if(taskDealerIdObject!=null)return new Dealer(inputs.get("nextDealerId").toString(),inputs.get("nextDealerName").toString());
		}
		taskDealerIdObject = activity.variables("nextDealerId");
		if(taskDealerIdObject!=null){
			return new Dealer(taskDealerIdObject.toString(),activity.variables("nextDealerName").toString());

		}
		return null;
	}
	@Override
	public Boolean entry(Activity activity ,Dealer dealer,StringMap inputs,DiagramBuilder builder,FlowContext ctx) throws Exception{
		Dealer taskDealer = resolveTaskDealer(activity,dealer,inputs,ctx);
		if(taskDealer!=null){
			Task task = new Task();
			task.setTitle(inputs.getString("taskTitle"));
			task.setActivityId(activity.id().toString());
			task.setDealerId(taskDealer.id());
			task.setDealerName(taskDealer.name());
			task.setStatus(0);
			Integer taskId= this._taskRepository.insertUseGeneratedKeys(task);
			activity.taskId(taskId.toString());
		}
		
		return true;
	}	

	@Override
	public Object deal(Activity activity,Dealer dealer,StringMap params,DiagramBuilder builder,FlowContext ctx){
		return params;
	}

	@Override
	public Boolean exit(Activity activity,Dealer dealer,FlowContext ctx) throws Exception{
		Task task = this._taskRepository.selectByPrimaryKey(activity.taskId());
		if(!task.getDealerId().equals(dealer.id())) throw new Exception("处理人与任务中指定的人不匹配");
		task.setStatus(1);
		return true;
	}
}
