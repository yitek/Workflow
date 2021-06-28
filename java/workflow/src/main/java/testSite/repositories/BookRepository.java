package testSite.repositories;

import org.springframework.stereotype.Service;
import testSite.BaseRepository;
import testSite.models.Book;

@Service
public interface BookRepository extends BaseRepository<Book> {
	
}
