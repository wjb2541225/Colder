using System;
using System.Collections.Generic;
using System.Text;

namespace Coldairarrow.Entity
{
    public abstract class BusinessEntityBase : EntityBase
    {
        public const string STR_DELETE_FIELD_NAME = "DeleteMark";
        public const int INT_DELETED = 1;

        /// <summary>
        /// 删除标记
        /// </summary>
        public Byte? DeleteMark { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 创建用户主键
        /// </summary>
        public String CreateUserId { get; set; }

        /// <summary>
        /// 创建用户
        /// </summary>
        public String CreateUserName { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? ModifyDate { get; set; }

        /// <summary>
        /// 修改用户主键
        /// </summary>
        public String ModifyUserId { get; set; }

        /// <summary>
        /// 修改用户
        /// </summary>
        public String ModifyUserName { get; set; }
    }
}
