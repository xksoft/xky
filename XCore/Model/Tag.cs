using System.Collections.Generic;
using System.ComponentModel;

namespace XCore.Model
{
    /// <summary>
    /// 标签模型
    /// </summary>
    public class Tag : INotifyPropertyChanged
    {
        private int _count;
        private string _name;
        private List<Device> _devices;

        /// <summary>
        /// 标签名
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
        /// 数量
        /// </summary>
        public int Count
        {
            get => _count;
            set
            {
                if (_count != value)
                {
                    _count = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Count"));
                }
            }
        }

        /// <summary>
        /// 分类的设备
        /// </summary>
        public List<Device> Devices
        {
            get => _devices;
            set
            {
                if (_devices != value)
                {
                    _devices = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Devices"));
                }
            }
        }

        /// <summary>
        /// 变更事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
    }
}