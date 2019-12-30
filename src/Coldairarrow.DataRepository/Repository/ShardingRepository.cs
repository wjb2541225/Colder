using Coldairarrow.Entity;
using Coldairarrow.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Coldairarrow.DataRepository
{
    public class ShardingRepository : IShardingRepository
    {
        #region 构造函数

        public ShardingRepository(IRepository db)
        {
            _db = db;
        }

        #endregion

        #region 私有成员



        private IRepository _db { get; }
        private Type MapTable(Type absTable, string targetTableName)
        {
            return ShardingHelper.MapTable(absTable, targetTableName);
        }
        private IList<(T targetObj, IRepository targetDb)> GetMapConfigs<T>(IList<T> entities)
        {
            List<(T targetObj, IRepository targetDb)> resList = new List<(T targetObj, IRepository targetDb)>();
            entities.ForEach(aEntity =>
            {
                (string tableName, string conString, DatabaseType dbType) = ShardingConfig.Instance.GetTheWriteTable(typeof(T).Name, aEntity);

                resList.Add((aEntity, DbFactory.GetRepository(conString, dbType)));
            });

            return resList;
        }
        private void WriteTable<T>(IList<T> entities, Action<T, IRepository> accessData)
        {
            var mapConfigs = GetMapConfigs<T>(entities);

            var dbs = mapConfigs.Select(x => x.targetDb).ToArray();
            if (!_openedTransaction)
            {
                DistributedTransaction transaction = new DistributedTransaction(dbs);
                using (transaction.BeginTransaction())
                {
                    Run();
                    var (Success, ex) = transaction.EndTransaction();
                    if (!Success)
                        throw ex;
                }
            }
            else
            {
                _transaction.AddRepository(dbs);
                Run();
            }

            void Run()
            {
                List<Task> tasks = new List<Task>();
                mapConfigs.ForEach(aConfig =>
                {
                    tasks.Add(Task.Run(() =>
                    {
                        accessData(aConfig.targetObj, aConfig.targetDb);
                    }));
                });
                Task.WaitAll(tasks.ToArray());
            }
        }
        private bool _openedTransaction { get; set; } = false;
        private DistributedTransaction _transaction { get; set; }
        private ITransaction _BeginTransaction(IsolationLevel? isolationLevel = null)
        {
            _openedTransaction = true;
            _transaction = new DistributedTransaction();
            if (isolationLevel == null)
                _transaction.BeginTransaction();
            else
                _transaction.BeginTransaction(isolationLevel.Value);

            return _transaction;
        }

        protected static PropertyInfo GetKeyProperty(Type type)
        {
            return GetKeyPropertys(type).FirstOrDefault();
        }
        protected static List<PropertyInfo> GetKeyPropertys(Type type)
        {
            var properties = type
                .GetProperties()
                .Where(x => x.GetCustomAttributes(true).Select(o => o.GetType().FullName).Contains(typeof(KeyAttribute).FullName))
                .ToList();

            return properties;
        }

        #endregion

        #region 外部接口

        /// <summary>
        /// 添加单条记录
        /// </summary>
        /// <typeparam name="T">实体泛型</typeparam>
        /// <param name="entity">实体对象</param>
        public void Insert<T>(T entity) where T : EntityBase, new()
        {
            InsertList<T>(new List<T> { entity });
        }

        /// <summary>
        /// 添加多条记录
        /// </summary>
        /// <typeparam name="T">实体泛型</typeparam>
        /// <param name="entities">实体对象集合</param>
        public void InsertList<T>(IList<T> entities) where T : EntityBase, new()
        {
            WriteTable(entities, (targetObj, targetDb) => targetDb.Insert(targetObj));
        }

        /// <summary>
        /// 删除所有记录
        /// </summary>
        /// <typeparam name="T">实体泛型</typeparam>
        public void DeleteAll<T>() where T : EntityBase, new()
        {
            var configs = ShardingConfig.Instance.GetAllWriteTables(typeof(T).Name);
            var allDbs = configs.Select(x => new
            {
                Db = DbFactory.GetRepository(x.conString, x.dbType),
                TargetType = MapTable(typeof(T), x.tableName)
            }).ToList();

            var dbs = allDbs.Select(x => x.Db).ToArray();

            if (!_openedTransaction)
            {
                using (DistributedTransaction transaction = new DistributedTransaction(dbs))
                {
                    transaction.BeginTransaction();

                    Run();

                    var (Success, ex) = transaction.EndTransaction();

                    if (!Success)
                        throw ex;
                }
            }
            else
            {
                _transaction.AddRepository(dbs);
                Run();
            }

            void Run()
            {
                allDbs.ForEach(x =>
                {
                    x.Db.DeleteAll<T>();
                });
            }
        }

        /// <summary>
        /// 删除单条记录
        /// </summary>
        /// <typeparam name="T">实体泛型</typeparam>
        /// <param name="entity">实体对象</param>
        public void Delete<T>(T entity) where T : EntityBase, new()
        {
            DeleteList<T>(new List<T> { entity });
        }

        /// <summary>
        /// 删除多条记录
        /// </summary>
        /// <typeparam name="T">实体泛型</typeparam>
        /// <param name="entities">实体对象集合</param>
        public void DeleteList<T>(IList<T> entities) where T : EntityBase, new()
        {
            WriteTable(entities, (targetObj, targetDb) => targetDb.Delete(targetObj));
        }

        /// <summary>
        /// 按条件删除记录
        /// </summary>
        /// <typeparam name="T">实体泛型</typeparam>
        /// <param name="condition">筛选条件</param>
        public void Delete<T>(Expression<Func<T, bool>> condition) where T : EntityBase, new()
        {
            var deleteList = GetIShardingQueryable<T>().Where(condition).ToList();

            DeleteList<T>(deleteList);
        }

        /// <summary>
        /// 删除所有数据
        /// </summary>
        public void LogicDeleteAll<T>() where T : BusinessEntityBase, new()
        {
            var configs = ShardingConfig.Instance.GetAllWriteTables(typeof(T).Name);
            var allDbs = configs.Select(x => new
            {
                Db = DbFactory.GetRepository(x.conString, x.dbType),
                TargetType = MapTable(typeof(T), x.tableName)
            }).ToList();

            var dbs = allDbs.Select(x => x.Db).ToArray();

            if (!_openedTransaction)
            {
                using (DistributedTransaction transaction = new DistributedTransaction(dbs))
                {
                    transaction.BeginTransaction();

                    Run();

                    var (Success, ex) = transaction.EndTransaction();

                    if (!Success)
                        throw ex;
                }
            }
            else
            {
                _transaction.AddRepository(dbs);
                Run();
            }

            void Run()
            {
                allDbs.ForEach(x =>
                {
                    x.Db.LogicDeleteAll<T>();
                });
            }
        }

        /// <summary>
        /// 删除指定主键数据
        /// </summary>
        /// <param name="key"></param>
        public void LogicDelete<T>(string key) where T : BusinessEntityBase, new()
        {
            LogicDeleteList<T>(new List<string> { key });
        }

        /// <summary>
        /// 通过主键删除多条数据
        /// </summary>
        /// <param name="keys"></param>
        public void LogicDeleteList<T>(IList<string> keys) where T : BusinessEntityBase, new()
        {
            LogicDeleteList(BuildEntity<T>(keys));
        }

        /// <summary>
        /// 删除单条数据
        /// </summary>
        /// <param name="entity">实体对象</param>
        public void LogicDelete<T>(T entity) where T : BusinessEntityBase, new()
        {
            LogicDeleteList<T>(new List<T> { entity });
        }

        /// <summary>
        /// 删除多条数据
        /// </summary>
        /// <param name="entities">实体对象集合</param>
        public void LogicDeleteList<T>(IList<T> entities) where T : BusinessEntityBase, new()
        {
            WriteTable(entities, (targetObj, targetDb) => { targetDb.LogicDelete(targetObj); });
        }

        /// <summary>
        /// 删除指定条件数据
        /// </summary>
        /// <param name="condition">筛选条件</param>
        public void LogicDelete<T>(Expression<Func<T, bool>> condition) where T : BusinessEntityBase, new()
        {
            var list = GetIShardingQueryable<T>().Where(condition).ToList();
            list.ForEach(aData => aData.DeleteMark = BusinessEntityBase.INT_DELETED);
            UpdateList<T>(list);
        }


        private IList<T> BuildEntity<T>(IList<string> keys)
        {
            var type = typeof(T);
            var theProperty = GetKeyProperty(type);
            if (theProperty == null)
                throw new Exception("该实体没有主键标识！请使用[Key]标识主键！");

            List<T> deleteList = new List<T>();
            keys.ForEach(aKey =>
            {
                T newData = (T)Activator.CreateInstance(type);
                var value = aKey.ChangeType(theProperty.PropertyType);
                theProperty.SetValue(newData, value);
                deleteList.Add(newData);
            });
            return deleteList;
        }

        /// <summary>
        /// 更新单条记录
        /// </summary>
        /// <typeparam name="T">实体泛型</typeparam>
        /// <param name="entity">实体对象</param>
        public void Update<T>(T entity) where T : EntityBase, new()
        {
            UpdateList<T>(new List<T> { entity });
        }

        /// <summary>
        /// 更新多条记录
        /// </summary>
        /// <typeparam name="T">实体泛型</typeparam>
        /// <param name="entities">实体对象集合</param>
        public void UpdateList<T>(IList<T> entities) where T : EntityBase, new()
        {
            WriteTable(entities, (targetObj, targetDb) => targetDb.Update(targetObj));
        }

        /// <summary>
        /// 更新单条记录的某些属性
        /// </summary>
        /// <typeparam name="T">实体泛型</typeparam>
        /// <param name="entity">实体对象</param>
        /// <param name="properties">属性</param>
        public void UpdateAny<T>(T entity, IList<string> properties) where T : EntityBase, new()
        {
            UpdateListAny<T>(new List<T> { entity }, properties);
        }

        /// <summary>
        /// 更新多条记录的某些属性
        /// </summary>
        /// <typeparam name="T">实体泛型</typeparam>
        /// <param name="entities">实体对象集合</param>
        /// <param name="properties">属性</param>
        public void UpdateListAny<T>(IList<T> entities,IList<string> properties) where T : EntityBase, new()
        {
            WriteTable(entities, (targetObj, targetDb) => targetDb.UpdateAny(targetObj, properties));
        }

        /// <summary>
        /// 按照条件更新记录
        /// </summary>
        /// <typeparam name="T">实体泛型</typeparam>
        /// <param name="whereExpre">筛选条件</param>
        /// <param name="set">更新操作</param>
        public void UpdateWhere<T>(Expression<Func<T, bool>> whereExpre, Action<T> set) where T : EntityBase, new()
        {
            var list = GetIShardingQueryable<T>().Where(whereExpre).ToList();
            list.ForEach(aData => set(aData));
            UpdateList<T>(list);
        }

        /// <summary>
        /// 获取IShardingQueryable
        /// </summary>
        /// <typeparam name="T">实体泛型</typeparam>
        /// <returns></returns>
        public IShardingQueryable<T> GetIShardingQueryable<T>() where T : EntityBase, new()
        {
            return new ShardingQueryable<T>(_db.GetIQueryable<T>(), _transaction);
        }

        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <typeparam name="T">实体泛型</typeparam>
        /// <returns></returns>
        public IList<T> GetList<T>() where T : EntityBase, new()
        {
            return GetIShardingQueryable<T>().ToList();
        }

        #endregion

        #region 事物处理

        /// <summary>
        /// 开始事物
        /// </summary>
        /// <returns></returns>
        public ITransaction BeginTransaction()
        {
            _BeginTransaction();

            return this;
        }

        /// <summary>
        /// 开始事物
        /// 注:自定义事物级别
        /// </summary>
        /// <param name="isolationLevel">事物级别</param>
        /// <returns></returns>
        public ITransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            _BeginTransaction(isolationLevel);

            return this;
        }

        /// <summary>
        /// 提交事物
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void CommitTransaction()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 回滚事物
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void RollbackTransaction()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 结束事物
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public (bool Success, Exception ex) EndTransaction()
        {
            _openedTransaction = false;

            return _transaction.EndTransaction();
        }

        public void CommitDb()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Dispose

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue)
                return;

            if (disposing)
            {
                _transaction.Dispose();
            }

            _openedTransaction = false;

            disposedValue = true;
        }

        ~ShardingRepository()
        {
            Dispose(false);
        }

        /// <summary>
        /// 执行与释放或重置非托管资源关联的应用程序定义的任务。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            //GC.SuppressFinalize(this);
        }

        #endregion
    }
}
