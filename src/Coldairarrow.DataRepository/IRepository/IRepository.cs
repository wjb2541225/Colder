using Coldairarrow.Entity;
using Coldairarrow.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;

namespace Coldairarrow.DataRepository
{
    public interface IRepository : IBaseRepository, ITransaction, IDisposable
    {
        #region 数据库相关

        /// <summary>
        /// SQL日志处理方法
        /// </summary>
        /// <value>
        /// The handle SQL log.
        /// </value>
        Action<string> HandleSqlLog {set; }

        /// <summary>
        /// 提交到数据库
        /// </summary>
        void CommitDb();

        /// <summary>
        /// 连接字符串
        /// </summary>
        string ConnectionString { get; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        DatabaseType DbType { get; }

        /// <summary>
        /// 使用已存在的事物
        /// </summary>
        /// <param name="transaction">事物对象</param>
        void UseTransaction(DbTransaction transaction);

        /// <summary>
        /// 获取事物对象
        /// </summary>
        /// <returns></returns>
        DbTransaction GetTransaction();

        #endregion

        #region 增加数据

        /// <summary>
        /// 使用Bulk批量导入,速度快
        /// </summary>
        /// <typeparam name="T">实体泛型</typeparam>
        /// <param name="entities">实体集合</param>
        void BulkInsert<T>(IList<T> entities) where T : EntityBase, new();

        #endregion

        #region 删除数据


        /// <summary>
        /// 删除单条记录
        /// </summary>
        /// <typeparam name="T">实体泛型</typeparam>
        /// <param name="key">主键</param>
        void Delete<T>(string key) where T : EntityBase, new();

        /// <summary>
        /// 删除多条记录
        /// </summary>
        /// <typeparam name="T">实体泛型</typeparam>
        /// <param name="keys">多条记录主键集合</param>
        void Delete<T>(IList<string> keys) where T : EntityBase, new();

        /// <summary>
        /// 使用SQL语句按照条件删除数据
        /// 用法:Delete_Sql"Base_User"(x=>x.Id == "Admin")
        /// 注：生成的SQL类似于DELETE FROM [Base_User] WHERE [Name] = 'xxx' WHERE [Id] = 'Admin'
        /// </summary>
        /// <typeparam name="T">实体泛型</typeparam>
        /// <param name="where">条件</param>
        /// <returns>影响条数</returns>
        int Delete_Sql<T>(Expression<Func<T, bool>> where) where T : EntityBase, new();

        #endregion


        #region 更新数据

        /// <summary>
        /// 使用SQL语句按照条件更新
        /// 用法:UpdateWhere_Sql"Base_User"(x=>x.Id == "Admin",("Name","小明"))
        /// 注：生成的SQL类似于UPDATE [TABLE] SET [Name] = 'xxx' WHERE [Id] = 'Admin'
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="where">筛选条件</param>
        /// <param name="values">字段值设置</param>
        /// <returns>影响条数</returns>
        int UpdateWhere_Sql<T>(Expression<Func<T, bool>> where, params (string field, object value)[] values) where T : EntityBase, new();

        #endregion

        #region 查询数据

        /// <summary>
        /// 获取单条记录
        /// </summary>
        /// <typeparam name="T">实体泛型</typeparam>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        T GetEntity<T>(params object[] keyValue) where T : EntityBase, new();


        /// <summary>
        /// 获取IQueryable
        /// 注:默认取消实体追踪
        /// </summary>
        /// <typeparam name="T">实体泛型</typeparam>
        /// <returns></returns>
        IQueryable<T> GetIQueryable<T>() where T : EntityBase, new();

        /// <summary>
        /// 获取IQueryable
        /// 注:默认取消实体追踪
        /// </summary>
        /// <param name="type">实体泛型</param>
        /// <returns></returns>
        IQueryable GetIQueryable(Type type);

        /// <summary>
        /// 通过SQL获取DataTable
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns></returns>
        DataTable GetDataTableWithSql(string sql);

        /// <summary>
        /// 通过SQL获取DataTable
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">SQL参数</param>
        /// <returns></returns>
        DataTable GetDataTableWithSql(string sql, IList<DbParameter> parameters);

        /// <summary>
        /// 通过SQL获取List
        /// </summary>
        /// <typeparam name="T">实体泛型</typeparam>
        /// <param name="sqlStr">SQL语句</param>
        /// <returns></returns>
        List<T> GetListBySql<T>(string sqlStr) where T : class, new();

        /// <summary>
        /// 通过SQL获取List
        /// </summary>
        /// <typeparam name="T">实体泛型</typeparam>
        /// <param name="sqlStr">SQL语句</param>
        /// <param name="parameters">SQL参数</param>
        /// <returns></returns>
        List<T> GetListBySql<T>(string sqlStr, IList<DbParameter> parameters) where T : class, new();

        #endregion

        #region 执行Sql语句

        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="sql">SQL语句</param>
        int ExecuteSql(string sql);

        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">SQL参数</param>
        int ExecuteSql(string sql, IList<DbParameter> parameters);


        #endregion
    }
}
