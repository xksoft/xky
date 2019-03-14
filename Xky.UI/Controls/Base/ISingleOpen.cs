using System;

namespace Xky.UI.Controls
{
    public interface ISingleOpen : IDisposable
    {
        bool CanDispose { get; }
    }
}