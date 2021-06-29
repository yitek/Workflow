package testSite.models;

import lombok.Data;

import java.io.Serializable;

import javax.persistence.*;

import org.springframework.stereotype.Repository;


@Data
@Repository
@Table(name = "wf_design")
public class Design  implements Serializable,Bill<Design>{
	@Id
	@GeneratedValue(strategy = GenerationType.IDENTITY)
	Integer id;
	String billNo;
	String title;
	String status; 
	String creatorId;
	String creatorName;
	String customerId;
	String customerName;
}
