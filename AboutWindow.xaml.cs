using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RetroSpy
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        private static bool willNavigate;

        public AboutWindow()
        {
            InitializeComponent();
            wb.NavigateToString(string.Format(CultureInfo.CurrentCulture, @"
<HTML>
    <BODY style='color: #CBCBCB; background-color: #252526; font-family: ""Segoe UI""'>
<CENTER>Version: {0}</CENTER>
<CENTER>Build Timestamp: {1}</CENTER>
<BR>
        <TABLE WIDTH='100%' BORDER='0' PADDING='0' >
            <TR><TD ALIGN='CENTER' ><H1>Supported By</H1></TD></TR>
            <TR><td ALIGN='CENTER'><A HREF=""https://twitch.tv/sk84uhlivin"">sk84uhlivin<A></TD></TR>
            <TR><td ALIGN='CENTER'>watsonpunk</TD></TR>            
            <TR><td ALIGN='CENTER'>Coltaho</TD></TR>            
            <TR><td ALIGN='CENTER'>40wattRange</TD></TR>
            <TR><td ALIGN='CENTER'><A HREF=""https://twitter.filyx20.dev"">Filyx20</A></TD></TR>
            <TR><td ALIGN='CENTER'>Evan Grill</TD></TR>
            <TR><td ALIGN='CENTER'>Vike</TD></TR>        
        </TABLE>
    </BODY>
</HTML>", Assembly.GetEntryAssembly().GetName().Version, DateTime.Now));

        }
        private void webBrowser1_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            // first page needs to be loaded in webBrowser control
            if (!willNavigate)
            {
                willNavigate = true;
                return;
            }

            // cancel navigation to the clicked link in the webBrowser control
            e.Cancel = true;

            var startInfo = new ProcessStartInfo
            {
                FileName = e.Uri.ToString()
            };

            Process.Start(startInfo);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            willNavigate = false;
        }
    }
}
