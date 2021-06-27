package yitek.workflow.core;

public enum ActivityStates {
	error(-1),
	created(0),
	entried(1),
	dealing(2),
	dealed(3),
	exited(4);
	private int value;
	private ActivityStates(int value) {     //必须是private的，否则编译错误  
        this.value = value;  
    }  
  
    public static ActivityStates valueOf(int value) {   //手写的从int到enum的转换函数  
        switch (value) {  
			case -1: return error;
			case 0 : return created;
        	case 1: return entried;  
        	case 2:  return dealing;
			case 3: return dealed;
			case 4: return exited;  
        default:  
            return null;  
        }  
    }  
  
    public int value() {  
        return this.value;  
    } 
}
