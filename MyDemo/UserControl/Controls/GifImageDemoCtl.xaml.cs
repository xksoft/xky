using System;

namespace MyDemo.UserControl
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
