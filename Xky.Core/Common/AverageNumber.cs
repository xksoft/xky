using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xky.Core.Common
{
    public class AverageNumber
    {
        private readonly long[] _total;
        private int _lastIndex;

        public AverageNumber(int length)
        {
            _total = new long[length];
        }

        public void Push(long value)
        {
            var index = DateTime.Now.Second % _total.Length;


            if (_lastIndex != index)
            {
                _total[index] = 0;
                _lastIndex = index;
            }

            _total[index] += value;
        }

        public long GetAverageNumber()
        {
            var lastNum = _total[_lastIndex];
            return (_total.Sum()- lastNum) / (_total.Length-1);
        }
    }
}