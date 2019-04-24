using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Xky.XModule.FileManager
{
    class FileManager : Core.XModule
    {
       
        public override string Description()
        {
            return "管理手机上的所有文件";
        }

        public override byte[] Icon()
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            Stream myStream = myAssembly.GetManifestResourceStream("Xky.XModule.FileManager.icon.png");
            byte[] bytes = new byte[myStream.Length];
            myStream.Read(bytes, 0, bytes.Length);
            return bytes;
        }

        public override bool IsBackground()
        {
            return false;
        }

        public override string Name()
        {
            return "文件管理器";
        }

        public override void Start()
        {
            throw new NotImplementedException();
        }
    }
}
