using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using WFlow.Entities;

namespace WFlow.Repositories
{
    public abstract class FlowRepository : IFlowRepository
    {
        public System.Data.Common.DbConnection Connection { get; private set; }
        public FlowRepository(DbConnection conn)
        {
            this.Connection = conn;
        }

        #region activity

        #region activity table and fields
        static readonly string ActivityTableName = "wf_activities";
        static readonly string ActivityRuntimeFields = 
 CombineWhitespace(@"id
,parentId
,flowId
,domain
,nodeName
,nodePath
,version
,hasChildren
,actionType
,status
,ownerId
,ownerName
,states
,nexts
,graph");
        static readonly string ActivityRecordFields =
CombineWhitespace(@"
,createTime
,creatorId
,creatorName
,dealTime
,dealerId
,dealerName
,doneTime
,closeTime
,closerId
,closerName
");
        static readonly string ActivityAllFields = ActivityRuntimeFields+ "," + ActivityRecordFields + ",results,inputs";

        
        #endregion activity table and fields

        #region activity command maker
        void ActivityStatusValues(ActivityEntity entity,DbCommand cmd)
        {
            cmd.Parameters.Add(this.CreateParameter(entity.States, "@states", DbType.String));
            cmd.Parameters.Add(this.CreateParameter((int)entity.Status,"@status",DbType.Int32));
            cmd.Parameters.Add(this.CreateParameter(entity.DealTime, "@dealTime", DbType.DateTime));
            cmd.Parameters.Add(this.CreateParameter(entity.DealerId, "@dealerId", DbType.String));
            cmd.Parameters.Add(this.CreateParameter(entity.DealerName, "@dealerName", DbType.String));
            cmd.Parameters.Add(this.CreateParameter(entity.CloseTime, "@doneTime", DbType.DateTime));
            cmd.Parameters.Add(this.CreateParameter(entity.CloseTime, "@closeTime", DbType.DateTime));
            cmd.Parameters.Add(this.CreateParameter(entity.CloserId, "@closerId", DbType.String));
            cmd.Parameters.Add(this.CreateParameter(entity.CloserName, "@closerName", DbType.String));

        }
        void ActivityRuntimeValues(ActivityEntity entity, DbCommand cmd) {
            cmd.Parameters.Add(this.CreateParameter(entity.Id, "@id", DbType.Guid));
            cmd.Parameters.Add(this.CreateParameter(entity.ParentId, "@parentId", DbType.Guid));
            cmd.Parameters.Add(this.CreateParameter(entity.FlowId, "@flowId", DbType.Guid));
            cmd.Parameters.Add(this.CreateParameter(entity.Domain, "@domain", DbType.String));
            cmd.Parameters.Add(this.CreateParameter(entity.NodeName, "@nodeName", DbType.String));
            cmd.Parameters.Add(this.CreateParameter(entity.NodePath, "@nodePath", DbType.String));
            cmd.Parameters.Add(this.CreateParameter(entity.Version, "@version", DbType.String));
            cmd.Parameters.Add(this.CreateParameter(entity.HasChildren ? 1 : 0, "@hasChild", DbType.Int32));
            cmd.Parameters.Add(this.CreateParameter(entity.ActionType, "@actionType", DbType.String));
            cmd.Parameters.Add(this.CreateParameter((int)entity.Status, "@status", DbType.Int32));
            cmd.Parameters.Add(this.CreateParameter(entity.OwnerId, "@ownerId", DbType.String));
            cmd.Parameters.Add(this.CreateParameter(entity.OwnerName, "@ownerName", DbType.String));
            cmd.Parameters.Add(this.CreateParameter(entity.States, "@states", DbType.String));
            cmd.Parameters.Add(this.CreateParameter(entity.Nexts, "@nexts", DbType.String));
            cmd.Parameters.Add(this.CreateParameter(entity.Graph, "@graph", DbType.String));

        }
        void ActivityRecordValues(ActivityEntity entity, DbCommand cmd)
        {
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
        void ActivityAllValues(ActivityEntity entity,DbCommand cmd)
        {
            this.ActivityRuntimeValues(entity,cmd);
            this.ActivityRecordValues(entity,cmd);
            cmd.Parameters.Add(this.CreateParameter(entity.Results, "@results", DbType.String));
            cmd.Parameters.Add(this.CreateParameter(entity.Inputs, "@inputs", DbType.String));
        }
        
        #endregion activity command maker

        #region entity filler
        protected ActivityEntity FillActivityRuntimeFields(DbDataReader reader, ref int at, bool includeParents = false, ActivityEntity entity = null)
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
            entity.Domain = reader.GetString(at++);
            //NodeName,
            entity.NodeName = reader.GetString(at++);
            //Fullname,
            entity.NodePath = reader.GetString(at++);
            
            //Version,
            entity.Version = reader.GetString(at++);

            entity.HasChildren = reader.GetInt32(at++) == 1;
            
            //InstanceType,
            entity.ActionType = (value = reader.GetValue(at++)) == DBNull.Value ? null : value.ToString();
            at++;
            //Status,
            entity.Status = (ActivityStates)reader.GetInt32(at++);

            entity.OwnerId = (value = reader.GetValue(at++)) == DBNull.Value ? null : value.ToString();

            entity.OwnerName = (value = reader.GetValue(at++)) == DBNull.Value ? null : value.ToString();
            //States,
            entity.States = reader.GetString(at++);

            entity.Nexts = (value = reader.GetValue(at++)) == DBNull.Value ? null : value.ToString();

            entity.Graph = reader.GetString(at++);
            if (includeParents)
            {
                Guid? pId = (value = reader.GetValue(at)) == DBNull.Value ? null : new Nullable<Guid>((Guid)value);
                if (pId == null)
                {
                    entity.FlowActivity = entity;
                }
                else
                {
                    entity.ParentActivity = FillActivityRuntimeFields(reader, ref at, false);
                    
                }
            }
            return entity;
        }

        protected ActivityEntity FillActivityRecordFields(DbDataReader reader, ref int at, ActivityEntity entity = null)
        {
            if (entity == null) entity = new ActivityEntity();
            object value;
           

            //CreateTime,
            entity.CreateTime = reader.GetDateTime(at++);
            //CreatorId,
            entity.CreatorId = reader.GetString(at++);
            //CreatorName,
            entity.CreatorName = reader.GetString(at++);
            //ExecuteTime,
            entity.DealTime = (value = reader.GetValue(at++)) == DBNull.Value ? null : new Nullable<DateTime>((DateTime)value);
            //DealerId,
            entity.DealerId = (value = reader.GetValue(at++)) == DBNull.Value ? null : value.ToString();
            //DealerName,
            entity.DealerName = (value = reader.GetValue(at++)) == DBNull.Value ? null : value.ToString();
            //DoneTime,
            entity.DoneTime = (value = reader.GetValue(at++)) == DBNull.Value ? null : new Nullable<DateTime>((DateTime)value);
            //CloseTime,
            entity.CloseTime = (value = reader.GetValue(at++)) == DBNull.Value ? null : new Nullable<DateTime>((DateTime)value);
            //CloserId,
            entity.CloserId = (value = reader.GetValue(at++)) == DBNull.Value ? null : value.ToString();
            //CloserName
            entity.CloserName = (value = reader.GetValue(at++)) == DBNull.Value ? null : value.ToString();
            return entity;
        }
        protected ActivityEntity FillActivityAllFields(DbDataReader reader, ref int at, ActivityEntity entity = null) {
            if (entity == null) entity = new ActivityEntity();
            this.FillActivityRuntimeFields(reader,ref at,false,entity);
            this.FillActivityRecordFields(reader,ref at,entity);
            object value;
            entity.Results = (value = reader.GetValue(at++)) == DBNull.Value ? null : value.ToString();
            entity.Inputs = (value = reader.GetValue(at++)) == DBNull.Value ? null : value.ToString();
            return entity;
        }

        #endregion entity filler

        #region insert activity
        readonly static string InsertActivitySql = "INSERT INTO " + ActivityTableName + " \n(" + ActivityAllFields + ") VALUES\n(?" + RepeatString(",?", 26) + ")";
        public Guid InsertActivity(ActivityEntity activity, ITransaction trans = null)
        {
            var id = activity.Id;
            if (id == Guid.Empty) id = activity.Id = Guid.NewGuid();

            using var cmd = this.CreateCommand(trans as DbTransaction, InsertActivitySql);
            ActivityAllValues(activity, cmd);
            TraceCommand(cmd);
            return (cmd.ExecuteNonQuery() == 1) ? id : Guid.Empty;
        }

        public async Task<Guid> InsertActivityAsync(ActivityEntity activity, ITransaction trans)
        {
            var id = activity.Id;
            if (id == Guid.Empty) id = activity.Id = Guid.NewGuid();

            using var cmd = await this.CreateCommandAsync(trans as DbTransaction, InsertActivitySql);
            ActivityAllValues(activity, cmd);
            TraceCommand(cmd);
            if (await cmd.ExecuteNonQueryAsync() == 1) return id;
            return Guid.Empty;
        }

        public int InsertActivities(IEnumerable<ActivityEntity> activities, ITransaction trans = null)
        {
            int count = 0;
            using var cmd = this.CreateCommand(trans as DbTransaction, InsertActivitySql, true);
            foreach (var activity in activities)
            {
                cmd.Parameters.Clear();
                ActivityAllValues(activity, cmd);
                TraceCommand(cmd);
                if (cmd.ExecuteNonQuery() == 1) count++;
            }


            return count;
        }

        public async Task<int> InsertActivitiesAsync(IEnumerable<ActivityEntity> activities, ITransaction trans = null)
        {
            int count = 0;
            using var cmd = await this.CreateCommandAsync(trans as DbTransaction, InsertActivitySql, true);
            foreach (var activity in activities)
            {
                cmd.Parameters.Clear();
                ActivityAllValues(activity, cmd);
                TraceCommand(cmd);
                if (await cmd.ExecuteNonQueryAsync() == 1) count++;
            }
            return count;
        }
        #endregion insert activity

        #region get activity

        readonly static string GetActivityRuntimeByIdSql = @"SELECT " + ActivityRuntimeFields + " \nFROM " + ActivityTableName + " WHERE Id=?";


        public ActivityEntity GetActivityRuntimeById(Guid id, ITransaction trans = null)
        {
            using var cmd = this.CreateCommand(trans as DbTransaction, GetActivityRuntimeByIdSql);
            cmd.Parameters.Add(this.CreateParameter(id,"@id",DbType.Guid));
            TraceCommand(cmd);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                int at = 0;
                return FillActivityRuntimeFields(reader,ref at,false);
            }
            return null;
        }

        public async Task<ActivityEntity> GetActivityRuntimeByIdAsync(Guid id, ITransaction trans = null)
        {
            using var cmd = await this.CreateCommandAsync(trans as DbTransaction, GetActivityRuntimeByIdSql);
            cmd.Parameters.Add(this.CreateParameter(id, "@id", DbType.Guid));
            TraceCommand(cmd);
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                int at = 0;
                return FillActivityRuntimeFields(reader,ref at,false);
            }
            else return null;
        }
        readonly static string GetActivityRunimeTimeByIdWithParentSql = @"SELECT __tbCurrent__." + ActivityRuntimeFields.Replace(",", ",__tbCurrent__.") + "\n,__tbParent__." + ActivityRuntimeFields.Replace(",", ",__tbParent__.") + "\nFROM " + ActivityTableName + " AS __tbCurrent__ \nLEFT JOIN " + ActivityTableName + " AS __tbParent__ ON __tbParent__.Id=__tbCurrent__.parentId\nWHERE __tbCurrent__.Id=?";

        public ActivityEntity GetActivityRuntimeWithParentById(Guid id, ITransaction trans = null)
        {
            using var cmd = this.CreateCommand(trans as DbTransaction, GetActivityRunimeTimeByIdWithParentSql);
            cmd.Parameters.Add(this.CreateParameter(id, "@id", DbType.Guid));
            TraceCommand(cmd);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                int at = 0;
                return FillActivityRuntimeFields(reader, ref at,true);
            }
            return null;
        }

        public async Task<ActivityEntity> GetActivityRuntimeWithParentByIdAsync(Guid id, ITransaction trans = null)
        {
            using var cmd = await this.CreateCommandAsync(trans as DbTransaction, GetActivityRunimeTimeByIdWithParentSql);
            cmd.Parameters.Add(this.CreateParameter(id, "@id", DbType.Guid));
            TraceCommand(cmd);
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                int at = 0;
                return FillActivityRuntimeFields(reader,ref at,true);
            }
            else return null;
        }
        #endregion

        #region list activities

        readonly static string ListActivitiesRuntimeByParentIdSql = @"SELECT " + ActivityRuntimeFields + " FROM " + ActivityTableName + " WHERE ParentId=?";
        public IList<ActivityEntity> ListActivitiesRuntimeByParentId(Guid parentId, ITransaction trans = null)
        {
            var ret = new List<ActivityEntity>();
            using var cmd = this.CreateCommand(trans as DbTransaction, ListActivitiesRuntimeByParentIdSql);
            cmd.Parameters.Add(this.CreateParameter(parentId, "@parentId", DbType.Guid));
            TraceCommand(cmd);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                int at = 0;
                ret.Add(FillActivityRuntimeFields(reader, ref at,false));
            }
            return ret;
        }

        public async Task<IList<ActivityEntity>> ListActivitiesRuntimeByParentIdAsync(Guid parentId, ITransaction trans = null)
        {
            var ret = new List<ActivityEntity>();
            using var cmd = await this.CreateCommandAsync(trans as DbTransaction, ListActivitiesRuntimeByParentIdSql);
            TraceCommand(cmd);
            cmd.Parameters.Add(this.CreateParameter(parentId, "@parentId", DbType.Guid));
            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                int at = 0;
                ret.Add(FillActivityRuntimeFields(reader, ref at,false));
            }
            return ret;
        }
        #endregion

        

        #region update
        readonly static string SetStatesSql = "states=?,status=?,dealTime=?,dealerId=?,dealerName=?,doneTime=?,closeTime=?,closerId=?,closerName=?";
        readonly static string SaveStatesSql = "UPDATE " + ActivityTableName + " SET " + SetStatesSql + " WHERE id=?";
        
        public bool SaveActivityStates(ActivityEntity entity, ITransaction trans = null) {
            using var cmd = this.CreateCommand(trans as DbTransaction, SaveStatesSql);
            ActivityStatusValues(entity, cmd);
            cmd.Parameters.Add(this.CreateParameter(entity.Id, "@id", DbType.Guid));
            TraceCommand(cmd);
            return cmd.ExecuteNonQuery() == 1;
        }
        public async Task<bool> SaveActivityStatesAsync(ActivityEntity entity, ITransaction trans = null) {
            using var cmd = await this.CreateCommandAsync(trans as DbTransaction, SaveStatesSql);
            ActivityStatusValues(entity, cmd);
            cmd.Parameters.Add(this.CreateParameter(entity.Id, "@id", DbType.Guid));
            TraceCommand(cmd);
            return await cmd.ExecuteNonQueryAsync() == 1;
        }

        public bool SaveActivityStatesAndValues(ActivityEntity entity, IDictionary<string, string> values, ITransaction trans = null) {
            using var cmd = this.CreateCommand(trans as DbTransaction,null);
            var sql = "UPDATE " + ActivityTableName + " SET " + SetStatesSql;
            ActivityStatusValues(entity,cmd);
            if (values != null && values.Count > 0) {
                foreach (var pair in values) {
                    sql += $",${pair.Key}=?";
                    cmd.Parameters.Add(this.CreateParameter(pair.Value,"@" + pair.Key,DbType.String));
                }
            }
            sql += " WHERE id=?";
            cmd.Parameters.Add(this.CreateParameter(entity.Id, "@id", DbType.Guid));
            cmd.CommandText = sql;

            cmd.Prepare();
            TraceCommand(cmd);
            return cmd.ExecuteNonQuery() == 1;
        }
        public async Task<bool> SaveActivityStatesAndValuesAsync(ActivityEntity entity, IDictionary<string, string> values, ITransaction trans = null) {
            using var cmd = this.CreateCommand(trans as DbTransaction, null);
            var sql = "UPDATE " + ActivityTableName + " SET " + SetStatesSql;
            ActivityStatusValues(entity, cmd);
            if (values != null && values.Count > 0)
            {
                foreach (var pair in values)
                {
                    sql += $",${pair.Key}=?";
                    cmd.Parameters.Add(this.CreateParameter(pair.Value, "@" + pair.Key, DbType.String));
                }
            }
            sql += " WHERE id=?";
            cmd.Parameters.Add(this.CreateParameter(entity.Id, "@id", DbType.Guid));
            cmd.CommandText = sql;

            await cmd.PrepareAsync();
            TraceCommand(cmd);
            return await cmd.ExecuteNonQueryAsync() == 1;
        }

        #endregion update

        #endregion activity

        #region navigation

        #region navigation table and fields
        static readonly string NavigationTableName = "wf_navigations";
        readonly static string NavigationRuntimeFields =
CombineWhitespace(@"fromActivityId
,toActivityId
,parentActivityId
,flowId
,name
,navigatorType
,createTime");
        readonly static string NavigationRecordFields =
CombineWhitespace(@"nextDealerId
,nextDealerName
,nextInputs
,prevResults
,graph");
        readonly static string NavigationAllFields = NavigationRuntimeFields + "," + NavigationRecordFields;
        #endregion

        #region navigation command maker
        void NavigationRuntimeValues(NavigationEntity entity, DbCommand cmd)
        {
            cmd.Parameters.Add(this.CreateParameter(entity.FromActivityId, "@fromId", DbType.Guid));
            cmd.Parameters.Add(this.CreateParameter(entity.ToActivityId, "@toId", DbType.Guid));
            cmd.Parameters.Add(this.CreateParameter(entity.ParentActivityId, "@ownId", DbType.Guid));
            cmd.Parameters.Add(this.CreateParameter(entity.FlowId, "@flowId", DbType.Guid));
            cmd.Parameters.Add(this.CreateParameter(entity.Name, "@name", DbType.String));
            cmd.Parameters.Add(this.CreateParameter(entity.NavigatorType, "@instType", DbType.String));
            cmd.Parameters.Add(this.CreateParameter(entity.CreateTime, "@createTime", DbType.DateTime));
            cmd.Parameters.Add(this.CreateParameter(entity.NextDealerId, "@nDealerId", DbType.String));
            cmd.Parameters.Add(this.CreateParameter(entity.NextDealerName, "@nDealerName", DbType.String));
            cmd.Parameters.Add(this.CreateParameter(entity.NextInputs, "@nInputs", DbType.String));
            cmd.Parameters.Add(this.CreateParameter(entity.PrevResults, "@prevResults", DbType.String));
            cmd.Parameters.Add(this.CreateParameter(entity.Graph, "@graph", DbType.String));
        }
        void NavigationRecordValues(NavigationEntity entity, DbCommand cmd)
        {
            cmd.Parameters.Add(this.CreateParameter(entity.NextDealerId, "@nDealerId", DbType.String));
            cmd.Parameters.Add(this.CreateParameter(entity.NextDealerName, "@nDealerName", DbType.String));
            cmd.Parameters.Add(this.CreateParameter(entity.NextInputs, "@nInputs", DbType.String));
            cmd.Parameters.Add(this.CreateParameter(entity.PrevResults, "@prevResults", DbType.String));
            cmd.Parameters.Add(this.CreateParameter(entity.Graph, "@graph", DbType.String));
        }

        void NavigationAllValues(NavigationEntity entity, DbCommand cmd) {
            this.NavigationRuntimeValues(entity,cmd);
            this.NavigationRecordValues(entity,cmd);
        }
        #endregion

        #region navigation entity filler
        protected NavigationEntity FillNavigationRuntimeFields(DbDataReader reader, ref int at, bool includeActivity = false, NavigationEntity entity = null)
        {
            if (entity == null) entity = new NavigationEntity();
            object value;
            entity.FromActivityId = reader.GetGuid(at++);
            entity.ToActivityId = reader.GetGuid(at++);
            entity.ParentActivityId = reader.GetGuid(at++);
            entity.FlowId = reader.GetGuid(at++);
            entity.Name = reader.GetString(at++);
            entity.NavigatorType = reader.GetString(at++);
            entity.CreateTime = reader.GetDateTime(at++);
            

            if (includeActivity)
            {
                entity.ToActivityEntity = FillActivityRuntimeFields(reader, ref at);
                Guid? fromId = (value = reader.GetValue(at)) == DBNull.Value ? null : new Nullable<Guid>((Guid)value);
                if (fromId != null) entity.FromActivityEntity = FillActivityRuntimeFields(reader, ref at);
            }
            return entity;
        }

        protected NavigationEntity FillNavigationRecordFields(DbDataReader reader, ref int at,  NavigationEntity entity = null)
        {
            if (entity == null) entity = new NavigationEntity();
            object value;
            entity.NextDealerId = (value=reader.GetValue(at++))==DBNull.Value?null:value.ToString();
            entity.NextDealerName = (value = reader.GetValue(at++)) == DBNull.Value ? null : value.ToString();
            entity.NextInputs = (value = reader.GetValue(at++)) == DBNull.Value ? null : value.ToString();
            entity.PrevResults = (value = reader.GetValue(at++)) == DBNull.Value ? null : value.ToString();
            entity.Graph = (value = reader.GetValue(at++)) == DBNull.Value ? null : value.ToString();
            return entity;
        }
        #endregion

        #region insert navigations
        readonly static string InsertNavigationSql = "INSERT INTO " + NavigationTableName + "\n(" + NavigationAllFields + ") VALUES \n(?"+ RepeatString(",?",11) + ")";
        public bool InsertNavigation(NavigationEntity entity, ITransaction trans = null)
        {


            using var cmd = this.CreateCommand(trans as DbTransaction, InsertNavigationSql);
            NavigationAllValues(entity, cmd);
            TraceCommand(cmd);
            return (cmd.ExecuteNonQuery() == 1);
        }

        public async Task<bool> InsertNavigationAsync(NavigationEntity entity, ITransaction trans)
        {

            using var cmd = await this.CreateCommandAsync(trans as DbTransaction, InsertNavigationSql);
            NavigationAllValues(entity, cmd);
            TraceCommand(cmd);
            return (await cmd.ExecuteNonQueryAsync() == 1);
        }

        public int InsertNavigations(IEnumerable<NavigationEntity> entities, ITransaction trans = null)
        {
            int count = 0;
            using var cmd = this.CreateCommand(trans as DbTransaction, InsertNavigationSql, true);
            foreach (var entity in entities)
            {
                cmd.Parameters.Clear();
                NavigationAllValues(entity, cmd);
                TraceCommand(cmd);
                if (cmd.ExecuteNonQuery() == 1) count++;
            }


            return count;
        }

        public async Task<int> InsertNavigationsAsync(IEnumerable<NavigationEntity> entities, ITransaction trans = null)
        {
            int count = 0;
            using var cmd = await this.CreateCommandAsync(trans as DbTransaction, InsertNavigationSql, true);
            foreach (var entity in entities)
            {
                cmd.Parameters.Clear();
                NavigationAllValues(entity, cmd);
                TraceCommand(cmd);
                if (await cmd.ExecuteNonQueryAsync() == 1) count++;
            }


            return count;
        }
        #endregion

        #endregion

        #region utils

        static string RepeatString(string str, int count)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < count; i++) sb.Append(str);
            return sb.ToString();
        }
        static string CombineWhitespace(string str)
        {
            var reg = new System.Text.RegularExpressions.Regex("\\w+");
            return reg.Replace(str, " ");
        }

        public ITransaction CreateTransaction(object context=null)
        {
            return new WFlow.Repositories.DbTransaction(this.Connection);
        }


        protected virtual DbCommand CreateCommand(System.Data.Common.DbTransaction trans, string sql, bool prepair = true)
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
            if (prepair && sql!=null) cmd.Prepare();

            return cmd;
        }

        protected virtual async Task<DbCommand> CreateCommandAsync(System.Data.Common.DbTransaction trans, string sql, bool prepair = true)
        {
            DbConnection conn;
            if (trans == null)
            {
                conn = this.Connection;
                if (this.Connection.State == ConnectionState.Closed) await this.Connection.OpenAsync();
            }
            else conn = trans.Connection;
            var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            cmd.Transaction = trans;
            if (prepair && sql !=null) await cmd.PrepareAsync();

            return cmd;
        }
        protected abstract DbParameter CreateParameter(object value, string name = null, DbType? type = null);

        private void TraceCommand(DbCommand cmd)
        {
            if (!Engine.Development) return;
            Console.WriteLine("[SQL]:" + DateTime.Now.ToString());
            Console.WriteLine(cmd.CommandText);
            foreach (DbParameter par in cmd.Parameters)
            {
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
                Console.WriteLine("SET " + par.ParameterName + "=" + valStr + ";");

            }
            Console.WriteLine("[/SQL]\n");
        }
        #endregion
    }
}
