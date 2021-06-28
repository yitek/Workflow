package yitek.workflow.core;

import java.util.*;

public interface ActivityRepository {
	ActivityEntity getActivityById(UUID id) throws Exception;
	ActivityEntity getActivityByTaskId(String id) throws Exception;

	List<ActivityEntity> listLivedActivitiesById(UUID id)throws Exception;
	List<ActivityEntity> listActivitiesBySuperId(UUID supperId)throws Exception;
	Integer countLivedSubordinatesBySuperId(UUID superId)throws Exception;
	List<ActivityEntity> listNextActivities(UUID fromId)throws Exception;
	boolean removeNextActivities(UUID fromId)throws Exception;

	void entryActivity(ActivityEntity entity)throws Exception;
	void dealActivity(ActivityEntity entity)throws Exception;
	void exitActivity(ActivityEntity entity)throws Exception;
	void createActivity(ActivityEntity entity)throws Exception;
	void createActivities(List<ActivityEntity> entities)throws Exception;
}
