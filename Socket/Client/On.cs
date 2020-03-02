using System;
using Xky.Socket.Engine.ComponentEmitter;

namespace Xky.Socket.Client
{
    public class On
    {
        private On()
        {
        }

        public static IHandle Create(Emitter obj, string ev, IListener fn)
        {
            obj.On(ev, fn);
            return new HandleImpl(obj, ev, fn);
        }

        public class HandleImpl : IHandle
        {
            private readonly string _ev;
            private readonly IListener _fn;
            private readonly Emitter _obj;

            public HandleImpl(Emitter obj, string ev, IListener fn)
            {
                _obj = obj;
                _ev = ev;
                _fn = fn;
            }

            public void Destroy()
            {
                _obj.Off(_ev, _fn);
            }
        }

        public class ActionHandleImpl : IHandle
        {
            private readonly Action _fn;

            public ActionHandleImpl(Action fn)
            {
                _fn = fn;
            }

            public void Destroy()
            {
                _fn();
            }
        }

        public interface IHandle
        {
            void Destroy();
        }
    }
}