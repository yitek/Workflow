package yitek.workflow.core;

import java.sql.*;
import java.util.*;

public abstract class SqlActivityRepository implements ActivityRepository {

	public abstract Connection connection() throws Exception;
	
	public ActivityEntity getActivityById(UUID id) throws Exception{
		PreparedStatement stmt=null;
		ResultSet rs=null;
		try{
			String sql= "SELECT * FROM wf_activity WHERE id=?";//查询student数据表
			stmt = this.connection().prepareStatement(sql);
			stmt.setBytes(1, ActivityEntity.UUID2Bytes(id));
			rs = stmt.executeQuery();
			ActivityEntity entity = new ActivityEntity();
			if(rs.next()) fillEntity(rs, entity);
			return entity;
		}finally{
			if(rs!=null) rs.close();
			if(stmt!=null) stmt.close();
		}
		
	}
	public ActivityEntity getActivityByTaskId(String taskId) throws Exception{
		PreparedStatement stmt=null;
		ResultSet rs=null;
		try{
			String sql= "SELECT * FROM wf_activity WHERE taskId=?";//查询student数据表
			stmt = this.connection().prepareStatement(sql);
			stmt.setString(1, taskId);
			rs = stmt.executeQuery(sql);
			ActivityEntity entity = new ActivityEntity();
			if(rs.next()) fillEntity(rs, entity);
			return entity;
		}finally{
			if(rs!=null) rs.close();
			if(stmt!=null) stmt.close();
		}
		
	}

	public Integer countLivedSubordinatesBySuperId(UUID superId)throws Exception{
		PreparedStatement stmt=null;
		ResultSet rs=null;
		try{
			String sql= "SELECT count(id) FROM wf_activity WHERE superId=? AND activityStatus<>0 AND activityStatus<>4";
			stmt = this.connection().prepareStatement(sql);
			stmt.setBytes(1, ActivityEntity.UUID2Bytes(superId));
			rs = stmt.executeQuery();
			if(rs.next()) return rs.getInt(1);
			return 0;
		}finally{
			if(rs!=null) rs.close();
			if(stmt!=null) stmt.close();
		}
		

	}
	public List<ActivityEntity> listLivedActivitiesById(UUID id) throws Exception{
		PreparedStatement stmt=null;
		ResultSet rs=null;
		PreparedStatement stmt1=null;
		ResultSet rs1=null;
		try{
			String sql= "SELECT flowId FROM wf_activity WHERE id=? AND activityStatus<>0 AND activityStatus<>-1 AND activityStatus<>4";//查询student数据表
			stmt = this.connection().prepareStatement(sql);
			stmt.setBytes(1, ActivityEntity.UUID2Bytes(id));
			rs = stmt.executeQuery();
			byte[] flowId ;
			if(rs.next()){
				flowId = rs.getBytes(1);
			}else{return null;}
			sql= "SELECT * FROM wf_activity WHERE flowId=? AND activityStatus<>0 AND activityStatus<>4 AND activityStatus<>-1";
			System.out.println(ActivityEntity.Bytes2UUID(flowId));
			stmt1 = this.connection().prepareStatement(sql);
			stmt1.setBytes(1, flowId);
			rs1 = stmt1.executeQuery();
			List<ActivityEntity> list = new ArrayList<>();
			while(rs1.next()){
				ActivityEntity entity = new ActivityEntity();
				fillEntity(rs1, entity);
				list.add(entity);
			}
			
			return list;
		}finally{
			if(rs!=null) rs.close();
			if(stmt!=null) stmt.close();
			if(rs1!=null) rs.close();
			if(stmt1!=null) stmt.close();
		}
		
	}
	
	public List<ActivityEntity> listActivitiesBySuperId(UUID superId) throws Exception{
		PreparedStatement stmt=null;
		ResultSet rs=null;
		try{
			String sql= "SELECT * FROM wf_activity WHERE superId=?";//查询student数据表
			stmt = this.connection().prepareStatement(sql);
			stmt.setBytes(1, ActivityEntity.UUID2Bytes(superId));
			rs = stmt.executeQuery();
			
			List<ActivityEntity> list = new ArrayList<>();
			while(rs.next()){
				ActivityEntity entity = new ActivityEntity();
				fillEntity(rs, entity);
				list.add(entity);
			}
			return list;
		}finally{
			if(rs!=null) rs.close();
			if(stmt!=null) stmt.close();
		}
		
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
		String sql= "SELECT id,name,version,pathname,flowId,superId,fromId,transitionName,activityStatus,createTime,creatorId,creatorName,dealTime,dealerId,dealerName,doneTime,billId,taskId,businessId,billStatus,actionName,suspended,subCount,isAuto,startType FROM wf_activity WHERE fromId=?";
		PreparedStatement stmt = this.connection().prepareStatement(sql);
		stmt.setBytes(1, ActivityEntity.UUID2Bytes(fromId));
		ResultSet rs = stmt.executeQuery();
		
		List<ActivityEntity> list = new ArrayList<>();
		while(rs.next()){
			ActivityEntity entity = new ActivityEntity();
			fillInfoEntity(rs, entity);
			list.add(entity);
		}
		rs.close();
		stmt.close();
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

		entity.activityStatus(ActivityStates.valueOf(rs.getInt("ActivityStatus")));
		entity.billStatus(rs.getString("billStatus"));

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
		entity.isAuto(rs.getInt("isAuto")!=0);
		entity.startType(StartTypes.valueOf(rs.getInt("startType")));
	}
	public boolean removeActivityById(UUID aId)throws Exception{
		PreparedStatement stmt= null;
		try{
			String sql= "DELETE FROM wf_activity WHERE id=?";
			stmt = this.connection().prepareStatement(sql);
			stmt.setBytes(1, ActivityEntity.UUID2Bytes(aId));
			stmt.execute();
			return true;
		}finally{
			if(stmt!=null) stmt.close();
		}
		
	}
	private final String insertSql = "INSERT INTO wf_activity (" +
			"id,flowId,superId,name,version,pathname" +
			",fromId,transitionName," +
			"activityStatus,billStatus,actionName,startType" +
			",state,inputs,createTime,creatorId,creatorName)VALUES(" +
			"?,?,?,?,?,?" +
			",?,?" +
			",?,?,?,?" +
			",?,?,?,?,?)";//
	public void createActivity(ActivityEntity entity) throws Exception{
		PreparedStatement stmt= null;
		try{
			stmt = this.connection().prepareStatement(insertSql);
			fillCreateStatement(stmt, entity);
			stmt.execute();
		}finally{
			if(stmt!=null)stmt.close();
		}
		
		
	}
	public void createActivities(List<ActivityEntity> entities) throws Exception{
		PreparedStatement stmt = null;
		try{
			stmt = this.connection().prepareStatement(insertSql);
			for(ActivityEntity entity : entities){
				fillCreateStatement(stmt, entity);
				stmt.execute();
			}
		}finally{
			if(stmt!=null) stmt.close();
		}		
	}
	static void fillCreateStatement(PreparedStatement stmt,ActivityEntity entity) throws Exception{
		
		int index = 1;
		stmt.setBytes(index++,ActivityEntity.UUID2Bytes(entity.id()));
		stmt.setBytes(index++,ActivityEntity.UUID2Bytes(entity.flowId()));
		stmt.setBytes(index++,ActivityEntity.UUID2Bytes(entity.superId()));
		stmt.setString(index++,entity.name());
		stmt.setString(index++,entity.version());
		stmt.setString(index++,entity.pathname());

		stmt.setBytes(index++,entity.fromId()==null?null:ActivityEntity.UUID2Bytes(entity.fromId()));
		stmt.setString(index++,entity.transitionName());

		stmt.setInt(index++,entity.activityStatus().value());
		stmt.setString(index++,entity.billStatus());
		stmt.setString(index++, entity.actionName());
		stmt.setInt(index++,entity.startType().value());

		stmt.setString(index++, entity.state());
		stmt.setString(index++, entity.inputs());

		stmt.setTimestamp(index++,new Timestamp(entity.createTime().getTime()));
		stmt.setString(index++,entity.creatorId());
		stmt.setString(index,entity.creatorName());

	}

	

	public void entryActivity(ActivityEntity entity) throws Exception{
		String sql= "UPDATE wf_activity SET activityStatus=?,variables=?,dealTime=?,dealerId=?,dealerName=?,state=?,businessId=?,billId=?,taskId=?,subCount=? WHERE id=?";//查询student数据表
		PreparedStatement stmt = null;
		try{
			stmt=this.connection().prepareStatement(sql);
			fillEntryStatement(stmt, entity);
			stmt.execute();
		}finally{
			if(stmt!=null)stmt.close();
		}
	}
	
	static void fillEntryStatement(PreparedStatement stmt,ActivityEntity entity) throws Exception{
		int index = 1;
		stmt.setInt(index++,entity.activityStatus().value());
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
		String sql= "UPDATE wf_activity SET activityStatus=?,params=?,variables=?,results=?,state=?,dealTime=?,dealerId=?,dealerName=?,subCount=?,isAuto=? WHERE id=?";//查询student数据表
		PreparedStatement stmt = null;
		try{
			stmt = this.connection().prepareStatement(sql);
			fillDealStatement(stmt, entity);
			stmt.execute();
		}finally{
			if(stmt!=null) stmt.close();
		}
	}
	static void fillDealStatement(PreparedStatement stmt,ActivityEntity entity) throws Exception{
		int index = 1;
		stmt.setInt(index++,entity.activityStatus().value());
		stmt.setString(index++,entity.params());
		stmt.setString(index++,entity.variables());
		stmt.setString(index++,entity.results());
		stmt.setString(index++,entity.state());
		stmt.setTimestamp(index++,new Timestamp(entity.dealTime().getTime()));
		stmt.setString(index++,entity.dealerId());
		stmt.setString(index++,entity.dealerName());

		stmt.setInt(index++,entity.subCount());
		stmt.setInt(index++,entity.isAuto()?1:0);

		stmt.setBytes(index,ActivityEntity.UUID2Bytes(entity.id()));
	}

	public void exitActivity(ActivityEntity entity) throws Exception{
		String sql= "UPDATE wf_activity SET activityStatus=?,dealTime=?,dealerId=?,dealerName=?,doneTime=? WHERE id=?";//查询student数据表
		PreparedStatement stmt =  null;
		try{
			stmt = this.connection().prepareStatement(sql);
			fillExitStatement(stmt, entity);
			stmt.execute();
		}finally{
			if(stmt!=null) stmt.close();
		}
	}

	static void fillExitStatement(PreparedStatement stmt,ActivityEntity entity) throws Exception{
		int index = 1;
		stmt.setInt(index++,entity.activityStatus().value());
		stmt.setTimestamp(index++,new Timestamp(entity.dealTime().getTime()));
		stmt.setString(index++,entity.dealerId());
		stmt.setString(index++,entity.dealerName());
		if(entity.doneTime()==null){
			stmt.setTimestamp(index++,null);
		}else stmt.setTimestamp(index++,new Timestamp(entity.doneTime().getTime()));

		stmt.setBytes(index,ActivityEntity.UUID2Bytes(entity.id()));
	}
}
