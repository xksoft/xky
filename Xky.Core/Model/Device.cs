using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;

namespace Xky.Core.Model
{
    public class Device : INotifyPropertyChanged
    {
        private string _connectionHash;
        private int _cpus;
        private string _description;
        private string _forward;
        private string _gpsLat;
        private string _gpsLng;
        private int _id;
        private int _memory;
        private string _model;
        private string _name;
        private string _node;
        private string _nodeSerial;
        private string _nodeUrl;
        private string _product;
        private ImageSource _screenShot;
        private string _sn;
        private int _cpuUseage = 5;
        private int _memoryUseage = 5;
        private int _diskUseage=5;
       

        //加载时序
        public long LoadTick { get; set; }

        /// <summary>
        ///     设备id
        /// </summary>
        public int Id
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Id"));
                }
            }
        }

        /// <summary>
        ///     设备序列号
        /// </summary>
        public string Sn
        {
            get => _sn;
            set
            {
                if (_sn != value)
                {
                    _sn = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Sn"));
                }
            }
        }

        /// <summary>
        ///     节点信息
        /// </summary>
        public string NodeSerial
        {
            get => _nodeSerial;
            set
            {
                if (_nodeSerial != value)
                {
                    _nodeSerial = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NodeSerial"));
                }
            }
        }

        /// <summary>
        ///     设备名
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
                }
            }
        }

        /// <summary>
        ///     坐落的服务器
        /// </summary>
        public string Forward
        {
            get => _forward;
            set
            {
                if (_forward != value)
                {
                    _forward = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Forward"));
                }
            }
        }

        /// <summary>
        ///     节点地址
        /// </summary>
        public string NodeUrl
        {
            get => _nodeUrl;
            set
            {
                if (_nodeUrl != value)
                {
                    _nodeUrl = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NodeUrl"));
                }
            }
        }

        /// <summary>
        ///     设备备注
        /// </summary>
        public string Description
        {
            get => _description;
            set
            {
                if (_description != value)
                {
                    _description = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Description"));
                }
            }
        }

        /// <summary>
        ///     节点服务器
        /// </summary>
        public string Node
        {
            get => _node;
            set
            {
                if (_node != value)
                {
                    _node = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Node"));
                }
            }
        }

        /// <summary>
        ///     设备型号
        /// </summary>
        public string Model
        {
            get => _model;
            set
            {
                if (_model != value)
                {
                    _model = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Model"));
                }
            }
        }

        /// <summary>
        ///     设备厂商
        /// </summary>
        public string Product
        {
            get => _product;
            set
            {
                if (_product != value)
                {
                    _product = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Product"));
                }
            }
        }

        /// <summary>
        ///     连接密钥
        /// </summary>
        public string ConnectionHash
        {
            get => _connectionHash;
            set
            {
                if (_connectionHash != value)
                {
                    _connectionHash = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ConnectionHash"));
                }
            }
        }

        /// <summary>
        ///     GpsLat
        /// </summary>
        public string GpsLat
        {
            get => _gpsLat;
            set
            {
                if (_gpsLat != value)
                {
                    _gpsLat = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("GpsLat"));
                }
            }
        }

        /// <summary>
        ///     GpsLng
        /// </summary>
        public string GpsLng
        {
            get => _gpsLng;
            set
            {
                if (_gpsLng != value)
                {
                    _gpsLng = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("GpsLng"));
                }
            }
        }


        /// <summary>
        ///     Cpu个数
        /// </summary>
        public int Cpus
        {
            get => _cpus;
            set
            {
                if (_cpus != value)
                {
                    _cpus = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Cpus"));
                }
            }
        }

        /// <summary>
        ///     内存数量
        /// </summary>
        public int Memory
        {
            get => _memory;
            set
            {
                if (_memory != value)
                {
                    _memory = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Memory"));
                }
            }
        }

        public string GetHardwareDesc => Cpus + "核心 " + (Memory / (double) 1024 / 1024).ToString("F2") + "GB内存";


        /// <summary>
        ///     画面截图
        /// </summary>
        public ImageSource ScreenShot
        {
            get => _screenShot;
            set
            {
                _screenShot = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ScreenShot"));
            }
        }


        public int CpuUseage
        {
            get => _cpuUseage;
            set
            {
                if (_cpuUseage != value)
                {
                    _cpuUseage = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CpuUseage"));
                }
            }
        }

        public int MemoryUseage
        {
            get => _memoryUseage;
            set
            {
                if (_memoryUseage != value)
                {
                    _memoryUseage = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MemoryUseage"));
                }
            }
        }

        public int DiskUseage
        {
            get => _diskUseage;
            set
            {
                if (_diskUseage != value)
                {
                    _diskUseage = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DiskUseage"));
                }
            }
        }

        public Script ScriptEngine { get; set; }
        public ObservableCollection<Module> RunningModules = new ObservableCollection<Module>();
        public event PropertyChangedEventHandler PropertyChanged;
    }
}