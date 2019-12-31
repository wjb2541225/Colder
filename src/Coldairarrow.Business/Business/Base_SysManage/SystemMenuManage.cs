using Coldairarrow.Entity.Base_SysManage;
using Coldairarrow.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Coldairarrow.Business.Base_SysManage
{
    /// <summary>
    /// 系统菜单管理
    /// </summary>
    public class SystemMenuManage : ISystemMenuManage, IDependency
    {
        IBase_SysMenuBusiness _base_SysMenuBusiness;
        #region 构造函数

        public SystemMenuManage(IOperator @operator, IPermissionManage permissionManage, IBase_SysMenuBusiness base_SysMenuBusiness)
        {
            _operator = @operator;
            _permissionManage = permissionManage;
            _base_SysMenuBusiness = base_SysMenuBusiness;
            Init();
        }
        IOperator _operator { get; }
        IPermissionManage _permissionManage { get; }

        #endregion

        #region 私有成员

        private static List<Menu> _allMenu { get; set; }

        private void Init()
        {
            var menuDefines = _base_SysMenuBusiness.GetDataList(new Pagination(), null, null);
            var grouped = menuDefines.Where(p => p.MenuType == 1).GroupBy(p => p.ParentId).OrderBy(p => p.Key);


            List<Menu> menus = new List<Menu>();
            foreach (var menuDefineGroup in grouped)
            {
                foreach (var menuDefine in menuDefineGroup)
                {
                    Menu developMenu = new Menu();
                    developMenu.text = menuDefine.MenuTitle;
                    developMenu.icon = menuDefine.MenuImg;
                    developMenu.url = GetUrl(menuDefine.NavigateUrl);
                    developMenu.PermissionId = menuDefine.Id;
                    if (grouped.Any(p => p.Key == menuDefine.Id))
                    {
                        developMenu.children = new List<Menu>();
                    }

                    var parent = menus.FirstOrDefault(p => p.id == menuDefine.ParentId);
                    if (parent == null)
                    {
                        menus.Add(developMenu);
                    }
                    else
                    {
                        parent.children.Add(developMenu);
                    }
                }
            }

            if (GlobalSwitch.RunModel == RunModel.LocalTest)
            {
                Menu developMenu = new Menu
                {
                    text = "开发",
                    icon = "glyphicon glyphicon-console",
                    children = new List<Menu>()
                };
                menus.Add(developMenu);
                developMenu.children.Add(new Menu
                {
                    text = "代码生成",
                    icon = "fa fa-circle-o",
                    url = GetUrl("~/Base_SysManage/RapidDevelopment/Index")
                });
                developMenu.children.Add(new Menu
                {
                    text = "数据库连接管理",
                    icon = "fa fa-circle-o",
                    url = GetUrl("~/Base_SysManage/Base_DatabaseLink/Index")
                });
                developMenu.children.Add(new Menu
                {
                    text = "UMEditor Demo",
                    icon = "fa fa-circle-o",
                    url = GetUrl("~/Demo/UMEditor")
                });
                developMenu.children.Add(new Menu
                {
                    text = "下拉搜索",
                    icon = "fa fa-circle-o",
                    url = GetUrl("~/Demo/SelectSearch")
                });
                developMenu.children.Add(new Menu
                {
                    text = "上传文件",
                    icon = "fa fa-circle-o",
                    url = GetUrl("~/Demo/UploadFile")
                });
                developMenu.children.Add(new Menu
                {
                    text = "下载文件",
                    icon = "fa fa-circle-o",
                    url = GetUrl("~/Demo/DownloadFile")
                });
                developMenu.children.Add(new Menu
                {
                    text = "表格树及下拉树",
                    icon = "fa fa-circle-o",
                    url = GetUrl("~/Base_SysManage/Base_Department/Index")
                });
                developMenu.children.Add(new Menu
                {
                    text = "API签名Demo",
                    icon = "fa fa-circle-o",
                    url = GetUrl("~/Demo/ApiSignDemo")
                });
                developMenu.children.Add(new Menu
                {
                    text = "Tab页",
                    icon = "fa fa-circle-o",
                    url = GetUrl("~/Demo/Tab")
                });
                developMenu.children.Add(new Menu
                {
                    text = "详情页",
                    icon = "fa fa-circle-o",
                    url = GetUrl("~/Demo/Details")
                });
                developMenu.children.Add(new Menu
                {
                    text = "菜单管理",
                    icon = "fa fa-circle-o",
                    url = GetUrl("~/Base_SysManage/Base_SysMenu/Index")
                });
            }


            _allMenu = menus;
        }

        public static string GetUrl(string virtualUrl) => PathHelper.GetUrl(virtualUrl);

        #endregion

        #region 外部接口

        /// <summary>
        /// 获取系统所有菜单
        /// </summary>
        /// <returns></returns>
        public List<Menu> GetAllSysMenu()
        {
            return _allMenu.DeepClone();
        }

        /// <summary>
        /// 获取用户菜单
        /// </summary>
        /// <returns></returns>
        public List<Menu> GetOperatorMenu()
        {
            List<Menu> resList = GetAllSysMenu();

            if (_operator.IsAdmin())
                return resList;

            var userPermissions = _permissionManage.GetUserPermissionModules(_operator.UserId);
            RemoveNoPermission(resList, userPermissions);

            return resList;

            void RemoveNoPermission(List<Menu> menus, IList<Permission> userPermissionValues)
            {
                for (int i = menus.Count - 1; i >= 0; i--)
                {
                    var theMenu = menus[i];
                    if (!theMenu.PermissionId.IsNullOrEmpty() && !userPermissions.Any(p => p.Id == theMenu.PermissionId))
                        menus.RemoveAt(i);
                    else if (theMenu.children?.Count > 0)
                    {
                        RemoveNoPermission(theMenu.children, userPermissions);
                        if (theMenu.children.Count == 0 && theMenu.url.IsNullOrEmpty())
                            menus.RemoveAt(i);
                    }
                }
            }
        }

        #endregion
    }
}