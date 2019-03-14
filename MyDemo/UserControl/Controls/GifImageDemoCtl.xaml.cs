using System;

namespace MyDemo.UserControl.Controls
{
    public partial class GifImageDemoCtl : IDisposable
    {
        public GifImageDemoCtl()
        {
            InitializeComponent();
        }

        public void Dispose()
        {
            GifImageMain.Dispose();
        }
    }
}
