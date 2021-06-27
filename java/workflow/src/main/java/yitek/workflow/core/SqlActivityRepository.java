package yitek.workflow.core;

import java.sql.*;
import java.util.*;

public class SqlActivityRepository implements ActivityRepository {
	public SqlActivityRepository(String driver,String db,String user,String password){
		this.driverName = driver;
		this.db = db;
		this.user = user;
		this.password = password;
	}
	String driverName;
	String db = "jdbc:mysql://localhost:3306/text?useSSL=false&serverTimezone=UTC";
	String user = "root";//数据库用户名
	String password = "123456";//数据库密码
	Connection _conn;
	public Connection connection() throws Exception{
		if(this._conn==null){
			Class.forName(this.driverName);
			this._conn = DriverManager.getConnection(this.db,this.user,this.password);
		}
		return this._conn;
	}
	public ActivityEntity getActivityById(UUID id) throws Exception{
		String sql= "SELECT flowId FROM wf_activity WHERE id=?";//查询student数据表
		PreparedStatement stmt = this.connection().prepareStatement(sql);
		stmt.setBytes(0, ActivityEntity.UUID2Bytes(id));
		ResultSet rs = stmt.executeQuery(sql);
		ActivityEntity entity = new ActivityEntity();
		if(rs.next()) fillEntity(rs, entity);
		return entity;
	}

	public Integer countLivedSubsidariesIdsBySuperId(UUID superId)throws Exception{
		String sql= "SELECT count(id) FROM wf_activity WHERE superId=? AND status<>0 AND status<>4";
		//查询student数据表
		PreparedStatement stmt = this.connection().prepareStatement(sql);
		stmt.setBytes(0, ActivityEntity.UUID2Bytes(superId));
		ResultSet rs = stmt.executeQuery();
		if(rs.next()) return rs.getInt(0);
		return 0;

	}
	public List<ActivityEntity> listLivedActivitiesById(UUID id) throws Exception{
		
		String sql= "SELECT flowId FROM wf_activity WHERE id=?";//查询student数据表
		PreparedStatement stmt = this.connection().prepareStatement(sql);
		stmt.setBytes(0, ActivityEntity.UUID2Bytes(id));
		ResultSet rs = stmt.executeQuery();
		byte[] flowId =null;
		if(rs.next()){
			flowId = rs.getBytes(0);		
		}else return null;
		sql= "SELECT * FROM wf_activity WHERE flowId=? AND status<>0 AND status<>4";
		//查询student数据表
		stmt = this.connection().prepareStatement(sql);
		stmt.setBytes(0, flowId);
		List<ActivityEntity> list = new ArrayList<ActivityEntity>();
		while(rs.next()){
			ActivityEntity entity = new ActivityEntity();
			fillEntity(rs, entity);
			list.add(entity);
		}
		return list;
	}
	static protected void fillEntity(ResultSet rs,ActivityEntity entity) throws Exception{
		Object value=null;
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
		entity.params(rs.getString("params"));
		entity.variables(rs.getString("variables"));
		entity.results(rs.getString("results"));

		entity.state(rs.getString("state"));
		entity.actionType(rs.getString("actionType"));

		entity.createTime(rs.getTimestamp("createTime"));
		entity.creatorId(rs.getString("creatorId"));
		entity.creatorName(rs.getString("creatorName"));
		entity.dealTime(rs.getTimestamp("dealTime"));
		entity.dealerId(rs.getString("dealerId"));
		entity.dealerName(rs.getString("dealerName"));
		entity.doneTime(rs.getTimestamp("doneTime"));
	}
	public void createActivity(ActivityEntity entity,Object ctx) throws Exception{
		String sql= "INSERT INTO wf_activity (id,flowId,superId,name,version,pathname,fromId,transitionName,status,actionType,state,createTime,creatorId,creatorName)VALUES(?,?,?,?,?,?,?,?,?,?,?,?,?,?)";//查询student数据表
		PreparedStatement stmt = this.connection().prepareStatement(sql);
		fillCreateStatement(stmt, entity);
		stmt.execute(sql);
	}
	public void createActivities(List<ActivityEntity> entities,Object ctx) throws Exception{
		String sql= "INSERT INTO wf_activity (id,flowId,superId,name,version,pathname,fromId,transitionName,status,actionType,state,createTime,creatorId,creatorName)VALUES(?,?,?,?,?,?,?,?,?,?,?,?,?,?)";//查询student数据表
		PreparedStatement stmt = this.connection().prepareStatement(sql);
		for(ActivityEntity entity : entities){
			fillCreateStatement(stmt, entity);
			stmt.execute(sql);
		}
		
	}
	static void fillCreateStatement(PreparedStatement stmt,ActivityEntity entity) throws Exception{
		stmt.setBytes(0,ActivityEntity.UUID2Bytes(entity.id()));
		stmt.setBytes(1,ActivityEntity.UUID2Bytes(entity.flowId()));
		stmt.setBytes(2,ActivityEntity.UUID2Bytes(entity.superId()));

		stmt.setString(3,entity.name());
		stmt.setString(4,entity.version());
		stmt.setString(5,entity.pathname());

		stmt.setBytes(6,entity.fromId()==null?null:ActivityEntity.UUID2Bytes(entity.fromId()));
		stmt.setString(7,entity.transitionName());

		stmt.setInt(8,entity.status().value());
		stmt.setString(9,entity.params());
		stmt.setString(10,entity.variables());
		stmt.setString(11,entity.results());

		stmt.setString(12, entity.actionType());
		stmt.setString(13, entity.state());

		stmt.setTimestamp(14,new Timestamp(entity.createTime().getTime()));
		stmt.setString(15,entity.creatorId());
		stmt.setString(16,entity.creatorName());

		stmt.setTimestamp(17,new Timestamp(entity.dealTime().getTime()));
		stmt.setString(18,entity.dealerId());
		stmt.setString(19,entity.dealerName());
	}

	

	public void entryActivity(ActivityEntity entity,Object ctx) throws Exception{
		String sql= "UPDATE wf_activity SET status=?,params=?,variables=?,dealTime=?,dealerId=?,dealerName=?,state=?,businessId=?,billId=?,taskId=? WHERE id=?";//查询student数据表
		PreparedStatement stmt = this.connection().prepareStatement(sql);
		fillDealStatement(stmt, entity);
		stmt.execute();
	}
	
	static void fillEntryStatement(PreparedStatement stmt,ActivityEntity entity) throws Exception{
		stmt.setInt(0,entity.status().value());
		stmt.setString(1,entity.params());
		stmt.setString(2,entity.variables());
		stmt.setString(3,entity.state());

		stmt.setTimestamp(4,new Timestamp(entity.dealTime().getTime()));
		stmt.setString(5,entity.dealerId());
		stmt.setString(6,entity.dealerName());

		stmt.setString(7,entity.businessId());
		stmt.setString(8,entity.billId());
		stmt.setString(9,entity.taskId());

		stmt.setBytes(7,ActivityEntity.UUID2Bytes(entity.id()));
	}

	public void dealActivity(ActivityEntity entity,Object ctx) throws Exception{
		String sql= "UPDATE wf_activity SET status=?,params=?,variables=?,results=?,dealTime=?,dealerId=?,dealerName=? WHERE id=?";//查询student数据表
		PreparedStatement stmt = this.connection().prepareStatement(sql);
		fillDealStatement(stmt, entity);
		stmt.execute();
	}
	static void fillDealStatement(PreparedStatement stmt,ActivityEntity entity) throws Exception{
		stmt.setInt(0,entity.status().value());
		stmt.setString(1,entity.params());
		stmt.setString(2,entity.variables());
		stmt.setString(3,entity.results());

		stmt.setTimestamp(4,new Timestamp(entity.dealTime().getTime()));
		stmt.setString(5,entity.dealerId());
		stmt.setString(6,entity.dealerName());
		stmt.setBytes(7,ActivityEntity.UUID2Bytes(entity.id()));
	}

	public void exitActivity(ActivityEntity entity,Object ctx) throws Exception{
		String sql= "UPDATE wf_activity SET status=?,dealTime=?,dealerId=?,dealerName=?,doneTime=? WHERE id=?";//查询student数据表
		PreparedStatement stmt = this.connection().prepareStatement(sql);
		fillDealStatement(stmt, entity);
		stmt.execute();
	}

	static void fillExitStatement(PreparedStatement stmt,ActivityEntity entity) throws Exception{
		stmt.setInt(0,entity.status().value());
		stmt.setTimestamp(1,new Timestamp(entity.dealTime().getTime()));
		stmt.setString(2,entity.dealerId());
		stmt.setString(3,entity.dealerName());
		stmt.setTimestamp(4,new Timestamp(entity.doneTime().getTime()));
		stmt.setBytes(5,ActivityEntity.UUID2Bytes(entity.id()));
	}
}
