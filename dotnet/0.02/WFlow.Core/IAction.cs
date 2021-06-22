using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WFlow.Graphs;

namespace WFlow
{
    public interface IAction:INavigator
    {
        /// <summary>
        /// 执行前检查
        /// Creating/Created阶段调用
        /// 如果有返回，且返回为有效值，准备完成，进入Dealing阶段
        /// 所谓有效值是指:返回的用户对象的Id!=null
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="inputs"></param>
        /// <param name="dealer"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        IUser Predicate(IActivity activity, object inputs,  ProcessContext processContext);

        Task<IUser> PredicateAsync(IActivity activity, object inputs,  ProcessContext processContext);

        object Execute(IActivity activity, object inputs,  ProcessContext processContext);

        Task<object> ExecuteAsync(IActivity activity, object inputs,  ProcessContext processContext);

        bool Postdicate(IActivity activity, ProcessContext context);

        Task<bool> PostdicateAsync (IActivity activity, ProcessContext processContext);

        

    }
}
