package yitek.workflow.core;

import java.util.*;

public class ActivityEntity {
	UUID id;
	public UUID getId() {return this.id;}
	public ActivityEntity setId(UUID value){ this.id = value;return this;}

	UUID fromActivityId;
	public UUID getfromActivityId() {return this.fromActivityId;}
	public ActivityEntity setfromActivityId(UUID value){ this.fromActivityId = value;return this;}

	UUID parentId;
	public UUID getParentId() {return this.parentId;}
	public ActivityEntity setParentId(UUID value){ this.parentId = value;return this;}

	UUID flowId;
	public UUID getFlowId() {return this.flowId;}
	public ActivityEntity setFlowId(UUID value){ this.flowId = value;return this;}

	String name;
	public String getName() {return this.name;}
	public ActivityEntity setName(String value){ this.name = value;return this;}

	ActivityStates status;
	public ActivityStates getStatus() {return this.status;}
	public ActivityEntity setStatus(ActivityStates value){ this.status = value;return this;}

	Date createTime;
	public Date getCreateTime() {return this.createTime;}
	public ActivityEntity setCreateTime(Date value){ this.createTime = value;return this;}

	String creatorId;
	public String getCreatorId() {return this.creatorId;}
	public ActivityEntity setCreatorId(String value){ this.creatorId = value;return this;}

	String creatorName;
	public String getCreatorName() {return this.creatorName;}
	public ActivityEntity setCreatorName(String value){ this.creatorName = value;return this;}

	Date dealTime;
	public Date getDealTime() {return this.dealTime;}
	public ActivityEntity setDealTime(Date value){ this.dealTime = value;return this;}

	String dealerId;
	public String getDealerId() {return this.dealerId;}
	public ActivityEntity setDealerId(String value){ this.dealerId = value;return this;}

	String dealerName;
	public String getDealerName() {return this.creatorName;}
	public ActivityEntity setDealerName(String value){ this.creatorName = value;return this;}


	String std;
	public String getSTD() {return this.std;}
	public ActivityEntity setSTD(String value){ this.std = value;return this;}

	String activityType;
	public String getActivityType() {return this.activityType;}
	public ActivityEntity setActivityType(String value){ this.activityType = value;return this;}

	



	String inputs;
	public String getInputs() {return this.inputs;}
	public ActivityEntity setInputs(String value){ this.inputs = value;return this;}	

	String params;
	public String getParams() {return this.params;}
	public ActivityEntity setParams(String value){ this.params = value;return this;}

	String imports;
	public String geImports() {return this.imports;}
	public ActivityEntity setImports(String value){ this.imports = value;return this;}

	String exports;
	public String getExports() {return this.exports;}
	public ActivityEntity setExports(String value){ this.exports = value;return this;}

	String variables;
	public String getVariables() {return this.variables;}
	public ActivityEntity setVariables(String value){ this.variables = value;return this;}

	String results;
	public String getResults() {return this.results;}
	public ActivityEntity setResults(String value){ this.results = value;return this;}

	List<ActivityEntity> subsidaries;
	public List<ActivityEntity> getSubsidaries() {return this.subsidaries;}
	public ActivityEntity setSubsidaries(List<ActivityEntity> value){ this.subsidaries = value;return this;}
}
