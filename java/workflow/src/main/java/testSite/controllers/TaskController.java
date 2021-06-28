package testSite.controllers;

import java.util.*;

import org.springframework.beans.factory.annotation.*;
import org.springframework.web.bind.annotation.*;

import testSite.models.*;
import testSite.repositories.TaskRepository;
import yitek.workflow.core.*;

@RestController
@RequestMapping("task")
public class TaskController {
	@Autowired
	private Session _session;

	@Autowired
	private TaskRepository _taskRepository;
 
	@GetMapping("/list/all")
	public List<TaskActionParams> listAll() throws Exception{
		
		Dealer dealer = new Dealer("1","yiy");
		this._session.startFlow("test",null,dealer,null);
		List<TaskActionParams> users = new ArrayList<TaskActionParams>();
		return users;
	}

	@PostMapping("add")
	public Task add(@RequestBody Task task) throws Exception{
		//Task task = (Task)JSON.parseObject(taskText, Task.class);
		Integer id = this._taskRepository.insert(task);
		task.setId(id);
		return task;
	}
	@PostMapping("start")
	public void start(@RequestBody TaskActionParams taskInfo) throws Exception{
		Dealer dealer = new Dealer("1","yiy");
		this._session.startFlow("test",null,dealer,taskInfo);
	}

	@PostMapping("submit")
	public Task submit(@RequestBody TaskActionParams taskInfo) throws Exception{
		Task task = this._taskRepository.selectByPrimaryKey(taskInfo.getTaskId());
		Dealer dealer = new Dealer(taskInfo.getDealerId(),taskInfo.getDealerName());
		this._session.active(UUID.fromString(task.getActivityId()), dealer, taskInfo);
		
		return null;
	}

	@PostMapping("recall")
	public Task recall(@RequestBody TaskActionParams taskInfo) throws Exception{
		Dealer dealer = new Dealer(taskInfo.getDealerId(),taskInfo.getDealerName());
		this._session.recall(taskInfo.getActivityId(), dealer);
		return null;
	}
}