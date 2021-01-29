using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace WFlow.Repositories
{
    public abstract class FlowRepository : IFlowRepository
    {
        static readonly string ActivityTableName = "wf_activities";
        static readonly string NavigationTableName = "wf_navigations";
        static readonly string _ActivityFields = @"
    id,
    parentId,
    flowId,
`   version,
    domain,
    nodeName,
    nodePath,
    graph,
    hasChildren,
    instanceType,
    status,
    states,
    results,
    createTime,
    creatorId,
    creatorName,
    dealTime,
    dealerId,
    dealerName,
    doneTime,
    closeTime,
    closerId,
    closerName
";
        static readonly string ActivityFields = CombineWhitespace(_ActivityFields);
        
        readonly static string _NaviationFields = @"
    fromActivityId,
    toActivityId,
    ownActivityId,
    flowId,
    graph,
    name,
    instanceType,
    nextDealerId,
    nextDealerName,
    value,
    createTime
";
        readonly static string NavigationFields = CombineWhitespace(_NaviationFields);
        readonly static string SetStatusSql = "status=?,dealTime=?,dealerId=?,dealerName=?,doneTime=?,closeTime=?,closerId=?,closerName=?";


        static readonly string SelectSql = @"SELECT " + ActivityFields + " \nFROM " + ActivityTableName + " WHERE ";
        readonly static string GetByIdSql = SelectSql + "Id=?";
        readonly static string ActivityGetByIdWithParentsSql = @"SELECT me." + ActivityFields.Replace(",",",me.") + "\n,p." + ActivityFields.Replace(",", ",p.") + "\n,f." + ActivityFields.Replace(",", ",p.") + " \nFROM " + ActivityTableName + " AS me \nLEFT JOIN " + ActivityTableName + " AS p ON p.Id=me.parentId \nLEFT JOIN " + ActivityTableName + " AS f ON f.Id=me.flowId \n" + " \nWHERE me.Id=?";
        readonly static string ActivityInsertSql = @"INSERT INTO " + ActivityTableName + "(" + ActivityFields + ") VALUES (?" + RepeatString(",?", 22) + ")";
        readonly static string ListByFlowIdSql = @"SELECT " + ActivityFields + " FROM " + ActivityTableName + " WHERE FlowId=?";
        readonly static string ListByParentIdSql = @"SELECT " + ActivityFields + " FROM " + ActivityTableName + " WHERE ParentId=?";
        readonly static string UpdateStatusSql = "UPDATE " + ActivityTableName + " SET " + SetStatusSql + " WHERE Id=?";
        readonly static string UpdateStatesSql = "UPDATE " + ActivityTableName + " SET States=? WHERE Id=?";

        readonly static string UpdateStatusAndStatesSql = "UPDATE " + ActivityTableName + " SET " + SetStatusSql + ",States=? WHERE Id=?";
        readonly static string UpdateStatusAndResultsSql = "UPDATE" + ActivityTableName + " SET " + SetStatusSql + ",Results=? WHERE Id=?";
        readonly static string UpdateResultsSql = "UPDATE " + ActivityTableName + " SET " + SetStatusSql + ",States=?,Results=? WHERE Id=?";

        readonly static string NavigationInsertSql = @"INSERT INTO " + NavigationTableName + "(" + NavigationFields + ") VALUES (?" + RepeatString(",?", 10) + ")";
        static string CombineWhitespace(string str) {
            var reg = new System.Text.RegularExpressions.Regex("\\w+");
            return reg.Replace(str," ");
        }
        void StatusValues(ActivityEntity entity,DbCommand cmd)
        {
            cmd.Parameters.Add(this.CreateParameter((int)entity.Status,"@status",DbType.Int32));
            cmd.Parameters.Add(this.CreateParameter(entity.DealTime, "@dealTime", DbType.DateTime));
            cmd.Parameters.Add(this.CreateParameter(entity.DealerId, "@dealerId", DbType.String));
            cmd.Parameters.Add(this.CreateParameter(entity.DealerName, "@dealerName", DbType.String));
            cmd.Parameters.Add(this.CreateParameter(entity.CloseTime, "@doneTime", DbType.DateTime));
            cmd.Parameters.Add(this.CreateParameter(entity.CloseTime, "@closeTime", DbType.DateTime));
            cmd.Parameters.Add(this.CreateParameter(entity.CloserId, "@closerId", DbType.String));
            cmd.Parameters.Add(this.CreateParameter(entity.CloserName, "@closerName", DbType.String));

        }

        void ActivityValues(ActivityEntity entity,DbCommand cmd)
        {
            cmd.Parameters.Add(this.CreateParameter(entity.Id,"@id",DbType.Guid));
            cmd.Parameters.Add(this.CreateParameter(entity.ParentId, "@parentId", DbType.Guid));
            cmd.Parameters.Add(this.CreateParameter(entity.FlowId, "@flowId", DbType.Guid));
            cmd.Parameters.Add(this.CreateParameter(entity.Version, "@version", DbType.String));
            cmd.Parameters.Add(this.CreateParameter(entity.Domain, "@domain", DbType.String));
            cmd.Parameters.Add(this.CreateParameter(entity.NodeName, "@nodeName", DbType.String));
            cmd.Parameters.Add(this.CreateParameter(entity.NodePath, "@nodePath", DbType.String));
            cmd.Parameters.Add(this.CreateParameter(entity.Graph, "@graph", DbType.String));
            cmd.Parameters.Add(this.CreateParameter(entity.HasChildren?1:0, "@hasChild", DbType.Int32));
            cmd.Parameters.Add(this.CreateParameter(entity.InstanceType, "@instType", DbType.String));
            cmd.Parameters.Add(this.CreateParameter((int)entity.Status, "@status", DbType.Int32));
            cmd.Parameters.Add(this.CreateParameter(entity.States, "@states", DbType.String));
            cmd.Parameters.Add(this.CreateParameter(entity.Results, "@results", DbType.String));
            cmd.Parameters.Add(this.CreateParameter(entity.CreateTime, "@createTime", DbType.DateTime));
            cmd.Parameters.Add(this.CreateParameter(entity.CreatorId, "@creatorId", DbType.String));
            cmd.Parameters.Add(this.CreateParameter(entity.CreatorName, "@creatorName", DbType.String));
            cmd.Parameters.Add(this.CreateParameter(entity.DealTime, "@dealTime", DbType.DateTime));
            cmd.Parameters.Add(this.CreateParameter(entity.DealerId, "@dealerId", DbType.String));
            cmd.Parameters.Add(this.CreateParameter(entity.DealerName, "@dealerName", DbType.String));
            cmd.Parameters.Add(this.CreateParameter(entity.DoneTime, "@doneTime", DbType.DateTime));
            cmd.Parameters.Add(this.CreateParameter(entity.CloseTime, "@closeTime", DbType.DateTime));
            cmd.Parameters.Add(this.CreateParameter(entity.CloserId, "@closerId", DbType.String));
            cmd.Parameters.Add(this.CreateParameter(entity.CloserName, "@closerName", DbType.String));
           
        }
        void NavigationValues(NavigationEntity entity, DbCommand cmd) {
            cmd.Parameters.Add(this.CreateParameter(entity.FromActivityId, "@fromId", DbType.Guid));
            cmd.Parameters.Add(this.CreateParameter(entity.ToActivityId, "@toId", DbType.Guid));
            cmd.Parameters.Add(this.CreateParameter(entity.OwnActivityId, "@ownId", DbType.Guid));
            cmd.Parameters.Add(this.CreateParameter(entity.FlowId, "@flowId", DbType.Guid));
            cmd.Parameters.Add(this.CreateParameter(entity.Graph, "@graph", DbType.String));
            cmd.Parameters.Add(this.CreateParameter(entity.Name, "@name", DbType.String));
            cmd.Parameters.Add(this.CreateParameter(entity.InstanceType, "@instType", DbType.String));
            cmd.Parameters.Add(this.CreateParameter(entity.NextDealerId, "@nDealerId", DbType.String));
            cmd.Parameters.Add(this.CreateParameter(entity.NextDealerName, "@nDealerName", DbType.String));
            cmd.Parameters.Add(this.CreateParameter(entity.Value, "@value", DbType.String));
            cmd.Parameters.Add(this.CreateParameter(entity.CreateTime, "@createTime", DbType.DateTime));
        }



        static string RepeatString(string str, int count)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < count; i++) sb.Append(str);
            return sb.ToString();
        }

        public System.Data.Common.DbConnection Connection { get; private set; }
        public FlowRepository(DbConnection conn)
        {
            this.Connection = conn;
        }
        public ITransaction CreateTransaction()
        {
            return new WFlow.Repositories.DbTransaction(this.Connection);
        }


        protected virtual DbCommand CreateCommand(System.Data.Common.DbTransaction trans, string sql,bool prepair=true)
        {
            DbConnection conn;
            if (trans == null)
            {
                conn = this.Connection;
                if (this.Connection.State == ConnectionState.Closed) this.Connection.Open();
            }
            else conn = trans.Connection;
            
            var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            cmd.Transaction = trans;
            if (prepair) cmd.Prepare();

            return cmd;
        }

        protected virtual async Task<DbCommand> CreateCommandAsync(System.Data.Common.DbTransaction trans, string sql, bool prepair = true)
        {
            DbConnection conn;
            if (trans == null)
            {
                conn = this.Connection;
                if (this.Connection.State == ConnectionState.Closed)  await this.Connection.OpenAsync();
            }
            else conn = trans.Connection;
            var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            cmd.Transaction = trans;
            if (prepair) await cmd.PrepareAsync();

            return cmd;
        }
        protected abstract DbParameter CreateParameter(object value, string name = null,DbType? type=null);

        private void TraceCommand(DbCommand cmd) {
            if (!Engine.Development) return;
            Console.WriteLine("[SQL]:" + DateTime.Now.ToString());
            Console.WriteLine(cmd.CommandText);
            foreach (DbParameter par in cmd.Parameters) {
                string valStr;
                if (par.Value == DBNull.Value || par.Value == null)
                {
                    valStr = "NULL";
                }
                else if (par.DbType == DbType.Boolean)
                {
                    valStr = (bool)par.Value == true ? "1" : "0";
                }
                else if (par.DbType == DbType.Int32 || par.DbType == DbType.Int16 || par.DbType == DbType.Int64 || par.DbType == DbType.Byte || par.DbType == DbType.Currency || par.DbType == DbType.Decimal || par.DbType == DbType.Single || par.DbType == DbType.UInt16 || par.DbType == DbType.UInt32 || par.DbType == DbType.UInt64)
                {
                    valStr = par.Value.ToString();
                }
                else valStr = "'" + par.Value.ToString().Replace("\'", "''") + "'";
                Console.WriteLine("SET " + par.ParameterName+"=" + valStr + ";");
                
            }
            Console.WriteLine("[/SQL]\n");
        }

        protected ActivityEntity FillActivityEntity(DbDataReader reader, ref int at,bool includeParents=false, ActivityEntity entity = null)
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
            entity.NodePath = reader.GetString(at++);
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
            if (includeParents) { 
                Guid? pId = (value = reader.GetValue(at)) == DBNull.Value ? null : new Nullable<Guid>((Guid)value);
                if (pId == null)
                {
                    entity.FlowActivity = entity;
                }
                else {
                    entity.ParentActivity = FillActivityEntity(reader,ref at,false);
                    entity.FlowActivity = FillActivityEntity(reader,ref at,false);
                }
            }
            return entity;
        }

        protected NavigationEntity FillNavigationEntity(DbDataReader reader,ref int at,bool includeActivity=false ,NavigationEntity entity = null) {
            if (entity == null) entity = new NavigationEntity();
            object value;
            entity.FromActivityId = reader.GetGuid(at++);
            entity.ToActivityId = reader.GetGuid(at++);
            entity.OwnActivityId = reader.GetGuid(at++);
            entity.FlowId = reader.GetGuid(at++);
            entity.Graph = reader.GetString(at++);
            entity.Name = reader.GetString(at++);
            entity.InstanceType = reader.GetString(at++);
            entity.NextDealerId = (value = reader.GetValue(at)) == DBNull.Value ? null : value.ToString();
            at++;
            entity.NextDealerName = (value = reader.GetValue(at)) == DBNull.Value ? null : value.ToString();
            at++;
            entity.CreateTime = reader.GetDateTime(at++);
            if (includeActivity) {
                entity.ToActivityEntity = FillActivityEntity(reader,ref at);
                Guid? fromId = (value = reader.GetValue(at)) == DBNull.Value ? null : new Nullable<Guid>((Guid)value);
                if(fromId!=null)entity.FromActivityEntity = FillActivityEntity(reader,ref at);
            }
            return entity;
        }




        public ActivityEntity GetById(Guid id, ITransaction trans = null)
        {
            using var cmd = this.CreateCommand(trans as DbTransaction, GetByIdSql);
            cmd.Parameters.Add(this.CreateParameter(id,"@id",DbType.Guid));
            TraceCommand(cmd);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                int at = 0;
                return FillActivityEntity(reader,ref at,false);
            }
            return null;
        }

        public async Task<ActivityEntity> GetByIdAsync(Guid id, ITransaction trans = null)
        {
            using var cmd = await this.CreateCommandAsync(trans as DbTransaction, GetByIdSql);
            cmd.Parameters.Add(this.CreateParameter(id, "@id", DbType.Guid));
            TraceCommand(cmd);
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                int at = 0;
                return FillActivityEntity(reader,ref at,false);
            }
            else return null;
        }

        public ActivityEntity GetWithParentsById(Guid id, ITransaction trans = null)
        {
            using var cmd = this.CreateCommand(trans as DbTransaction,ActivityGetByIdWithParentsSql);
            cmd.Parameters.Add(this.CreateParameter(id, "@id", DbType.Guid));
            TraceCommand(cmd);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                int at = 0;
                return FillActivityEntity(reader, ref at,true);
            }
            return null;
        }

        public async Task<ActivityEntity> GetWithParentsByIdAsync(Guid id, ITransaction trans = null)
        {
            using var cmd = await this.CreateCommandAsync(trans as DbTransaction, ActivityGetByIdWithParentsSql);
            cmd.Parameters.Add(this.CreateParameter(id, "@id", DbType.Guid));
            TraceCommand(cmd);
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                int at = 0;
                return FillActivityEntity(reader,ref at,true);
            }
            else return null;
        }


        public Guid Insert(ActivityEntity activity, ITransaction trans = null)
        {
            var id = activity.Id;
            if (id == Guid.Empty) id = activity.Id = Guid.NewGuid();

            using var cmd = this.CreateCommand(trans as DbTransaction, ActivityInsertSql);
            ActivityValues(activity, cmd);
            TraceCommand(cmd);
            return (cmd.ExecuteNonQuery() == 1) ? id : Guid.Empty;
        }

        public async Task<Guid> InsertAsync(ActivityEntity activity, ITransaction trans)
        {
            var id = activity.Id;
            if (id == Guid.Empty) id = activity.Id = Guid.NewGuid();

            using var cmd = await this.CreateCommandAsync(trans as DbTransaction, ActivityInsertSql);
            ActivityValues(activity, cmd);
            TraceCommand(cmd);
            if (await cmd.ExecuteNonQueryAsync() == 1) return id;
            return Guid.Empty;
        }

        public bool InsertNavigation(NavigationEntity entity, ITransaction trans = null)
        {
            

            using var cmd = this.CreateCommand(trans as DbTransaction, NavigationInsertSql);
            NavigationValues(entity, cmd);
            TraceCommand(cmd);
            return (cmd.ExecuteNonQuery() == 1);
        }

        public async Task<bool> InsertNavigationAsync(NavigationEntity entity, ITransaction trans)
        {
            
            using var cmd = await this.CreateCommandAsync(trans as DbTransaction, NavigationInsertSql);
            NavigationValues(entity, cmd);
            TraceCommand(cmd);
            return (await cmd.ExecuteNonQueryAsync() == 1);
        }

        public int Inserts(IList<ActivityEntity> activities, ITransaction trans = null)
        {
            int count = 0;
            using var cmd = this.CreateCommand(trans as DbTransaction, ActivityInsertSql);
            foreach (var activity in activities)
            {
                cmd.Parameters.Clear();
                ActivityValues(activity,cmd);
                TraceCommand(cmd);
                if (cmd.ExecuteNonQuery() == 1) count++;
            }


            return count;
        }

        public async Task<int> InsertsAsync(IList<ActivityEntity> activities, ITransaction trans = null)
        {
            int count = 0;
            using var cmd = await this.CreateCommandAsync(trans as DbTransaction, ActivityInsertSql);
            foreach (var activity in activities)
            {
                cmd.Parameters.Clear();
                ActivityValues(activity,cmd);
                TraceCommand(cmd);
                if (await cmd.ExecuteNonQueryAsync() == 1) count++;
            }
            return count;
        }



        public IList<ActivityEntity> ListByFlowId(Guid flowId, ITransaction trans = null)
        {
            var ret = new List<ActivityEntity>();
            using var cmd = this.CreateCommand(trans as DbTransaction, ListByFlowIdSql);
            cmd.Parameters.Add(this.CreateParameter(flowId, "@flowId", DbType.Guid));
            TraceCommand(cmd);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                int at = 0;
                ret.Add(FillActivityEntity(reader,ref at));
            }
            return ret;
        }

        public async Task<IList<ActivityEntity>> ListByFlowIdAsync(Guid flowId, ITransaction trans = null)
        {
            var ret = new List<ActivityEntity>();
            using var cmd = await this.CreateCommandAsync(trans as DbTransaction, ListByFlowIdSql);
            cmd.Parameters.Add(this.CreateParameter(flowId, "@flowId", DbType.Guid));
            TraceCommand(cmd);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                int at = 0;
                ret.Add(FillActivityEntity(reader,ref at));
            }
            return ret;
        }

        public IList<ActivityEntity> ListByParentId(Guid parentId, ITransaction trans = null)
        {
            var ret = new List<ActivityEntity>();
            using var cmd = this.CreateCommand(trans as DbTransaction, ListByParentIdSql);
            cmd.Parameters.Add(this.CreateParameter(parentId, "@parentId", DbType.Guid));
            TraceCommand(cmd);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                int at = 0;
                ret.Add(FillActivityEntity(reader,ref at));
            }
            return ret;
        }

        public async Task<IList<ActivityEntity>> ListByParentIdAsync(Guid parentId, ITransaction trans = null)
        {
            var ret = new List<ActivityEntity>();
            using var cmd = await this.CreateCommandAsync(trans as DbTransaction, ListByParentIdSql);
            TraceCommand(cmd);
            cmd.Parameters.Add(this.CreateParameter(parentId, "@parentId", DbType.Guid));
            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                int at = 0;
                ret.Add(FillActivityEntity(reader,ref at));
            }
            return ret;
        }

        public bool SaveStatus(ActivityEntity entity, ITransaction trans = null)
        {
            using var cmd = this.CreateCommand(trans as DbTransaction, UpdateStatusSql);
            StatusValues(entity, cmd);
            cmd.Parameters.Add(this.CreateParameter(entity.Id,"@id",DbType.Guid));
            TraceCommand(cmd);
            return cmd.ExecuteNonQuery() == 1;
        }

        public async Task<bool> SaveStatusAsync(ActivityEntity entity, ITransaction trans = null)
        {
            using var cmd = await this.CreateCommandAsync(trans as DbTransaction, UpdateStatusSql);
            StatusValues(entity,cmd);
            cmd.Parameters.Add(this.CreateParameter(entity.Id, "@id", DbType.Guid));
            TraceCommand(cmd);
            return await cmd.ExecuteNonQueryAsync() == 1;
        }

        public bool SaveStates(ActivityEntity entity, ITransaction trans = null)
        {
            using var cmd = this.CreateCommand(trans as DbTransaction, UpdateStatesSql);
            cmd.Parameters.Add(this.CreateParameter(entity.States, "@states", DbType.String));
            cmd.Parameters.Add(this.CreateParameter(entity.Id, "@id", DbType.Guid));
            TraceCommand(cmd);
            return cmd.ExecuteNonQuery() == 1;
        }

        public async Task<bool> SaveStatesAsync(ActivityEntity entity, ITransaction trans = null)
        {
            using var cmd = await this.CreateCommandAsync(trans as DbTransaction, UpdateStatesSql);
            cmd.Parameters.Add(this.CreateParameter(entity.States, "@states", DbType.String));
            cmd.Parameters.Add(this.CreateParameter(entity.Id, "@id", DbType.Guid));
            TraceCommand(cmd);
            return await cmd.ExecuteNonQueryAsync() == 1;
        }

        public bool SaveStatusAndStates(ActivityEntity entity, ITransaction trans = null)
        {
            using var cmd = this.CreateCommand(trans as DbTransaction, UpdateStatusAndStatesSql);
            StatusValues(entity, cmd);
            cmd.Parameters.Add(this.CreateParameter(entity.States,"@states",DbType.String));
            cmd.Parameters.Add(this.CreateParameter(entity.Id, "@id", DbType.Guid));
            TraceCommand(cmd);
            return cmd.ExecuteNonQuery() == 1;
        }
        public async Task<bool> SaveStatusAndStatesAsync(ActivityEntity entity, ITransaction trans = null)
        {
            using var cmd = await this.CreateCommandAsync(trans as DbTransaction, UpdateStatusAndStatesSql);
            cmd.Parameters.Add(entity.States);
            cmd.Parameters.Add(entity.Id);
            TraceCommand(cmd);
            return await cmd.ExecuteNonQueryAsync() == 1;
        }

        public bool SaveResults(ActivityEntity entity, ITransaction trans = null)
        {
            using var cmd = this.CreateCommand(trans as DbTransaction, UpdateResultsSql);
            StatusValues(entity, cmd);
            cmd.Parameters.Add(this.CreateParameter(entity.States, "@states", DbType.String));
            cmd.Parameters.Add(this.CreateParameter(entity.Results, "@results", DbType.String));
            cmd.Parameters.Add(this.CreateParameter(entity.Id, "@id", DbType.Guid));
            TraceCommand(cmd);
            return cmd.ExecuteNonQuery() == 1;
        }
        public async Task<bool> SaveResultsAsync(ActivityEntity entity, ITransaction trans = null)
        {
            using var cmd = await this.CreateCommandAsync(trans as DbTransaction, UpdateResultsSql);
            StatusValues(entity, cmd);
            cmd.Parameters.Add(this.CreateParameter(entity.States, "@states", DbType.String));
            cmd.Parameters.Add(this.CreateParameter(entity.Results, "@results", DbType.String));
            cmd.Parameters.Add(this.CreateParameter(entity.Id, "@id", DbType.Guid));
            TraceCommand(cmd);
            return await cmd.ExecuteNonQueryAsync() == 1;
        }

        public bool SaveStatusAndResults(ActivityEntity entity, ITransaction trans) {
            using var cmd = this.CreateCommand(trans as DbTransaction, UpdateStatusAndResultsSql);
            StatusValues(entity, cmd);
            cmd.Parameters.Add(this.CreateParameter(entity.Results, "@results", DbType.String));
            cmd.Parameters.Add(this.CreateParameter(entity.Id, "@id", DbType.Guid));
            TraceCommand(cmd);
            return cmd.ExecuteNonQuery() == 1;
        }
        public async Task<bool> SaveStatusAndResultsAsync(ActivityEntity entity, ITransaction trans) {
            using var cmd =await  this.CreateCommandAsync(trans as DbTransaction, UpdateStatusAndResultsSql);
            StatusValues(entity, cmd);
            cmd.Parameters.Add(this.CreateParameter(entity.Results, "@results", DbType.String));
            cmd.Parameters.Add(this.CreateParameter(entity.Id, "@id", DbType.Guid));
            TraceCommand(cmd);
            return await cmd.ExecuteNonQueryAsync() == 1;
        }
    }
}
