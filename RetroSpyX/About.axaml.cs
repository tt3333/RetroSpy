using Avalonia.Controls;
using System.Collections;
using Xilium.CefGlue.Demo.Avalonia;

namespace RetroSpy
{
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();

            var tabItems = ((IList)this.FindControl<DockPanel>("dockPanel").Children);

            var view = new BrowserView();




            tabItems.Add(view);

        }
    }
}
