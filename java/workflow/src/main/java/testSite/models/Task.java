package testSite.models;

import lombok.Data;

import java.io.Serializable;

import javax.persistence.*;
import org.springframework.stereotype.Repository;

@Data
@Repository
@Table(name = "wf_task")
public class Task  implements Serializable{
	public Task(){}
	@Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
	Integer id;
	String title;
	String dealerId;
	String dealerName;
	String activityId;
	int status;
}
