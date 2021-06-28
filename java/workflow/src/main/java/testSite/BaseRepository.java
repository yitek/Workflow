package testSite;


//import tk.mybatis.mapper.additional.insert.InsertListMapper;
import tk.mybatis.mapper.common.*;

public interface BaseRepository<E> extends BaseMapper<E>,MySqlMapper<E>{
	
}
