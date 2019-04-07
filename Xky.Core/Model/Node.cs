using System.ComponentModel;

namespace Xky.Core.Model
{
    public class Node : INotifyPropertyChanged
    {
        private string _connectionHash;
        private int _connectStatus;
        private int _deviceCount;
        private string _forward;
        private string _ip;
        private string _name;
        private Socket.Client.Socket _nodeSocket;
        private string _nodeUrl;
        private string _serial;

        public long LoadTick { get; set; }


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

        public Socket.Client.Socket NodeSocket
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