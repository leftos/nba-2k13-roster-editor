using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace NBA_2K13_Roster_Editor
{
    internal class PlayerEntry : INotifyPropertyChanged
    {
        // IMPORTANT NOTE
        // When adding a property to a Player Entry, you should take care of:
        // - Loading it via PopulatePlayersTab
        // - Saving it via btnSavePlayers_Click
        // - Making it pasteable via dgPlayers_PreviewKeyDown

        private string _name;
        private int _id;
        private int _cfid;
        private int _plType;
        private bool _genericF;
        private int _portraitID;
        private List<SignatureSkill> _ssList;
        private List<byte> _ratings;
        private List<byte> _tendencies;
        private List<byte> _hotSpots;
        private List<HotZone> _hotZones;
        private int _asaID;
        private ShoeBrand _shoeBrand;
        private int _shoeModel;
        private CAPHairType _capHairType;
        private byte _jerseyNumber;
        private MuscleTone _muscleTone;
        private BodyType _bodyType;
        private EyeColor _eyeColor;

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
            set { _asaID = value; OnPropertyChanged("ASAID"); }
        }

        public ShoeBrand ShoeBrand
        {
            get { return _shoeBrand; }
            set { _shoeBrand = value;
                OnPropertyChanged("ShoeBrand");
            }
        }

        public int ShoeModel
        {
            get { return _shoeModel; }
            set { _shoeModel = value; OnPropertyChanged("ShoeModel"); }
        }

        public List<SignatureSkill> SSList
        {
            get { return _ssList; }
            set
            {
                _ssList = value;
                OnPropertyChanged("SSList");
            }
        }

        public List<byte> Ratings
        {
            get { return _ratings; }
            set
            {
                _ratings = value;
                OnPropertyChanged("Ratings");
            }
        }

        public List<byte> Tendencies
        {
            get { return _tendencies; }
            set
            {
                _tendencies = value;
                OnPropertyChanged("Tendencies");
            }
        }

        public List<byte> HotSpots
        {
            get { return _hotSpots; }
            set
            {
                _hotSpots = value;
                OnPropertyChanged("HotSpots");
            }
        }

        public List<HotZone> HotZones
        {
            get { return _hotZones; }
            set
            {
                _hotZones = value;
                OnPropertyChanged("HotZones");
            }
        }

        public CAPHairType CAPHairType
        {
            get { return _capHairType; }
            set { _capHairType = value;
                OnPropertyChanged("CAPHairType");
            }
        }

        public byte JerseyNumber
        {
            get { return _jerseyNumber; }
            set { _jerseyNumber = value; OnPropertyChanged("JerseyNumber"); }
        }

        public MuscleTone MuscleTone
        {
            get { return _muscleTone; }
            set { _muscleTone = value; OnPropertyChanged("MuscleTone"); }
        }

        public BodyType BodyType
        {
            get { return _bodyType; }
            set { _bodyType = value; OnPropertyChanged("BodyType"); }
        }

        public EyeColor EyeColor
        {
            get { return _eyeColor; }
            set { _eyeColor = value; OnPropertyChanged("EyeColor"); }
        }

        public PlayerEntry()
        {
            SSList = new List<SignatureSkill>();
            Ratings = new List<byte>();
            Tendencies = new List<byte>();
            HotSpots = new List<byte>();
            HotZones = new List<HotZone>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
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

    internal enum Rating : int
    {
        ShotIns = 0,
        ShotCls = 1,
        ShotMid = 2,
        BallHnd = 3,
        Shot3PT = 4,
        FT = 5,
        Hidden = 6,
        Layup = 7,
        Dunk = 8,
        StndDnk = 9,
        ShtInTr = 10,
        ShtOffDr = 11,
        Hstl = 12,
        OffHndDr = 13,
        BallSec = 14,
        Pass = 15,
        LowPstD = 16,
        LowPstO = 17,
        Block = 18,
        Hands = 19,
        Steal = 20,
        Speed = 21,
        Stmn = 22,
        Emtn = 23,
        Vert = 24,
        OffRbd = 25,
        DefRbd = 26,
        Drblty = 27,
        DefAwr = 28,
        OffAwr = 29,
        Cnstnc = 30,
        OnBallD = 31,
        Quick = 32,
        Pot = 33,
        Strng = 34,
        LowPstFade = 35,
        LowPstHook = 36
    }

    internal enum Tendency : int
    {
        ShotTnd = 0,
        InsShot = 1,
        ClsShot = 2,
        MidShot = 3,
        Shot3PT = 4,
        DrvLane = 5,
        DrvRight = 6,
        PullUp = 7,
        PumpFake = 8,
        TrplThrt = 9,
        NoTrplThrt = 10,
        TrplThrtSht = 11,
        Sizeup = 12,
        Hesttn = 13,
        StrtDrbl = 14,
        Cross = 15,
        Spin = 16,
        Stepback = 17,
        Halfspin = 18,
        DblCross = 19,
        BhndBack = 20,
        HesCross = 21,
        InNOut = 22,
        SmplDrv = 23,
        Attack = 24,
        PassOut = 25,
        Hopstep = 26,
        SpnLayup = 27,
        Eurostep = 28,
        Runner = 29,
        Fade = 30,
        Dunk = 31,
        Crash = 32,
        Touch = 33,
        UsePick = 34,
        SetPick = 35,
        Isltn = 36,
        UseOffBScrn = 37,
        SetOffBScrn = 38,
        PostUp = 39,
        SpotUp = 40,
        PostSpin = 41,
        DropStep = 42,
        Shimmy = 43,
        FaceUp = 44,
        LeavePost = 45,
        BackDown = 46,
        AggrBackDown = 47,
        PostShot = 48,
        PostHook = 49,
        PostFade = 50,
        PostDrv = 51,
        HopShot = 52,
        Putback = 53,
        FlashyPass = 54,
        AlleyOop = 55,
        DrawFoul = 56,
        PlayPassLane = 57,
        TakeChrg = 58,
        OnBSteal = 59,
        Contest = 60,
        CommitFl = 61,
        HardFoul = 62,
        UseGlass = 63,
        StpbckJmpr = 64,
        SpnJmpr = 65,
        StepThru = 66,
        ThrowAlleyOop = 67,
        GiveNGo = 68
    }

    internal enum HotSpot : int
    {
        Iso3L = 0,
        Iso3C = 1,
        Iso3R = 2,
        IsoPstL = 3,
        IsoPstC = 4,
        IsoPstR = 5,
        Spt3LC = 6,
        Spt3LW = 7,
        Spt3T = 8,
        Spt3RW = 9,
        Spt3RC = 10,
        SptMidLB = 11,
        SptMidLW = 12,
        SptMidC = 13,
        SptMidRW = 14,
        SptMidRB = 15,
        PnRLC = 16,
        PnRLW = 17,
        PnRT = 18,
        PnRRW = 19,
        PnRRC = 20,
        PstRH = 21,
        PstRL = 22,
        PstLH = 23,
        PstLL = 24
    }

    internal enum ShoeBrand : byte
    {
        Generic = 0,
        Nike = 1,
        Adidas = 2,
        Jordan = 3,
        Converse = 4,
        Reebok = 5,
        UnderArmour = 6,
        Spalding = 7
    }

    internal enum EyeColor : byte
    {
        Blue = 0,
        Brown = 1,
        Green = 2,
        Hazel = 3,
        Amber = 4,
        Gray = 5
    }

    internal enum MuscleTone : byte
    {
        Buff = 0,
        Ripped = 1
    }

    internal enum BodyType: byte
    {
        Slim = 0,
        Normal = 1,
        Fat = 2,
        Athletic = 3
    }

    internal enum CAPHairType: byte
    {
        NoHair = 0,
        ShortStubble = 1,
        MediumStubble = 2,
        DarkStubble = 3,
        DarkRecessedStubble = 4,
        BaldingStubble = 5,
        ShortBuzz = 6,
        Buzz = 7,
        WidowPeakBuzz = 8,
        BaldingBuzz = 9,
        NaturalWaves = 10,
        NaturalPatches = 11,
        NaturalPart = 12,
        NaturalFauxhawk = 13,
        NaturalBalding = 14,
        ThickCornrows = 15,
        ThinCornrows = 16,
        Afro = 17,
        Messy = 18,
        Twisties = 19,
        ShortDreads = 20,
        MediumDreads = 21,
        TiedDreads = 22,
        DreadsTail = 23,
        Mop = 24,
        MopTail = 25,
        StraightShort = 26,
        StraightLong = 27,
        StraightFlat = 28,
        StraightPart = 29,
        StraightTail = 30,
        StraightBalding = 31,
        Spikey = 32,
        Curly = 33,
        BaldingFlat = 34,
        ShortFlat = 35,
        MediumFlat = 36,
        Wavy = 37,
        Shaggy = 38,
        Mohawk = 39,
        ThePatch = 40
    }
}