using System.ComponentModel;
using System.Windows.Media;

namespace Xky.Core.Model
{
    public class Tag : INotifyPropertyChanged
    {
        private string _name;
        private int _count;

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

        public event PropertyChangedEventHandler PropertyChanged;
    }
}