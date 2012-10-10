using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
        private int firstSSOffset;
        private int firstSSOffsetBit;
        private string currentFile;
        private bool doCRC;
        private Mode mode;
        private Dictionary<int, string> names = new Dictionary<int, string>(); 

        public static string DocsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\NBA 2K13 Roster Editor";
        private DataTable dt;

        private DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();

            if (Directory.Exists(DocsPath) == false)
            {
                Directory.CreateDirectory(DocsPath);
            }

            firstSSOffset = GetRegistrySetting("FirstSSOffset", 40916);
            firstSSOffsetBit = GetRegistrySetting("FirstSSOffsetBIt", 2);
            txtFirstSSOffset.Text = firstSSOffset.ToString();
            txtFirstSSOffsetBit.Text = firstSSOffsetBit.ToString();

            doCRC = (GetRegistrySetting("CRC", 1) == 1);
            chkRecalculateCRC.IsChecked = doCRC;

            mode = (Mode) Enum.Parse(typeof (Mode), GetRegistrySetting("Mode", "PC"));
            if (mode == Mode.PC)
                btnModePC.IsChecked = true;
            else
                btnMode360.IsChecked = true;

            string file = DocsPath + @"\names.txt";
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

            timer = new DispatcherTimer();
            timer.Tick += timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 3);
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
            
            dt = new DataTable();
            dt.Columns.Add("Team");
            dt.Columns.Add("PlNum");
            for (int i = 0; i < 256;i++)
            {
                dt.Columns.Add("R" + (i + 1).ToString());
            }
            
            long firstRosterOffset = 862911 - 720;
            int firstRosterOffsetBit = 6 - 2;
            br.BaseStream.Position = firstRosterOffset;
            br.InBytePosition = firstRosterOffsetBit;

            if (mode == Mode.X360)
                br.BaseStream.Position += 69632;


            DataRow dr;
            for (int i = 0; i<30;i++)
            {
                dr = dt.NewRow();
                dr["Team"] = teams[i];

                br.MoveStreamPosition(720, 2);
                long curOffset = br.BaseStream.Position;
                int curOffsetBit = br.InBytePosition;

                PopulateRosterRow(18, dr);
                br.BaseStream.Position = curOffset;
                br.InBytePosition = curOffsetBit;
            }

            // Free Agents
            dr = dt.NewRow();
            dr["Team"] = "Free Agents";

            br.BaseStream.Position = 853505;
            br.InBytePosition = 6;

            if (mode == Mode.X360)
                br.BaseStream.Position += 69632;

            PopulateRosterRow(256, dr, true);
            //

            RelinkDataGrid();

            br.BaseStream.Position = firstSSOffset;
            br.InBytePosition = firstSSOffsetBit;

            txtPlayerID.Text = "0";

            btnRefresh_Click(null, null);
        }

        private void PopulateRosterRow(int countToRead, DataRow dr, bool isFArow = false)
        {
            var startOfRoster = br.BaseStream.Position;
            var startOfRosterBit = br.InBytePosition;

            for (int i = 0; i < countToRead; i++)
            {
                dr["R" + (i + 1).ToString()] = ReadRosterSpot();
            }

            br.BaseStream.Position = startOfRoster;
            br.InBytePosition = startOfRosterBit;

            if (!isFArow) 
                br.MoveStreamPosition(125, 0);
            else
                br.MoveStreamPosition(4001, 0);

            dr["PlNum"] = br.ReadNonByteAlignedByte().ToString();
            dt.Rows.Add(dr);
        }

        private void RelinkDataGrid()
        {
            DataView dv = dt.DefaultView;
            dv.AllowEdit = true;
            dv.AllowNew = false;
            dv.AllowDelete = false;

            dgRosters.DataContext = dv;
        }

        private string ReadRosterSpot()
        {
            byte[] b = br.ReadNonByteAlignedBytes(2);
            if (b[0] == 0 && b[1] == 0)
                return "-1";

            b = br.ReadNonByteAlignedBytes(2);
            return Convert.ToInt16(NonByteAlignedBinaryReader.ByteArrayToBitString(b), 2).ToString();
        }

        private void ReadSignatureSkills()
        {
            byte b = NonByteAlignedBinaryReader.BitStringToByte(br.ReadNonByteAlignedBits(6));
            cmbSS1.SelectedItem = Enum.IsDefined(typeof (SignatureSkills), b) ? ((SignatureSkills) b) : (SignatureSkills) 0;
            b = NonByteAlignedBinaryReader.BitStringToByte(br.ReadNonByteAlignedBits(6));
            cmbSS2.SelectedItem = Enum.IsDefined(typeof (SignatureSkills), b) ? ((SignatureSkills) b) : (SignatureSkills) 0;
            br.ReadNonByteAlignedBits(14);
            b = NonByteAlignedBinaryReader.BitStringToByte(br.ReadNonByteAlignedBits(6));
            cmbSS3.SelectedItem = Enum.IsDefined(typeof (SignatureSkills), b) ? ((SignatureSkills) b) : (SignatureSkills) 0;
            b = NonByteAlignedBinaryReader.BitStringToByte(br.ReadNonByteAlignedBits(6));
            cmbSS4.SelectedItem = Enum.IsDefined(typeof (SignatureSkills), b) ? ((SignatureSkills) b) : (SignatureSkills) 0;
            b = NonByteAlignedBinaryReader.BitStringToByte(br.ReadNonByteAlignedBits(6));
            cmbSS5.SelectedItem = Enum.IsDefined(typeof (SignatureSkills), b) ? ((SignatureSkills) b) : (SignatureSkills) 0;
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                br.Close();
            }
            catch
            {

            }
            br = new NonByteAlignedBinaryReader(File.Open(currentFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));

            MoveStreamToSpecifiedPlayerSignatureSkills();

            lblOffset.Content = "SS1 Offset: " + br.BaseStream.Position + " + " + br.InBytePosition + " bits";

            Dictionary<string, string> appearance = ReadInfo();

            txtCFID.Text = appearance["CFID"];
            txtPortraitID.Text = appearance["PortraitID"];
            chkGenericF.IsChecked = appearance["GenericF"] == "1";
            txtPlType.Text = appearance["PlType"];

            ReadSignatureSkills();

            FindPlayerName();
        }

        private void FindPlayerName()
        {
            int id = Convert.ToInt32(txtPlayerID.Text);
            if (names.ContainsKey(id))
            {
                lblPlayerName.Content = names[id];
            }
            else
            {
                lblPlayerName.Content = "";
            }
        }

        private Dictionary<string, string> ReadInfo()
        {
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

        private void MoveStreamToSpecifiedPlayerSignatureSkills()
        {
            int id = Convert.ToInt32(txtPlayerID.Text);
            MoveStreamToFirstSS(id);
        }

        private void MoveStreamToFirstSS(int playerID)
        {
            br.BaseStream.Position = firstSSOffset;
            br.InBytePosition = firstSSOffsetBit;

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

        private void btnSaveOffsets_Click(object sender, RoutedEventArgs e)
        {
            SetRegistrySetting("FirstSSOffset", txtFirstSSOffset.Text);
            SetRegistrySetting("FirstSSOffsetBit", txtFirstSSOffsetBit.Text);
            firstSSOffset = GetRegistrySetting("FirstSSOffset", 40916);
            firstSSOffsetBit = GetRegistrySetting("FirstSSOffsetBIt", 2);
            btnRefresh_Click(null, null);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            NonByteAlignedBinaryWriter bw =
                new NonByteAlignedBinaryWriter(File.Open(currentFile, FileMode.Open, FileAccess.Write, FileShare.ReadWrite));
            MoveStreamToSpecifiedPlayerSignatureSkills();
            bw.BaseStream.Position = br.BaseStream.Position;
            bw.InBytePosition = br.InBytePosition;

            // Appearance
            long prevPos = br.BaseStream.Position;
            int prevPosIn = br.InBytePosition;

            br.MoveStreamPosition(-300, -2);

            Write2ByteStringToRoster(txtPortraitID.Text, bw);

            
            // PlType
            br.MoveStreamPosition(2, 6);
            SyncBWwithBR(ref bw);

            bw.WriteNonByteAlignedBits(Convert.ToString(Convert.ToByte(txtPlType.Text), 2).PadLeft(3, '0'), br.ReadBytes(2));

            SyncBRwithBW(ref bw);
            br.MoveStreamPosition(-3, -1);
            //


            br.MoveStreamPosition(28, 0);

            Write2ByteStringToRoster(txtCFID.Text, bw);

            br.MoveStreamPosition(94, 1);

            SyncBWwithBR(ref bw);
            byte b = br.ReadByte();
            bw.WriteNonByteAlignedBits(chkGenericF.IsChecked.GetValueOrDefault() ? "1" : "0", new byte[]{b});

            br.BaseStream.Position = prevPos;
            br.InBytePosition = prevPosIn;
            SyncBWwithBR(ref bw);
            //
            
            byte[] original = br.ReadBytes(2);
            string s = cmbSS1.SelectedItem.ToString();
            bw.WriteNonByteAlignedBits(ConvertSignatureSkillToBinary(s), original);
            SyncBRwithBW(ref bw);

            original = br.ReadBytes(2);
            s = cmbSS2.SelectedItem.ToString();
            bw.WriteNonByteAlignedBits(ConvertSignatureSkillToBinary(s), original);
            SyncBRwithBW(ref bw);

            br.MoveStreamPosition(0, 14);
            bw.MoveStreamPosition(0, 14);

            original = br.ReadBytes(2);
            s = cmbSS3.SelectedItem.ToString();
            bw.WriteNonByteAlignedBits(ConvertSignatureSkillToBinary(s), original);
            SyncBRwithBW(ref bw);

            original = br.ReadBytes(2);
            s = cmbSS4.SelectedItem.ToString();
            bw.WriteNonByteAlignedBits(ConvertSignatureSkillToBinary(s), original);
            SyncBRwithBW(ref bw);

            original = br.ReadBytes(2);
            s = cmbSS5.SelectedItem.ToString();
            bw.WriteNonByteAlignedBits(ConvertSignatureSkillToBinary(s), original);
            SyncBRwithBW(ref bw);
            
            bw.Close();

            if (doCRC)
            {
                RecalculateCRC();
            }

            updateStatus("Player saved.");
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
            return Convert.ToString((byte) Enum.Parse(typeof (SignatureSkills), signatureSkill), 2).PadLeft(6, '0');
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

        private void btnPrevPlayer_Click(object sender, RoutedEventArgs e)
        {
            int id = Convert.ToInt32(txtPlayerID.Text);
            if (id > 0)
            {
                txtPlayerID.Text = (id-1).ToString();
                btnRefresh_Click(null, null);
            }
        }

        private void btnNextPlayer_Click(object sender, RoutedEventArgs e)
        {
            txtPlayerID.Text = (Convert.ToInt32(txtPlayerID.Text) + 1).ToString();
            btnRefresh_Click(null, null);
        }
        
        private void btnSaveTest_Click(object sender, RoutedEventArgs e)
        {
            long firstRosterOffset = 862911 - 720;
            int firstRosterOffsetBit = 6 - 2;
            br.BaseStream.Position = firstRosterOffset;
            br.InBytePosition = firstRosterOffsetBit;

            if (mode == Mode.X360)
                br.BaseStream.Position += 69632;

            for (int i = 0; i < 30; i++)
            {
                br.MoveStreamPosition(720, 2);
                long curOffset = br.BaseStream.Position;
                int curOffsetBit = br.InBytePosition;

                WriteRosterRow(i, 18);
                br.BaseStream.Position = curOffset;
                br.InBytePosition = curOffsetBit;
            }

            // Free Agents
            int faRow = 30;
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

            updateStatus("Rosters saved.");
        }

        private void WriteRosterRow(int row, int spotsToWrite, bool isFArow = false)
        {
            NonByteAlignedBinaryWriter bw = new NonByteAlignedBinaryWriter(new FileStream(currentFile, FileMode.Open));

            long startOfRoster = br.BaseStream.Position;
            int startOfRosterBit = br.InBytePosition;
            SyncBWwithBR(ref bw);

            int plNum = Convert.ToInt32(myCell(row, 1));
            for (int i = 0; i < spotsToWrite; i++)
            {
                WriteRosterSpot(myCell(row, i + 2).ToString(), bw, i, plNum);
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
            return GetCellValue(dgRosters, row, col);
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
            RelinkDataGrid();
        }

        private void btnModePC_Checked(object sender, RoutedEventArgs e)
        {
            SetRegistrySetting("Mode", "PC");
            mode = Mode.PC;
            chkRecalculateCRC.IsChecked = true;
            
            SetRegistrySetting("FirstSSOffset", 40916);
            SetRegistrySetting("FirstSSOffsetBit", 2);
            firstSSOffset = GetRegistrySetting("FirstSSOffset", 40916);
            firstSSOffsetBit = GetRegistrySetting("FirstSSOffsetBIt", 2);
            txtFirstSSOffset.Text = firstSSOffset.ToString();
            txtFirstSSOffsetBit.Text = firstSSOffsetBit.ToString();
            try
            {
                btnRefresh_Click(null, null);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void btnMode360_Checked(object sender, RoutedEventArgs e)
        {
            SetRegistrySetting("Mode", "X360");
            mode = Mode.X360;
            chkRecalculateCRC.IsChecked = false;

            SetRegistrySetting("FirstSSOffset", 94164);
            SetRegistrySetting("FirstSSOffsetBit", 2);
            firstSSOffset = GetRegistrySetting("FirstSSOffset", 40916);
            firstSSOffsetBit = GetRegistrySetting("FirstSSOffsetBIt", 2);
            txtFirstSSOffset.Text = firstSSOffset.ToString();
            txtFirstSSOffsetBit.Text = firstSSOffsetBit.ToString();
            try
            {
                btnRefresh_Click(null, null);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void btnSearchByCF_Click(object sender, RoutedEventArgs e)
        {
            string idToFind = txtPlayerID.Text;

            ushort id = 0;
            MoveStreamToFirstSS(id);
            var dict = ReadInfo();

            bool found = false;
            while (true)
            {
                if (idToFind == dict["CFID"])
                {
                    found = true;
                    break;
                }
                MoveStreamToFirstSS(++id);
                if (br.BaseStream.Length - br.BaseStream.Position >= 477 && br.InBytePosition <= 3)
                {
                    dict = ReadInfo();
                }
                else
                {
                    break;
                }
            }

            if (found)
            {
                txtPlayerID.Text = id.ToString();
                btnRefresh_Click(null, null);
            }
            else
            {
                MessageBox.Show("No player found with that CF ID.");
                txtPlayerID.Text = "";
            }
        }

        private void dgRosters_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.V && Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                string[] lines = Tools.SplitLinesToArray(Clipboard.GetText());
                lines = lines.TakeWhile(s => !String.IsNullOrWhiteSpace(s)).ToArray();

                int row = dgRosters.Items.IndexOf(dgRosters.CurrentCell.Item);
                int col = dgRosters.CurrentCell.Column.DisplayIndex;

                if (row + lines.Length > dgRosters.Items.Count)
                {
                    MessageBox.Show(
                        "You're trying to paste more rows than currently available. Make sure you're not selecting the shader/range names when copying data.");
                    return;
                }

                DataTable dt = ((DataView)dgRosters.DataContext).Table;

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

                RelinkDataGrid();
            }
        }

        private void btnFix_Click(object sender, RoutedEventArgs e)
        {
            
            for (int i = 0; i< dt.Rows.Count; i++)
            {
                for (int j = 2; j < 258; j++)
                {
                    string curCell = dt.Rows[i][j].ToString();
                    if (curCell == "-1" || String.IsNullOrWhiteSpace(curCell))
                    {
                        for (int k = 257; k > j; k--)
                        {
                            string subCell = myCell(i, k).ToString();
                            if (!(subCell == "-1" || String.IsNullOrWhiteSpace(subCell)))
                            {
                                dt.Rows[i][j] = subCell;
                                dt.Rows[i][k] = "-1";
                            }
                        }
                    }
                }
                int count = 0;
                for (int j = 2; j < 258; j++)
                {
                    string curCell = dt.Rows[i][j].ToString();
                    if (curCell != "-1" && !String.IsNullOrWhiteSpace(curCell))
                    {
                        count++;
                    }
                    else
                    {
                        break;
                    }
                }
                dt.Rows[i][1] = count.ToString();
            }

            updateStatus("Player order and PlNum entries fixed.");
        }
    }

    internal enum Mode
    {
        PC, X360
    }

    internal enum SignatureSkills : byte
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
}