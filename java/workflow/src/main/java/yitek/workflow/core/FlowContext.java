package yitek.workflow.core;

public interface FlowContext {
	// 从依赖注入中获取实例
	Object resolveInstance(String name);
	Action resolveAction(String actionType);
	
	
}
