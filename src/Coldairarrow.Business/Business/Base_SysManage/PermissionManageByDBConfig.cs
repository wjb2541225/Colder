using Coldairarrow.Business.Base_SysManage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coldairarrow.Business.Business.Base_SysManage
{
    public class PermissionManageByDBConfig : IPermissionManage
    {
        public void ClearUserPermissionCache()
        {
            throw new NotImplementedException();
        }

        public List<string> GetAllPermissionValues()
        {
            throw new NotImplementedException();
        }

        public List<PermissionModule> GetAppIdPermissionModules(string appId)
        {
            throw new NotImplementedException();
        }

        public List<string> GetAppIdPermissionValues(string appId)
        {
            throw new NotImplementedException();
        }

        public List<string> GetOperatorPermissionValues()
        {
            throw new NotImplementedException();
        }

        public List<PermissionModule> GetRolePermissionModules(string roleId)
        {
            throw new NotImplementedException();
        }

        public List<PermissionModule> GetUserPermissionModules(string userId)
        {
            throw new NotImplementedException();
        }

        public List<string> GetUserPermissionValues(string userId)
        {
            throw new NotImplementedException();
        }

        public bool OperatorHasPermissionValue(string value)
        {
            throw new NotImplementedException();
        }

        public void SetAppIdPermission(string appId, List<string> permissions)
        {
            throw new NotImplementedException();
        }

        public void SetUserPermission(string userId, List<string> permissions)
        {
            throw new NotImplementedException();
        }

        public void UpdateUserPermissionCache(string userId)
        {
            throw new NotImplementedException();
        }
    }
}
