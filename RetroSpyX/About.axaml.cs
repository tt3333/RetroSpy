using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using System;
using System.Collections;
using System.Security.Policy;
using Xilium.CefGlue.Common;
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
