package WF.activities;

public enum  ActivityStates {
    Initialized(1),
    Dealed(2),
    Done(3),
    End(4);
    //必须增加一个构造函数,变量,得到该变量的值
    private int  mState=0;
    private ActivityStates(int value)
    {
    mState=value;
    }
    /**
    * @return 枚举变量实际返回值
    */
    public int getState()
    {
    return mState;
    }  
}
