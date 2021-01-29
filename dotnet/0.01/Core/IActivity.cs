using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WFlow.Graphs;

namespace WFlow
{
    public interface IActivity
    {
        bool Predicate(object inputs, IUser dealer, IReadOnlyState states, Context context);

        Task<bool> PredicateAsync(object inputs, IUser dealer, IReadOnlyState states, Context context);

        object Execute(object inputs, IUser dealer, IReadOnlyState states,Context context);

        Task<object> ExecuteAsync(object inputs, IUser dealer, IReadOnlyState states, Context context);

        bool Postdicate(object executeResults, IReadOnlyState states, Context context);

        Task<bool> PostdicateAsync (object executeResults, IReadOnlyState states, Context context);

        NavigateResults Navigate(IArrow arrow, IReadOnlyState states, Context context);
        Task<NavigateResults> NavigateAsync(IArrow arrow, IReadOnlyState states, Context context);

    }
}
