using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Xky.Core
{
    /// <summary>
    /// 模块帮助类
    /// </summary>
    public class XModuleHelper
    {
        /// <summary>
        /// 加载dll模块中的模块
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static List<XModule> LoadXModules(string path)
        {
            var a = Assembly.Load(File.ReadAllBytes(path));
            var classes = a.GetTypes();
            return classes.Select(item => a.CreateInstance(item.FullName ?? throw new InvalidOperationException()))
                .OfType<XModule>().ToList();
        }
    }
}