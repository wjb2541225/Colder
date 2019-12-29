using Coldairarrow.Entity.Base_SysManage;
using Coldairarrow.Util;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Coldairarrow.Business.Base_SysManage
{
    public class Base_SysMenuBusiness : BaseBusiness<Base_SysMenu>, IBase_SysMenuBusiness, IDependency
    {
        #region 外部接口

        public List<Base_SysMenu> GetDataList(Pagination pagination, string condition, string keyword)
        {
            var q = GetIQueryable();
            //筛选
            if (!condition.IsNullOrEmpty() && !keyword.IsNullOrEmpty())
                q = q.Where($@"{condition}.Contains(@0)", keyword);

            return q.GetPagination(pagination).ToList();
        }

        public Base_SysMenu GetTheData(string id)
        {
            return GetEntity(id);
        }

        public AjaxResult AddData(Base_SysMenu newData)
        {
            Insert(newData);

            return Success();
        }

        public AjaxResult UpdateData(Base_SysMenu theData)
        {
            Update(theData);

            return Success();
        }

        public AjaxResult DeleteData(List<string> ids)
        {
            Delete(ids);

            return Success();
        }

        #endregion

        #region 私有成员

        #endregion

        #region 数据模型

        #endregion
    }
}