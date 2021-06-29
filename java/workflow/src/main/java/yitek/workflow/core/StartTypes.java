package yitek.workflow.core;


public enum StartTypes {
	notStart(0),
	diagramStart(1),
	flowStart(3);
	private int value;
	private StartTypes(int value) {     //必须是private的，否则编译错误  
        this.value = value;  
    }  
  
    public static StartTypes valueOf(int value) {   //手写的从int到enum的转换函数  
        switch (value) {  
			case 0 : return notStart;
        	case 1: return diagramStart;  
        	case 3:  return flowStart;  
        default:  
            return null;  
        }  
    }  
  
    public int value() {  
        return this.value;  
    } 
}
