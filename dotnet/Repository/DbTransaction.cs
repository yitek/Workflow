using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace Flow.Repositories
{
    public class DbTransaction : Transaction
    {
        public new System.Data.Common.DbTransaction Raw { get; private set; }
        public new DbConnection Connection { get; private set; }

        public DbTransaction(DbConnection connection):base(connection) {
            this.Connection = connection;
            
        }
        protected override void Begin()
        {
            if (this.Connection.State == ConnectionState.Closed) this.Connection.Open();
            if (this.Raw != null) {
                throw new Exception("数据库事务已经开始");
            }
            base.Raw = this.Raw = this.Connection.BeginTransaction();
        }

        protected override async Task BeginAsync()
        {
            if (this.Connection.State == ConnectionState.Closed) await this.Connection.OpenAsync();
            if (this.Raw != null)
            {
                throw new Exception("数据库事务已经开始");
            }
            base.Raw = this.Raw = await this.Connection.BeginTransactionAsync();
        }

        protected override void Commit()
        {
            this.Raw.Commit();
        }

        protected override async Task CommitAsync()
        {
            await this.Raw.CommitAsync();
        }

        protected override void Rollback()
        {
            this.Raw.Rollback();
        }

        protected override async Task RollbackAsync()
        {
            await this.Raw.RollbackAsync();
        }

        public static implicit operator System.Data.Common.DbTransaction(Flow.Repositories.DbTransaction tran) {
            return tran?.Raw;
        } 
    }
}
