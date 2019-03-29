using System;
using System.ComponentModel;

namespace Xky.Core.Model
{
    public class License : INotifyPropertyChanged
    {
        private string _avatra;
        private string _email;
        private int _id;
        private string _licenseCustom;
        private DateTime _licenseExpiration;
        private string _licenseKey;
        private int _licenseLevel;
        private string _licenseName;
        private string _name;
        private string _phone;
        private string _session;

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

        public string Email
        {
            get => _email;
            set
            {
                if (_email != value)
                {
                    _email = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Email"));
                }
            }
        }

        public string Phone
        {
            get => _phone;
            set
            {
                if (_phone != value)
                {
                    _phone = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Phone"));
                }
            }
        }

        public string Avatra
        {
            get => _avatra;
            set
            {
                if (_avatra != value)
                {
                    _avatra = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Avatra"));
                }
            }
        }

        public string LicenseKey
        {
            get => _licenseKey;
            set
            {
                if (_licenseKey != value)
                {
                    _licenseKey = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LicenseKey"));
                }
            }
        }

        public string LicenseName
        {
            get => _licenseName;
            set
            {
                if (_licenseName != value)
                {
                    _licenseName = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LicenseName"));
                }
            }
        }

        public int LicenseLevel
        {
            get => _licenseLevel;
            set
            {
                if (_licenseLevel != value)
                {
                    _licenseLevel = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LicenseLevel"));
                }
            }
        }

        public string LicenseCustom
        {
            get => _licenseCustom;
            set
            {
                if (_licenseCustom != value)
                {
                    _licenseCustom = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LicenseCustom"));
                }
            }
        }

        public DateTime LicenseExpiration
        {
            get => _licenseExpiration;
            set
            {
                if (_licenseExpiration != value)
                {
                    _licenseExpiration = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("_licenseExpiration"));
                }
            }
        }

        public string Session
        {
            get => _session;
            set
            {
                if (_session != value)
                {
                    _session = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Session"));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}