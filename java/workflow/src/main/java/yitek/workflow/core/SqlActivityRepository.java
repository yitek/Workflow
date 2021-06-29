package yitek.workflow.core;

import java.sql.*;
import java.util.*;

public abstract class SqlActivityRepository implements ActivityRepository {

	public abstract Connection connection() throws Exception;
	
	public ActivityEntity getActivityById(UUID id) throws Exception{
		String sql= "SELECT * FROM wf_activity WHERE id=?";//查询student数据表
		PreparedStatement stmt = this.connection().prepareStatement(sql);
		stmt.setBytes(1, ActivityEntity.UUID2Bytes(id));
		ResultSet rs = stmt.executeQuery();
		ActivityEntity entity = new ActivityEntity();
		if(rs.next()) fillEntity(rs, entity);
		return entity;
	}
	public ActivityEntity getActivityByTaskId(String taskId) throws Exception{
		String sql= "SELECT * FROM wf_activity WHERE taskId=?";//查询student数据表
		PreparedStatement stmt = this.connection().prepareStatement(sql);
		stmt.setString(1, taskId);
		ResultSet rs = stmt.executeQuery(sql);
		ActivityEntity entity = new ActivityEntity();
		if(rs.next()) fillEntity(rs, entity);
		return entity;
	}

	public Integer countLivedSubordinatesBySuperId(UUID superId)throws Exception{
		String sql= "SELECT count(id) FROM wf_activity WHERE superId=? AND status<>0 AND status<>4";
		//查询student数据表
		PreparedStatement stmt = this.connection().prepareStatement(sql);
		stmt.setBytes(1, ActivityEntity.UUID2Bytes(superId));
		ResultSet rs = stmt.executeQuery();
		if(rs.next()) return rs.getInt(0);
		return 0;

	}
	public List<ActivityEntity> listLivedActivitiesById(UUID id) throws Exception{
		
		String sql= "SELECT flowId FROM wf_activity WHERE id=? AND status<>0 AND status<>-1 AND status<>4";//查询student数据表
		PreparedStatement stmt = this.connection().prepareStatement(sql);
		stmt.setBytes(1, ActivityEntity.UUID2Bytes(id));
		ResultSet rs = stmt.executeQuery();
		byte[] flowId ;
		if(rs.next()){
			flowId = rs.getBytes(1);
			rs.close();
		}else{ rs.close();return null;}
		sql= "SELECT * FROM wf_activity WHERE flowId=? AND status<>0 AND status<>4 AND status<>-1";
		System.out.println(ActivityEntity.Bytes2UUID(flowId));
		stmt = this.connection().prepareStatement(sql);
		stmt.setBytes(1, flowId);
		rs = stmt.executeQuery();
		List<ActivityEntity> list = new ArrayList<>();
		while(rs.next()){
			ActivityEntity entity = new ActivityEntity();
			fillEntity(rs, entity);
			list.add(entity);
		}
		rs.close();
		return list;
	}
	
	public List<ActivityEntity> listActivitiesBySuperId(UUID superId) throws Exception{
		
		String sql= "SELECT * FROM wf_activity WHERE superId=?";//查询student数据表
		PreparedStatement stmt = this.connection().prepareStatement(sql);
		stmt.setBytes(1, ActivityEntity.UUID2Bytes(superId));
		ResultSet rs = stmt.executeQuery();
		
		List<ActivityEntity> list = new ArrayList<>();
		while(rs.next()){
			ActivityEntity entity = new ActivityEntity();
			fillEntity(rs, entity);
			list.add(entity);
		}
		return list;
	}
	static protected void fillEntity(ResultSet rs,ActivityEntity entity) throws Exception{
		fillInfoEntity(rs,entity);
		entity.inputs(rs.getString("inputs"));
		entity.params(rs.getString("params"));
		entity.variables(rs.getString("variables"));
		entity.results(rs.getString("results"));

		entity.state(rs.getString("state"));

	}
	public List<ActivityEntity> listNextActivities(UUID fromId)throws Exception{
		String sql= "SELECT id,name,version,pathname,fromId,transitionName,status,createTime,creatorId,creatorName,dealTime,dealerId,dealerName,doneTime,billId,taskId,businessId,actionName,suspended,subCount,startType FROM wf_activity WHERE fromId=?";
		PreparedStatement stmt = this.connection().prepareStatement(sql);
		stmt.setBytes(1, ActivityEntity.UUID2Bytes(fromId));
		ResultSet rs = stmt.executeQuery();
		
		List<ActivityEntity> list = new ArrayList<>();
		while(rs.next()){
			ActivityEntity entity = new ActivityEntity();
			fillInfoEntity(rs, entity);
			list.add(entity);
		}
		return list;
	}
	static protected void fillInfoEntity(ResultSet rs,ActivityEntity entity) throws Exception{
		Object value;
		entity.id(ActivityEntity.Bytes2UUID(rs.getBytes("id")));
		entity.flowId(ActivityEntity.Bytes2UUID(rs.getBytes("flowId")));
		entity.superId(ActivityEntity.Bytes2UUID(rs.getBytes("superId")));
		

		entity.name(rs.getString("name"));
		entity.version(rs.getString("version"));
		entity.pathname(rs.getString("pathname"));

		value = rs.getBytes("fromId");
		if(value!=null) entity.fromId(ActivityEntity.Bytes2UUID((byte[])value));
		entity.transitionName(rs.getString("transitionName"));

		entity.status(ActivityStates.valueOf(rs.getInt("status")));

		entity.createTime(rs.getTimestamp("createTime"));
		entity.creatorId(rs.getString("creatorId"));
		entity.creatorName(rs.getString("creatorName"));
		entity.dealTime(rs.getTimestamp("dealTime"));
		entity.dealerId(rs.getString("dealerId"));
		entity.dealerName(rs.getString("dealerName"));
		entity.doneTime(rs.getTimestamp("doneTime"));
		entity.businessId(rs.getString("businessId"));
		entity.billId(rs.getString("billId"));
		entity.taskId(rs.getString("taskId"));
		entity.actionName(rs.getString("actionName"));
		entity.suspended(rs.getBoolean("suspended"));
		entity.subCount(rs.getInt("subCount"));
		entity.startType(StartTypes.valueOf(rs.getInt("startType")));
	}
	public boolean removeNextActivities(UUID fromId)throws Exception{
		String sql= "DELETE FROM wf_activity WHERE fromId=?";
		PreparedStatement stmt = this.connection().prepareStatement(sql);
		stmt.setBytes(1, ActivityEntity.UUID2Bytes(fromId));
		return stmt.execute();
	}
	private final String insertSql = "INSERT INTO wf_activity (" +
			"id,flowId,superId,name,version,pathname" +
			",fromId,transitionName," +
			"status,actionName,startType" +
			",state,inputs,createTime,creatorId,creatorName)VALUES(" +
			"?,?,?,?,?,?" +
			",?,?" +
			",?,?,?" +
			",?,?,?,?,?)";//
	public void createActivity(ActivityEntity entity) throws Exception{
		PreparedStatement stmt = this.connection().prepareStatement(insertSql);
		fillCreateStatement(stmt, entity);
		stmt.execute();
	}
	public void createActivities(List<ActivityEntity> entities) throws Exception{

		PreparedStatement stmt = this.connection().prepareStatement(insertSql);
		for(ActivityEntity entity : entities){
			fillCreateStatement(stmt, entity);
			stmt.execute();
		}
		
	}
	static void fillCreateStatement(PreparedStatement stmt,ActivityEntity entity) throws Exception{
		//id,flowId,superId,name,version,pathname,fromId,transitionName,status,actionType,state,createTime,creatorId,creatorName
		int index = 1;
		stmt.setBytes(index++,ActivityEntity.UUID2Bytes(entity.id()));
		stmt.setBytes(index++,ActivityEntity.UUID2Bytes(entity.flowId()));
		stmt.setBytes(index++,ActivityEntity.UUID2Bytes(entity.superId()));
		stmt.setString(index++,entity.name());
		stmt.setString(index++,entity.version());
		stmt.setString(index++,entity.pathname());

		stmt.setBytes(index++,entity.fromId()==null?null:ActivityEntity.UUID2Bytes(entity.fromId()));
		stmt.setString(index++,entity.transitionName());

		stmt.setInt(index++,entity.status().value());
		stmt.setString(index++, entity.actionName());
		stmt.setInt(index++,entity.startType().value());

		stmt.setString(index++, entity.state());
		stmt.setString(index++, entity.inputs());

		stmt.setTimestamp(index++,new Timestamp(entity.createTime().getTime()));
		stmt.setString(index++,entity.creatorId());
		stmt.setString(index,entity.creatorName());

	}

	

	public void entryActivity(ActivityEntity entity) throws Exception{
		String sql= "UPDATE wf_activity SET status=?,variables=?,dealTime=?,dealerId=?,dealerName=?,state=?,businessId=?,billId=?,taskId=?,subCount=? WHERE id=?";//查询student数据表
		PreparedStatement stmt = this.connection().prepareStatement(sql);
		fillEntryStatement(stmt, entity);
		stmt.execute();
	}
	
	static void fillEntryStatement(PreparedStatement stmt,ActivityEntity entity) throws Exception{
		int index = 1;
		stmt.setInt(index++,entity.status().value());
		//stmt.setString(index++,entity.inputs());
		//stmt.setString(index++,entity.params());
		stmt.setString(index++,entity.variables());
		
		stmt.setTimestamp(index++,new Timestamp(entity.dealTime().getTime()));
		stmt.setString(index++,entity.dealerId());
		stmt.setString(index++,entity.dealerName());
		stmt.setString(index++,entity.state());

		stmt.setString(index++,entity.businessId());
		stmt.setString(index++,entity.billId());
		stmt.setString(index++,entity.taskId());

		stmt.setInt(index++,entity.subCount());
		//stmt.setBoolean(index++,entity.isStart());

		stmt.setBytes(index,ActivityEntity.UUID2Bytes(entity.id()));
	}

	public void dealActivity(ActivityEntity entity) throws Exception{
		String sql= "UPDATE wf_activity SET status=?,params=?,variables=?,results=?,state=?,dealTime=?,dealerId=?,dealerName=?,subCount=? WHERE id=?";//查询student数据表
		PreparedStatement stmt = this.connection().prepareStatement(sql);
		fillDealStatement(stmt, entity);
		stmt.execute();
	}
	static void fillDealStatement(PreparedStatement stmt,ActivityEntity entity) throws Exception{
		int index = 1;
		stmt.setInt(index++,entity.status().value());
		stmt.setString(index++,entity.params());
		stmt.setString(index++,entity.variables());
		stmt.setString(index++,entity.results());
		stmt.setString(index++,entity.state());
		stmt.setTimestamp(index++,new Timestamp(entity.dealTime().getTime()));
		stmt.setString(index++,entity.dealerId());
		stmt.setString(index++,entity.dealerName());

		stmt.setInt(index++,entity.subCount());


		stmt.setBytes(index,ActivityEntity.UUID2Bytes(entity.id()));
	}

	public void exitActivity(ActivityEntity entity) throws Exception{
		String sql= "UPDATE wf_activity SET status=?,dealTime=?,dealerId=?,dealerName=?,doneTime=? WHERE id=?";//查询student数据表
		PreparedStatement stmt = this.connection().prepareStatement(sql);
		fillExitStatement(stmt, entity);
		stmt.execute();
	}

	static void fillExitStatement(PreparedStatement stmt,ActivityEntity entity) throws Exception{
		int index = 1;
		stmt.setInt(index++,entity.status().value());
		stmt.setTimestamp(index++,new Timestamp(entity.dealTime().getTime()));
		stmt.setString(index++,entity.dealerId());
		stmt.setString(index++,entity.dealerName());
		if(entity.doneTime()==null){
			stmt.setTimestamp(index++,null);
		}else stmt.setTimestamp(index++,new Timestamp(entity.doneTime().getTime()));

		stmt.setBytes(index,ActivityEntity.UUID2Bytes(entity.id()));
	}
}
