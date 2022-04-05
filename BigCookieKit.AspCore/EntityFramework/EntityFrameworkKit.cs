using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Text;

namespace BigCookieKit.EntityFramework
{
    public static class EntityFrameworkKit
    {
        /// <summary>
        /// 根据SQL获取DataTable
        /// </summary>
        public static DataTable ReadDataTable(this DbContext context, string sql, params DbParameter[] parameters)
        {
            return SqlToDataSet(context.Database, sql, parameters).Tables[0];
        }

        /// <summary>
        /// 根据SQL获取DataSet
        /// </summary>
        public static DataSet ReadDataSet(this DbContext context, string sql, params DbParameter[] parameters)
        {
            return SqlToDataSet(context.Database, sql, parameters);
        }

        /// <summary>
        /// 获取自定义sql的结果集
        /// </summary>
        private static DataSet SqlToDataSet(DatabaseFacade facade, string sql, params DbParameter[] parameters)
        {
            var ds = new DataSet();
            var con = facade.GetDbConnection();

            if (con.State != ConnectionState.Open) con.Open();
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.Parameters.AddRange(parameters);
                cmd.Transaction = facade.CurrentTransaction?.GetDbTransaction();

                using (var reader = cmd.ExecuteReader())
                {
                    while (!reader.IsClosed)
                    {
                        var dt = new DataTable();
                        dt.Load(reader);
                        ds.Tables.Add(dt);
                    }
                }
            }
            return ds;
        }
    }
}