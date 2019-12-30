using Coldairarrow.Entity;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Coldairarrow.DataRepository
{
    /// <summary>
    /// 基数据仓储
    /// </summary>
    public interface IBaseRepository : ITransaction
    {
        #region 增加数据

        /// <summary>
        /// 添加单条记录
        /// </summary>
        /// <typeparam name="T">实体泛型</typeparam>
        /// <param name="entity">实体对象</param>
        void Insert<T>(T entity) where T : EntityBase, new();

        /// <summary>
        /// 添加多条记录
        /// </summary>
        /// <typeparam name="T">实体泛型</typeparam>
        /// <param name="entities">实体对象集合</param>
        void InsertList<T>(IList<T> entities) where T : EntityBase, new();

        #endregion

        #region 删除数据

        /// <summary>
        /// 删除所有记录
        /// </summary>
        /// <typeparam name="T">实体泛型</typeparam>
        void DeleteAll<T>() where T : EntityBase, new();

        /// <summary>
        /// 删除单条记录
        /// </summary>
        /// <typeparam name="T">实体泛型</typeparam>
        /// <param name="entity">实体对象</param>
        void Delete<T>(T entity) where T : EntityBase, new();

        /// <summary>
        /// 删除多条记录
        /// </summary>
        /// <typeparam name="T">实体泛型</typeparam>
        /// <param name="entities">实体对象集合</param>
        void DeleteList<T>(IList<T> entities) where T : EntityBase, new();

        /// <summary>
        /// 按条件删除记录
        /// </summary>
        /// <typeparam name="T">实体泛型</typeparam>
        /// <param name="condition">筛选条件</param>
        void Delete<T>(Expression<Func<T, bool>> condition) where T : EntityBase, new();

        /// <summary>
        /// 删除所有数据
        /// </summary>
        void LogicDeleteAll<T>() where T : BusinessEntityBase, new();

        /// <summary>
        /// 删除指定主键数据
        /// </summary>
        /// <param name="key"></param>
        void LogicDelete<T>(string key) where T : BusinessEntityBase, new();

        /// <summary>
        /// 通过主键删除多条数据
        /// </summary>
        /// <param name="keys"></param>
        void LogicDeleteList<T>(IList<string> keys) where T : BusinessEntityBase, new();

        /// <summary>
        /// 删除单条数据
        /// </summary>
        /// <param name="entity">实体对象</param>
        void LogicDelete<T>(T entity) where T : BusinessEntityBase, new();

        /// <summary>
        /// 删除多条数据
        /// </summary>
        /// <param name="entities">实体对象集合</param>
        void LogicDeleteList<T>(IList<T> entities) where T : BusinessEntityBase, new();

        /// <summary>
        /// 删除指定条件数据
        /// </summary>
        /// <param name="condition">筛选条件</param>
        void LogicDelete<T>(Expression<Func<T, bool>> condition) where T : BusinessEntityBase, new();

        #endregion

        #region 更新数据

        /// <summary>
        /// 更新单条记录
        /// </summary>
        /// <typeparam name="T">实体泛型</typeparam>
        /// <param name="entity">实体对象</param>
        void Update<T>(T entity) where T : EntityBase, new();

        /// <summary>
        /// 更新多条记录
        /// </summary>
        /// <typeparam name="T">实体泛型</typeparam>
        /// <param name="entities">实体对象集合</param>
        void UpdateList<T>(IList<T> entities) where T : EntityBase, new();

        /// <summary>
        /// 更新单条记录的某些属性
        /// </summary>
        /// <typeparam name="T">实体泛型</typeparam>
        /// <param name="entity">实体对象</param>
        /// <param name="properties">属性</param>
        void UpdateAny<T>(T entity, IList<string> properties) where T : EntityBase, new();

        /// <summary>
        /// 更新多条记录的某些属性
        /// </summary>
        /// <typeparam name="T">实体泛型</typeparam>
        /// <param name="entities">实体对象集合</param>
        /// <param name="properties">属性</param>
        void UpdateListAny<T>(IList<T> entities, IList<string> properties) where T : EntityBase, new();

        /// <summary>
        /// 按照条件更新记录
        /// </summary>
        /// <typeparam name="T">实体泛型</typeparam>
        /// <param name="whereExpre">筛选条件</param>
        /// <param name="set">更新操作</param>
        void UpdateWhere<T>(Expression<Func<T, bool>> whereExpre, Action<T> set) where T : EntityBase, new();

        #endregion

        #region 查询数据

        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <typeparam name="T">实体泛型</typeparam>
        /// <returns></returns>
        IList<T> GetList<T>() where T : EntityBase, new();

        #endregion
    }
}
