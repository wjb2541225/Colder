using Coldairarrow.Util;
using System.Collections.Generic;

namespace Coldairarrow.Business.Base_SysManage
{
    /// <summary>
    /// 权限管理接口
    /// </summary>
    public interface IPermissionManage
    {
        #region 所有权限
        IList<Permission> LoadAllPermission();


        #endregion

        #region 角色权限

        /// <summary>
        /// 获取角色权限模块
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        IList<Permission> GetRolePermission(string roleId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="permissions"></param>
        void SetRolePermission(string roleId, IList<Permission> permissions);

        #endregion

        #region AppId权限

        /// <summary>
        /// 获取AppId权限模块
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        IList<Permission> GetAppIdPermissions(string appId);

        /// <summary>
        /// 设置AppId权限
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="permissions"></param>
        void SetAppIdPermission(string appId, List<Permission> permissions);

        #endregion

        #region 用户权限

        /// <summary>
        /// 获取用户权限模块
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        void SetUserPermission(string userId, List<Permission> permissions);


        /// <summary>
        /// 清除所有用户权限缓存
        /// </summary>
        void ClearUserPermissionCache();

        /// <summary>
        /// 更新用户权限缓存
        /// </summary>
        /// <param name="userId"><用户Id/param>
        void UpdateUserPermissionCache(string userId);

        #endregion

        #region 当前操作用户权限

        /// <summary>
        /// 获取当前操作者在当前模块下的权限
        /// </summary>
        /// <returns></returns>
        IList<Permission> GetUserPermissionOfOperator(string userId, string moduleUrl);

        /// <summary>
        /// 获取当前用户的模块权限 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        IList<Permission> GetUserPermissionOfModule(string userId);

        /// <summary>
        /// 获取当前用户的所有权限
        /// </summary>
        /// <param name="value">权限值</param>
        /// <returns></returns>
        IList<Permission> GetUserPermission(string userId);

        IList<>

        #endregion
    }

    #region 数据模型

    public class Permission
    {
        public string Id { get; set; }

        public string ModuleUrl { get; set; }

        public string Name { get; set; }
    }

    public class PermissionModule
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public List<PermissionItem> Items { get; set; }
    }

    public class PermissionItem
    {
        public string Id { get; set; } = IdHelper.GetId();
        public string Name { get; set; }
        public string Value { get; set; }
        public bool IsChecked { get; set; }
    }

    #endregion
}