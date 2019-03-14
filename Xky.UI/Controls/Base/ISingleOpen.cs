using System;

namespace Xky.UI.Controls.Base
{
    public interface ISingleOpen : IDisposable
    {
        bool CanDispose { get; }
    }
}