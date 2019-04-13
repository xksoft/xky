using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Newtonsoft.Json.Linq;
using Xky.Core.Model;

namespace Xky.Core
{
    /// <summary>
    /// 用户自定义参数面板
    /// </summary>
    public abstract class XUserControl : UserControl
    {
        /// <summary>
        /// 返回结果
        /// </summary>
        public abstract JObject Result { get; set; }

        
    }
}