package yitek.workflow.core;

import java.util.*;

public interface ActivityRepository {
	List<ActivityEntity> ListLivedActivitiesById(UUID id);
	void SaveActivity(ActivityEntity entity);
	void AddActivity(ActivityEntity entity);
}
