package testSite.models;

import java.io.Serializable;

import javax.persistence.Table;
import org.springframework.stereotype.Repository;
import lombok.Data;

@Data
@Repository
@Table(name = "wf_book")
public class Book implements Serializable,Bill<Book> {
	int id;
	String billNo;
	String title;
	String status;
	String creatorId;
	String creatorName;
	String customerId;
	String customerName;

}
