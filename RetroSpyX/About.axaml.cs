using Avalonia.Controls;
using System.Collections;
using System.Runtime.InteropServices;
using Xilium.CefGlue.Demo.Avalonia;

namespace RetroSpy
{
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var tabItems = ((IList)this.FindControl<DockPanel>("dockPanel").Children);
                var view = new BrowserView();
                tabItems.Add(view);
            }

        }
    }
}
