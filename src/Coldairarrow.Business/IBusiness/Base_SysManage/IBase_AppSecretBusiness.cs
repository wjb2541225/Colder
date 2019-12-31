using Coldairarrow.Entity.Base_SysManage;
using Coldairarrow.Util;
using System.Collections.Generic;

namespace Coldairarrow.Business.Base_SysManage
{
    public interface IBase_AppSecretBusiness
    {
        IList<Base_AppSecret> GetDataList(Pagination pagination, string keyword);
        IList<Base_PermissionAppId> GetPermissions(string appId);
        Base_AppSecret GetTheData(string id);
        string GetAppSecret(string appId);
        AjaxResult AddData(Base_AppSecret newData);
        AjaxResult UpdateData(Base_AppSecret theData);
        AjaxResult DeleteData(IList<string> ids);
        AjaxResult SavePermission(string appId, IList<Permission> permissions);
    }
}