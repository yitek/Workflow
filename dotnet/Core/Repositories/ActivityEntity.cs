using System;
using System.Collections.Generic;
using System.Text;

namespace Flow.Repositories
{
    public class ActivityEntity
    {
        /// <summary>
        /// 唯一编号
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 上级活动编号
        /// </summary>
        public Guid? ParentId { get; set; }
        /// <summary>
        /// 流程Id(流程就是最顶层的活动)
        /// </summary>
        public Guid FlowId { get; set; }
        /// <summary>
        /// 当前活动节点名称
        /// </summary>
        public string NodeName { get; set; }
        /// <summary>
        /// 当前活动的节点路径名
        /// </summary>

        public string Fullname { get; set; }

        public string Version { get; set; }

        public string Graph { get; set; }

        public string InstanceType { get; set; }
        /// <summary>
        /// 当前流程所在状态
        /// </summary>
        public ActivityStates Status { get; set; }
        /// <summary>
        /// 序列化后的内部状态，一般采用json格式
        /// </summary>
        public string States { get; set; }
        /// <summary>
        /// 执行结果
        /// </summary>
        public string Results { get; set; }

        /// <summary>
        /// 该活动的上一个活动
        /// </summary>
        public Guid? PrevActivityId { get; set; }
        /// <summary>
        /// 该活动由那根线条导航过来的
        /// </summary>
        public string AssociationName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 创建者Id
        /// </summary>
        public string CreatorId { get; set; }
        /// <summary>
        /// 创建者账号
        /// </summary>
        public string CreatorName { get; set; }

        /// <summary>
        /// 最后处理时间
        /// </summary>
        public DateTime? DealTime { get; set; }
        /// <summary>
        /// 处理者Id
        /// 只有处理者才可以推进流程
        /// </summary>
        public string DealerId { get; set; }
        /// <summary>
        /// 更新者账号
        /// </summary>
        public string DealerName { get; set; }

        /// <summary>
        /// 关闭时间
        /// </summary>
        public DateTime? DoneTime { get; set; }

        /// <summary>
        /// 关闭时间
        /// </summary>
        public DateTime? CloseTime { get; set; }
        /// <summary>
        /// 关闭者Id
        /// 一般是dealer，但中途abort的就会是其他人
        /// </summary>
        public string CloserId { get; set; }
        /// <summary>
        /// 关闭者者账号
        /// </summary>
        public string CloserName { get; set; }

    }
}
