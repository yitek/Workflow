package yitek.workflow.web.services;

import java.sql.*;
import javax.sql.*;

import org.springframework.beans.factory.annotation.*;
import org.springframework.boot.autoconfigure.condition.*;
import org.springframework.context.annotation.*;
import org.springframework.stereotype.Service;

import yitek.workflow.core.*;

@Configuration
@Service
@ConditionalOnMissingBean({DataSource.class})
// @ConditionalOnProperty(
// 	name = {"spring.datasource.type"}
// )
public class SpringActivityRepository extends SqlActivityRepository{
	@Autowired
    DataSource dataSource ;

	public SpringActivityRepository(){

	}
	private Connection _conn;
	@Override
	public Connection connection() throws Exception{
		if(this._conn==null) {
			this._conn = this.dataSource.getConnection();
			//if(this._conn.isClosed()) this._conn.
		}
		return this._conn;
	}

	public void dispose() throws Exception{
		//if(this._conn!=null) this._conn.close();;
	}
	
}
