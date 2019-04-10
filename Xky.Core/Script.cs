using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Xky.Core.Model;

namespace Xky.Core
{
    public class Script
    {
        private readonly ScriptEngine _engine;

        public Script(Device device)
        {
            _engine = new ScriptEngine(device);
        }

        /// <summary>
        /// 弹出toast提示框
        /// </summary>
        /// <param name="toast">提示内容</param>
        /// <param name="style">风格</param>
        /// <returns></returns>
        public Response Toast(string toast, int style = 2)
        {
            return _engine.Toast(toast, style);
        }

        public Response AdbCommand(string command)
        {
            return _engine.AdbCommand(command);
        }

        public Response AdbShell(string command)
        {
            return _engine.AdbShell(command);
        }
    }

    internal class ScriptEngine
    {
        private readonly Device _device;

        internal ScriptEngine(Device device)
        {
            _device = device;
        }

        internal Response Toast(string toast, int style = 1)
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "toast", new JArray(toast, style));
        }

        internal Response AdbCommand(string command)
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "adbCommand", new JArray(command.Trim()));
        }

        internal Response AdbShell(string command)
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "adbShell", new JArray(command.Trim()));
        }
    }
}