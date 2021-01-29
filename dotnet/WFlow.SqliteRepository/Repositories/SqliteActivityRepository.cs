using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;
using System.Data.Common;
using System.Data;

namespace WFlow.Repositories
{
    public class SqliteFlowRepository: FlowRepository
    {
        public SqliteFlowRepository(SQLiteConnection connection) : base(connection) { }

        public SqliteFlowRepository(string connectionString) : base(new SQLiteConnection(connectionString)) { 
        }

        protected override DbParameter CreateParameter(object value, string name = null,DbType? type=null)
        {
            var param = new SQLiteParameter
            {
                Value = value ?? DBNull.Value,
                ParameterName = name
            };
            if (type!=null)param.DbType = type.Value;
            return param;
            
        }
    }
}
