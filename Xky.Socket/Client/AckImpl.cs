using System;

namespace Xky.Socket.Client
{
    public class AckImpl : IAck
    {
        private readonly Action _fn0;
        private readonly Action<object> _fn1;
        private readonly Action<object, object> _fn2;
        private readonly Action<object, object, object> _fn3;
        private readonly Action<object, object, object, object> _fn4;

        public AckImpl(Action fn)
        {
            _fn0 = fn;
        }

        public AckImpl(Action<object> fn)
        {
            _fn1 = fn;
        }

        public AckImpl(Action<object, object> fn)
        {
            _fn2 = fn;
        }

        public AckImpl(Action<object, object, object> fn)
        {
            _fn3 = fn;
        }

        public AckImpl(Action<object, object, object, object> fn)
        {
            _fn4 = fn;
        }

        public void Call(params object[] args)
        {
            if (_fn0 != null)
            {
                _fn0();
            }
            else if (_fn1 != null)
            {
                var arg0 = args.Length > 0 ? args[0] : null;
                _fn1(arg0);
            }
            else if (_fn2 != null)
            {
                var arg0 = args.Length > 0 ? args[0] : null;
                var arg1 = args.Length > 1 ? args[1] : null;
                _fn2(arg0, arg1);
            }
            else if (_fn3 != null)
            {
                var arg0 = args.Length > 0 ? args[0] : null;
                var arg1 = args.Length > 1 ? args[1] : null;
                var arg2 = args.Length > 2 ? args[2] : null;
                _fn3(arg0, arg1, arg2);
            }
            else if (_fn4 != null)
            {
                var arg0 = args.Length > 0 ? args[0] : null;
                var arg1 = args.Length > 1 ? args[1] : null;
                var arg2 = args.Length > 2 ? args[2] : null;
                var arg3 = args.Length > 3 ? args[3] : null;
                _fn4(arg0, arg1, arg2, arg3);
            }
        }
    }
}