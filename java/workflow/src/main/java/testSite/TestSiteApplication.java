package testSite;

//import javax.sql.DataSource;

import org.springframework.boot.CommandLineRunner;
import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.EnableAutoConfiguration;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.context.ApplicationContext;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.ComponentScan;
import org.springframework.transaction.annotation.*;
//import org.springframework.context.annotation.*;
//import org.springframework.jdbc.datasource.DataSourceTransactionManager;
//import org.springframework.transaction.PlatformTransactionManager;
import org.springframework.web.servlet.config.annotation.ContentNegotiationConfigurer;

import tk.mybatis.spring.annotation.MapperScan;

import java.util.Arrays;

@SpringBootApplication
@EnableTransactionManagement
@ComponentScan({"yitek.workflow","testSite"})
@MapperScan("testSite.repositories")
public class TestSiteApplication {


	@Bean
	public CommandLineRunner commandLineRunner(ApplicationContext ctx) {
		return args -> {
			String[] beanNames = ctx.getBeanDefinitionNames();
			Arrays.sort(beanNames);
			for (String beanName : beanNames) {
				//System.out.println(beanName);
				if(beanName=="billStatusAction"){

				}
			}


		};
	}


	
	public static void main(String[] args) {
		SpringApplication.run(TestSiteApplication.class, args);
	}

}
