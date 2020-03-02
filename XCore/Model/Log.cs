using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCore.Model
{
   public class Log
    {
        private int _type = 0;
        private string _content = "";
        private string _date = "";
        private string _title = "";
       

        public int Type
        {
            get => _type;
            set
            {
                if (_type != value)
                {
                    _type = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Type"));
                }
            }
        }
        public string Content { get => _content;
            set
            {
                if (_content != value)
                {
                    _content = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Content"));
                }
            }
        }
        public string Date { get => _date;
            set
            {
                if (_date != value)
                {
                    _date = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Date"));
                }
            }
        }
        public string Title { get => _title;
            set
            {
                if (_title != value)
                {
                    _title = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Title"));
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

    }
}
