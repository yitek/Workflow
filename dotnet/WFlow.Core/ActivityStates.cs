using System;
using System.Collections.Generic;
using System.Text;

namespace WFlow
{
    public enum ActivityStates
    { 
        /// <summary>
        /// 创建中，尚未执行过Predicate
        /// </summary>
        Creating,
        /// <summary>
        /// 已经创建，至少执行过一次predicate，但未通过检查
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
