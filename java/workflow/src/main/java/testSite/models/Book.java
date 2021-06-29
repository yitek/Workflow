package testSite.models;

import java.io.Serializable;

import javax.persistence.*;
import org.springframework.stereotype.Repository;
import lombok.Data;

@Data
@Repository
@Table(name = "wf_book")
public class Book implements Serializable,Bill<Book> {
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
