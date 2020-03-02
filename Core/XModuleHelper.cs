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
            List<XModule> list = new List<XModule>();
            var assembly = Assembly.Load(File.ReadAllBytes(path));
            var classes = assembly.GetTypes();
            foreach (Type t in classes)
            {
                try
                {
                   list.Add((XModule)assembly.CreateInstance(t.FullName));
                }
                catch (Exception error)
                {
                   
                }
            }
            //return classes.Select(item => a.CreateInstance(item.FullName ?? throw new InvalidOperationException()))
            //    .OfType<XModule>().ToList();
            return list;
        }
    }
}