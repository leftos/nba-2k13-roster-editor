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
using NBA_2K13_Roster_Editor.Data.Players;
using NBA_2K13_Roster_Editor.Data.Players.Parameters;
using NBA_2K13_Roster_Editor.Data.Teams;
using NonByteAlignedBinaryRW;
using WPFColorPickerLib;

namespace NBA_2K13_Roster_Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string NBA2K13SavesPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                                @"\2K Sports\NBA 2K13\Saves";

        public static string DocsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\NBA 2K13 Roster Editor";
        public static string AppPath = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
        public static MainWindow mw;

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

        private readonly DispatcherTimer timer;

        private NonByteAlignedBinaryReader br;
        private int curFoundID = -1;
        private string currentFile;
        private bool doCRC;
        //private List<int> foundIDs = new List<int>();
        private Mode mode;
        private Dictionary<string, string> names;
        private SaveType saveType;

        public MainWindow()
        {
            InitializeComponent();

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

            PreparePlayersDataGrid();

            PrepareTeamsDataGrid();

            jerseysList = new ObservableCollection<JerseyEntry>();
            dgJerseys.ItemsSource = jerseysList;

            timer = new DispatcherTimer();
            timer.Tick += timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 3);
        }

        private void PrepareTeamsDataGrid()
        {
            dgTeams.Columns.Clear();
            dgTeams.Columns.Add(new DataGridTextColumn
                                {Header = "ID", Binding = new Binding {Path = new PropertyPath("ID"), Mode = BindingMode.TwoWay}});
            dgTeams.Columns.Add(new DataGridTextColumn { Header = "Name", Binding = new Binding { Path = new PropertyPath("Name"), Mode = BindingMode.TwoWay } });
            dgTeams.Columns.Add(new DataGridTextColumn { Header = "StHeadCoach", Binding = new Binding { Path = new PropertyPath("StHeadCoach"), Mode = BindingMode.TwoWay } });
            dgTeams.Columns.Add(new DataGridTextColumn
                                {Header = "StAsstCoach", Binding = new Binding {Path = new PropertyPath("StAsstCoach"), Mode = BindingMode.TwoWay}});
            dgTeams.Columns.Add(new DataGridTextColumn
                                {Header = "PlNum", Binding = new Binding {Path = new PropertyPath("PlNum"), Mode = BindingMode.TwoWay}});
            for (int i = 0; i < 512; i++)
            {
                dgTeams.Columns.Add(new DataGridTextColumn
                                    {
                                        Header = string.Format("R{0}", (i + 1)),
                                        Binding =
                                            new Binding {Path = new PropertyPath(string.Format("RosterSpots[{0}]", i)), Mode = BindingMode.TwoWay}
                                    });
            }

            teamsList = new ObservableCollection<TeamEntry>();
            dgTeams.ItemsSource = teamsList;
        }

        private ObservableCollection<PlayerEntry> playersList { get; set; }
        private ObservableCollection<Option> optionsList { get; set; }
        private ObservableCollection<TeamEntry> teamsList { get; set; }
        private ObservableCollection<JerseyEntry> jerseysList { get; set; }

        private void PreparePlayersDataGrid()
        {
            for (int i = 0; i < 37; i++)
            {
                dgPlayers.Columns.Add(new DataGridTextColumn
                                      {
                                          Header = string.Format("R{0}", Enum.GetName(typeof (Rating), i)),
                                          Binding =
                                              new Binding
                                              {Path = new PropertyPath(string.Format("Ratings[{0}]", i)), Mode = BindingMode.TwoWay}
                                      });
            }

            for (int i = 0; i < 69; i++)
            {
                dgPlayers.Columns.Add(new DataGridTextColumn
                                      {
                                          Header = string.Format("T{0}", Enum.GetName(typeof (Tendency), i)),
                                          Binding =
                                              new Binding
                                              {Path = new PropertyPath(string.Format("Tendencies[{0}]", i)), Mode = BindingMode.TwoWay}
                                      });
            }

            for (int i = 0; i < 25; i++)
            {
                dgPlayers.Columns.Add(new DataGridTextColumn
                                      {
                                          Header = string.Format("HS{0}", Enum.GetName(typeof (HotSpot), i)),
                                          Binding =
                                              new Binding
                                              {Path = new PropertyPath(string.Format("HotSpots[{0}]", i)), Mode = BindingMode.TwoWay}
                                      });
            }

            dgPlayers.Columns.Add(new DataGridComboBoxColumn
                                  {
                                      Header = "ContractOpt",
                                      SelectedValueBinding = new Binding {Path = new PropertyPath("ContractOpt"), Mode = BindingMode.TwoWay},
                                      ItemsSource = Enum.GetValues(typeof (ContractOption))
                                  });

            dgPlayers.Columns.Add(new DataGridCheckBoxColumn
                                  {
                                      Header = "ContNoTrade",
                                      Binding = new Binding {Path = new PropertyPath("ContNoTrade"), Mode = BindingMode.TwoWay}
                                  });

            for (int i = 0; i < 7; i++)
            {
                dgPlayers.Columns.Add(new DataGridTextColumn
                                      {
                                          Header = string.Format("ContractY{0}", i + 1),
                                          Binding =
                                              new Binding
                                              {Path = new PropertyPath(string.Format("ContractY{0}", i + 1)), Mode = BindingMode.TwoWay}
                                      });
            }

            dgPlayers.Columns.Add(new DataGridTextColumn
            {
                Header = string.Format("SigFT"),
                Binding =
                    new Binding { Path = new PropertyPath(string.Format("SigFT")), Mode = BindingMode.TwoWay }
            });
            dgPlayers.Columns.Add(new DataGridTextColumn
            {
                Header = string.Format("SigShtForm"),
                Binding =
                    new Binding { Path = new PropertyPath(string.Format("SigShtForm")), Mode = BindingMode.TwoWay }
            });
            dgPlayers.Columns.Add(new DataGridTextColumn
            {
                Header = string.Format("SigShtBase"),
                Binding =
                    new Binding { Path = new PropertyPath(string.Format("SigShtBase")), Mode = BindingMode.TwoWay }
            });

            for (int i = 0; i < 15; i++)
            {
                dgPlayers.Columns.Add(new DataGridTextColumn
                                      {
                                          Header = string.Format("DunkPkg{0}", i),
                                          Binding =
                                              new Binding
                                              {Path = new PropertyPath(string.Format("DunkPackages[{0}]", i)), Mode = BindingMode.TwoWay}
                                      });
            }

            for (int i = 0; i < 22; i++)
            {
                dgPlayers.Columns.Add(new DataGridTextColumn
                {
                    Header = string.Format("SeasonStats{0}", i),
                    Binding =
                        new Binding { Path = new PropertyPath(string.Format("SeasonStats[{0}]", i)), Mode = BindingMode.TwoWay }
                });
            }

            dgPlayers.Columns.Add(new DataGridTextColumn
            {
                Header = string.Format("PlayoffStats"),
                Binding =
                    new Binding { Path = new PropertyPath(string.Format("PlayoffStats")), Mode = BindingMode.TwoWay }
            });

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
                            string id =parts[0];
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
            optionsList = new ObservableCollection<Option>();
            optionsList.Add(new Option {Setting = "FirstSSOffset", Value = GetRegistrySetting("FirstSSOffset", 40916)});
            optionsList.Add(new Option { Setting = "FirstSSOffsetBit", Value = GetRegistrySetting("FirstSSOffsetBIt", 2) });
            optionsList.Add(new Option {Setting = "LastPlayerID", Value = GetRegistrySetting("LastPlayerID", 1514)});
            optionsList.Add(new Option {Setting = "LastTeamID", Value = GetRegistrySetting("LastTeamID", 90)});
            optionsList.Add(new Option {Setting = "LastJerseyID", Value = GetRegistrySetting("LastJerseyID", 425)});
            optionsList.Add(new Option {Setting = "NamesFile", Value = GetRegistrySetting("NamesFile", "names.txt")});
            optionsList.Add(new Option { Setting = "ChooseNameBy", Value = GetRegistrySetting("ChooseNameBy", "ID") });

            optionsList.Add(new Option { Setting = "CustomSSOffset", Value = GetRegistrySetting("CustomSSOffset", 40916) });
            optionsList.Add(new Option { Setting = "CustomSSOffsetBit", Value = GetRegistrySetting("CustomSSOffsetBIt", 2) });
            optionsList.Add(new Option { Setting = "CustomRosterOffset", Value = GetRegistrySetting("CustomRosterOffset", 862911) });
            optionsList.Add(new Option { Setting = "CustomRosterOffsetBit", Value = GetRegistrySetting("CustomRosterOffsetBIt", 6) });
            optionsList.Add(new Option { Setting = "CustomJerseyOffset", Value = GetRegistrySetting("CustomJerseyOffset", 1486997) });
            optionsList.Add(new Option { Setting = "CustomJerseyOffsetBit", Value = GetRegistrySetting("CustomJerseyOffsetBIt", 0) });

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
            ofd.InitialDirectory = GetRegistrySetting("LastOpenPath", NBA2K13SavesPath);
            ofd.Filter = "All compatible NBA 2K13 files (*.ROS; *.FXG; *.CMG)|*.ROS;*.FXG;*.CMG|" + "Roster files (*.ROS)|*.ROS|" +
                         "Association files (*.FXG)|*.FXG|" + "MyCareer files (*.CMG)|*.CMG|" + "All files (*.*)|*.*";
            ofd.DefaultExt = ".ROS";
            ofd.ShowDialog();

            if (ofd.FileName == "")
                return;

            SetRegistrySetting("LastOpenPath", Path.GetDirectoryName(ofd.FileName));

            txtFile.Text = Path.GetFileName(ofd.FileName);
            currentFile = ofd.FileName;
            br = new NonByteAlignedBinaryReader(File.Open(currentFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));

            string ext = Path.GetExtension(currentFile);
            switch (ext)
            {
                case ".CMG":
                    saveType = SaveType.MyCareer;
                    break;
                case ".FXG":
                    saveType = SaveType.Association;
                    break;
                case ".FDC":
                    saveType = SaveType.DraftClass;
                    break;
                default:
                    saveType = SaveType.Roster;
                    break;
            }

            ReloadEverything();

            updateStatus("Roster loaded.");
        }

        private bool editing;
        private List<int> foundIDList = new List<int>();

        private void dataGrid_CellEditEnding(object sender,
                                          DataGridCellEditEndingEventArgs e)
        {
            if (!editing && e.EditAction == DataGridEditAction.Commit)
            {
                editing = true;
                ((DataGrid)sender).CommitEdit(DataGridEditingUnit.Row, true);
                editing = false;
            }
        }

        private void PopulateJerseysTab()
        {
            jerseysList = new ObservableCollection<JerseyEntry>();
            
            for (int i = 0; i <= 20000; i++)
            {
                MoveStreamToFirstJersey();
                br.MoveStreamPosition(68*i, 0);

                var je = new JerseyEntry();
                je.ID = i;
                ushort gid = BitConverter.ToUInt16(br.ReadNonByteAlignedBytes(2).Reverse().ToArray(), 0);
                if (!Enum.IsDefined(typeof(JerseyType),gid))
                {
                    /*
                    MessageBox.Show("GID with decimal value " + gid +
                                    " isn't in the Roster Editor's database. The jersey table will have to stop at ID " + (i-1) + ".\n\n" +
                                    "Please contact the developer and inform him of this error.");
                    */
                    optionsList.Single(o => o.Setting == "LastJerseyID").Value = i - 1;
                    //btnSaveOptions_Click(null, null);
                    break;
                }
                je.GID = (JerseyType) Enum.Parse(typeof (JerseyType), gid.ToString());
                
                br.MoveStreamPosition(0, 6);
                je.Neck = (NeckType) Enum.Parse(typeof (NeckType), Convert.ToByte(br.ReadNonByteAlignedBits(3),2).ToString());
                br.MoveStreamPosition(0, 2);
                je.SockClr = (SockColor) Enum.Parse(typeof (SockColor), br.ReadNonByteAlignedBits(1));
                br.MoveStreamPosition(0, 4);

                br.MoveStreamPosition(10, 0);
                string hexName = Tools.ByteArrayToHexString(br.ReadNonByteAlignedBytes(4));
                try
                {
                    je.Name = (JerseyName) Enum.Parse(typeof(JerseyName), JerseyEntry.JerseyNames.Single(n => n.Value == hexName).Key);
                }
                catch (Exception)
                {
                    Console.WriteLine(hexName);
                }
                br.MoveStreamPosition(3, 5);
                byte[] ba = br.ReadNonByteAlignedBytes(4);
                je.TeamColor1 = ba.Aggregate("#", (current, b) => current + Convert.ToString(b, 16).PadLeft(2, '0')).ToUpperInvariant();
                ba = br.ReadNonByteAlignedBytes(4);
                je.TeamColor2 = ba.Aggregate("#", (current, b) => current + Convert.ToString(b, 16).PadLeft(2, '0')).ToUpperInvariant();
                ba = br.ReadNonByteAlignedBytes(4);
                je.TeamColor3 = ba.Aggregate("#", (current, b) => current + Convert.ToString(b, 16).PadLeft(2, '0')).ToUpperInvariant();
                ba = br.ReadNonByteAlignedBytes(4);
                je.TeamColor4 = ba.Aggregate("#", (current, b) => current + Convert.ToString(b, 16).PadLeft(2, '0')).ToUpperInvariant();
                ba = br.ReadNonByteAlignedBytes(4);
                je.TeamColor5 = ba.Aggregate("#", (current, b) => current + Convert.ToString(b, 16).PadLeft(2, '0')).ToUpperInvariant();
                ba = br.ReadNonByteAlignedBytes(4);
                je.TeamColor6 = ba.Aggregate("#", (current, b) => current + Convert.ToString(b, 16).PadLeft(2, '0')).ToUpperInvariant();
                jerseysList.Add(je);
            }

            dgJerseys.ItemsSource = null;
            dgJerseys.ItemsSource = jerseysList;
        }

        private void MoveStreamToFirstJersey()
        {
            br.BaseStream.Position = 1486997;
            br.InBytePosition = 0;

            if (mode == Mode.X360)
            {
                br.MoveStreamPosition(77824, 0);
            }
            else if (mode == Mode.PCNov10 || mode == Mode.X360Nov10)
            {
                //br.MoveStreamPosition(1911, -4);
                br.BaseStream.Position = 1489588; // 4080
                br.InBytePosition = 4;
            }
            else if (mode == Mode.Custom)
            {
                br.BaseStream.Position = Convert.ToInt64(GetOption("CustomJerseyOffset"));
                br.InBytePosition = Convert.ToInt32(GetOption("CustomJerseyOffsetBit"));
            }

            MoveStreamForSaveType();
        }

        private object GetOption(string setting)
        {
            return optionsList.First(o => o.Setting == setting).Value;
        }

        private void PopulatePlayersTab()
        {
            playersList = new ObservableCollection<PlayerEntry>();
            for (int i = 0; i <= Convert.ToInt32(GetOption("LastPlayerID")); i++)
            {
                var pe = new PlayerEntry();
                MoveStreamToFirstSS(i);
                pe.ID = i;
                ReadPlayerInfo(ref pe);
                pe.Name = FindPlayerName(pe);
                pe.SSList = ReadSignatureSkills(i);
                pe.Ratings = ReadRatings(i);
                pe.Tendencies = ReadTendencies(i);
                pe.HotSpots = ReadHotSpots(i);
                pe.AssignedTo = FindPlayerInTeams(i);
                pe.IsFA = teamsList.Single(te => te.ID == 999).RosterSpots.Contains(pe.ID);
                pe.IsHidden = (pe.AssignedTo == -1 && !pe.IsFA);
                playersList.Add(pe);
            }
            dgPlayers.ItemsSource = playersList;
        }

        internal ObservableCollection<PlayerEntry> filteredPlayersList { get; set; }

        private int FindPlayerInTeams(int playerID)
        {
            try
            {
                var id = teamsList.First(t => t.RosterSpots.Contains(playerID)).ID;
                return id != 999 ? id : -1;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        private ObservableCollection<byte> ReadRatings(int playerID)
        {
            var ratings = new ObservableCollection<byte>();

            MoveStreamToFirstSS(playerID);
            br.MoveStreamPosition(14, 3);

            for (int i = 0; i < 37; i++)
            {
                byte b = br.ReadNonByteAlignedByte();
                var realRating = (byte) (b/3 + 25);
                ratings.Add(realRating);
            }

            return ratings;
        }

        private ObservableCollection<byte> ReadTendencies(int playerID)
        {
            var tend = new ObservableCollection<byte>();

            MoveStreamToFirstSS(playerID);
            br.MoveStreamPosition(51, 3);

            for (int i = 0; i < 69; i++)
            {
                byte b = br.ReadNonByteAlignedByte();
                tend.Add(b);
            }

            return tend;
        }

        private ObservableCollection<byte> ReadHotSpots(int playerID)
        {
            var hs = new ObservableCollection<byte>();

            MoveStreamToFirstSS(playerID);
            br.MoveStreamPosition(120, 3);

            for (int i = 0; i < 25; i++)
            {
                byte b = br.ReadNonByteAlignedByte();
                hs.Add(b);
            }

            return hs;
        }

        private void PopulateTeamsTab()
        {
            teamsList = new ObservableCollection<TeamEntry>();

            TeamEntry te;
            for (int i = 0; i <= Convert.ToInt32(GetOption("LastTeamID")); i++)
            {
                te = new TeamEntry();
                te.ID = i;
                try
                {
                    te.Name = teamNames[i];
                }
                catch
                {
                    te.Name = "";
                }

                MoveStreamToCurrentTeamRoster(i);
                PopulateRosterRow(18, ref te);

                //
                MoveStreamToCurrentTeamRoster(i);
                br.MoveStreamPosition(166, 0);
                /*if (saveType == SaveType.Roster)
                    br.MoveStreamPosition(8, 0);*/
                te.StHeadCoach = Convert.ToInt16(NonByteAlignedBinaryReader.ByteArrayToBitString(br.ReadNonByteAlignedBytes(2)), 2);

                MoveStreamToCurrentTeamRoster(i);
                br.MoveStreamPosition(170, 0);
                te.StAsstCoach = Convert.ToInt16(NonByteAlignedBinaryReader.ByteArrayToBitString(br.ReadNonByteAlignedBytes(2)), 2);
                //
            }

            // Free Agents
            te = new TeamEntry();
            te.ID = 999;
            te.Name = "Free Agents";

            br.BaseStream.Position = 853505;
            br.InBytePosition = 6;

            if (mode == Mode.X360)
            {
                br.BaseStream.Position += 69632;
            }
            else if (mode == Mode.PCNov10 || mode == Mode.X360Nov10)
            {
                br.BaseStream.Position += 1911;
                br.InBytePosition = 2;
            }
            else if (mode == Mode.Custom)
            {
                MoveStreamToFirstRoster();
                br.BaseStream.Position -= 9406;
            }

            MoveStreamForSaveType();

            PopulateRosterRow(512, ref te, true);
            //

            RelinkTeamsDataGrid();
        }

        private void MoveStreamToCurrentTeamRoster(int i)
        {
            MoveStreamToFirstRoster();
            //MoveStreamForSaveType();
            br.MoveStreamPosition(720*i, 2*i);
        }

        private void MoveStreamToFirstRoster()
        {
            long firstRosterOffset = 862911;
            int firstRosterOffsetBit = 6;
            br.BaseStream.Position = firstRosterOffset;
            br.InBytePosition = firstRosterOffsetBit;

            if (mode == Mode.X360)
            {
                br.BaseStream.Position += 69632;
            }
            else if (mode == Mode.PCNov10 || mode == Mode.X360Nov10)
            {
                br.BaseStream.Position += 1911;//2119;
                br.InBytePosition = 2;
            }
            else if (mode == Mode.Custom)
            {
                br.BaseStream.Position = Convert.ToInt64(GetOption("CustomRosterOffset"));
                br.InBytePosition = Convert.ToInt32(GetOption("CustomRosterOffsetBit"));
            }

            MoveStreamForSaveType();
        }

        private void PopulateRosterRow(int countToRead, ref TeamEntry te, bool isFArow = false)
        {
            long startOfRoster = br.BaseStream.Position;
            int startOfRosterBit = br.InBytePosition;

            for (int i = 0; i < countToRead; i++)
            {
                te.RosterSpots.Add(Convert.ToInt32(ReadRosterSpot()));
            }

            br.BaseStream.Position = startOfRoster;
            br.InBytePosition = startOfRosterBit;

            if (!isFArow)
                br.MoveStreamPosition(125, 0);
            else
                br.MoveStreamPosition(4001, 0);

            br.MoveStreamPosition(0, -1);

            te.PlNum = Convert.ToUInt16(br.ReadNonByteAlignedBits(9),2);
            teamsList.Add(te);
        }

        private void RelinkTeamsDataGrid()
        {
            dgTeams.ItemsSource = null;
            dgTeams.ItemsSource = teamsList;
        }

        private string ReadRosterSpot()
        {
            byte[] b = br.ReadNonByteAlignedBytes(2);
            if (b[0] == 0 && b[1] == 0)
                return "-1";

            b = br.ReadNonByteAlignedBytes(2);
            return Convert.ToInt16(NonByteAlignedBinaryReader.ByteArrayToBitString(b), 2).ToString();
        }

        private ObservableCollection<SignatureSkill> ReadSignatureSkills(int playerID)
        {
            MoveStreamToFirstSS(playerID);

            var ssList = new ObservableCollection<SignatureSkill>();

            byte b = NonByteAlignedBinaryReader.BitStringToByte(br.ReadNonByteAlignedBits(6));
            ssList.Add(Enum.IsDefined(typeof (SignatureSkill), b) ? ((SignatureSkill) b) : 0);
            b = NonByteAlignedBinaryReader.BitStringToByte(br.ReadNonByteAlignedBits(6));
            ssList.Add(Enum.IsDefined(typeof (SignatureSkill), b) ? ((SignatureSkill) b) : 0);
            br.ReadNonByteAlignedBits(14);
            b = NonByteAlignedBinaryReader.BitStringToByte(br.ReadNonByteAlignedBits(6));
            ssList.Add(Enum.IsDefined(typeof (SignatureSkill), b) ? ((SignatureSkill) b) : 0);
            b = NonByteAlignedBinaryReader.BitStringToByte(br.ReadNonByteAlignedBits(6));
            ssList.Add(Enum.IsDefined(typeof (SignatureSkill), b) ? ((SignatureSkill) b) : 0);
            b = NonByteAlignedBinaryReader.BitStringToByte(br.ReadNonByteAlignedBits(6));
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

        private void ReadPlayerInfo(ref PlayerEntry pe)
        {
            int playerID = pe.ID;

            MoveStreamToPortraitID(playerID); // 40616

            byte[] por = br.ReadNonByteAlignedBytes(2);

            // PlType
            br.MoveStreamPosition(2, 6);

            byte plType = Convert.ToByte(br.ReadNonByteAlignedBits(3), 2);

            br.MoveStreamPosition(-3, -1);
            //


            br.MoveStreamPosition(28, 0);

            byte[] cf = br.ReadNonByteAlignedBytes(2);

            br.MoveStreamPosition(94, 1);

            bool genericF = br.ReadNonByteAlignedBits(1) == "1";

            br.MoveStreamPosition(137, 6);

            byte[] audio = br.ReadNonByteAlignedBytes(2);


            MoveStreamToPortraitID(playerID);
            br.MoveStreamPosition(-5, 0);
            pe.TeamID1 = br.ReadNonByteAlignedByte();
            br.MoveStreamPosition(261, 0);
            pe.TeamID2 = br.ReadNonByteAlignedByte();

            MoveStreamToPortraitID(playerID);
            br.MoveStreamPosition(-16, 0);
            pe.Height = BitConverter.ToSingle(br.ReadNonByteAlignedBytes(4).Reverse().ToArray(), 0);
            pe.Weight = BitConverter.ToSingle(br.ReadNonByteAlignedBytes(4).Reverse().ToArray(), 0);


            MoveStreamToPortraitID(playerID);
            br.MoveStreamPosition(126, 6);
            pe.Skintone = Convert.ToInt32(br.ReadNonByteAlignedBits(3), 2);
            br.MoveStreamPosition(0, 6);
            pe.CAPHairClr = (HairColor) Enum.Parse(typeof (HairColor), Convert.ToByte(br.ReadNonByteAlignedBits(4), 2).ToString());
            br.MoveStreamPosition(0, 3);
            pe.CAPEyebrow = Convert.ToInt32(br.ReadNonByteAlignedBits(4), 2);
            br.MoveStreamPosition(0, 6);
            pe.CAPMstch = Convert.ToInt32(br.ReadNonByteAlignedBits(3), 2);
            pe.CAPFclHairClr = (HairColor) Enum.Parse(typeof (HairColor), Convert.ToByte(br.ReadNonByteAlignedBits(4), 2).ToString());
            pe.CAPBeard = Convert.ToInt32(br.ReadNonByteAlignedBits(4), 2);
            pe.CAPGoatee = Convert.ToInt32(br.ReadNonByteAlignedBits(5), 2);

            MoveStreamToPortraitID(playerID);
            br.MoveStreamPosition(154, 5);
            pe.PlayStyle = Convert.ToInt32(br.ReadNonByteAlignedBits(5), 2);
            MoveStreamToPortraitID(playerID);
            br.MoveStreamPosition(143, 5);
            pe.PlayType1 = (PlayType)Enum.Parse(typeof(PlayType), Convert.ToByte(br.ReadNonByteAlignedBits(4), 2).ToString());
            pe.PlayType2 = (PlayType)Enum.Parse(typeof(PlayType), Convert.ToByte(br.ReadNonByteAlignedBits(4), 2).ToString());
            pe.PlayType3 = (PlayType)Enum.Parse(typeof(PlayType), Convert.ToByte(br.ReadNonByteAlignedBits(4), 2).ToString());
            pe.PlayType4 = (PlayType)Enum.Parse(typeof(PlayType), Convert.ToByte(br.ReadNonByteAlignedBits(4), 2).ToString());

            MoveStreamToPortraitID(playerID);
            br.MoveStreamPosition(178, 0);
            pe.DunkPackages = new ObservableCollection<int>();
            for (int i = 0; i < 15; i++)
            {
                pe.DunkPackages.Add(Convert.ToInt32(br.ReadNonByteAlignedByte()));
            }

            MoveStreamToPortraitID(playerID);
            br.MoveStreamPosition(25, 0);
            pe.Position1 = ReadAndConvertToEnum<Position>(3);
            pe.Position2 = ReadAndConvertToEnum<Position>(3);

            //Shoe
            MoveStreamToPortraitID(playerID);

            br.MoveStreamPosition(122, 0);
            int shoeModel = Convert.ToInt32(br.ReadNonByteAlignedBits(12), 2);

            var shoeBrand = (ShoeBrand) Enum.Parse(typeof (ShoeBrand), Convert.ToByte(br.ReadNonByteAlignedBits(3), 2).ToString());


            MoveStreamToPortraitID(playerID);
            br.MoveStreamPosition(98, 0);
            byte[] ba = br.ReadNonByteAlignedBytes(4);
            pe.ShHomeTeam1 = ba.Aggregate("#", (current, b) => current + Convert.ToString(b, 16).PadLeft(2, '0')).ToUpperInvariant();
            ba = br.ReadNonByteAlignedBytes(4);
            pe.ShHomeTeam2 = ba.Aggregate("#", (current, b) => current + Convert.ToString(b, 16).PadLeft(2, '0')).ToUpperInvariant();
            ba = br.ReadNonByteAlignedBytes(4);
            pe.ShHomeBase = ba.Aggregate("#", (current, b) => current + Convert.ToString(b, 16).PadLeft(2, '0')).ToUpperInvariant();
            ba = br.ReadNonByteAlignedBytes(4);
            pe.ShAwayBase = ba.Aggregate("#", (current, b) => current + Convert.ToString(b, 16).PadLeft(2, '0')).ToUpperInvariant();
            ba = br.ReadNonByteAlignedBytes(4);
            pe.ShAwayTeam2 = ba.Aggregate("#", (current, b) => current + Convert.ToString(b, 16).PadLeft(2, '0')).ToUpperInvariant();
            ba = br.ReadNonByteAlignedBytes(4);
            pe.ShAwayTeam1 = ba.Aggregate("#", (current, b) => current + Convert.ToString(b, 16).PadLeft(2, '0')).ToUpperInvariant();
            br.MoveStreamPosition(3, 5);
            pe.ShCustomClr = br.ReadNonByteAlignedBits(1) == "1";
            //

            //Hair
            MoveStreamToPortraitID(playerID);
            br.MoveStreamPosition(127, 1);
            var capHair = (CAPHairType) Enum.Parse(typeof (CAPHairType), Convert.ToByte(br.ReadNonByteAlignedBits(6), 2).ToString());
            //

            //
            MoveStreamToPortraitID(playerID);
            br.MoveStreamPosition(5, 4);
            byte number = br.ReadNonByteAlignedByte();
            //

            //
            MoveStreamToPortraitID(playerID);
            br.MoveStreamPosition(126, 3);
            var bodyType = (BodyType) Enum.Parse(typeof (BodyType), Convert.ToByte(br.ReadNonByteAlignedBits(2), 2).ToString());
            var muscleType = (MuscleTone) Enum.Parse(typeof (MuscleTone), Convert.ToByte(br.ReadNonByteAlignedBits(1), 2).ToString());

            MoveStreamToPortraitID(playerID);
            br.MoveStreamPosition(128, 3);
            var eyes = (EyeColor) Enum.Parse(typeof (EyeColor), Convert.ToByte(br.ReadNonByteAlignedBits(3), 2).ToString());
            //


            MoveStreamToPortraitID(playerID);
            br.MoveStreamPosition(154, 0);
            pe.ContractOpt =
                (ContractOption) Enum.Parse(typeof (ContractOption), Convert.ToByte(br.ReadNonByteAlignedBits(2), 2).ToString());

            MoveStreamToPortraitID(playerID);
            br.MoveStreamPosition(177, 5);
            pe.ContNoTrade = br.ReadNonByteAlignedBits(1) == "1";

            MoveStreamToPortraitID(playerID);
            br.MoveStreamPosition(214, 0);
            pe.ContractY1 = Convert.ToUInt32(NonByteAlignedBinaryReader.ByteArrayToBitString(br.ReadNonByteAlignedBytes(4)), 2);
            pe.ContractY2 = Convert.ToUInt32(NonByteAlignedBinaryReader.ByteArrayToBitString(br.ReadNonByteAlignedBytes(4)), 2);
            pe.ContractY3 = Convert.ToUInt32(NonByteAlignedBinaryReader.ByteArrayToBitString(br.ReadNonByteAlignedBytes(4)), 2);
            pe.ContractY4 = Convert.ToUInt32(NonByteAlignedBinaryReader.ByteArrayToBitString(br.ReadNonByteAlignedBytes(4)), 2);
            pe.ContractY5 = Convert.ToUInt32(NonByteAlignedBinaryReader.ByteArrayToBitString(br.ReadNonByteAlignedBytes(4)), 2);
            pe.ContractY6 = Convert.ToUInt32(NonByteAlignedBinaryReader.ByteArrayToBitString(br.ReadNonByteAlignedBytes(4)), 2);
            pe.ContractY7 = Convert.ToUInt32(NonByteAlignedBinaryReader.ByteArrayToBitString(br.ReadNonByteAlignedBytes(4)), 2);

            MoveStreamToPortraitID(playerID);
            br.MoveStreamPosition(193, 0);
            pe.SigFT = br.ReadNonByteAlignedByte();
            pe.SigShtForm = br.ReadNonByteAlignedByte();
            pe.SigShtBase = br.ReadNonByteAlignedByte();

            MoveStreamToPortraitID(playerID);
            br.MoveStreamPosition(314, 2);
            pe.ClothesType = (ClothesType)Enum.Parse(typeof(ClothesType), Convert.ToByte(br.ReadNonByteAlignedBits(2), 2).ToString());


            MoveStreamToPortraitID(playerID);
            br.MoveStreamPosition(24, 1);
            pe.InjuryType = Convert.ToUInt16(br.ReadNonByteAlignedBits(7), 2);
            br.MoveStreamPosition(3, 0);
            pe.InjuryDays = Convert.ToInt16(NonByteAlignedBinaryReader.ByteArrayToBitString(br.ReadNonByteAlignedBytes(2)), 2);


            MoveStreamToPortraitID(playerID);
            br.MoveStreamPosition(2, 0);
            pe.BirthYear = Convert.ToUInt16(br.ReadNonByteAlignedBits(12), 2);
            pe.BirthMonth = Convert.ToByte(br.ReadNonByteAlignedBits(4), 2);
            pe.BirthDay = Convert.ToByte(br.ReadNonByteAlignedBits(5), 2);
            MoveStreamToPortraitID(playerID);
            br.MoveStreamPosition(87, 6);
            pe.YearsPro = Convert.ToByte(br.ReadNonByteAlignedBits(5), 2);


            MoveStreamToPortraitID(playerID);
            br.MoveStreamPosition(40, 0);
            for (int i = 0; i < 22; i++)
            {
                pe.SeasonStats.Add(Convert.ToInt32(br.ReadNonByteAlignedBits(16), 2));
            }
            pe.PlayoffStats = Convert.ToInt32(br.ReadNonByteAlignedBits(16), 2);

            MoveStreamToFirstSS(playerID);

            pe.CFID = Convert.ToUInt16(NonByteAlignedBinaryReader.ByteArrayToBitString(cf), 2);
            pe.PortraitID = Convert.ToUInt16(NonByteAlignedBinaryReader.ByteArrayToBitString(por), 2);
            pe.ASAID = Convert.ToUInt16(NonByteAlignedBinaryReader.ByteArrayToBitString(audio), 2);
            pe.GenericF = genericF;
            pe.PlType = plType;
            pe.ShoeBrand = shoeBrand;
            pe.ShoeModel = shoeModel;
            pe.CAPHairType = capHair;
            pe.JerseyNumber = number;
            pe.MuscleTone = muscleType;
            pe.BodyType = bodyType;
            pe.EyeColor = eyes;
        }

        private T ReadAndConvertToEnum<T>(int bits)
        {
            return (T) Enum.Parse(typeof (T), Convert.ToByte(br.ReadNonByteAlignedBits(bits), 2).ToString());
        }

        private void MoveStreamToPortraitID(int playerID)
        {
            MoveStreamToFirstSS(playerID);
            try
            {
                br.MoveStreamPosition(-300, -2);
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("Invalid roster offset.");
                return;
            }
        }

        private void MoveStreamToFirstSS(int playerID)
        {
            if (mode != Mode.Custom)
            {
                br.BaseStream.Position = Convert.ToInt64(GetOption("FirstSSOffset"));
                br.InBytePosition = Convert.ToInt32(GetOption("FirstSSOffsetBit"));
            }
            else
            {
                br.BaseStream.Position = Convert.ToInt64(GetOption("CustomSSOffset"));
                br.InBytePosition = Convert.ToInt32(GetOption("CustomSSOffsetBit"));
            }

            MoveStreamForSaveType();

            if (playerID >= 1365 && mode == Mode.X360)
            {
                br.BaseStream.Position += 16384;
            }

            int playerBits = 477*8 + 5;
            int totalBits = playerBits*playerID;
            br.MoveStreamPosition(totalBits/8, totalBits%8);
        }

        private void MoveStreamForSaveType()
        {
            if (saveType == SaveType.Association)
            {
                br.MoveStreamPosition(8, 0);
            }
            else if (saveType == SaveType.MyCareer)
            {
                br.MoveStreamPosition(2190248, 0);
            }
            else if (saveType == SaveType.DraftClass)
            {
                br.MoveStreamPosition(-39596, 0);
                optionsList.Single(o => o.Setting == "LastPlayerID").Value = 79;
            }
        }

        private void Window_Closing_1(object sender, CancelEventArgs e)
        {
            SetRegistrySetting("Height", Height);
            SetRegistrySetting("Width", Width);
            SetRegistrySetting("X", Left);
            SetRegistrySetting("Y", Top);
            try
            {
                br.Close();
            }
            catch (Exception)
            {
            }
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

        public static int GetRegistrySetting(string setting, int defaultValue)
        {
            RegistryKey rk = Registry.CurrentUser;
            int settingValue = defaultValue;
            try
            {
                if (rk == null)
                    throw new Exception();

                rk = rk.OpenSubKey(@"SOFTWARE\Lefteris Aslanoglou\NBA 2K13 Roster Editor");
                if (rk != null)
                    settingValue = Convert.ToInt32(rk.GetValue(setting, defaultValue));
            }
            catch
            {
                settingValue = defaultValue;
            }

            return settingValue;
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
            NonByteAlignedBinaryWriter bw;
            using (bw = new NonByteAlignedBinaryWriter(File.Open(currentFile, FileMode.Open, FileAccess.Write, FileShare.ReadWrite)))
            {
                for (int i = 0; i < playersList.Count; i++)
                {
                    //Console.WriteLine("Saving Player " + i);
                    PlayerEntry pe = playersList[i];
                    MoveStreamToFirstSS(pe.ID);
                    bw.BaseStream.Position = br.BaseStream.Position;
                    bw.InBytePosition = br.InBytePosition;

                    // Appearance
                    long prevPos = br.BaseStream.Position;
                    int prevPosIn = br.InBytePosition;

                    MoveStreamToPortraitID(pe.ID);
                    Write2ByteStringToRoster(pe.PortraitID.ToString(), bw);

                    // PlType
                    br.MoveStreamPosition(2, 6);
                    SyncBWwithBR(ref bw);

                    bw.WriteNonByteAlignedBits(Convert.ToString(Convert.ToByte(pe.PlType.ToString()), 2).PadLeft(3, '0'), br.ReadBytes(2));

                    SyncBRwithBW(bw);
                    br.MoveStreamPosition(-3, -1);
                    //


                    br.MoveStreamPosition(28, 0);

                    Write2ByteStringToRoster(pe.CFID.ToString(), bw);

                    br.MoveStreamPosition(94, 1);

                    SyncBWwithBR(ref bw);
                    byte b = br.ReadByte();
                    bw.WriteNonByteAlignedBits(pe.GenericF ? "1" : "0", new[] {b});
                    SyncBRwithBW(bw);

                    br.MoveStreamPosition(137, 6);

                    Write2ByteStringToRoster(pe.ASAID.ToString(), bw);


                    MoveStreamToPortraitID(pe.ID);
                    br.MoveStreamPosition(-5, 0);
                    SyncBWwithBR(ref bw);
                    bw.WriteNonByteAlignedByte(Convert.ToByte(pe.TeamID1), br.ReadBytes(2));
                    SyncBRwithBW(bw);
                    br.MoveStreamPosition(261, 0);
                    SyncBWwithBR(ref bw);
                    bw.WriteNonByteAlignedByte(Convert.ToByte(pe.TeamID2), br.ReadBytes(2));
                    SyncBRwithBW(bw);

                    // Play Style
                    MoveStreamToPortraitID(pe.ID);
                    br.MoveStreamPosition(154, 5);
                    SyncBWwithBR(ref bw);
                    bw.WriteNonByteAlignedBits(Convert.ToString(pe.PlayStyle, 2).PadLeft(5, '0'), br.ReadBytes(2));
                    SyncBRwithBW(bw);
                    MoveStreamToPortraitID(pe.ID);
                    br.MoveStreamPosition(143, 5);
                    SyncBWwithBR(ref bw);
                    bw.WriteNonByteAlignedBits(
                        Convert.ToString((byte) Enum.Parse(typeof (PlayType), pe.PlayType1.ToString()), 2).PadLeft(4, '0'), br.ReadBytes(2));
                    SyncBRwithBW(bw);
                    bw.WriteNonByteAlignedBits(
                        Convert.ToString((byte)Enum.Parse(typeof(PlayType), pe.PlayType2.ToString()), 2).PadLeft(4, '0'), br.ReadBytes(2));
                    SyncBRwithBW(bw);
                    bw.WriteNonByteAlignedBits(
                        Convert.ToString((byte)Enum.Parse(typeof(PlayType), pe.PlayType3.ToString()), 2).PadLeft(4, '0'), br.ReadBytes(2));
                    SyncBRwithBW(bw);
                    bw.WriteNonByteAlignedBits(
                        Convert.ToString((byte)Enum.Parse(typeof(PlayType), pe.PlayType4.ToString()), 2).PadLeft(4, '0'), br.ReadBytes(2));
                    SyncBRwithBW(bw);


                    // Height & Weight
                    MoveStreamToPortraitID(pe.ID);
                    br.MoveStreamPosition(-16, 0);
                    SyncBWwithBR(ref bw);
                    bw.WriteNonByteAlignedBits(
                        NonByteAlignedBinaryReader.ByteArrayToBitString(BitConverter.GetBytes(pe.Height).Reverse().ToArray()),
                        br.ReadBytes(5));
                    SyncBRwithBW(bw);
                    bw.WriteNonByteAlignedBits(
                        NonByteAlignedBinaryReader.ByteArrayToBitString(BitConverter.GetBytes(pe.Weight).Reverse().ToArray()),
                        br.ReadBytes(5));
                    //

                    // Positions
                    MoveStreamToPortraitID(pe.ID);
                    br.MoveStreamPosition(25, 0);
                    SyncBWwithBR(ref bw);
                    bw.WriteNonByteAlignedBits(
                        Convert.ToString((byte) Enum.Parse(typeof (Position), pe.Position1.ToString()), 2).PadLeft(3, '0'), br.ReadBytes(2));
                    SyncBRwithBW(bw);
                    bw.WriteNonByteAlignedBits(
                        Convert.ToString((byte) Enum.Parse(typeof (Position), pe.Position2.ToString()), 2).PadLeft(3, '0'), br.ReadBytes(2));

                    // Shoes
                    MoveStreamToPortraitID(pe.ID);
                    br.MoveStreamPosition(122, 0);
                    SyncBWwithBR(ref bw);
                    byte[] original = br.ReadBytes(3);
                    bw.WriteNonByteAlignedBits(Convert.ToString(pe.ShoeModel, 2).PadLeft(12, '0'), original);
                    SyncBRwithBW(bw);

                    original = br.ReadBytes(2);
                    bw.WriteNonByteAlignedBits(
                        Convert.ToString((byte) Enum.Parse(typeof (ShoeBrand), pe.ShoeBrand.ToString()), 2).PadLeft(3, '0'), original);
                    SyncBRwithBW(bw);

                    MoveStreamToPortraitID(pe.ID);
                    br.MoveStreamPosition(98, 0);
                    SyncBWwithBR(ref bw);

                    bw.WriteNonByteAlignedBits(ConvertHexColorStringToBinaryString(pe.ShHomeTeam1.Substring(1)), br.ReadBytes(5));
                    SyncBRwithBW(bw);
                    bw.WriteNonByteAlignedBits(ConvertHexColorStringToBinaryString(pe.ShHomeTeam2.Substring(1)), br.ReadBytes(5));
                    SyncBRwithBW(bw);
                    bw.WriteNonByteAlignedBits(ConvertHexColorStringToBinaryString(pe.ShHomeBase.Substring(1)), br.ReadBytes(5));
                    SyncBRwithBW(bw);
                    bw.WriteNonByteAlignedBits(ConvertHexColorStringToBinaryString(pe.ShAwayBase.Substring(1)), br.ReadBytes(5));
                    SyncBRwithBW(bw);
                    bw.WriteNonByteAlignedBits(ConvertHexColorStringToBinaryString(pe.ShAwayTeam2.Substring(1)), br.ReadBytes(5));
                    SyncBRwithBW(bw);
                    bw.WriteNonByteAlignedBits(ConvertHexColorStringToBinaryString(pe.ShAwayTeam1.Substring(1)), br.ReadBytes(5));
                    SyncBRwithBW(bw);

                    br.MoveStreamPosition(3, 5);
                    SyncBWwithBR(ref bw);
                    bw.WriteNonByteAlignedBits(pe.ShCustomClr ? "1" : "0", br.ReadBytes(1));
                    //


                    //Hair
                    MoveStreamToPortraitID(pe.ID);
                    br.MoveStreamPosition(127, 1);
                    SyncBWwithBR(ref bw);
                    bw.WriteNonByteAlignedBits(
                        Convert.ToString((byte) Enum.Parse(typeof (CAPHairType), pe.CAPHairType.ToString()), 2).PadLeft(6, '0'),
                        br.ReadBytes(2));
                    //

                    //
                    MoveStreamToPortraitID(pe.ID);
                    br.MoveStreamPosition(5, 4);
                    SyncBWwithBR(ref bw);
                    bw.WriteNonByteAlignedByte(pe.JerseyNumber, br.ReadBytes(2));
                    //

                    //
                    MoveStreamToPortraitID(pe.ID);
                    br.MoveStreamPosition(126, 3);
                    SyncBWwithBR(ref bw);
                    bw.WriteNonByteAlignedBits(
                        Convert.ToString((byte) Enum.Parse(typeof (BodyType), pe.BodyType.ToString()), 2).PadLeft(2, '0'), br.ReadBytes(2));
                    SyncBRwithBW(bw);
                    bw.WriteNonByteAlignedBits(
                        Convert.ToString((byte) Enum.Parse(typeof (MuscleTone), pe.MuscleTone.ToString()), 2).PadLeft(1, '0'),
                        br.ReadBytes(1));

                    MoveStreamToPortraitID(pe.ID);
                    br.MoveStreamPosition(128, 3);
                    SyncBWwithBR(ref bw);
                    bw.WriteNonByteAlignedBits(
                        Convert.ToString((byte) Enum.Parse(typeof (EyeColor), pe.EyeColor.ToString()), 2).PadLeft(3, '0'), br.ReadBytes(2));
                    //

                    MoveStreamToPortraitID(pe.ID);
                    br.MoveStreamPosition(314, 2);
                    SyncBWwithBR(ref bw);
                    bw.WriteNonByteAlignedBits(
                        Convert.ToString((byte)Enum.Parse(typeof(ClothesType), pe.ClothesType.ToString()), 2).PadLeft(2, '0'), br.ReadBytes(2));


                    br.BaseStream.Position = prevPos;
                    br.InBytePosition = prevPosIn;
                    SyncBWwithBR(ref bw);
                    //

                    original = br.ReadBytes(2);
                    string s = pe.SSList[0].ToString();
                    bw.WriteNonByteAlignedBits(ConvertSignatureSkillToBinary(s), original);
                    SyncBRwithBW(bw);

                    original = br.ReadBytes(2);
                    s = pe.SSList[1].ToString();
                    bw.WriteNonByteAlignedBits(ConvertSignatureSkillToBinary(s), original);
                    SyncBRwithBW(bw);

                    br.MoveStreamPosition(0, 14);
                    bw.MoveStreamPosition(0, 14);

                    original = br.ReadBytes(2);
                    s = pe.SSList[2].ToString();
                    bw.WriteNonByteAlignedBits(ConvertSignatureSkillToBinary(s), original);
                    SyncBRwithBW(bw);

                    original = br.ReadBytes(2);
                    s = pe.SSList[3].ToString();
                    bw.WriteNonByteAlignedBits(ConvertSignatureSkillToBinary(s), original);
                    SyncBRwithBW(bw);

                    original = br.ReadBytes(2);
                    s = pe.SSList[4].ToString();
                    bw.WriteNonByteAlignedBits(ConvertSignatureSkillToBinary(s), original);
                    SyncBRwithBW(bw);


                    //Ratings
                    MoveStreamToFirstSS(i);
                    br.MoveStreamPosition(14, 3);
                    SyncBWwithBR(ref bw);
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
                        bw.WriteNonByteAlignedByte(byteToWrite, br.ReadBytes(2));
                        SyncBRwithBW(bw);
                    }

                    //Tendencies
                    MoveStreamToFirstSS(i);
                    br.MoveStreamPosition(51, 3);
                    SyncBWwithBR(ref bw);
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
                        bw.WriteNonByteAlignedByte(byteToWrite, br.ReadBytes(2));
                        SyncBRwithBW(bw);
                    }

                    //Hot Spots
                    MoveStreamToFirstSS(i);
                    br.MoveStreamPosition(120, 3);
                    SyncBWwithBR(ref bw);
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
                        bw.WriteNonByteAlignedByte(byteToWrite, br.ReadBytes(2));
                        SyncBRwithBW(bw);
                    }

                    #region Contracts

                    MoveStreamToPortraitID(pe.ID);
                    br.MoveStreamPosition(154, 0);
                    SyncBWwithBR(ref bw);
                    bw.WriteNonByteAlignedBits(
                        Convert.ToString((byte) Enum.Parse(typeof (ContractOption), pe.ContractOpt.ToString()), 2).PadLeft(2, '0'),
                        br.ReadBytes(2));
                    SyncBRwithBW(bw);

                    MoveStreamToPortraitID(pe.ID);
                    br.MoveStreamPosition(177, 5);
                    SyncBWwithBR(ref bw);
                    bw.WriteNonByteAlignedBits(pe.ContNoTrade ? "1" : "0", br.ReadBytes(1));
                    SyncBRwithBW(bw);

                    MoveStreamToPortraitID(pe.ID);
                    br.MoveStreamPosition(214, 0);
                    SyncBWwithBR(ref bw);
                    bw.WriteNonByteAlignedBits(Convert.ToString(pe.ContractY1, 2).PadLeft(32, '0'), br.ReadBytes(5));
                    SyncBRwithBW(bw);
                    bw.WriteNonByteAlignedBits(Convert.ToString(pe.ContractY2, 2).PadLeft(32, '0'), br.ReadBytes(5));
                    SyncBRwithBW(bw);
                    bw.WriteNonByteAlignedBits(Convert.ToString(pe.ContractY3, 2).PadLeft(32, '0'), br.ReadBytes(5));
                    SyncBRwithBW(bw);
                    bw.WriteNonByteAlignedBits(Convert.ToString(pe.ContractY4, 2).PadLeft(32, '0'), br.ReadBytes(5));
                    SyncBRwithBW(bw);
                    bw.WriteNonByteAlignedBits(Convert.ToString(pe.ContractY5, 2).PadLeft(32, '0'), br.ReadBytes(5));
                    SyncBRwithBW(bw);
                    bw.WriteNonByteAlignedBits(Convert.ToString(pe.ContractY6, 2).PadLeft(32, '0'), br.ReadBytes(5));
                    SyncBRwithBW(bw);
                    bw.WriteNonByteAlignedBits(Convert.ToString(pe.ContractY7, 2).PadLeft(32, '0'), br.ReadBytes(5));
                    SyncBRwithBW(bw);

                    #endregion

                    MoveStreamToPortraitID(pe.ID);
                    br.MoveStreamPosition(126, 6);
                    SyncBWwithBR(ref bw);
                    bw.WriteNonByteAlignedBits(Convert.ToString(pe.Skintone, 2).PadLeft(3, '0'), br.ReadBytes(2));
                    SyncBRwithBW(bw);

                    br.MoveStreamPosition(0, 6);
                    SyncBWwithBR(ref bw);
                    bw.WriteNonByteAlignedBits(
                        Convert.ToString((byte) Enum.Parse(typeof (HairColor), pe.CAPHairClr.ToString()), 2).PadLeft(4, '0'),
                        br.ReadBytes(2));
                    SyncBRwithBW(bw);

                    br.MoveStreamPosition(0, 3);
                    SyncBWwithBR(ref bw);
                    bw.WriteNonByteAlignedBits(Convert.ToString(pe.CAPEyebrow, 2).PadLeft(4, '0'), br.ReadBytes(2));
                    SyncBRwithBW(bw);
                    br.MoveStreamPosition(0, 6);
                    SyncBWwithBR(ref bw);
                    bw.WriteNonByteAlignedBits(Convert.ToString(pe.CAPMstch, 2).PadLeft(3, '0'), br.ReadBytes(2));
                    SyncBRwithBW(bw);
                    bw.WriteNonByteAlignedBits(
                        Convert.ToString((byte) Enum.Parse(typeof (HairColor), pe.CAPFclHairClr.ToString()), 2).PadLeft(4, '0'),
                        br.ReadBytes(2));
                    SyncBRwithBW(bw);
                    bw.WriteNonByteAlignedBits(Convert.ToString(pe.CAPBeard, 2).PadLeft(4, '0'), br.ReadBytes(2));
                    SyncBRwithBW(bw);
                    bw.WriteNonByteAlignedBits(Convert.ToString(pe.CAPGoatee, 2).PadLeft(5, '0'), br.ReadBytes(2));
                    SyncBRwithBW(bw);

                    MoveStreamToPortraitID(pe.ID);
                    br.MoveStreamPosition(178, 0);
                    SyncBWwithBR(ref bw);
                    for (int j = 0; j < 15; j++)
                    {
                        bw.WriteNonByteAlignedBits(Convert.ToString(pe.DunkPackages[j], 2).PadLeft(8, '0'), br.ReadBytes(2));
                        SyncBRwithBW(bw);
                    }

                    MoveStreamToPortraitID(pe.ID);
                    br.MoveStreamPosition(193, 0);
                    SyncBWwithBR(ref bw);
                    bw.WriteNonByteAlignedByte(Convert.ToByte(pe.SigFT), br.ReadBytes(2));
                    SyncBRwithBW(bw);
                    bw.WriteNonByteAlignedByte(Convert.ToByte(pe.SigShtForm), br.ReadBytes(2));
                    SyncBRwithBW(bw);
                    bw.WriteNonByteAlignedByte(Convert.ToByte(pe.SigShtBase), br.ReadBytes(2));
                    SyncBRwithBW(bw);

                    MoveStreamToPortraitID(pe.ID);
                    br.MoveStreamPosition(24, 1);
                    SyncBWwithBR(ref bw);
                    bw.WriteNonByteAlignedBits(Convert.ToString(pe.InjuryType, 2).PadLeft(7, '0'), br.ReadBytes(2));
                    SyncBRwithBW(bw);
                    br.MoveStreamPosition(3, 0);
                    SyncBWwithBR(ref bw);
                    bw.WriteNonByteAlignedBits(Convert.ToString(pe.InjuryDays, 2).PadLeft(16, '0'), br.ReadBytes(3));
                    SyncBRwithBW(bw);

                    MoveStreamToPortraitID(pe.ID);
                    br.MoveStreamPosition(2, 0);
                    SyncBWwithBR(ref bw);
                    bw.WriteNonByteAlignedBits(Convert.ToString(pe.BirthYear, 2).PadLeft(12, '0'), br.ReadBytes(3));
                    SyncBRwithBW(bw);
                    bw.WriteNonByteAlignedBits(Convert.ToString(pe.BirthMonth, 2).PadLeft(4, '0'), br.ReadBytes(2));
                    SyncBRwithBW(bw);
                    bw.WriteNonByteAlignedBits(Convert.ToString(pe.BirthDay, 2).PadLeft(5, '0'), br.ReadBytes(2));
                    MoveStreamToPortraitID(pe.ID);
                    br.MoveStreamPosition(87, 6);
                    SyncBWwithBR(ref bw);
                    bw.WriteNonByteAlignedBits(Convert.ToString(pe.YearsPro, 2).PadLeft(5, '0'), br.ReadBytes(2));

                    MoveStreamToPortraitID(pe.ID);
                    br.MoveStreamPosition(40, 0);
                    SyncBWwithBR(ref bw);
                    for (int j = 0; j < 22; j++)
                    {
                        bw.WriteNonByteAlignedBits(Convert.ToString(pe.SeasonStats[j], 2).PadLeft(16, '0'), br.ReadBytes(3));
                        SyncBRwithBW(bw);
                    }
                    bw.WriteNonByteAlignedBits(Convert.ToString(pe.PlayoffStats, 2).PadLeft(16, '0'), br.ReadBytes(3));
                }

                bw.Close();
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

        private void SyncBRwithBW(NonByteAlignedBinaryWriter bw)
        {
            br.BaseStream.Position = bw.BaseStream.Position;
            br.InBytePosition = bw.InBytePosition;
        }

        private void SyncBWwithBR(ref NonByteAlignedBinaryWriter bw)
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

            for (int i = 0; i < teamsList.Count - 1; i++)
            {
                MoveStreamToCurrentTeamRoster(i);
                WriteRosterRow(i, 18);

                //
                MoveStreamToCurrentTeamRoster(i);
                br.MoveStreamPosition(158, 0);
                if (saveType == SaveType.Roster)
                    br.MoveStreamPosition(8, 0);

                var bw = new NonByteAlignedBinaryWriter(new FileStream(currentFile, FileMode.Open));
                SyncBWwithBR(ref bw);

                int hc = teamsList[i].StHeadCoach;
                bw.WriteNonByteAlignedBits(Convert.ToString(hc, 2).PadLeft(16, '0'), br.ReadBytes(3));
                //

                //
                MoveStreamToCurrentTeamRoster(i);
                br.MoveStreamPosition(170, 0);

                SyncBWwithBR(ref bw);

                int ac = teamsList[i].StAsstCoach;
                bw.WriteNonByteAlignedBits(Convert.ToString(ac, 2).PadLeft(16, '0'), br.ReadBytes(3));

                bw.Close();
                //
            }

            // Free Agents
            int faRow = 999;
            br.BaseStream.Position = 853505;
            br.InBytePosition = 6;

            if (mode == Mode.X360)
            {
                br.BaseStream.Position = 923137;
            }
            else if (mode == Mode.PCNov10 || mode == Mode.X360Nov10)
            {
                br.BaseStream.Position += 1911;
                br.InBytePosition = 2;
            }
            else if (mode == Mode.Custom)
            {
                MoveStreamToFirstRoster();
                br.BaseStream.Position -= 9406;
            }

            MoveStreamForSaveType();

            WriteRosterRow(faRow, 512, true);
            //

            if (doCRC)
            {
                RecalculateCRC();
            }

            PopulatePlayersTab();

            updateStatus("Teams saved.");
        }

        private void WriteRosterRow(int teamID, int spotsToWrite, bool isFArow = false)
        {
            var bw = new NonByteAlignedBinaryWriter(new FileStream(currentFile, FileMode.Open));

            long startOfRoster = br.BaseStream.Position;
            int startOfRosterBit = br.InBytePosition;
            SyncBWwithBR(ref bw);

            TeamEntry te = teamsList.Single(t => t.ID == teamID);

            int plNum = te.PlNum;
            for (int i = 0; i < spotsToWrite; i++)
            {
                WriteRosterSpot(te.RosterSpots[i].ToString(), bw, i, plNum);
            }

            br.BaseStream.Position = startOfRoster;
            br.InBytePosition = startOfRosterBit;

            if (!isFArow)
                br.MoveStreamPosition(125, 0);
            else
                br.MoveStreamPosition(4001, 0);
            br.MoveStreamPosition(0, -1);

            SyncBWwithBR(ref bw);
            byte[] original = br.ReadBytes(2);
            SyncBRwithBW(bw);
            bw.WriteNonByteAlignedBits(Convert.ToString(Convert.ToUInt16(plNum.ToString()),2).PadLeft(9,'0'), original);

            bw.Close();
        }

        private object GetCellValue(DataGrid dataGrid, int row, int col)
        {
            var dataRowView = dataGrid.Items[row] as DataRowView;
            if (dataRowView != null)
                return dataRowView.Row.ItemArray[col];

            return null;
        }
        
        private void WriteRosterSpot(string text, NonByteAlignedBinaryWriter bw, int spot, int plNum)
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
            Write2ByteStringToRoster(text, bw, first2);
        }

        private void Write2ByteStringToRoster(string p, NonByteAlignedBinaryWriter bw, byte[] first2 = null)
        {
            bw.BaseStream.Position = br.BaseStream.Position;
            bw.InBytePosition = br.InBytePosition;

            UInt16 id = Convert.ToUInt16(p);
            byte[] original;
            string s = "";

            if (first2 != null)
            {
                original = br.ReadBytes(5);
                s = NonByteAlignedBinaryReader.ByteArrayToBitString(first2);
            }
            else
            {
                original = br.ReadBytes(3);
            }
            s += Convert.ToString(id, 2).PadLeft(16, '0');
            bw.WriteNonByteAlignedBits(s, original);

            br.BaseStream.Position = bw.BaseStream.Position;
            br.InBytePosition = bw.InBytePosition;
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
                PopulateJerseysTab();
                PopulateTeamsTab();
                PopulatePlayersTab();
            }
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
                string[] lines = Tools.SplitLinesToArray(Clipboard.GetText());
                if (lines[0].Contains("ID") == false)
                {
                    MessageBox.Show("The pasted data must have the column headers in the first row.");
                    return;
                }
                List<Dictionary<string, string>> dictList = CSV.DictionaryListFromTSV(lines);

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
                            TryParseTeamDictionaryList(ref te, dict);
                            teamsList[i] = te;
                            break;
                        }
                    }
                }

                dgTeams.ItemsSource = null;
                dgTeams.ItemsSource = teamsList;

                updateStatus("Data pasted into Teams table successfully.");
            }
        }

        private void TryParseTeamDictionaryList(ref TeamEntry te, Dictionary<string, string> dict)
        {
            int rCount = teamsList[0].RosterSpots.Count;

            te.PlNum = te.PlNum.TrySetValue(dict, "PlNum", true);
            for (int i = 0; i < rCount; i++)
            {
                te.RosterSpots[i] = te.RosterSpots[i].TrySetValue(dict, "R" + (i + 1).ToString(), true);
            }
            te.StAsstCoach = te.StAsstCoach.TrySetValue(dict, "StAsstCoach", true);
            te.StHeadCoach = te.StHeadCoach.TrySetValue(dict, "StHeadCoach", true);
        }

        private void FixPlNumAndOrder()
        {
            for (int i = 0; i < teamsList.Count; i++)
            {
                //if (i == teamsList.Count - 1) System.Diagnostics.Debugger.Break();

                int spotsCount = teamsList[i].RosterSpots.Count;
                for (int j = 0; j < spotsCount; j++)
                {
                    int curCell = teamsList[i].RosterSpots[j];
                    if (curCell == -1) // || String.IsNullOrWhiteSpace(curCell))
                    {
                        for (int k = spotsCount - 1; k > j; k--)
                        {
                            int subCell = teamsList[i].RosterSpots[k];
                            if (subCell != -1) // || String.IsNullOrWhiteSpace(subCell)))
                            {
                                teamsList[i].RosterSpots[j] = subCell;
                                teamsList[i].RosterSpots[k] = -1;
                                break;
                            }
                        }
                    }
                }
                int count = 0;
                for (int j = 0; j < spotsCount; j++)
                {
                    int curCell = teamsList[i].RosterSpots[j];
                    if (curCell != -1) // || String.IsNullOrWhiteSpace(curCell))
                    {
                        count++;
                    }
                    else
                    {
                        break;
                    }
                }
                teamsList[i].PlNum = count;
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
                string[] lines = Tools.SplitLinesToArray(Clipboard.GetText());
                if (lines[0].Contains("ID") == false)
                {
                    MessageBox.Show("The pasted data must have the column headers in the first row.");
                    return;
                }
                List<Dictionary<string, string>> dictList = CSV.DictionaryListFromTSV(lines);

                dgPlayers.CommitEdit();
                dgPlayers.CancelEdit();

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
                        Trace.WriteLine(string.Format("{0}: Couldn't parse Player ID on line {1}. Skipping.", DateTime.Now, (index + 2)));
                        continue;
                    }
                    for (int i = 0; i < playersList.Count; i++)
                    {
                        if (playersList[i].ID == ID)
                        {
                            PlayerEntry pe = playersList[i];
                            TryParsePlayerDictionaryList(ref pe, dict);
                            playersList[i] = pe;
                            break;
                        }
                    }
                }

                dgPlayers.ItemsSource = null;
                dgPlayers.ItemsSource = playersList;

                updateStatus("Data pasted into Players table successfully.");
            }
        }

        private void TryParsePlayerDictionaryList(ref PlayerEntry pe, Dictionary<string, string> dict)
        {
            pe.CFID = pe.CFID.TrySetValue(dict, "CFID", true);
            pe.PlType = pe.PlType.TrySetValue(dict, "PlType", true);
            pe.Position1 = pe.Position1.TrySetValue(dict, "Position1", true);
            pe.Position2 = pe.Position2.TrySetValue(dict, "Position2", true);
            pe.Height = pe.Height.TrySetValue(dict, "Height", true);
            pe.Weight = pe.Weight.TrySetValue(dict, "Weight", true);
            pe.GenericF = pe.GenericF.TrySetValue(dict, "GenericF", true);
            pe.PortraitID = pe.PortraitID.TrySetValue(dict, "PortraitID", true);
            pe.ASAID = pe.ASAID.TrySetValue(dict, "ASAID", true);
            pe.ShoeBrand = pe.ShoeBrand.TrySetValue(dict, "ShoeBrand", true);
            pe.ShoeModel = pe.ShoeModel.TrySetValue(dict, "ShoeModel", true);
            pe.JerseyNumber = pe.JerseyNumber.TrySetValue(dict, "Number", true);
            pe.EyeColor = pe.EyeColor.TrySetValue(dict, "EyeColor", true);
            pe.CAPHairType = pe.CAPHairType.TrySetValue(dict, "CAPHairType", true);
            pe.BodyType = pe.BodyType.TrySetValue(dict, "BodyType", true);
            pe.MuscleTone = pe.MuscleTone.TrySetValue(dict, "MuscleTone", true);
            pe.ShHomeTeam1 = pe.ShHomeTeam1.TrySetValue(dict, "ShHomeTeam1", true);
            pe.ShHomeTeam2 = pe.ShHomeTeam2.TrySetValue(dict, "ShHomeTeam2", true);
            pe.ShHomeBase = pe.ShHomeBase.TrySetValue(dict, "ShHomeBase", true);
            pe.ShAwayTeam1 = pe.ShAwayTeam1.TrySetValue(dict, "ShAwayTeam1", true);
            pe.ShAwayTeam2 = pe.ShAwayTeam2.TrySetValue(dict, "ShAwayTeam2", true);
            pe.ShAwayBase = pe.ShAwayBase.TrySetValue(dict, "ShAwayBase", true);
            pe.SSList[0] = pe.SSList[0].TrySetValue(dict, "SS1", true);
            pe.SSList[1] = pe.SSList[1].TrySetValue(dict, "SS2", true);
            pe.SSList[2] = pe.SSList[2].TrySetValue(dict, "SS3", true);
            pe.SSList[3] = pe.SSList[3].TrySetValue(dict, "SS4", true);
            pe.SSList[4] = pe.SSList[4].TrySetValue(dict, "SS5", true);
            pe.PlayStyle = pe.PlayStyle.TrySetValue(dict, "PlayStyle", true);
            pe.PlayType1 = pe.PlayType1.TrySetValue(dict, "PlayType1", true);
            pe.PlayType2 = pe.PlayType2.TrySetValue(dict, "PlayType2", true);
            pe.PlayType3 = pe.PlayType3.TrySetValue(dict, "PlayType3", true);
            pe.PlayType4 = pe.PlayType4.TrySetValue(dict, "PlayType4", true);
            pe.ClothesType = pe.ClothesType.TrySetValue(dict, "ClothesType", true);
            pe.InjuryDays = pe.InjuryDays.TrySetValue(dict, "InjuryDays", true);
            pe.InjuryType = pe.InjuryType.TrySetValue(dict, "InjuryType", true);
            pe.BirthYear = pe.BirthYear.TrySetValue(dict, "BirthYear", true);
            pe.BirthMonth = pe.BirthMonth.TrySetValue(dict, "BirthMonth", true);
            pe.BirthDay = pe.BirthDay.TrySetValue(dict, "BirthDay", true);
            pe.YearsPro = pe.YearsPro.TrySetValue(dict, "YearsPro", true);
            
            string[] rtNames = Enum.GetNames(typeof (Rating));
            foreach (string rtName in rtNames)
            {
                var curInd = (byte) Enum.Parse(typeof (Rating), rtName);
                pe.Ratings[curInd] = pe.Ratings[curInd].TrySetValue(dict, "R" + rtName, true);
            }

            string[] tNames = Enum.GetNames(typeof (Tendency));
            foreach (string tName in tNames)
            {
                var curInd = (byte) Enum.Parse(typeof (Tendency), tName);
                pe.Tendencies[curInd] = pe.Tendencies[curInd].TrySetValue(dict, "T" + tName, true);
            }

            string[] hsNames = Enum.GetNames(typeof (HotSpot));
            foreach (string hsName in hsNames)
            {
                var curInd = (byte) Enum.Parse(typeof (HotSpot), hsName);
                pe.HotSpots[curInd] = pe.HotSpots[curInd].TrySetValue(dict, "HS" + hsName, true);
            }

            pe.ContractOpt = pe.ContractOpt.TrySetValue(dict, "ContractOpt", true);
            pe.ContNoTrade = pe.ContNoTrade.TrySetValue(dict, "ContNoTrade", true);
            pe.ContractY1 = pe.ContractY1.TrySetValue(dict, "ContractY1", true);
            pe.ContractY2 = pe.ContractY2.TrySetValue(dict, "ContractY2", true);
            pe.ContractY3 = pe.ContractY3.TrySetValue(dict, "ContractY3", true);
            pe.ContractY4 = pe.ContractY4.TrySetValue(dict, "ContractY4", true);
            pe.ContractY5 = pe.ContractY5.TrySetValue(dict, "ContractY5", true);
            pe.ContractY6 = pe.ContractY6.TrySetValue(dict, "ContractY6", true);
            pe.ContractY7 = pe.ContractY7.TrySetValue(dict, "ContractY7", true);

            for (int i = 0; i < 15; i++)
            {
                pe.DunkPackages[i] = pe.DunkPackages[i].TrySetValue(dict, "DunkPkg" + i, true);
            } 
            
            for (int i = 0; i < 22; i++)
            {
                pe.SeasonStats[i] = pe.SeasonStats[i].TrySetValue(dict, "SeasonStats" + i, true);
            }
            pe.PlayoffStats = pe.PlayoffStats.TrySetValue(dict, "PlayoffStats", true);

            pe.Skintone = pe.Skintone.TrySetValue(dict, "Skintone", true);
            pe.CAPHairClr = pe.CAPHairClr.TrySetValue(dict, "CAPHairClr", true);
            pe.CAPEyebrow = pe.CAPEyebrow.TrySetValue(dict, "CAPEyebrow", true);
            pe.CAPMstch = pe.CAPMstch.TrySetValue(dict, "CAPMstch", true);
            pe.CAPFclHairClr = pe.CAPFclHairClr.TrySetValue(dict, "CAPFclHairClr", true);
            pe.CAPBeard = pe.CAPBeard.TrySetValue(dict, "CAPBeard", true);
            pe.CAPGoatee = pe.CAPGoatee.TrySetValue(dict, "CAPGoatee", true);

            pe.SigFT = pe.SigFT.TrySetValue(dict, "SigFT", true);
            pe.SigShtForm = pe.SigShtForm.TrySetValue(dict, "SigShtForm", true);
            pe.SigShtBase = pe.SigShtBase.TrySetValue(dict, "SigShtBase", true);

            pe.TeamID1 = pe.TeamID1.TrySetValue(dict, "TeamID1", true);
            pe.TeamID2 = pe.TeamID2.TrySetValue(dict, "TeamID2", true);
        }

        private void dgPlayers_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            AnyDataGrid_MouseDoubleClick(sender, e);
        }

        private void btnSaveJerseys_Click(object sender, RoutedEventArgs e)
        {
            NonByteAlignedBinaryWriter bw;
            using (bw = new NonByteAlignedBinaryWriter(File.Open(currentFile, FileMode.Open, FileAccess.Write, FileShare.ReadWrite)))
            {
                for (int i = 0; i < jerseysList.Count; i++)
                {
                    JerseyEntry je = jerseysList[i];

                    MoveStreamToFirstJersey();
                    br.MoveStreamPosition(68*i, 0);
                    SyncBWwithBR(ref bw);

                    bw.WriteNonByteAlignedBits(
                        Convert.ToString((ushort) Enum.Parse(typeof(JerseyType), je.GID.ToString()), 2).PadLeft(16, '0'), br.ReadBytes(3));
                    SyncBRwithBW(bw);

                    br.MoveStreamPosition(0, 6);
                    SyncBWwithBR(ref bw);

                    bw.WriteNonByteAlignedBits(Convert.ToString((byte) Enum.Parse(typeof(NeckType), je.Neck.ToString()),2).PadLeft(3, '0'), br.ReadBytes(2));
                    SyncBRwithBW(bw);

                    br.MoveStreamPosition(0, 2);
                    SyncBWwithBR(ref bw);

                    bw.WriteNonByteAlignedBits(Convert.ToString((byte)Enum.Parse(typeof(SockColor), je.SockClr.ToString()), 2), br.ReadBytes(1));
                    SyncBRwithBW(bw);

                    br.MoveStreamPosition(0, 4);
                    SyncBWwithBR(ref bw);
                    br.MoveStreamPosition(10, 0);
                    SyncBWwithBR(ref bw);

                    bw.WriteNonByteAlignedBits(
                        NonByteAlignedBinaryReader.ByteArrayToBitString(
                            Tools.HexStringToByteArray(JerseyEntry.JerseyNames[je.Name.ToString()])), br.ReadBytes(5));
                    SyncBRwithBW(bw);

                    br.MoveStreamPosition(3, 5);
                    SyncBWwithBR(ref bw);

                    bw.WriteNonByteAlignedBits(ConvertHexColorStringToBinaryString(je.TeamColor1.Substring(1)), br.ReadBytes(5));
                    SyncBRwithBW(bw);
                    bw.WriteNonByteAlignedBits(ConvertHexColorStringToBinaryString(je.TeamColor2.Substring(1)), br.ReadBytes(5));
                    SyncBRwithBW(bw);
                    bw.WriteNonByteAlignedBits(ConvertHexColorStringToBinaryString(je.TeamColor3.Substring(1)), br.ReadBytes(5));
                    SyncBRwithBW(bw);
                    bw.WriteNonByteAlignedBits(ConvertHexColorStringToBinaryString(je.TeamColor4.Substring(1)), br.ReadBytes(5));
                    SyncBRwithBW(bw);
                    bw.WriteNonByteAlignedBits(ConvertHexColorStringToBinaryString(je.TeamColor5.Substring(1)), br.ReadBytes(5));
                    SyncBRwithBW(bw);
                    bw.WriteNonByteAlignedBits(ConvertHexColorStringToBinaryString(je.TeamColor6.Substring(1)), br.ReadBytes(5));
                    SyncBRwithBW(bw);
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
                            typeof (PlayerEntry).GetProperty(dgci.Column.Header.ToString()).GetValue(dgci.Item, null).ToString().Substring(1);
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
                }
            }
        }

        private void dgJerseys_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.V && Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                string[] lines = Tools.SplitLinesToArray(Clipboard.GetText());
                if (lines[0].Contains("ID") == false)
                {
                    MessageBox.Show("The pasted data must have the column headers in the first row.");
                    return;
                }
                List<Dictionary<string, string>> dictList = CSV.DictionaryListFromTSV(lines);

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
                            TryParseJerseyDictionaryList(ref je, dict);
                            jerseysList[i] = je;
                            break;
                        }
                    }
                }

                dgJerseys.ItemsSource = null;
                dgJerseys.ItemsSource = jerseysList;

                updateStatus("Data pasted into Jerseys table successfully.");
            }
        }


        private void TryParseJerseyDictionaryList(ref JerseyEntry je, Dictionary<string, string> dict)
        {
            je.GID = je.GID.TrySetValue(dict, "Type", true);
            je.Name = je.Name.TrySetValue(dict, "Name", true);
            je.Neck = je.Neck.TrySetValue(dict, "Neck", true);
            je.SockClr = je.SockClr.TrySetValue(dict, "SockClr", true);
            je.TeamColor1 = je.TeamColor1.TrySetValue(dict, "TeamColor1", true);
            je.TeamColor2 = je.TeamColor2.TrySetValue(dict, "TeamColor2", true);
            je.TeamColor3 = je.TeamColor3.TrySetValue(dict, "TeamColor3", true);
            je.TeamColor4 = je.TeamColor4.TrySetValue(dict, "TeamColor4", true);
            je.TeamColor5 = je.TeamColor5.TrySetValue(dict, "TeamColor5", true);
            je.TeamColor6 = je.TeamColor6.TrySetValue(dict, "TeamColor6", true);
        }

        private void dgJerseys_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            AnyDataGrid_MouseDoubleClick(sender, e);
        }

        private void AnyDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGrid && (e == null || e.MouseDevice.DirectlyOver is DataGridCell || e.MouseDevice.DirectlyOver is TextBox))
            {
                var grid = sender as DataGrid;
                if (grid != null && grid.CurrentItem != null)
                {
                    DataGridCellInfo dgci = grid.CurrentCell;

                    if (colorColumns.Contains(dgci.Column.Header.ToString()))
                    {
                        var c = new Color();
                        try
                        {
                            string s =
                                dgci.Item.GetType().GetProperty(dgci.Column.Header.ToString()).GetValue(dgci.Item, null).ToString().
                                    Substring(1);
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
                            dgci.Item.GetType().GetProperty(dgci.Column.Header.ToString()).SetValue(dgci.Item, cd.SelectedColor.ToString(),
                                                                                                    null);
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
                            typeof (JerseyEntry).GetProperty(dgci.Column.Header.ToString()).GetValue(dgci.Item, null).ToString().Substring(1);
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
            if (sender == rctJerseyColor)
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
            else if (sender == rctPlayerColor)
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
                    var foundIDs = FindMatchingPlayers(sw);
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
                var foundIDs = FindMatchingPlayers(sw);
                foundIDList = foundIDs;

                if (foundIDs.Count > 0)
                {
                    curFoundID = foundIDs[0];
                    dgPlayers.ScrollIntoView(playersList[foundIDs[0]]);
                    updateStatus(string.Format("Found player matching filters: ID {0} ({1} matching players found)", foundIDs[0], foundIDs.Count));

                    if (sw.ReplaceFilters.Count > 0)
                    {
                        foreach (string filter in sw.ReplaceFilters)
                        {
                            string[] parts = filter.Split(' ');

                            bool isArray = false;
                            int index = -1;
                            string prop = parts[0];
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
                            int toReplace = Convert.ToInt32(parts[2]);

                            foreach (int id in foundIDs)
                            {
                                if (isArray)
                                {
                                    var p = typeof (PlayerEntry).GetProperty(prop).GetValue(playersList[id], null) as IList;
                                    Type type = p.GetType().GetGenericArguments()[0];
                                    try
                                    {
                                        p[index] = Convert.ChangeType(toReplace, type);
                                    }
                                    catch (Exception)
                                    {
                                        MessageBox.Show(String.Format("{0} is not a valid value for {1}. Replace cancelled.", toReplace, prop));
                                    }
                                    typeof (PlayerEntry).GetProperty(prop).SetValue(playersList[id], p, null);
                                }
                                else
                                {
                                    typeof (PlayerEntry).GetProperty(prop).SetValue(playersList[id], toReplace, null);
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
                var newFoundIDs = new List<int>();

                bool isArray = false;
                int index = -1;
                string prop = parts[0];
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
                    int val;
                    if (isArray)
                    {
                        var p = typeof (PlayerEntry).GetProperty(prop).GetValue(playersList[id], null) as IList;
                        val = Convert.ToInt32(p[index]);
                    }
                    else
                    {
                        val = Convert.ToInt32(typeof (PlayerEntry).GetProperty(prop).GetValue(playersList[id], null));
                    }
                    int toFind = Convert.ToInt32(parts[2]);
                    switch (parts[1])
                    {
                        case "=":
                            if (val == toFind)
                            {
                                newFoundIDs.Add(id);
                            }
                            break;
                        case "<":
                            if (val < toFind)
                            {
                                newFoundIDs.Add(id);
                            }
                            break;
                        case "<=":
                            if (val <= toFind)
                            {
                                newFoundIDs.Add(id);
                            }
                            break;
                        case ">":
                            if (val > toFind)
                            {
                                newFoundIDs.Add(id);
                            }
                            break;
                        case ">=":
                            if (val >= toFind)
                            {
                                newFoundIDs.Add(id);
                            }
                            break;
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
            Process.Start(AppPath + @"\Readme.txt");
        }

        private void btnEnumerables_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(AppPath + @"\Enumerables.txt");
        }

        /// <summary>
        /// Checks for software updates asynchronously.
        /// </summary>
        /// <param name="showMessage">if set to <c>true</c>, a message will be shown even if no update is found.</param>
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
        /// Checks the downloaded version file to see if there's a newer version, and displays a message if needed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="AsyncCompletedEventArgs" /> instance containing the event data.</param>
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
                            changelog += "\n" + updateInfo[j].Replace('\t',' ');
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
            Height = GetRegistrySetting("Height", (int) Height);
            Width = GetRegistrySetting("Width", (int)Width);
            Left = GetRegistrySetting("Left", (int)Left);
            Top = GetRegistrySetting("Top", (int)Top);
            var w = new BackgroundWorker();
            w.DoWork += w_DoWork;
            w.RunWorkerAsync();
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

            var files = Directory.GetFiles(Path.GetDirectoryName(ofd.FileName)).ToList();
            files.Sort();

            foreach (var file in files)
            {
                SearchWindow sw = new SearchWindow();
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
    }

    internal enum Mode
    {
        PC,
        X360,
        PCNov10,
        Custom,
        X360Nov10
    }

    internal enum SaveType
    {
        Roster,
        Association,
        MyCareer,
        DraftClass
    }
}