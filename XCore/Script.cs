using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1;
using XCore.Model;

namespace XCore
{
    /// <summary>
    /// 脚本引擎
    /// </summary>
    public class Script
    {
        private readonly Device _device;

        /// <summary>
        /// 脚本引擎
        /// </summary>
        /// <param name="device"></param>
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
        /// 查找当前设备
        /// </summary>
        /// <returns></returns>
        public Response FindMe()
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "findMe",
                new JArray());
        }

        /// <summary>
        /// 唤醒当前设备
        /// </summary>
        /// <returns></returns>
        public Response WakeUp()
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "wakeup",
                new JArray());
        }

        /// <summary>
        /// 锁屏当前设备
        /// </summary>
        /// <returns></returns>
        public Response LockScreen()
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "lockScreen",
                new JArray());
        }

        /// <summary>
        /// 重启app
        /// </summary>
        /// <param name="package">app包名</param>
        /// <returns></returns>
        public Response RestartApp(string package)
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "restartApp",
                new JArray(package));
        }

        /// <summary>
        /// 获取版本号
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public Response GetAppVersion(string package)
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "getAppVersion",
                new JArray(package));
        }

        /// <summary>
        /// 结束app
        /// </summary>
        /// <param name="package">app包名</param>
        /// <returns></returns>
        public Response KillApp(string package)
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "killApp",
                new JArray(package));
        }

        /// <summary>
        /// 清空app(相当于重装)
        /// </summary>
        /// <param name="package">app包名</param>
        /// <returns></returns>
        public Response ClearApp(string package)
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "clearApp",
                new JArray(package));
        }

        /// <summary>
        /// 从url地址安装apk
        /// </summary>
        /// <param name="url">远程url地址</param>
        /// <returns></returns>
        public Response InstallApkFromUrl(string url)
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "installApkFromUrl",
                new JArray(url));
        }


        /// <summary>
        /// 查找所有UI元素
        /// </summary>
        /// <returns></returns>
        public Response FindAllUiObject()
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "findAllUiObject",
                new JArray());
        }


        /// <summary>
        /// 查找界面元素
        /// </summary>
        /// <param name="name">查找条件</param>
        /// <param name="option">可选条件：{"regex":false,"timeout":10}</param>
        /// <returns></returns>
        public Response FindUiObjects(string name, JObject option=null)
        {
            if(option==null)
                option=new JObject();
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "findUiObjects",
                new JArray(name, option));
        }

        /// <summary>
        /// 查找界面元素并点击它
        /// </summary>
        /// <param name="name">查找条件</param>
        /// <param name="option">可选条件：{"regex":false,"timeout":10,"index":0}</param>
        /// <returns></returns>
        public Response FindAndClick(string name, JObject option=null)
        {
            if (option == null)
                option = new JObject();
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "findAndClick",
                new JArray(name, option));
        }

        /// <summary>
        /// 查找界面元素并给它赋值
        /// </summary>
        /// <param name="name">查找条件</param>
        /// <param name="value">赋值内容</param>
        /// <param name="option">可选条件：{"regex":false,"timeout":10,"index":0}</param>
        /// <returns></returns>
        public Response FindAndInput(string name, string value, JObject option=null)
        {
            if (option == null)
                option = new JObject();
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "findAndInput",
                new JArray(name, value, option));
        }

        /// <summary>
        /// 输入文本
        /// </summary>
        /// <param name="text">文本内容</param>
        /// <returns></returns>
        public Response Input(string text)
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "input",
                new JArray(text));
        }


        /// <summary>
        /// 点击指定位置
        /// </summary>
        /// <param name="x">x坐标(0-1范围）</param>
        /// <param name="y">y坐标(0-1范围）</param>
        /// <returns></returns>
        public Response Click(double x, double y)
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "click",
                new JArray(x, y));
        }

        /// <summary>
        /// 按下指定位置
        /// </summary>
        /// <param name="x">x坐标(0-1范围）</param>
        /// <param name="y">y坐标(0-1范围）</param>
        /// <returns></returns>
        public Response MouseDown(double x, double y)
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "mousedown",
                new JArray(x, y));
        }

        /// <summary>
        /// 释放指定位置
        /// </summary>
        /// <param name="x">x坐标(0-1范围）</param>
        /// <param name="y">y坐标(0-1范围）</param>
        /// <returns></returns>
        public Response MouseUp(double x, double y)
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "mouseup",
                new JArray(x, y));
        }

        /// <summary>
        /// 拖动指定位置
        /// </summary>
        /// <param name="x">x坐标(0-1范围）</param>
        /// <param name="y">y坐标(0-1范围）</param>
        /// <returns></returns>
        public Response MouseDrag(double x, double y)
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "mousedrag",
                new JArray(x, y));
        }

        /// <summary>
        /// 滑动指定位置
        /// </summary>
        /// <param name="startx">滑动起始坐标x(0-1范围）</param>
        /// <param name="starty">滑动起始坐标y(0-1范围）</param>
        /// <param name="endx">滑动终点坐标x(0-1范围）</param>
        /// <param name="endy">滑动终点坐标y(0-1范围）</param>
        /// <param name="steps">滑动步数(默认10)</param>
        /// <returns></returns>
        public Response Swipe(double startx = 0.5, double starty = 0.8, double endx = 0.5, double endy = 0.2,
            double steps = 10)
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "swipe",
                new JArray(startx, starty, endx, endy, steps));
        }

        /// <summary>
        /// 滚动
        /// </summary>
        /// <param name="x">滚动起始坐标x(0-1范围）</param>
        /// <param name="y">滚动起始坐标y(0-1范围）</param>
        /// <param name="updown">上下滚动量</param>
        /// <param name="leftright">左右滚动量</param>
        /// <returns></returns>
        public Response Wheel(double x, double y, int updown = 0, int leftright = 0)
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "wheel",
                new JArray(x, y, updown, leftright));
        }

        /// <summary>
        /// 按下并释放按键
        /// </summary>
        /// <param name="keycode">安卓按键代码</param>
        /// <returns></returns>
        public Response PressKey(int keycode)
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "pressKey",
                new JArray(keycode));
        }

        /// <summary>
        /// 发送编辑器动作
        /// </summary>
        /// <param name="actioncode">动作代码</param>
        /// <returns></returns>
        public Response SendEditorAction(int actioncode)
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "sendEditorAction",
                new JArray(actioncode));
        }

        /// <summary>
        /// 剪切
        /// </summary>
        /// <returns></returns>
        public Response Cut()
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "cut",
                new JArray());
        }

        /// <summary>
        /// 复制
        /// </summary>
        /// <returns></returns>
        public Response Copy()
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "copy",
                new JArray());
        }

        /// <summary>
        /// 粘贴
        /// </summary>
        /// <returns></returns>
        public Response Paste()
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "paste",
                new JArray());
        }

        /// <summary>
        /// 设置剪贴板
        /// </summary>
        /// <param name="value">剪贴板内容</param>
        /// <returns></returns>
        public Response SetClipboardText(string value)
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "setClipboardText",
                new JArray(value));
        }

        /// <summary>
        /// 读取剪贴板
        /// </summary>
        /// <returns></returns>
        public Response GetClipboardText()
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "getClipboardText",
                new JArray());
        }

        /// <summary>
        /// 设置输入法为X输入法
        /// </summary>
        /// <returns></returns>
        public Response SetInputMethod()
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "setInputMethod",
                new JArray());
        }

        /// <summary>
        /// 显示输入法选择框
        /// </summary>
        /// <returns></returns>
        public Response ShowInputMethod()
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "showInputMethod",
                new JArray());
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
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "writeBufferToFile2",
                new JArray(path, data));
        }

        /// <summary>
        /// 从设备读取二进制数据
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Response ReadBufferFromFile(string path)
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "readBufferFromFile",
                new JArray(path));
        }

        /// <summary>
        /// 读取设备上的目录信息
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Response ReadDir(string path)
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "readDir",
                new JArray(path));
        }

        /// <summary>
        /// 创建一个新的硬件全息信息
        /// </summary>
        /// <param name="model">指定型号</param>
        /// <returns></returns>
        public Response CreateHardware(string model = null)
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "createHardware",
                new JArray(model));
        }

        /// <summary>
        /// 还原一个硬件全息信息
        /// </summary>
        /// <param name="key">硬件key</param>
        /// <returns></returns>
        public Response RestoreHardware(string key)
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "restoreHardware",
                new JArray(key));
        }

        /// <summary>
        /// 读取当前设备的硬件全息key
        /// </summary>
        /// <returns></returns>
        public Response GetHardwareKey()
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "getHardwareKey",
                new JArray());
        }

        /// <summary>
        /// 获取当前设备的快照信息
        /// </summary>
        /// <param name="packagename">包名</param>
        /// <returns></returns>
        public Response GetCurrentAppSnapshot(string packagename)
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "getCurrentAppSnapshot",
                new JArray(packagename));
        }

        /// <summary>
        /// 创建全息快照插槽
        /// </summary>
        /// <param name="packageName">包名</param>
        /// <param name="name">快照名</param>
        /// <param name="basepath">基础地址（默认：/data/AppSnapshot/））</param>
        /// <returns></returns>
        public Response CreateAppSnapshot(string packageName, string name, string basepath = null)
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "createAppSnapshot",
                new JArray(packageName, name, basepath));
        }

        /// <summary>
        /// 切换全息快照插槽
        /// </summary>
        /// <param name="packageName">包名</param>
        /// <param name="name">快照名</param>
        /// <param name="basepath">基础地址（默认：/data/AppSnapshot/））</param>
        /// <returns></returns>
        public Response SetAppSnapshot(string packageName, string name, string basepath = null)
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "setAppSnapshot",
                new JArray(packageName, name, basepath));
        }

        /// <summary>
        /// 删除全息快照插槽
        /// </summary>
        /// <param name="packageName">包名</param>
        /// <param name="name">快照名</param>
        /// <param name="basepath">基础地址（默认：/data/AppSnapshot/））</param>
        /// <returns></returns>
        public Response DelAppSnapshot(string packageName, string name, string basepath = null)
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "delAppSnapshot",
                new JArray(packageName, name, basepath));
        }

        /// <summary>
        /// 获取全息快照插槽列表
        /// </summary>
        /// <param name="packageName">包名</param>
        /// <param name="basepath">基础地址（默认：/data/AppSnapshot/））</param>
        /// <returns></returns>
        public Response GetAppSnapshotList(string packageName, string basepath = null)
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "getAppSnapshotList",
                new JArray(packageName, basepath));
        }

        /// <summary>
        /// 设置定位坐标
        /// </summary>
        /// <param name="location">坐标，如：22.517631,114.071045</param>
        /// <returns></returns>
        public Response SetLocation(string location)
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "setLocation",
                new JArray(location));
        }

        /// <summary>
        /// 读取当前定位坐标
        /// </summary>
        /// <returns></returns>
        public Response GetLocation()
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "getLocation",
                new JArray());
        }

        /// <summary>
        /// 通过远程图片url更新摄像头内容
        /// </summary>
        /// <param name="url">图片url地址</param>
        /// <returns></returns>
        public Response UpdateCameraFromUrl(string url)
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "updateCameraFromUrl",
                new JArray(url));
        }

        /// <summary>
        /// 通过二进制图片数据更新摄像头内容
        /// </summary>
        /// <param name="data">图片二进制</param>
        /// <returns></returns>
        public Response UpdateCameraFromFile(byte[] data)
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "updateCameraFromFile",
                new JArray(Convert.ToBase64String(data)));
        }

        /// <summary>
        /// 将文本内容保存为二维码更新摄像头内容
        /// </summary>
        /// <param name="text">二维码文本</param>
        /// <returns></returns>
        public Response UpdateCameraFromText(string text)
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "updateCameraFromText",
                new JArray(text));
        }

        /// <summary>
        /// 更新摄像头
        /// </summary>
        /// <param name="data">图片数据</param>
        /// <returns></returns>
        public Response UpdateCamera(byte[] data)
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "updateCamera",
                new JArray(data));
        }


        /// <summary>
        /// 获取联系人列表
        /// </summary>
        /// <returns></returns>
        public Response GetContacts()
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "getContacts",
                new JArray());
        }

        /// <summary>
        /// 插入联系人列表
        /// </summary>
        /// <param name="contacts"></param>
        /// <returns></returns>
        public Response InsertContacts(JArray contacts)
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "insertContacts",
                contacts);
        }

        /// <summary>
        /// 清空联系人
        /// </summary>
        /// <returns></returns>
        public Response ClearContacts()
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "clearContacts",
                new JArray());
        }

        /// <summary>
        /// 插入媒体库
        /// </summary>
        /// <param name="path"> 媒体文件路径（在设备上的路径，非本地）</param>
        /// <returns></returns>
        public Response InsertMedia(string path)
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "insertMedia",
                new JArray(path));
        }

        /// <summary>
        /// 清空媒体库
        /// </summary>
        /// <param name="path">媒体库路径（默认:/sdcard/DCIM）</param>
        /// <returns></returns>
        public Response ClearDcim(string path = "/sdcard/DCIM")
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "clearDCIM",
                new JArray(path));
        }

        /// <summary>
        /// 调用第三方接口
        /// </summary>
        /// <param name="name">接口名称</param>
        /// <param name="option">参数</param>
        /// <returns></returns>
        public Response CallApi(string name, JObject option)
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "callEvent",
                new JArray(name, option));
        }


        #region 插槽相关

        /// <summary>
        /// 获取插槽信息
        /// </summary>
        /// <param name="packageName"></param>
        /// <returns></returns>
        public Response GetSlot(string packageName)
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "getSlot",
                new JArray(packageName));
        }

        /// <summary>
        /// 创建插槽
        /// </summary>
        /// <param name="packageName"></param>
        /// <param name="name"></param>
        /// <param name="basePath"></param>
        /// <returns></returns>
        public Response CreateSlot(string packageName, string name, string basePath = "/data/AppSlot/")
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "createSlot",
                new JArray(packageName, name, basePath));
        }

        /// <summary>
        /// 设置切换插槽
        /// </summary>
        /// <param name="packageName"></param>
        /// <param name="name"></param>
        /// <param name="basePath"></param>
        /// <returns></returns>
        public Response SetSlot(string packageName, string name, string basePath = "/data/AppSlot/")
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "setSlot",
                new JArray(packageName, name, basePath));
        }

        /// <summary>
        /// 删除插槽
        /// </summary>
        /// <param name="packageName"></param>
        /// <param name="name"></param>
        /// <param name="basePath"></param>
        /// <returns></returns>
        public Response DelSlot(string packageName, string name, string basePath = "/data/AppSlot/")
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "delSlot",
                new JArray(packageName, name, basePath));
        }

        /// <summary>
        /// 获取插槽列表
        /// </summary>
        /// <param name="packageName">包名</param>
        /// <param name="basePath">原始路径</param>
        /// <returns></returns>
        public Response GetSlotList(string packageName, string basePath = "/data/AppSlot/")
        {
            return Client.CallNodeApi(_device.NodeSerial, _device.Sn, "getSlotList",
                new JArray(packageName, basePath));
        }

        #endregion
    }
}