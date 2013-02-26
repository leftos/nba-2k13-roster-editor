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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using LeftosCommonLibrary;
using Microsoft.Win32;
using NBA_2K13_Roster_Editor.Data;
using NBA_2K13_Roster_Editor.Data.Jerseys;
using NBA_2K13_Roster_Editor.Data.Playbooks;
using NBA_2K13_Roster_Editor.Data.Players;
using NBA_2K13_Roster_Editor.Data.Players.Parameters;
using NBA_2K13_Roster_Editor.Data.PlayerStats;
using NBA_2K13_Roster_Editor.Data.Staff;
using NBA_2K13_Roster_Editor.Data.Staff.Parameters;
using NBA_2K13_Roster_Editor.Data.Teams;
using NBA_2K13_Roster_Editor.Data.TeamStats;
using NonByteAlignedBinaryRW;
using WPFColorPickerLib;

#endregion

namespace NBA_2K13_Roster_Editor
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string NBA2K13SavesPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                                @"\2K Sports\NBA 2K13\Saves";

        public static string DocsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\NBA 2K13 Roster Editor";
        public static string AppPath = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
        public static MainWindow mw;
        public static Mode mode;
        public static SaveType saveType;
        public static List<string> LastFindFilters, LastReplaceFilters;

        private readonly List<string> colorColumns = new List<string>
                                                     {
                                                         "ShHomeTeam1",
                                                         "ShHomeTeam2",
                                                         "ShHomeBase",
                                                         "ShAwayTeam1",
                                                         "ShAwayTeam2",
                                                         "ShAwayBase",
                                                         "TeamColor1",
                                                         "TeamColor2",
                                                         "TeamColor3",
                                                         "TeamColor4",
                                                         "TeamColor5",
                                                         "TeamColor6"
                                                     };

        private readonly List<string> gNames;
        private readonly List<string> hsNames;
        //private List<int> foundIDs = new List<int>();
        private readonly List<string> pColsToAvoid;
        private readonly Random random;
        private readonly List<string> rtNames;
        private readonly List<string> tColsToAvoid;
        private readonly List<string> tNames;
        private readonly List<string> teamColumns = new List<string> {"TeamID1", "TeamID2", "AssignedTo*"};
        private readonly DispatcherTimer timer;

        //private RosterReader brOpen;
        private int curFoundID = -1;
        private string currentFile;
        private bool doCRC;
        private bool editing;
        private int expCount;
        private List<int> foundIDList = new List<int>();
        private Dictionary<string, string> names;

        #region Team Names

        private readonly Dictionary<int, string> teamNames = new Dictionary<int, string>
                                                             {
                                                                 {0, "76ers"},
                                                                 {1, "Bobcats"},
                                                                 {2, "Bucks"},
                                                                 {3, "Bulls"},
                                                                 {4, "Cavaliers"},
                                                                 {5, "Celtics"},
                                                                 {6, "Clippers"},
                                                                 {7, "Grizzlies"},
                                                                 {8, "Hawks"},
                                                                 {9, "Heat"},
                                                                 {10, "Hornets"},
                                                                 {11, "Jazz"},
                                                                 {12, "Kings"},
                                                                 {13, "Knicks"},
                                                                 {14, "Lakers"},
                                                                 {15, "Magic"},
                                                                 {16, "Mavericks"},
                                                                 {17, "Nets"},
                                                                 {18, "Nuggets"},
                                                                 {19, "Pacers"},
                                                                 {20, "Pistons"},
                                                                 {21, "Raptors"},
                                                                 {22, "Rockets"},
                                                                 {23, "Spurs"},
                                                                 {24, "Suns"},
                                                                 {25, "Thunder"},
                                                                 {26, "Timberwolves"},
                                                                 {27, "Trail Blazers"},
                                                                 {28, "Warriors"},
                                                                 {29, "Wizards"},
                                                                 {30, "East All-Stars"},
                                                                 {31, "West All-Stars"},
                                                                 {32, "Rookies"},
                                                                 {33, "Sophmores"},
                                                                 {34, "2k Team"},
                                                                 {35, "Tutorial Trainees"},
                                                                 {36, "Under Armour Trainers"},
                                                                 {37, "Celeberty Team"},
                                                                 {38, "Dream Team"},
                                                                 {39, "Team USA"},
                                                                 {40, "'65 Celtics"},
                                                                 {41, "'65 Lakers"},
                                                                 {42, "'71 Bucks"},
                                                                 {43, "'71 Lakers"},
                                                                 {44, "'71 Hawks"},
                                                                 {45, "'72 Lakers"},
                                                                 {46, "'72 Knicks"},
                                                                 {47, "'77 76ers"},
                                                                 {48, "'85 76ers"},
                                                                 {49, "'85 Bucks"},
                                                                 {50, "'86 Bulls"},
                                                                 {51, "'86 Celtics"},
                                                                 {52, "'86 Hawks"},
                                                                 {53, "'87 Lakers"},
                                                                 {54, "'89 Pistons"},
                                                                 {55, "'89 Bulls"},
                                                                 {56, "'90 Cavaliers"},
                                                                 {57, "'91 Bulls"},
                                                                 {58, "'91 Lakers"},
                                                                 {59, "'91 Trail Blazers"},
                                                                 {60, "'91 Warriors"},
                                                                 {61, "'93 Bulls"},
                                                                 {62, "'93 Hornets"},
                                                                 {63, "'94 Rockets"},
                                                                 {64, "'94 Nuggets"},
                                                                 {65, "'95 Knicks"},
                                                                 {66, "'95 Magic"},
                                                                 {67, "'96 Bulls"},
                                                                 {68, "'96 SuperSonics"},
                                                                 {69, "'98 Bulls"},
                                                                 {70, "'98 Jazz"},
                                                                 {71, "'98 Lakers"},
                                                                 {72, "'99 Spurs"},
                                                                 {73, "'01 76ers"},
                                                                 {74, "'02 Kings"},
                                                                 {75, "Home"},
                                                                 {76, "Away"},
                                                                 {77, "Black"},
                                                                 {78, "White"},
                                                                 {79, "Elites"},
                                                                 {80, "Stars"},
                                                                 {81, "Custom 1"},
                                                                 {82, "Custom 2"},
                                                                 {83, "Custom 3"},
                                                                 {84, "Custom 4"},
                                                                 {85, "Custom 5"},
                                                                 {86, "Custom 6"},
                                                                 {87, "Custom 7"},
                                                                 {88, "Custom 8"},
                                                                 {89, "Custom 9"},
                                                                 {90, "Custom 10"},
                                                                 {91, "Unknown Slot 1"},
                                                                 {92, "Unknown Slot 2"}
                                                             };

        #endregion

        public MainWindow()
        {
            InitializeComponent();

            random = new Random();

            btnMode360Nov10.Visibility = Visibility.Collapsed; //

            mw = this;

            Title += " v" + Assembly.GetExecutingAssembly().GetName().Version;

            if (Directory.Exists(DocsPath) == false)
            {
                Directory.CreateDirectory(DocsPath);
            }

            if (Directory.Exists(DocsPath + @"\Search Filters") == false)
            {
                Directory.CreateDirectory(DocsPath + @"\Search Filters");
            }

            ReloadOptions();
            dgOptions.ItemsSource = optionsList;

            doCRC = (GetRegistrySetting("CRC", 1) == 1);
            chkRecalculateCRC.IsChecked = doCRC;

            try
            {
                mode = (Mode) Enum.Parse(typeof (Mode), GetRegistrySetting("Mode", "PC"));
            }
            catch (ArgumentException)
            {
                mode = Mode.PC;
            }
            /*
            if (mode == Mode.PC)
                btnModePC.IsChecked = true;
            else if (mode == Mode.X360)
                btnMode360.IsChecked = true;
            else if (mode == Mode.PCNov10)
                btnModePCNov.IsChecked = true;
            else if (mode == Mode.X360Nov10)
                btnMode360Nov10.IsChecked = true;
            else if (mode == Mode.Custom)
                btnModeCustom.IsChecked = true;
            */
            if (mode.ToString().Contains("360"))
            {
                btnModeCustom360.IsChecked = true;
            }
            else
            {
                btnModeCustom.IsChecked = true;
            }

            PreparePlayersDataGrid();

            PrepareTeamsDataGrid();

            PreparePlaybooksDataGrid();

            PrepareStaffDataGrid();

            //
            pColsToAvoid = new List<string>();
            rtNames = new List<string>();
            tNames = new List<string>();
            hsNames = new List<string>();
            gNames = new List<string>();
            Enum.GetNames(typeof (Rating)).ToList().ForEach(name => rtNames.Add("R" + name));
            pColsToAvoid.AddRange(rtNames);
            Enum.GetNames(typeof (Tendency)).ToList().ForEach(name => tNames.Add("T" + name));
            pColsToAvoid.AddRange(tNames);
            Enum.GetNames(typeof (HotSpot)).ToList().ForEach(name => hsNames.Add("HS" + name));
            pColsToAvoid.AddRange(hsNames);
            Enum.GetNames(typeof (Gear)).ToList().ForEach(name => gNames.Add("G" + name));
            pColsToAvoid.AddRange(gNames);
            pColsToAvoid.AddRange(new[] {"SS1", "SS2", "SS3", "SS4", "SS5"});
            for (int i = 0; i < 15; i++)
            {
                pColsToAvoid.Add("DunkPkg" + i);
            }
            for (int i = 0; i < 22; i++)
            {
                pColsToAvoid.Add("SeasonStats" + i);
            }

            tColsToAvoid = new List<string>();
            for (int i = 1; i <= 512; i++)
            {
                tColsToAvoid.Add("R" + i);
            }
            //

            jerseysList = new ObservableCollection<JerseyEntry>();
            playersList = new ObservableCollection<PlayerEntry>();
            teamsList = new ObservableCollection<TeamEntry>();
            staffList = new ObservableCollection<StaffEntry>();
            playbooksList = new ObservableCollection<PlaybookEntry>();
            teamStatsList = new ObservableCollection<TeamStatsEntry>();

            if (!File.Exists(DocsPath + @"\CFNames.txt"))
            {
                File.Copy(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\CFNames.txt", DocsPath + @"\CFNames.txt");
            }

            timer = new DispatcherTimer();
            timer.Tick += timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 3);

            Height = GetRegistrySetting("Height", (int) Height);
            Width = GetRegistrySetting("Width", (int) Width);
            Left = GetRegistrySetting("Left", 0);
            Top = GetRegistrySetting("Top", 0);
        }

        private ObservableCollection<PlayerEntry> playersList { get; set; }
        private static ObservableCollection<Option> optionsList { get; set; }
        private ObservableCollection<TeamEntry> teamsList { get; set; }
        private ObservableCollection<JerseyEntry> jerseysList { get; set; }
        private ObservableCollection<PlaybookEntry> playbooksList { get; set; }
        private ObservableCollection<StaffEntry> staffList { get; set; }
        internal ObservableCollection<PlayerEntry> filteredPlayersList { get; set; }
        protected ObservableCollection<PlayerStatsEntry> playerStatsList { get; set; }

        protected ObservableCollection<TeamStatsEntry> teamStatsList { get; set; }

        private void PreparePlaybooksDataGrid()
        {
            dgPlaybooks.Columns.Clear();
            dgPlaybooks.Columns.Add(new DataGridTextColumn
                                    {
                                        Header = "ID",
                                        Binding = new Binding {Path = new PropertyPath("ID"), Mode = BindingMode.TwoWay}
                                    });
            for (int i = 0; i < 50; i++)
            {
                dgPlaybooks.Columns.Add(new DataGridTextColumn
                                        {
                                            Header = string.Format("Play{0}", (i + 1)),
                                            Binding =
                                                new Binding
                                                {
                                                    Path = new PropertyPath(string.Format("Plays[{0}]", i)),
                                                    Mode = BindingMode.TwoWay
                                                }
                                        });
            }
        }

        private void PrepareTeamsDataGrid()
        {
            dgTeams.Columns.Clear();
            dgTeams.Columns.Add(new DataGridTextColumn
                                {
                                    Header = "ID",
                                    Binding = new Binding {Path = new PropertyPath("ID"), Mode = BindingMode.TwoWay}
                                });
            dgTeams.Columns.Add(new DataGridTextColumn
                                {
                                    Header = "Name",
                                    Binding = new Binding {Path = new PropertyPath("Name"), Mode = BindingMode.TwoWay}
                                });
            dgTeams.Columns.Add(new DataGridTextColumn
                                {
                                    Header = "StHeadCoach",
                                    Binding =
                                        new Binding {Path = new PropertyPath("StHeadCoach"), Mode = BindingMode.TwoWay}
                                });
            dgTeams.Columns.Add(new DataGridTextColumn
                                {
                                    Header = "PlaybookID*",
                                    Binding =
                                        new Binding {Path = new PropertyPath("PlaybookID"), Mode = BindingMode.TwoWay},
                                    IsReadOnly = true
                                });
            dgTeams.Columns.Add(new DataGridTextColumn
                                {
                                    Header = "StAsstCoach",
                                    Binding =
                                        new Binding {Path = new PropertyPath("StAsstCoach"), Mode = BindingMode.TwoWay}
                                });
            dgTeams.Columns.Add(new DataGridTextColumn
                                {
                                    Header = "CurSeaSta",
                                    Binding =
                                        new Binding {Path = new PropertyPath("CurSeaSta"), Mode = BindingMode.TwoWay}
                                });
            dgTeams.Columns.Add(new DataGridTextColumn
                                {
                                    Header = "CurPlaSta",
                                    Binding =
                                        new Binding {Path = new PropertyPath("CurPlaSta"), Mode = BindingMode.TwoWay}
                                });
            dgTeams.Columns.Add(new DataGridTextColumn
                                {
                                    Header = "PrvSeaSta",
                                    Binding =
                                        new Binding {Path = new PropertyPath("PrvSeaSta"), Mode = BindingMode.TwoWay}
                                });
            dgTeams.Columns.Add(new DataGridTextColumn
                                {
                                    Header = "PrvPlaSta",
                                    Binding =
                                        new Binding {Path = new PropertyPath("PrvPlaSta"), Mode = BindingMode.TwoWay}
                                });
            dgTeams.Columns.Add(new DataGridTextColumn
                                {
                                    Header = "PlNum",
                                    Binding = new Binding {Path = new PropertyPath("PlNum"), Mode = BindingMode.TwoWay}
                                });
            for (int i = 0; i < 512; i++)
            {
                dgTeams.Columns.Add(new DataGridTextColumn
                                    {
                                        Header = string.Format("R{0}", (i + 1)),
                                        Binding =
                                            new Binding
                                            {
                                                Path = new PropertyPath(string.Format("RosterSpots[{0}]", i)),
                                                Mode = BindingMode.TwoWay
                                            }
                                    });
            }

            teamsList = new ObservableCollection<TeamEntry>();
            dgTeams.ItemsSource = teamsList;
        }

        private void PrepareStaffDataGrid()
        {
            for (int i = 0; i < 21; i++)
            {
                dgStaff.Columns.Add(new DataGridTextColumn
                                    {
                                        Header = string.Format("CP{0}", Enum.GetName(typeof (CPSetting), i)),
                                        Binding =
                                            new Binding
                                            {
                                                Path =
                                                    new PropertyPath(string.Format("CoachingProfile[{0}]", i)),
                                                Mode = BindingMode.TwoWay
                                            }
                                    });
            }
        }

        private void PreparePlayersDataGrid()
        {
            for (int i = 0; i < 37; i++)
            {
                dgPlayers.Columns.Add(new DataGridTextColumn
                                      {
                                          Header = string.Format("R{0}", Enum.GetName(typeof (Rating), i)),
                                          Binding =
                                              new Binding
                                              {
                                                  Path = new PropertyPath(string.Format("Ratings[{0}]", i)),
                                                  Mode = BindingMode.TwoWay
                                              }
                                      });
            }

            for (int i = 0; i < 69; i++)
            {
                dgPlayers.Columns.Add(new DataGridTextColumn
                                      {
                                          Header = string.Format("T{0}", Enum.GetName(typeof (Tendency), i)),
                                          Binding =
                                              new Binding
                                              {
                                                  Path = new PropertyPath(string.Format("Tendencies[{0}]", i)),
                                                  Mode = BindingMode.TwoWay
                                              }
                                      });
            }

            for (int i = 0; i < 25; i++)
            {
                dgPlayers.Columns.Add(new DataGridTextColumn
                                      {
                                          Header = string.Format("HS{0}", Enum.GetName(typeof (HotSpot), i)),
                                          Binding =
                                              new Binding
                                              {
                                                  Path = new PropertyPath(string.Format("HotSpots[{0}]", i)),
                                                  Mode = BindingMode.TwoWay
                                              }
                                      });
            }
            /*
            for (int i = 0; i < 25; i++)
            {
                dgPlayers.Columns.Add(new DataGridTextColumn
                {
                    Header = string.Format("HZ{0}", Enum.GetName(typeof(HotZoneName), i)),
                    Binding =
                        new Binding { Path = new PropertyPath(string.Format("HotZones[{0}]", i)), Mode = BindingMode.TwoWay }
                });
            }
            */
            dgPlayers.Columns.Add(new DataGridTextColumn
                                  {
                                      Header = "ContYBefOpt",
                                      Binding =
                                          new Binding
                                          {
                                              Path = new PropertyPath("ContYBefOpt"),
                                              Mode = BindingMode.TwoWay
                                          }
                                  });

            dgPlayers.Columns.Add(new DataGridComboBoxColumn
                                  {
                                      Header = "ContractOpt",
                                      SelectedValueBinding =
                                          new Binding
                                          {
                                              Path = new PropertyPath("ContractOpt"),
                                              Mode = BindingMode.TwoWay
                                          },
                                      ItemsSource = Enum.GetValues(typeof (ContractOption))
                                  });

            dgPlayers.Columns.Add(new DataGridCheckBoxColumn
                                  {
                                      Header = "ContNoTrade",
                                      Binding =
                                          new Binding
                                          {
                                              Path = new PropertyPath("ContNoTrade"),
                                              Mode = BindingMode.TwoWay
                                          }
                                  });

            for (int i = 0; i < 7; i++)
            {
                dgPlayers.Columns.Add(new DataGridTextColumn
                                      {
                                          Header = string.Format("ContractY{0}", i + 1),
                                          Binding =
                                              new Binding
                                              {
                                                  Path = new PropertyPath(string.Format("ContractY{0}", i + 1)),
                                                  Mode = BindingMode.TwoWay
                                              }
                                      });
            }

            dgPlayers.Columns.Add(new DataGridTextColumn
                                  {
                                      Header = string.Format("SigFT"),
                                      Binding =
                                          new Binding
                                          {
                                              Path = new PropertyPath(string.Format("SigFT")),
                                              Mode = BindingMode.TwoWay
                                          }
                                  });
            dgPlayers.Columns.Add(new DataGridTextColumn
                                  {
                                      Header = string.Format("SigShtForm"),
                                      Binding =
                                          new Binding
                                          {
                                              Path = new PropertyPath(string.Format("SigShtForm")),
                                              Mode = BindingMode.TwoWay
                                          }
                                  });
            dgPlayers.Columns.Add(new DataGridTextColumn
                                  {
                                      Header = string.Format("SigShtBase"),
                                      Binding =
                                          new Binding
                                          {
                                              Path = new PropertyPath(string.Format("SigShtBase")),
                                              Mode = BindingMode.TwoWay
                                          }
                                  });

            dgPlayers.Columns.Add(new DataGridTextColumn
                                  {
                                      Header = string.Format("LayupPkg"),
                                      Binding =
                                          new Binding
                                          {
                                              Path = new PropertyPath(string.Format("LayupPkg")),
                                              Mode = BindingMode.TwoWay
                                          }
                                  });

            for (int i = 0; i < 15; i++)
            {
                dgPlayers.Columns.Add(new DataGridTextColumn
                                      {
                                          Header = string.Format("DunkPkg{0}", i),
                                          Binding =
                                              new Binding
                                              {
                                                  Path =
                                                      new PropertyPath(string.Format("DunkPackages[{0}]", i)),
                                                  Mode = BindingMode.TwoWay
                                              }
                                      });
            }

            var gearOrder = new Dictionary<int, int>
                            {
                                {0, 0},
                                {1, 30},
                                {2, 35},
                                {3, 20},
                                {4, 1},
                                {5, 18},
                                {6, 3},
                                {7, 22},
                                {8, 23},
                                {9, 7},
                                {10, 26},
                                {11, 36},
                                {12, 21},
                                {13, 2},
                                {14, 19},
                                {15, 6},
                                {16, 24},
                                {17, 25},
                                {18, 8},
                                {20, 4},
                                {19, 27},
                                {21, 16},
                                {22, 5},
                                {23, 17},
                                {24, 11},
                                {25, 31},
                                {26, 9},
                                {27, 28},
                                {28, 13},
                                {29, 33},
                                {30, 12},
                                {31, 32},
                                {32, 10},
                                {33, 29},
                                {34, 15},
                                {35, 34},
                                {36, 14},
                                {37, 37},
                                {38, 38},
                                {39, 39},
                                {40, 40}
                            };

            for (int i = 0; i < 41; i++)
            {
                dgPlayers.Columns.Add(new DataGridTextColumn
                                      {
                                          Header = string.Format("G{0}", Enum.GetName(typeof (Gear), gearOrder[i])),
                                          Binding =
                                              new Binding
                                              {
                                                  Path =
                                                      new PropertyPath(string.Format("Accessories[{0}]",
                                                                                     gearOrder[i])),
                                                  Mode = BindingMode.TwoWay
                                              }
                                      });
            }

            for (int i = 0; i < 22; i++)
            {
                dgPlayers.Columns.Add(new DataGridTextColumn
                                      {
                                          Header = string.Format("SeasonStats{0}", i),
                                          Binding =
                                              new Binding
                                              {
                                                  Path = new PropertyPath(string.Format("SeasonStats[{0}]", i)),
                                                  Mode = BindingMode.TwoWay
                                              }
                                      });
            }

            dgPlayers.Columns.Add(new DataGridTextColumn
                                  {
                                      Header = string.Format("PlayoffStats"),
                                      Binding =
                                          new Binding
                                          {
                                              Path = new PropertyPath(string.Format("PlayoffStats")),
                                              Mode = BindingMode.TwoWay
                                          }
                                  });

#if DEBUG
            dgPlayers.Columns.Add(new DataGridTextColumn
                                  {
                                      Header = string.Format("Offset"),
                                      Binding =
                                          new Binding
                                          {
                                              Path = new PropertyPath(string.Format("Offset")),
                                              Mode = BindingMode.TwoWay
                                          }
                                  });
#endif

            playersList = new ObservableCollection<PlayerEntry>();
            dgPlayers.ItemsSource = playersList;
        }

        private void PopulateNamesDictionary()
        {
            names = new Dictionary<string, string>();
            string file = DocsPath + "\\" + GetOption("NamesFile");
            if (File.Exists(file))
            {
                string[] lines = File.ReadAllLines(file);
                for (int index = 0; index < lines.Length; index++)
                {
                    string line = lines[index];
                    string[] parts = line.Split(new[] {'\t'}, 2);
                    if (parts.Length == 2)
                    {
                        try
                        {
                            string id = parts[0];
                            if (names.ContainsKey(id) == false)
                            {
                                names.Add(id, parts[1]);
                            }
                        }
                        catch (Exception)
                        {
                            Trace.WriteLine(string.Format("{0}: Couldn't convert line {1}'s ID while parsing {2}, value is '{3}'",
                                                          DateTime.Now.ToString(), (index + 1), Path.GetFileName(file), parts[0]));
                            continue;
                        }
                    }
                }
            }
        }

        private void ReloadOptions()
        {
            optionsList = new ObservableCollection<Option>
                          {
                              new Option
                              {
                                  Setting = "FirstSSOffset",
                                  Value = GetRegistrySetting("FirstSSOffset", 40916)
                              },
                              new Option
                              {
                                  Setting = "FirstSSOffsetBit",
                                  Value = GetRegistrySetting("FirstSSOffsetBIt", 2)
                              },
                              new Option
                              {
                                  Setting = "LastPlayerID",
                                  Value = GetRegistrySetting("LastPlayerID", 1514)
                              },
                              new Option
                              {
                                  Setting = "LastTeamID",
                                  Value = GetRegistrySetting("LastTeamID", 90)
                              },
                              new Option
                              {
                                  Setting = "LastJerseyID",
                                  Value = GetRegistrySetting("LastJerseyID", 425)
                              },
                              new Option
                              {
                                  Setting = "LastStaffID",
                                  Value = GetRegistrySetting("LastStaffID", 725)
                              },
                              new Option
                              {
                                  Setting = "LastPlaybookID",
                                  Value = GetRegistrySetting("LastPlaybookID", 69)
                              },
                              new Option
                              {
                                  Setting = "NamesFile",
                                  Value = GetRegistrySetting("NamesFile", "CFnames.txt")
                              },
                              new Option
                              {
                                  Setting = "ChooseNameBy",
                                  Value = GetRegistrySetting("ChooseNameBy", "CFID")
                              },
                              new Option
                              {
                                  Setting = "DoPlayersPasteBy",
                                  Value = GetRegistrySetting("DoPlayersPasteBy", "ID")
                              },
                              new Option
                              {
                                  Setting = "CustomSSOffset",
                                  Value = GetRegistrySetting("CustomSSOffset", 40916)
                              },
                              new Option
                              {
                                  Setting = "CustomSSOffsetBit",
                                  Value = GetRegistrySetting("CustomSSOffsetBIt", 2)
                              },
                              new Option
                              {
                                  Setting = "CustomRosterOffset",
                                  Value = GetRegistrySetting("CustomRosterOffset", 862911)
                              },
                              new Option
                              {
                                  Setting = "CustomRosterOffsetBit",
                                  Value = GetRegistrySetting("CustomRosterOffsetBIt", 6)
                              },
                              new Option
                              {
                                  Setting = "CustomJerseyOffset",
                                  Value = GetRegistrySetting("CustomJerseyOffset", 1486997)
                              },
                              new Option
                              {
                                  Setting = "CustomJerseyOffsetBit",
                                  Value = GetRegistrySetting("CustomJerseyOffsetBIt", 0)
                              },
                              new Option
                              {
                                  Setting = "CustomPlaybookOffset",
                                  Value = GetRegistrySetting("CustomPlaybookOffset", 1099333)
                              },
                              new Option
                              {
                                  Setting = "CustomPlaybookOffsetBit",
                                  Value = GetRegistrySetting("CustomPlaybookOffsetBit", 3)
                              },
                              new Option
                              {
                                  Setting = "CustomStaffPlaybookIDOffset",
                                  Value = GetRegistrySetting("CustomStaffPlaybookIDOffset", 991131)
                              },
                              new Option
                              {
                                  Setting = "CustomStaffPlaybookIDOffsetBit",
                                  Value = GetRegistrySetting("CustomStaffPlaybookIDOffsetBit", 6)
                              },
                              new Option
                              {
                                  Setting = "CustomTeamStatsOffset",
                                  Value = GetRegistrySetting("CustomTeamStatsOffset", 1434425)
                              },
                              new Option
                              {
                                  Setting = "CustomTeamStatsOffsetBit",
                                  Value = GetRegistrySetting("CustomTeamStatsOffsetBit", 5)
                              },
                              new Option
                              {
                                  Setting = "CustomPlayerStatsOffset",
                                  Value = GetRegistrySetting("CustomPlayerStatsOffset", 1475604)
                              },
                              new Option
                              {
                                  Setting = "CustomPlayerStatsOffsetBit",
                                  Value = GetRegistrySetting("CustomPlayerStatsOffsetBit", 0)
                              },
                              new Option
                              {
                                  Setting = "DumbPasting",
                                  Value = GetRegistrySetting("DumbPasting", "False")
                              }
                          };

#if DEBUG
#endif

            if (GetOption("DumbPasting").ToString() == "True")
            {
                dgPlayers.ClipboardCopyMode = DataGridClipboardCopyMode.ExcludeHeader;
                dgTeams.ClipboardCopyMode = DataGridClipboardCopyMode.ExcludeHeader;
                dgJerseys.ClipboardCopyMode = DataGridClipboardCopyMode.ExcludeHeader;
                dgPlaybooks.ClipboardCopyMode = DataGridClipboardCopyMode.ExcludeHeader;
                dgStaff.ClipboardCopyMode = DataGridClipboardCopyMode.ExcludeHeader;
                dgTeamStats.ClipboardCopyMode = DataGridClipboardCopyMode.ExcludeHeader;
            }
            else
            {
                dgPlayers.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
                dgTeams.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
                dgJerseys.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
                dgPlaybooks.ClipboardCopyMode = DataGridClipboardCopyMode.ExcludeHeader;
                dgStaff.ClipboardCopyMode = DataGridClipboardCopyMode.ExcludeHeader;
                dgTeamStats.ClipboardCopyMode = DataGridClipboardCopyMode.ExcludeHeader;
            }

            optionsList.Add(new Option {Setting = "CheckForUpdates", Value = GetRegistrySetting("CheckForUpdates", "True")});

            PopulateNamesDictionary();
            dgOptions.ItemsSource = optionsList;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            txbStatus.FontWeight = FontWeights.Normal;
            txbStatus.Text = "Ready";
            timer.Stop();
        }

        private void updateStatus(string text)
        {
            timer.Stop();
            txbStatus.FontWeight = FontWeights.Bold;
            txbStatus.Text = text;
            timer.Start();
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
            string lastPath = GetRegistrySetting("LastOpenPath", NBA2K13SavesPath);
            ofd.InitialDirectory = Directory.Exists(lastPath) ? lastPath : null;
            ofd.Filter = "All compatible NBA 2K13 files (*.ROS; *.FXG; *.CMG)|*.ROS;*.FXG;*.CMG|" + "Roster files (*.ROS)|*.ROS|" +
                         "Association files (*.FXG)|*.FXG|" + "MyCareer files (*.CMG)|*.CMG|" + "All files (*.*)|*.*";
            ofd.DefaultExt = ".ROS";
            ofd.ShowDialog();

            if (ofd.FileName == "")
                return;

            SetRegistrySetting("LastOpenPath", Path.GetDirectoryName(ofd.FileName));

            txtFile.Text = Path.GetFileName(ofd.FileName);
            currentFile = ofd.FileName;

            string ext = Path.GetExtension(currentFile);
            switch (ext)
            {
                case ".CMG":
                    saveType = SaveType.MyCareer;
                    break;
                case ".FXG":
                    saveType = SaveType.Association;
                    break;
                    /*
                case ".FDC":
                    saveType = SaveType.DraftClass;
                    break;
                     */
                default:
                    saveType = SaveType.Roster;
                    break;
            }

            tabTeamStats.IsEnabled = saveType != SaveType.MyCareer;

            if (mode == Mode.Custom || mode == Mode.CustomX360)
            {
                FindAllOffsets();
            }
            else
            {
                ReloadEverything();
            }

            updateStatus("Roster loaded.");
        }

        private void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            var dg = (DataGrid) sender;
            if (!editing && e.EditAction == DataGridEditAction.Commit)
            {
                editing = true;
                dg.CommitEdit(DataGridEditingUnit.Row, true);
                editing = false;
            }
            IList<DataGridCellInfo> selCells = dg.SelectedCells;
            dg.SelectedCells.Clear();
            selCells.ToList().ForEach(c => dg.SelectedCells.Add(c));
        }

        private void PopulateJerseysTab()
        {
            jerseysList = new ObservableCollection<JerseyEntry>();

            RosterReader brOpen;
            using (brOpen = new RosterReader(new MemoryStream(File.ReadAllBytes(currentFile))))
            {
                for (int i = 0; i <= 2000; i++)
                {
                    brOpen.MoveStreamToFirstJersey();
                    brOpen.MoveStreamPosition(68*i, 0);

                    var je = new JerseyEntry {ID = i};
                    ushort gid = BitConverter.ToUInt16(brOpen.ReadNonByteAlignedBytes(2).Reverse().ToArray(), 0);
                    if (!Enum.IsDefined(typeof (JerseyType), gid))
                    {
                        /*
                        Console.WriteLine("Unknown GID: " + gid);
                        je.GID = JerseyType.Unknown;
                        */
                        optionsList.Single(o => o.Setting == "LastJerseyID").Value = i - 1;
                        break;
                    }
                    else
                    {
                        je.GID = (JerseyType) Enum.Parse(typeof (JerseyType), gid.ToString());
                    }

                    brOpen.MoveStreamPosition(0, 6);
                    je.Neck = (NeckType) Enum.Parse(typeof (NeckType), Convert.ToByte(brOpen.ReadNonByteAlignedBits(3), 2).ToString());
                    brOpen.MoveStreamPosition(0, 2);
                    je.SockClr = (SockColor) Enum.Parse(typeof (SockColor), brOpen.ReadNonByteAlignedBits(1));
                    brOpen.MoveStreamPosition(0, 4);

                    brOpen.MoveStreamPosition(10, 0);
                    string hexName = Tools.ByteArrayToHexString(brOpen.ReadNonByteAlignedBytes(4));
                    try
                    {
                        je.Name = (JerseyName) Enum.Parse(typeof (JerseyName), JerseyEntry.JerseyNames.Single(n => n.Value == hexName).Key);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine(hexName);
                    }
                    brOpen.MoveStreamPosition(3, 5);
                    byte[] ba = brOpen.ReadNonByteAlignedBytes(4);
                    je.TeamColor1 = ba.Aggregate("#", (current, b) => current + Convert.ToString(b, 16).PadLeft(2, '0')).ToUpperInvariant();
                    ba = brOpen.ReadNonByteAlignedBytes(4);
                    je.TeamColor2 = ba.Aggregate("#", (current, b) => current + Convert.ToString(b, 16).PadLeft(2, '0')).ToUpperInvariant();
                    ba = brOpen.ReadNonByteAlignedBytes(4);
                    je.TeamColor3 = ba.Aggregate("#", (current, b) => current + Convert.ToString(b, 16).PadLeft(2, '0')).ToUpperInvariant();
                    ba = brOpen.ReadNonByteAlignedBytes(4);
                    je.TeamColor4 = ba.Aggregate("#", (current, b) => current + Convert.ToString(b, 16).PadLeft(2, '0')).ToUpperInvariant();
                    ba = brOpen.ReadNonByteAlignedBytes(4);
                    je.TeamColor5 = ba.Aggregate("#", (current, b) => current + Convert.ToString(b, 16).PadLeft(2, '0')).ToUpperInvariant();
                    ba = brOpen.ReadNonByteAlignedBytes(4);
                    je.TeamColor6 = ba.Aggregate("#", (current, b) => current + Convert.ToString(b, 16).PadLeft(2, '0')).ToUpperInvariant();
                    jerseysList.Add(je);
                }
            }

            dgJerseys.ItemsSource = null;
            dgJerseys.ItemsSource = jerseysList;
        }


        public static object GetOption(string setting)
        {
            return optionsList.First(o => o.Setting == setting).Value;
        }

        private void PopulatePlayersTab()
        {
            playersList = new ObservableCollection<PlayerEntry>();
            RosterReader brOpen;
            using (brOpen = new RosterReader(new MemoryStream(File.ReadAllBytes(currentFile))))
            {
                for (int i = 0; i <= Convert.ToInt32(GetOption("LastPlayerID")); i++)
                {
                    var pe = new PlayerEntry();
                    brOpen.MoveStreamToFirstSS(i);
                    pe.ID = i;
                    ReadPlayerInfo(ref pe, brOpen);
                    pe.Name = FindPlayerName(pe);
                    pe.SSList = ReadSignatureSkills(i, brOpen);
                    pe.Ratings = ReadRatings(i, brOpen);
                    pe.Tendencies = ReadTendencies(i, brOpen);
                    pe.HotSpots = ReadHotSpots(i, brOpen);
                    pe.HotZones = ReadHotZones(i, brOpen);
                    pe.Accessories = ReadGear(i, brOpen);
                    pe.AssignedTo = FindPlayerInTeams(i);
                    pe.IsInFAPool = teamsList.Count > 0 && teamsList.Single(te => te.ID == 999).RosterSpots.Contains(pe.ID);
                    pe.IsHidden = (pe.AssignedTo == -1 && !pe.IsInFAPool);
                    playersList.Add(pe);
                }
            }
            dgPlayers.ItemsSource = playersList;
        }

        private ObservableCollection<byte> ReadGear(int i, RosterReader brOpen)
        {
            var gear = new ObservableCollection<byte>();
            brOpen.MoveStreamToPortraitID(i);
            brOpen.MoveStreamPosition(129, 7);
            gear.Add(brOpen.ReadNBAByte(1));
            brOpen.MoveStreamPosition(2, 0);
            gear.Add(brOpen.ReadNBAByte(3));
            gear.Add(brOpen.ReadNBAByte(3));
            gear.Add(brOpen.ReadNBAByte(4));
            brOpen.MoveStreamPosition(1, 0);
            gear.Add(brOpen.ReadNBAByte(2));
            gear.Add(brOpen.ReadNBAByte(1));
            brOpen.MoveStreamPosition(0, 1);
            gear.Add(brOpen.ReadNBAByte(4));
            gear.Add(brOpen.ReadNBAByte(3));
            gear.Add(brOpen.ReadNBAByte(3));
            brOpen.MoveStreamPosition(0, 1);
            gear.Add(brOpen.ReadNBAByte(3));
            brOpen.MoveStreamPosition(0, 1);
            gear.Add(brOpen.ReadNBAByte(3));
            gear.Add(brOpen.ReadNBAByte(3));
            gear.Add(brOpen.ReadNBAByte(3));
            gear.Add(brOpen.ReadNBAByte(2));
            gear.Add(brOpen.ReadNBAByte(4)); // Socks
            for (int j = 0; j < 15; j++)
            {
                gear.Add(brOpen.ReadNBAByte(2));
            }
            brOpen.MoveStreamPosition(1, 0);
            gear.Add(brOpen.ReadNBAByte(3));
            brOpen.MoveStreamPosition(2, 3);
            for (int j = 0; j < 4; j++)
            {
                gear.Add(brOpen.ReadNBAByte(2));
            }
            brOpen.MoveStreamPosition(152, 0);
            gear.Add(brOpen.ReadNBAByte(3));
            gear.Add(brOpen.ReadNBAByte(3));

            // TUSS11 code for shoe trim
            brOpen.MoveStreamPosition(-175, -6);
            gear.Add(brOpen.ReadNBAByte(4)); // Home trim 1
            var h2 = brOpen.ReadNBAByte(4);
            var HShoeTrim2 = (byte)(h2 / 2);
            gear.Add(HShoeTrim2); // Home trim 2

            var a1 = brOpen.ReadNBAByte(4);
            var AShoeTrim1 = (byte)(a1 / 4);
            gear.Add(AShoeTrim1); // Away trim 1
            var a2 = brOpen.ReadNBAByte(4);
            var AShoeTrim2 = a2;
            if (a1 % 2 == 0)
            {
                AShoeTrim2 = (byte)(a2 / 8);
            }
            else
            {
                AShoeTrim2 = (byte)((a2 / 8) + 2);
            }
            gear.Add(AShoeTrim2); // Away trim 2
            //
            return gear;
        }

        private int FindPlayerInTeams(int playerID)
        {
            try
            {
                int id = teamsList.First(t => t.RosterSpots.Contains(playerID)).ID;
                return id != 999 ? id : -1;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        private ObservableCollection<byte> ReadRatings(int playerID, RosterReader brOpen)
        {
            var ratings = new ObservableCollection<byte>();

            brOpen.MoveStreamToFirstSS(playerID);
            brOpen.MoveStreamPosition(14, 3);

            for (int i = 0; i < 37; i++)
            {
                byte b = brOpen.ReadNonByteAlignedByte();
                var realRating = (byte) (b/3 + 25);
                ratings.Add(realRating);
            }

            return ratings;
        }

        private ObservableCollection<byte> ReadTendencies(int playerID, RosterReader brOpen)
        {
            var tend = new ObservableCollection<byte>();

            brOpen.MoveStreamToFirstSS(playerID);
            brOpen.MoveStreamPosition(51, 3);

            for (int i = 0; i < 69; i++)
            {
                byte b = brOpen.ReadNonByteAlignedByte();
                tend.Add(b);
            }

            return tend;
        }

        private ObservableCollection<byte> ReadHotSpots(int playerID, RosterReader brOpen)
        {
            var hs = new ObservableCollection<byte>();

            brOpen.MoveStreamToFirstSS(playerID);
            brOpen.MoveStreamPosition(120, 3);

            for (int i = 0; i < 25; i++)
            {
                byte b = brOpen.ReadNonByteAlignedByte();
                hs.Add(b);
            }

            return hs;
        }

        private ObservableCollection<HotZoneValue> ReadHotZones(int playerID, RosterReader brOpen)
        {
            var hz = new ObservableCollection<HotZoneValue>();

            brOpen.MoveStreamToPortraitID(playerID);
            brOpen.MoveStreamPosition(309, 2);

            for (int i = 0; i < 14; i++)
            {
                hz.Add((HotZoneValue) Enum.Parse(typeof (HotZoneValue), Convert.ToByte(brOpen.ReadNonByteAlignedBits(2), 2).ToString()));
            }

            return hz;
        }

        private void PopulateTeamsTab()
        {
            teamsList = new ObservableCollection<TeamEntry>();

            TeamEntry te;
            RosterReader brOpen;
            using (brOpen = new RosterReader(new MemoryStream(File.ReadAllBytes(currentFile))))
            {
                for (int i = Convert.ToInt32(GetOption("LastTeamID")); i >= 0; i--)
                {
                    te = new TeamEntry {ID = i};
                    try
                    {
                        te.Name = teamNames[i];
                    }
                    catch
                    {
                        te.Name = "";
                    }

                    brOpen.MoveStreamToCurrentTeamRoster(i);
                    PopulateRosterRow(18, ref te, brOpen);
                    if (te.RosterSpots.Any(rs => rs < -1))
                    {
                        teamsList.Clear();
                        dgTeams.ItemsSource = null;
                        throw (new Exception("Invalid Teams Offset, found player with ID lower than -1"));
                    }

                    //
                    brOpen.MoveStreamToCurrentTeamRoster(i);
                    brOpen.MoveStreamPosition(166, 0);
                    /*if (saveType == SaveType.Roster)
                        br.MoveStreamPosition(8, 0);*/
                    te.StHeadCoach = Convert.ToInt16(NonByteAlignedBinaryReader.ByteArrayToBitString(brOpen.ReadNonByteAlignedBytes(2)), 2);

                    brOpen.MoveStreamToCurrentTeamRoster(i);
                    brOpen.MoveStreamPosition(170, 0);
                    te.StAsstCoach = Convert.ToInt16(NonByteAlignedBinaryReader.ByteArrayToBitString(brOpen.ReadNonByteAlignedBytes(2)), 2);
                    //

                    brOpen.MoveStreamToCurrentTeamRoster(i);
                    brOpen.MoveStreamPosition(360, 0);
                    te.CurSeaSta = brOpen.ReadBigEndianInt16();
                    te.CurPlaSta = brOpen.ReadBigEndianInt16();
                    te.PrvSeaSta = brOpen.ReadBigEndianInt16();
                    te.PrvPlaSta = brOpen.ReadBigEndianInt16();
                }

                List<TeamEntry> temp = teamsList.ToList();
                temp.Sort((te1, te2) => te1.ID.CompareTo(te2.ID));
                teamsList = new ObservableCollection<TeamEntry>(temp);

                // Free Agents
                te = new TeamEntry {ID = 999, Name = "Free Agents"};
                
                brOpen.MoveStreamToFirstRoster();
                brOpen.BaseStream.Position -= 9406;

                PopulateRosterRow(512, ref te, brOpen, true);
                //
            }
            RelinkTeamsDataGrid();
        }

        private void PopulateRosterRow(int countToRead, ref TeamEntry te, RosterReader brOpen, bool isFArow = false)
        {
            long startOfRoster = brOpen.BaseStream.Position;
            int startOfRosterBit = brOpen.InBytePosition;

            for (int i = 0; i < countToRead; i++)
            {
                te.RosterSpots.Add(Convert.ToInt32(ReadRosterSpot(brOpen)));
            }

            brOpen.BaseStream.Position = startOfRoster;
            brOpen.InBytePosition = startOfRosterBit;

            brOpen.MoveStreamPosition(!isFArow ? 125 : 4001, 0);

            brOpen.MoveStreamPosition(0, -1);

            te.PlNum = Convert.ToUInt16(brOpen.ReadNonByteAlignedBits(9), 2);
            teamsList.Add(te);
        }

        private void RelinkTeamsDataGrid()
        {
            dgTeams.ItemsSource = null;
            dgTeams.ItemsSource = teamsList;
        }

        private string ReadRosterSpot(RosterReader brOpen)
        {
            byte[] b = brOpen.ReadNonByteAlignedBytes(2);
            if (b[0] == 0 && b[1] == 0)
                return "-1";

            b = brOpen.ReadNonByteAlignedBytes(2);
            return Convert.ToInt16(NonByteAlignedBinaryReader.ByteArrayToBitString(b), 2).ToString();
        }

        private ObservableCollection<SignatureSkill> ReadSignatureSkills(int playerID, RosterReader brOpen)
        {
            brOpen.MoveStreamToFirstSS(playerID);

            var ssList = new ObservableCollection<SignatureSkill>();

            byte b = NonByteAlignedBinaryReader.BitStringToByte(brOpen.ReadNonByteAlignedBits(6));
            ssList.Add(Enum.IsDefined(typeof (SignatureSkill), b) ? ((SignatureSkill) b) : 0);
            b = NonByteAlignedBinaryReader.BitStringToByte(brOpen.ReadNonByteAlignedBits(6));
            ssList.Add(Enum.IsDefined(typeof (SignatureSkill), b) ? ((SignatureSkill) b) : 0);
            brOpen.ReadNonByteAlignedBits(14);
            b = NonByteAlignedBinaryReader.BitStringToByte(brOpen.ReadNonByteAlignedBits(6));
            ssList.Add(Enum.IsDefined(typeof (SignatureSkill), b) ? ((SignatureSkill) b) : 0);
            b = NonByteAlignedBinaryReader.BitStringToByte(brOpen.ReadNonByteAlignedBits(6));
            ssList.Add(Enum.IsDefined(typeof (SignatureSkill), b) ? ((SignatureSkill) b) : 0);
            b = NonByteAlignedBinaryReader.BitStringToByte(brOpen.ReadNonByteAlignedBits(6));
            ssList.Add(Enum.IsDefined(typeof (SignatureSkill), b) ? ((SignatureSkill) b) : 0);

            return ssList;
        }

        private string FindPlayerName(PlayerEntry pe)
        {
            string val = typeof (PlayerEntry).GetProperty(GetOption("ChooseNameBy").ToString()).GetValue(pe, null).ToString();
            if (names.ContainsKey(val))
            {
                return names[val];
            }
            else
            {
                return "";
            }
        }

        private void ReadPlayerInfo(ref PlayerEntry pe, RosterReader brOpen)
        {
            int playerID = pe.ID;

            brOpen.MoveStreamToPortraitID(playerID); // 40616
            pe.Offset = brOpen.BaseStream.Position;

            byte[] por = brOpen.ReadNonByteAlignedBytes(2);

            // PlType
            brOpen.MoveStreamPosition(2, 6);

            byte plType = Convert.ToByte(brOpen.ReadNonByteAlignedBits(3), 2);

            brOpen.MoveStreamPosition(-3, -1);
            //


            brOpen.MoveStreamPosition(28, 0);

            byte[] cf = brOpen.ReadNonByteAlignedBytes(2);

            brOpen.MoveStreamPosition(94, 1);

            bool genericF = brOpen.ReadNonByteAlignedBits(1) == "1";

            brOpen.MoveStreamPosition(137, 6);

            byte[] audio = brOpen.ReadNonByteAlignedBytes(2);


            brOpen.MoveStreamToPortraitID(playerID);
            brOpen.MoveStreamPosition(-8, 0);
            pe.IsFAPar1 = brOpen.ReadNBAInt16(16);
            pe.IsFA1 = pe.IsFAPar1 != 266;
            brOpen.MoveStreamPosition(1, 0);
            pe.TeamID1 = brOpen.ReadNBAInt32(8);
            brOpen.MoveStreamPosition(258, 0);
            pe.IsFAPar2 = brOpen.ReadNBAInt16(16);
            pe.IsFA2 = pe.IsFAPar2 != 266;
            brOpen.MoveStreamPosition(1, 0);
            pe.TeamID2 = brOpen.ReadNBAInt32(8);
            brOpen.MoveStreamPosition(15, 4);
            pe.IsFAPar3 = brOpen.ReadNBAByte(1);
            pe.RFA = pe.IsFAPar3 == 1;

            brOpen.MoveStreamToPortraitID(playerID);
            brOpen.MoveStreamPosition(-16, 0);
            pe.Height = BitConverter.ToSingle(brOpen.ReadNonByteAlignedBytes(4).Reverse().ToArray(), 0);
            pe.Weight = BitConverter.ToSingle(brOpen.ReadNonByteAlignedBytes(4).Reverse().ToArray(), 0);


            brOpen.MoveStreamToPortraitID(playerID);
            brOpen.MoveStreamPosition(126, 6);
            pe.Skintone = Convert.ToInt32(brOpen.ReadNonByteAlignedBits(3), 2);
            brOpen.MoveStreamPosition(0, 6);
            pe.CAPHairClr = (HairColor) Enum.Parse(typeof (HairColor), Convert.ToByte(brOpen.ReadNonByteAlignedBits(4), 2).ToString());
            brOpen.MoveStreamPosition(0, 3);
            pe.CAPEyebrow = Convert.ToInt32(brOpen.ReadNonByteAlignedBits(4), 2);
            brOpen.MoveStreamPosition(0, 6);
            pe.CAPMstch = Convert.ToInt32(brOpen.ReadNonByteAlignedBits(3), 2);
            pe.CAPFclHairClr = (HairColor) Enum.Parse(typeof (HairColor), Convert.ToByte(brOpen.ReadNonByteAlignedBits(4), 2).ToString());
            pe.CAPBeard = Convert.ToInt32(brOpen.ReadNonByteAlignedBits(4), 2);
            pe.CAPGoatee = Convert.ToInt32(brOpen.ReadNonByteAlignedBits(5), 2);

            brOpen.MoveStreamToPortraitID(playerID);
            brOpen.MoveStreamPosition(154, 5);
            pe.PlayStyle = Convert.ToInt32(brOpen.ReadNonByteAlignedBits(5), 2);
            brOpen.MoveStreamToPortraitID(playerID);
            brOpen.MoveStreamPosition(143, 5);
            pe.PlayType1 = (PlayType) Enum.Parse(typeof (PlayType), Convert.ToByte(brOpen.ReadNonByteAlignedBits(4), 2).ToString());
            pe.PlayType2 = (PlayType) Enum.Parse(typeof (PlayType), Convert.ToByte(brOpen.ReadNonByteAlignedBits(4), 2).ToString());
            pe.PlayType3 = (PlayType) Enum.Parse(typeof (PlayType), Convert.ToByte(brOpen.ReadNonByteAlignedBits(4), 2).ToString());
            pe.PlayType4 = (PlayType) Enum.Parse(typeof (PlayType), Convert.ToByte(brOpen.ReadNonByteAlignedBits(4), 2).ToString());

            brOpen.MoveStreamToPortraitID(playerID);
            brOpen.MoveStreamPosition(178, 0);
            pe.DunkPackages = new ObservableCollection<int>();
            for (int i = 0; i < 15; i++)
            {
                pe.DunkPackages.Add(Convert.ToInt32(brOpen.ReadNonByteAlignedByte()));
            }

            brOpen.MoveStreamToPortraitID(playerID);
            brOpen.MoveStreamPosition(274, 2);
            pe.LayupPkg = brOpen.ReadNBAByte(4);

            brOpen.MoveStreamToPortraitID(playerID);
            brOpen.MoveStreamPosition(25, 0);
            pe.Position1 = brOpen.ReadAndConvertToEnum<Position>(3);
            pe.Position2 = brOpen.ReadAndConvertToEnum<Position>(3);

            //Shoe
            brOpen.MoveStreamToPortraitID(playerID);

            brOpen.MoveStreamPosition(122, 0);
            int shoeModel = Convert.ToInt32(brOpen.ReadNonByteAlignedBits(12), 2);

            var shoeBrand = (ShoeBrand) Enum.Parse(typeof (ShoeBrand), Convert.ToByte(brOpen.ReadNonByteAlignedBits(3), 2).ToString());


            brOpen.MoveStreamToPortraitID(playerID);
            brOpen.MoveStreamPosition(98, 0);
            byte[] ba = brOpen.ReadNonByteAlignedBytes(4);
            pe.ShHomeTeam1 = ba.Aggregate("#", (current, b) => current + Convert.ToString(b, 16).PadLeft(2, '0')).ToUpperInvariant();
            ba = brOpen.ReadNonByteAlignedBytes(4);
            pe.ShHomeTeam2 = ba.Aggregate("#", (current, b) => current + Convert.ToString(b, 16).PadLeft(2, '0')).ToUpperInvariant();
            ba = brOpen.ReadNonByteAlignedBytes(4);
            pe.ShHomeBase = ba.Aggregate("#", (current, b) => current + Convert.ToString(b, 16).PadLeft(2, '0')).ToUpperInvariant();
            ba = brOpen.ReadNonByteAlignedBytes(4);
            pe.ShAwayBase = ba.Aggregate("#", (current, b) => current + Convert.ToString(b, 16).PadLeft(2, '0')).ToUpperInvariant();
            ba = brOpen.ReadNonByteAlignedBytes(4);
            pe.ShAwayTeam2 = ba.Aggregate("#", (current, b) => current + Convert.ToString(b, 16).PadLeft(2, '0')).ToUpperInvariant();
            ba = brOpen.ReadNonByteAlignedBytes(4);
            pe.ShAwayTeam1 = ba.Aggregate("#", (current, b) => current + Convert.ToString(b, 16).PadLeft(2, '0')).ToUpperInvariant();
            brOpen.MoveStreamPosition(3, 5);
            pe.ShCustomClr = brOpen.ReadNonByteAlignedBits(1) == "1";
            //

            //Hair
            brOpen.MoveStreamToPortraitID(playerID);
            brOpen.MoveStreamPosition(127, 1);
            var capHair = (CAPHairType) Enum.Parse(typeof (CAPHairType), Convert.ToByte(brOpen.ReadNonByteAlignedBits(6), 2).ToString());
            //

            //
            brOpen.MoveStreamToPortraitID(playerID);
            brOpen.MoveStreamPosition(5, 4);
            byte number = brOpen.ReadNonByteAlignedByte();
            //

            //
            brOpen.MoveStreamToPortraitID(playerID);
            brOpen.MoveStreamPosition(126, 3);
            var bodyType = (BodyType) Enum.Parse(typeof (BodyType), Convert.ToByte(brOpen.ReadNonByteAlignedBits(2), 2).ToString());
            var muscleType = (MuscleTone) Enum.Parse(typeof (MuscleTone), Convert.ToByte(brOpen.ReadNonByteAlignedBits(1), 2).ToString());

            brOpen.MoveStreamToPortraitID(playerID);
            brOpen.MoveStreamPosition(128, 3);
            var eyes = (EyeColor) Enum.Parse(typeof (EyeColor), Convert.ToByte(brOpen.ReadNonByteAlignedBits(3), 2).ToString());
            //

            brOpen.MoveStreamToPortraitID(playerID);
            brOpen.MoveStreamPosition(88, 3);
            pe.ContYBefOpt = brOpen.ReadNBAByte(3);

            brOpen.MoveStreamToPortraitID(playerID);
            brOpen.MoveStreamPosition(154, 0);
            pe.ContractOpt =
                (ContractOption) Enum.Parse(typeof (ContractOption), Convert.ToByte(brOpen.ReadNonByteAlignedBits(2), 2).ToString());

            brOpen.MoveStreamToPortraitID(playerID);
            brOpen.MoveStreamPosition(177, 5);
            pe.ContNoTrade = brOpen.ReadNonByteAlignedBits(1) == "1";

            brOpen.MoveStreamToPortraitID(playerID);
            brOpen.MoveStreamPosition(214, 0);
            pe.ContractY1 = Convert.ToUInt32(NonByteAlignedBinaryReader.ByteArrayToBitString(brOpen.ReadNonByteAlignedBytes(4)), 2);
            pe.ContractY2 = Convert.ToUInt32(NonByteAlignedBinaryReader.ByteArrayToBitString(brOpen.ReadNonByteAlignedBytes(4)), 2);
            pe.ContractY3 = Convert.ToUInt32(NonByteAlignedBinaryReader.ByteArrayToBitString(brOpen.ReadNonByteAlignedBytes(4)), 2);
            pe.ContractY4 = Convert.ToUInt32(NonByteAlignedBinaryReader.ByteArrayToBitString(brOpen.ReadNonByteAlignedBytes(4)), 2);
            pe.ContractY5 = Convert.ToUInt32(NonByteAlignedBinaryReader.ByteArrayToBitString(brOpen.ReadNonByteAlignedBytes(4)), 2);
            pe.ContractY6 = Convert.ToUInt32(NonByteAlignedBinaryReader.ByteArrayToBitString(brOpen.ReadNonByteAlignedBytes(4)), 2);
            pe.ContractY7 = Convert.ToUInt32(NonByteAlignedBinaryReader.ByteArrayToBitString(brOpen.ReadNonByteAlignedBytes(4)), 2);

            brOpen.MoveStreamToPortraitID(playerID);
            brOpen.MoveStreamPosition(193, 0);
            pe.SigFT = brOpen.ReadNonByteAlignedByte();
            pe.SigShtForm = brOpen.ReadNonByteAlignedByte();
            pe.SigShtBase = brOpen.ReadNonByteAlignedByte();

            brOpen.MoveStreamToPortraitID(playerID);
            brOpen.MoveStreamPosition(314, 2);
            pe.ClothesType = (ClothesType) Enum.Parse(typeof (ClothesType), Convert.ToByte(brOpen.ReadNonByteAlignedBits(2), 2).ToString());


            brOpen.MoveStreamToPortraitID(playerID);
            brOpen.MoveStreamPosition(24, 1);
            pe.InjuryType = Convert.ToUInt16(brOpen.ReadNonByteAlignedBits(7), 2);
            brOpen.MoveStreamPosition(3, 0);
            pe.InjuryDays = Convert.ToInt16(NonByteAlignedBinaryReader.ByteArrayToBitString(brOpen.ReadNonByteAlignedBytes(2)), 2);


            brOpen.MoveStreamToPortraitID(playerID);
            brOpen.MoveStreamPosition(2, 0);
            pe.BirthYear = Convert.ToUInt16(brOpen.ReadNonByteAlignedBits(12), 2);
            pe.BirthMonth = Convert.ToByte(brOpen.ReadNonByteAlignedBits(4), 2);
            pe.BirthDay = Convert.ToByte(brOpen.ReadNonByteAlignedBits(5), 2);
            brOpen.MoveStreamToPortraitID(playerID);
            brOpen.MoveStreamPosition(87, 6);
            pe.YearsPro = Convert.ToByte(brOpen.ReadNonByteAlignedBits(5), 2);


            brOpen.MoveStreamToPortraitID(playerID);
            brOpen.MoveStreamPosition(40, 0);
            for (int i = 0; i < 22; i++)
            {
                pe.SeasonStats.Add(brOpen.ReadBigEndianInt16());
            }
            pe.PlayoffStats = brOpen.ReadBigEndianInt16();

            brOpen.MoveStreamToFirstSS(playerID);

            pe.CFID = Convert.ToUInt16(NonByteAlignedBinaryReader.ByteArrayToBitString(cf), 2);
            pe.PortraitID = Convert.ToUInt16(NonByteAlignedBinaryReader.ByteArrayToBitString(por), 2);
            pe.ASAID = Convert.ToUInt16(NonByteAlignedBinaryReader.ByteArrayToBitString(audio), 2);
            pe.GenericF = genericF;
            pe.PlType = plType;
            pe.ShoeBrand = shoeBrand;
            pe.ShoeModel = shoeModel;
            pe.CAPHairType = capHair;
            pe.Number = number;
            pe.MuscleTone = muscleType;
            pe.BodyType = bodyType;
            pe.EyeColor = eyes;
        }

        private void Window_Closing_1(object sender, CancelEventArgs e)
        {
            SetRegistrySetting("Height", Height);
            SetRegistrySetting("Width", Width);
            SetRegistrySetting("Left", Left);
            SetRegistrySetting("Top", Top);
        }

        public static void SetRegistrySetting<T>(string setting, T value)
        {
            RegistryKey rk = Registry.CurrentUser;
            try
            {
                try
                {
                    rk = rk.OpenSubKey(@"SOFTWARE\Lefteris Aslanoglou\NBA 2K13 Roster Editor", true);
                    if (rk == null)
                        throw new Exception();
                }
                catch (Exception)
                {
                    rk = Registry.CurrentUser;
                    rk.CreateSubKey(@"SOFTWARE\Lefteris Aslanoglou\NBA 2K13 Roster Editor");
                    rk = rk.OpenSubKey(@"SOFTWARE\Lefteris Aslanoglou\NBA 2K13 Roster Editor", true);
                    if (rk == null)
                        throw new Exception();
                }

                rk.SetValue(setting, value);
            }
            catch
            {
                MessageBox.Show("Couldn't save changed setting.");
            }
        }
        
        public static T GetRegistrySetting<T>(string setting, T defaultValue)
        {
            return (T)Convert.ChangeType(GetRegistrySetting(setting, defaultValue.ToString()), typeof(T));
        }

        public static string GetRegistrySetting(string setting, string defaultValue)
        {
            RegistryKey rk = Registry.CurrentUser;
            string settingValue = defaultValue;
            try
            {
                if (rk == null)
                    throw new Exception();

                rk = rk.OpenSubKey(@"SOFTWARE\Lefteris Aslanoglou\NBA 2K13 Roster Editor");
                if (rk != null)
                    settingValue = rk.GetValue(setting, defaultValue).ToString();
            }
            catch
            {
                settingValue = defaultValue;
            }

            return settingValue;
        }

        private void btnSavePlayers_Click(object sender, RoutedEventArgs e)
        {
            RosterReader brSave;
            using (brSave = new RosterReader(File.Open(currentFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
            {
                NonByteAlignedBinaryWriter bw;
                bool hadError = false;
                using (bw = new NonByteAlignedBinaryWriter(File.Open(currentFile, FileMode.Open, FileAccess.Write, FileShare.ReadWrite)))
                {
                    for (int i = 0; i < playersList.Count; i++)
                    {
                        try
                        {
                            //Console.WriteLine("Saving Player " + i);
                            PlayerEntry pe = playersList[i];
                            if (mode == Mode.CustomX360 && pe.ID == 1364)
                            {
                                continue;
                            }
                            brSave.MoveStreamToFirstSS(pe.ID);
                            bw.BaseStream.Position = brSave.BaseStream.Position;
                            bw.InBytePosition = brSave.InBytePosition;

                            // Appearance
                            long prevPos = brSave.BaseStream.Position;
                            int prevPosIn = brSave.InBytePosition;

                            brSave.MoveStreamToPortraitID(pe.ID);
                            Write2ByteStringToRoster(pe.PortraitID.ToString(), bw, ref brSave);

                            // PlType
                            brSave.MoveStreamPosition(2, 6);
                            SyncBWwithBR(ref bw, brSave);

                            bw.WriteNonByteAlignedBits(Convert.ToString(Convert.ToByte(pe.PlType.ToString()), 2).PadLeft(3, '0'),
                                                       brSave.ReadBytes(2));

                            SyncBRwithBW(ref brSave, bw);
                            brSave.MoveStreamPosition(-3, -1);
                            //


                            brSave.MoveStreamPosition(28, 0);

                            Write2ByteStringToRoster(pe.CFID.ToString(), bw, ref brSave);

                            brSave.MoveStreamPosition(94, 1);

                            SyncBWwithBR(ref bw, brSave);
                            byte b = brSave.ReadByte();
                            bw.WriteNonByteAlignedBits(pe.GenericF ? "1" : "0", new[] {b});
                            SyncBRwithBW(ref brSave, bw);

                            brSave.MoveStreamPosition(137, 6);

                            Write2ByteStringToRoster(pe.ASAID.ToString(), bw, ref brSave);


                            brSave.MoveStreamToPortraitID(pe.ID);
                            brSave.MoveStreamPosition(-8, 0);
                            SyncBWwithBR(ref bw, brSave);
                            WriteUInt16(bw, (ushort) (pe.IsFA1 ? 0 : 266), 16, ref brSave);
                            brSave.MoveStreamPosition(1, 0);
                            SyncBWwithBR(ref bw, brSave);
                            bw.WriteNonByteAlignedByte(Convert.ToByte(pe.TeamID1), brSave.ReadBytes(2));
                            SyncBRwithBW(ref brSave, bw);
                            brSave.MoveStreamPosition(258, 0);
                            SyncBWwithBR(ref bw, brSave);
                            WriteUInt16(bw, (ushort) (pe.IsFA2 ? 0 : 266), 16, ref brSave);
                            brSave.MoveStreamPosition(1, 0);
                            SyncBWwithBR(ref bw, brSave);
                            bw.WriteNonByteAlignedByte(Convert.ToByte(pe.TeamID2), brSave.ReadBytes(2));
                            SyncBRwithBW(ref brSave, bw);
                            brSave.MoveStreamPosition(15, 4);
                            SyncBWwithBR(ref bw, brSave);
                            WriteByte((byte) (pe.RFA ? 1 : 0), 1, bw, ref brSave);

                            // Play Style
                            brSave.MoveStreamToPortraitID(pe.ID);
                            brSave.MoveStreamPosition(154, 5);
                            SyncBWwithBR(ref bw, brSave);
                            bw.WriteNonByteAlignedBits(Convert.ToString(pe.PlayStyle, 2).PadLeft(5, '0'), brSave.ReadBytes(2));
                            SyncBRwithBW(ref brSave, bw);
                            brSave.MoveStreamToPortraitID(pe.ID);
                            brSave.MoveStreamPosition(143, 5);
                            SyncBWwithBR(ref bw, brSave);
                            bw.WriteNonByteAlignedBits(
                                Convert.ToString((byte) Enum.Parse(typeof (PlayType), pe.PlayType1.ToString()), 2).PadLeft(4, '0'),
                                brSave.ReadBytes(2));
                            SyncBRwithBW(ref brSave, bw);
                            bw.WriteNonByteAlignedBits(
                                Convert.ToString((byte) Enum.Parse(typeof (PlayType), pe.PlayType2.ToString()), 2).PadLeft(4, '0'),
                                brSave.ReadBytes(2));
                            SyncBRwithBW(ref brSave, bw);
                            bw.WriteNonByteAlignedBits(
                                Convert.ToString((byte) Enum.Parse(typeof (PlayType), pe.PlayType3.ToString()), 2).PadLeft(4, '0'),
                                brSave.ReadBytes(2));
                            SyncBRwithBW(ref brSave, bw);
                            bw.WriteNonByteAlignedBits(
                                Convert.ToString((byte) Enum.Parse(typeof (PlayType), pe.PlayType4.ToString()), 2).PadLeft(4, '0'),
                                brSave.ReadBytes(2));
                            SyncBRwithBW(ref brSave, bw);


                            // Height & Weight
                            brSave.MoveStreamToPortraitID(pe.ID);
                            brSave.MoveStreamPosition(-16, 0);
                            SyncBWwithBR(ref bw, brSave);
                            bw.WriteNonByteAlignedBits(
                                NonByteAlignedBinaryReader.ByteArrayToBitString(BitConverter.GetBytes(pe.Height).Reverse().ToArray()),
                                brSave.ReadBytes(5));
                            SyncBRwithBW(ref brSave, bw);
                            bw.WriteNonByteAlignedBits(
                                NonByteAlignedBinaryReader.ByteArrayToBitString(BitConverter.GetBytes(pe.Weight).Reverse().ToArray()),
                                brSave.ReadBytes(5));
                            //

                            // Positions
                            brSave.MoveStreamToPortraitID(pe.ID);
                            brSave.MoveStreamPosition(25, 0);
                            SyncBWwithBR(ref bw, brSave);
                            bw.WriteNonByteAlignedBits(
                                Convert.ToString((byte) Enum.Parse(typeof (Position), pe.Position1.ToString()), 2).PadLeft(3, '0'),
                                brSave.ReadBytes(2));
                            SyncBRwithBW(ref brSave, bw);
                            bw.WriteNonByteAlignedBits(
                                Convert.ToString((byte) Enum.Parse(typeof (Position), pe.Position2.ToString()), 2).PadLeft(3, '0'),
                                brSave.ReadBytes(2));

                            // Shoes
                            brSave.MoveStreamToPortraitID(pe.ID);
                            brSave.MoveStreamPosition(122, 0);
                            SyncBWwithBR(ref bw, brSave);
                            byte[] original = brSave.ReadBytes(3);
                            bw.WriteNonByteAlignedBits(Convert.ToString(pe.ShoeModel, 2).PadLeft(12, '0'), original);
                            SyncBRwithBW(ref brSave, bw);

                            original = brSave.ReadBytes(2);
                            bw.WriteNonByteAlignedBits(
                                Convert.ToString((byte) Enum.Parse(typeof (ShoeBrand), pe.ShoeBrand.ToString()), 2).PadLeft(3, '0'),
                                original);
                            SyncBRwithBW(ref brSave, bw);

                            brSave.MoveStreamToPortraitID(pe.ID);
                            brSave.MoveStreamPosition(98, 0);
                            SyncBWwithBR(ref bw, brSave);

                            bw.WriteNonByteAlignedBits(ConvertHexColorStringToBinaryString(pe.ShHomeTeam1.Substring(1)), brSave.ReadBytes(5));
                            SyncBRwithBW(ref brSave, bw);
                            bw.WriteNonByteAlignedBits(ConvertHexColorStringToBinaryString(pe.ShHomeTeam2.Substring(1)), brSave.ReadBytes(5));
                            SyncBRwithBW(ref brSave, bw);
                            bw.WriteNonByteAlignedBits(ConvertHexColorStringToBinaryString(pe.ShHomeBase.Substring(1)), brSave.ReadBytes(5));
                            SyncBRwithBW(ref brSave, bw);
                            bw.WriteNonByteAlignedBits(ConvertHexColorStringToBinaryString(pe.ShAwayBase.Substring(1)), brSave.ReadBytes(5));
                            SyncBRwithBW(ref brSave, bw);
                            bw.WriteNonByteAlignedBits(ConvertHexColorStringToBinaryString(pe.ShAwayTeam2.Substring(1)), brSave.ReadBytes(5));
                            SyncBRwithBW(ref brSave, bw);
                            bw.WriteNonByteAlignedBits(ConvertHexColorStringToBinaryString(pe.ShAwayTeam1.Substring(1)), brSave.ReadBytes(5));
                            SyncBRwithBW(ref brSave, bw);

                            brSave.MoveStreamPosition(3, 5);
                            SyncBWwithBR(ref bw, brSave);
                            bw.WriteNonByteAlignedBits(pe.ShCustomClr ? "1" : "0", brSave.ReadBytes(1));
                            //


                            //Hair
                            brSave.MoveStreamToPortraitID(pe.ID);
                            brSave.MoveStreamPosition(127, 1);
                            SyncBWwithBR(ref bw, brSave);
                            bw.WriteNonByteAlignedBits(
                                Convert.ToString((byte) Enum.Parse(typeof (CAPHairType), pe.CAPHairType.ToString()), 2).PadLeft(6, '0'),
                                brSave.ReadBytes(2));
                            //

                            //
                            brSave.MoveStreamToPortraitID(pe.ID);
                            brSave.MoveStreamPosition(5, 4);
                            SyncBWwithBR(ref bw, brSave);
                            bw.WriteNonByteAlignedByte(pe.Number, brSave.ReadBytes(2));
                            //

                            //
                            brSave.MoveStreamToPortraitID(pe.ID);
                            brSave.MoveStreamPosition(126, 3);
                            SyncBWwithBR(ref bw, brSave);
                            bw.WriteNonByteAlignedBits(
                                Convert.ToString((byte) Enum.Parse(typeof (BodyType), pe.BodyType.ToString()), 2).PadLeft(2, '0'),
                                brSave.ReadBytes(2));
                            SyncBRwithBW(ref brSave, bw);
                            bw.WriteNonByteAlignedBits(
                                Convert.ToString((byte) Enum.Parse(typeof (MuscleTone), pe.MuscleTone.ToString()), 2).PadLeft(1, '0'),
                                brSave.ReadBytes(1));

                            brSave.MoveStreamToPortraitID(pe.ID);
                            brSave.MoveStreamPosition(128, 3);
                            SyncBWwithBR(ref bw, brSave);
                            bw.WriteNonByteAlignedBits(
                                Convert.ToString((byte) Enum.Parse(typeof (EyeColor), pe.EyeColor.ToString()), 2).PadLeft(3, '0'),
                                brSave.ReadBytes(2));
                            //

                            brSave.MoveStreamToPortraitID(pe.ID);
                            brSave.MoveStreamPosition(314, 2);
                            SyncBWwithBR(ref bw, brSave);
                            bw.WriteNonByteAlignedBits(
                                Convert.ToString((byte) Enum.Parse(typeof (ClothesType), pe.ClothesType.ToString()), 2).PadLeft(2, '0'),
                                brSave.ReadBytes(2));


                            brSave.BaseStream.Position = prevPos;
                            brSave.InBytePosition = prevPosIn;
                            SyncBWwithBR(ref bw, brSave);
                            //

                            original = brSave.ReadBytes(2);
                            string s = pe.SSList[0].ToString();
                            bw.WriteNonByteAlignedBits(ConvertSignatureSkillToBinary(s), original);
                            SyncBRwithBW(ref brSave, bw);

                            original = brSave.ReadBytes(2);
                            s = pe.SSList[1].ToString();
                            bw.WriteNonByteAlignedBits(ConvertSignatureSkillToBinary(s), original);
                            SyncBRwithBW(ref brSave, bw);

                            brSave.MoveStreamPosition(0, 14);
                            bw.MoveStreamPosition(0, 14);

                            original = brSave.ReadBytes(2);
                            s = pe.SSList[2].ToString();
                            bw.WriteNonByteAlignedBits(ConvertSignatureSkillToBinary(s), original);
                            SyncBRwithBW(ref brSave, bw);

                            original = brSave.ReadBytes(2);
                            s = pe.SSList[3].ToString();
                            bw.WriteNonByteAlignedBits(ConvertSignatureSkillToBinary(s), original);
                            SyncBRwithBW(ref brSave, bw);

                            original = brSave.ReadBytes(2);
                            s = pe.SSList[4].ToString();
                            bw.WriteNonByteAlignedBits(ConvertSignatureSkillToBinary(s), original);
                            SyncBRwithBW(ref brSave, bw);


                            //Ratings
                            brSave.MoveStreamToFirstSS(i);
                            brSave.MoveStreamPosition(14, 3);
                            SyncBWwithBR(ref bw, brSave);
                            for (int j = 0; j < 37; j++)
                            {
                                byte byteToWrite;
                                try
                                {
                                    byteToWrite = (byte) ((pe.Ratings[j] - 25)*3);
                                }
                                catch (Exception)
                                {
                                    MessageBox.Show("Problem with Player ID " + pe.ID + ", Rating " + j + "\nWill try to continue.");
                                    continue;
                                }
                                bw.WriteNonByteAlignedByte(byteToWrite, brSave.ReadBytes(2));
                                SyncBRwithBW(ref brSave, bw);
                            }

                            //Tendencies
                            brSave.MoveStreamToFirstSS(i);
                            brSave.MoveStreamPosition(51, 3);
                            SyncBWwithBR(ref bw, brSave);
                            for (int j = 0; j < 69; j++)
                            {
                                byte byteToWrite;
                                try
                                {
                                    byteToWrite = pe.Tendencies[j];
                                }
                                catch (Exception)
                                {
                                    MessageBox.Show("Problem with Player ID " + pe.ID + ", Tendency " + j + "\nWill try to continue.");
                                    continue;
                                }
                                bw.WriteNonByteAlignedByte(byteToWrite, brSave.ReadBytes(2));
                                SyncBRwithBW(ref brSave, bw);
                            }

                            //Hot Spots
                            brSave.MoveStreamToFirstSS(i);
                            brSave.MoveStreamPosition(120, 3);
                            SyncBWwithBR(ref bw, brSave);
                            for (int j = 0; j < 25; j++)
                            {
                                byte byteToWrite;
                                try
                                {
                                    byteToWrite = pe.HotSpots[j];
                                }
                                catch (Exception)
                                {
                                    MessageBox.Show("Problem with Player ID " + pe.ID + ", Hot Spot " + j + "\nWill try to continue.");
                                    continue;
                                }
                                bw.WriteNonByteAlignedByte(byteToWrite, brSave.ReadBytes(2));
                                SyncBRwithBW(ref brSave, bw);
                            }

                            #region Contracts

                            if (pe.ContYBefOpt == 0 && pe.ContractY1 == 1 && pe.ContractY2 == 0)
                            {
                                pe.ContractY1 = 0;
                            }

                            pe.ContYBefOpt = 7;
                            for (int j = 1; j <= 7; j++)
                            {
                                if (Convert.ToInt32(typeof (PlayerEntry).GetProperty("ContractY" + j).GetValue(pe, null)) == 0)
                                {
                                    switch (pe.ContractOpt)
                                    {
                                        case ContractOption.None:
                                            pe.ContYBefOpt = (byte)(j - 1);
                                            break;
                                        case ContractOption.Team:
                                        case ContractOption.Player:
                                            pe.ContYBefOpt = (byte)(j - 2);
                                            break;
                                        case ContractOption.Team2Yr:
                                            pe.ContYBefOpt = (byte)(j - 3);
                                            break;
                                        default:
                                            throw new ArgumentOutOfRangeException();
                                    }
                                    break;
                                }
                            }
                            
                            brSave.MoveStreamToPortraitID(pe.ID);
                            brSave.MoveStreamPosition(88, 3);
                            SyncBWwithBR(ref bw, brSave);
                            WriteByte(pe.ContYBefOpt, 3, bw, ref brSave);

                            brSave.MoveStreamToPortraitID(pe.ID);
                            brSave.MoveStreamPosition(154, 0);
                            SyncBWwithBR(ref bw, brSave);
                            bw.WriteNonByteAlignedBits(
                                Convert.ToString((byte) Enum.Parse(typeof (ContractOption), pe.ContractOpt.ToString()), 2).PadLeft(2, '0'),
                                brSave.ReadBytes(2));
                            SyncBRwithBW(ref brSave, bw);

                            brSave.MoveStreamToPortraitID(pe.ID);
                            brSave.MoveStreamPosition(177, 5);
                            SyncBWwithBR(ref bw, brSave);
                            bw.WriteNonByteAlignedBits(pe.ContNoTrade ? "1" : "0", brSave.ReadBytes(1));
                            SyncBRwithBW(ref brSave, bw);

                            brSave.MoveStreamToPortraitID(pe.ID);
                            brSave.MoveStreamPosition(214, 0);
                            SyncBWwithBR(ref bw, brSave);
                            bw.WriteNonByteAlignedBits(Convert.ToString(pe.ContractY1, 2).PadLeft(32, '0'), brSave.ReadBytes(5));
                            SyncBRwithBW(ref brSave, bw);
                            bw.WriteNonByteAlignedBits(Convert.ToString(pe.ContractY2, 2).PadLeft(32, '0'), brSave.ReadBytes(5));
                            SyncBRwithBW(ref brSave, bw);
                            bw.WriteNonByteAlignedBits(Convert.ToString(pe.ContractY3, 2).PadLeft(32, '0'), brSave.ReadBytes(5));
                            SyncBRwithBW(ref brSave, bw);
                            bw.WriteNonByteAlignedBits(Convert.ToString(pe.ContractY4, 2).PadLeft(32, '0'), brSave.ReadBytes(5));
                            SyncBRwithBW(ref brSave, bw);
                            bw.WriteNonByteAlignedBits(Convert.ToString(pe.ContractY5, 2).PadLeft(32, '0'), brSave.ReadBytes(5));
                            SyncBRwithBW(ref brSave, bw);
                            bw.WriteNonByteAlignedBits(Convert.ToString(pe.ContractY6, 2).PadLeft(32, '0'), brSave.ReadBytes(5));
                            SyncBRwithBW(ref brSave, bw);
                            bw.WriteNonByteAlignedBits(Convert.ToString(pe.ContractY7, 2).PadLeft(32, '0'), brSave.ReadBytes(5));
                            SyncBRwithBW(ref brSave, bw);

                            #endregion

                            brSave.MoveStreamToPortraitID(pe.ID);
                            brSave.MoveStreamPosition(126, 6);
                            SyncBWwithBR(ref bw, brSave);
                            bw.WriteNonByteAlignedBits(Convert.ToString(pe.Skintone, 2).PadLeft(3, '0'), brSave.ReadBytes(2));
                            SyncBRwithBW(ref brSave, bw);

                            brSave.MoveStreamPosition(0, 6);
                            SyncBWwithBR(ref bw, brSave);
                            bw.WriteNonByteAlignedBits(
                                Convert.ToString((byte) Enum.Parse(typeof (HairColor), pe.CAPHairClr.ToString()), 2).PadLeft(4, '0'),
                                brSave.ReadBytes(2));
                            SyncBRwithBW(ref brSave, bw);

                            brSave.MoveStreamPosition(0, 3);
                            SyncBWwithBR(ref bw, brSave);
                            bw.WriteNonByteAlignedBits(Convert.ToString(pe.CAPEyebrow, 2).PadLeft(4, '0'), brSave.ReadBytes(2));
                            SyncBRwithBW(ref brSave, bw);
                            brSave.MoveStreamPosition(0, 6);
                            SyncBWwithBR(ref bw, brSave);
                            bw.WriteNonByteAlignedBits(Convert.ToString(pe.CAPMstch, 2).PadLeft(3, '0'), brSave.ReadBytes(2));
                            SyncBRwithBW(ref brSave, bw);
                            bw.WriteNonByteAlignedBits(
                                Convert.ToString((byte) Enum.Parse(typeof (HairColor), pe.CAPFclHairClr.ToString()), 2).PadLeft(4, '0'),
                                brSave.ReadBytes(2));
                            SyncBRwithBW(ref brSave, bw);
                            bw.WriteNonByteAlignedBits(Convert.ToString(pe.CAPBeard, 2).PadLeft(4, '0'), brSave.ReadBytes(2));
                            SyncBRwithBW(ref brSave, bw);
                            bw.WriteNonByteAlignedBits(Convert.ToString(pe.CAPGoatee, 2).PadLeft(5, '0'), brSave.ReadBytes(2));
                            SyncBRwithBW(ref brSave, bw);

                            brSave.MoveStreamToPortraitID(pe.ID);
                            brSave.MoveStreamPosition(178, 0);
                            SyncBWwithBR(ref bw, brSave);
                            for (int j = 0; j < 15; j++)
                            {
                                bw.WriteNonByteAlignedBits(Convert.ToString(pe.DunkPackages[j], 2).PadLeft(8, '0'), brSave.ReadBytes(2));
                                SyncBRwithBW(ref brSave, bw);
                            }

                            brSave.MoveStreamToPortraitID(pe.ID);
                            brSave.MoveStreamPosition(274, 2);
                            SyncBWwithBR(ref bw, brSave);
                            bw.WriteNonByteAlignedBits(Convert.ToString(pe.LayupPkg, 2).PadLeft(4, '0'), brSave.ReadBytes(2));

                            brSave.MoveStreamToPortraitID(pe.ID);
                            brSave.MoveStreamPosition(193, 0);
                            SyncBWwithBR(ref bw, brSave);
                            bw.WriteNonByteAlignedByte(Convert.ToByte(pe.SigFT), brSave.ReadBytes(2));
                            SyncBRwithBW(ref brSave, bw);
                            bw.WriteNonByteAlignedByte(Convert.ToByte(pe.SigShtForm), brSave.ReadBytes(2));
                            SyncBRwithBW(ref brSave, bw);
                            bw.WriteNonByteAlignedByte(Convert.ToByte(pe.SigShtBase), brSave.ReadBytes(2));
                            SyncBRwithBW(ref brSave, bw);

                            brSave.MoveStreamToPortraitID(pe.ID);
                            brSave.MoveStreamPosition(24, 1);
                            SyncBWwithBR(ref bw, brSave);
                            bw.WriteNonByteAlignedBits(Convert.ToString(pe.InjuryType, 2).PadLeft(7, '0'), brSave.ReadBytes(2));
                            SyncBRwithBW(ref brSave, bw);
                            brSave.MoveStreamPosition(3, 0);
                            SyncBWwithBR(ref bw, brSave);
                            bw.WriteNonByteAlignedBits(Convert.ToString(pe.InjuryDays, 2).PadLeft(16, '0'), brSave.ReadBytes(3));
                            SyncBRwithBW(ref brSave, bw);

                            brSave.MoveStreamToPortraitID(pe.ID);
                            brSave.MoveStreamPosition(2, 0);
                            SyncBWwithBR(ref bw, brSave);
                            bw.WriteNonByteAlignedBits(Convert.ToString(pe.BirthYear, 2).PadLeft(12, '0'), brSave.ReadBytes(3));
                            SyncBRwithBW(ref brSave, bw);
                            bw.WriteNonByteAlignedBits(Convert.ToString(pe.BirthMonth, 2).PadLeft(4, '0'), brSave.ReadBytes(2));
                            SyncBRwithBW(ref brSave, bw);
                            bw.WriteNonByteAlignedBits(Convert.ToString(pe.BirthDay, 2).PadLeft(5, '0'), brSave.ReadBytes(2));
                            brSave.MoveStreamToPortraitID(pe.ID);
                            brSave.MoveStreamPosition(87, 6);
                            SyncBWwithBR(ref bw, brSave);
                            bw.WriteNonByteAlignedBits(Convert.ToString(pe.YearsPro, 2).PadLeft(5, '0'), brSave.ReadBytes(2));

                            brSave.MoveStreamToPortraitID(pe.ID);
                            brSave.MoveStreamPosition(40, 0);
                            SyncBWwithBR(ref bw, brSave);
                            for (int j = 0; j < 22; j++)
                            {
                                bw.WriteNonByteAlignedBits(Convert.ToString(pe.SeasonStats[j], 2).PadLeft(16, '0'), brSave.ReadBytes(3));
                                SyncBRwithBW(ref brSave, bw);
                            }
                            bw.WriteNonByteAlignedBits(Convert.ToString(pe.PlayoffStats, 2).PadLeft(16, '0'), brSave.ReadBytes(3));

                            // Gear
                            int k = 0;
                            brSave.MoveStreamToPortraitID(pe.ID);
                            brSave.MoveStreamPosition(129, 7);
                            SyncBWwithBR(ref bw, brSave);
                            WriteByte(pe.Accessories[k++], 1, bw, ref brSave);
                            brSave.MoveStreamPosition(2, 0);
                            SyncBWwithBR(ref bw, brSave);
                            WriteByte(pe.Accessories[k++], 3, bw, ref brSave);
                            WriteByte(pe.Accessories[k++], 3, bw, ref brSave);
                            WriteByte(pe.Accessories[k++], 4, bw, ref brSave);
                            brSave.MoveStreamPosition(1, 0);
                            SyncBWwithBR(ref bw, brSave);
                            WriteByte(pe.Accessories[k++], 2, bw, ref brSave);
                            WriteByte(pe.Accessories[k++], 1, bw, ref brSave);
                            brSave.MoveStreamPosition(0, 1);
                            SyncBWwithBR(ref bw, brSave);
                            WriteByte(pe.Accessories[k++], 4, bw, ref brSave);
                            WriteByte(pe.Accessories[k++], 3, bw, ref brSave);
                            WriteByte(pe.Accessories[k++], 3, bw, ref brSave);
                            brSave.MoveStreamPosition(0, 1);
                            SyncBWwithBR(ref bw, brSave);
                            WriteByte(pe.Accessories[k++], 3, bw, ref brSave);
                            brSave.MoveStreamPosition(0, 1);
                            SyncBWwithBR(ref bw, brSave);
                            WriteByte(pe.Accessories[k++], 3, bw, ref brSave);
                            WriteByte(pe.Accessories[k++], 3, bw, ref brSave);
                            WriteByte(pe.Accessories[k++], 3, bw, ref brSave);
                            WriteByte(pe.Accessories[k++], 2, bw, ref brSave);
                            WriteByte(pe.Accessories[k++], 4, bw, ref brSave);
                            for (int l = 0; l < 15; l++)
                            {
                                WriteByte(pe.Accessories[k++], 2, bw, ref brSave);
                            }
                            brSave.MoveStreamPosition(1, 0);
                            SyncBWwithBR(ref bw, brSave);
                            WriteByte(pe.Accessories[k++], 3, bw, ref brSave);
                            brSave.MoveStreamPosition(2, 3);
                            SyncBWwithBR(ref bw, brSave);
                            for (int l = 0; l < 4; l++)
                            {
                                WriteByte(pe.Accessories[k++], 2, bw, ref brSave);
                            }
                            brSave.MoveStreamPosition(152, 0);
                            SyncBWwithBR(ref bw, brSave);
                            WriteByte(pe.Accessories[k++], 3, bw, ref brSave);
                            WriteByte(pe.Accessories[k++], 3, bw, ref brSave);
                            //
                            brSave.MoveStreamPosition(-175, -6); // Shoe trim colors
                            SyncBWwithBR(ref bw, brSave);
                            byte homeShoeByte;
                            try
                            {
                                homeShoeByte = (byte)((pe.Accessories[37] * 16) + (pe.Accessories[38] * 2));
                            }
                            catch (Exception)
                            {
                                MessageBox.Show("Problem with Player ID " + pe.ID + ", HomeShoeTrim1 and HomeShoeTrim2 " + "\nWill try to continue.");
                                continue;
                            }
                            bw.WriteNonByteAlignedByte(homeShoeByte, brSave.ReadBytes(2));
                            SyncBRwithBW(ref brSave, bw);

                            byte awayShoeByte;
                            try
                            {
                                awayShoeByte = (byte)((pe.Accessories[39] * 64) + (pe.Accessories[40] * 8));
                            }
                            catch (Exception)
                            {
                                MessageBox.Show("Problem with Player ID " + pe.ID + ", AwayShoeTrim1 and AwayShoeTrim2 " + "\nWill try to continue.");
                                continue;
                            }
                            bw.WriteNonByteAlignedByte(awayShoeByte, brSave.ReadBytes(2));
                            SyncBRwithBW(ref brSave, bw);
                        }
                        catch (Exception ex)
                        {
                            Trace.Write("Error while trying to save player with ID " + playersList[i].ID + ": " + ex.Message + "\n" +
                                        ex.StackTrace + "\n\n");
                            hadError = true;
                        }
                    }
                    if (hadError)
                    {
                        MessageBox.Show(
                            "One or more errors happened while trying to save this tab. Please check the tracelog.txt file in " +
                            "My Documents\\NBA 2K13 Roster Editor and, if needed, report the errors to the developer.");
                    }
                }
            }

            if (doCRC)
            {
                RecalculateCRC();
            }

            updateStatus("Players saved.");
        }

        private static string ConvertHexColorStringToBinaryString(string s)
        {
            var colorBytes = new byte[4];
            colorBytes[0] = Convert.ToByte(s.Substring(0, 2), 16);
            colorBytes[1] = Convert.ToByte(s.Substring(2, 2), 16);
            colorBytes[2] = Convert.ToByte(s.Substring(4, 2), 16);
            colorBytes[3] = Convert.ToByte(s.Substring(6, 2), 16);
            string toWrite = NonByteAlignedBinaryReader.ByteArrayToBitString(colorBytes);
            return toWrite;
        }

        private void SyncBRwithBW(ref RosterReader br, NonByteAlignedBinaryWriter bw)
        {
            br.BaseStream.Position = bw.BaseStream.Position;
            br.InBytePosition = bw.InBytePosition;
        }

        private void SyncBWwithBR(ref NonByteAlignedBinaryWriter bw, RosterReader br)
        {
            bw.BaseStream.Position = br.BaseStream.Position;
            bw.InBytePosition = br.InBytePosition;
        }

        private void RecalculateCRC()
        {
            byte[] file = File.ReadAllBytes(currentFile).Skip(4).ToArray();
            string crc = Crc32.CalculateCRC(file);

            using (var bw2 = new BinaryWriter(new FileStream(currentFile, FileMode.Open)))
            {
                bw2.Write(Tools.HexStringToByteArray(crc));
                bw2.Write(file);
            }
        }

        private string ConvertSignatureSkillToBinary(string signatureSkill)
        {
            return Convert.ToString((byte) Enum.Parse(typeof (SignatureSkill), signatureSkill), 2).PadLeft(6, '0');
        }

        private void chkRecalculateCRC_Checked(object sender, RoutedEventArgs e)
        {
            SetRegistrySetting("CRC", 1);
            doCRC = true;
        }

        private void chkRecalculateCRC_Unchecked(object sender, RoutedEventArgs e)
        {
            SetRegistrySetting("CRC", 0);
            doCRC = false;
        }

        private void btnSaveTeams_Click(object sender, RoutedEventArgs e)
        {
            FixPlNumAndOrder();

            RosterReader brSave;
            using (brSave = new RosterReader(File.Open(currentFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
            {
                for (int i = 0; i < teamsList.Count - 1; i++)
                {
                    brSave.MoveStreamToCurrentTeamRoster(i);
                    WriteRosterRow(i, 18, brSave);

                    //
                    brSave.MoveStreamToCurrentTeamRoster(i);
                    brSave.MoveStreamPosition(166, 0);
                    //if (saveType == SaveType.Roster)
                    //brSave.MoveStreamPosition(8, 0);

                    NonByteAlignedBinaryWriter bw;
                    using (bw = new NonByteAlignedBinaryWriter(new FileStream(currentFile, FileMode.Open)))
                    {
                        SyncBWwithBR(ref bw, brSave);

                        int hc = teamsList[i].StHeadCoach;
                        bw.WriteNonByteAlignedBits(Convert.ToString(hc, 2).PadLeft(16, '0'), brSave.ReadBytes(3));
                        //

                        //
                        brSave.MoveStreamToCurrentTeamRoster(i);
                        brSave.MoveStreamPosition(170, 0);

                        SyncBWwithBR(ref bw, brSave);

                        int ac = teamsList[i].StAsstCoach;
                        bw.WriteNonByteAlignedBits(Convert.ToString(ac, 2).PadLeft(16, '0'), brSave.ReadBytes(3));
                    }
                    //
                }

                // Free Agents
                const int faRow = 999;

                brSave.MoveStreamToFirstRoster();
                brSave.BaseStream.Position -= 9406;

                WriteRosterRow(faRow, 512, brSave, true);
                //
            }

            if (doCRC)
            {
                RecalculateCRC();
            }

            PopulatePlayersTab();

            updateStatus("Teams saved.");
        }

        private void WriteRosterRow(int teamID, int spotsToWrite, RosterReader brSave, bool isFArow = false)
        {
            NonByteAlignedBinaryWriter bw;
            using (bw = new NonByteAlignedBinaryWriter(new FileStream(currentFile, FileMode.Open)))
            {
                long startOfRoster = brSave.BaseStream.Position;
                int startOfRosterBit = brSave.InBytePosition;
                SyncBWwithBR(ref bw, brSave);

                TeamEntry te = teamsList.Single(t => t.ID == teamID);

                int plNum = te.PlNum;
                for (int i = 0; i < spotsToWrite; i++)
                {
                    WriteRosterSpot(te.RosterSpots[i].ToString(), bw, i, plNum, ref brSave);
                }

                brSave.BaseStream.Position = startOfRoster;
                brSave.InBytePosition = startOfRosterBit;

                brSave.MoveStreamPosition(!isFArow ? 125 : 4001, 0);
                brSave.MoveStreamPosition(0, -1);

                SyncBWwithBR(ref bw, brSave);
                byte[] original = brSave.ReadBytes(2);
                SyncBRwithBW(ref brSave, bw);
                bw.WriteNonByteAlignedBits(Convert.ToString(Convert.ToUInt16(plNum.ToString()), 2).PadLeft(9, '0'), original);
            }
        }

        private object GetCellValue(DataGrid dataGrid, int row, int col)
        {
            var dataRowView = dataGrid.Items[row] as DataRowView;
            if (dataRowView != null)
                return dataRowView.Row.ItemArray[col];

            return null;
        }

        private void WriteRosterSpot(string text, NonByteAlignedBinaryWriter bw, int spot, int plNum, ref RosterReader brSave)
        {
            var first2 = new byte[2];
            if (spot < plNum)
            {
                first2[0] = 1;
                first2[1] = 3;
            }
            else
            {
                first2[0] = 0;
                first2[0] = 0;
                text = ("").PadLeft(16, '0');
            }
            Write2ByteStringToRoster(text, bw, ref brSave, first2);
        }

        private void Write2ByteStringToRoster(string p, NonByteAlignedBinaryWriter bw, ref RosterReader brSave, byte[] first2 = null)
        {
            bw.BaseStream.Position = brSave.BaseStream.Position;
            bw.InBytePosition = brSave.InBytePosition;

            UInt16 id = Convert.ToUInt16(p);
            byte[] original;
            string s = "";

            if (first2 != null)
            {
                original = brSave.ReadBytes(5);
                s = NonByteAlignedBinaryReader.ByteArrayToBitString(first2);
            }
            else
            {
                original = brSave.ReadBytes(3);
            }
            s += Convert.ToString(id, 2).PadLeft(16, '0');
            bw.WriteNonByteAlignedBits(s, original);

            brSave.BaseStream.Position = bw.BaseStream.Position;
            brSave.InBytePosition = bw.InBytePosition;
        }

        private void AnyTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ((TextBox) sender).SelectAll();
        }

        private void btnFill_Click(object sender, RoutedEventArgs e)
        {
            int id = teamsList[0].RosterSpots[0];
            for (int i = 1; i < teamsList[0].PlNum; i++)
            {
                teamsList[0].RosterSpots[i] = ++id;
            }
        }

        private void btnModePC_Checked(object sender, RoutedEventArgs e)
        {
            SetRegistrySetting("Mode", "PC");
            mode = Mode.PC;
            chkRecalculateCRC.IsChecked = true;

            SetRegistrySetting("FirstSSOffset", 40916);
            SetRegistrySetting("FirstSSOffsetBit", 2);
            try
            {
                ReloadEverything();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void ReloadEverything()
        {
            ReloadOptions();
            if (!String.IsNullOrWhiteSpace(currentFile))
            {
                try
                {
                    PopulateJerseysTab();
                }
                catch
                {
                    dgJerseys.ItemsSource = null;
                }
                try
                {
                    PopulateTeamsTab();
                }
                catch
                {
                    dgTeams.ItemsSource = null;
                }
                try
                {
                    PopulateStaffTab();
                }
                catch
                {
                    try
                    {
                        SetRegistrySetting("LastStaffID", Math.Max(teamsList.Max(t => t.StHeadCoach), teamsList.Max(t => t.StAsstCoach)));
                        ReloadOptions();
                        PopulateStaffTab();
                    }
                    catch
                    {
                        dgStaff.ItemsSource = null;
                    }
                }
                try
                {
                    PopulatePlayersTab();
                }
                catch
                {
                    dgPlayers.ItemsSource = null;
                }
                try
                {
                    PopulatePlaybooksTab();
                }
                catch
                {
                    dgPlaybooks.ItemsSource = null;
                }
                try
                {
                    PopulateTeamStatsTab();
                }
                catch
                {
                    dgTeamStats.ItemsSource = null;
                }
                expCount = 0;

#if DEBUG
                //try
                //{
                PopulatePlayerStatsTab();
                //}
                //catch
                //{
                //dgPlayerStats.ItemsSource = null;
                //}
#endif
            }
        }

        private void PopulateTeamStatsTab()
        {
            /*
            dgTeamStats.Columns.Where(c => c.Header.ToString().Contains("Exp")).ToList().ForEach(c => dgTeamStats.Columns.Remove(c));
            for (int i = 0; i < 4; i++)
            {
                dgTeamStats.Columns.Add(new DataGridTextColumn
                                        {
                                            Header = "Exp" + i,
                                            Binding =
                                                new Binding
                                                {
                                                    Path = new PropertyPath(string.Format("Experimental[{0}]", i)),
                                                    Mode = BindingMode.TwoWay
                                                }
                                        });
            }
            */
            teamStatsList = new ObservableCollection<TeamStatsEntry>();
            RosterReader brOpen;
            using (brOpen = new RosterReader(new MemoryStream(File.ReadAllBytes(currentFile))))
            {
                for (int i = 0; i < 248; i++)
                {
                    brOpen.MoveStreamToTeamStats(i);
                    var tse = new TeamStatsEntry();
                    tse.ID = i;
                    tse.Wins = brOpen.ReadNonByteAlignedByte();
                    tse.Losses = brOpen.ReadNonByteAlignedByte();
                    tse.MINS = brOpen.ReadBigEndianUInt16();
                    tse.PF = brOpen.ReadBigEndianUInt16();
                    tse.PA = brOpen.ReadBigEndianUInt16();
                    brOpen.MoveStreamPosition(2, 0);
                    tse.FGM = brOpen.ReadBigEndianUInt16();
                    tse.FGA = brOpen.ReadBigEndianUInt16();
                    tse.TPM = brOpen.ReadBigEndianUInt16();
                    tse.TPA = brOpen.ReadBigEndianUInt16();
                    tse.FTM = brOpen.ReadBigEndianUInt16();
                    tse.FTA = brOpen.ReadBigEndianUInt16();
                    tse.OREB = brOpen.ReadBigEndianUInt16();
                    tse.DREB = brOpen.ReadBigEndianUInt16();
                    tse.STL = brOpen.ReadBigEndianUInt16();
                    tse.TOS = brOpen.ReadBigEndianUInt16();
                    tse.BLK = brOpen.ReadBigEndianUInt16();
                    tse.AST = brOpen.ReadBigEndianUInt16();
                    tse.FOUL = brOpen.ReadBigEndianUInt16();
                    /*
                    for (int j = 0; j < 3; j++)
                    {
                        tse.Experimental.Add(ReadBigEndianUInt16());
                    }
                    */
                    teamStatsList.Add(tse);
                }
            }

            dgTeamStats.ItemsSource = null;
            dgTeamStats.ItemsSource = teamStatsList;
        }

        private void PopulatePlayerStatsTab()
        {
            dgPlayerStats.Columns.Where(c => c.Header.ToString().Contains("Exp")).ToList().ForEach(c => dgPlayerStats.Columns.Remove(c));
            for (int i = 0; i < expCount; i++)
            {
                dgPlayerStats.Columns.Add(new DataGridTextColumn
                                          {
                                              Header = "Exp" + i,
                                              Binding =
                                                  new Binding
                                                  {
                                                      Path =
                                                          new PropertyPath(string.Format(
                                                              "Experimental[{0}]", i)),
                                                      Mode = BindingMode.TwoWay
                                                  }
                                          });
            }
            playerStatsList = new ObservableCollection<PlayerStatsEntry>();

            RosterReader brOpen;
            using (brOpen = new RosterReader(new MemoryStream(File.ReadAllBytes(currentFile))))
            {
                for (int i = 0; i < 200; i++)
                {
                    var pse = new PlayerStatsEntry {ID = i};

                    brOpen.MoveStreamRelativeToPlayerStatsEntry(i, 0, 1);
                    pse.GP = brOpen.ReadUInt16(7);

                    brOpen.MoveStreamRelativeToPlayerStatsEntry(i, 0, 0);
                    ushort temp = brOpen.ReadUInt16(1);
                    brOpen.MoveStreamRelativeToPlayerStatsEntry(i, 1, 2);
                    temp += brOpen.ReadUInt16AndRaise<UInt16>(6, 1);
                    pse.GS = temp;

                    brOpen.MoveStreamRelativeToPlayerStatsEntry(i, 24, 0);
                    pse.PTS = brOpen.ReadUInt16();

                    brOpen.MoveStreamRelativeToPlayerStatsEntry(i, 20, 0);
                    pse.DREB = brOpen.ReadUInt16();

                    for (int j = 0; j < expCount; j++)
                    {
                        pse.Experimental.Add(brOpen.ReadBigEndianUInt16());
                    }

                    playerStatsList.Add(pse);
                }
            }

            dgPlayerStats.ItemsSource = null;
            dgPlayerStats.ItemsSource = playerStatsList;
        }


        private void PopulateStaffTab()
        {
            staffList = new ObservableCollection<StaffEntry>();
            RosterReader brOpen;
            using (brOpen = new RosterReader(new MemoryStream(File.ReadAllBytes(currentFile))))
            {
                for (int i = 0; i <= Convert.ToInt32(GetOption("LastStaffID")); i++)
                {
                    brOpen.MoveStreamToStaffPlaybookID(i);

                    var se = new StaffEntry {ID = i};

                    try
                    {
                        se.PlaybookID = Convert.ToInt32(brOpen.ReadNonByteAlignedBits(7), 2);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        se.PlaybookID = -1;
                    }

                    brOpen.MoveStreamToStaffPlaybookID(i);
                    brOpen.MoveStreamPosition(-24, 0);
                    for (int j = 0; j < 21; j++)
                    {
                        se.CoachingProfile.Add(brOpen.ReadNonByteAlignedByte());
                    }

                    brOpen.MoveStreamToStaffPlaybookID(i);
                    brOpen.MoveStreamPosition(6, 2);
                    se.CFID = brOpen.ReadNBAInt32(13);
                    brOpen.MoveStreamPosition(1, 0);
                    se.PortraitID = brOpen.ReadNBAInt32(13);

                    try
                    {
                        se.HeadCoachOf =
                            teamsList.Where(te => te.StHeadCoach == se.ID)
                                     .Select(te => te.ID.ToString())
                                     .Aggregate((i1, i2) => i1 + ", " + i2);
                    }
                    catch (InvalidOperationException)
                    {
                        se.HeadCoachOf = "";
                    }

                    List<int> teamsToUpdate = teamsList.Where(te => te.StHeadCoach == se.ID).Select(te => te.ID).ToList();
                    foreach (int teamID in teamsToUpdate)
                    {
                        teamsList.Single(te => te.ID == teamID).PlaybookID = se.PlaybookID;
                    }

                    staffList.Add(se);
                }
            }

            dgStaff.ItemsSource = null;
            dgStaff.ItemsSource = staffList;
        }

        private void PopulatePlaybooksTab()
        {
            playbooksList = new ObservableCollection<PlaybookEntry>();
            RosterReader brOpen;
            using (brOpen = new RosterReader(new MemoryStream(File.ReadAllBytes(currentFile))))
            {
                for (int i = 0; i <= Convert.ToInt32(GetOption("LastPlaybookID")); i++)
                {
                    brOpen.MoveStreamToPlaybook(i);

                    var pb = new PlaybookEntry {ID = i};

                    for (int j = 0; j < 50; j++)
                    {
                        byte[] ba = brOpen.ReadNonByteAlignedBytes(4);
                        pb.Plays.Add(ba.Aggregate("", (current, b) => current + Convert.ToString(b, 16).PadLeft(2, '0')).ToUpperInvariant());
                    }
                    playbooksList.Add(pb);
                }
            }

            dgPlaybooks.ItemsSource = null;
            dgPlaybooks.ItemsSource = playbooksList;
        }

        private void btnMode360_Checked(object sender, RoutedEventArgs e)
        {
            SetRegistrySetting("Mode", "X360");
            mode = Mode.X360;
            chkRecalculateCRC.IsChecked = false;

            SetRegistrySetting("FirstSSOffset", 94164);
            SetRegistrySetting("FirstSSOffsetBit", 2);
            try
            {
                ReloadEverything();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void dgTeams_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.V && Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                if (GetOption("DumbPasting").ToString() == "True")
                {
                    updateStatus(!GenericEventHandlers.OnExecutedPaste(sender, null)
                                     ? "Paste into table completed but with errors. Check tracelog.txt for details."
                                     : "Paste into table completed.");
                }
                else
                {
                    bool noErrors = true;
                    string text = Clipboard.GetText();
                    string[] lines = Tools.SplitLinesToArray(text);
                    if (!lines[0].StartsWith("ID\t") && !lines[0].Contains("\tID\t"))
                    {
                        MessageBox.Show("The pasted data must have the column headers in the first row.");
                        return;
                    }
                    List<Dictionary<string, string>> dictList = CSV.DictionaryListFromTSVString(text);

                    dgTeams.CommitEdit();
                    dgTeams.CancelEdit();

                    for (int index = 0; index < dictList.Count; index++)
                    {
                        Dictionary<string, string> dict = dictList[index];
                        int ID;
                        try
                        {
                            ID = Convert.ToInt32(dict["ID"]);
                        }
                        catch (Exception)
                        {
                            Trace.WriteLine(string.Format("{0}: Couldn't parse Team ID on line {1}. Skipping.", DateTime.Now, (index + 2)));
                            continue;
                        }
                        for (int i = 0; i < teamsList.Count; i++)
                        {
                            if (teamsList[i].ID == ID)
                            {
                                TeamEntry te = teamsList[i];
                                if (!TryParseTeamDictionaryList(ref te, dict))
                                {
                                    noErrors = false;
                                }
                                teamsList[i] = te;
                                break;
                            }
                        }
                    }

                    dgTeams.ItemsSource = null;
                    dgTeams.ItemsSource = teamsList;

                    updateStatus(noErrors
                                     ? "Data pasted into table successfully."
                                     : "Data pasted into table but with errors. See tracelog.txt for details.");
                }
                e.Handled = true;
            }
        }

        private bool TryParseTeamDictionaryList(ref TeamEntry te, Dictionary<string, string> dict)
        {
            bool noErrors = SmartPasting(te, dict, tColsToAvoid);

            int rCount = te.RosterSpots.Count;
            for (int i = 0; i < rCount; i++)
            {
                te.RosterSpots[i] = te.RosterSpots[i].TrySetValue(dict, "R" + (i + 1).ToString(), true);
            }

            return noErrors;
        }

        private void FixPlNumAndOrder()
        {
            foreach (TeamEntry t in teamsList)
            {
//if (i == teamsList.Count - 1) System.Diagnostics.Debugger.Break();

                int spotsCount = t.RosterSpots.Count;
                for (int j = 0; j < spotsCount; j++)
                {
                    int curCell = t.RosterSpots[j];
                    if (curCell != -1)
                        continue;
                    for (int k = spotsCount - 1; k > j; k--)
                    {
                        int subCell = t.RosterSpots[k];
                        if (subCell != -1) // || String.IsNullOrWhiteSpace(subCell)))
                        {
                            t.RosterSpots[j] = subCell;
                            t.RosterSpots[k] = -1;
                            break;
                        }
                    }
                }
                int count = 0;
                for (int j = 0; j < spotsCount; j++)
                {
                    int curCell = t.RosterSpots[j];
                    if (curCell != -1) // || String.IsNullOrWhiteSpace(curCell))
                    {
                        count++;
                    }
                    else
                    {
                        break;
                    }
                }
                t.PlNum = count;
            }

            updateStatus("Player order and PlNum entries fixed.");
        }

        private void btnSaveOptions_Click(object sender, RoutedEventArgs e)
        {
            foreach (Option o in optionsList)
            {
                SetRegistrySetting(o.Setting, o.Value);
            }
            ReloadEverything();

            updateStatus("Options saved.");
        }

        private void dgPlayers_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.V && Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                if (GetOption("DumbPasting").ToString() == "True")
                {
                    updateStatus(!GenericEventHandlers.OnExecutedPaste(sender, null)
                                     ? "Paste into table completed but with errors. Check tracelog.txt for details."
                                     : "Paste into table completed.");
                }
                else
                {
                    bool noErrors = true;

                    string text = Clipboard.GetText();
                    string[] lines = Tools.SplitLinesToArray(text);
                    string doPasteBy = GetOption("DoPlayersPasteBy").ToString();
                    if (!lines[0].StartsWith(string.Format("{0}\t", doPasteBy)) && !lines[0].Contains(string.Format("\t{0}\t", doPasteBy)))
                    {
                        MessageBox.Show("The pasted data must have the column headers in the first row.");
                        return;
                    }
                    List<Dictionary<string, string>> dictList = CSV.DictionaryListFromTSVString(text);

                    dgPlayers.CommitEdit();
                    dgPlayers.CancelEdit();

                    foreach (var dict in dictList)
                    {
                        var myDict = dict;
                        List<int> matchingIDs =
                            playersList.Where(pl => pl.GetType().GetProperty(doPasteBy).GetValue(pl, null).ToString() == myDict[doPasteBy])
                                       .Select(pl => pl.ID)
                                       .ToList();
                        foreach (int t in matchingIDs)
                        {
                            PlayerEntry pe = playersList[t];
                            if (!TryParsePlayerDictionaryList(ref pe, myDict))
                                noErrors = false;
                            playersList[t] = pe;
                        }
                    }
                    dgPlayers.ItemsSource = null;
                    dgPlayers.ItemsSource = playersList;
                    updateStatus(noErrors
                                     ? "Data pasted into table successfully."
                                     : "Data pasted into table but with errors. See tracelog.txt for details.");
                }

                e.Handled = true;
            }
        }

        private bool TryParsePlayerDictionaryList(ref PlayerEntry pe, Dictionary<string, string> dict)
        {
            bool noErrors = SmartPasting(pe, dict, pColsToAvoid);

            foreach (string rtName in rtNames)
            {
                var curInd = (byte) Enum.Parse(typeof (Rating), rtName.Substring(1));
                pe.Ratings[curInd] = pe.Ratings[curInd].TrySetValue(dict, rtName, true);
            }

            foreach (string tName in tNames)
            {
                var curInd = (byte) Enum.Parse(typeof (Tendency), tName.Substring(1));
                pe.Tendencies[curInd] = pe.Tendencies[curInd].TrySetValue(dict, tName, true);
            }

            foreach (string hsName in hsNames)
            {
                var curInd = (byte) Enum.Parse(typeof (HotSpot), hsName.Substring(2));
                pe.HotSpots[curInd] = pe.HotSpots[curInd].TrySetValue(dict, hsName, true);
            }

            foreach (string gName in gNames)
            {
                var curInd = (byte) Enum.Parse(typeof (Gear), gName.Substring(1));
                pe.Accessories[curInd] = pe.Accessories[curInd].TrySetValue(dict, gName, true);
            }

            for (int i = 0; i < 15; i++)
            {
                pe.DunkPackages[i] = pe.DunkPackages[i].TrySetValue(dict, "DunkPkg" + i, true);
            }

            for (int i = 0; i < 22; i++)
            {
                pe.SeasonStats[i] = pe.SeasonStats[i].TrySetValue(dict, "SeasonStats" + i, true);
            }
            pe.SSList[0] = pe.SSList[0].TrySetValue(dict, "SS1", true);
            pe.SSList[1] = pe.SSList[1].TrySetValue(dict, "SS2", true);
            pe.SSList[2] = pe.SSList[2].TrySetValue(dict, "SS3", true);
            pe.SSList[3] = pe.SSList[3].TrySetValue(dict, "SS4", true);
            pe.SSList[4] = pe.SSList[4].TrySetValue(dict, "SS5", true);

            return noErrors;
        }

        private bool SmartPasting<T>(T entry, Dictionary<string, string> dict, List<string> columnsToAvoid)
        {
            bool noErrors = true;
            foreach (string key in dict.Keys)
            {
                if (columnsToAvoid.Contains(key) || key.Contains("*") || key == "ID")
                    continue;

                PropertyInfo propertyInfo = typeof (T).GetProperty(key);
                Type pType = propertyInfo.PropertyType;
                object convertedValue;
                try
                {
                    convertedValue = Convert.ChangeType(dict[key], pType);
                }
                catch
                {
                    if (pType.BaseType != null)
                    {
                        if (pType.BaseType == typeof (Enum))
                        {
                            try
                            {
                                convertedValue = Enum.Parse(pType, dict[key]);
                            }
                            catch (ArgumentException)
                            {
                                Tools.WriteToTrace(String.Format("\"{0}\" is not a valid member of \"{1}\"", dict[key], pType.Name));
                                noErrors = false;
                                continue;
                            }
                        }
                        else
                        {
                            Tools.WriteToTrace(String.Format("\"{0}\" couldn't be converted to {1}", dict[key], pType.Name));
                            noErrors = false;
                            continue;
                        }
                    }
                    else
                    {
                        Tools.WriteToTrace(String.Format("\"{0}\" couldn't be converted to {1}", dict[key], pType.Name));
                        noErrors = false;
                        continue;
                    }
                }
                propertyInfo.SetValue(entry, convertedValue, null);
            }
            return noErrors;
        }

        private void dgPlayers_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            AnyDataGrid_MouseDoubleClick(sender, e);
        }

        private void btnSaveJerseys_Click(object sender, RoutedEventArgs e)
        {
            RosterReader brSave;
            using (brSave = new RosterReader(File.Open(currentFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
            {
                NonByteAlignedBinaryWriter bw;
                using (bw = new NonByteAlignedBinaryWriter(File.Open(currentFile, FileMode.Open, FileAccess.Write, FileShare.ReadWrite)))
                {
                    for (int i = 0; i < jerseysList.Count; i++)
                    {
                        JerseyEntry je = jerseysList[i];

                        brSave.MoveStreamToFirstJersey();
                        brSave.MoveStreamPosition(68*i, 0);
                        SyncBWwithBR(ref bw, brSave);

                        bw.WriteNonByteAlignedBits(
                            Convert.ToString((ushort) Enum.Parse(typeof (JerseyType), je.GID.ToString()), 2).PadLeft(16, '0'),
                            brSave.ReadBytes(3));
                        SyncBRwithBW(ref brSave, bw);

                        brSave.MoveStreamPosition(0, 6);
                        SyncBWwithBR(ref bw, brSave);

                        bw.WriteNonByteAlignedBits(
                            Convert.ToString((byte) Enum.Parse(typeof (NeckType), je.Neck.ToString()), 2).PadLeft(3, '0'),
                            brSave.ReadBytes(2));
                        SyncBRwithBW(ref brSave, bw);

                        brSave.MoveStreamPosition(0, 2);
                        SyncBWwithBR(ref bw, brSave);

                        bw.WriteNonByteAlignedBits(Convert.ToString((byte) Enum.Parse(typeof (SockColor), je.SockClr.ToString()), 2),
                                                   brSave.ReadBytes(1));
                        SyncBRwithBW(ref brSave, bw);

                        brSave.MoveStreamPosition(0, 4);
                        SyncBWwithBR(ref bw, brSave);
                        brSave.MoveStreamPosition(10, 0);
                        SyncBWwithBR(ref bw, brSave);

                        bw.WriteNonByteAlignedBits(
                            NonByteAlignedBinaryReader.ByteArrayToBitString(
                                Tools.HexStringToByteArray(JerseyEntry.JerseyNames[je.Name.ToString()])), brSave.ReadBytes(5));
                        SyncBRwithBW(ref brSave, bw);

                        brSave.MoveStreamPosition(3, 5);
                        SyncBWwithBR(ref bw, brSave);

                        bw.WriteNonByteAlignedBits(ConvertHexColorStringToBinaryString(je.TeamColor1.Substring(1)), brSave.ReadBytes(5));
                        SyncBRwithBW(ref brSave, bw);
                        bw.WriteNonByteAlignedBits(ConvertHexColorStringToBinaryString(je.TeamColor2.Substring(1)), brSave.ReadBytes(5));
                        SyncBRwithBW(ref brSave, bw);
                        bw.WriteNonByteAlignedBits(ConvertHexColorStringToBinaryString(je.TeamColor3.Substring(1)), brSave.ReadBytes(5));
                        SyncBRwithBW(ref brSave, bw);
                        bw.WriteNonByteAlignedBits(ConvertHexColorStringToBinaryString(je.TeamColor4.Substring(1)), brSave.ReadBytes(5));
                        SyncBRwithBW(ref brSave, bw);
                        bw.WriteNonByteAlignedBits(ConvertHexColorStringToBinaryString(je.TeamColor5.Substring(1)), brSave.ReadBytes(5));
                        SyncBRwithBW(ref brSave, bw);
                        bw.WriteNonByteAlignedBits(ConvertHexColorStringToBinaryString(je.TeamColor6.Substring(1)), brSave.ReadBytes(5));
                        SyncBRwithBW(ref brSave, bw);
                    }
                }
            }

            if (doCRC)
            {
                RecalculateCRC();
            }

            updateStatus("Jerseys saved.");
        }

        private void dgPlayers_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (dgPlayers.SelectedCells.Count == 1)
            {
                DataGridCellInfo dgci = dgPlayers.CurrentCell;
                if (colorColumns.Contains(dgci.Column.Header.ToString()))
                {
                    var c = new Color();
                    try
                    {
                        string s =
                            typeof (PlayerEntry).GetProperty(dgci.Column.Header.ToString())
                                                .GetValue(dgci.Item, null)
                                                .ToString()
                                                .Substring(1);
                        c.A = Convert.ToByte(s.Substring(0, 2), 16);
                        c.R = Convert.ToByte(s.Substring(2, 2), 16);
                        c.G = Convert.ToByte(s.Substring(4, 2), 16);
                        c.B = Convert.ToByte(s.Substring(6, 2), 16);
                    }
                    catch
                    {
                        return;
                    }
                    rctPlayerColor.Fill = new SolidColorBrush(c);
                }
                else
                {
                    rctPlayerColor.Fill = grdPlayers.Background;
                    if (teamColumns.Contains(dgci.Column.Header.ToString()))
                    {
                        int teamID =
                            Convert.ToInt32(
                                typeof (PlayerEntry).GetProperty(dgci.Column.Header.ToString().Replace("*", "")).GetValue(dgci.Item, null));
                        try
                        {
                            txbStatus.Text = "Currently selected team: " + teamsList.Single(te => te.ID == teamID).Name;
                        }
                        catch
                        {
                            txbStatus.Text = "Ready";
                        }
                    }
                    else
                    {
                        txbStatus.Text = "Ready";
                    }
                }
            }
        }

        private void dgJerseys_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.V && Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                if (GetOption("DumbPasting").ToString() == "True")
                {
                    updateStatus(!GenericEventHandlers.OnExecutedPaste(sender, null)
                                     ? "Paste into table completed but with errors. Check tracelog.txt for details."
                                     : "Paste into table completed.");
                }
                else
                {
                    bool noErrors = true;
                    string text = Clipboard.GetText();
                    string[] lines = Tools.SplitLinesToArray(text);
                    if (!lines[0].StartsWith("ID\t") && !lines[0].Contains("\tID\t"))
                    {
                        MessageBox.Show("The pasted data must have the column headers in the first row.");
                        return;
                    }
                    List<Dictionary<string, string>> dictList = CSV.DictionaryListFromTSVString(text);

                    dgJerseys.CommitEdit();
                    dgJerseys.CancelEdit();

                    for (int index = 0; index < dictList.Count; index++)
                    {
                        Dictionary<string, string> dict = dictList[index];
                        int ID;
                        try
                        {
                            ID = Convert.ToInt32(dict["ID"]);
                        }
                        catch (Exception)
                        {
                            Trace.WriteLine(string.Format("{0}: Couldn't parse Jersey ID on line {1}. Skipping.", DateTime.Now, (index + 2)));
                            continue;
                        }
                        for (int i = 0; i < jerseysList.Count; i++)
                        {
                            if (jerseysList[i].ID == ID)
                            {
                                JerseyEntry je = jerseysList[i];
                                if (!TryParseJerseyDictionaryList(ref je, dict))
                                {
                                    noErrors = false;
                                }
                                jerseysList[i] = je;
                                break;
                            }
                        }
                    }

                    dgJerseys.ItemsSource = null;
                    dgJerseys.ItemsSource = jerseysList;

                    updateStatus(noErrors
                                     ? "Data pasted into table successfully."
                                     : "Data pasted into table but with errors. See tracelog.txt for details.");
                }
            }
        }


        private bool TryParseJerseyDictionaryList(ref JerseyEntry je, Dictionary<string, string> dict)
        {
            return SmartPasting(je, dict, new List<string>());
        }

        private void dgJerseys_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            AnyDataGrid_MouseDoubleClick(sender, e);
        }

        private void AnyDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGrid && (e == null || e.MouseDevice.DirectlyOver is DataGridCell || e.MouseDevice.DirectlyOver is TextBox))
            {
                var grid = (DataGrid) sender;
                if (grid.CurrentItem != null)
                {
                    DataGridCellInfo dgci = grid.CurrentCell;

                    if (colorColumns.Contains(dgci.Column.Header.ToString()))
                    {
                        var c = new Color();
                        try
                        {
                            string s =
                                dgci.Item.GetType()
                                    .GetProperty(dgci.Column.Header.ToString())
                                    .GetValue(dgci.Item, null)
                                    .ToString()
                                    .Substring(1);
                            c.A = Convert.ToByte(s.Substring(0, 2), 16);
                            c.R = Convert.ToByte(s.Substring(2, 2), 16);
                            c.G = Convert.ToByte(s.Substring(4, 2), 16);
                            c.B = Convert.ToByte(s.Substring(6, 2), 16);
                        }
                        catch
                        {
                            return;
                        }
                        var cd = new ColorDialog(c);
                        if (cd.ShowDialog() == true)
                        {
                            dgci.Item.GetType()
                                .GetProperty(dgci.Column.Header.ToString())
                                .SetValue(dgci.Item, cd.SelectedColor.ToString(), null);
                        }
                    }
                }
            }
        }

        private void dgJerseys_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (dgJerseys.SelectedCells.Count == 1)
            {
                DataGridCellInfo dgci = dgJerseys.CurrentCell;
                if (colorColumns.Contains(dgci.Column.Header.ToString()))
                {
                    var c = new Color();
                    try
                    {
                        string s =
                            typeof (JerseyEntry).GetProperty(dgci.Column.Header.ToString())
                                                .GetValue(dgci.Item, null)
                                                .ToString()
                                                .Substring(1);
                        c.A = Convert.ToByte(s.Substring(0, 2), 16);
                        c.R = Convert.ToByte(s.Substring(2, 2), 16);
                        c.G = Convert.ToByte(s.Substring(4, 2), 16);
                        c.B = Convert.ToByte(s.Substring(6, 2), 16);
                    }
                    catch
                    {
                        return;
                    }
                    rctJerseyColor.Fill = new SolidColorBrush(c);
                }
                else
                {
                    rctJerseyColor.Fill = grdPlayers.Background;
                }
            }
        }

        private void AnyRect_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Equals(sender, rctJerseyColor))
            {
                if (dgJerseys.SelectedCells.Count == 1)
                {
                    if (colorColumns.Contains(dgJerseys.CurrentCell.Column.Header.ToString()))
                    {
                        AnyDataGrid_MouseDoubleClick(dgJerseys, null);
                        dgJerseys_SelectedCellsChanged(null, null);
                    }
                }
            }
            else if (Equals(sender, rctPlayerColor))
            {
                if (dgPlayers.SelectedCells.Count == 1)
                {
                    DataGridCellInfo dgci = dgPlayers.CurrentCell;
                    if (colorColumns.Contains(dgci.Column.Header.ToString()))
                    {
                        AnyDataGrid_MouseDoubleClick(dgPlayers, null);
                        dgPlayers_SelectedCellsChanged(null, null);
                    }
                }
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(currentFile))
                return;

            var sw = new SearchWindow();
            if (sw.ShowDialog() == true)
            {
                LastFindFilters = sw.FindFilters;
                LastReplaceFilters = sw.ReplaceFilters;
                foundIDList = new List<int>();
                if (sw.FindFilters.Count == 0 && sw.ReplaceFilters.Count == 0)
                {
                    ICollectionView filteredList = CollectionViewSource.GetDefaultView(playersList);
                    filteredList.Filter = null;
                    dgPlayers.ItemsSource = filteredList;
                }
                else if (sw.FilterFilters.Count == 0)
                {
                    FindAndReplace(sw);
                }
                else
                {
                    List<int> foundIDs = FindMatchingPlayers(sw);
                    ICollectionView filteredList = CollectionViewSource.GetDefaultView(playersList);
                    filteredList.Filter = o => foundIDs.Contains(((PlayerEntry) o).ID);
                    dgPlayers.ItemsSource = filteredList;
                }
            }
        }

        private void FindAndReplace(SearchWindow sw)
        {
            if (sw.FindFilters.Count > 0)
            {
                List<int> foundIDs = FindMatchingPlayers(sw);
                foundIDList = foundIDs;

                if (foundIDs.Count > 0)
                {
                    curFoundID = foundIDs[0];
                    dgPlayers.ScrollIntoView(playersList[foundIDs[0]]);
                    updateStatus(string.Format("Found player matching filters: ID {0} ({1} matching players found)", foundIDs[0],
                                               foundIDs.Count));

                    if (sw.ReplaceFilters.Count > 0)
                    {
                        foreach (string filter in sw.ReplaceFilters)
                        {
                            string[] parts = filter.Split(' ');
                            parts[2] = parts.Skip(2).Aggregate((p1, p2) => p1 + " " + p2);

                            bool isArray = false;
                            int index = -1;
                            string prop = parts[0].Replace("*", "");
                            if (Char.IsUpper(parts[0][1]))
                            {
                                switch (parts[0][0])
                                {
                                    case 'R':
                                        index = (byte) Enum.Parse(typeof (Rating), parts[0].Substring(1));
                                        prop = "Ratings";
                                        isArray = true;
                                        break;
                                    case 'T':
                                        index = (byte) Enum.Parse(typeof (Tendency), parts[0].Substring(1));
                                        prop = "Tendencies";
                                        isArray = true;
                                        break;
                                    case 'H':
                                        if (parts[0][1] == 'S' && Char.IsUpper(parts[0][2]))
                                        {
                                            index = (byte) Enum.Parse(typeof (HotSpot), parts[0].Substring(2));
                                            prop = "HotSpots";
                                            isArray = true;
                                        }
                                        break;
                                    case 'S':
                                        if (parts[0][1] == 'S' && Char.IsNumber(parts[0][2]))
                                        {
                                            index = Convert.ToInt32(parts[0][2].ToString()) - 1;
                                            prop = "SSList";
                                            isArray = true;
                                        }
                                        break;
                                    case 'G':
                                        index = (byte) Enum.Parse(typeof (Gear), parts[0].Substring(1));
                                        prop = "Accessories";
                                        isArray = true;
                                        break;
                                }
                            }
                            double test;
                            bool isNumeric = double.TryParse(parts[2], out test);
                            double toReplaceD = isNumeric ? Convert.ToDouble(parts[2]) : double.NaN;
                            string toReplaceS = parts[2];
                            object value = isNumeric ? (object) toReplaceD : toReplaceS;

                            foreach (int id in foundIDs)
                            {
                                PropertyInfo propertyInfo = typeof (PlayerEntry).GetProperty(prop);
                                if (isArray)
                                {
                                    var p = propertyInfo.GetValue(playersList[id], null) as IList;
                                    if (p == null)
                                    {
                                        continue;
                                    }

                                    Type type = p.GetType().GetGenericArguments()[0];
                                    try
                                    {
                                        //var conversionType = propertyInfo.PropertyType;
                                        if (!type.IsEnum)
                                        {
                                            double curVal = Convert.ToDouble(p[index]);
                                            if (isNumeric && parts[1] != "=")
                                            {
                                                switch (parts[1])
                                                {
                                                    case "+=":
                                                        value = curVal + toReplaceD;
                                                        break;
                                                    case "-=":
                                                        value = curVal - toReplaceD;
                                                        break;
                                                    case "*=":
                                                        value = curVal*toReplaceD;
                                                        break;
                                                    case "/=":
                                                        value = curVal/toReplaceD;
                                                        break;
                                                }
                                            }
                                            else if (parts[1].Contains("Rand"))
                                            {
                                                string[] limits = parts[2].Split(';');
                                                int rndVal = random.Next(Convert.ToInt32(limits[0]), Convert.ToInt32(limits[1]));
                                                switch (parts[1].Substring(0, 2))
                                                {
                                                    case "=R":
                                                        value = rndVal;
                                                        break;
                                                    case "+=":
                                                        value = curVal + rndVal;
                                                        break;
                                                    case "-=":
                                                        value = curVal - rndVal;
                                                        break;
                                                    case "*=":
                                                        value = curVal*rndVal;
                                                        break;
                                                    case "/=":
                                                        value = curVal/rndVal;
                                                        break;
                                                }
                                            }
                                            p[index] = Convert.ChangeType(value, type);
                                        }
                                        else
                                        {
                                            if (parts[1] == "=Rand")
                                            {
                                                string[] limits = parts[2].Split(';');
                                                toReplaceS =
                                                    random.Next(Convert.ToInt32(limits[0]), Convert.ToInt32(limits[1])).ToString();
                                            }
                                            p[index] = Enum.Parse(type, toReplaceS);
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        MessageBox.Show(String.Format("{0} is not a valid value for {1}. Replace cancelled.", toReplaceS,
                                                                      prop));
                                        return;
                                    }
                                    propertyInfo.SetValue(playersList[id], p, null);
                                }
                                else
                                {
                                    Type conversionType = propertyInfo.PropertyType;
                                    if (!conversionType.IsEnum)
                                    {
                                        double curVal = double.NaN;
                                        try
                                        {
                                            curVal = Convert.ToDouble(propertyInfo.GetValue(playersList[id], null));
                                        }
                                        catch (FormatException)
                                        {
                                            Console.WriteLine("Couldn't convert current value " + value + " to double.");
                                        }
                                        if (isNumeric && parts[1] != "=")
                                        {
                                            switch (parts[1])
                                            {
                                                case "+=":
                                                    value = curVal + toReplaceD;
                                                    break;
                                                case "-=":
                                                    value = curVal - toReplaceD;
                                                    break;
                                                case "*=":
                                                    value = curVal*toReplaceD;
                                                    break;
                                                case "/=":
                                                    value = curVal/toReplaceD;
                                                    break;
                                            }
                                        }
                                        else if (parts[1].Contains("Rand"))
                                        {
                                            string[] limits = parts[2].Split(';');
                                            int rndVal = random.Next(Convert.ToInt32(limits[0]), Convert.ToInt32(limits[1]));
                                            switch (parts[1].Substring(0, 2))
                                            {
                                                case "=R":
                                                    value = rndVal;
                                                    break;
                                                case "+=":
                                                    value = curVal + rndVal;
                                                    break;
                                                case "-=":
                                                    value = curVal - rndVal;
                                                    break;
                                                case "*=":
                                                    value = curVal*rndVal;
                                                    break;
                                                case "/=":
                                                    value = curVal/rndVal;
                                                    break;
                                            }
                                        }
                                        propertyInfo.SetValue(playersList[id], Convert.ChangeType(value, conversionType), null);
                                    }
                                    else
                                    {
                                        if (parts[1] == "=Rand")
                                        {
                                            string[] limits = parts[2].Split(';');
                                            toReplaceS = random.Next(Convert.ToInt32(limits[0]), Convert.ToInt32(limits[1])).ToString();
                                        }
                                        propertyInfo.SetValue(playersList[id],
                                                              Enum.ToObject(conversionType, Enum.Parse(conversionType, toReplaceS)), null);
                                    }
                                }
                            }
                        }
                        dgPlayers.ItemsSource = null;
                        dgPlayers.ItemsSource = playersList;
                        updateStatus(String.Format("{0} players successfully replaced!", foundIDs.Count));
                    }
                }
                else
                {
                    updateStatus("No matching players found");
                }
            }
        }

        private List<int> FindMatchingPlayers(SearchWindow sw)
        {
            var foundIDs = new List<int>();
            for (int i = 0; i <= Convert.ToInt32(GetOption("LastPlayerID")); i++)
            {
                foundIDs.Add(i);
            }

            foreach (string filter in sw.FindFilters)
            {
                string[] parts = filter.Split(' ');
                parts[2] = parts.Skip(2).Aggregate((p1, p2) => p1 + " " + p2);
                var newFoundIDs = new List<int>();

                bool isArray = false;
                int index = -1;
                string prop = parts[0].Replace("*", "");
                if (Char.IsUpper(parts[0][1]))
                {
                    switch (parts[0][0])
                    {
                        case 'R':
                            index = (byte) Enum.Parse(typeof (Rating), parts[0].Substring(1));
                            prop = "Ratings";
                            isArray = true;
                            break;
                        case 'T':
                            index = (byte) Enum.Parse(typeof (Tendency), parts[0].Substring(1));
                            prop = "Tendencies";
                            isArray = true;
                            break;
                        case 'H':
                            if (parts[0][1] == 'S' && Char.IsUpper(parts[0][2]))
                            {
                                index = (byte) Enum.Parse(typeof (HotSpot), parts[0].Substring(1));
                                prop = "HotSpots";
                                isArray = true;
                            }
                            break;
                        case 'S':
                            if (parts[0][1] == 'S' && Char.IsNumber(parts[0][2]))
                            {
                                index = Convert.ToInt32(parts[0][2].ToString()) - 1;
                                prop = "SSList";
                                isArray = true;
                            }
                            break;
                    }
                }
                foreach (int id in foundIDs)
                {
                    double test;
                    bool isNumeric = double.TryParse(parts[2], out test);
                    object val;

                    if (isArray)
                    {
                        var p = typeof (PlayerEntry).GetProperty(prop).GetValue(playersList[id], null) as IList;
                        if (isNumeric)
                        {
                            val = Convert.ToDouble(p[index]);
                        }
                        else
                        {
                            val = p[index].ToString();
                        }
                    }
                    else
                    {
                        if (isNumeric)
                        {
                            val = Convert.ToDouble(typeof (PlayerEntry).GetProperty(prop).GetValue(playersList[id], null));
                        }
                        else
                        {
                            val = typeof (PlayerEntry).GetProperty(prop).GetValue(playersList[id], null).ToString();
                        }
                    }
                    object toFind = isNumeric ? (object) Convert.ToDouble(parts[2]) : parts[2];
                    if (isNumeric)
                    {
                        var valD = (double) val;
                        var toFindD = (double) toFind;
                        switch (parts[1])
                        {
                            case "=":
                                if (valD == toFindD)
                                {
                                    newFoundIDs.Add(id);
                                }
                                break;
                            case "<":
                                if (valD < toFindD)
                                {
                                    newFoundIDs.Add(id);
                                }
                                break;
                            case "<=":
                                if (valD <= toFindD)
                                {
                                    newFoundIDs.Add(id);
                                }
                                break;
                            case ">":
                                if (valD > toFindD)
                                {
                                    newFoundIDs.Add(id);
                                }
                                break;
                            case ">=":
                                if (valD >= toFindD)
                                {
                                    newFoundIDs.Add(id);
                                }
                                break;
                        }
                    }
                    else
                    {
                        var valS = (string) val;
                        var toFindS = (string) toFind;
                        switch (parts[1])
                        {
                            case "=":
                                if (valS.Equals(toFindS))
                                {
                                    newFoundIDs.Add(id);
                                }
                                break;
                            case "Contains(Text)":
                                if (valS.Contains(toFindS))
                                {
                                    newFoundIDs.Add(id);
                                }
                                break;
                        }
                    }
                }

                foundIDs = new List<int>(newFoundIDs);
            }
            return foundIDs;
        }

        private void btnFindNext_Click(object sender, RoutedEventArgs e)
        {
            if (foundIDList.Count > 0)
            {
                int curIndex = foundIDList.IndexOf(curFoundID);
                if (curIndex < foundIDList.Count - 1)
                {
                    curIndex++;
                }
                else
                {
                    curIndex = 0;
                }
                curFoundID = foundIDList[curIndex];
                dgPlayers.ScrollIntoView(playersList[curFoundID]);
                updateStatus(string.Format("Next player matching filters: ID {0} ({2}/{1})", foundIDList[curIndex], foundIDList.Count,
                                           curIndex + 1));
            }
        }

        private void btnReadme_Click(object sender, RoutedEventArgs e)
        {
            File.Copy(AppPath + @"\README", AppPath + @"\Readme.txt", true);
            Process.Start(AppPath + @"\Readme.txt");
        }

        private void btnEnumerables_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(AppPath + @"\Enumerables.txt");
        }

        /// <summary>
        ///     Checks for software updates asynchronously.
        /// </summary>
        /// <param name="showMessage">
        ///     if set to <c>true</c>, a message will be shown even if no update is found.
        /// </param>
        public static void CheckForUpdates(bool showMessage = false)
        {
            //showUpdateMessage = showMessage;
            try
            {
                var webClient = new WebClient();
                string updateUri = "http://users.tellas.gr/~aslan16/re13version.txt";
                if (!showMessage)
                {
                    webClient.DownloadFileCompleted += CheckForUpdatesCompleted;
                    webClient.DownloadFileAsync(new Uri(updateUri), DocsPath + @"re13version.txt");
                }
                else
                {
                    webClient.DownloadFile(new Uri(updateUri), DocsPath + @"re13version.txt");
                    CheckForUpdatesCompleted(null, null);
                }
            }
            catch
            {
            }
        }

        /// <summary>
        ///     Checks the downloaded version file to see if there's a newer version, and displays a message if needed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">
        ///     The <see cref="AsyncCompletedEventArgs" /> instance containing the event data.
        /// </param>
        private static void CheckForUpdatesCompleted(object sender, AsyncCompletedEventArgs e)
        {
            string[] updateInfo;
            string[] versionParts;
            try
            {
                updateInfo = File.ReadAllLines(DocsPath + @"re13version.txt");
                versionParts = updateInfo[0].Split('.');
            }
            catch
            {
                return;
            }
            string[] curVersionParts = Assembly.GetExecutingAssembly().GetName().Version.ToString().Split('.');
            var iVP = new int[versionParts.Length];
            var iCVP = new int[versionParts.Length];
            for (int i = 0; i < versionParts.Length; i++)
            {
                iVP[i] = Convert.ToInt32(versionParts[i]);
                iCVP[i] = Convert.ToInt32(curVersionParts[i]);
                if (iCVP[i] > iVP[i])
                    break;
                if (iVP[i] > iCVP[i])
                {
                    string changelog = "\n\nVersion " + String.Join(".", versionParts);
                    try
                    {
                        for (int j = 2; j < updateInfo.Length; j++)
                        {
                            changelog += "\n" + updateInfo[j].Replace('\t', ' ');
                        }
                    }
                    catch
                    {
                    }
                    MessageBoxResult mbr = MessageBox.Show("A new version is available! Would you like to download it?" + changelog,
                                                           "NBA 2K13 Roster Editor", MessageBoxButton.YesNo, MessageBoxImage.Information);
                    if (mbr == MessageBoxResult.Yes)
                    {
                        Process.Start(updateInfo[1]);
                        break;
                    }
                    return;
                }
            }
            /*
            if (showUpdateMessage)
                MessageBox.Show("No updates found!");
            */
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
#if !DEBUG
            tabPlayerStats.Visibility = Visibility.Collapsed;
            #endif

            if (GetOption("CheckForUpdates").ToString() == "True")
            {
                var w = new BackgroundWorker();
                w.DoWork += w_DoWork;
                w.RunWorkerAsync();
            }
        }

        private void w_DoWork(object sender, DoWorkEventArgs e)
        {
            CheckForUpdates();
        }

        private void btnBatchEdit_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog
                      {
                          InitialDirectory = Path.GetFullPath(DocsPath + @"\Search Filters"),
                          Filter = "Roster Editor Search Filters (*.rsf)|*.rsf",
                          DefaultExt = "rsf",
                          Title = "Select a file from the folder containing the replace filters you want to apply"
                      };

            ofd.ShowDialog();

            if (String.IsNullOrWhiteSpace(ofd.FileName))
                return;

            List<string> files = Directory.GetFiles(Path.GetDirectoryName(ofd.FileName)).ToList();
            files.Sort();

            foreach (string file in files)
            {
                var sw = new SearchWindow();
                sw.LoadFilters(file);
                sw.AddFiltersToFields();

                FindAndReplace(sw);
                sw.Close();
            }
        }

        private void btnModePCNov_Checked(object sender, RoutedEventArgs e)
        {
            SetRegistrySetting("Mode", "PCNov10");
            mode = Mode.PCNov10;
            chkRecalculateCRC.IsChecked = true;

            SetRegistrySetting("FirstSSOffset", 40916);
            SetRegistrySetting("FirstSSOffsetBit", 2);
            try
            {
                ReloadEverything();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void btnModeCustom_Checked(object sender, RoutedEventArgs e)
        {
            SetRegistrySetting("Mode", "Custom");
            mode = Mode.Custom;
            chkRecalculateCRC.IsChecked = true;

            SetRegistrySetting("CustomSSOffset", 40916);
            SetRegistrySetting("CustomSSOffsetBit", 2);
            try
            {
                ReloadEverything();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void btnResetOptions_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult r = MessageBox.Show(
                "Are you sure you want to reset all options to their default values? This can't be undone.", "NBA 2K13 Roster Editor",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (r != MessageBoxResult.Yes)
                return;

            RegistryKey rk = Registry.CurrentUser;
            try
            {
                try
                {
                    rk.DeleteSubKeyTree(@"SOFTWARE\Lefteris Aslanoglou\NBA 2K13 Roster Editor");
                }
                catch (Exception)
                {
                }
                ReloadOptions();
            }
            catch
            {
                MessageBox.Show("Couldn't save changed setting.");
            }
        }

        private void btnMode360Nov10_Checked(object sender, RoutedEventArgs e)
        {
            SetRegistrySetting("Mode", "X360Nov10");
            mode = Mode.X360Nov10;
            chkRecalculateCRC.IsChecked = false;

            SetRegistrySetting("FirstSSOffset", 94164);
            SetRegistrySetting("FirstSSOffsetBit", 2);
            try
            {
                ReloadEverything();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void btnModeCustom360_Checked(object sender, RoutedEventArgs e)
        {
            SetRegistrySetting("Mode", "CustomX360");
            mode = Mode.CustomX360;
            chkRecalculateCRC.IsChecked = false;

            SetRegistrySetting("CustomSSOffset", 94164);
            SetRegistrySetting("CustomSSOffsetBit", 2);
            try
            {
                ReloadEverything();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void dgPlaybooks_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.V && Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                updateStatus(!GenericEventHandlers.OnExecutedPaste(sender, null)
                                 ? "Paste into table completed but with errors. Check tracelog.txt for details."
                                 : "Paste into table completed.");
            }
        }

        private void btnSavePlaybooks_Click(object sender, RoutedEventArgs e)
        {
            RosterReader brSave;
            using (brSave = new RosterReader(File.Open(currentFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
            {
                NonByteAlignedBinaryWriter bw;
                using (bw = new NonByteAlignedBinaryWriter(File.Open(currentFile, FileMode.Open, FileAccess.Write, FileShare.ReadWrite)))
                {
                    for (int i = 0; i < playbooksList.Count; i++)
                    {
                        PlaybookEntry pb = playbooksList[i];

                        brSave.MoveStreamToPlaybook(i);
                        SyncBWwithBR(ref bw, brSave);

                        for (int j = 0; j < 50; j++)
                        {
                            bw.WriteNonByteAlignedBits(
                                NonByteAlignedBinaryReader.ByteArrayToBitString(Tools.HexStringToByteArray(pb.Plays[j])),
                                brSave.ReadBytes(5));
                            SyncBRwithBW(ref brSave, bw);
                        }
                    }
                }
            }

            if (doCRC)
            {
                RecalculateCRC();
            }

            updateStatus("Playbooks saved.");
        }

        private void btnSaveStaff_Click(object sender, RoutedEventArgs e)
        {
            RosterReader brSave;
            using (brSave = new RosterReader(File.Open(currentFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
            {
                NonByteAlignedBinaryWriter bw;
                using (bw = new NonByteAlignedBinaryWriter(File.Open(currentFile, FileMode.Open, FileAccess.Write, FileShare.ReadWrite)))
                {
                    for (int i = 0; i < staffList.Count; i++)
                    {
                        StaffEntry se = staffList[i];

                        brSave.MoveStreamToStaffPlaybookID(i);
                        SyncBWwithBR(ref bw, brSave);
                        bw.WriteNonByteAlignedBits(Convert.ToString(se.PlaybookID, 2).PadLeft(7, '0'), brSave.ReadBytes(2));
                        SyncBRwithBW(ref brSave, bw);

                        brSave.MoveStreamToStaffPlaybookID(i);
                        brSave.MoveStreamPosition(6, 2);
                        SyncBWwithBR(ref bw, brSave);
                        bw.WriteNonByteAlignedBits(Convert.ToString(se.CFID, 2).PadLeft(13, '0'), brSave.ReadBytes(3));
                        SyncBRwithBW(ref brSave, bw);
                        brSave.MoveStreamPosition(1, 0);
                        SyncBWwithBR(ref bw, brSave);
                        bw.WriteNonByteAlignedBits(Convert.ToString(se.PortraitID, 2).PadLeft(13, '0'), brSave.ReadBytes(3));
                        SyncBRwithBW(ref brSave, bw);

                        brSave.MoveStreamToStaffPlaybookID(i);
                        brSave.MoveStreamPosition(-24, 0);
                        SyncBWwithBR(ref bw, brSave);
                        for (int j = 0; j < 21; j++)
                        {
                            bw.WriteNonByteAlignedBits(Convert.ToString(se.CoachingProfile[j], 2).PadLeft(8, '0'), brSave.ReadBytes(2));
                            SyncBRwithBW(ref brSave, bw);
                        }
                    }
                }
            }

            if (doCRC)
            {
                RecalculateCRC();
            }

            updateStatus("Staff saved.");
        }

        private void dgStaff_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.V && Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                updateStatus(!GenericEventHandlers.OnExecutedPaste(sender, null)
                                 ? "Paste into table completed but with errors. Check tracelog.txt for details."
                                 : "Paste into table completed.");
            }
        }

        private void btnResetPlaybooks_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 30; i++)
            {
                staffList[teamsList[i].StHeadCoach].PlaybookID = i;
            }
            btnSaveStaff_Click(null, null);
            PopulateStaffTab();
            PopulateTeamsTab();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            expCount = Convert.ToInt32(txtExpCount.Text);
            PopulatePlayerStatsTab();
        }

        private void btnSaveTeamStats_Click(object sender, RoutedEventArgs e)
        {
            RosterReader brSave;
            using (brSave = new RosterReader(File.Open(currentFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
            {
                NonByteAlignedBinaryWriter bw;
                using (bw = new NonByteAlignedBinaryWriter(File.Open(currentFile, FileMode.Open, FileAccess.Write, FileShare.ReadWrite)))
                {
                    for (int i = 0; i < teamStatsList.Count; i++)
                    {
                        TeamStatsEntry tse = teamStatsList[i];

                        brSave.MoveStreamToTeamStats(i);
                        SyncBWwithBR(ref bw, brSave);

                        bw.WriteNonByteAlignedByte(tse.Wins, brSave.ReadBytes(2));
                        SyncBRwithBW(ref brSave, bw);
                        bw.WriteNonByteAlignedByte(tse.Losses, brSave.ReadBytes(2));
                        SyncBRwithBW(ref brSave, bw);
                        WriteUInt16(bw, tse.MINS, 16, ref brSave);
                        WriteUInt16(bw, tse.PF, 16, ref brSave);
                        WriteUInt16(bw, tse.PA, 16, ref brSave);
                        brSave.MoveStreamPosition(2, 0);
                        SyncBWwithBR(ref bw, brSave);
                        WriteUInt16(bw, tse.FGM, 16, ref brSave);
                        WriteUInt16(bw, tse.FGA, 16, ref brSave);
                        WriteUInt16(bw, tse.TPM, 16, ref brSave);
                        WriteUInt16(bw, tse.TPA, 16, ref brSave);
                        WriteUInt16(bw, tse.FTM, 16, ref brSave);
                        WriteUInt16(bw, tse.FTA, 16, ref brSave);
                        WriteUInt16(bw, tse.OREB, 16, ref brSave);
                        WriteUInt16(bw, tse.DREB, 16, ref brSave);
                        WriteUInt16(bw, tse.STL, 16, ref brSave);
                        WriteUInt16(bw, tse.TOS, 16, ref brSave);
                        WriteUInt16(bw, tse.BLK, 16, ref brSave);
                        WriteUInt16(bw, tse.AST, 16, ref brSave);
                        WriteUInt16(bw, tse.FOUL, 16, ref brSave);
                    }
                }
            }

            if (doCRC)
            {
                RecalculateCRC();
            }

            updateStatus("Team Stats saved.");
        }

        private void WriteByte(byte toWrite, int bits, NonByteAlignedBinaryWriter bw, ref RosterReader brSave)
        {
            bw.WriteNonByteAlignedBits(Convert.ToString(toWrite, 2).PadLeft(bits, '0'),
                                       brSave.ReadBytes(Convert.ToInt32(Math.Floor(1.865 + 0.1282*bits))));
            SyncBRwithBW(ref brSave, bw);
        }

        private void WriteInt32(NonByteAlignedBinaryWriter bw, Int32 toWrite, int bits, ref RosterReader brSave)
        {
            bw.WriteNonByteAlignedBits(Convert.ToString(toWrite, 2).PadLeft(bits, '0'),
                                       brSave.ReadBytes(Convert.ToInt32(Math.Floor(1.865 + 0.1282*bits))));
            SyncBRwithBW(ref brSave, bw);
        }

        private void WriteUInt16(NonByteAlignedBinaryWriter bw, UInt16 toWrite, int bits, ref RosterReader brSave)
        {
            bw.WriteNonByteAlignedBits(Convert.ToString(toWrite, 2).PadLeft(bits, '0'),
                                       brSave.ReadBytes(Convert.ToInt32(Math.Floor(1.865 + 0.1282*bits))));
            SyncBRwithBW(ref brSave, bw);
        }

        private void dgTeams_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (dgTeams.SelectedCells.Count == 1 && dgTeams.SelectedCells[0].Column.Header.ToString().StartsWith("R"))
            {
                try
                {
                    int playerID =
                        ((TeamEntry) dgTeams.CurrentCell.Item).RosterSpots[
                            Convert.ToInt32(dgTeams.CurrentCell.Column.Header.ToString().Substring(1)) - 1];
                    txbStatus.Text = "Currently selected player: " + playersList.Single(p => p.ID == playerID).Name;
                }
                catch
                {
                    txbStatus.Text = "Ready";
                }
            }
            else
            {
                txbStatus.Text = "Ready";
            }
        }

        private void FindAllOffsets()
        {
            if (String.IsNullOrWhiteSpace(currentFile))
                return;

            var bgwrk = new BackgroundWorker();

            lstFOResults.Items.Clear();
            btnFOSearch.IsEnabled = false;
            btnFindAllOffsets.IsEnabled = false;
            tbcMain.SelectedItem = tabFindOffsets;
            tbcMain.IsEnabled = false;

            RosterReader br = null;
            bool found = false;
            var list = new List<OffsetThingy>
                       {
                           new OffsetThingy("CustomRosterOffset", "657F9098010A0000", 8, 0, 860000, 0),
                           new OffsetThingy("CustomJerseyOffset", "0000020D0440096E", -4080, 0, 1486000, 0),
                           new OffsetThingy("CustomPlaybookOffset", "BEF53E7D", 0, 0, 1099000, 0),
                           new OffsetThingy("CustomStaffPlaybookIDOffset", "7e013bf0", -7, 0, 991000, 0),
                           new OffsetThingy("CustomTeamStatsOffset", "231F0C6F", 0, 0, 1434000, 0)
                       };
            bgwrk.DoWork += delegate
                            {
                                br = new RosterReader(new MemoryStream(File.ReadAllBytes(currentFile)));

                                for (int i = 0; i < list.Count; i++)
                                {
                                    var offsetThingy = list[i];
                                    if (offsetThingy.Option == "CustomTeamStatsOffset" && saveType == SaveType.MyCareer)
                                    {
                                        continue;
                                    }
                                    br.BaseStream.Position = offsetThingy.InitialPosition;
                                    br.InBytePosition = offsetThingy.InitialInBytePosition;
                                    br.MoveStreamForSaveType();
                                    string s = offsetThingy.S;
                                    char[] ca = s.ToUpperInvariant().ToCharArray();
                                    string valid = "0123456789ABCDEF";
                                    foreach (char c in ca)
                                    {
                                        if (!valid.Contains(c))
                                        {
                                            MessageBox.Show("Hex string contains invalid character \"" + c + "\"");
                                            return;
                                        }
                                    }

                                    found = true;
                                    byte s1;
                                    byte s2 = Convert.ToByte(s.Substring(0, 2), 16);
                                    byte[] sba = Tools.HexStringToByteArray(s);
                                    while (true)
                                    {
                                        if (br.BaseStream.Position%500000 == 0 && br.InBytePosition == 0)
                                        {
                                            bgwrk.ReportProgress(Convert.ToInt32(br.BaseStream.Position*100/br.BaseStream.Length));
                                        }
                                        s1 = br.ReadNonByteAlignedByte();
                                        //Console.WriteLine("Compared {0} to {1} (at {2} +{3})", s1, s2, br.BaseStream.Position - 1, br.InBytePosition);
                                        while (s1 != s2)
                                        {
                                            br.MoveStreamPosition(0, -7);
                                            if (br.BaseStream.Length - br.BaseStream.Position == 1 && br.InBytePosition > 0)
                                            {
                                                found = false;
                                                break;
                                            }
                                            if (br.BaseStream.Position%500000 == 0 && br.InBytePosition == 0)
                                            {
                                                bgwrk.ReportProgress(Convert.ToInt32(br.BaseStream.Position*100/br.BaseStream.Length));
                                            }
                                            s1 = br.ReadNonByteAlignedByte();
                                            //Console.WriteLine("Compared {0} to {1} (at {2} +{3})", s1, s2, br.BaseStream.Position - 1, br.InBytePosition);
                                        }

                                        if (!found)
                                        {
                                            break;
                                        }

                                        br.BaseStream.Position--;
                                        long distanceFromEnd = br.BaseStream.Length - br.BaseStream.Position;
                                        if (distanceFromEnd < s.Length/2 || (distanceFromEnd == s.Length/2 && br.InBytePosition > 0))
                                        {
                                            found = false;
                                            break;
                                        }
                                        if (br.ReadNonByteAlignedBytes(s.Length/2).SequenceEqual(sba))
                                        {
                                            offsetThingy.FinalPosition = br.BaseStream.Position;
                                            offsetThingy.FinalInBytePosition = br.InBytePosition;
                                            bgwrk.ReportProgress(0, offsetThingy);
                                            break;
                                        }
                                        else
                                        {
                                            //Console.Write("Was at {0} +{1}, ", br.BaseStream.Position, br.InBytePosition);
                                            br.MoveStreamPosition(0 - (s.Length/2), 1);
                                            //Console.WriteLine("now at {0} +{1}.", br.BaseStream.Position, br.InBytePosition);
                                        }
                                    }
                                }
                                found = true;
                            };

            bgwrk.ProgressChanged += delegate(object o, ProgressChangedEventArgs args)
                                     {
                                         if (args.UserState == null)
                                         {
                                             lstFOResults.Items.Add(String.Format("Searching ({0}%)...", args.ProgressPercentage));
                                         }
                                         else
                                         {
                                             var offsetThingy = ((OffsetThingy) args.UserState);
                                             optionsList.Single(opt => opt.Setting == offsetThingy.Option).Value =
                                                 offsetThingy.FinalPosition - (offsetThingy.S.Length/2) + offsetThingy.BytesToMove -
                                                 RosterReader.SaveTypeOffset(saveType);
                                             optionsList.Single(opt => opt.Setting == offsetThingy.Option + "Bit").Value =
                                                 offsetThingy.FinalInBytePosition + offsetThingy.BitsToMove;
                                             lstFOResults.Items.Add("Found " + offsetThingy.Option);
                                         }
                                     };

            bgwrk.RunWorkerCompleted += delegate
                                        {
                                            lstFOResults.Items.Add(!found
                                                                       ? "Hex string not found after last occurrence, if any."
                                                                       : "Done! Select the Custom profile to edit the roster!");
                                            btnFOSearch.IsEnabled = true;
                                            btnFindAllOffsets.IsEnabled = true;
                                            btnSaveOptions_Click(null, null);
                                            tbcMain.SelectedItem = tabPlayers;
                                            tbcMain.IsEnabled = true;
                                            br.Close();
                                        };

            bgwrk.WorkerReportsProgress = true;
            bgwrk.RunWorkerAsync();
        }

        private void btnFOSearch_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(currentFile) || String.IsNullOrWhiteSpace(txtFOHex.Text))
                return;

            var bgwrk = new BackgroundWorker();

            lstFOResults.Items.Clear();
            btnFOSearch.IsEnabled = false;

            NonByteAlignedBinaryReader br = null;
            bool found = false;
            string s = txtFOHex.Text.ToUpperInvariant();
            bgwrk.DoWork += delegate
                            {
                                br = new NonByteAlignedBinaryReader(new MemoryStream(File.ReadAllBytes(currentFile)));

                                char[] ca = s.ToCharArray();
                                string valid = "0123456789ABCDEF";
                                foreach (char c in ca)
                                {
                                    if (!valid.Contains(c))
                                    {
                                        MessageBox.Show("Hex string contains invalid character \"" + c + "\"");
                                        return;
                                    }
                                }

                                found = true;
                                byte s1;
                                byte s2 = Convert.ToByte(s.Substring(0, 2), 16);
                                byte[] sba = Tools.HexStringToByteArray(s);
                                while (true)
                                {
                                    if (br.BaseStream.Position%500000 == 0 && br.InBytePosition == 0)
                                    {
                                        bgwrk.ReportProgress(Convert.ToInt32(br.BaseStream.Position*100/br.BaseStream.Length));
                                    }
                                    s1 = br.ReadNonByteAlignedByte();
                                    //Console.WriteLine("Compared {0} to {1} (at {2} +{3})", s1, s2, br.BaseStream.Position - 1, br.InBytePosition);
                                    while (s1 != s2)
                                    {
                                        br.MoveStreamPosition(0, -7);
                                        if (br.BaseStream.Length - br.BaseStream.Position == 1 && br.InBytePosition > 0)
                                        {
                                            found = false;
                                            break;
                                        }
                                        if (br.BaseStream.Position%500000 == 0 && br.InBytePosition == 0)
                                        {
                                            bgwrk.ReportProgress(Convert.ToInt32(br.BaseStream.Position*100/br.BaseStream.Length));
                                        }
                                        s1 = br.ReadNonByteAlignedByte();
                                        //Console.WriteLine("Compared {0} to {1} (at {2} +{3})", s1, s2, br.BaseStream.Position - 1, br.InBytePosition);
                                    }

                                    if (!found)
                                    {
                                        break;
                                    }

                                    br.BaseStream.Position--;
                                    long distanceFromEnd = br.BaseStream.Length - br.BaseStream.Position;
                                    if (distanceFromEnd < s.Length/2 || (distanceFromEnd == s.Length/2 && br.InBytePosition > 0))
                                    {
                                        found = false;
                                        break;
                                    }
                                    if (br.ReadNonByteAlignedBytes(s.Length/2).SequenceEqual(sba))
                                    {
                                        bgwrk.ReportProgress(0,
                                                             String.Format("Found at {0} +{1}!", (br.BaseStream.Position - (s.Length/2)),
                                                                           br.InBytePosition));
                                    }
                                    else
                                    {
                                        //Console.Write("Was at {0} +{1}, ", br.BaseStream.Position, br.InBytePosition);
                                        br.MoveStreamPosition(0 - (s.Length/2), 1);
                                        //Console.WriteLine("now at {0} +{1}.", br.BaseStream.Position, br.InBytePosition);
                                    }
                                }
                            };

            bgwrk.ProgressChanged += (o, args) =>
                lstFOResults.Items.Add(args.UserState == null
                                           ? String.Format("Searching ({0}%)...", args.ProgressPercentage)
                                           : args.UserState.ToString());

            bgwrk.RunWorkerCompleted += delegate
                                        {
                                            if (!found)
                                            {
                                                lstFOResults.Items.Add("Hex string not found after last occurrence, if any.");
                                            }
                                            btnFOSearch.IsEnabled = true;
                                            br.Close();
                                        };

            bgwrk.WorkerReportsProgress = true;
            bgwrk.RunWorkerAsync();
        }

        private void dgStaff_Sorting(object sender, DataGridSortingEventArgs e)
        {
            if (e.Column.Header.ToString() == "HeadCoachOf*")
            {
                e.Handled = true;

                e.Column.SortDirection = (e.Column.SortDirection != ListSortDirection.Ascending)
                                             ? ListSortDirection.Ascending
                                             : ListSortDirection.Descending;

                var lcv = (ListCollectionView) CollectionViewSource.GetDefaultView(((DataGrid) sender).ItemsSource);
                lcv.CustomSort = new SortStaffOnHeadCoachOf();
            }
        }

        private void dgTeamStats_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.V && Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                updateStatus(!GenericEventHandlers.OnExecutedPaste(sender, null)
                                 ? "Paste into table completed but with errors. Check tracelog.txt for details."
                                 : "Paste into table completed.");
            }
        }

        private void btnFindAllOffsets_Click(object sender, RoutedEventArgs e)
        {
            FindAllOffsets();
        }

        #region Nested type: OffsetThingy

        private struct OffsetThingy
        {
            public readonly int BitsToMove;
            public readonly int BytesToMove;
            public readonly int InitialInBytePosition;
            public readonly long InitialPosition;
            public readonly string Option;
            public readonly string S;
            public int FinalInBytePosition;
            public long FinalPosition;

            public OffsetThingy(string option, string s, int bytesToMove, int bitsToMove)
            {
                Option = option;
                S = s;
                BytesToMove = bytesToMove;
                BitsToMove = bitsToMove;
                FinalPosition = 0;
                FinalInBytePosition = 0;
                InitialInBytePosition = 0;
                InitialPosition = 0;
            }

            public OffsetThingy(string option, string s, int bytesToMove, int bitsToMove, long initialPosition, int initialInBytePosition)
            {
                Option = option;
                S = s;
                BytesToMove = bytesToMove;
                BitsToMove = bitsToMove;
                FinalPosition = 0;
                FinalInBytePosition = 0;
                InitialInBytePosition = initialInBytePosition;
                InitialPosition = initialPosition;
            }
        }

        #endregion

        private void btnFixIsFA_Click(object sender, RoutedEventArgs e)
        {
            if (
                MessageBox.Show(
                    "This function will automatically fix the IsFA property for all players that are in the FA pool. " +
                    "Players assigned to teams or that are hidden will not be edited.\n\n" + "Are you sure you want to continue?",
                    "NBA 2K13 Roster Editor", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
            {
                return;
            }

            List<int> playersInFAPool;
            try
            {
                playersInFAPool = teamsList.Single(te => te.ID == 999).RosterSpots.ToList();
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("The Teams tab doesn't seem to be working. Operation aborted.");
                return;
            }

            foreach (var pID in playersInFAPool)
            {
                playersList[pID].IsFA1 = true;
                playersList[pID].IsFA2 = true;
            }

            updateStatus("IsFA property for players in FA pool corrected.");
        }

        private void btnFixAllIsFA_Click(object sender, RoutedEventArgs e)
        {
            if (
                MessageBox.Show(
                    "This function will automatically fix the IsFA property for all players. Only players that are in the FA pool will have IsFA set if you continue.\n\n" +
                    "Are you sure you want to continue?", "NBA 2K13 Roster Editor", MessageBoxButton.YesNo, MessageBoxImage.Question) !=
                MessageBoxResult.Yes)
            {
                return;
            }

            if (teamsList.All(te => te.ID != 999))
            {
                MessageBox.Show("The Teams tab doesn't seem to be working. Operation aborted.");
                return;
            }

            foreach (var player in playersList)
            {
                player.IsFA1 = player.IsInFAPool;
                player.IsFA2 = player.IsInFAPool;
            }

            updateStatus("IsFA property for all players corrected.");
        }

        private void btnFixWaivedRFAs_Click(object sender, RoutedEventArgs e)
        {
            if (
                MessageBox.Show(
                    "This function will automatically fix the RFA property for players that are currently free agents. This MUST NOT be used during the off-season.\n\n" +
                    "Are you sure you want to continue?", "NBA 2K13 Roster Editor", MessageBoxButton.YesNo, MessageBoxImage.Question) !=
                MessageBoxResult.Yes)
            {
                return;
            }

            foreach (var player in playersList)
            {
                if (player.IsFA1)
                {
                    player.RFA = false;
                }

            updateStatus("RFA property corrected.");
        }
        }
    }

    internal class SortStaffOnHeadCoachOf : IComparer
    {
        #region IComparer Members

        public int Compare(object x, object y)
        {
            string s1;
            string s2;
            try
            {
                s1 = ((StaffEntry) x).HeadCoachOf;
                s2 = ((StaffEntry) y).HeadCoachOf;
            }
            catch (InvalidCastException)
            {
                throw new InvalidCastException("SortStaffOnHeadCoachOf can only compare StaffEntry instances.");
            }
            int i1, i2;
            try
            {
                i1 = Convert.ToInt32(s1.Split(',')[0]);
            }
            catch
            {
                i1 = int.MaxValue;
            }
            try
            {
                i2 = Convert.ToInt32(s2.Split(',')[0]);
            }
            catch
            {
                i2 = int.MaxValue;
            }
            return i1.CompareTo(i2);
        }

        #endregion
    }


    public enum Mode
    {
        PC,
        X360,
        PCNov10,
        Custom,
        X360Nov10,
        CustomX360
    }

    public enum SaveType
    {
        Roster,
        Association,
        MyCareer,
        DraftClass
    }
}