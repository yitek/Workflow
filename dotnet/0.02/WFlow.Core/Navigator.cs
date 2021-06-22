
using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using WFlow.Graphs;
using WFlow.Utils;

namespace WFlow
{
    public class Navigator:INavigator
    {
        public virtual NavigateResults Navigate(IActivity activity, IAssociation assoc, object executeResults, ProcessContext processContext)
        {
            //未指定Key,看是否已经完成
            if (string.IsNullOrEmpty(assoc.Key))
            {
                // 未完成，不能进行下一步
                if (activity.Status != ActivityStates.Done) return null;
            }
            // 看是状态是否满足
            else
            {
                if (activity.State<string>(assoc.Key) != assoc.Value) return null;
            }
            var nextDealerIdPath = assoc.NextDealerIdPath;
            if (string.IsNullOrEmpty(nextDealerIdPath))
            {
                nextDealerIdPath = processContext.Engine.DefaultNextDealerIdPath;
            }
            var nextDealerNamePath = assoc.NextDealerNamePath;
            if (string.IsNullOrEmpty(nextDealerNamePath))
            {
                nextDealerNamePath = processContext.Engine.DefaultNextDealerNamePath;
            }
            var nextDealerId = activity.State<string>(nextDealerIdPath);
            if (string.IsNullOrEmpty(nextDealerId)) throw new Exception(string.Format("活动对象{0}在执行导航{1}时，已经通过，但未能找到下一处理人", activity.Id, assoc.Name));
            var nextDealer = new User(nextDealerId, activity.State<string>(nextDealerNamePath));

            object nextInputs;
            if (assoc.NextInputsMaps != null)
            {
                var jObj = new JObject();
                foreach (var pair in assoc.NextInputsMaps)
                {
                    var value = activity.State(pair.Key);
                    JSON.SetPathValue(jObj, pair.Value, value);
                }
                nextInputs = jObj;
            }
            else nextInputs = executeResults;
            return new NavigateResults(nextDealer, nextInputs);
        }

        public virtual Task<NavigateResults> NavigateAsync(IActivity activity, IAssociation assoc, object executeResults, ProcessContext processContext)
        {
            throw new NotImplementedException();
        }
    }
}
