using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Navigation;

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
    <BODY ALINK=""#CBCBCB"" VLINK=""#CBCBCB"" LINK=""#CBCBCB"" style='color: #CBCBCB; background-color: #252526; font-family: ""Segoe UI""'>
<CENTER>Version: {0}</CENTER>
<CENTER>Build Timestamp: {1}</CENTER>
<BR>
        <TABLE WIDTH='100%' BORDER='0' PADDING='0' >
            <TR><TD ALIGN='CENTER' ><H1>Supported By</H1></TD></TR>            
            <TR><td ALIGN='CENTER'>sk84uhlivin</TD></TR>
            <TR><td ALIGN='CENTER'><A HREF=""https://twitch.tv/watsonpunk"">watsonpunk</A></TD></TR>            
            <TR><td ALIGN='CENTER'>Coltaho</TD></TR>            
            <TR><td ALIGN='CENTER'><A HREF=""http://evangrill.com"">Evan Grill</A></TD></TR>  
            <TR><td ALIGN='CENTER'><A HREF=""https://twitter.com/VikeMK"">Vike</A></TD></TR>       
            <TR><td ALIGN='CENTER'><A HREF=""https://twitch.tv/mrgamy"">MrGamy</A></TD></TR>
            <TR><td ALIGN='CENTER'><A HREF=""https://discord.gg/tyvzPbu5fv"">peco_de_guile</A></TD></TR>
            <TR><td ALIGN='CENTER'>Mellified</TD></TR> 
            <TR><td ALIGN='CENTER'>Karl W. Reinsch</TD></TR>
        </TABLE>
    </BODY>
</HTML>", Assembly.GetEntryAssembly().GetName().Version, DateTime.Now));

        }
        private void WebBrowser1_Navigating(object sender, NavigatingCancelEventArgs e)
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
