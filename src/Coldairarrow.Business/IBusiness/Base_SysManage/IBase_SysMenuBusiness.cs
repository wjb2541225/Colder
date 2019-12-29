using Coldairarrow.Entity.Base_SysManage;
using Coldairarrow.Util;
using System.Collections.Generic;

namespace Coldairarrow.Business.Base_SysManage
{
    public interface IBase_SysMenuBusiness
    {
        List<Base_SysMenu> GetDataList(Pagination pagination, string condition, string keyword);
        Base_SysMenu GetTheData(string id);
        AjaxResult AddData(Base_SysMenu newData);
        AjaxResult UpdateData(Base_SysMenu theData);
        AjaxResult DeleteData(List<string> ids);
    }
}