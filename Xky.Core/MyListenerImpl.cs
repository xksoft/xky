using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quobject.EngineIoClientDotNet.ComponentEmitter;

namespace Xky.Core
{
    public class MyListenerImpl : IListener, IComparable<IListener>
    {
        private static int id_counter;
        private int Id;
        private readonly Action fn1;
        private readonly Action<object> fn;

        public MyListenerImpl(Action<object> fn)
        {
            this.fn = fn;
            Id = id_counter++;
        }

        public MyListenerImpl(Action fn)
        {
            fn1 = fn;
            Id = id_counter++;
        }

        public void Call(params object[] args)
        {
            if (fn != null)
                fn(args.Length != 0 ? args[0] : (object) null);
            else
                fn1();
        }

        public int CompareTo(IListener other)
        {
            return GetId().CompareTo(other.GetId());
        }

        public int GetId()
        {
            return Id;
        }
    }
}