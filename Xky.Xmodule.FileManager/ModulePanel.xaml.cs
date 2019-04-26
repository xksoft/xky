using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
        public string CurrentDirectory = "/";
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
            private string _fullName = "";
            public string Name { get => _name; set => _name = value; }
            public string Type { get => _type; set => _type = value; }
            public string FullName { get => _fullName; set => _fullName = value; }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           
        }
        public void Ls(string dir)
        {
            CurrentDirectory = dir;
            TextBox_Current.Text = CurrentDirectory;
            Console.WriteLine("打开目录：" + dir);
            Response res = device.ScriptEngine.AdbShell("cd " + dir + "&&ls -al");
            if (res.Json["result"] != null)
            {
                DeviceFiles.Clear();
                List<DeviceFile> list = new List<DeviceFile>();
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
                        deviceFile.FullName = dir + "/" + deviceFile.Name;
                        if (deviceFile.Name == ".")
                        {
                            continue;
                        }
                        else if (deviceFile.Name == "..")
                        {
                            continue;
                        }
                        if (deviceFile.Name.Contains("->"))
                        {
                            deviceFile.FullName = deviceFile.Name.Substring(deviceFile.Name.IndexOf("->") + 2);
                        }

                        list.Add(deviceFile);
                    }
                }
                list.Sort((left, right) =>
                {
                    if (left.Type != right.Type)
                    {
                        if (left.Type == "file") { return 1; }
                        else { return -1; }
                    }
                    else { return 0; }
                });
             
                list.Insert(0, new DeviceFile { Name = "上级目录", FullName =((dir=="/"||dir=="")?"/":dir.Remove(dir.LastIndexOf("/"))), Type = "directory" });
                list.Insert(0,new DeviceFile { Name="根目录",FullName="/",Type="directory"});
                
                DeviceFiles = new ObservableCollection<DeviceFile>(list);
                
                ItemListBox.ItemsSource = DeviceFiles;

            }

        }

        private void Grid_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                Array arr = (System.Array)e.Data.GetData(DataFormats.FileDrop);
                foreach (var obj in arr)
                {
                    string filename = obj.ToString();
                    device.ScriptEngine.WriteBufferToFile(CurrentDirectory+"/"+new FileInfo(filename).Name, File.ReadAllBytes(filename));
                    Console.WriteLine("文件上传完毕");
                }
                Ls(CurrentDirectory);
            }

            
        }

        private void MenuItem_Delete_Click(object sender, RoutedEventArgs e)
        {
            DeviceFile deviceFile = (DeviceFile)((MenuItem)sender).DataContext;
            Console.WriteLine(deviceFile.FullName);
            Response res = device.ScriptEngine.AdbShell("rm -r -f "+deviceFile.FullName);
            Ls(CurrentDirectory);
        }

        private void ItemListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ItemListBox.SelectedItem!=null)
            {
               

                var deviceFile = (DeviceFile)(ItemListBox.SelectedItem);
               
                if (deviceFile.Type == "file") { MessageBox.Show("下载文件到本地"); }
                else
                {

                    Ls(deviceFile.FullName);

                }
            }

        }

        private void ItemListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ItemListBox.SelectedItem != null)
            {


                var deviceFile = (DeviceFile)(ItemListBox.SelectedItem);
                TextBox_Current.Text = deviceFile.FullName;

            }
        }
    }
}
