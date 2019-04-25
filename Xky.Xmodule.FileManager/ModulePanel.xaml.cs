using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xky.Core;
using Xky.Core.Model;

namespace Xky.XModule.FileManager
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
        public string currentDirectory = "/";
        public ObservableCollection<DeviceFile> DeviceFiles = new ObservableCollection<DeviceFile>();
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ItemListBox.ItemsSource = DeviceFiles;
            Ls("/");


        }

        private void btn_cancel_Click(object sender, RoutedEventArgs e)
        {

            Client.CloseDialogPanel();

        }
        public class DeviceFile
        {
            private string _name = "";
            private string _type = "文件";
            private string _fullname = "";
            public string Name { get => _name; set => _name = value; }
            public string Type { get => _type; set => _type = value; }
            public string Fullname { get => _fullname; set => _fullname = value; }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string name = ((Button)sender).Content.ToString();
            var deviceFile = DeviceFiles.ToList().Find(f => f.Name == name);
            if (deviceFile.Type == "file") { MessageBox.Show("下载文件到本地"); }
            else
            {
                Ls(deviceFile.Fullname);

            }
        }
        public void Ls(string dir)
        {

            Console.WriteLine("打开目录：" + dir);
            Response res = device.ScriptEngine.AdbShell("cd " + dir + "&&ls -al");
            if (res.Json["result"] != null)
            {
                DeviceFiles.Clear();

                List<string> files = res.Json["result"].ToString().Replace(" -> ", "->").Split('\n').ToList();
                foreach (string file in files)
                {
                    string[] infos = file.Split(' ');
                    List<string> infolist = new List<string>();
                    foreach (string i in infos)
                    {
                        if (i.Trim().Length > 0)
                        {
                            infolist.Add(i.Trim());
                        }
                    }
                    DeviceFile deviceFile = new DeviceFile();
                    //string s = "";
                    //Console.WriteLine(infolist.Count);
                    //foreach (string ii in infolist)
                    //{
                    //    s += ii + "|";
                    //}
                    //Console.WriteLine(s);
                    if (infolist.Count >= 8)
                    {
                        deviceFile.Name = infolist[7];
                        if (file.StartsWith("-"))
                        {
                            deviceFile.Type = "file";

                        }
                        else if (file.StartsWith("d"))
                        {
                            deviceFile.Type = "directory";

                        }
                        else if (file.StartsWith("l"))
                        {
                            deviceFile.Type = "link";

                        }
                        deviceFile.Fullname = dir + "/" + deviceFile.Name;
                        if (deviceFile.Name == ".")
                        {
                            deviceFile.Fullname = "/";
                        }
                        if (deviceFile.Name.Contains("->"))
                        {
                            deviceFile.Fullname = deviceFile.Name.Substring(deviceFile.Name.IndexOf("->") + 2);
                        }
                        Console.WriteLine(deviceFile.Fullname);
                        DeviceFiles.Add(deviceFile);
                    }
                }
            }

        }
    }
}
