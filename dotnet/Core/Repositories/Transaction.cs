using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Flow.Repositories
{
    public abstract class Transaction : ITransaction
    {
        protected Transaction(object conn,object trans = null) {
            this.Connection = conn;
            this.Raw = trans;
        }

        public virtual object Raw { get; protected set; }

        public object Connection { get; private set; }

        int count;
        public bool TryBegin()
        {
            this.count++;
            if (count == 1) {
                this.Begin();
                return true;
            }
            return false;
            
        }

        protected abstract void Begin();

        public async Task<bool> TryBeginAsync()
        {
            this.count++;
            if (count == 1)
            {
                await this.BeginAsync();
                return true;
            }
            return false;
        }

        protected abstract Task BeginAsync();

        public bool TryCommit()
        {
            if (--this.count == 0) { this.Commit();  return true; }
            if (this.count < 0) throw new Exception("多余的Commit");
            return false;

        }

        protected abstract void Commit();

        public async Task<bool> TryCommitAsync()
        {
            if (--this.count == 0) { await this.CommitAsync(); return true; }
            if (this.count < 0) throw new Exception("多余的Commit");
            return false;

        }

        protected abstract Task CommitAsync();

        public void TryRollback()
        {
            if (this.count == 0) this.Rollback();
        }

        protected abstract void Rollback();

        public Task TryRollbackAsync()
        {
            throw new NotImplementedException();
        }

        protected abstract Task RollbackAsync();
    }
}
