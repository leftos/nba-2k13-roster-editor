using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
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

        public MainWindow()
        {
            InitializeComponent();

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
                    Header = string.Format("Rtg{0}", (i + 1)),
                    Binding =
                        new Binding { Path = new PropertyPath(string.Format("Ratings[{0}]", i)), Mode = BindingMode.TwoWay }
                });
            }

            for (int i = 0; i < 69; i++)
            {
                dgPlayers.Columns.Add(new DataGridTextColumn
                {
                    Header = string.Format("Tnd{0}", (i + 1)),
                    Binding =
                        new Binding { Path = new PropertyPath(string.Format("Tendencies[{0}]", i)), Mode = BindingMode.TwoWay }
                });
            }
            
            for (int i = 0; i < 25; i++)
            {
                dgPlayers.Columns.Add(new DataGridTextColumn
                {
                    Header = string.Format("HS{0}", (i + 1)),
                    Binding =
                        new Binding { Path = new PropertyPath(string.Format("HotSpots[{0}]", i)), Mode = BindingMode.TwoWay }
                });
            }
            
            playersList = new ObservableCollection<PlayerEntry>();
            dgPlayers.ItemsSource = playersList;

            dgTeams.Columns.Clear();
            dgTeams.Columns.Add(new DataGridTextColumn
                                  {Header = "ID", Binding = new Binding {Path = new PropertyPath("ID"), Mode = BindingMode.TwoWay}});
            dgTeams.Columns.Add(new DataGridTextColumn
                                  {Header = "Name", Binding = new Binding {Path = new PropertyPath("Name"), Mode = BindingMode.TwoWay}});
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
                foreach (string line in lines)
                {
                    string[] parts = line.Split(new char[] {'\t'}, 2);
                    if (parts.Length == 2)
                    {
                        int id = Convert.ToInt32(parts[0]);
                        if (names.ContainsKey(id) == false)
                        {
                            names.Add(id, parts[1]);
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
            ofd.Filter = "Roster files (*.ROS)|*.ROS|All files (*.*)|*.*";
            ofd.DefaultExt = ".ROS";
            ofd.ShowDialog();

            if (ofd.FileName == "")
                return;

            txtFile.Text = Path.GetFileName(ofd.FileName);
            currentFile = ofd.FileName;
            br = new NonByteAlignedBinaryReader(File.Open(currentFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));

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
                Dictionary<string, string> info = ReadInfo(i);
                pe.ID = i;
                pe.CFID = Convert.ToInt32(info["CFID"]);
                pe.PortraitID = Convert.ToInt32(info["PortraitID"]);
                pe.GenericF = info["GenericF"] == "1";
                pe.PlType = Convert.ToInt32(info["PlType"]);
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
            long firstRosterOffset = 862911 - 720;
            int firstRosterOffsetBit = 6 - 2;
            br.BaseStream.Position = firstRosterOffset;
            br.InBytePosition = firstRosterOffsetBit;

            if (mode == Mode.X360)
                br.BaseStream.Position += 69632;

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

            PopulateRosterRow(256, ref te, true);
            //

            RelinkTeamsDataGrid();
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

        private Dictionary<string, string> ReadInfo(int playerID)
        {
            MoveStreamToFirstSS(playerID);
            long prevPos = br.BaseStream.Position;
            int prevPosIn = br.InBytePosition;

            br.MoveStreamPosition(-300, -2);

            byte[] por = br.ReadNonByteAlignedBytes(2);


            // PlType
            br.MoveStreamPosition(2, 6);

            string plType = Convert.ToByte(br.ReadNonByteAlignedBits(3),2).ToString();

            br.MoveStreamPosition(-3, -1);
            //
            

            br.MoveStreamPosition(28, 0);

            byte[] cf = br.ReadNonByteAlignedBytes(2);

            br.MoveStreamPosition(94, 1);

            //string hello = NonByteAlignedBinaryReader.ByteArrayToBitString(br.ReadNonByteAlignedBytes(2));
            string genericF = br.ReadNonByteAlignedBits(1);
            
            br.BaseStream.Position = prevPos;
            br.InBytePosition = prevPosIn;

            var ret = new Dictionary<string, string>();
            ret["CFID"] = Convert.ToUInt16(NonByteAlignedBinaryReader.ByteArrayToBitString(cf), 2).ToString();
            ret["PortraitID"] = Convert.ToUInt16(NonByteAlignedBinaryReader.ByteArrayToBitString(por), 2).ToString();
            ret["GenericF"] = genericF;
            ret["PlType"] = plType;
            return ret;
        }
        
        private void MoveStreamToFirstSS(int playerID)
        {
            br.BaseStream.Position = Convert.ToInt64(GetOption("FirstSSOffset"));
            br.InBytePosition = Convert.ToInt32(GetOption("FirstSSOffsetBit"));

            if (playerID >= 1365 && mode == Mode.X360)
            {
                br.BaseStream.Position += 16384;
            }

            int playerBits = 477 * 8 + 5;
            int totalBits = playerBits * playerID;
            br.MoveStreamPosition(totalBits / 8, totalBits % 8);
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

                    SyncBRwithBW(ref bw);
                    br.MoveStreamPosition(-3, -1);
                    //


                    br.MoveStreamPosition(28, 0);

                    Write2ByteStringToRoster(pe.CFID.ToString(), bw);

                    br.MoveStreamPosition(94, 1);

                    SyncBWwithBR(ref bw);
                    byte b = br.ReadByte();
                    bw.WriteNonByteAlignedBits(pe.GenericF ? "1" : "0", new byte[] {b});

                    br.BaseStream.Position = prevPos;
                    br.InBytePosition = prevPosIn;
                    SyncBWwithBR(ref bw);
                    //

                    byte[] original = br.ReadBytes(2);
                    string s = pe.SSList[0].ToString();
                    bw.WriteNonByteAlignedBits(ConvertSignatureSkillToBinary(s), original);
                    SyncBRwithBW(ref bw);

                    original = br.ReadBytes(2);
                    s = pe.SSList[1].ToString();
                    bw.WriteNonByteAlignedBits(ConvertSignatureSkillToBinary(s), original);
                    SyncBRwithBW(ref bw);

                    br.MoveStreamPosition(0, 14);
                    bw.MoveStreamPosition(0, 14);

                    original = br.ReadBytes(2);
                    s = pe.SSList[2].ToString();
                    bw.WriteNonByteAlignedBits(ConvertSignatureSkillToBinary(s), original);
                    SyncBRwithBW(ref bw);

                    original = br.ReadBytes(2);
                    s = pe.SSList[3].ToString();
                    bw.WriteNonByteAlignedBits(ConvertSignatureSkillToBinary(s), original);
                    SyncBRwithBW(ref bw);

                    original = br.ReadBytes(2);
                    s = pe.SSList[4].ToString();
                    bw.WriteNonByteAlignedBits(ConvertSignatureSkillToBinary(s), original);
                    SyncBRwithBW(ref bw);


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
                        SyncBRwithBW(ref bw);
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
                        SyncBRwithBW(ref bw);
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
                        SyncBRwithBW(ref bw);
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

        private void SyncBRwithBW(ref NonByteAlignedBinaryWriter bw)
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

            long firstRosterOffset = 862911 - 720;
            int firstRosterOffsetBit = 6 - 2;
            br.BaseStream.Position = firstRosterOffset;
            br.InBytePosition = firstRosterOffsetBit;

            if (mode == Mode.X360)
                br.BaseStream.Position += 69632;

            for (int i = 0; i < teamsList.Count - 1; i++)
            {
                br.MoveStreamPosition(720, 2);
                long curOffset = br.BaseStream.Position;
                int curOffsetBit = br.InBytePosition;

                WriteRosterRow(i, 18);
                br.BaseStream.Position = curOffset;
                br.InBytePosition = curOffsetBit;
            }

            // Free Agents
            int faRow = 999;
            br.BaseStream.Position = 853505;
            br.InBytePosition = 6;

            if (mode == Mode.X360)
                br.BaseStream.Position = 923137;

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
            SyncBRwithBW(ref bw);
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
            int id = Convert.ToUInt16(myCell(0,2).ToString());
            for (int i = 1; i<Convert.ToInt32(myCell(0,1)); i++)
            {
                dt.Rows[0][i + 2] = ++id;
            }
            RelinkTeamsDataGrid();
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
                lines = lines.TakeWhile(s => !String.IsNullOrWhiteSpace(s)).ToArray();

                int row = dgTeams.Items.IndexOf(dgTeams.CurrentCell.Item);
                int col = dgTeams.CurrentCell.Column.DisplayIndex;

                if (row + lines.Length > dgTeams.Items.Count)
                {
                    MessageBox.Show(
                        "You're trying to paste more rows than currently available. Make sure you're not selecting the shader/range names when copying data.");
                    return;
                }

                DataTable dt = ((DataView)dgTeams.DataContext).Table;

                int length = lines[0].Split('\t').Length;
                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];
                    string[] parts = line.Split('\t');
                    if (parts.Length == length)
                    {
                        for (int j = 0; j < parts.Length; j++)
                        {
                            dt.Rows[row + i][col + j] = (!String.IsNullOrWhiteSpace(parts[j])) ? (object)parts[j] : DBNull.Value;
                        }
                    }
                }

                RelinkTeamsDataGrid();
            }
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

                foreach (var dict in dictList)
                {
                    int ID;
                    try
                    {
                        ID = Convert.ToInt32(dict["ID"]);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Couldn't detect a player's ID in the pasted data. " +
                                        "\nUse a copy of this table as a base by copying it and pasting it into a spreadsheet and making changes there.");
                        return;
                    }
                    for (int i = 0; i < playersList.Count; i++)
                    {
                        if (playersList[i].ID == ID)
                        {
                            PlayerEntry pe = playersList[i];
                            TryParseDictionaryList(ref pe, dict);
                            playersList[i] = pe;
                            break;
                        }
                    }
                }

                dgPlayers.ItemsSource = null;
                dgPlayers.ItemsSource = playersList;
            }
        }

        private void TryParseDictionaryList(ref PlayerEntry pe, Dictionary<string, string> dict)
        {
            pe.CFID = pe.CFID.TrySetValue(dict, "CF ID");
            pe.PlType = pe.PlType.TrySetValue(dict, "PlType");
            pe.GenericF = pe.GenericF.TrySetValue(dict, "GenericF");
            pe.PortraitID = pe.PortraitID.TrySetValue(dict, "PortraitID");
            pe.SSList[0] = pe.SSList[0].TrySetValue(dict, "SS1");
            pe.SSList[1] = pe.SSList[1].TrySetValue(dict, "SS2");
            pe.SSList[2] = pe.SSList[2].TrySetValue(dict, "SS3");
            pe.SSList[3] = pe.SSList[3].TrySetValue(dict, "SS4");
            pe.SSList[4] = pe.SSList[4].TrySetValue(dict, "SS5");
        }
    }

    internal enum Mode
    {
        PC, X360
    }

    internal enum SignatureSkill : byte
    {
        None = 0,
        Posterizer = 1,
        HighlightFilm = 2,
        Finisher = 3,
        Acrobat = 4,
        SpotUpShooter = 5,
        ShotCreator = 6,
        Deadeye = 7,
        CornerSpecialist = 8,
        PostProficiency = 9,
        AnkleBreaker = 10,
        PostPlaymaker = 11,
        Dimer = 12,
        BreakStarter = 13,
        AlleyOoper = 14,
        BrickWall = 15,
        LockdownDefender = 16,
        ChargeCard = 17,
        Interceptor = 18,
        PickPocket = 19,
        ActiveHands = 20,
        Eraser = 21,
        ChasedownArtist = 22,
        Bruiser = 23,
        HustlePoints = 24,
        Scraper = 25,
        AntiFreeze = 26,
        Microwave = 27,
        HeatRetention = 28,
        Closer = 29,
        FloorGeneral = 30,
        DefensiveAnchor = 31,
        GatoradePrimePack = 32,
        OnCourtCoach = 33
    }

    internal enum HotZone
    {
        Cold = 0,
        Neutral = 1,
        Hot = 2,
        Burned = 3
    }
}