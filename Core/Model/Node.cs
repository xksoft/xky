using System.ComponentModel;

namespace Xky.Core.Model
{
    /// <summary>
    /// 节点模型
    /// </summary>
    public class Node : INotifyPropertyChanged
    {
        private int _id = 0;
        private string _connectionHash;
        private int _connectStatus;
        private int _deviceCount;
        private string _forward;
        private string _ip;
        private string _name;
        private XSocket.Client.Socket _nodeSocket;
        private string _nodeUrl;
        private string _serial;

        public long LoadTick { get; set; }

        /// <summary>
        ///  编号
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
        /// 节点名称
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
        /// 序列号
        /// </summary>
        public string Serial
        {
            get => _serial;
            set
            {
                if (_serial != value)
                {
                    _serial = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Serial"));
                }
            }
        }

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

        public string Ip
        {
            get => _ip;
            set
            {
                if (_ip != value)
                {
                    _ip = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Ip"));
                }
            }
        }

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

        public int DeviceCount
        {
            get => _deviceCount;
            set
            {
                if (_deviceCount != value)
                {
                    _deviceCount = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DeviceCount"));
                }
            }
        }

        /// <summary>
        /// 连接状态 0：未连接 1：局域网连接 2：外网连接
        /// </summary>
        public int ConnectStatus
        {
            get => _connectStatus;
            set
            {
                if (_connectStatus != value)
                {
                    _connectStatus = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ConnectStatus"));
                }
            }
        }

        public XSocket.Client.Socket NodeSocket
        {
            get => _nodeSocket;
            set
            {
                if (_nodeSocket != value)
                {
                    _nodeSocket = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NodeSocket"));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}