﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Xky.XModule.WangZhe
{
    public class WangZhe : XCore.XModule
    {
        ModulePanel panel = null;
        public override string Name()
        {
            return "王者中单";
        }
        public override bool IsBackground()
        {
            return false;
        }
        public override byte[] Icon()
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            Stream myStream = myAssembly.GetManifestResourceStream("Xky.XModule.WangZhe.logo.png");
            if (myStream != null)
            {
                byte[] bytes = new byte[myStream.Length];
                myStream.Read(bytes, 0, bytes.Length);
                return bytes;
            }
            else
            {
                Console.WriteLine("模块图标路径有误，无法加载！");
                return null;
            }

        }
        public override string Description()
        {
            return "王者中单，不屈白银";
        }

        public override void Start()
        {
            panel.Exit=true;
        }
        public override bool ShowUserControl()
        {
            panel = new ModulePanel();

            panel.device = Devices[0];
            XCore.Client.ShowDialogPanel(panel);
            return true;
        }
    }
}
