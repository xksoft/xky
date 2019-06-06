using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xky.XModule.AppBackup
{
  public  class Model
    {
        public class BDevice
        {
            private string _device_Name = "";
            private string _device_Sn = "";
            public string Device_Name { get => _device_Name; set => _device_Name = value; }
            public ObservableCollection<Backup> Backups
            {
                get => _backups;
                set=>_backups = value;
                     
            }
            public string Device_Sn { get => _device_Sn; set => _device_Sn = value; }

            private ObservableCollection<Backup> _backups = new ObservableCollection<Backup>();
            /// <summary>
            /// 属性触发
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;
        }
        public class Backup
        {
            private string _name = "";
            private string _device_Sn = "";
            private bool _isCurrent = false;
            public string Name { get => _name; set => _name = value; }
            public string Device_Sn { get => _device_Sn; set => _device_Sn = value; }
            public bool IsCurrent { get => _isCurrent;
                set
                {
                    if (_isCurrent != value)
                    {
                        _isCurrent = value;
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsCurrent"));
                    }
                }
            }
            /// <summary>
            /// 属性触发
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;
        }
    }
}
