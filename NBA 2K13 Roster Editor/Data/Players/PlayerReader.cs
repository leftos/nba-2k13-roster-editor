using System;
using System.Collections.ObjectModel;
using System.Linq;
using NBA_2K13_Roster_Editor.Data.Players.Parameters;
using NonByteAlignedBinaryRW;

namespace NBA_2K13_Roster_Editor.Data.Players
{
    static internal class PlayerReader
    {
        internal static ObservableCollection<byte> ReadGear(int i, RosterReader brOpen)
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

            // Shoe trim (help by TUSS11)
            brOpen.MoveStreamPosition(-175, -5);
            gear.Add(brOpen.ReadNBAByte(3)); // Home trim 1
            gear.Add(brOpen.ReadNBAByte(3)); // Home trim 2
            gear.Add(brOpen.ReadNBAByte(3)); // Away trim 1
            gear.Add(brOpen.ReadNBAByte(3)); // Away trim 2
            //
            return gear;
        }

        internal static ObservableCollection<byte> ReadRatings(int playerID, RosterReader brOpen)
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

        internal static ObservableCollection<byte> ReadTendencies(int playerID, RosterReader brOpen)
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

        internal static ObservableCollection<byte> ReadHotSpots(int playerID, RosterReader brOpen)
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

        internal static ObservableCollection<HotZoneValue> ReadHotZones(int playerID, RosterReader brOpen)
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

        internal static ObservableCollection<SignatureSkill> ReadSignatureSkills(int playerID, RosterReader brOpen)
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

        internal static void ReadPlayerInfo(ref PlayerEntry pe, RosterReader brOpen)
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
    }
}