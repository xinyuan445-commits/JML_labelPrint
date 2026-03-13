using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading;

namespace WinFormsApp1
{
    /// <summary>
    /// SQL Server 数据访问基础类
    /// 负责：连接、增删改查
    /// </summary>
    public static class SqlServerDb
    {
        /// <summary>
        /// SQL Server 连接字符串（程序启动时设置一次）
        /// </summary>
        public static string ConnectionString { get; set; }

        #region 连接

        /// <summary>
        /// 创建并打开 SQL Server 连接（带重试）
        /// </summary>
        private static SqlConnection CreateOpenConnection()
        {
            if (string.IsNullOrWhiteSpace(ConnectionString))
                throw new InvalidOperationException("未设置数据库连接字符串");

            const int maxRetry = 3;

            for (int i = 1; i <= maxRetry; i++)
            {
                try
                {
                    var conn = new SqlConnection(ConnectionString);
                    conn.Open();
                    return conn;
                }
                catch (SqlException)
                {
                    if (i == maxRetry)
                        throw;

                    Thread.Sleep(1000);
                }
            }

            throw new Exception("数据库连接失败");
        }

        #endregion

        #region 查询

        /// <summary>
        /// 执行查询，返回 DataTable
        /// </summary>
        public static DataTable ExecuteDataTable(
            string sql,
            params SqlParameter[] parameters)
        {
            using (var conn = CreateOpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                if (parameters != null && parameters.Length > 0)
                    cmd.Parameters.AddRange(parameters);

                using (var adapter = new SqlDataAdapter(cmd))
                {
                    var table = new DataTable();
                    adapter.Fill(table);
                    return table;
                }
            }
        }

        /// <summary>
        /// 执行查询，返回第一行第一列
        /// </summary>
        public static object ExecuteScalar(
            string sql,
            params SqlParameter[] parameters)
        {
            using (var conn = CreateOpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                if (parameters != null && parameters.Length > 0)
                    cmd.Parameters.AddRange(parameters);

                return cmd.ExecuteScalar();
            }
        }

        #endregion

        #region 增 / 删 / 改

        /// <summary>
        /// 执行 INSERT / UPDATE / DELETE
        /// 返回受影响行数
        /// </summary>
        public static int ExecuteNonQuery(
            string sql,
            params SqlParameter[] parameters)
        {
            using (var conn = CreateOpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                if (parameters != null && parameters.Length > 0)
                    cmd.Parameters.AddRange(parameters);

                return cmd.ExecuteNonQuery();
            }
        }

        #endregion

        #region 事务

        /// <summary>
        /// 执行事务（多条 SQL）
        /// </summary>
        public static void ExecuteTransaction(Action<SqlCommand> action)
        {
            using (var conn = CreateOpenConnection())
            using (var tran = conn.BeginTransaction())
            using (var cmd = conn.CreateCommand())
            {
                cmd.Transaction = tran;

                try
                {
                    action(cmd);
                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
            }
        }

        #endregion

        #region 参数辅助

        /// <summary>
        /// 创建 SQL 参数（简化写法）
        /// </summary>
        public static SqlParameter Param(
            string name,
            object value)
        {
            return new SqlParameter(name, value ?? DBNull.Value);
        }

        #endregion
    }
}
