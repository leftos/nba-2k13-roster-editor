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
using System.Reflection;
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
        private void app_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var exceptionString = e.Exception.ToString();
            var innerExceptionString = e.Exception.InnerException == null
                                           ? "No inner exception information."
                                           : e.Exception.InnerException.Message;
            var versionString = "Version " + Assembly.GetExecutingAssembly().GetName().Version;

            try
            {
                var errorReportPath = NBA_2K13_Roster_Editor.MainWindow.DocsPath + @"\errorlog_unh.txt";
                var f = new StreamWriter(errorReportPath);

                f.WriteLine("Unhandled Exception Error Report for NBA 2K13 Roster Editor");
                f.WriteLine(versionString);
                f.WriteLine();
                f.WriteLine("Exception information:");
                f.Write(exceptionString);
                f.WriteLine();
                f.WriteLine();
                f.WriteLine("Inner Exception information:");
                f.Write(innerExceptionString);
                f.Close();

                MessageBox.Show(
                    "NBA 2K13 Roster Editor encountered a critical error and will be terminated.\n\n" + "An Error Log has been saved at \n" +
                    errorReportPath, "NBA 2K13 Roster Editor Error", MessageBoxButton.OK, MessageBoxImage.Error);

                Process.Start(errorReportPath);
            }
            catch (Exception ex)
            {
                string s = "Can't create errorlog!\nException: " + ex;
                s += ex.InnerException != null ? "\nInner Exception: " + ex.InnerException : "";
                s += "\n\n";
                s += versionString;
                s += "Exception Information:\n" + exceptionString + "\n\n";
                s += "Inner Exception Information:\n" + innerExceptionString;
                MessageBox.Show(s, "NBA 2K13 Roster Editor Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

            var twtl = new TextWriterTraceListener(NBA_2K13_Roster_Editor.MainWindow.DocsPath + @"\tracelog.txt")
                       {
                           Name = "TextLogger",
                           TraceOutputOptions =
                               TraceOptions.ThreadId |
                               TraceOptions.DateTime
                       };

            var ctl = new ConsoleTraceListener(false) {TraceOutputOptions = TraceOptions.DateTime};

            Trace.Listeners.Add(twtl);
            Trace.Listeners.Add(ctl);
            Trace.AutoFlush = true;

            app.Run();
        }
    }
}