#工作流

+ ## 概述
  + ### 设计目标
  + ### 典型工作流分析
    + #### 请假申请流程
	+ #### 采购需求单流程
  + ### 工作流与状态机
    + #### 工作流定义
    + #### 有限状态机
  + ### 现有产品分析
    + #### activiti
    + #### microsoft workflow
  
+ ## 流程图 flowchart
  + ### 流程名称
  + ### 状态节点 node
    + #### 节点名称
    + #### 执行内容
  + ### 迁移连线 transition
    + #### 目标节点 toNode
    + #### 迁移判断 predicate
    + #### 数据传递 transports
  + ### 默认开始节点 start-node
  + ### 导出配置 exports
  + ### 可层叠的配置项 

+ ## 工作流 flow 
  + ### 启动 start(start-node-name)
  + ### 活动 activity
    + #### 活动状态 ActivityStates
    + #### 流程状态 flow-states 与 活动数据 Activity Datas
    + #### 激活数据 inputs
  + ### 状态迁移
    + #### 流程的状态迁移 state transfer
    + #### 活动的激活函数 active activity
    + #### 状态迁移判断
  + ### 活动的处理 proccess activity
    + #### 进入阶段 entry
    + #### 执行阶段 deal
    + #### 查找下一步的状态节点 nexts
    + #### 退出阶段 exit
  + ### 执行内容与活动钩子 action hook
  
+ ## 子流程 sub flow 
流程(flow)作为活动(activity)的执行内容
  + ### 子流程的定义与导入
  + ### 子流程的启动
	inputs穿透
  + ###  子流程的结束
  根据flowchart上的exports配置，导出数据到上级活动的活动数据
  + ### 子流程的数据 
  + ### 工作流作为活动

+ ## 流程上的操作
  + ### 撤回活动 recall
    + #### 撤销操作 undo
    + #### 活动冲入 reenter
  + ### 中止流程 abort flow
    + #### 挂起 suspend
    + #### 恢复 resume
    + #### 中止 abort
    + #### 中止审批 
  + ### 回拨 wheel back
+ ## 其他  
  + ### 处理过程的设计 Process
  + ### 嵌入与集成 embeded & integration
  + ### 并行 concurrent
    + #### 并行活动
    + #### 锁 lock
  + ### 错误处理与可重入设计
  + ### 存储




