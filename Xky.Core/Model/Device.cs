using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using Xky.Core.Annotations;

namespace Xky.Core.Model
{
    public class Device : INotifyPropertyChanged
    {
        private string _name;
        private int _id;
        private string _sn;
        private string _forward;
        private string _description;
        private string _node;
        private string _model;
        private string _product;
        private string _connectionHash;
        private string _gpsLat;
        private string _gpsLng;
        private ImageSource _screenShot;
        private int _cpus;
        private int _memory;
        private string _nodeUrl;

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
        /// 节点地址
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
        /// Cpu个数
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
        /// 内存数量
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


        /// <summary>
        /// 画面截图
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

        public event PropertyChangedEventHandler PropertyChanged;
    }
}