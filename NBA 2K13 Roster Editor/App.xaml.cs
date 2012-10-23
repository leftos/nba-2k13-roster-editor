using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace NBA_2K13_Roster_Editor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // Add code to output the exception details to a message box/event log/log file,   etc.
            // Be sure to include details about any inner exceptions
            try
            {
                var f = new StreamWriter(NBA_2K13_Roster_Editor.MainWindow.DocsPath + @"\errorlog_unh.txt");

                f.Write(e.Exception.ToString());
                f.WriteLine();
                f.WriteLine();
                f.Write(e.Exception.InnerException == null ? "None" : e.Exception.InnerException.Message);
                f.WriteLine();
                f.WriteLine();
                f.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can't create errorlog!\n\n" + ex + "\n\n" + ex.InnerException);
            }

            MessageBox.Show(
                "NBA 2K13 Roster Editor encountered a critical error and will be terminated.\n\nAn Error Log has been saved at " +
                NBA_2K13_Roster_Editor.MainWindow.DocsPath + @"\errorlog_unh.txt");
            try
            {
                Process.Start(NBA_2K13_Roster_Editor.MainWindow.DocsPath + @"\errorlog_unh.txt");
            }
            catch (Exception)
            {
            }

            // Prevent default unhandled exception processing
            e.Handled = true;

            Environment.Exit(-1);
        }

        [STAThread]
        private static void Main()
        {
            var app = new App();
            app.InitializeComponent();

            Trace.Listeners.Clear();

            var twtl = new TextWriterTraceListener(NBA_2K13_Roster_Editor.MainWindow.DocsPath + @"\tracelog.txt");
            twtl.Name = "TextLogger";
            twtl.TraceOutputOptions = TraceOptions.ThreadId | TraceOptions.DateTime;

            var ctl = new ConsoleTraceListener(false);
            ctl.TraceOutputOptions = TraceOptions.DateTime;

            Trace.Listeners.Add(twtl);
            Trace.Listeners.Add(ctl);
            Trace.AutoFlush = true;

            app.Run();
        }
    }
}