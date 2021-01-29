using System;
using System.Collections.Generic;
using System.Text;

namespace WFlow
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
        /// 已经创建，但尚未通过predicate
        /// </summary>
        Created,
        Dealing,
        /// <summary>
        /// 已执行,但尚未完成
        /// 下次还会继续执行
        /// </summary>
        Dealed,
        /// <summary>
        /// 已经执行，并达成目标状态
        /// </summary>
        Done,
        /// <summary>
        /// 中途中止了
        /// </summary>
        Canceled,
        Error
    }
}
