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
	@Override
	public Connection connection() throws Exception{
		return this.dataSource.getConnection();
	}
	
}
