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

        /// <summary>
        /// 在设备外部执行adb指令
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public Response AdbCommand(string command)
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "adbCommand", new JArray(command));
        }

        /// <summary>
        /// 在设备内部执行adb指令
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public Response AdbShell(string command)
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "adbShell", new JArray(command));
        }

        /// <summary>
        /// 写入文本到设备上
        /// </summary>
        /// <param name="path"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public Response WriteStringToFile(string path, string str)
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "writeStringToFile", new JArray(path, str));
        }

        /// <summary>
        /// 把二进制数组写入设备
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public Response WriteBufferToFile(string path, byte[] data)
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "writeBufferToFile",
                new JArray(path, Convert.ToBase64String(data)));
        }

        public Response CreateHardware(string model = null)
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "createHardware",
                new JArray(model));
        }

        public Response RestoreHardware(string key)
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "restoreHardware",
                new JArray(key));
        }

        public Response GetHardwareKey()
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "getHardwareKey",
                new JArray());
        }

        public Response GetCurrentAppSnapshot(string packagename)
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "getCurrentAppSnapshot",
                new JArray(packagename));
        }

        public Response CreateAppSnapshot(string packageName, string name, string basepath = null)
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "createAppSnapshot",
                new JArray(packageName, name, basepath));
        }

        public Response SetAppSnapshot(string packageName, string name, string basepath = null)
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "setAppSnapshot",
                new JArray(packageName, name, basepath));
        }

        public Response DelAppSnapshot(string packageName, string name, string basepath = null)
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "delAppSnapshot",
                new JArray(packageName, name, basepath));
        }

        public Response GetAppSnapshotList(string packageName, string basepath = null)
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "getAppSnapshotList",
                new JArray(packageName, basepath));
        }

        public Response SetLocation(string location)
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "setLocation",
                new JArray(location));
        }

        public Response GetLocation()
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "getLocation",
                new JArray());
        }

        public Response UpdateCameraFromUrl(string url)
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "updateCameraFromUrl",
                new JArray(url));
        }
    }
}