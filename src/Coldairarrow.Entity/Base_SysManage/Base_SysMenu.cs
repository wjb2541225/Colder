using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coldairarrow.Entity.Base_SysManage
{
    /// <summary>
    /// Base_SysMenu
    /// </summary>
    [Table("Base_SysMenu")]
    public class Base_SysMenu : BusinessEntityBase
    {

        /// <summary>
        /// 菜单主键
        /// </summary>
        [Key, Column(Order = 1)]
        public String Id { get; set; }

        /// <summary>
        /// 父节点主键
        /// </summary>
        public String ParentId { get; set; }

        /// <summary>
        /// 菜单名称
        /// </summary>
        public String MenuName { get; set; }

        /// <summary>
        /// 菜单标记
        /// </summary>
        public String MenuTitle { get; set; }

        /// <summary>
        /// 菜单图标
        /// </summary>
        public String MenuImg { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public Byte? MenuType { get; set; }

        /// <summary>
        /// 导航地址
        /// </summary>
        public String NavigateUrl { get; set; }

        /// <summary>
        /// 目标
        /// </summary>
        public String Target { get; set; }

        /// <summary>
        /// 排序吗
        /// </summary>
        public Byte? SortCode { get; set; }


        /// <summary>
        /// 允许编辑 
        /// </summary>
        public Byte? AllowEdit { get; set; }

        /// <summary>
        /// 允许删除
        /// </summary>
        public Byte? AllowDelete { get; set; }


    }
}