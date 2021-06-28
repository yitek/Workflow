package testSite.repositories;

import org.apache.ibatis.annotations.*;
import org.springframework.stereotype.Service;

import testSite.BaseRepository;
import testSite.models.Task;

@Service
public interface TaskRepository extends BaseRepository<Task> {
	// @Select("select * from wf_task where name = #{id}")
    // public Task getById(Integer id);

	// @Insert("INSERT INTO wf_task (title,dealerId,dealerName) VALUES (#{title},#{dealerId},#{dealerName},0)")
	// @Options(useGeneratedKeys = true, keyProperty = "id")
	// int Add(Task task);

	// @Insert("INSERT INTO wf_task (title,dealerId,dealerName) VALUES (#{title},#{dealerId},#{dealerName})")
	// @Options(useGeneratedKeys = true, keyProperty = "id")
	// public int Add(Task task);
	@Update("UPDATE wf_task SET status=1 WHERE id=#{id}")
	void complete(Integer id);
}
