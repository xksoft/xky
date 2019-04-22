using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xky.Core.Model
{
   public class Module
    {
        private string _guid = "";
        private string _name = "";
        private string _description = "";
        private string _groupname = "";
        private XModule _moduleContent;

        public string Guid { get => _guid; set => _guid = value; }
        public string Name { get => _name; set => _name = value; }
        public string Description { get => _description; set => _description = value; }
        public string Groupname { get => _groupname; set => _groupname = value; }
        public XModule ModuleContent { get => _moduleContent; set => _moduleContent = value; }
    }
}
