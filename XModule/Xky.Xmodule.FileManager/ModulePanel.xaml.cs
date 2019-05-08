using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        public static List<DeviceFile> FileList_Device = new List<DeviceFile>();
        public static string  DirectoryAbsolutePath="";
        
        public ObservableCollection<DeviceFile> DeviceFiles = new ObservableCollection<DeviceFile>();
        public class DeviceFile
        {
            private string _name = "";
            private string _type = "文件";
            private string _fullName = "";
            public string Name { get => _name; set => _name = value; }
            public string Type { get => _type; set => _type = value; }
            public string FullName { get => _fullName; set => _fullName = value; }
            public string Size { get; set; }
        }
        public class FileInformation
        {
            public string FileName { get; set; }
            public string RelativePath { get; set; }
            public bool IsFile { get; set; }

        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ItemListBox.ItemsSource = DeviceFiles;
            new Thread(() =>
            {
                ShowLoading("正在加载文件列表...");
                Ls("/storage/emulated/0");
                CloseLoading();
            })
            { IsBackground = true }.Start();
        }

       
        private void Button_SDcard_Click(object sender, RoutedEventArgs e)
        {

            GoToDir("/storage/emulated/0");

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

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
        public void Ls(string dir)
        {


            CurrentDirectory = dir;
            this.Dispatcher.Invoke(new Action(() =>
            {
                TextBox_CurrentPath.Text = CurrentDirectory;
            }));
            Console.WriteLine("打开目录：" + dir);

           



            Response res = device.ScriptEngine.ReadDir(dir);
            if (res.Json["list"] != null)
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    DeviceFiles.Clear();
                }));
               
                List<DeviceFile> list= GetDeviceFilesFromLs(dir,(JArray)res.Json["list"]);
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
        public void GetAllDeviceFilesAndDirectory(string dir)
        {
            Response res = device.ScriptEngine.ReadDir(dir);
            if (res.Json["result"] != null)
            {
                List<DeviceFile> list = GetDeviceFilesFromLs(dir, (JArray)res.Json["list"]);
                foreach (var deviceFile in list)
                {
                    FileList_Device.Add(deviceFile);
                    if (deviceFile.Type != "file")
                    {
                        GetAllDeviceFilesAndDirectory(deviceFile.FullName);
                    }

                }
            }
        }
        public List<DeviceFile> GetDeviceFilesFromLs(string dir, JArray array)
        {
            List<DeviceFile> list = new List<DeviceFile>();
            foreach (JObject obj in array)
            {
                DeviceFile deviceFile = new DeviceFile();
                deviceFile.Name = obj["name"].ToString();
                if (((bool)obj["isFIle"]))
                {
                    deviceFile.Type = "file";
                }
                else if (((bool)obj["isDirectory"]))
                {
                    deviceFile.Type = "directory";
                }
                else if (((bool)obj["isBlockDevice"]))
                {
                    deviceFile.Type = "blockdevice";
                }
                else if (((bool)obj["isCharacterDevice"]))
                {
                    deviceFile.Type = "characterdevice";
                }
                else if (((bool)obj["isSymbolicLink"]))
                {
                    deviceFile.Type = "symboliclink";
                }
                else {
                    deviceFile.Type = "directory";
                }
                if (deviceFile.Type == "file")
                {
                    long size = 0;
                    long.TryParse(obj["size"].ToString(), out size);
                    if (size >= 1073741824)
                    {
                        //超过1G
                        deviceFile.Size = (size / 1073741824.0).ToString("0.0") + "G";
                    }
                    else if (size >= 1024 * 1024)
                    {
                        //超过1M
                        deviceFile.Size = (size / 1048576.0).ToString("0.0") + "M";
                    }
                    else
                    {
                        deviceFile.Size = (size / 1024.0).ToString("0.0") + "KB";
                    }
                    if (deviceFile.Size == "0.0KB")
                    {
                        deviceFile.Size = "-";
                    }
                }
                deviceFile.FullName = dir + "/" + deviceFile.Name;
                list.Add(deviceFile);

            }
          
            return list;
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
            FileInfo fi = new FileInfo(filename);
            string filename_new = fi.Name.Remove(fi.Name.LastIndexOf(fi.Extension));
            if (filename_new.Length>50) {
                filename_new.Remove(50);
            }
            if (fi.Length > 10485760)
            {
                //大于10M的文件采用分段上传

                FileStream fs = new FileStream(filename, FileMode.Open);

                string tempdir = filename_new + "_temp";
                int i = 1;
                byte[] bytes = new byte[10485760];
                string catf = "";
                int readlength = fs.Read(bytes, 0, 10485760);
                while (readlength > 0)
                {
                    ShowLoading("使用大文件上传模式，预计还需" + ((fi.Length / 10485760) - i + 1) + "秒钟...");
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
                    ShowLoading("正在合并文件，预计还需" + w + "秒钟...");
                    Thread.Sleep(1000);
                }
                Response res = device.ScriptEngine.AdbShell("cd " + dir + "/" + tempdir + "&&cat " + catf + " > ../" + filename_new + fi.Extension + "&&rm -r -f " + dir + "/" + tempdir);


            }
            else
            {
                Response res = device.ScriptEngine.WriteBufferToFile(dir + "/" + filename_new+fi.Extension, File.ReadAllBytes(filename));
            }

            device.ScriptEngine.AdbShell("am broadcast -a android.intent.action.MEDIA_SCANNER_SCAN_FILE -d file://" + dir + "/" + filename_new + fi.Extension);
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
            if (ItemListBox.SelectedItems.Count > 0)
            {
                var deviceFiles =ItemListBox.SelectedItems;
                new Thread(() =>
                {
                    ShowLoading("准备删除...");
                   
                    for (int i = 0; i < deviceFiles.Count; i++)
                    {
                        var deviceFile = (DeviceFile)deviceFiles[i];
                        if (deviceFile.Name == "根目录" || deviceFile.Name == "上级目录")
                        {
                            continue;
                        }
                        ShowLoading("正在删除[" + deviceFile.Name + "]...");
                        Response res = device.ScriptEngine.AdbShell("rm -r -f " + deviceFile.FullName);
                    }
                    RescanningMedia();
                    Ls(CurrentDirectory);
                    CloseLoading();
                })
                { IsBackground = true }.Start();
            }
        }
        public void RescanningMedia()
        {
            Response res_reload = device.ScriptEngine.AdbShell("am broadcast -a android.intent.action.MEDIA_MOUNTED -d file:///sdcard/");
            Console.WriteLine(res_reload.Json);
            res_reload = device.ScriptEngine.AdbShell("am broadcast -a android.intent.action.MEDIA_SCANNER_SCAN_FILE -d file://"+CurrentDirectory);
            Console.WriteLine(res_reload.Json);
            res_reload = device.ScriptEngine.AdbShell("am broadcast -a android.intent.action.BOOT_COMPLETED -n com.android.providers.media/.MediaScannerReceiver");
            Console.WriteLine(res_reload.Json);
        }
        private void MenuItem_DownLoad_Click(object sender, RoutedEventArgs e)
        {
            if (ItemListBox.SelectedItems.Count > 0)
            {
                var openFileDialog = new FolderBrowserDialog();

                var result = openFileDialog.ShowDialog();

                if (result == DialogResult.OK)
                {
                    FileList_Device.Clear();
                    var deviceFiles = ItemListBox.SelectedItems;
                    new Thread(() =>
                    {
                        ShowLoading("准备下载...");
                        for (int i = 0; i < deviceFiles.Count; i++)
                        {
                            var deviceFile = (DeviceFile)deviceFiles[i];
                            if (deviceFile.Name == "根目录" || deviceFile.Name == "上级目录")
                            {
                                continue;
                            }
                           
                            if (deviceFile.Type == "file")
                            {
                                FileList_Device.Add(deviceFile);
                               
                            }
                            else
                            {
                                ShowLoading("正在遍历目录[" + deviceFile.FullName + "]...");
                                GetAllDeviceFilesAndDirectory(deviceFile.FullName);
                            }

                            var files = (from f in FileList_Device where f.Type == "file" select f).ToList();
                            var dirs = (from f in FileList_Device where f.Type != "file" select f).ToList();
                            foreach (var dir in dirs)
                            {

                                string RelativePath = dir.FullName.Substring(dir.FullName.IndexOf(CurrentDirectory) + CurrentDirectory.Length);
                                if (RelativePath.StartsWith("/"))
                                {
                                    RelativePath = RelativePath.Substring(1);
                                }
                                DirectoryInfo selectDir = new DirectoryInfo(openFileDialog.SelectedPath);
                                selectDir.CreateSubdirectory(RelativePath);
                            }
                            foreach (var file in files)
                            {
                                ShowLoading("正在下载文件[" + file.FullName + "]...");
                                DownloadFile(openFileDialog.SelectedPath,file);
                                //string RelativePath = file.FullName.Substring(file.FullName.IndexOf(CurrentDirectory) + CurrentDirectory.Length);
                                //if (RelativePath.StartsWith("/"))
                                //{
                                //    RelativePath = RelativePath.Substring(1);
                                //}
                              
                                //var response = device.ScriptEngine.ReadBufferFromFile(deviceFile.FullName);
                                //if (response.Result)
                                //{
                                //    var data = (byte[])(response.Json["buffer"] as JArray)?.First;
                                //    string filename = openFileDialog.SelectedPath + "\\" + RelativePath;
                                //    File.WriteAllBytes(filename,data);
                                //}
                            }
                        }
                        CloseLoading();
                        Process.Start(openFileDialog.SelectedPath);
                    })
                    { IsBackground = true }.Start();
                }
            }
        }
        public string DownloadFile(string path,DeviceFile file) {

            string RelativePath = file.FullName.Substring(file.FullName.IndexOf(CurrentDirectory) + CurrentDirectory.Length);
            if (RelativePath.StartsWith("/"))
            {
                RelativePath = RelativePath.Substring(1);
            }

            var response = device.ScriptEngine.ReadBufferFromFile(file.FullName);
            if (response.Result)
            {
                string filename = path + "\\" + RelativePath;
                var data = (response.Json["buffer"] as JArray);
              
                using (FileStream fsw = new FileStream(filename, FileMode.Create))
                {
                    if (data != null)
                    {

                        for (int i = 0; i < data.Count; i++)
                        {
                            byte[] bs = (byte[])data[i];
                            fsw.Write(bs, 0, bs.Length);
                        }

                        fsw.Close();
                        return filename;
                    }


                }



            }
            
            return ""; 
        }

        public void GoToDir(string dir)
        {
            new Thread(() =>
            {
                ShowLoading("正在加载文件列表...");
                Ls(dir);
                CloseLoading();
            })
            { IsBackground = true }.Start();
        }
        private void ItemListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ItemListBox.SelectedItem != null)
            {


                var deviceFile = (DeviceFile)(ItemListBox.SelectedItem);
                new Thread(() =>
                {
                    
                    if (deviceFile.Type == "file")
                    {
                        ShowLoading("正在下载文件["+ deviceFile .FullName+ "]...");
                        var tempdir = System.Environment.GetEnvironmentVariable("TEMP");
                        string filename = DownloadFile(tempdir, deviceFile);
                        if (filename.Length > 0)
                        {
                            try
                            {
                                Process p = Process.Start(filename);
                               
                            }
                            catch
                            {
                                System.Diagnostics.Process.Start("Explorer.exe", @"/select," + filename);
                            }
                        }
                        else {
                            System.Windows.MessageBox.Show("文件下载失败！");
                        }

                    }
                    else
                    {

                        GoToDir(deviceFile.FullName);
                           
                          
                       

                    }
                    CloseLoading();
                })
                { IsBackground = true }.Start();
            }

        }

        private void ItemListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ItemListBox.SelectedItem != null)
            {


                var deviceFile = (DeviceFile)(ItemListBox.SelectedItem);
                TextBox_CurrentPath.Text = deviceFile.FullName;

            }
        }

        private void Button_RootDirectory_Click(object sender, RoutedEventArgs e)
        {
            GoToDir("/");
        }

        private void Button_GoBack_Click(object sender, RoutedEventArgs e)
        {
            var deviceFile = DeviceFiles.ToList().Find(p => p.Name == "上级目录");
            if (deviceFile != null)
            {
                GoToDir(deviceFile.FullName);
              
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
                    RescanningMedia();
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
                    RescanningMedia();
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

        private void Button_TempDirectory_Click(object sender, RoutedEventArgs e)
        {
            var tempdir = System.Environment.GetEnvironmentVariable("TEMP");
            Process.Start(tempdir);
        }

        private void Button_SetClipboard_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Windows.Clipboard.SetDataObject(TextBox_CurrentPath.Text, true);
            }
            catch { }
        }
        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            Client.CloseDialogPanel();
        }
    }
}
