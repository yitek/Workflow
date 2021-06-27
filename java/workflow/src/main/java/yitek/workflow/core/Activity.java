package yitek.workflow.core;

import java.util.*;

import com.alibaba.fastjson.JSON;
import com.alibaba.fastjson.JSONObject;

import yitek.workflow.core.std.*;

public class Activity {
	// Flow创建
	public Activity(LocalFlowEngine engine,Dealer dealer,String name,String version,Diagram diagram){
		this._engine = engine;
		JSONObject state = new JSONObject();
		state.put("name", name);
		state.put("sub", diagram.jsonObject());
		this._entity = new ActivityEntity(name,version,state,dealer);
	}	

	public Activity(String name,Dealer dealer,Activity superActivity){
		State state = superActivity.state().subDiagram().states().get(name);
		this._entity = new ActivityEntity(name,state.jsonObject(),dealer,superActivity._entity);
		this._state = state;
		this.superActivity(superActivity);
		this._engine = superActivity.engine();
	}
	public Activity(Dealer dealer,Transition transition,Activity fromActivity){
		State state = transition.to();
		this._entity = new ActivityEntity(transition.to().name(),state.jsonObject(),dealer,fromActivity._entity);
		this._state = state;
		this.superActivity(fromActivity.superActivity());
		this._engine = fromActivity.engine();
	}

	public Activity(LocalFlowEngine engine,ActivityEntity entity){
		this._entity = entity;
		this._engine = engine;
	}
	
	LocalFlowEngine _engine;
	public LocalFlowEngine engine(){
		return this._engine;
	}
	UUID _id;
	public UUID id(){
		return this._entity._id;
	}
	public UUID activityId(){
		return this._entity._id;
	}
	ActivityEntity _entity;


	Activity _superActivity;
	public Activity superActivity(){
		return this._superActivity;
	}
	
	Activity superActivity(Activity value){
		if(this._superActivity==value) return this;
		if(this._superActivity!=null){
			this._superActivity._subsidaries.remove(value);
		}
		this._superActivity = value; 
		if(value._subsidaries==null)value._subsidaries = new ArrayList<Activity>();
		value._subsidaries.add(value);
		this._businessInfo = null;
		return this;
	}
	
	Activity _flow;
	public Activity flow(){
		if(this._flow==null){
			Activity spr = this.superActivity();
			while(spr!=null && spr != this){
				spr = spr._superActivity;
			}
			this._flow = spr;
		}
		return this.flow();
	}

	

	List<Activity> _subsidaries;
	public List<Activity> subsidaries(){
		return this._subsidaries==null?(this._subsidaries = new ArrayList<Activity>()):this._subsidaries;
	}

	public String pathname(){
		return this._entity.pathname();
	}

	public UUID flowId(){
		return this._entity.id();
	}
	
	

	public ActivityStates status(){
		return this._entity.status();
	}
	private Activity status(ActivityStates value){this._entity.status(value);return this;}
	
	State _state;
	public State state(){
		if(_state==null){
			JSONObject json = JSON.parseObject(this._entity.state());
			this._state = new State(this._entity.name(),json);
		}
		return this._state;
	}

	JSONObject _results;
	public JSONObject results(){
		if(this._results==null){
			this._results = JSON.parseObject(this._entity.results());
		}
		return this._results;
	}
	JSONObject _variables;
	public JSONObject variables(){
		if(this._variables==null){
			this._variables = JSON.parseObject(this._entity.variables());
		}
		return this._variables;
	}

	private BusinessInfo _businessInfo;
	private BusinessInfo businessInfo(){
		if(this._businessInfo==null) this._businessInfo = new BusinessInfoBuilder(this);
		return this._businessInfo;
	}

	

	Boolean entry(Dealer dealer,JSONObject inputs,Activity fromActivity,TriggleContext ctx) throws Exception{
		if(this.status()!= ActivityStates.created) return false;
		JSONObject variables = null;
		if(this.state().variables()!=null){
			variables = (JSONObject)State.cloneJSON(this.state().variables());
		}else variables = new JSONObject();

		Map<String,List<String>> imports = this.state().imports();
		if(imports!=null && imports.size()!=0){
			for(Map.Entry<String,List<String>> pair: imports.entrySet()){
				String key = pair.getKey();
				Object value = null;
				for(String memberExpr: pair.getValue()){
					value = this.resolveMember(memberExpr, fromActivity);
					if(value!=null){
						variables.put(key, value);
						break;
					} 
				}
			}
		}
		Diagram diagram = null;
		if(this._entity._actionType!=null){
			Action action = ctx.resolveAction(this._entity.actionType());
			diagram = action.entry(dealer, inputs, variables,this.state().subDiagram(),this.businessInfo() ,ctx);
		}
		this._entity.status(ActivityStates.entried);
		this._variables = variables;
		this._entity.variables(JSON.toJSONString(variables));
		if(diagram!=null){
			State.resetSubDiagram(this.state(),diagram);
			this._entity.state(JSON.toJSONString(this.state()));
		}
		this.setDealer(dealer);
		this.engine()._acitivityRepository.entryActivity(_entity, ctx);		
		return true;
	}
	private void setDealer(Dealer dealer){
		this._entity.dealerId(dealer.id());
		this._entity.dealTime(new Date());
		this._entity.dealerName(dealer.name());
	}

	protected Boolean deal(Dealer dealer,JSONObject params,TriggleContext ctx) throws Exception{
		if(this.status()!= ActivityStates.entried && this.status()!= ActivityStates.dealed) return null;
		Object results=null;
		this.setDealer(dealer);
		if(this._entity._actionType!=null){
			Action action = ctx.resolveAction(this._entity.actionType());
			results = action.deal(dealer, params, ctx);
		}else results = params;
		if(results==null){
			this.status(ActivityStates.dealing);
			this.engine()._acitivityRepository.dealActivity(_entity, ctx);
			return false;
		}
		JSONObject resultsJSON = this._results = (JSONObject)JSON.toJSON(results);
		JSONObject variables = this.variables();
		for(Map.Entry<String,Object> entry : resultsJSON.entrySet()){
			variables.put(entry.getKey(),entry.getValue());
		}
		this._entity.variables(JSON.toJSONString(variables));
		this._entity.results(JSON.toJSONString(results));
		this.status(ActivityStates.dealed);
		this.engine()._acitivityRepository.dealActivity(_entity, ctx);
		return true;
	}

	
	List<Activity> transfer(Dealer dealer, TriggleContext ctx) throws Exception{
		if(this.superActivity()==this) return null;
		List<Transition> transitions = this.checkTransitions(ctx);
		if(transitions==null) {
			if(!this.exit(dealer, ctx)) return null;
			return this.superActivity().transfer(dealer, ctx);
		}
		List<Activity> nextActivities = new ArrayList<Activity>();
		if(transitions.size()==0) return nextActivities;
		
		List<ActivityEntity> nextActEntities = new ArrayList<ActivityEntity>();		
		for(Transition transition : transitions){
			Activity acti = new Activity(dealer,transition,this.superActivity());
			nextActivities.add(acti);
			nextActEntities.add(acti._entity);
		}
		// fromActivity退出状态
		if(!this.exit(dealer, ctx)){
			return null;
		}

		// 新的节点插入状态
		this.engine()._acitivityRepository.createActivities(nextActEntities, ctx);

		//调用新节点的entry

		for(Activity nextActivity:nextActivities){
			this.engine().triggle(nextActivity, dealer, this._results, ctx);
		}

		return nextActivities;
	}	

	protected Boolean exit(Dealer dealer,TriggleContext ctx) throws Exception{
		Boolean canFinish = true;
		if(this._entity._actionType!=null){
			Action action = ctx.resolveAction(this._entity.actionType());
			canFinish = action.exit(dealer,  ctx);
		}
		if(!canFinish) return false;
		this.setDealer(dealer);
		this.status(ActivityStates.exited);
		this.engine()._acitivityRepository.exitActivity(_entity, ctx);
		if(this.superActivity()!=null && this.superActivity()!=this){
			Map<String,List<String>> exports = this.state().exports();
			
			if(exports!=null && exports.size()!=0){
				JSONObject superVariables = this.superActivity().variables();
				for(Map.Entry<String,List<String>> pair: exports.entrySet()){
					String key = pair.getKey();
					Object value = null;
					for(String memberExpr: pair.getValue()){
						value = this.resolveMember(memberExpr, this);
						if(value!=null){
							superVariables.put(key, value);
							break;
						} 
					}
				}
				this.superActivity()._entity.variables(JSON.toJSONString(superVariables));
				this.engine()._acitivityRepository.exitActivity(this.superActivity()._entity, ctx);
			}
		}
		return true;
	}

	List<Transition> checkTransitions(TriggleContext ctx) throws Exception{
		//没有线了，最后一步,完成
		if(this.state().transitions()==null || this.state().transitions().size()==0){

			return null;
		} 
		List<Transition> transactions = new ArrayList<Transition>();
				
		for(Map.Entry<String,Transition> entry : this.state().transitions().entrySet()){
			String checkRs = entry.getValue().predicate().eval(this,null,ctx);
			if(checkRs.equals("") || checkRs.equals("<FALSE>")){
				continue;
			}else transactions.add(entry.getValue());
		}
		return transactions;
	}
	

	// ./abc.abc:代表当前activity的变量
	// ../abc.abc:表示上级activity的变量
	// /abc.abc :表示根的变量
	// ?/abc.abc : 表示从各级activity查找变量
	// ~/abc.abc: 表示从prevActivity查找变量
	public Object resolveMember(String memberExpr,Activity prev) throws Exception{
		if(memberExpr==null) return null;
		Integer at = memberExpr.indexOf("/");
		if(at<=0) throw new Exception("错误的成员表达式，需要有/:" + memberExpr);
		String scope = memberExpr.substring(0,at-1);
		String varnames = memberExpr.substring(at+1);
		Object target=null;
		String[] pathnames = varnames.split(".");
		if(pathnames.length==0)  throw new Exception("错误的成员表达式" + memberExpr);
		String varname = pathnames[0];
		if(scope.equals("")) {
			JSONObject varValue = this.flow().variables();
			target = varValue.get(varname);
		} else if(scope.equals(".")){
			JSONObject varValue = this.variables();
			target = varValue.get(varname);
		} else if(scope.equals("..")){
			JSONObject varValue = this.superActivity().variables();
			target = varValue.get(varname);	
		}else if(scope.equals("~")){
			if(prev==null) return null;
			JSONObject varValue = prev.variables();
			target = varValue.get(varname);
		} else if(scope.equals("?")){
			Activity acti = this;
			while(acti!=null){
				JSONObject varValue = acti.variables();
				target = varValue.get(varname);
				if(target!=null) break;
				if(acti==acti.superActivity())break;
				acti = acti.superActivity();
			}
		}
		for(int i = 1;i<pathnames.length;i++){
			String pathname = pathnames[i];
			if(target instanceof JSONObject){
				target = ((JSONObject)target).get(pathname);
				
				
			}else return null;
		}
		return target;
	}

	public String ResolveMemberString(String memberExpr,Activity prev) throws Exception{
		Object value = this.resolveMember(memberExpr,prev);
		return value==null?"":value.toString();
	}


}
