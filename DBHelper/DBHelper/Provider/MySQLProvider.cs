﻿using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBUtil
{
    /// <summary>
    /// MySQL 数据库实现
    /// </summary>
    public class MySQLProvider : IProvider
    {
        #region 创建 DbConnection
        public DbConnection CreateConnection(string connectionString)
        {
            return new MySqlConnection(connectionString);
        }
        #endregion

        #region 生成 DbCommand
        public DbCommand GetCommand()
        {
            return new MySqlCommand();
        }
        #endregion

        #region 生成 DbCommand
        public DbCommand GetCommand(string sql, DbConnection conn)
        {
            DbCommand command = new MySqlCommand(sql);
            command.Connection = conn;
            return command;
        }
        #endregion

        #region 生成 DbParameter
        public DbParameter GetDbParameter(string name, object vallue)
        {
            return new MySqlParameter(name, vallue);
        }
        #endregion

        #region 生成 DbDataAdapter
        public DbDataAdapter GetDataAdapter(DbCommand cmd)
        {
            DbDataAdapter dataAdapter = new MySqlDataAdapter();
            dataAdapter.SelectCommand = cmd;
            return dataAdapter;
        }
        #endregion

        #region GetParameterMark
        public string GetParameterMark()
        {
            return "@";
        }
        #endregion

        #region 创建获取最大编号SQL
        public string CreateGetMaxIdSql(string key, Type type)
        {
            return string.Format("SELECT Max({0}) FROM {1}", key, type.Name);
        }
        #endregion

        #region 创建分页SQL
        public string CreatePageSql(string sql, string orderby, int pageSize, int currentPage, int totalRows)
        {
            StringBuilder sb = new StringBuilder();
            int startRow = 0;
            int endRow = 0;

            #region 分页查询语句
            startRow = pageSize * (currentPage - 1);

            sb.Append("select * from (");
            sb.Append(sql);
            if (!string.IsNullOrWhiteSpace(orderby))
            {
                sb.Append(" ");
                sb.Append(orderby);
            }
            sb.AppendFormat(" ) row_limit limit {0},{1}", startRow, pageSize);
            #endregion

            return sb.ToString();
        }
        #endregion

        #region ForContains
        public SqlValue ForContains(string value)
        {
            return new SqlValue("concat('%',{0},'%')", value);
        }
        #endregion

        #region ForStartsWith
        public SqlValue ForStartsWith(string value)
        {
            return new SqlValue("concat({0},'%')", value);
        }
        #endregion

        #region ForEndsWith
        public SqlValue ForEndsWith(string value)
        {
            return new SqlValue("concat('%',{0})", value);
        }
        #endregion

        #region ForDateTime
        public SqlValue ForDateTime(DateTime dateTime)
        {
            return new SqlValue("STR_TO_DATE({0}, '%Y-%m-%d %H:%i:%s')", dateTime.ToString("yyyy-MM-dd HH:mm:ss"));
        }
        #endregion

        #region ForList
        public SqlValue ForList(IList list)
        {
            List<string> argList = new List<string>();
            for (int i = 0; i < list.Count; i++)
            {
                argList.Add("@id" + i);
            }
            string args = string.Join(",", argList);

            return new SqlValue("(" + args + ")", list);
        }
        #endregion

    }
}
