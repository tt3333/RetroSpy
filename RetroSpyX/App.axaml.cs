using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace RetroSpy
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop!.Startup += (sender, args) =>
                {
                    bool skipSetup = args.Args.Length == 1 && args.Args[0] == "-skipSetup";

                    desktop.MainWindow = new SetupWindow(skipSetup);
                };
            }

            base.OnFrameworkInitializationCompleted();
        }


    }
}