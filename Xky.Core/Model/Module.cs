using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;

namespace Xky.Core.Model
{
   public class Module : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private int _id = 0;
        private int _type = 0;
        private int _status = 0;
        private int _uid = 0;
        private int _price = 0;
        private string _name = "";
        private string _description = "";
        private ImageSource _logo ;
        private List<string> _tags =new List<string>();
        public int Id { get => _id; set
            {
                if (_id != value)
                {
                    _id = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Id"));
                }
            }
        }

        public int Type { get => _type; set
            {
                if (_type != value)
                {
                    _type = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Type"));
                }
            }
        }
        public int Status { get => _status; set
            {
                if (_status != value)
                {
                    _status = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Status"));
                }
            }
        }
        public int Uid { get => _uid; set
            {
                if (_uid != value)
                {
                    _uid = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Uid"));
                }
            }
        }
        public int Price { get => _price; set
            {
                if (_price != value)
                {
                    _price = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Price"));
                }
            }
        }
        public string Name { get => _name; set
            {
                if (_name != value)
                {
                    _name = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
                }
            }
        }
        public string Description { get => _description; set
            {
                if (_description != value)
                {
                    _description = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Description"));
                }
            }
        }
        public ImageSource Logo { get => _logo; set
            {
                if (_logo != value)
                {
                    _logo =value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Logo"));
                }
            }
        }
        public List<string> Tags { get => _tags; set => _tags = value; }
    }
}
