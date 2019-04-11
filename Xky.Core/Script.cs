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
        private readonly Device _device;

        public Script(Device device)
        {
            _device = device;
        }

        /// <summary>
        /// 弹出toast提示框
        /// </summary>
        /// <param name="toast">提示内容</param>
        /// <param name="style">风格</param>
        /// <returns></returns>
        public Response Toast(string toast, int style = 2)
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "toast", new JArray(toast, style));
        }

        public Response AdbCommand(string command)
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "adbCommand", new JArray(command.Trim()));
        }

        public Response AdbShell(string command)
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "adbShell", new JArray(command.Trim()));
        }
    }
}