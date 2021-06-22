package yitek.workflow.web.controllers;

import java.util.*;
//import org.springframework.beans.factory.annotation.*;
import org.springframework.web.bind.annotation.*;

import yitek.workflow.core.*;

import yitek.workflow.web.models.*;
@RequestMapping("user")
@RestController
public class UserController {
 
 
 
	@GetMapping("/list/all")
	public List<User> listAll() throws Exception{
		FlowEngine flow = new FlowEngine();
		flow.startFlow("test", null,null,null);
		List<User> users = new ArrayList<User>();
		User user = new User();
		user.setId("1");user.setName("hello");
		users.add(user);
		return users;
	}
}