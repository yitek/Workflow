using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WFlow.Graphs;

namespace WFlow
{
    public interface INavigator
    {
        NavigateResults Navigate(IActivity activity, IAssociation assoc, object executeResults, ProcessContext processContext);
        Task<NavigateResults> NavigateAsync(IActivity activity, IAssociation assoc, object executeResults, ProcessContext processContext);
    }
}
