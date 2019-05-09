using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xky.XModule.ScreenTrain
{
   public class TransResult
    {
        public int status;
        public string action = "";
        public string msg = "";
        public long count;
    }
    public class TransItem {

        public string Type { get; set; }
        public double Confidence { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
