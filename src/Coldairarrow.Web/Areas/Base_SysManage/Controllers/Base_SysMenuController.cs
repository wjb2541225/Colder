using Coldairarrow.Business;
using Coldairarrow.Business.Base_SysManage;
using Coldairarrow.Entity.Base_SysManage;
using Coldairarrow.Util;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Coldairarrow.Web.Areas.Base_SysManage.Controllers
{
    [Area("Base_SysManage")]
    public class Base_SysMenuController : BaseMvcController
    {
        #region DI

        public Base_SysMenuController(IBase_SysMenuBusiness base_SysMenuBus, IOperator @operator)
        {
            _base_SysMenuBus = base_SysMenuBus;
            _operator = @operator;
        }
        IBase_SysMenuBusiness _base_SysMenuBus { get; }
        IOperator _operator;

        #endregion

        #region 视图功能

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Form(string id)
        {
            var theData = id.IsNullOrEmpty() ? new Base_SysMenu() : _base_SysMenuBus.GetTheData(id);

            return View(theData);
        }

        #endregion

        #region 获取数据

        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="condition">查询类型</param>
        /// <param name="keyword">关键字</param>
        /// <returns></returns>
        public ActionResult GetDataList(Pagination pagination, string condition, string keyword)
        {
            var dataList = _base_SysMenuBus.GetDataList(pagination, condition, keyword);

            return DataTable_Bootstrap(dataList, pagination);
        }
        public IList<Base_SysMenu> GetDataList()
        {
            return _base_SysMenuBus.GetDataList(new Pagination(), null, null);
        }

        #endregion

        #region 提交数据

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="theData">保存的数据</param>
        public ActionResult SaveData(Base_SysMenu theData)
        {
            AjaxResult res;
            if (theData.Id.IsNullOrEmpty())
            {
                theData.Id = IdHelper.GetId();

                res = _base_SysMenuBus.AddData(theData);
            }
            else
            {
                res = _base_SysMenuBus.UpdateData(theData);
            }

            return JsonContent(res.ToJson());
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="theData">删除的数据</param>
        public ActionResult DeleteData(string ids)
        {
            var res = _base_SysMenuBus.DeleteData(ids.ToList<string>());

            return JsonContent(res.ToJson());
        }

        #endregion
    }
}