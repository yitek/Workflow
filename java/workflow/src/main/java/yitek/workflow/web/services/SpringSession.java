package yitek.workflow.web.services;

import java.util.Arrays;
import java.util.function.Function;

import org.springframework.beans.BeansException;
import org.springframework.beans.factory.annotation.*;
import org.springframework.context.ApplicationContext;
import org.springframework.context.ApplicationContextAware;
import org.springframework.stereotype.*;
import org.springframework.transaction.*;
import org.springframework.transaction.support.*;
import yitek.workflow.core.*;

@Service
public class SpringSession extends LocalSession implements ApplicationContextAware {
	@Autowired
    private PlatformTransactionManager _transactionManager;

    @Autowired
    private ActivityRepository _activityRepository;

    @Override
    public ActivityRepository activityRepository(){
        return this._activityRepository;
    }

	@Override
    public <P,R> R transactional(P p,Function<P,R> fn) {
		
        TransactionStatus status = _transactionManager.getTransaction(new DefaultTransactionDefinition());
        try {
            R r = fn.apply(p);
            _transactionManager.commit(status);
            return r;
        } catch (Exception e) {
            _transactionManager.rollback(status);
            throw e;
        }
    }

    
    private static ApplicationContext _applicationContext;

    //@Override
    public void setApplicationContext(ApplicationContext applicationContext) throws BeansException {
        if(_applicationContext == null) {
            _applicationContext = applicationContext;

        }
        

        System.out.println("========ApplicationContext配置成功,在普通类可以通过调用SpringUtils.getAppContext()获取applicationContext对象,applicationContext="+_applicationContext+"========");

    }

    //获取applicationContext
    public static ApplicationContext getApplicationContext() {
        return _applicationContext;
    }

    //通过name获取 Bean.
    public static Object getBean(String name){
        return getApplicationContext().getBean(name);
    }

    //通过class获取Bean.
    public static <T> T getBean(Class<T> clazz){
        return getApplicationContext().getBean(clazz);
    }

    //通过name,以及Clazz返回指定的Bean
    public static <T> T getBean(String name,Class<T> clazz){
        return getApplicationContext().getBean(name, clazz);
    }
    @Override
    public Object resolveInstance(String name){
        return getBean(name);
    }
	public Action resolveAction(String actionType){ 
        Object obj = getBean(actionType);
        if(obj instanceof  Action) return (Action)obj;
        return null;
    }

}
