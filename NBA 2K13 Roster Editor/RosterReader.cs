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
using System.IO;
using System.Text;
using System.Windows;
using NonByteAlignedBinaryRW;

#endregion

namespace NBA_2K13_Roster_Editor
{
    internal class RosterReader : NonByteAlignedBinaryReader
    {
        public RosterReader(Stream stream) : base(stream)
        {
        }

        public RosterReader(Stream stream, Encoding encoding) : base(stream, encoding)
        {
        }

        public T ReadAndConvertToEnum<T>(int bits)
        {
            return (T) Enum.Parse(typeof (T), Convert.ToByte(ReadNonByteAlignedBits(bits), 2).ToString());
        }

        public void MoveStreamForSaveType()
        {
            if (MainWindow.saveType == SaveType.Association)
            {
                MoveStreamPosition(8, 0);
            }
            else if (MainWindow.saveType == SaveType.MyCareer)
            {
                MoveStreamPosition(2190248, 0);
            }
        }

        public static long SaveTypeOffset(SaveType saveType)
        {
            if (saveType == SaveType.Roster)
            {
                return 0;
            }
            else if (saveType == SaveType.Association)
            {
                return 8;
            }
            else if (saveType == SaveType.MyCareer)
            {
                return 2190248;
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        public void MoveStreamToCurrentTeamRoster(int i)
        {
            MoveStreamToFirstRoster();
            //MoveStreamForSaveType();
            MoveStreamPosition(720*i, 2*i);
        }

        public void MoveStreamToFirstJersey()
        {
            BaseStream.Position = 1486997;
            InBytePosition = 0;

            if (MainWindow.mode == Mode.X360)
            {
                MoveStreamPosition(77824, 0);
            }
            else if (MainWindow.mode == Mode.PCNov10 || MainWindow.mode == Mode.X360Nov10)
            {
                //MoveStreamPosition(1911, -4);
                BaseStream.Position = 1489588; // 4080
                InBytePosition = 4;
            }
            else if (MainWindow.mode == Mode.Custom || MainWindow.mode == Mode.CustomX360)
            {
                BaseStream.Position = Convert.ToInt64(MainWindow.GetOption("CustomJerseyOffset"));
                InBytePosition = Convert.ToInt32(MainWindow.GetOption("CustomJerseyOffsetBit"));
            }

            MoveStreamForSaveType();
        }

        public void MoveStreamToFirstRoster()
        {
            const long firstRosterOffset = 862911;
            const int firstRosterOffsetBit = 6;
            BaseStream.Position = firstRosterOffset;
            InBytePosition = firstRosterOffsetBit;

            if (MainWindow.mode == Mode.X360)
            {
                BaseStream.Position += 69632;
            }
            else if (MainWindow.mode == Mode.PCNov10 || MainWindow.mode == Mode.X360Nov10)
            {
                BaseStream.Position += 1911; //2119;
                InBytePosition = 2;
            }
            else if (MainWindow.mode == Mode.Custom || MainWindow.mode == Mode.CustomX360)
            {
                BaseStream.Position = Convert.ToInt64(MainWindow.GetOption("CustomRosterOffset"));
                InBytePosition = Convert.ToInt32(MainWindow.GetOption("CustomRosterOffsetBit"));
            }

            MoveStreamForSaveType();
        }

        public void MoveStreamToPlaybook(int i)
        {
            const long firstPlaybookOffset = 1099333;
            const int firstPlaybookOffsetBit = 3;

            BaseStream.Position = firstPlaybookOffset;
            InBytePosition = firstPlaybookOffsetBit;

            if (MainWindow.mode == Mode.Custom || MainWindow.mode == Mode.CustomX360)
            {
                BaseStream.Position = Convert.ToInt64(MainWindow.GetOption("CustomPlaybookOffset"));
                InBytePosition = Convert.ToInt32(MainWindow.GetOption("CustomPlaybookOffsetBit"));
            }
            else if (MainWindow.mode == Mode.PCNov10)
            {
                BaseStream.Position = 1101243;
                InBytePosition = 7;
            }

            MoveStreamForSaveType();

            MoveStreamPosition(215*i, i);
        }

        public void MoveStreamToPortraitID(int playerID)
        {
            MoveStreamToFirstSS(playerID);
            try
            {
                MoveStreamPosition(-300, -2);
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("Invalid roster offset.");
                return;
            }
        }

        public void MoveStreamToFirstSS(int playerID)
        {
            if (MainWindow.mode != Mode.Custom && MainWindow.mode != Mode.CustomX360)
            {
                BaseStream.Position = Convert.ToInt64(MainWindow.GetOption("FirstSSOffset"));
                InBytePosition = Convert.ToInt32(MainWindow.GetOption("FirstSSOffsetBit"));
            }
            else
            {
                BaseStream.Position = Convert.ToInt64(MainWindow.GetOption("CustomSSOffset"));
                InBytePosition = Convert.ToInt32(MainWindow.GetOption("CustomSSOffsetBit"));
            }

            MoveStreamForSaveType();

            if (playerID >= 1365 && (MainWindow.mode == Mode.X360 || MainWindow.mode == Mode.CustomX360))
            {
                BaseStream.Position += 16384;
            }

            const int playerBits = 477*8 + 5;
            int totalBits = playerBits*playerID;
            MoveStreamPosition(totalBits/8, totalBits%8);
        }

        public void MoveStreamToPlayerStats(int i)
        {
            long firstPlayerStatsOffset = 1475604;
            int firstPlayerStatsOffsetBit = 0;

            BaseStream.Position = firstPlayerStatsOffset;
            InBytePosition = firstPlayerStatsOffsetBit;

            if (MainWindow.mode == Mode.Custom || MainWindow.mode == Mode.CustomX360)
            {
                BaseStream.Position = Convert.ToInt64(MainWindow.GetOption("CustomPlayerStatsOffset"));
                InBytePosition = Convert.ToInt32(MainWindow.GetOption("CustomPlayerStatsOffsetBit"));
            }

            MoveStreamForSaveType();

            MoveStreamPosition(42*i, 0*i);
        }

        public void MoveStreamRelativeToPlayerStatsEntry(int i, int bytes, int bits)
        {
            MoveStreamToPlayerStats(i);
            if ((InBytePosition + bits >= 8))
            {
                BaseStream.Position += (InBytePosition + bits)/8;
            }
            if (InBytePosition + bits >= 0)
            {
                InBytePosition = (InBytePosition + bits)%8;
            }
            else
            {
                BaseStream.Position += ((InBytePosition + bits)/8) - 1;
                InBytePosition = ((InBytePosition + bits)%8) + 8;
            }
            BaseStream.Position += bytes;
        }

        public T ReadUInt16AndRaise<T>(int bits, int power)
        {
            return (T) Convert.ChangeType(ReadUInt16(bits)*Math.Pow(2, power), typeof (T));
        }

        public void MoveStreamToStaffPlaybookID(int i)
        {
            long firstStaffPlaybookIDOffset = 991131;
            int firstStaffPlaybookIDOffsetBit = 6;

            BaseStream.Position = firstStaffPlaybookIDOffset;
            InBytePosition = firstStaffPlaybookIDOffsetBit;

            if (MainWindow.mode == Mode.Custom || MainWindow.mode == Mode.CustomX360)
            {
                BaseStream.Position = Convert.ToInt64(MainWindow.GetOption("CustomStaffPlaybookIDOffset"));
                InBytePosition = Convert.ToInt32(MainWindow.GetOption("CustomStaffPlaybookIDOffsetBit"));
            }

            MoveStreamForSaveType();

            MoveStreamPosition(141*i, 4*i);
        }

        public void MoveStreamToTeamStats(int i)
        {
            long firstTeamStatsOffset = 1434425;
            int firstTeamStatsOffsetBit = 5;

            BaseStream.Position = firstTeamStatsOffset;
            InBytePosition = firstTeamStatsOffsetBit;

            if (MainWindow.mode == Mode.Custom || MainWindow.mode == Mode.CustomX360)
            {
                BaseStream.Position = Convert.ToInt64(MainWindow.GetOption("CustomTeamStatsOffset"));
                InBytePosition = Convert.ToInt32(MainWindow.GetOption("CustomTeamStatsOffsetBit"));
            }

            MoveStreamForSaveType();

            MoveStreamPosition(42*i, 0*i);
        }

        public ushort ReadBigEndianUInt16()
        {
            return Convert.ToUInt16(ReadNonByteAlignedBits(16), 2);
        }

        public UInt16 ReadUInt16(int bitsCount)
        {
            return Convert.ToUInt16(ReadNonByteAlignedBits(bitsCount), 2);
        }

        public byte ReadNBAByte(int bitsCount)
        {
            return Convert.ToByte(ReadNonByteAlignedBits(bitsCount), 2);
        }

        public Int16 ReadNBAInt16(int bitsCount)
        {
            return Convert.ToInt16(ReadNonByteAlignedBits(bitsCount), 2);
        }

        public UInt32 ReadNBAUInt32(int bitsCount)
        {
            return Convert.ToUInt32(ReadNonByteAlignedBits(bitsCount), 2);
        }

        public Int32 ReadNBAInt32(int bitsCount)
        {
            return Convert.ToInt32(ReadNonByteAlignedBits(bitsCount), 2);
        }

        public short ReadBigEndianInt16()
        {
            return Convert.ToInt16(ReadNonByteAlignedBits(16), 2);
        }
    }
}