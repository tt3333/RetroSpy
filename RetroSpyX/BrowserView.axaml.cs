using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Xilium.CefGlue.Avalonia;

namespace Xilium.CefGlue.Demo.Avalonia
{
    public partial class BrowserView : UserControl
    {
        private readonly AvaloniaCefBrowser browser;
        private readonly string aboutPath;

        public BrowserView()
        {
            AvaloniaXamlLoader.Load(this);

            var browserWrapper = this.FindControl<Decorator>("browserWrapper");
            browser = new AvaloniaCefBrowser();

            string strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string? strWorkPath = Path.GetDirectoryName(strExeFilePath) ?? ".";
            aboutPath = Path.Join(strWorkPath, "About.html");

            browser.Address = aboutPath;
            browser.LoadStart += OnBrowserLoadStart;
            browserWrapper.Child = browser;
        }

        private void OnBrowserLoadStart(object sender, Common.Events.LoadStartEventArgs e)
        {
            if (!e.Frame.Url.Contains("About.html"))
            {
                browser.Address = aboutPath;
                try
                {
                    Process.Start(e.Frame.Url);
                }
                catch
                {
                    string url = e.Frame.Url;
                    // hack because of this: https://github.com/dotnet/corefx/issues/10361
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        url = e.Frame.Url.Replace("&", "^&");
                        Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    {
                        Process.Start("xdg-open", url);
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    {
                        Process.Start("open", url);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        public void Dispose()
        {
            browser.Dispose();
        }
    }
}