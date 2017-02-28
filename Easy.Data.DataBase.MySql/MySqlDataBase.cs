using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using Easy.MetaData;
using Easy.Extend;
using System.Reflection;

namespace Easy.Data.DataBase.MySql
{
    public class MySqlDataBase : DataBasic
    {
        public MySqlDataBase()
        {
            ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Easy"].ConnectionString;
        }
        private string Replace(string command)
        {
            return command.Replace("[", "`").Replace("]", "`");
        }
        public override IEnumerable<string> DataBaseTypeNames()
        {
            yield return "MySql";
        }

        public override bool IsExistColumn(string tableName, string columnName)
        {
            throw new NotImplementedException();
        }

        public override bool IsExistTable(string tableName)
        {
            throw new NotImplementedException();
        }

        protected override DbCommand GetDbCommand()
        {
            return new MySqlCommand();
        }

        protected override DbCommandBuilder GetDbCommandBuilder(DbDataAdapter adapter)
        {
            return new MySqlCommandBuilder(adapter as MySqlDataAdapter);
        }

        protected override DbConnection GetDbConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        protected override DbDataAdapter GetDbDataAdapter(DbCommand command)
        {
            return new MySqlDataAdapter(command as MySqlCommand);
        }

        protected override DbParameter GetDbParameter(string key, object value)
        {
            return new MySqlParameter(key, value);
        }
        public override DataTable GetData(DbCommand command)
        {
            command.CommandText = Replace(command.CommandText);
            return base.GetData(command);
        }
        protected override int ExecCommand(DbCommand command)
        {
            command.CommandText = Replace(command.CommandText);
            return base.ExecCommand(command);
        }
        public override int ExecuteNonQuery(string command, CommandType ct, List<KeyValuePair<string, object>> parameter)
        {
            return base.ExecuteNonQuery(Replace(command), ct, parameter);
        }
        
        protected override void SetParameter(DbCommand comm, string key, object value)
        {
            comm.CommandText = Replace(comm.CommandText);
            base.SetParameter(comm, key, value);
        }
        public override IEnumerable<T> Get<T>(DataFilter filter, Pagination pagin)
        {
            DataConfigureAttribute custAttribute = DataConfigureAttribute.GetAttribute<T>();
            string alias = "T0";
            if (custAttribute != null)
            {
                filter = custAttribute.MetaData.DataAccess(filter);//数据权限    
                alias = custAttribute.MetaData.Alias;
            }
            string tableName = GetTableName<T>(custAttribute);
            List<KeyValuePair<string, string>> comMatch;
            string selectCol = GetSelectColumn<T>(custAttribute, out comMatch);
            string condition = filter.ToString();

            var primaryKey = GetPrimaryKeys(custAttribute);
            foreach (var item in primaryKey)
            {
                filter.OrderBy(string.Format("[{0}].[{1}]", alias, item.ColumnName), OrderType.Ascending);
            }
            string orderby = filter.GetOrderString();
            string orderByContrary = filter.GetContraryOrderString();

            var builderRela = new StringBuilder();
            if (custAttribute != null)
            {
                foreach (var item in custAttribute.MetaData.DataRelations)
                {
                    builderRela.Append(item);
                }
            }
            const string formatCount = "SELECT COUNT(*) FROM [{0}] {3} {2} {1}";
            DataTable recordCound = this.GetData(string.Format(formatCount,
                tableName,
                string.IsNullOrEmpty(condition) ? "" : "WHERE " + condition,
                builderRela,
                alias), filter.GetParameterValues());
            pagin.RecordCount = Convert.ToInt64(recordCound.Rows[0][0]);
            if (pagin.AllPage == pagin.PageIndex && pagin.RecordCount > 0)
            {
                pagin.PageIndex--;
            }
            int pageSize = pagin.PageSize;
            if ((pagin.PageIndex + 1) * pagin.PageSize > pagin.RecordCount && pagin.RecordCount != 0)
            {
                pageSize = (int)(pagin.RecordCount - pagin.PageIndex * pagin.PageSize);
                if (pageSize < 0)
                {
                    pageSize = pagin.PageSize;
                }
            }
            var builder = new StringBuilder();
            const string formatTable = "SELECT {0} FROM {3} {6} {5} {4} limit {2},{1}";
            builder.AppendFormat(formatTable,
                selectCol,
                pageSize,
                pagin.PageSize * pagin.PageIndex,
                string.Format("[{0}] {1}", tableName, custAttribute == null ? "T0" : custAttribute.MetaData.Alias),
                orderby,
                string.IsNullOrEmpty(condition) ? "" : "WHERE " + condition,
                builderRela,
                orderByContrary.Replace("[{0}].".FormatWith(alias), ""),
                orderby.Replace("[{0}].".FormatWith(alias), ""));


            DataTable table = this.GetData(builder.ToString(), filter.GetParameterValues());
            if (table == null) return new List<T>(); ;
            var list = new List<T>();
            Dictionary<string, PropertyInfo> properties = GetProperties<T>(custAttribute);
            for (int i = 0; i < table.Rows.Count; i++)
            {
                list.Add(Reflection.ClassAction.GetModel<T>(table, i, comMatch, properties));
            }
            return list;
        }

    }
}
