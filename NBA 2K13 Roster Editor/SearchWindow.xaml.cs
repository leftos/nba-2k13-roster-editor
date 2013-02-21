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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

#endregion

namespace NBA_2K13_Roster_Editor
{
    /// <summary>
    ///     Interaction logic for SearchWindow.xaml
    /// </summary>
    public partial class SearchWindow : Window
    {
        private readonly List<string> NumericOptions = new List<string> {"<", "<=", "=", ">=", ">", "Contains(Text)"};

        private readonly List<string> ReplaceOptions = new List<string>
                                                       {
                                                           "=",
                                                           "+=",
                                                           "-=",
                                                           "*=",
                                                           "/=",
                                                           "=Rand",
                                                           "+=Rand",
                                                           "-=Rand",
                                                           "*=Rand",
                                                           "/=Rand"
                                                       };

        private readonly string folder = MainWindow.DocsPath + @"\Search Filters";
        public List<string> FilterFilters = new List<string>();
        public List<string> FindFilters = new List<string>();
        public List<string> ReplaceFilters = new List<string>();

        public SearchWindow()
        {
            InitializeComponent();

            MainWindow.mw.dgPlayers.Columns.ToList().ForEach(delegate(DataGridColumn c)
                                                             {
                                                                 cmbReplacePar.Items.Add(c.Header);
                                                                 cmbFindPar.Items.Add(c.Header);
                                                             });

            cmbReplacePar.Items.Remove("ID");
            cmbReplacePar.Items.Remove("Name");

            NumericOptions.ForEach(no => cmbFindOp.Items.Add(no));
            ReplaceOptions.ForEach(ro => cmbReplaceOp.Items.Add(ro));

            cmbFindOp.SelectedIndex = 2;
            cmbReplaceOp.SelectedIndex = 0;
        }

        private void btnFindAdd_Click(object sender, RoutedEventArgs e)
        {
            if (cmbFindPar.SelectedIndex == -1 || cmbFindOp.SelectedIndex == -1 || String.IsNullOrWhiteSpace(txtFindVal.Text))
                return;

            /*
            try
            {
                Convert.ToSingle(txtFindVal.Text);
            }
            catch
            {
                return;
            }
            */

            lstFind.Items.Add(cmbFindPar.SelectedItem + " " + cmbFindOp.SelectedItem + " " + txtFindVal.Text);
            cmbFindPar.SelectedIndex = -1;
            txtFindVal.Text = "";
        }

        private void btnFindDel_Click(object sender, RoutedEventArgs e)
        {
            if (lstFind.SelectedIndex == -1)
                return;

            if (lstFind.SelectedItems.Count == 1)
            {
                string item = lstFind.SelectedItem.ToString();
                lstFind.Items.Remove(item);
                string[] parts = item.Split(' ');
                cmbFindPar.SelectedItem = parts[0];
                cmbFindOp.SelectedItem = parts[1];
                txtFindVal.Text = parts[2];
            }
            else
            {
                foreach (var item in new List<string>(lstFind.SelectedItems.Cast<string>()))
                {
                    lstFind.Items.Remove(item);
                }
            }
        }

        private void btnReplaceAdd_Click(object sender, RoutedEventArgs e)
        {
            if (cmbReplacePar.SelectedIndex == -1 || String.IsNullOrWhiteSpace(txtReplaceVal.Text))
                return;
            /*
            try
            {
                Convert.ToSingle(txtReplaceVal.Text);
            }
            catch
            {
                return;
            }
            */
            lstReplace.Items.Add(string.Format("{0} {1} {2}", cmbReplacePar.SelectedItem, cmbReplaceOp.SelectedItem, txtReplaceVal.Text));
            cmbReplacePar.SelectedIndex = -1;
            txtReplaceVal.Text = "";
        }

        private void btnReplaceDel_Click(object sender, RoutedEventArgs e)
        {
            if (lstReplace.SelectedIndex == -1)
                return;

            if (lstReplace.SelectedItems.Count == 1)
            {
                string item = lstReplace.SelectedItem.ToString();
                lstReplace.Items.Remove(item);
                string[] parts = item.Split(' ');
                cmbReplacePar.SelectedItem = parts[0];
                cmbReplaceOp.SelectedItem = parts[1];
                txtReplaceVal.Text = parts[2];
            }
            else
            {
                foreach (var item in new List<string>(lstReplace.SelectedItems.Cast<string>()))
                {
                    lstReplace.Items.Remove(item);
                }
            }
        }

        private void btnLoadFilters_Click(object sender, RoutedEventArgs e)
        {
            var sfd = new OpenFileDialog
                      {
                          InitialDirectory = Path.GetFullPath(folder),
                          Filter = "Roster Editor Search Filters (*.rsf)|*.rsf",
                          DefaultExt = "rsf"
                      };

            sfd.ShowDialog();

            if (String.IsNullOrWhiteSpace(sfd.FileName))
                return;

            int filterCount = lstFind.Items.Count + lstReplace.Items.Count;
            if (filterCount > 0)
            {
                MessageBoxResult mbr = MessageBox.Show("Do you want to clear the current stat filters before loading the new ones?",
                                                       "Roster Editor", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (mbr == MessageBoxResult.Cancel)
                    return;
                if (mbr == MessageBoxResult.Yes)
                {
                    lstFind.Items.Clear();
                    lstReplace.Items.Clear();
                }
            }

            string file = sfd.FileName;
            LoadFilters(file);
        }

        public void LoadFilters(string file)
        {
            string[] s = File.ReadAllLines(file);
            for (int i = 0; i < s.Length; i++)
            {
                string[] parts = s[i].Split('\t');
                switch (parts[0])
                {
                    case "Find":
                        while (true)
                        {
                            string line = s[++i];
                            if (line.StartsWith("FindEND"))
                                break;

                            lstFind.Items.Add(line);
                        }
                        break;

                    case "Replace":
                        while (true)
                        {
                            string line = s[++i];
                            if (line.StartsWith("ReplaceEND"))
                                break;

                            lstReplace.Items.Add(line);
                        }
                        break;
                }
            }
        }

        private void btnSaveFilters_Click(object sender, RoutedEventArgs e)
        {
            string s = "";
            s += String.Format("Find\n");
            s = lstFind.Items.Cast<string>().Aggregate(s, (current, item) => current + (item + "\n"));
            s += "FindEND\n";
            s += String.Format("Replace\n");
            s = lstReplace.Items.Cast<string>().Aggregate(s, (current, item) => current + (item + "\n"));
            s += "ReplaceEND\n";

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var sfd = new SaveFileDialog
                      {
                          InitialDirectory = Path.GetFullPath(folder),
                          Filter = "Roster Editor Search Filters (*.rsf)|*.rsf",
                          DefaultExt = "rsf"
                      };

            sfd.ShowDialog();

            if (String.IsNullOrWhiteSpace(sfd.FileName))
                return;

            File.WriteAllText(sfd.FileName, s);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void btnFind_Click(object sender, RoutedEventArgs e)
        {
            FindFilters.AddRange(lstFind.Items.Cast<string>());

            DialogResult = true;
            Close();
        }

        private void btnReplace_Click(object sender, RoutedEventArgs e)
        {
            AddFiltersToFields();

            DialogResult = true;
            Close();
        }

        public void AddFiltersToFields()
        {
            FindFilters.AddRange(lstFind.Items.Cast<string>());
            ReplaceFilters.AddRange(lstReplace.Items.Cast<string>());
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            FindFilters.AddRange(lstFind.Items.Cast<string>());
            FilterFilters.AddRange(lstFind.Items.Cast<string>());

            DialogResult = true;
            Close();
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindow.LastFindFilters.ForEach(f => lstFind.Items.Add(f));
            }
            catch (NullReferenceException)
            {
            }
            try
            {
                MainWindow.LastReplaceFilters.ForEach(f => lstReplace.Items.Add(f));
            }
            catch (NullReferenceException)
            {
            }
        }
    }
}