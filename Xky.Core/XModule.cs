using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xky.Core
{
    /// <summary>
    /// 侠客模块基础类
    /// </summary>
    public abstract class XModule : Object, ICloneable
    {
        /// <summary>
        /// 克隆这个对象
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return MemberwiseClone();
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
    }
}
