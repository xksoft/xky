using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using Xky.Core.Model;

namespace Xky.Core
{
    /// <inheritdoc />
    /// <summary>
    /// 侠客模块基础类
    /// </summary>
    public abstract class XModule : Object, ICloneable
    {

        public List<Device> Devices = new List<Device>();
        private List<Core.XModule> XModules = new List<Core.XModule>();
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
        public virtual bool Init()
        {
            foreach (var device in Devices)
            {
                var xmodule = (XModule)this.Clone();
                xmodule.Device = device;
                XModules.Add(xmodule);
            }
            return true;
        }

        public virtual bool ShowUserControl()
        {
            
            return true;
        }
      
        public List<Core.XModule> GetXModules()
        {
            return XModules;
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
        /// 模块图标
        /// </summary>
        /// <returns></returns>
        public abstract byte[] Icon();

        /// <summary>
        /// 是否是后台模块
        /// </summary>
        /// <returns></returns>
        public abstract bool IsBackground ();

        /// <summary>
        /// 模块动作脚本
        /// </summary>
        /// <returns></returns>
        public abstract void Start();
    }
}