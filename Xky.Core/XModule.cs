using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Xky.Core.Model;

namespace Xky.Core
{
    /// <inheritdoc />
    /// <summary>
    /// 侠客模块基础类
    /// </summary>
    public abstract class XModule : Object, ICloneable
    {
        public XModule()
        {
            ModuleName = Name();
            ModuleDescription = Description();
        }
        public string ModuleName { get; set; }
        public string ModuleDescription { get; set; }
        /// <summary>
        /// 分组名称
        /// </summary>
        public string GroupName { get; set; }
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
        /// 显示用户赋值界面
        /// </summary>
        /// <returns>是否继续</returns>
        public virtual bool ShowUserControl()
        {
            return true;
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
        public abstract void Start();
    }
}