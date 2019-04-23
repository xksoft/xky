using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xky.Core.Model
{
   public class Module:Object,ICloneable
    {
        private string _md5 = "";
        private string _name = "";
        private string _description = "";
        private string _groupname = "";
        private XModule _xmodule;
        private int _state = 0;

        public string Md5 { get => _md5; set => _md5 = value; }
        public string Name { get => _name; set => _name = value; }
        public string Description { get => _description; set => _description = value; }
        public string GroupName { get => _groupname; set => _groupname = value; }
        public XModule XModule { get => _xmodule; set => _xmodule = value; }
        public int State { get => _state; set => _state = value; }
        /// <summary>
        /// 克隆这个对象
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
