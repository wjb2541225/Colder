using Coldairarrow.Entity.Base_SysManage;
using Coldairarrow.Util;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using static Coldairarrow.Entity.Base_SysManage.EnumType;

namespace Coldairarrow.Business.Base_SysManage
{
    /// <summary>
    /// 权限管理静态类
    /// </summary>
    public class PermissionManage : IPermissionManage, IDependency
    {
        #region DI

        public IBase_UserBusiness _sysUserBus { get => AutofacHelper.GetScopeService<IBase_UserBusiness>(); }
        public IOperator _operator { get => AutofacHelper.GetScopeService<IOperator>(); }
        public IBase_SysRoleBusiness RoleBus { get => AutofacHelper.GetScopeService<IBase_SysRoleBusiness>(); }
        private static IBase_SysMenuBusiness _base_SysMenuBusiness { get => AutofacHelper.GetScopeService<IBase_SysMenuBusiness>(); }

        private IBase_AppSecretBusiness base_AppSecretBusiness;
        private IBase_UserBusiness base_UserBusiness;
        private IBase_SysRoleBusiness base_SysRoleBusiness;
        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        static PermissionManage()
        {
            InitAllPermission();
        }

        #endregion

        #region 内部成员
        private static List<PermissionModule> _allPermissionModules { get; set; }
        private static List<string> _allPermissionValues { get; set; }

        private static IList<Permission> _permissions;

        protected static void InitAllPermission()
        {
            _permissions = _base_SysMenuBusiness.GetDataList(new Pagination(), null, null)
                .Select(p => new Permission { Id = p.Id, ModuleUrl = p.NavigateUrl, Name = p.MenuTitle })
                .ToList();
        }

        public IList<Permission> LoadAllPermission()
        {
            return _permissions?.DeepClone();
        }

        private static string _cacheKey { get; } = "Permission";
        private static string BuildCacheKey(string key)
        {
            return $"{GlobalSwitch.ProjectName}_{_cacheKey}_{key}";
        }

        #endregion


        #region 角色权限


        /// <summary>
        /// 获取角色拥有的所有权限值
        /// </summary>
        /// <param name="roleId">用户Id</param>
        /// <returns></returns>
        public IList<Permission> GetRolePermission(string roleId)
        {

            string cacheKey = BuildCacheKey(roleId);
            var permissions = CacheHelper.Cache.GetCache<IList<Permission>>(cacheKey);
            if (permissions == null)
            {

                permissions = GetRolePermissionFromDB(roleId);
                CacheHelper.Cache.SetCache(cacheKey, permissions);
            }

            return permissions.DeepClone();
        }

        private IList<Permission> GetRolePermissionFromDB(string roleId)
        {
            return GetRolePermissionFromDB(new List<string> { roleId });

        }

        private IList<Permission> GetRolePermissionFromDB(IList<string> roleIds)
        {
            if (roleIds == null || !roleIds.Any())
            {
                return new List<Permission>();
            }

            IList<Base_PermissionRole> rolePermissions = base_SysRoleBusiness.GetPermissions(roleIds);
            return _permissions.Where(p => rolePermissions.Any(a => a.PermissionValue == p.Id)).ToList();

        }

        /// <summary>
        /// 设置用户权限
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="permissions">权限值列表</param>
        public void SetRolePermission(string roleId, IList<Permission> permissions)
        {

            base_SysRoleBusiness.SavePermission(roleId, permissions);
            base_UserBusiness.SaveUserPermission(roleId, permissions);
            //更新缓存
            UpdateRolePermissionCache(roleId);
        }

        /// <summary>
        /// 更新用户权限缓存
        /// </summary>
        /// <param name="userId"><用户Id/param>
        private void UpdateRolePermissionCache(string roleId)
        {
            string cacheKey = BuildCacheKey(roleId);
            var permissions = GetRolePermissionFromDB(roleId);
            CacheHelper.Cache.SetCache(cacheKey, permissions);
        }

        #endregion

        #region AppId权限

        /// <summary>
        /// 获取AppId权限值
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public IList<Permission> GetAppIdPermissions(string appId)
        {
            string cacheKey = BuildCacheKey(appId);
            var permissions = CacheHelper.Cache.GetCache<List<Permission>>(cacheKey);
            if (permissions == null)
            {

                IList<Base_PermissionAppId> appPermissions = base_AppSecretBusiness.GetPermissions(appId);
                IList<Permission> permissionsOfApp = _permissions.Where(p => appPermissions.Any(a => a.PermissionValue == p.Id)).ToList();


                CacheHelper.Cache.SetCache(cacheKey, permissionsOfApp);
            }

            return permissions.DeepClone();
        }

        /// <summary>
        /// 设置AppId权限
        /// </summary>
        /// <param name="appId">AppId</param>
        /// <param name="permissions">权限值列表</param>
        public void SetAppIdPermission(string appId, List<Permission> permissions)
        {
            //更新缓存
            string cacheKey = BuildCacheKey(appId);
            CacheHelper.Cache.SetCache(cacheKey, permissions);



            base_AppSecretBusiness.SavePermission(appId, permissions);
        }

        #endregion

        #region 用户权限

        private IList<Permission> GetPermissionsFromDB(string userId)
        {
            IList<Base_PermissionUser> userPermissions = base_UserBusiness.GetPermissions(userId);
            IEnumerable<Permission> permissionsOfApp = _permissions.Where(p => userPermissions.Any(a => a.PermissionValue == p.Id));

            IList<string> userRolesIds = base_UserBusiness.GetUserRoleIds(userId);

            IList<Permission> rolePermissions = GetRolePermissionFromDB(userRolesIds);
            permissionsOfApp = permissionsOfApp.Union(rolePermissions);

            return permissionsOfApp.ToList();
        }

        /// <summary>
        /// 设置用户权限
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="permissions">权限值列表</param>
        public void SetUserPermission(string userId, List<Permission> permissions)
        {


            base_UserBusiness.SaveUserPermission(userId, permissions);
            //更新缓存
            UpdateUserPermissionCache(userId);
        }

        /// <summary>
        /// 清除所有用户权限缓存
        /// </summary>
        public void ClearUserPermissionCache()
        {
            BaseBusiness<Base_UnitTest> _db = new BaseBusiness<Base_UnitTest>();
            var userIdList = base_UserBusiness.GetDataList(new Pagination(), null, null).Select(p => p.Id).ToList();
            userIdList.ForEach(aUserId =>
            {
                CacheHelper.Cache.RemoveCache(BuildCacheKey(aUserId));
            });
        }

        /// <summary>
        /// 更新用户权限缓存
        /// </summary>
        /// <param name="userId"><用户Id/param>
        public void UpdateUserPermissionCache(string userId)
        {
            string cacheKey = BuildCacheKey(userId);
            var permissions = GetPermissionsFromDB(userId);
            CacheHelper.Cache.SetCache(cacheKey, permissions);
        }

        #endregion

        #region 当前操作用户权限

        /// <summary>
        /// 获取当前操作者拥有的所有权限值
        /// </summary>
        /// <returns></returns>
        public IList<Permission> GetUserPermissionOfOperator(string userId, string moduleUrl)
        {
            return GetUserPermission(userId).Where(p => p.ModuleUrl == moduleUrl).ToList();
        }

        /// <summary>
        /// 获取用户权限模块
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IList<Permission> GetUserPermissionOfModule(string userId)
        {
            return GetUserPermission(userId).Where(p => string.IsNullOrEmpty(p.ModuleUrl)).ToList();
        }

        /// <summary>
        /// 获取用户拥有的所有权限值
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        public IList<Permission> GetUserPermission(string userId)
        {

            string cacheKey = BuildCacheKey(userId);
            var permissions = CacheHelper.Cache.GetCache<IList<Permission>>(cacheKey);
            if (permissions == null)
            {

                permissions = GetPermissionsFromDB(userId);
                CacheHelper.Cache.SetCache(cacheKey, permissions);
            }

            return permissions.DeepClone();
        }

        #endregion
    }
}