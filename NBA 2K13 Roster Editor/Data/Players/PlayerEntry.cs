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

using System.Collections.ObjectModel;
using System.ComponentModel;
using NBA_2K13_Roster_Editor.Data.Players.Parameters;

#endregion

namespace NBA_2K13_Roster_Editor.Data.Players
{
    internal class PlayerEntry : INotifyPropertyChanged
    {
        // IMPORTANT NOTE
        // When adding a property to a Player Entry, you should take care of:
        // - Loading it via PopulatePlayersTab
        // - Saving it via btnSavePlayers_Click
        // - Making it pasteable via dgPlayers_PreviewKeyDown

        private ObservableCollection<byte> _accessories;
        private int _asaID;
        private int _assignedTo;
        private byte _birthDay;
        private byte _birthMonth;
        private ushort _birthYear;
        private BodyType _bodyType;
        private int _cAPBeard;
        private int _cAPEyebrow;
        private HairColor _cAPFclHairClr;
        private HairColor _cAPHairClr;
        private int _cAPMstch;
        private int _capGoatee;
        private CAPHairType _capHairType;
        private int _cfid;
        private ClothesType _clothesType;
        private bool _contNoTrade;
        private ContractOption _contractOpt;
        private uint _contractY1;
        private uint _contractY2;
        private uint _contractY3;
        private uint _contractY4;
        private uint _contractY5;
        private uint _contractY6;
        private uint _contractY7;
        private ObservableCollection<int> _dunkPackages;
        private EyeColor _eyeColor;
        private bool _genericF;
        private float _height;
        private ObservableCollection<byte> _hotSpots;
        private ObservableCollection<HotZoneValue> _hotZones;
        private int _id;
        private short _injuryDays;
        private ushort _injuryType;
        private bool _isInFAPool;
        private bool _isFA1;
        private bool _isHidden;
        private byte _layupPkg;
        private MuscleTone _muscleTone;
        private string _name;
        private byte _number;
        private int _plType;
        private int _playStyle;
        private PlayType _playType1;
        private PlayType _playType2;
        private PlayType _playType3;
        private PlayType _playType4;
        private short _playoffStats;
        private int _portraitID;
        private Position _position1;
        private Position _position2;
        private ObservableCollection<byte> _ratings;
        private ObservableCollection<short> _seasonStats;
        private string _shAwayBase;
        private string _shAwayTeam1;
        private string _shAwayTeam2;
        private bool _shCustomClr;
        private string _shHomeBase;
        private string _shHomeTeam1;
        private string _shHomeTeam2;
        private ShoeBrand _shoeBrand;
        private int _shoeModel;
        private int _sigFT;
        private int _sigShtBase;
        private int _sigShtForm;
        private int _skintone;
        private ObservableCollection<SignatureSkill> _ssList;
        private int _teamID1;
        private int _teamID2;
        private ObservableCollection<byte> _tendencies;
        private float _weight;
        private byte _yearsPro;

        public PlayerEntry()
        {
            SSList = new ObservableCollection<SignatureSkill>();
            Ratings = new ObservableCollection<byte>();
            Tendencies = new ObservableCollection<byte>();
            HotSpots = new ObservableCollection<byte>();
            HotZones = new ObservableCollection<HotZoneValue>();
            SeasonStats = new ObservableCollection<short>();
            Accessories = new ObservableCollection<byte>();
        }

        public long Offset
        {
            get { return _offset; }
            set
            {
                _offset = value;
                OnPropertyChanged("Offset");
            }
        }

        private long _offset;

        public byte IsFAPar3
        {
            get { return _isFAPar3; }
            set
            {
                _isFAPar3 = value;
                OnPropertyChanged("IsFAPar3");
            }
        }

        private byte _isFAPar3;

        public int IsFAPar1
        {
            get { return _isFAPar1; }
            set
            {
                _isFAPar1 = value;
                OnPropertyChanged("IsFAPar1");
            }
        }

        private int _isFAPar1;

        public int IsFAPar2
        {
            get { return _isFAPar2; }
            set
            {
                _isFAPar2 = value;
                OnPropertyChanged("IsFAPar2");
            }
        }

        private int _isFAPar2;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public int ID
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged("ID");
            }
        }

        public ObservableCollection<byte> Accessories
        {
            get { return _accessories; }
            set
            {
                _accessories = value;
                OnPropertyChanged("Accessories");
            }
        }

        public byte LayupPkg
        {
            get { return _layupPkg; }
            set
            {
                _layupPkg = value;
                OnPropertyChanged("LayupPkg");
            }
        }

        public int TeamID1
        {
            get { return _teamID1; }
            set
            {
                _teamID1 = value;
                OnPropertyChanged("TeamID1");
            }
        }

        public int TeamID2
        {
            get { return _teamID2; }
            set
            {
                _teamID2 = value;
                OnPropertyChanged("TeamID2");
            }
        }

        public int AssignedTo
        {
            get { return _assignedTo; }
            set
            {
                _assignedTo = value;
                OnPropertyChanged("AssignedTo");
            }
        }

        public bool IsInFAPool
        {
            get { return _isInFAPool; }
            set
            {
                _isInFAPool = value;
                OnPropertyChanged("IsInFAPool");
            }
        }

        public bool IsHidden
        {
            get { return _isHidden; }
            set
            {
                _isHidden = value;
                OnPropertyChanged("IsHidden");
            }
        }

        public int CFID
        {
            get { return _cfid; }
            set
            {
                _cfid = value;
                OnPropertyChanged("CFID");
            }
        }

        public int PlType
        {
            get { return _plType; }
            set
            {
                _plType = value;
                OnPropertyChanged("PlType");
            }
        }

        public int PlayStyle
        {
            get { return _playStyle; }
            set
            {
                _playStyle = value;
                OnPropertyChanged("PlayStyle");
            }
        }

        public PlayType PlayType1
        {
            get { return _playType1; }
            set
            {
                _playType1 = value;
                OnPropertyChanged("PlayType1");
            }
        }

        public PlayType PlayType2
        {
            get { return _playType2; }
            set
            {
                _playType2 = value;
                OnPropertyChanged("PlayType2");
            }
        }

        public PlayType PlayType3
        {
            get { return _playType3; }
            set
            {
                _playType3 = value;
                OnPropertyChanged("PlayType3");
            }
        }

        public PlayType PlayType4
        {
            get { return _playType4; }
            set
            {
                _playType4 = value;
                OnPropertyChanged("PlayType4");
            }
        }

        public Position Position1
        {
            get { return _position1; }
            set
            {
                _position1 = value;
                OnPropertyChanged("Position1");
            }
        }

        public Position Position2
        {
            get { return _position2; }
            set
            {
                _position2 = value;
                OnPropertyChanged("Position2");
            }
        }

        public ushort InjuryType
        {
            get { return _injuryType; }
            set
            {
                _injuryType = value;
                OnPropertyChanged("InjuryType");
            }
        }

        public short InjuryDays
        {
            get { return _injuryDays; }
            set
            {
                _injuryDays = value;
                OnPropertyChanged("InjuryDays");
            }
        }

        public ushort BirthYear
        {
            get { return _birthYear; }
            set
            {
                _birthYear = value;
                OnPropertyChanged("BirthYear");
            }
        }

        public byte BirthMonth
        {
            get { return _birthMonth; }
            set
            {
                _birthMonth = value;
                OnPropertyChanged("BirthMonth");
            }
        }

        public byte BirthDay
        {
            get { return _birthDay; }
            set
            {
                _birthDay = value;
                OnPropertyChanged("BirthDay");
            }
        }

        public byte YearsPro
        {
            get { return _yearsPro; }
            set
            {
                _yearsPro = value;
                OnPropertyChanged("YearsPro");
            }
        }

        public bool GenericF
        {
            get { return _genericF; }
            set
            {
                _genericF = value;
                OnPropertyChanged("GenericF");
            }
        }

        public int PortraitID
        {
            get { return _portraitID; }
            set
            {
                _portraitID = value;
                OnPropertyChanged("PortraitID");
            }
        }

        public int ASAID
        {
            get { return _asaID; }
            set
            {
                _asaID = value;
                OnPropertyChanged("ASAID");
            }
        }

        public ShoeBrand ShoeBrand
        {
            get { return _shoeBrand; }
            set
            {
                _shoeBrand = value;
                OnPropertyChanged("ShoeBrand");
            }
        }

        public int ShoeModel
        {
            get { return _shoeModel; }
            set
            {
                _shoeModel = value;
                OnPropertyChanged("ShoeModel");
            }
        }

        public string ShHomeTeam1
        {
            get { return _shHomeTeam1; }
            set
            {
                _shHomeTeam1 = value;
                OnPropertyChanged("ShHomeTeam1");
            }
        }

        public string ShHomeTeam2
        {
            get { return _shHomeTeam2; }
            set
            {
                _shHomeTeam2 = value;
                OnPropertyChanged("ShHomeTeam2");
            }
        }

        public string ShHomeBase
        {
            get { return _shHomeBase; }
            set
            {
                _shHomeBase = value;
                OnPropertyChanged("ShHomeBase");
            }
        }

        public string ShAwayTeam1
        {
            get { return _shAwayTeam1; }
            set
            {
                _shAwayTeam1 = value;
                OnPropertyChanged("ShAwayTeam1");
            }
        }

        public string ShAwayTeam2
        {
            get { return _shAwayTeam2; }
            set
            {
                _shAwayTeam2 = value;
                OnPropertyChanged("ShAwayTeam2");
            }
        }

        public string ShAwayBase
        {
            get { return _shAwayBase; }
            set
            {
                _shAwayBase = value;
                OnPropertyChanged("ShAwayBase");
            }
        }

        public ObservableCollection<SignatureSkill> SSList
        {
            get { return _ssList; }
            set
            {
                _ssList = value;
                OnPropertyChanged("SSList");
            }
        }

        public ObservableCollection<byte> Ratings
        {
            get { return _ratings; }
            set
            {
                _ratings = value;
                OnPropertyChanged("Ratings");
            }
        }

        public ObservableCollection<byte> Tendencies
        {
            get { return _tendencies; }
            set
            {
                _tendencies = value;
                OnPropertyChanged("Tendencies");
            }
        }

        public ObservableCollection<byte> HotSpots
        {
            get { return _hotSpots; }
            set
            {
                _hotSpots = value;
                OnPropertyChanged("HotSpots");
            }
        }

        public ObservableCollection<HotZoneValue> HotZones
        {
            get { return _hotZones; }
            set
            {
                _hotZones = value;
                OnPropertyChanged("HotZones");
            }
        }

        public ObservableCollection<short> SeasonStats
        {
            get { return _seasonStats; }
            set
            {
                _seasonStats = value;
                OnPropertyChanged("SeasonStats");
            }
        }

        public short PlayoffStats
        {
            get { return _playoffStats; }
            set
            {
                _playoffStats = value;
                OnPropertyChanged("PlayoffStats");
            }
        }

        public CAPHairType CAPHairType
        {
            get { return _capHairType; }
            set
            {
                _capHairType = value;
                OnPropertyChanged("CAPHairType");
            }
        }

        public byte Number
        {
            get { return _number; }
            set
            {
                _number = value;
                OnPropertyChanged("Number");
            }
        }

        public MuscleTone MuscleTone
        {
            get { return _muscleTone; }
            set
            {
                _muscleTone = value;
                OnPropertyChanged("MuscleTone");
            }
        }

        public BodyType BodyType
        {
            get { return _bodyType; }
            set
            {
                _bodyType = value;
                OnPropertyChanged("BodyType");
            }
        }

        public EyeColor EyeColor
        {
            get { return _eyeColor; }
            set
            {
                _eyeColor = value;
                OnPropertyChanged("EyeColor");
            }
        }

        public ClothesType ClothesType
        {
            get { return _clothesType; }
            set
            {
                _clothesType = value;
                OnPropertyChanged("ClothesType");
            }
        }

        public bool ShCustomClr
        {
            get { return _shCustomClr; }
            set
            {
                _shCustomClr = value;
                OnPropertyChanged("ShCustomColor");
            }
        }

        public uint ContractY1
        {
            get { return _contractY1; }
            set
            {
                _contractY1 = value;
                OnPropertyChanged("ContractY1");
            }
        }

        public uint ContractY2
        {
            get { return _contractY2; }
            set
            {
                _contractY2 = value;
                OnPropertyChanged("ContractY2");
            }
        }

        public uint ContractY3
        {
            get { return _contractY3; }
            set
            {
                _contractY3 = value;
                OnPropertyChanged("ContractY3");
            }
        }

        public uint ContractY4
        {
            get { return _contractY4; }
            set
            {
                _contractY4 = value;
                OnPropertyChanged("ContractY4");
            }
        }

        public uint ContractY5
        {
            get { return _contractY5; }
            set
            {
                _contractY5 = value;
                OnPropertyChanged("ContractY5");
            }
        }

        public uint ContractY6
        {
            get { return _contractY6; }
            set
            {
                _contractY6 = value;
                OnPropertyChanged("ContractY6");
            }
        }

        public uint ContractY7
        {
            get { return _contractY7; }
            set
            {
                _contractY7 = value;
                OnPropertyChanged("ContractY7");
            }
        }

        public ContractOption ContractOpt
        {
            get { return _contractOpt; }
            set
            {
                _contractOpt = value;
                OnPropertyChanged("ContractOpt");
            }
        }

        public bool ContNoTrade
        {
            get { return _contNoTrade; }
            set
            {
                _contNoTrade = value;
                OnPropertyChanged("ContNoTrade");
            }
        }

        public float Height
        {
            get { return _height; }
            set
            {
                _height = value;
                OnPropertyChanged("Height");
            }
        }

        public float Weight
        {
            get { return _weight; }
            set
            {
                _weight = value;
                OnPropertyChanged("Weight");
            }
        }

        public int CAPBeard
        {
            get { return _cAPBeard; }
            set
            {
                _cAPBeard = value;
                OnPropertyChanged("CAPBeard");
            }
        }

        public int Skintone
        {
            get { return _skintone; }
            set
            {
                _skintone = value;
                OnPropertyChanged("Skintone");
            }
        }

        public HairColor CAPHairClr
        {
            get { return _cAPHairClr; }
            set
            {
                _cAPHairClr = value;
                OnPropertyChanged("CAPHairClr");
            }
        }

        public int CAPEyebrow
        {
            get { return _cAPEyebrow; }
            set
            {
                _cAPEyebrow = value;
                OnPropertyChanged("CAPEyebrow");
            }
        }

        public int CAPMstch
        {
            get { return _cAPMstch; }
            set
            {
                _cAPMstch = value;
                OnPropertyChanged("CAPMstch");
            }
        }

        public HairColor CAPFclHairClr
        {
            get { return _cAPFclHairClr; }
            set
            {
                _cAPFclHairClr = value;
                OnPropertyChanged("CAPFclHairClr");
            }
        }

        public int CAPGoatee
        {
            get { return _capGoatee; }
            set
            {
                _capGoatee = value;
                OnPropertyChanged("Goatee");
            }
        }

        public ObservableCollection<int> DunkPackages
        {
            get { return _dunkPackages; }
            set
            {
                _dunkPackages = value;
                OnPropertyChanged("DunkPackages");
            }
        }

        public int SigShtForm
        {
            get { return _sigShtForm; }
            set
            {
                _sigShtForm = value;
                OnPropertyChanged("SigShtForm");
            }
        }

        public int SigShtBase
        {
            get { return _sigShtBase; }
            set
            {
                _sigShtBase = value;
                OnPropertyChanged("SigShtBase");
            }
        }

        public int SigFT
        {
            get { return _sigFT; }
            set
            {
                _sigFT = value;
                OnPropertyChanged("SigFT");
            }
        }

        public bool IsFA1
        {
            get { return _isFA1; }
            set
            {
                _isFA1 = value;
                OnPropertyChanged("IsFA1");
            }
        }

        public bool IsFA2
        {
            get { return _isFA2; }
            set
            {
                _isFA2 = value;
                OnPropertyChanged("IsFA2");
            }
        }

        private bool _isFA2;

        public bool RFA
        {
            get { return _rFA; }
            set
            {
                _rFA = value;
                OnPropertyChanged("RFA");
            }
        }

        private bool _rFA;

        public byte ContYBefOpt
        {
            get { return _contYBefOpt; }
            set
            {
                _contYBefOpt = value;
                OnPropertyChanged("ContYBefOpt");
            }
        }

        private byte _contYBefOpt;

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}