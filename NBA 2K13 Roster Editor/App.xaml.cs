#region Copyright Notice

//    Copyright 2011-2013 Eleftherios Aslanoglou
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.

#endregion

#region Using Directives

using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Threading;

#endregion

namespace NBA_2K13_Roster_Editor
{
    /// <summary>
    ///     Interaction logic for App.xaml
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

            if (!Directory.Exists(NBA_2K13_Roster_Editor.MainWindow.DocsPath))
            {
                try
                {
                    Directory.CreateDirectory(NBA_2K13_Roster_Editor.MainWindow.DocsPath);
                }
                catch (Exception)
                {
                    Debug.WriteLine("DocsPath couldn't be created.");
                }
            }

            try
            {
                File.Delete(NBA_2K13_Roster_Editor.MainWindow.DocsPath + @"\tracelog.txt");
            }
            catch
            {
                Debug.WriteLine("Couldn't delete previous trace file, if any.");
            }

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