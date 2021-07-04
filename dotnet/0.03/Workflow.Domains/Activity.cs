using System;
using System.Collections.Generic;
using System.Text;

namespace Workflow.Domains
{
    public class Activity
    {
        public Guid Id { get; set; }
        /// <summary>
        /// 状态图
        /// </summary>
        public State BluePrint { get; set; }

        public string this[string statusName] { get { return null; } }

        public ActivityStates Status { get; set; }

        public Activity SupperActivity { get; set; }

        /// <summary>
        /// 挂起的子活动
        /// </summary>
        public IEnumerable<Activity> PaddingActivities { get; set; }
        virtual protected bool Initialize(IDictionary<string, string> data, IDealer dealer, object? context) { return true; }

        virtual protected IDictionary<string,string> Deal(IDictionary<string, string> data, IDealer dealer, object? context) { return true; }


        public readonly static IDictionary<string, string> Dealing = new Dictionary<string, string>();
        public bool Execute(IDictionary<string, string> data,IDealer dealer,object context=null) {
            if (this.Initialize(data,dealer,context)) {
                this.Status = ActivityStates.Dealing;
                // 保存Activity
                // ....
                var result =  this.Deal(data,dealer,context);
                if (result == null) {
                    this.Status = ActivityStates.Padding;
                } else if (result == Dealing) {
                    this.Status = ActivityStates.Dealing;
                }
                if (this.Status == ActivityStates.Dealed) { 
                    // save result
                    //
                }

            }
            return true;
        }
        protected bool Transite(IDealer dealer,IDictionary<string,string> result, object context) {
            // 循环蓝图中的迁移对象
            foreach(var transaction in this.BluePrint.Transactions) { 
                //做迁移判断
            }
        }

        bool TransitCheck(Transiction transition) { 
            
        }
    }
}