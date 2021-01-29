using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace WFlow.Repositories
{
    public interface IFlowRepository
    {
        DbConnection Connection { get; }

        ITransaction CreateTransaction();
        ActivityEntity GetById(Guid id, ITransaction trans = null);
        Task<ActivityEntity> GetByIdAsync(Guid id, ITransaction trans = null);

        ActivityEntity GetWithParentById(Guid id, ITransaction trans = null);
        Task<ActivityEntity> GetWithParentByIdAsync(Guid id, ITransaction trans = null);

        ActivityEntity GetWithParentsById(Guid id, ITransaction trans = null);
        Task<ActivityEntity> GetWithParentsByIdAsync(Guid id, ITransaction trans = null);

        Guid Insert(ActivityEntity activity, ITransaction trans = null);
        Task<Guid> InsertAsync(ActivityEntity activity, ITransaction trans);

        bool InsertNavigation(NavigationEntity entity, ITransaction trans = null);

        Task<bool> InsertNavigationAsync(NavigationEntity entity, ITransaction trans = null);

        int InsertNavigations(IEnumerable<NavigationEntity> entities, ITransaction trans = null);

        Task<int> InsertNavigationsAsync(IEnumerable<NavigationEntity> entities, ITransaction trans = null);
        int Inserts(IList<ActivityEntity> activities, ITransaction trans = null);
        Task<int> InsertsAsync(IList<ActivityEntity> activities, ITransaction trans = null);
        IList<ActivityEntity> ListByFlowId(Guid flowId, ITransaction trans = null);
        Task<IList<ActivityEntity>> ListByFlowIdAsync(Guid flowId, ITransaction trans = null);
        IList<ActivityEntity> ListByParentId(Guid parentId, ITransaction trans = null);
        Task<IList<ActivityEntity>> ListByParentIdAsync(Guid parentId, ITransaction trans = null);
        bool SaveResults(ActivityEntity entity, ITransaction trans = null);
        Task<bool> SaveResultsAsync(ActivityEntity entity, ITransaction trans = null);
        bool SaveStates(ActivityEntity entity, ITransaction trans = null);
        Task<bool> SaveStatesAsync(ActivityEntity entity, ITransaction trans = null);
        bool SaveStatus(ActivityEntity entity, ITransaction trans = null);
        bool SaveStatusAndStates(ActivityEntity entity, ITransaction trans = null);
        Task<bool> SaveStatusAndStatesAsync(ActivityEntity entity, ITransaction trans = null);
        Task<bool> SaveStatusAsync(ActivityEntity entity, ITransaction trans = null);
        bool SaveStatusAndResults(ActivityEntity entity, ITransaction trans);
        Task<bool> SaveStatusAndResultsAsync(ActivityEntity entity, ITransaction trans);
    }
}