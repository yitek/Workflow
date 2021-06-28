package testSite.configs;

import tk.mybatis.spring.annotation.MapperScan;
import org.mybatis.spring.boot.autoconfigure.ConfigurationCustomizer;
import org.springframework.context.annotation.*;

@Configuration
@MapperScan("testSite.repositories")
public class MyBatisConfig {
	
	@Bean
    public ConfigurationCustomizer configurationCustomizer(){
        return new ConfigurationCustomizer() {
            @Override
            public void customize(org.apache.ibatis.session.Configuration configuration) {
                configuration.setLazyLoadingEnabled(true);
                configuration.setMapUnderscoreToCamelCase(true);
                //configuration.setLogImpl(Log4jImpl.class);
            }
        };
    }
}
