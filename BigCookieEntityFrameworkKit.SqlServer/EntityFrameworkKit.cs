using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Text;

namespace BigCookieEntityFrameworkKit.SqlServer
{
    public static class EntityFrameworkKit
    {
        /// <summary>
        /// 根据SQL获取DataTable
        /// <code/>Author: zhangbingbin
        /// <code/>CreateData: 2020-11-13
        /// </summary>
        public static DataTable ReadDataTable(this DbContext context, string sql, params DbParameter[] parameters)
        {
            return SqlToDataSet(context.Database, sql, parameters).Tables[0];
        }

        /// <summary>
        /// 根据SQL获取DataSet
        /// <code/>Author: zhangbingbin
        /// <code/>CreateData: 2020-11-14
        /// </summary>
        public static DataSet ReadDataSet(this DbContext context, string sql, params DbParameter[] parameters)
        {
            return SqlToDataSet(context.Database, sql, parameters);
        }

        /// <summary>
        /// 获取自定义sql的结果集
        /// <code/>Author: zhangbingbin
        /// <code/>CreateData: 2020-11-5
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