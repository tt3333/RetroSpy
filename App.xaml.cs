using System;
using System.Collections.Generic;
using System.Windows;

namespace RetroSpy
{
	public partial class App : Application
	{
		private void Application_Startup(object sender, StartupEventArgs e)
		{

			bool skipSetup = e.Args.Length == 1 && e.Args[0] == "-skipSetup";

			_ = new SetupWindow(skipSetup);
		}
	}
}