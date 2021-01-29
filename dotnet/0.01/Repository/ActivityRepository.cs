using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace Flow.Repositories
{
    public class ActivityRepository : IActivityRepository
    {
        static readonly string TableSql = "flow_activities";
        static readonly string FieldsSql = @"
    Id,
    ParentId,
    FlowId,
    NodeName,
    Fullname,
    Version,
    Graph,
    InstanceType,
    Status,
    States,
    PrevActivityId,
    AssociationName,
    CreateTime,
    CreatorId,
    CreatorName,
    DealTime,
    DealerId,
    DealerName,
    DoneTime,
    CloseTime,
    CloserId,
    CloserName
";
        readonly static string SetStatusSql = "Status=?,DealTime=?,DealerId=?,DealerName=?,DoneTime=?,CloseTime=?,CloserId=?,CloserName=?";


        static readonly string SelectSql = @"SELECT " + FieldsSql + " FROM " + TableSql + " WHERE ";
        readonly static string GetByIdSql = SelectSql + "Id=?";
        readonly static string InsertSql = @"INSERT INTO " + TableSql + "(" + FieldsSql + ") VALUES (?" + RepeatString(",?", 21) + ")";
        readonly static string ListByFlowIdSql = @"SELECT " + FieldsSql + " FROM " + TableSql + "WHERE FlowId=?";
        readonly static string ListByParentIdSql = @"SELECT " + FieldsSql + " FROM " + TableSql + "WHERE ParentId=?";
        readonly static string UpdateStatusSql = "UPDATE " + TableSql + " SET " + SetStatusSql + " WHERE Id=?";
        readonly static string UpdateStatesSql = "UPDATE " + TableSql + " SET States=? WHERE Id=?";

        readonly static string UpdateStatusAndStatesSql = "UPDATE " + TableSql + " SET " + SetStatusSql + ",States=? WHERE Id=?";
        readonly static string UpdateStatusAndResultsSql = "UPDATE" + TableSql + " SET " + SetStatusSql + ",Results=? WHERE Id=?";
        readonly static string UpdateResultsSql = "UPDATE " + TableSql + " SET " + SetStatusSql + ",States=?,Results=? WHERE Id=?";


        static object[] StatusValues(ActivityEntity entity)
        {
            return new object[] {
                (int)entity.Status,
                entity.DealTime,
                entity.DealerId,
                entity.DealerName,
                entity.DoneTime,
                entity.CloseTime,
                entity.CloserId,
                entity.CloserName
            };
        }


        static object[] EntityValues(ActivityEntity entity)
        {
            return new object[] {
                entity.Id,
        entity.ParentId,
        entity.FlowId,
        entity.NodeName,
        entity.Fullname,
        entity.Version,
        entity.Graph,
        entity.InstanceType,
        entity.Status,
        entity.States,
        entity.PrevActivityId,
        entity.AssociationName,
        entity.CreateTime,
        entity.CreatorId,
        entity.CreatorName,
        entity.DealTime,
        entity.DealerId,
        entity.DealerName,
        entity.DoneTime,
        entity.CloseTime,
        entity.CloserId,
        entity.CloserName
            };
        }

        static string RepeatString(string str, int count)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < count; i++) sb.Append(str);
            return sb.ToString();
        }

        public System.Data.Common.DbConnection Connection { get; private set; }
        public ActivityRepository(DbConnection conn)
        {
            this.Connection = conn;
        }
        public ITransaction CreateTransaction()
        {
            return new Flow.Repositories.DbTransaction(this.Connection);
        }


        protected virtual DbCommand CreateCommand(System.Data.Common.DbTransaction trans, string sql, params object[] args)
        {
            if (trans.Connection.State == ConnectionState.Closed) trans.Connection.Open();
            var cmd = trans.Connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.Transaction = trans;
            if (args != null && args.Length > 0)
            {
                cmd.Parameters.AddRange(args);
                cmd.Prepare();
            }

            return cmd;
        }

        protected virtual async Task<DbCommand> CreateCommandAsync(System.Data.Common.DbTransaction trans, string sql, params object[] args)
        {
            if (this.Connection.State == ConnectionState.Closed) await this.Connection.OpenAsync();
            var cmd = this.Connection.CreateCommand();
            cmd.CommandText = GetByIdSql;
            cmd.Transaction = trans;
            if (args != null && args.Length > 0)
            {
                cmd.Parameters.AddRange(args);
                await cmd.PrepareAsync();
            }

            return cmd;
        }

        ActivityEntity FillEntity(DbDataReader reader, int at = 0, ActivityEntity entity = null)
        {
            if (entity == null) entity = new ActivityEntity();
            object value;
            //Id,
            entity.Id = reader.GetGuid(at++);
            //ParentId,
            entity.ParentId = (value = reader.GetValue(at)) == DBNull.Value ? null : new Nullable<Guid>((Guid)value);
            at++;
            //FlowId,
            entity.FlowId = reader.GetGuid(at++);
            //NodeName,
            entity.NodeName = reader.GetString(at++);
            //Fullname,
            entity.Fullname = reader.GetString(at++);
            //Version,
            entity.Version = reader.GetString(at++);
            //Graph,
            entity.Graph = reader.GetString(at++);
            //InstanceType,
            entity.InstanceType = (value = reader.GetValue(at)) == DBNull.Value ? null : value.ToString();
            at++;
            //Status,
            entity.Status = (ActivityStates)reader.GetInt32(at++);
            //States,
            entity.States = reader.GetString(at++);
            //PrevActivityId,
            entity.PrevActivityId = (value = reader.GetValue(at)) == DBNull.Value ? null : new Nullable<Guid>((Guid)value); ;
            at++;
            //AssociationName,
            entity.AssociationName = (value = reader.GetValue(at)) == DBNull.Value ? null : value.ToString();
            at++;
            //CreateTime,
            entity.CreateTime = reader.GetDateTime(at++);
            //CreatorId,
            entity.CreatorId = reader.GetString(at++);
            //CreatorName,
            entity.CreatorName = reader.GetString(at++);
            //ExecuteTime,
            entity.DealTime = (value = reader.GetValue(at)) == DBNull.Value ? null : new Nullable<DateTime>((DateTime)value);
            at++;
            //DealerId,
            entity.DealerId = (value = reader.GetValue(at)) == DBNull.Value ? null : value.ToString();
            at++;
            //DealerName,
            entity.DealerName = (value = reader.GetValue(at)) == DBNull.Value ? null : value.ToString();
            at++;
            //DoneTime,
            entity.DoneTime = (value = reader.GetValue(at)) == DBNull.Value ? null : new Nullable<DateTime>((DateTime)value);
            at++;
            //CloseTime,
            entity.CloseTime = (value = reader.GetValue(at)) == DBNull.Value ? null : new Nullable<DateTime>((DateTime)value);
            at++;
            //CloserId,
            entity.CloserId = (value = reader.GetValue(at)) == DBNull.Value ? null : value.ToString();
            at++;
            //CloserName
            entity.CloserName = (value = reader.GetValue(at)) == DBNull.Value ? null : value.ToString();
            //at++;
            return entity;
        }


        public ActivityEntity GetById(Guid id, ITransaction trans = null)
        {
            using var cmd = this.CreateCommand(trans as DbTransaction, GetByIdSql, id);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return FillEntity(reader);
            }
            return null;
        }

        public async Task<ActivityEntity> GetByIdAsync(Guid id, ITransaction trans = null)
        {
            using var cmd = await this.CreateCommandAsync(trans as DbTransaction, GetByIdSql, id);
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return FillEntity(reader);
            }
            else return null;
        }

        public Guid Insert(ActivityEntity activity, ITransaction trans = null)
        {
            var id = activity.Id;
            if (id == Guid.Empty) id = activity.Id = Guid.NewGuid();

            using var cmd = this.CreateCommand(trans as DbTransaction, InsertSql, EntityValues(activity));
            return (cmd.ExecuteNonQuery() == 1) ? id : Guid.Empty;
        }

        public async Task<Guid> InsertAsync(ActivityEntity activity, ITransaction trans)
        {
            var id = activity.Id;
            if (id == Guid.Empty) id = activity.Id = Guid.NewGuid();

            using var cmd = await this.CreateCommandAsync(trans as DbTransaction, InsertSql, EntityValues(activity));
            if (await cmd.ExecuteNonQueryAsync() == 1) return id;
            return Guid.Empty;
        }

        public int Inserts(IList<ActivityEntity> activities, ITransaction trans = null)
        {
            int count = 0;
            using var cmd = this.CreateCommand(trans as DbTransaction, InsertSql);
            cmd.Prepare();
            foreach (var activity in activities)
            {
                cmd.Parameters.Clear();
                cmd.Parameters.AddRange(EntityValues(activity));
                if (cmd.ExecuteNonQuery() == 1) count++;
            }


            return count;
        }

        public async Task<int> InsertsAsync(IList<ActivityEntity> activities, ITransaction trans = null)
        {
            int count = 0;
            using var cmd = await this.CreateCommandAsync(trans as DbTransaction, InsertSql);
            cmd.Prepare();
            foreach (var activity in activities)
            {
                cmd.Parameters.Clear();
                cmd.Parameters.AddRange(EntityValues(activity));
                if (await cmd.ExecuteNonQueryAsync() == 1) count++;
            }
            return count;
        }



        public IList<ActivityEntity> ListByFlowId(Guid flowId, ITransaction trans = null)
        {
            var ret = new List<ActivityEntity>();
            using var cmd = this.CreateCommand(trans as DbTransaction, ListByFlowIdSql, flowId);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ret.Add(FillEntity(reader));
            }
            return ret;
        }

        public async Task<IList<ActivityEntity>> ListByFlowIdAsync(Guid flowId, ITransaction trans = null)
        {
            var ret = new List<ActivityEntity>();
            using var cmd = await this.CreateCommandAsync(trans as DbTransaction, ListByFlowIdSql, flowId);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                ret.Add(FillEntity(reader));
            }
            return ret;
        }

        public IList<ActivityEntity> ListByParentId(Guid parentId, ITransaction trans = null)
        {
            var ret = new List<ActivityEntity>();
            using var cmd = this.CreateCommand(trans as DbTransaction, ListByParentIdSql, parentId);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ret.Add(FillEntity(reader));
            }
            return ret;
        }

        public async Task<IList<ActivityEntity>> ListByParentIdAsync(Guid parentId, ITransaction trans = null)
        {
            var ret = new List<ActivityEntity>();
            using var cmd = await this.CreateCommandAsync(trans as DbTransaction, ListByParentIdSql, parentId);
            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                ret.Add(FillEntity(reader));
            }
            return ret;
        }

        public bool SaveStatus(ActivityEntity entity, ITransaction trans = null)
        {
            using var cmd = this.CreateCommand(trans as DbTransaction, UpdateStatusSql, StatusValues(entity));
            cmd.Parameters.Add(entity.Id);
            return cmd.ExecuteNonQuery() == 1;
        }

        public async Task<bool> SaveStatusAsync(ActivityEntity entity, ITransaction trans = null)
        {
            using var cmd = await this.CreateCommandAsync(trans as DbTransaction, UpdateStatusSql, StatusValues(entity));
            cmd.Parameters.Add(entity.Id);
            return await cmd.ExecuteNonQueryAsync() == 1;
        }

        public bool SaveStates(ActivityEntity entity, ITransaction trans = null)
        {
            using var cmd = this.CreateCommand(trans as DbTransaction, UpdateStatesSql, new object[] { entity.States, entity.Id });
            return cmd.ExecuteNonQuery() == 1;
        }

        public async Task<bool> SaveStatesAsync(ActivityEntity entity, ITransaction trans = null)
        {
            using var cmd = await this.CreateCommandAsync(trans as DbTransaction, UpdateStatesSql, new object[] { entity.States, entity.Id });
            return await cmd.ExecuteNonQueryAsync() == 1;
        }

        public bool SaveStatusAndStates(ActivityEntity entity, ITransaction trans = null)
        {
            using var cmd = this.CreateCommand(trans as DbTransaction, UpdateStatusAndStatesSql, StatusValues(entity));
            cmd.Parameters.Add(entity.States);
            cmd.Parameters.Add(entity.Id);
            return cmd.ExecuteNonQuery() == 1;
        }
        public async Task<bool> SaveStatusAndStatesAsync(ActivityEntity entity, ITransaction trans = null)
        {
            using var cmd = await this.CreateCommandAsync(trans as DbTransaction, UpdateStatusAndStatesSql, StatusValues(entity));
            cmd.Parameters.Add(entity.States);
            cmd.Parameters.Add(entity.Id);
            return await cmd.ExecuteNonQueryAsync() == 1;
        }

        public bool SaveResults(ActivityEntity entity, ITransaction trans = null)
        {
            using var cmd = this.CreateCommand(trans as DbTransaction, UpdateResultsSql, StatusValues(entity));
            cmd.Parameters.Add(entity.States);
            cmd.Parameters.Add(entity.Results);
            cmd.Parameters.Add(entity.Id);
            return cmd.ExecuteNonQuery() == 1;
        }
        public async Task<bool> SaveResultsAsync(ActivityEntity entity, ITransaction trans = null)
        {
            using var cmd = await this.CreateCommandAsync(trans as DbTransaction, UpdateResultsSql, StatusValues(entity));
            cmd.Parameters.Add(entity.States);
            cmd.Parameters.Add(entity.Results);
            cmd.Parameters.Add(entity.Id);
            return await cmd.ExecuteNonQueryAsync() == 1;
        }

        public bool SaveStatusAndResults(ActivityEntity entity, ITransaction trans) {
            using var cmd = this.CreateCommand(trans as DbTransaction, UpdateStatusAndResultsSql, StatusValues(entity));
            cmd.Parameters.Add(entity.Results);
            cmd.Parameters.Add(entity.Id);
            return cmd.ExecuteNonQuery() == 1;
        }
        public async Task<bool> SaveStatusAndResultsAsync(ActivityEntity entity, ITransaction trans) {
            using var cmd =await  this.CreateCommandAsync(trans as DbTransaction, UpdateStatusAndResultsSql, StatusValues(entity));
            cmd.Parameters.Add(entity.Results);
            cmd.Parameters.Add(entity.Id);
            return await cmd.ExecuteNonQueryAsync() == 1;
        }
    }
}
