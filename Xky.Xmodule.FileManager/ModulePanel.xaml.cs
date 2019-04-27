using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using Xky.Core;
using Xky.Core.Model;

namespace Xky.XModule.FileManager
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
        public Device device;
        public string CurrentDirectory = "/storage/emulated/0";
        public static List<FileInformation> FileList = new List<FileInformation>();
        public static string  DirectoryAbsolutePath="";
        public ObservableCollection<DeviceFile> DeviceFiles = new ObservableCollection<DeviceFile>();
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ItemListBox.ItemsSource = DeviceFiles;
            Ls("/storage/emulated/0");


        }

        private void btn_cancel_Click(object sender, RoutedEventArgs e)
        {

            Client.CloseDialogPanel();

        }
        private void Button_SDcard_Click(object sender, RoutedEventArgs e)
        {

            Ls("/storage/emulated/0");

        }
        private void Button_AddDirectory_Click(object sender, RoutedEventArgs e)
        {
            ContentControl_MessageBox.Content = ContentControl_AddDirectory.Content;

            Grid_MessageBox.Visibility = Visibility.Visible;

        }
        private void Button_AddDirectory_Ok_Click(object sender, RoutedEventArgs e)
        {
            string DirectoryName = TextBox_AddDirectory_DirectoryName.Text.Trim();
            if (new Regex(".*[\u4e00-\u9fa5]{1,}.*").IsMatch(DirectoryName))
            {
                System.Windows.MessageBox.Show("目录名称中不能包含中文");
            }
            else
            {
                if (DirectoryName.Length > 0)
                {
                    Response res = device.ScriptEngine.AdbShell("cd " + CurrentDirectory + "&&mkdir -m 0777 " + DirectoryName);
                    Ls(CurrentDirectory);
                }
                Grid_MessageBox.Visibility = Visibility.Hidden;
            }

        }
        private void Button_AddDirectory_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Grid_MessageBox.Visibility = Visibility.Hidden;
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


        public static void GetAllFilesAndDirectory(DirectoryInfo dir)
        {
            FileInfo[] allFile = dir.GetFiles();
            foreach (FileInfo fi in allFile)
            {
                int index = fi.FullName.IndexOf(DirectoryAbsolutePath) + DirectoryAbsolutePath.Length;
                string RelativePath = fi.FullName.Substring(index,fi.FullName.Length- DirectoryAbsolutePath.Length).Replace("\\", "/");
                RelativePath = RelativePath.Remove(RelativePath.LastIndexOf("/"));
                FileList.Add(new FileInformation { FileName = fi.FullName, RelativePath = RelativePath, IsFile = true });
            }
            DirectoryInfo[] allDir = dir.GetDirectories();
            foreach (DirectoryInfo d in allDir)
            {
                int index = d.FullName.IndexOf(DirectoryAbsolutePath)+DirectoryAbsolutePath.Length;
                string RelativePath = d.FullName.Substring(index, d.FullName.Length - DirectoryAbsolutePath.Length).Replace("\\", "/");
                FileList.Add(new FileInformation { FileName = d.Name, RelativePath = RelativePath, IsFile = false });
                GetAllFilesAndDirectory(d);
            }
           
        }
        
        public class FileInformation
        {
            public string FileName { get; set; }
            public string RelativePath { get; set; }
            public bool IsFile { get; set; }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
        public void Ls(string dir)
        {
            CurrentDirectory = dir;
            this.Dispatcher.Invoke(new Action(() =>
            {
                TextBox_Current.Text = CurrentDirectory;
            }));
            Console.WriteLine("打开目录：" + dir);
            Response res = device.ScriptEngine.AdbShell("cd " + dir + "&&ls -al");
            if (res.Json["result"] != null)
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    DeviceFiles.Clear();
                }));
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

                list.Insert(0, new DeviceFile { Name = "上级目录", FullName = ((dir == "/" || dir == "") ? "/" : dir.Remove(dir.LastIndexOf("/"))), Type = "directory" });
                list.Insert(0, new DeviceFile { Name = "根目录", FullName = "/", Type = "directory" });

                DeviceFiles = new ObservableCollection<DeviceFile>(list);
                this.Dispatcher.Invoke(new Action(() =>
                {
                    ItemListBox.ItemsSource = DeviceFiles;
                }));

            }

        }

        private void Grid_Drop(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                new Thread(() =>
                {
                    Array arr = (System.Array)e.Data.GetData(System.Windows.DataFormats.FileDrop);
                    ShowLoading("准备上传...");
                    foreach (var obj in arr)
                    {
                        string filename = obj.ToString();
                        if (File.Exists(filename))
                        {
                            //上传文件
                            UploadFile(CurrentDirectory,filename);
                        }
                        else
                        {
                            DirectoryAbsolutePath = filename.Remove(filename.LastIndexOf("\\")+1);
                            Console.WriteLine("绝对路径："+DirectoryAbsolutePath);
                            //上传文件夹
                            UploadDirectory(CurrentDirectory,filename);
                        }
                       
                    }
                    Ls(CurrentDirectory);
                    CloseLoading();
                })
                { IsBackground = true }.Start();
            }


        }
        public void UploadFile(string dir,string filename)
        {
            ShowLoading("正在上传文件[" + filename + "]...");
            Response res = device.ScriptEngine.WriteBufferToFile(dir + "/" + new FileInfo(filename).Name, File.ReadAllBytes(filename));
            Console.WriteLine("文件上传完毕：" + res.Json.ToString());
           

        }
        public void UploadDirectory(string dir, string dirname)
        {
            ShowLoading("正在上传目录[" + dirname + "]...");
            FileList.Clear();
            GetAllFilesAndDirectory(new DirectoryInfo(dirname));
            var dirs = (from d in FileList where d.IsFile == false select d).ToList();
            var files = (from d in FileList where d.IsFile == true select d).ToList();
            //开始创建目录
            foreach (FileInformation fi in dirs)
            {
                ShowLoading("正在创建目录[" + fi.RelativePath + "]...");
                Response res = device.ScriptEngine.AdbShell("cd " + dir + "&&mkdir -m 0777 -p " + fi.RelativePath);
            }
            foreach (FileInformation fi in files)
            {
                UploadFile(dir+"/"+fi.RelativePath,fi.FileName);
            }

        }
        private void MenuItem_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (ItemListBox.SelectedItem != null)
            {


                var deviceFile = (DeviceFile)(ItemListBox.SelectedItem);
                Response res = device.ScriptEngine.AdbShell("rm -r -f " + deviceFile.FullName);
                Ls(CurrentDirectory);
            }
        }

        private void ItemListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ItemListBox.SelectedItem != null)
            {


                var deviceFile = (DeviceFile)(ItemListBox.SelectedItem);

                if (deviceFile.Type == "file") { System.Windows.MessageBox.Show("下载文件到本地"); }
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

        private void Button_RootDirectory_Click(object sender, RoutedEventArgs e)
        {
            Ls("/");
        }

        private void Button_GoBack_Click(object sender, RoutedEventArgs e)
        {
            var deviceFile = DeviceFiles.ToList().Find(p => p.Name == "上级目录");
            if (deviceFile != null)
            {
                Ls(deviceFile.FullName);
            }
        }

        private void Button_UploadDirectory_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new FolderBrowserDialog();

            var result = openFileDialog.ShowDialog();
           
            if (result == DialogResult.OK)
            {
                new Thread(() =>
                {

                    ShowLoading("准备上传...");

                    string filename = openFileDialog.SelectedPath;
                    DirectoryAbsolutePath = filename.Remove(filename.LastIndexOf("\\") + 1);
                    Console.WriteLine("绝对路径：" + DirectoryAbsolutePath);
                    UploadDirectory (CurrentDirectory, filename);

                    
                    Ls(CurrentDirectory);
                    CloseLoading();
                })
                { IsBackground = true }.Start();
            }

        }

        private void Button_UploadFile_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            var result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                new Thread(() =>
                {
                   
                    ShowLoading("准备上传...");
                    foreach (var obj in openFileDialog.FileNames)
                    {
                        string filename = obj.ToString();
                        UploadFile(CurrentDirectory, filename);
                        
                    }
                    Ls(CurrentDirectory);
                    CloseLoading();
                })
                { IsBackground = true }.Start();
            }
        }
        public void ShowLoading(string text)
        {
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


    }
}
