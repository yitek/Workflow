using System;
using System.Collections.Generic;
using System.Text;

namespace Flow
{
    public enum ActivityStates
    { 
        /// <summary>
        /// 正在初始化
        /// 如果该活动需要多个前活动完成进行，会在该状态停留
        /// </summary>
        Initializing,
        /// <summary>
        /// 已经初始化，内部状态可用
        /// </summary>
        Initialized,
        /// <summary>
        /// 已执行
        /// </summary>
        Executed,
        Done,
        Closed,
        /// <summary>
        /// 中途中止了
        /// </summary>
        Abort
    }
}
