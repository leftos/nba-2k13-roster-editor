using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using LeftosCommonLibrary;
using Microsoft.Win32;

namespace NBA_2K13_Roster_Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string NBA2K13SavesPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                                @"\2K Sports\NBA 2K13\Saves";

        private NonByteAlignedBinaryReader br;
        private string currentFile;
        private bool doCRC;
        private Mode mode;
        private Dictionary<int, string> names;

        public static string DocsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\NBA 2K13 Roster Editor";
        private DataTable dt;

        private DispatcherTimer timer;

        private ObservableCollection<PlayerEntry> playersList { get; set; }
        private ObservableCollection<Option> optionsList { get; set; }
        private ObservableCollection<TeamEntry> teamsList { get; set; }

        private SaveType saveType;

        public MainWindow()
        {
            InitializeComponent();

            Title += " v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

            if (Directory.Exists(DocsPath) == false)
            {
                Directory.CreateDirectory(DocsPath);
            }

            ReloadOptions();
            dgOptions.ItemsSource = optionsList;

            doCRC = (GetRegistrySetting("CRC", 1) == 1);
            chkRecalculateCRC.IsChecked = doCRC;

            mode = (Mode) Enum.Parse(typeof (Mode), GetRegistrySetting("Mode", "PC"));
            if (mode == Mode.PC)
                btnModePC.IsChecked = true;
            else
                btnMode360.IsChecked = true;

            for (int i = 0; i < 37; i++)
            {
                dgPlayers.Columns.Add(new DataGridTextColumn
                {
                    Header = string.Format("R{0}", Enum.GetName(typeof (Rating), i)),
                    Binding =
                        new Binding { Path = new PropertyPath(string.Format("Ratings[{0}]", i)), Mode = BindingMode.TwoWay }
                });
            }

            for (int i = 0; i < 69; i++)
            {
                dgPlayers.Columns.Add(new DataGridTextColumn
                {
                    Header = string.Format("T{0}", Enum.GetName(typeof (Tendency), i)),
                    Binding =
                        new Binding { Path = new PropertyPath(string.Format("Tendencies[{0}]", i)), Mode = BindingMode.TwoWay }
                });
            }
            
            for (int i = 0; i < 25; i++)
            {
                dgPlayers.Columns.Add(new DataGridTextColumn
                {
                    Header = string.Format("HS{0}", Enum.GetName(typeof (HotSpot), i)),
                    Binding =
                        new Binding { Path = new PropertyPath(string.Format("HotSpots[{0}]", i)), Mode = BindingMode.TwoWay }
                });
            }
            
            playersList = new ObservableCollection<PlayerEntry>();
            dgPlayers.ItemsSource = playersList;

            dgTeams.Columns.Clear();
            dgTeams.Columns.Add(new DataGridTextColumn
                                  {Header = "ID", Binding = new Binding {Path = new PropertyPath("ID"), Mode = BindingMode.TwoWay}});
            dgTeams.Columns.Add(new DataGridTextColumn { Header = "Name", Binding = new Binding { Path = new PropertyPath("Name"), Mode = BindingMode.TwoWay } });
            dgTeams.Columns.Add(new DataGridTextColumn { Header = "StAsstCoach", Binding = new Binding { Path = new PropertyPath("StAsstCoach"), Mode = BindingMode.TwoWay } });
            dgTeams.Columns.Add(new DataGridTextColumn
                                  {Header = "PlNum", Binding = new Binding {Path = new PropertyPath("PlNum"), Mode = BindingMode.TwoWay}});
            for (int i = 0; i < 256; i++)
            {
                dgTeams.Columns.Add(new DataGridTextColumn
                                      {
                                          Header = string.Format("R{0}", (i + 1)),
                                          Binding =
                                              new Binding
                                              {Path = new PropertyPath(string.Format("RosterSpots[{0}]", i)), Mode = BindingMode.TwoWay}
                                      });
            }

            teamsList = new ObservableCollection<TeamEntry>();
            dgTeams.ItemsSource = teamsList;

            timer = new DispatcherTimer();
            timer.Tick += timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 3);
        }

        private void PopulateNamesDictionary()
        {
            names = new Dictionary<int, string>();
            string file = DocsPath + "\\" + GetOption("NamesFile");
            if (File.Exists(file))
            {
                string[] lines = File.ReadAllLines(file);
                for (int index = 0; index < lines.Length; index++)
                {
                    string line = lines[index];
                    string[] parts = line.Split(new char[] {'\t'}, 2);
                    if (parts.Length == 2)
                    {
                        try
                        {
                            int id = Convert.ToInt32(parts[0]);
                            if (names.ContainsKey(id) == false)
                            {
                                names.Add(id, parts[1]);
                            }
                        }
                        catch (Exception e)
                        {
                            Trace.WriteLine(
                                string.Format("{0}: Couldn't convert line {1}'s ID while parsing {2}, value is '{3}'",
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
            optionsList.Add(new Option {Setting = "FirstSSOffsetBit", Value = GetRegistrySetting("FirstSSOffsetBIt", 2)});
            optionsList.Add(new Option {Setting = "LastPlayerID", Value = GetRegistrySetting("LastPlayerID", 1500)});
            optionsList.Add(new Option {Setting = "NamesFile", Value = GetRegistrySetting("NamesFile", "names.txt")});

            PopulateNamesDictionary();
            dgOptions.ItemsSource = optionsList;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            txbStatus.Text = "Ready";
            timer.Stop();
        }

        void updateStatus(string text)
        {
            timer.Stop();
            txbStatus.Text = text;
            timer.Start();
        }

        Dictionary<int, string> teams = new Dictionary<int, string>
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
                                            {29, "Wizards"}
                                        }; 

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.InitialDirectory = NBA2K13SavesPath;
            ofd.Filter = "All compatible NBA 2K13 files (*.ROS; *.FXG; *.CMG)|*.ROS;*.FXG;*.CMG|" +
                         "Roster files (*.ROS)|*.ROS|" +
                         "Association files (*.FXG)|*.FXG|" +
                         "MyCareer files (*.CMG)|*.CMG|" +
                         "All files (*.*)|*.*";
            ofd.DefaultExt = ".ROS";
            ofd.ShowDialog();

            if (ofd.FileName == "")
                return;

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
                default:
                    saveType = SaveType.Roster;
                    break;
            }

            PopulateTeamsTab();

            PopulatePlayersTab();

            updateStatus("Roster loaded.");
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
                PlayerEntry pe = new PlayerEntry();
                MoveStreamToFirstSS(i);
                pe.ID = i;
                ReadInfo(ref pe);
                pe.Name = FindPlayerName(i);
                pe.SSList = ReadSignatureSkills(i);
                pe.Ratings = ReadRatings(i);
                pe.Tendencies = ReadTendencies(i);
                pe.HotSpots = ReadHotSpots(i);
                playersList.Add(pe);
            }
            dgPlayers.ItemsSource = playersList;
        }

        private List<byte> ReadRatings(int playerID)
        {
            var ratings = new List<byte>();

            MoveStreamToFirstSS(playerID);
            br.MoveStreamPosition(14, 3);

            for (int i = 0; i < 37;i++)
            {
                byte b = br.ReadNonByteAlignedByte();
                byte realRating = (byte) (b/3 + 25);
                ratings.Add(realRating);
            }

            return ratings;
        }

        private List<byte> ReadTendencies(int playerID)
        {
            var tend = new List<byte>();

            MoveStreamToFirstSS(playerID);
            br.MoveStreamPosition(51, 3);

            for (int i = 0; i < 69; i++)
            {
                byte b = br.ReadNonByteAlignedByte();
                tend.Add(b);
            }

            return tend;
        }

        private List<byte> ReadHotSpots(int playerID)
        {
            var hs = new List<byte>();

            MoveStreamToFirstSS(playerID);
            br.MoveStreamPosition(120, 3);

            for (int i = 0; i <25;i++)
            {
                byte b = br.ReadNonByteAlignedByte();
                hs.Add(b);
            }

            return hs;
        }

        private void PopulateTeamsTab()
        {
            teamsList = new ObservableCollection<TeamEntry>();
            MoveStreamBeforeFirstRoster();

            TeamEntry te;
            for (int i = 0; i < 30; i++)
            {
                te = new TeamEntry();
                te.ID = i;
                te.Name = teams[i];
                
                br.MoveStreamPosition(720, 2);
                long curOffset = br.BaseStream.Position;
                int curOffsetBit = br.InBytePosition;

                PopulateRosterRow(18, ref te);

                //
                br.BaseStream.Position = 863081;
                br.InBytePosition = 6;
                br.MoveStreamPosition(720 * i, 2 * i);

                if (mode == Mode.X360)
                    br.BaseStream.Position += 69632;

                MoveStreamForSaveType();
                
                te.StAsstCoach =
                    Convert.ToInt16(NonByteAlignedBinaryReader.ByteArrayToBitString(br.ReadNonByteAlignedBytes(2)), 2);
                //

                br.BaseStream.Position = curOffset;
                br.InBytePosition = curOffsetBit;
            }

            // Free Agents
            te = new TeamEntry();
            te.ID = 999;
            te.Name = "Free Agents";

            br.BaseStream.Position = 853505;
            br.InBytePosition = 6;

            if (mode == Mode.X360)
                br.BaseStream.Position += 69632;

            MoveStreamForSaveType();

            PopulateRosterRow(256, ref te, true);
            //

            RelinkTeamsDataGrid();
        }

        private void MoveStreamBeforeFirstRoster()
        {
            long firstRosterOffset = 862911 - 720;
            int firstRosterOffsetBit = 6 - 2;
            br.BaseStream.Position = firstRosterOffset;
            br.InBytePosition = firstRosterOffsetBit;

            if (mode == Mode.X360)
                br.BaseStream.Position += 69632;

            MoveStreamForSaveType();
        }

        private void PopulateRosterRow(int countToRead, ref TeamEntry te, bool isFArow = false)
        {
            var startOfRoster = br.BaseStream.Position;
            var startOfRosterBit = br.InBytePosition;

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

            te.PlNum = Convert.ToInt32(br.ReadNonByteAlignedByte().ToString());
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

        private List<SignatureSkill> ReadSignatureSkills(int playerID)
        {
            MoveStreamToFirstSS(playerID);

            List<SignatureSkill> ssList = new List<SignatureSkill>();

            byte b = NonByteAlignedBinaryReader.BitStringToByte(br.ReadNonByteAlignedBits(6));
            ssList.Add(Enum.IsDefined(typeof(SignatureSkill), b) ? ((SignatureSkill)b) : (SignatureSkill)0);
            b = NonByteAlignedBinaryReader.BitStringToByte(br.ReadNonByteAlignedBits(6));
            ssList.Add(Enum.IsDefined(typeof(SignatureSkill), b) ? ((SignatureSkill)b) : (SignatureSkill)0);
            br.ReadNonByteAlignedBits(14);
            b = NonByteAlignedBinaryReader.BitStringToByte(br.ReadNonByteAlignedBits(6));
            ssList.Add(Enum.IsDefined(typeof(SignatureSkill), b) ? ((SignatureSkill)b) : (SignatureSkill)0);
            b = NonByteAlignedBinaryReader.BitStringToByte(br.ReadNonByteAlignedBits(6));
            ssList.Add(Enum.IsDefined(typeof(SignatureSkill), b) ? ((SignatureSkill)b) : (SignatureSkill)0);
            b = NonByteAlignedBinaryReader.BitStringToByte(br.ReadNonByteAlignedBits(6));
            ssList.Add(Enum.IsDefined(typeof(SignatureSkill), b) ? ((SignatureSkill)b) : (SignatureSkill)0);

            return ssList;
        }

        private string FindPlayerName(int playerID)
        {
            if (names.ContainsKey(playerID))
            {
                return names[playerID];
            }
            else
            {
                return "";
            }
        }

        private void ReadInfo(ref PlayerEntry pe)
        {
            int playerID = pe.ID;

            MoveStreamToCFID(playerID);

            byte[] por = br.ReadNonByteAlignedBytes(2);


            // PlType
            br.MoveStreamPosition(2, 6);

            byte plType = Convert.ToByte(br.ReadNonByteAlignedBits(3),2);

            br.MoveStreamPosition(-3, -1);
            //
            

            br.MoveStreamPosition(28, 0);

            byte[] cf = br.ReadNonByteAlignedBytes(2);

            br.MoveStreamPosition(94, 1);

            bool genericF = br.ReadNonByteAlignedBits(1) == "1";

            br.MoveStreamPosition(137, 6);

            byte[] audio = br.ReadNonByteAlignedBytes(2);


            //Shoe
            MoveStreamToCFID(playerID);

            br.MoveStreamPosition(122, 0);
            int shoeModel = Convert.ToInt32(br.ReadNonByteAlignedBits(12), 2);

            ShoeBrand shoeBrand =
                (ShoeBrand) Enum.Parse(typeof (ShoeBrand), Convert.ToByte(br.ReadNonByteAlignedBits(3), 2).ToString());
            //

            //Hair
            MoveStreamToCFID(playerID);
            br.MoveStreamPosition(127, 1);
            var capHair =
                (CAPHairType)
                Enum.Parse(typeof (CAPHairType), Convert.ToByte(br.ReadNonByteAlignedBits(6), 2).ToString());
            //

            //
            MoveStreamToCFID(playerID);
            br.MoveStreamPosition(5, 4);
            var number = br.ReadNonByteAlignedByte();
            //

            //
            MoveStreamToCFID(playerID);
            br.MoveStreamPosition(126, 3);
            var muscleType =
                (MuscleTone)
                Enum.Parse(typeof(MuscleTone), Convert.ToByte(br.ReadNonByteAlignedBits(1), 2).ToString());

            var bodyType =
                (BodyType) Enum.Parse(typeof (BodyType), Convert.ToByte(br.ReadNonByteAlignedBits(2), 2).ToString());

            MoveStreamToCFID(playerID);
            br.MoveStreamPosition(128, 3);
            var eyes =
                (EyeColor)
                Enum.Parse(typeof(EyeColor), Convert.ToByte(br.ReadNonByteAlignedBits(3), 2).ToString());
            //



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

        private void MoveStreamToCFID(int playerID)
        {
            MoveStreamToFirstSS(playerID);
            br.MoveStreamPosition(-300, -2);
        }

        private void MoveStreamToFirstSS(int playerID)
        {
            br.BaseStream.Position = Convert.ToInt64(GetOption("FirstSSOffset"));
            br.InBytePosition = Convert.ToInt32(GetOption("FirstSSOffsetBit"));

            MoveStreamForSaveType();

            if (playerID >= 1365 && mode == Mode.X360)
            {
                br.BaseStream.Position += 16384;
            }

            int playerBits = 477 * 8 + 5;
            int totalBits = playerBits * playerID;
            br.MoveStreamPosition(totalBits / 8, totalBits % 8);
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
        }

        private void Window_Closing_1(object sender, CancelEventArgs e)
        {
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
                    PlayerEntry pe = playersList[i];
                    MoveStreamToFirstSS(pe.ID);
                    bw.BaseStream.Position = br.BaseStream.Position;
                    bw.InBytePosition = br.InBytePosition;

                    // Appearance
                    long prevPos = br.BaseStream.Position;
                    int prevPosIn = br.InBytePosition;

                    br.MoveStreamPosition(-300, -2);

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
                    bw.WriteNonByteAlignedBits(pe.GenericF ? "1" : "0", new byte[] { b });
                    SyncBRwithBW(bw);

                    br.MoveStreamPosition(137, 6);

                    Write2ByteStringToRoster(pe.ASAID.ToString(), bw);

                    // Shoes
                    MoveStreamToCFID(pe.ID);
                    br.MoveStreamPosition(122, 0);
                    SyncBWwithBR(ref bw);
                    byte[] original = br.ReadBytes(3);
                    bw.WriteNonByteAlignedBits(Convert.ToString(pe.ShoeModel, 2).PadLeft(12, '0'), original);
                    SyncBRwithBW(bw);

                    original = br.ReadBytes(2);
                    bw.WriteNonByteAlignedBits(
                        Convert.ToString((byte) Enum.Parse(typeof (ShoeBrand), pe.ShoeBrand.ToString()), 2).PadLeft(
                            3, '0'), original);
                    SyncBRwithBW(bw);
                    //


                    //Hair
                    MoveStreamToCFID(pe.ID);
                    br.MoveStreamPosition(127, 1);
                    SyncBWwithBR(ref bw);
                    bw.WriteNonByteAlignedBits(
                        Convert.ToString((byte) Enum.Parse(typeof (CAPHairType), pe.CAPHairType.ToString()), 2).PadLeft(
                            6, '0'), br.ReadBytes(2));
                    //

                    //
                    MoveStreamToCFID(pe.ID);
                    br.MoveStreamPosition(5, 4);
                    SyncBWwithBR(ref bw);
                    bw.WriteNonByteAlignedByte(pe.JerseyNumber, br.ReadBytes(2));
                    //

                    //
                    MoveStreamToCFID(pe.ID);
                    br.MoveStreamPosition(126, 3);
                    SyncBWwithBR(ref bw);
                    bw.WriteNonByteAlignedBits(
                        Convert.ToString((byte) Enum.Parse(typeof (MuscleTone), pe.MuscleTone.ToString()), 2).PadLeft(
                            1, '0'), br.ReadBytes(1));

                    SyncBRwithBW(bw);
                    bw.WriteNonByteAlignedBits(Convert.ToString((byte)Enum.Parse(typeof(BodyType), pe.BodyType.ToString()), 2).PadLeft(
                            2, '0'), br.ReadBytes(2));
                    

                    MoveStreamToCFID(pe.ID);
                    br.MoveStreamPosition(128, 3);
                    SyncBWwithBR(ref bw);
                    bw.WriteNonByteAlignedBits(Convert.ToString((byte)Enum.Parse(typeof(EyeColor), pe.EyeColor.ToString()), 2).PadLeft(
                            3, '0'), br.ReadBytes(2));
                    //



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
                    for (int j = 0; j<37;j++)
                    {
                        byte byteToWrite;
                        try
                        {
                            byteToWrite = (byte) ((pe.Ratings[j] - 25)*3);
                        }
                        catch(Exception ex)
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
                        catch (Exception ex)
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
                        catch (Exception ex)
                        {
                            MessageBox.Show("Problem with Player ID " + pe.ID + ", Hot Spot " + j + "\nWill try to continue.");
                            continue;
                        }
                        bw.WriteNonByteAlignedByte(byteToWrite, br.ReadBytes(2));
                        SyncBRwithBW(bw);
                    }
                }

                bw.Close();
            }

            if (doCRC)
            {
                RecalculateCRC();
            }

            updateStatus("Players saved.");
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

            using (BinaryWriter bw2 = new BinaryWriter(new FileStream(currentFile, FileMode.Open)))
            {
                bw2.Write(Tools.StringToByteArray(crc));
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
            MoveStreamBeforeFirstRoster();

            for (int i = 0; i < teamsList.Count - 1; i++)
            {
                br.MoveStreamPosition(720, 2);
                long curOffset = br.BaseStream.Position;
                int curOffsetBit = br.InBytePosition;

                WriteRosterRow(i, 18);

                //
                br.BaseStream.Position = 863081;
                br.InBytePosition = 6;
                br.MoveStreamPosition(720 * i, 2 * i);

                if (mode == Mode.X360)
                    br.BaseStream.Position += 69632;

                MoveStreamForSaveType();

                NonByteAlignedBinaryWriter bw = new NonByteAlignedBinaryWriter(new FileStream(currentFile, FileMode.Open));
                SyncBWwithBR(ref bw);

                int ac = teamsList[i].StAsstCoach;
                bw.WriteNonByteAlignedBits(Convert.ToString(ac, 2).PadLeft(16, '0'), br.ReadBytes(3));

                bw.Close();
                //

                br.BaseStream.Position = curOffset;
                br.InBytePosition = curOffsetBit;
            }

            // Free Agents
            int faRow = 999;
            br.BaseStream.Position = 853505;
            br.InBytePosition = 6;

            if (mode == Mode.X360)
                br.BaseStream.Position = 923137;

            MoveStreamForSaveType();

            WriteRosterRow(faRow, 256, true);
            //

            if (doCRC)
            {
                RecalculateCRC();
            }

            updateStatus("Teams saved.");
        }

        private void WriteRosterRow(int teamID, int spotsToWrite, bool isFArow = false)
        {
            NonByteAlignedBinaryWriter bw = new NonByteAlignedBinaryWriter(new FileStream(currentFile, FileMode.Open));

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

            SyncBWwithBR(ref bw);
            byte[] original = br.ReadBytes(2);
            SyncBRwithBW(bw);
            bw.WriteNonByteAlignedByte(Convert.ToByte(plNum.ToString()), original);

            bw.Close();
        }

        private object GetCellValue(DataGrid dataGrid, int row, int col)
        {
            var dataRowView = dataGrid.Items[row] as DataRowView;
            if (dataRowView != null)
                return dataRowView.Row.ItemArray[col];

            return null;
        }

        private object myCell2(int row, int col)
        {
            return GetCellValue(dgTeams, row, col);
        }

        private object myCell(int row, int col)
        {
            return dt.Rows[row][col];
        }

        private void WriteRosterSpot(string text, NonByteAlignedBinaryWriter bw, int spot, int plNum)
        {
            byte[] first2 = new byte[2];
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
            for (int i = 1; i<teamsList[0].PlNum; i++)
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
            PopulatePlayersTab();
            PopulateTeamsTab();
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

        private void dgTeams_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.V && Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                string[] lines = Tools.SplitLinesToArray(Clipboard.GetText());
                if (lines[0].Contains("ID") == false)
                {
                    MessageBox.Show("The pasted date must have the column headers in the first row.");
                    return;
                }
                List<Dictionary<string, string>> dictList = CSV.DictionaryListFromTSV(lines);

                for (int index = 0; index < dictList.Count; index++)
                {
                    var dict = dictList[index];
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
            for (int i = 0; i<rCount;i++)
            {
                te.RosterSpots[i] = te.RosterSpots[i].TrySetValue(dict, "R" + (i + 1).ToString(), true);
            }
            te.StAsstCoach = te.StAsstCoach.TrySetValue(dict, "StAsstCoach", true);
        }

        private void FixPlNumAndOrder()
        {
            for (int i = 0; i< teamsList.Count; i++)
            {
                int spotsCount = teamsList[i].RosterSpots.Count;
                for (int j = 0; j < spotsCount; j++)
                {
                    int curCell = teamsList[i].RosterSpots[j];
                    if (curCell == -1)// || String.IsNullOrWhiteSpace(curCell))
                    {
                        for (int k = spotsCount-1; k > j; k--)
                        {
                            int subCell = teamsList[i].RosterSpots[k];
                            if (subCell != -1)// || String.IsNullOrWhiteSpace(subCell)))
                            {
                                teamsList[i].RosterSpots[j] = subCell;
                                teamsList[i].RosterSpots[k] = -1;
                            }
                        }
                    }
                }
                int count = 0;
                for (int j = 0; j < spotsCount; j++)
                {
                    int curCell = teamsList[i].RosterSpots[j];
                    if (curCell != -1)// || String.IsNullOrWhiteSpace(curCell))
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
            foreach (var o in optionsList)
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
                    MessageBox.Show("The pasted date must have the column headers in the first row.");
                    return;
                }
                List<Dictionary<string, string>> dictList = CSV.DictionaryListFromTSV(lines);

                for (int index = 0; index < dictList.Count; index++)
                {
                    var dict = dictList[index];
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
            pe.CFID = pe.CFID.TrySetValue(dict, "CF ID", true);
            pe.PlType = pe.PlType.TrySetValue(dict, "PlType", true);
            pe.GenericF = pe.GenericF.TrySetValue(dict, "GenericF", true);
            pe.PortraitID = pe.PortraitID.TrySetValue(dict, "Portrait ID", true);
            pe.ASAID = pe.ASAID.TrySetValue(dict, "Audio ID", true);
            pe.ShoeBrand = pe.ShoeBrand.TrySetValue(dict, "Shoe Brand", true);
            pe.ShoeModel = pe.ShoeModel.TrySetValue(dict, "Shoe Model", true);
            pe.JerseyNumber = pe.JerseyNumber.TrySetValue(dict, "Number", true);
            pe.EyeColor = pe.EyeColor.TrySetValue(dict, "Eye Color", true);
            pe.CAPHairType = pe.CAPHairType.TrySetValue(dict, "CAP Hair Type", true);
            pe.BodyType = pe.BodyType.TrySetValue(dict, "Body Type", true);
            pe.MuscleTone = pe.MuscleTone.TrySetValue(dict, "Muscle Tone", true);
            pe.SSList[0] = pe.SSList[0].TrySetValue(dict, "SS1", true);
            pe.SSList[1] = pe.SSList[1].TrySetValue(dict, "SS2", true);
            pe.SSList[2] = pe.SSList[2].TrySetValue(dict, "SS3", true);
            pe.SSList[3] = pe.SSList[3].TrySetValue(dict, "SS4", true);
            pe.SSList[4] = pe.SSList[4].TrySetValue(dict, "SS5", true);

            var rtNames = Enum.GetNames(typeof (Rating));
            foreach (string rtName in rtNames)
            {
                int curInd = (int) Enum.Parse(typeof (Rating), rtName);
                pe.Ratings[curInd] = pe.Ratings[curInd].TrySetValue(dict, "R" + rtName, true);
            }

            var tNames = Enum.GetNames(typeof(Tendency));
            foreach (string tName in tNames)
            {
                int curInd = (int)Enum.Parse(typeof(Tendency), tName);
                pe.Tendencies[curInd] = pe.Tendencies[curInd].TrySetValue(dict, "T" + tName, true);
            }

            var hsNames = Enum.GetNames(typeof(HotSpot));
            foreach (string hsName in hsNames)
            {
                int curInd = (int)Enum.Parse(typeof(HotSpot), hsName);
                pe.HotSpots[curInd] = pe.HotSpots[curInd].TrySetValue(dict, "HS" + hsName, true);
            }
        }
    }

    internal enum Mode
    {
        PC, X360
    }

    internal enum SaveType
    {
        Roster, Association,
        MyCareer
    }
}