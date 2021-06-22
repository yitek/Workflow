using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using WFlow.Entities;

namespace WFlow.Repositories
{
    public interface IFlowRepository
    {
        

        ITransaction CreateTransaction(object context);
        #region Activity
        #region Inserts
        Guid InsertActivity(ActivityEntity activity, ITransaction trans = null);
        Task<Guid> InsertActivityAsync(ActivityEntity activity, ITransaction trans);

        int InsertActivities(IEnumerable<ActivityEntity> activities, ITransaction trans = null);
        Task<int> InsertActivitiesAsync(IEnumerable<ActivityEntity> activities, ITransaction trans = null);

        #endregion

        #region get

        ActivityEntity GetActivityRuntimeById(Guid id, ITransaction trans = null);
        Task<ActivityEntity> GetActivityRuntimeByIdAsync(Guid id, ITransaction trans = null);

        ActivityEntity GetActivityRuntimeWithParentById(Guid id, ITransaction trans = null);
        Task<ActivityEntity> GetActivityRuntimeWithParentByIdAsync(Guid id, ITransaction trans = null);
        #endregion

        #region list
        //IList<ActivityEntity> ListActivitiesByFlowId(Guid flowId, ITransaction trans = null);
        //Task<IList<ActivityEntity>> ListActivitiesByFlowIdAsync(Guid flowId, ITransaction trans = null);
        IList<ActivityEntity> ListActivitiesRuntimeByParentId(Guid parentId, ITransaction trans = null);
        Task<IList<ActivityEntity>> ListActivitiesRuntimeByParentIdAsync(Guid parentId, ITransaction trans = null);
        #endregion

        #region update
        bool SaveActivityStates(ActivityEntity entity,ITransaction trans= null);
        Task<bool> SaveActivityStatesAsync(ActivityEntity entity, ITransaction trans = null);

        bool SaveActivityStatesAndValues(ActivityEntity entity,IDictionary<string,string> values, ITransaction trans = null);
        Task<bool> SaveActivityStatesAndValuesAsync(ActivityEntity entity,IDictionary<string ,string> values, ITransaction trans = null);

        
        #endregion

        #endregion

        #region navigation

        bool InsertNavigation(NavigationEntity entity, ITransaction trans = null);

        Task<bool> InsertNavigationAsync(NavigationEntity entity, ITransaction trans = null);

        int InsertNavigations(IEnumerable<NavigationEntity> entities, ITransaction trans = null);

        Task<int> InsertNavigationsAsync(IEnumerable<NavigationEntity> entities, ITransaction trans = null);

        #endregion

    }
}