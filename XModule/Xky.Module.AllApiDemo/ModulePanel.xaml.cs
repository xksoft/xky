using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xky.Core;
using Xky.Core.Model;
using Xky.Core.UserControl;

namespace Xky.XModule.AllApiDemo
{
    /// <summary>
    /// ModulePanel.xaml 的交互逻辑
    /// </summary>
    public partial class ModulePanel : UserControl
    {
        public ModulePanel()
        {
            InitializeComponent();
        }
        public Device device;
        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            Client.CloseDialogPanel();
        }

        private void MyButton_Click(object sender, RoutedEventArgs e)
        {
            string tag = ((MyButton)sender).Tag.ToString();
            Response res = null;
            switch (tag)
            {

                case "AdbShell":
                    {
                        res = device.ScriptEngine.AdbShell("ls sdcard");
                        break;
                    }
                case "Toast":
                    {
                        res = device.ScriptEngine.Toast("提示内容");
                        break;
                    }
                case "Log":
                    {
                        Client.Log("测试普通日志内容", "设备[" + device.Name + "]", 0);
                        Client.Log("测试成功日志内容", "设备[" + device.Name + "]", 1);
                        Client.Log("测试警告日志内容", "设备[" + device.Name + "]", 2);
                        Client.Log("测试错误日志内容", "设备[" + device.Name + "]", 3);
                        break;
                    }
                case "FindMe":
                    {
                        res = device.ScriptEngine.FindMe();
                        break;
                    }
                case "WakeUp":
                    {
                        res = device.ScriptEngine.WakeUp();
                        break;
                    }
                case "LockScreen":
                    {
                        res = device.ScriptEngine.LockScreen();
                        break;
                    }
                case "InstallApkFromUrl":
                    {
                        res = device.ScriptEngine.InstallApkFromUrl("http://gdown.baidu.com/data/wisegame/a3e97fe7b40416b0/mojitianqi_7090202.apk");
                        break;
                    }
                case "RestartApp":
                    {
                        res = device.ScriptEngine.RestartApp("com.android.settings");
                        break;
                    }
                case "KillApp":
                    {
                        res = device.ScriptEngine.KillApp("com.android.settings");
                        break;
                    }
                case "ClearApp":
                    {
                        res = device.ScriptEngine.ClearApp("com.android.settings");
                        break;
                    }
                case "FindAllUiObject":
                    {
                        res = device.ScriptEngine.FindAllUiObject();
                        string activityName = res.Json["activity"].ToString();
                        Console.WriteLine("activity名称：" + activityName);
                        var nodes = res.Json["nodes"].ToList();
                        foreach (var node in nodes)
                        {
                            Console.WriteLine(node["className"].ToString());
                            var subnodes = node["nodes"];
                            if (subnodes != null)
                            {
                                Console.WriteLine("子元素：" + subnodes.Count());
                            }
                        }
                        break;
                    }
                case "FindUiObjects":
                    {
                        res = device.ScriptEngine.FindUiObjects("电话", new Newtonsoft.Json.Linq.JObject() { ["regex"] = true, ["timeout"] = 1000 });
                        Console.WriteLine(res.Json["uiObjects"]);
                        if (res.Json["uiObjects"].Count() > 0)
                        {
                            Console.WriteLine("成功找到元素");
                            foreach (var obj in res.Json["uiObjects"])
                            {
                                Console.WriteLine("元素文本：" + obj["text"].ToString());
                            }


                        }
                        break;
                    }
                case "FindAndClick":
                    {
                        res = device.ScriptEngine.FindAndClick("电话", new Newtonsoft.Json.Linq.JObject());
                        res = device.ScriptEngine.FindAndClick("电话", new Newtonsoft.Json.Linq.JObject() { ["regex"] = false, ["timeout"] = 1000, ["index"] = 0 });
                        break;
                    }
                case "FindAndInput":
                    {
                        res = device.ScriptEngine.FindAndInput("com.android.messaging:id/recipient_text_view", "侠客云SDK", new Newtonsoft.Json.Linq.JObject());
                        res = device.ScriptEngine.FindAndInput("com.android.messaging:id/recipient_text_view", "侠客云SDK", new Newtonsoft.Json.Linq.JObject() { ["regex"] = false, ["timeout"] = 1000, ["index"] = 0 });
                        break;
                    }
                case "Input":
                    {
                        res = device.ScriptEngine.Input("侠客云SDK");
                        break;
                    }
                case "Click":
                    {
                        res = device.ScriptEngine.Click(0.5, 0.5);
                        break;
                    }
                case "MouseDown":
                    {
                        res = device.ScriptEngine.MouseDown(0.5, 0.5);
                        break;
                    }
                case "MouseUp":
                    {
                        //长按屏幕中心2秒钟
                        res = device.ScriptEngine.MouseDown(0.5, 0.5);
                        Thread.Sleep(2000);
                        res = device.ScriptEngine.MouseUp(0.5, 0.5);
                        break;
                    }
                case "MouseDrag":
                    {
                        //先按下
                        res = device.ScriptEngine.MouseDown(0.5, 0.5);
                        //再拖动到指定位置
                        res = device.ScriptEngine.MouseDrag(0.6, 0.6);
                        //在指定位置释放
                        res = device.ScriptEngine.MouseUp(0.5, 0.5);
                        break;
                    }
                case "Swipe":
                    {
                        //向右滑动屏幕
                        res = device.ScriptEngine.Swipe(0.5, 0.5, 0.8, 0.5, 10);
                        break;
                    }
                case "Wheel":
                    {
                        //向下滚动
                        res = device.ScriptEngine.Wheel(0.5, 0.5, -10, 0);
                        break;
                    }
                case "PressKey":
                    {
                        //按下home键
                        res = device.ScriptEngine.PressKey(3);
                        break;
                    }
                case "SendEditorAction":
                    {
                        //输入法的发送按键
                        res = device.ScriptEngine.SendEditorAction(4);
                        break;
                    }
                case "Copy":
                    {
                        res = device.ScriptEngine.Copy();
                        break;
                    }
                case "Paste":
                    {
                        res = device.ScriptEngine.Paste();
                        break;
                    }
                case "Cut":
                    {
                        res = device.ScriptEngine.Cut();
                        break;
                    }
                case "SetClipboardText":
                    {
                        res = device.ScriptEngine.SetClipboardText("www.xky.com");
                        break;
                    }
                case "GetClipboardText":
                    {
                        res = device.ScriptEngine.GetClipboardText();
                        if (res.Json["value"] != null)
                        {
                            Console.WriteLine("设备剪贴板内容：" + res.Json["value"].ToString());
                        }
                        else {

                            Console.WriteLine("无法读取设备剪贴板：" + res.Message);
                        }

                        break;
                    }
                case "SetInputMethod":
                    {
                        res = device.ScriptEngine.SetInputMethod();
                        break;
                    }
                case "ShowInputMethod":
                    {
                        res = device.ScriptEngine.ShowInputMethod();
                        break;
                    }
                case "WriteStringToFile":
                    {
                        res = device.ScriptEngine.WriteStringToFile("/sdcard/test.txt", "自定义文本内容");
                        break;
                    }
                case "WriteBufferToFile":
                    {
                        //二进制可以是任何图片、视频、文本以及其他内容
                        res = device.ScriptEngine.WriteBufferToFile("/sdcard/test.txt", Encoding.UTF8.GetBytes("自定义文本内容"));
                        break;
                    }
                case "ReadBufferFromFile":
                    {
                        res = device.ScriptEngine.ReadBufferFromFile("/sdcard/test.txt");
                        if (res.Json["buffer"] != null)
                        {
                            JArray buffer = res.Json["buffer"] as JArray;
                            //如果文件过大可能会分段读取
                            for (int i = 0; i < buffer.Count; i++)
                            {
                                byte[] bs = (byte[])buffer[i];
                                Console.WriteLine(Encoding.UTF8.GetString(bs));
                            }
                        }
                        else
                        {
                            Console.WriteLine("文件读取失败：" + res.Message);

                        }

                        break;
                    }
                case "ReadDir":
                    {
                        res = device.ScriptEngine.ReadDir("/sdcard");
                        if (res.Json["list"] != null)
                        {
                            foreach (var fileordir in res.Json["list"])
                            {
                                Console.WriteLine("名称：" + fileordir["name"] + " 是否是文件：" + fileordir["isFIle"] + " 是否是目录：" + fileordir["isDirectory"]);
                            }
                        }
                        else
                        {
                            Console.WriteLine("目录读取失败：" + res.Message);
                        }
                        break;
                    }
                case "CreateHardware":
                    {
                        res = device.ScriptEngine.CreateHardware();
                        break;
                    }
                case "RestoreHardware":
                    {
                        res = device.ScriptEngine.RestoreHardware("00000");
                        break;
                    }
                case "GetHardwareKey":
                    {
                        res = device.ScriptEngine.GetHardwareKey();

                        break;
                    }
                case "GetCurrentAppSnapshot":
                    {
                        res = device.ScriptEngine.GetCurrentAppSnapshot("第三方应用包名");
                        break;
                    }
                case "CreateAppSnapshot":
                    {
                        res = device.ScriptEngine.CreateAppSnapshot("第三方应用包名", "快照名称1");
                        break;
                    }
                case "SetAppSnapshot":
                    {
                        res = device.ScriptEngine.SetAppSnapshot("第三方应用包名", "快照名称");
                        break;
                    }
                case "DelAppSnapshot":
                    {
                        res = device.ScriptEngine.DelAppSnapshot("第三方应用包名", "快照名称");
                        break;
                    }
                case "GetAppSnapshotList":
                    {
                        res = device.ScriptEngine.GetAppSnapshotList("第三方应用包名");
                        break;
                    }
                case "SetLocation":
                    {
                        res = device.ScriptEngine.SetLocation("22.517631,114.071045");
                        break;
                    }
                case "GetLocation":
                    {
                        res = device.ScriptEngine.GetLocation();
                        break;
                    }
                case "UpdateCameraFromUrl":
                    {
                        res = device.ScriptEngine.UpdateCameraFromUrl("https://www.xky.com/static/main/img/prepare/bg_wechat.jpg");
                        break;
                    }
                case "UpdateCameraFromFile":
                    {
                        res = device.ScriptEngine.UpdateCameraFromFile(File.ReadAllBytes(@"C:\Users\xiaoyi\Downloads\bg_wechat.jpg"));
                        break;
                    }
                case "UpdateCameraFromText":
                    {
                        res = device.ScriptEngine.UpdateCameraFromText("www.xky.com");
                        break;
                    }
                case "GetContacts":
                    {
                        res = device.ScriptEngine.GetContacts();
                        if (res.Json["contacts"] != null)
                        {
                            foreach (var contact in res.Json["contacts"])
                            {
                                Console.WriteLine("姓名：" + contact["name"] + " 手机号：" + contact["number"]);
                            }
                        }
                        else
                        {
                            Console.WriteLine("无法获取联系人列表");

                        }
                        break;
                    }
                case "InsertContacts":
                    {
                        JObject job1 = new JObject { { "name", "小明" }, { "number", "10000000000" } };
                        JObject job2 = new JObject { { "小红", "小明" }, { "number", "10000000001" } };
                        JArray jArray = new JArray();
                        jArray.Add(job1);
                        jArray.Add(job2);
                        JObject jobContacts = new JObject {["contacts"]=jArray };
                        res = device.ScriptEngine.InsertContacts(jArray);
                        break;
                    }
                case "ClearContacts":
                    {
                        res = device.ScriptEngine.ClearContacts();
                        break;
                    }
                case "InsertMedia":
                    {
                        res = device.ScriptEngine.InsertMedia("/sdcard/Pictures/test.jpg");
                        break;
                    }
                case "ClearDcim":
                    {
                        res = device.ScriptEngine.ClearDcim();
                        break;
                    }

            }
            if (res!=null)
            {
                TextBox_Result.Text = res.Json.ToString();
            }
        }
    }
}
