﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using XCore;
using XCore.Model;

namespace Xky.XModule.AppManager
{
    /// <summary>
    /// ModulePanel.xaml 的交互逻辑
    /// </summary>
    public partial class ModulePanel : System.Windows.Controls.UserControl
    {
        public ModulePanel()
        {
            InitializeComponent();
        }
        public List<XCore.XModule> xmodules;
        public string CurrentDirectory = "/";
        public ObservableCollection<DeviceApp> DeviceApps = new ObservableCollection<DeviceApp>();
        public Dictionary<string, string> PackageNames = new Dictionary<string, string>();
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ItemListBox.ItemsSource = DeviceApps;

            new Thread(() =>
            {
                Assembly myAssembly = Assembly.GetExecutingAssembly();
                Stream myStream = myAssembly.GetManifestResourceStream("Xky.XModule.AppManager.packages.txt");
                byte[] bytes = new byte[myStream.Length];
                myStream.Read(bytes, 0, bytes.Length);
                string[] infos = Encoding.UTF8.GetString(bytes).Split('\n');
                foreach (string info in infos)
                {
                    string inf = info.Trim();
                    if (inf.Length > 0 && inf.Contains("|"))
                    {
                        string[] infs = inf.Split('|');
                        string appname = infs[0];
                        string packagename = infs[1];
                        if (!PackageNames.ContainsKey(packagename))
                        {
                            PackageNames.Add(packagename, appname);
                        }
                    }


                }
                ShowLoading("正在加载设备应用列表...");
                LoadPackages();
                CloseLoading();
            })
            { IsBackground = true }.Start();
        }

        public class DeviceApp
        {
            private string _name = "";
            private string _type = "";
            private string _packageName = "";
            public string Name { get => _name; set => _name = value; }
            public string Type { get => _type; set => _type = value; }
            public string PackageName { get => _packageName; set => _packageName = value; }
        }


        public void LoadPackages()
        {


            List<DeviceApp> list = new List<DeviceApp>();
            Response res_system = xmodules[0].Device.ScriptEngine.AdbShell("pm list package -s");
            if (res_system.Json["result"] != null)
            {
                List<string> res = res_system.Json["result"].ToString().Split('\n').ToList();
                foreach (string s in res)
                {
                    if (s.StartsWith("package:"))
                    {
                        DeviceApp deviceApp = new DeviceApp();
                        deviceApp.Type = "system";
                        int index = s.IndexOf("package:") + 8;
                        deviceApp.PackageName = s.Substring(index, s.Length - index).Trim();
                        if (PackageNames.ContainsKey(deviceApp.PackageName))
                        {
                            deviceApp.Name = PackageNames[deviceApp.PackageName];
                        }
                        else
                        {
                            deviceApp.Name = deviceApp.PackageName;
                        }
                        list.Add(deviceApp);
                    }
                }

            }
            res_system = xmodules[0].Device.ScriptEngine.AdbShell("pm list package -3");
            if (res_system.Json["result"] != null)
            {
                List<string> res = res_system.Json["result"].ToString().Split('\n').ToList();
                foreach (string s in res)
                {
                    if (s.StartsWith("package:"))
                    {
                        DeviceApp deviceApp = new DeviceApp();
                        deviceApp.Type = "user";
                        int index = s.IndexOf("package:") + 8;
                        deviceApp.PackageName = s.Substring(index, s.Length - index).Trim();
                        if (PackageNames.ContainsKey(deviceApp.PackageName))
                        {
                            deviceApp.Name = PackageNames[deviceApp.PackageName];
                        }
                        else
                        {
                            deviceApp.Name = deviceApp.PackageName;
                        }
                        list.Add(deviceApp);
                    }
                }

            }
            list.Sort((left, right) =>
            {
                if (left.Type != right.Type)
                {
                    if (left.Type == "system") { return 1; }
                    else { return -1; }
                }
                else { return 0; }
            });
            DeviceApps = new ObservableCollection<DeviceApp>(list);
            this.Dispatcher.Invoke(new Action(() =>
            {
                ItemListBox.ItemsSource = DeviceApps;
            }));


        }
        public void ReLoadPackages()
        {
            new Thread(() =>
            {
                ShowLoading("正在加载设备应用列表...");
                LoadPackages();
                CloseLoading();
            })
            { IsBackground = true }.Start();
        }
        private void Grid_Drop(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                Array arr = (System.Array)e.Data.GetData(System.Windows.DataFormats.FileDrop);
                foreach (var obj in arr)
                {
                    string filename = obj.ToString();
                    for (int i = 0; i < xmodules.Count; i++)
                    {
                        if (xmodules.Count > i)
                        {
                            xmodules[i].Device.ScriptEngine.WriteBufferToFile(CurrentDirectory + "/" + new FileInfo(filename).Name, File.ReadAllBytes(filename));
                            Console.WriteLine("文件上传完毕");
                        }
                    }

                }

            }


        }


        private void ContextMenuItem_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.MenuItem menuItem = (System.Windows.Controls.MenuItem)sender;
            DeviceApp deviceApp=null;

            if (ItemListBox.SelectedItem!=null) {deviceApp= (DeviceApp)ItemListBox.SelectedItem; } 
            switch (menuItem.Tag)
            {
                case "Open":
                    {
                        if (deviceApp==null) { break; }
                        new Thread(() =>
                        {

                            ShowLoading("正在打开app...");
                            for (int index = 0; index < xmodules.Count; index++)
                            {
                                if (xmodules.Count > index)
                                {
                                    var device = xmodules[index].Device;
                                    ShowLoading("正在打开设备[" + device.Name + "]上的APP["+deviceApp.Name+"]...");
                                    Response res = device.ScriptEngine.RestartApp(deviceApp.PackageName);
                                  
                                }
                            }
                           
                            CloseLoading();
                        })
                        { IsBackground = true }.Start();
                        break;
                        
                    }
                case "Stop":
                    {
                        if (deviceApp == null) { break; }
                        new Thread(() =>
                        {

                            ShowLoading("正在结束app...");
                            for (int index = 0; index < xmodules.Count; index++)
                            {
                                if (xmodules.Count > index)
                                {
                                    var device = xmodules[index].Device;
                                    ShowLoading("正在结束设备[" + device.Name + "]上的APP[" + deviceApp.Name + "]...");
                                    Response res = device.ScriptEngine.KillApp(deviceApp.PackageName);

                                }
                            }

                            CloseLoading();
                        })
                        { IsBackground = true }.Start();
                        break;
                       

                    }
                case "Install":
                    {
                        Button_InstallAPK_Click(null,null);
                        break;

                    }
                case "Clear":
                    {
                        if (deviceApp == null) { break; }
                        new Thread(() =>
                        {

                            ShowLoading("正在清空app数据...");
                            for (int index = 0; index < xmodules.Count; index++)
                            {
                                if (xmodules.Count > index)
                                {
                                    var device = xmodules[index].Device;
                                    ShowLoading("正在清空设备[" + device.Name + "]上的APP[" + deviceApp.Name + "]数据...");
                                    Response res = device.ScriptEngine.ClearApp(deviceApp.PackageName);

                                }
                            }

                            CloseLoading();
                        })
                        { IsBackground = true }.Start();
                        break;

                    }
                case "Uninstall":
                    {
                        if (deviceApp == null) { break; }
                        new Thread(() =>
                        {

                            ShowLoading("正在卸载app...");
                            for (int index = 0; index < xmodules.Count; index++)
                            {
                                if (xmodules.Count > index)
                                {
                                    var device = xmodules[index].Device;
                                    ShowLoading("正在卸载设备[" + device.Name + "]上的APP[" + deviceApp.Name + "]...");
                                    Response res = device.ScriptEngine.AdbShell("pm uninstall "+deviceApp.PackageName);

                                }
                            }

                            CloseLoading();
                        })
                        { IsBackground = true }.Start();
                        break;

                    }

            }
         

        }
        private void MenuItem_Stop_Click(object sender, RoutedEventArgs e)
        {

        }
   

        private void ItemListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ItemListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }
        public void ShowLoading(string text)
        {
            Client.Log(text, "模块[应用管理器]");
            if (Grid_MessageBox.Visibility == Visibility.Visible)
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    Label_Loading.Content = text;
                }));
            }
            else
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    ContentControl_MessageBox.Content = ContentControl_Loading.Content;
                    Label_Loading.Content = text;
                    Grid_MessageBox.Visibility = Visibility.Visible;
                }));
            }
        }
        public void CloseLoading()
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                Grid_MessageBox.Visibility = Visibility.Hidden;
            }));
        }
        public void UploadFile(string dir, string filename)
        {
            ShowLoading("正在上传文件[" + filename + "]...");
            FileInfo fi = new FileInfo(filename);
            string filename_new = fi.Name.Remove(fi.Name.LastIndexOf(fi.Extension));
            if (filename_new.Length > 50)
            {
                filename_new.Remove(50);
            }//大于10M的文件采用分段上传
            for (int index = 0; index < xmodules.Count; index++)
            {
                if (xmodules.Count > index)
                {
                    var device = xmodules[index].Device;
                    ShowLoading("正在上传文件[" + filename + "]到设备["+device.Name+"]...");
                    if (fi.Length > 10485760)
                    {

                        FileStream fs = new FileStream(filename, FileMode.Open);

                        string tempdir = filename_new + "_temp";
                        int i = 1;
                        byte[] bytes = new byte[10485760];
                        string catf = "";
                        int readlength = fs.Read(bytes, 0, 10485760);
                        while (readlength > 0)
                        {
                            ShowLoading("设备["+device.Name+"]使用大文件上传模式，预计还需" + ((fi.Length / 10485760) - i + 1) + "秒钟...");
                            if (readlength < 10485760)
                            {
                                device.ScriptEngine.WriteBufferToFile(dir + "/" + tempdir + "/" + i + ".tmp", bytes.Take(readlength).ToArray());

                            }
                            else { device.ScriptEngine.WriteBufferToFile(dir + "/" + tempdir + "/" + i + ".tmp", bytes); }
                            Console.WriteLine("正在上传第" + i + "个拆分文件，大小" + readlength + "...");
                            catf += i + ".tmp  ";

                            i++;
                            readlength = fs.Read(bytes, 0, 10485760);

                        }
                        fs.Close();
                        for (int w = i / 2; w > 0; w--)
                        {
                            ShowLoading("设备["+device.Name+"]正在合并文件，预计还需" + w + "秒钟...");
                            Thread.Sleep(1000);
                        }

                        Response res = device.ScriptEngine.AdbShell("cd " + dir + "/" + tempdir + "&&cat " + catf + " > ../" + filename_new + fi.Extension + "&&rm -r -f " + dir + "/" + tempdir);
                    
                }
                else
                {
                    Response res = device.ScriptEngine.WriteBufferToFile(dir + "/" + filename_new + fi.Extension, File.ReadAllBytes(filename));
                }
            }
        }
    

        }
        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            Client.CloseDialogPanel();
        }

        private void Button_InstallAPK_Click(object sender, RoutedEventArgs e)
        {

            var openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "安卓安装包|*.apk";
            var result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                new Thread(() =>
                {

                    ShowLoading("正在上传安装包到设备中...");
                   
                    string filename = openFileDialog.FileName.ToString();
                    UploadFile("/sdcard", filename);
                    Thread.Sleep(5000);
                   
                    for (int index = 0; index < xmodules.Count; index++)
                    {
                        if (xmodules.Count > index)
                        {
                            var device = xmodules[index].Device;
                            ShowLoading("正在安装应用到设备["+device.Name+"]...");
                            Response res = device.ScriptEngine.AdbShell("pm install /sdcard/" + new FileInfo(filename).Name + "&");
                            Console.WriteLine("设备["+device.Name+"]安装结果："+res.Json.ToString());
                        }
                    }
                    Thread.Sleep(5000);
                    ShowLoading("成功发送安装命令，请稍后刷新应用列表查看是否成功安装！");
                    Thread.Sleep(5000);
                    CloseLoading();
                    ShowLoading("正在加载设备应用列表...");
                    LoadPackages();
                    CloseLoading();
                })
                { IsBackground = true }.Start();
            }
        }

        private void Button_Reload_Click(object sender, RoutedEventArgs e)
        {
            ReLoadPackages();
        }
    }
}
