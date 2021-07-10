package yitek.workflow.core;

import java.util.*;

import com.alibaba.fastjson.*;

import yitek.workflow.core.std.*;

public class Activity {
	// Flow创建
	public Activity(LocalSession session,Dealer dealer,String name,String version,Diagram diagram,Object inputs,List<String> starts) throws Exception{
		this._session = session;
		this._state = State.createFlow(name, diagram,starts);
		this._entity = new ActivityEntity(name,version,this._state.jsonObject(),dealer);
		this._inputs = new StringMap(inputs);
		this._entity.inputs(JSON.toJSONString(inputs));
		//this._entity.actionType(diagram.actionType());
		
	}	
	// 子级别创建
	public Activity(State subState,Dealer dealer,Activity superActivity,Object inputs) throws Exception{
		
		this._entity = new ActivityEntity(subState.name(),subState.jsonObject(),dealer,superActivity._entity);
		this._entity.actionName(subState.actionName());
		this._state = subState;
		this.superActivity(superActivity);
		this._entity.actionName(subState.actionName());
		this._session = superActivity.session();
		this._inputs = new StringMap(inputs);
		this._entity.inputs(JSON.toJSONString(inputs));
		this._entity.billStatus(subState.billStatus());
		
	}
	public Activity(Dealer dealer,Transition transition,Activity fromActivity,Object inputs) throws Exception{
		State state = transition.to();
		this._entity = new ActivityEntity(transition.to().name(),state.jsonObject(),dealer,fromActivity._entity);
		this._entity.actionName(transition.to().actionName());
		this._state = state;
		this._entity.transitionName(transition.name());
		this._entity.billStatus(state.billStatus());
		this.superActivity(fromActivity.superActivity());
		this._session = fromActivity.session();
		this._fromActivity = fromActivity;
		this._entity.fromId(fromActivity.id());
		this._inputs = new StringMap(inputs);
		this._entity.inputs(JSON.toJSONString(inputs));
	}
	//普通的getById
	public Activity(LocalSession engine,ActivityEntity entity){
		this._entity = entity;
		this._session = engine;
	}
	
	LocalSession _session;
	public LocalSession session(){
		return this._session;
	}
	public UUID id(){
		return this._entity._id;
	}

	public String name(){
		return this._entity._name;
	}
	public String version(){
		return this._entity.version();
	}

	public boolean isAuto(){
		return this._entity.isAuto();
	}
	
	ActivityEntity _entity;


	Activity _superActivity;
	public Activity superActivity() throws Exception{
		if(this._superActivity==null){
			if(this._entity.superId()!=null){
				if(this._entity.superId().equals(this._entity.id())) this.superActivity(this);
				else this.superActivity(this._session.activity(this._entity.superId()));
			}
		}
		return this._superActivity;
	}
	
	Activity superActivity(Activity value){
		if(this._superActivity==value || this==value) return this;
		if(this._superActivity!=null){
			this._superActivity._subsidiaries.remove(value);
		}
		this._superActivity = value; 
		if(value._subsidiaries ==null)value._subsidiaries = new ArrayList<>();
		value._subsidiaries.add(this);
		return this;
	}

	Activity _fromActivity;
	public Activity fromActivity() throws Exception{
		if(this._fromActivity==null){
			if(this._entity.fromId()!=null){
				this._fromActivity = this.session().activity(this._entity.fromId());
			}
		}
		return this._fromActivity;
	}
	private Activity fromActivity(Activity from){
		this._fromActivity = from;
		if(from!=null)this._entity.fromId(from.id());
		return this;
	}
	
	Activity _flow;
	public Activity flow() throws Exception{
		if(this._flow==null){
			Activity spr = this.superActivity();
			while(spr!=null && spr != this){
				spr = spr.superActivity();
			}
			this._flow = spr;
		}
		return this._flow;
	}

	String _startStatus;
	public void startStatus(String value){ this._startStatus = value;}

	

	List<Activity> _subsidiaries;
	public List<Activity> subsidiaries(){
		return this._subsidiaries ==null?(this._subsidiaries = new ArrayList<>()):this._subsidiaries;
	}

	public String pathname(){
		return this._entity.pathname();
	}

	public UUID flowId(){
		return this._entity.id();
	}
	public String billId(){
		return this._entity.billId();
	}
	public Activity billId(String value) throws Exception{
		if(this.activityStatus()!= ActivityStates.created) throw new Exception("只能在entry阶段修改billId");
		this._entity.billId(value);
		return this;
	}
	Object _bill;
	// 传递单据对象，不用每次都去数据库查
	public Object bill(){
		return _bill;
	}
	public Activity bill(Object value){
		this._bill = value;
		return this;
	}

	Object _task;
	// 传递单据对象，不用每次都去数据库查
	public Object task(){
		return _task;
	}
	public Activity task(Object value){
		this._task = value;
		return this;
	}

	public String businessId(){
		return this._entity.businessId();
	}
	public Activity businessId(String value) throws Exception{
		if(this.activityStatus()!= ActivityStates.created) throw new Exception("businessId");
		this._entity.businessId(value);
		return this;
	}
	public String taskId(){
		return this._entity.taskId();
	}
	public Activity taskId(String value) throws Exception{
		if(this.activityStatus()!= ActivityStates.created) throw new Exception("taskId");
		this._entity.taskId(value);
		return this;
	}
	
	public StartTypes startType(){
		return this._entity.startType();
	}

	public String transitionName(){
		return this._entity.transitionName();
	}

	public String billStatus(){
		return this._entity.billStatus();
	}

	public ActivityStates activityStatus(){
		return this._entity.activityStatus();
	}
	private Activity status(ActivityStates value){this._entity.activityStatus(value);return this;}
	
	State _state;
	public State state() throws Exception{
		if(_state==null){
			StringMap defMap = new StringMap(this._entity.state());
			Diagram ownDiagram = null;
			if(this.superActivity()!=null && !this.superActivity().equals(this)) ownDiagram = this.superActivity().state().subDiagram();
			this._state = new State(this._entity.name(),defMap,ownDiagram);
		}
		return this._state;
	}

	StringMap _inputs;
	public StringMap inputs(){
		if(this._inputs==null && this._entity._inputs!=null){
			this._inputs = new StringMap(JSON.parseObject(this._entity.inputs())).readonly(true);
		}
		return this._inputs;
	}

	StringMap _results;
	public StringMap results(){
		if(this._results==null && this._entity._results!=null){
			this._results = new StringMap(JSON.parseObject(this._entity.results()));
			if(this.activityStatus()!= ActivityStates.dealed){
				this._results.readonly(true);
			}
		}
		return this._results;
	}
	StringMap _variables;
	public StringMap variables(){
		if(this._variables==null){
			this._variables = new StringMap(JSON.parseObject(this._entity.variables()));
			if(this.activityStatus()!=ActivityStates.created) this._variables.readonly(true);
		}
		return this._variables;
	}

	public Object variables(String name){
		return this.variables().get(name);
	}

	private void setDealer(Dealer dealer){
		this._entity.dealerId(dealer.id());
		this._entity.dealTime(new Date());
		this._entity.dealerName(dealer.name());
	}

	private Action _action;
	public Action action(){
		if(_action==null){
			if(this._entity.actionName()!=null){
				this._action = this.session().resolveAction(this._entity.actionName());
			}else this._action = BasAction.empty();
		}
		return this._action==BasAction.empty()?null:this._action;
		
	}

	// 进入阶段
	Boolean entry(Dealer dealer,StringMap inputs) throws Exception{
		if(this.activityStatus()!= ActivityStates.created) return false;
		this.setDealer(dealer);

		StringMap variables;
		// 图中定义的变量先进来
		if(this.state().variables()!=null){
			variables = this.state().variables().clone();
		}else variables = new StringMap();
		this._variables = variables;
		// 要求导入的变量继续进来
		Map<String,List<String>> imports = this.state().imports();
		if(imports!=null && imports.size()!=0){
			for(Map.Entry<String,List<String>> pair: imports.entrySet()){
				String key = pair.getKey();
				Object value;
				for(String memberExpr: pair.getValue()){
					value = this.resolveVariable(memberExpr);
					if(value!=null){
						variables.put(key, value);
						break;
					} 
				}
			}
		}
		// inputs的值进入到variables
		if(inputs!=null){
			if(this.state().internals()!=null){
				List<String> internals = this.state().internals();
				for(Map.Entry<String,Object> entry : inputs.entrySet()){
					String key = entry.getKey();
					if(internals.stream().anyMatch(p->p.equals(key))) continue;
					variables.put(key, entry.getValue());
				}
			}else{
				for(Map.Entry<String,Object> entry : inputs.entrySet()){
					variables.put(entry.getKey(), entry.getValue());
				}
			}
			
		}
		if(this.state().subDiagram() instanceof  ReferenceDiagram){
			Diagram diagram = this.session().diagramRepository().getDiagramByName(((ReferenceDiagram) this.state().subDiagram()).reference(),this._entity.version());
			State.resetSubDiagram(this.state(),diagram);
		}
		this._entity._inputs = JSON.toJSONString(inputs);
		this._inputs = inputs;
		this._variables = variables;
		if(this.action()!=null){
			DiagramBuilder builder = new DiagramBuilder(this.state());
			if(!this.action().entry(this,dealer, inputs,builder, this.session())){
				return false;
			}
			Diagram subDiagram = builder.build();
			if(subDiagram!=null){
				State.resetSubDiagram(this.state(),subDiagram);
				this._entity.state(JSON.toJSONString(this.state()));
			}
		}
		
		this._entity.activityStatus(ActivityStates.entried);
		this._entity.variables(JSON.toJSONString(this._variables));
		this.session().activityRepository().entryActivity(_entity);		
		return true;
	}
	

	protected Boolean deal(Dealer dealer,StringMap rawParams) throws Exception{
		
		if(this.dealSubDiagram(dealer, rawParams)) return false;
		if(this.activityStatus()!= ActivityStates.entried && this.activityStatus()!= ActivityStates.dealed) return false;
		StringMap params = rawParams;
		this.setDealer(dealer);
		Object results;
		if(this.action()!=null){
			if(this.state().subDiagram()==null){
				DiagramBuilder builder = new DiagramBuilder(this.state());
				results = this.action().deal(this,dealer, params,builder, this.session());
				Diagram sub = builder.build();
				if(sub!=null){
					throw new Exception("TODO: not implement.在deal时开启子流程");
				}
			}else{
				results = this.action().deal(this,dealer, params,null, this.session());
			}
			
			if(results==null){
				return false;
			}
			if(StringMap.empty().equals(results)){
				this.status(ActivityStates.dealing);
				this._entity.params(JSON.toJSONString(params));
				this.session().activityRepository().dealActivity(_entity);
				return false;
			}
		}else {
			StringMap filteredParams=null;
			if(this.state().params()!=null){
				filteredParams = new StringMap();
				for(String n:this.state().params()){
					filteredParams.put(n, params.get(n));
				}
			}
			List<String> internals = this.state().internals();
			if(internals!=null && filteredParams!=null){
				params = filteredParams;
				filteredParams = new StringMap();
				for(Map.Entry<String,Object> entry:params.entrySet()){
					String key = entry.getKey();
					if(internals.stream().anyMatch(p->p.equals(key)))continue;
					filteredParams.put(key, entry.getValue());
				}
			}
			if(filteredParams!=null)results= filteredParams;
			else results = rawParams;
		}
		if(results!=null){
			StringMap resultsJSON = this._results = new StringMap(results);
			StringMap variables = this.variables();
			variables.readonly(false);
			for(Map.Entry<String,Object> entry : resultsJSON.entrySet()){
				variables.put(entry.getKey(),entry.getValue());
			}
			variables.readonly(true);
			this._entity.variables(JSON.toJSONString(variables));
			this._entity.results(JSON.toJSONString(results));
		}
		this._entity.params(JSON.toJSONString(rawParams));
		this.status(ActivityStates.dealed);
		this.session().activityRepository().dealActivity(_entity);
		return true;
	}

	Boolean dealSubDiagram(Dealer dealer,StringMap params) throws Exception{
		if(this.activityStatus()!= ActivityStates.entried) return false;
		Diagram subDiagram = this.state().subDiagram();
		if(subDiagram==null || subDiagram==Diagram.empty()) return false;

		this.setDealer(dealer);
		this._entity.params(JSON.toJSONString(params));
		this.status(ActivityStates.dealing);
		this.session().activityRepository().dealActivity(_entity);
		int count = 0;
		for(State startState :subDiagram.starts()){
			count++;
			Activity subActivity = new Activity(startState,dealer,this,this._inputs);
			subActivity._entity.startType(StartTypes.diagramStart);
			subActivity.fromActivity(this.fromActivity());
			subActivity._bill = this.bill();
			subActivity._entity.billId(this.billId());
			subActivity._entity.businessId(this.businessId());
			this.session().activityRepository().createActivity(subActivity._entity);
			
			this.session().active(subActivity, dealer, this.inputs());
		}
		//this._entity._subCount = count;
		//this.status(ActivityStates.dealed);
		//this.session().activityRepository().dealActivity(_entity);
		return true;
	}
	
	List<Activity> transfer(Dealer dealer) throws Exception{
		//根节点
		if(this.superActivity()==this) return null;
		// 检查下一步
		List<Transition> transitions = this.checkTransitions();
		//只找到一个结束转移，等同结束
		if(transitions!=null && transitions.size()==1 && transitions.get(0).to()==State.empty()){
			transitions = null;
		}
		// null表示没有下一步，自己是end节点
		if(transitions==null) {
			// 检查子节点是否全部完成
			if(this.session().activityRepository().countLivedSubordinatesBySuperId(this.id())!=0) return null;
			// 退出当前节点
			if(!this.exit(dealer,null)) return null;
			// 上级节点做状态转换
			return this.superActivity().transfer(dealer);
		}

		// 有一下一步节点
		List<Activity> nextActivities = new ArrayList<>();
		// 没有满足条件路径，直接返回，不会调用当前节点的exit
		if(transitions.size()==0) return nextActivities;
		
		List<ActivityEntity> nextActEntities = new ArrayList<>();
		for(Transition transition : transitions){
			Object nextInputs=null;
			if(this.action()!=null){
				nextInputs = this.action().transfer(this, dealer, transition, _session);
				if(nextInputs==null) return new ArrayList<>();
				else if(StringMap.empty()==nextInputs) continue;
			}
			if(transition.to()!=State.empty()){

				Activity acti = new Activity(dealer,transition,this.superActivity(),nextInputs);
				acti.billId(this.billId());
				acti.bill(this.bill());
				acti.fromActivity(this);
				nextActivities.add(acti);
				nextActEntities.add(acti._entity);
			}

		}
		// fromActivity退出阶段
		if(!this.exit(dealer,nextActivities)){
			//不允许退出
			return null;
		}

		// 新的节点插入状态
		this.session().activityRepository().createActivities(nextActEntities);

		//调用新节点的entry
		for(Activity nextActivity:nextActivities){
			this.session().active(nextActivity, dealer, nextActivity.inputs());
		}
		return nextActivities;
	}	

	protected Boolean exit(Dealer dealer,List<Activity> nextActEntities) throws Exception{
		if(this.action()!=null){
			if(!this.action().exit(this,dealer,nextActEntities, this.session())) return false;
		}
		this.setDealer(dealer);
		this.status(ActivityStates.exited);
		this._entity.doneTime(new Date());
		this.session().activityRepository().exitActivity(_entity);
		if(this.superActivity()!=null && this.superActivity()!=this){
			Map<String,List<String>> exports = this.state().exports();
			
			if(exports!=null && exports.size()!=0){
				Map<String,Object> superVariables = this.superActivity().variables();
				for(Map.Entry<String,List<String>> pair: exports.entrySet()){
					String key = pair.getKey();
					Object value;
					for(String memberExpr: pair.getValue()){
						value = this.resolveVariable(memberExpr);
						if(value!=null){
							superVariables.put(key, value);
							break;
						} 
					}
				}
				this.superActivity()._entity.variables(JSON.toJSONString(superVariables));
				this.session().activityRepository().exitActivity(this.superActivity()._entity);
			}
		}
		return true;
	}

	List<Transition> checkTransitions() throws Exception{
		//没有线了，最后一步,完成
		if(this.state().transitions()==null || this.state().transitions().size()==0){

			return null;
		} 
		List<Transition> transactions = new ArrayList<>();

		for(Map.Entry<String,Transition> entry : this.state().transitions().entrySet()){
			String checkRs = entry.getValue().predicate().eval(this,null,this.session());
			if(!checkRs.equals("") && !checkRs.equals("<FALSE>")){
				transactions.add(entry.getValue());
			}
		}
		return transactions;
	}
	static List<ActivityEntity> undoableActivities(ActivityEntity entity,Dealer dealer,Session session) throws Exception{
		if(entity.activityStatus()!= ActivityStates.exited) return null;
		if(entity.dealerId()!=null){
			if(dealer==null) return null;
			if(!entity.dealerId().equals(dealer.id())) return null;
		}
		List<ActivityEntity> nexts = session.activityRepository().listNextActivities(entity.id());
		boolean hasExits = nexts.stream().anyMatch(p->p.activityStatus()==ActivityStates.exited);
		if(hasExits) return null;
		return nexts;
	}

	public String recallable(Dealer dealer) throws Exception{
		if(!this._entity.dealerId().equals(dealer.id())) return null;
		String recallUrl  = this.state().recallable();
		if(recallUrl.equals("")) return null;
		List<ActivityEntity> paddings = new ArrayList<>();
		if(recallable(this._session,this._entity,paddings)){
			return recallUrl;
		}
		return null;
		
	}

	public boolean recall(Dealer dealer) throws Exception{
		if(!this._entity.dealerId().equals(dealer.id())) return false;
		if(this.state().recallable().equals("")) return false;
		List<ActivityEntity> paddings = new ArrayList<>();
		if(!recallable(this._session,this._entity,paddings)) return false;
		for(ActivityEntity entity:paddings){
			if(entity.actionName()!=null){
				Action action = this._session.resolveAction(entity.actionName());
				action.undo(new Activity(this._session,entity),dealer, _session);
			}
			this._session.activityRepository().removeActivityById(entity.id());
		}
		if(this.action()!=null)this.action().reenter(this,dealer, _session);
		return true;
	}

	static boolean recallable(LocalSession session,ActivityEntity curr,List<ActivityEntity> paddings) throws Exception{
		List<ActivityEntity> nexts = session.activityRepository().listNextActivities(curr.id());
		for (ActivityEntity next:nexts) {
			if(next.activityStatus()!= ActivityStates.exited){
				paddings.add(next);
			}else if(next.isAuto()){
				paddings.add(next);
				if(!recallable(session, next,paddings)){
					return false;
				}
			}
		}
		if(paddings.size()!=nexts.size()){
			return false;
		}
		return true;
		//return undoableActivities(this._entity, dealer, this._session)!=null;
	}

	

	// ./abc.abc:代表当前activity的变量
	// ../abc.abc:表示上级activity的变量
	// /abc.abc :表示根的变量
	// ?/abc.abc : 表示从各级activity查找变量
	// ~/abc.abc: 表示从prevActivity查找变量
	public Object resolveVariable(String memberExpr) throws Exception{
		if(memberExpr==null) return null;
		int at = memberExpr.indexOf("/");
		String scope;
		String varnames;
		if(at<0) {
			return memberExpr;
		}else{
			scope = memberExpr.substring(0,at);
			varnames = memberExpr.substring(at+1);
		}
		if(varnames.length()==0) return memberExpr;
	
		Activity targetActivity=null;
		String[] pathnames = varnames.split("\\.");
		if(scope.equals("")){
			targetActivity = this.flow();
		}
		if(scope.equals(".")||scope.equals("?")){
			targetActivity = this;
		}
		if(scope.equals("..")){
			targetActivity = this.superActivity();
		}
		if(scope.equals("~")){
			targetActivity = this.fromActivity();
		}
//		switch (scope) {
//			case "" -> targetActivity = this.flow();
//			case ".", "?" -> targetActivity = this;
//			case ".." -> targetActivity = this.superActivity();
//			case "~" -> targetActivity = this.fromActivity();
//		}
		if(targetActivity==null) return null;
		if(pathnames.length==0) {
			if(varnames.charAt(0)=='@'){
				return resolvePropertyValue(targetActivity,varnames);
			}
			pathnames = new String[]{varnames};
		}
		String varname = pathnames[0];
		Object value=null;
		if(scope.equals("?")){
			while (targetActivity != null) {
				value = targetActivity.variables(varname);
				if (value != null) break;
				if (targetActivity == targetActivity.superActivity()) break;
				targetActivity = targetActivity.superActivity();
			}
		}else value = targetActivity.variables(varname);
		
		return value==null?null:StringMap.resolve(value, pathnames,1);
	}

	static Object resolvePropertyValue(Activity activity,String name){
		if(name.equals("@name")) return activity._entity.name();
		if(name.equals("@id")) return activity.id();
		if(name.equals("@transitionName")) return activity.transitionName();
		if(name.equals("@billId")) return activity.billId();
		if(name.equals("@businessId")) return activity.businessId();
		if(name.equals("@taskId")) return activity.taskId();
		return null;
	}

	public String resolveVariableString(String memberExpr) throws Exception{
		Object value = this.resolveVariable(memberExpr);
		//if(value==null) return "";
		return value==null?"":value.toString();
	}


}
