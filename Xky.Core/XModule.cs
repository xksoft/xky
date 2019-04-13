using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xky.Core.Model;

namespace Xky.Core
{
    /// <inheritdoc />
    /// <summary>
    /// 侠客模块基础类
    /// </summary>
    public abstract class XModule : Object, ICloneable
    {
        /// <summary>
        /// 当前设备
        /// </summary>
        public Device Device { get; set; }

        /// <summary>
        /// 克隆这个对象
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return MemberwiseClone();
        }

        /// <summary>
        /// 用户参数面板
        /// </summary>
        public virtual XUserControl UserControl()
        {
            return null;
        }

        /// <summary>
        /// 模块名称
        /// </summary>
        /// <returns></returns>
        public abstract string Name();

        /// <summary>
        /// 模块说明
        /// </summary>
        /// <returns></returns>
        public abstract string Description();

        /// <summary>
        /// 模块动作脚本
        /// </summary>
        /// <returns></returns>
        public abstract void Action();
    }
}