using System;

namespace Xky.Core.Model
{
    public class License
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Avatra { get; set; }

        public string LicenseKey { get; set; }

        public string LicenseName { get; set; }

        public int LicenseLevel { get; set; }

        public string LicenseCustom { get; set; }

        public DateTime LicenseExpiration { get; set; }

        public string Session { get; set; }
    }
}