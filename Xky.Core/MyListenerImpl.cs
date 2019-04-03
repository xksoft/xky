using System;
using Quobject.EngineIoClientDotNet.ComponentEmitter;

namespace Xky.Core
{
    public class MyListenerImpl : IListener, IComparable<IListener>
    {
        private static int _idCounter;
        private readonly int Id;
        private readonly Action _fn;
        private readonly Action<object> _fn1;
        private readonly Action<object, object> _fn2;

        public MyListenerImpl(Action<object> fn)
        {
            _fn1 = fn;
            Id = _idCounter++;
        }


        public MyListenerImpl(Action<object, object> fn)
        {
            _fn2 = fn;
            Id = _idCounter++;
        }

        public MyListenerImpl(Action fn)
        {
            this._fn = fn;
            Id = _idCounter++;
        }

        public void Call(params object[] args)
        {
            if (_fn2 != null)
                _fn2(args[0], args[1]);
            else if (_fn1 != null)
                _fn1(args[0]);
            else
                _fn();
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