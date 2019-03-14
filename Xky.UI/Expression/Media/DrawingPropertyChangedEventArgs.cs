using System;

namespace Xky.UI.Expression.Media
{
    internal class DrawingPropertyChangedEventArgs : EventArgs
    {
        public bool IsAnimated { get; set; }

        public DrawingPropertyMetadata Metadata { get; set; }
    }
}