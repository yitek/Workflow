package yitek.workflow.core;

import java.util.*;

public interface ActivityRepository {
	ActivityEntity getActivityById(UUID id) throws Exception;

	List<ActivityEntity> listLivedActivitiesById(UUID id)throws Exception;

	Integer countLivedSubsidariesIdsBySuperId(UUID superId)throws Exception;

	void entryActivity(ActivityEntity entity,Object ctx)throws Exception;
	void dealActivity(ActivityEntity entity,Object ctx)throws Exception;
	void exitActivity(ActivityEntity entity,Object ctx)throws Exception;
	void createActivity(ActivityEntity entity,Object ctx)throws Exception;
	void createActivities(List<ActivityEntity> entities,Object ctx)throws Exception;
}
