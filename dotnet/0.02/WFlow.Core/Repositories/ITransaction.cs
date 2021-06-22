using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WFlow.Repositories
{
    public interface ITransaction
    {
        object Connection { get; }

        object Raw { get; }
        bool TryBegin();
        Task<bool> TryBeginAsync();

        bool TryCommit();

        Task<bool> TryCommitAsync();

        void TryRollback();

        Task TryRollbackAsync();
    }
}
