using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace NBA_2K13_Roster_Editor
{
    internal class PlayerEntry : INotifyPropertyChanged
    {
        private string _name;
        private int _id;
        private int _cfid;
        private int _plType;
        private bool _genericF;
        private int _portraitID;
        private List<SignatureSkills> _ssList;
        private List<byte> _ratings;
        private List<byte> _tendencies;
        private List<byte> _hotSpots;
        private List<HotZones> _hotZones;

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

        public List<SignatureSkills> SSList
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

        public List<HotZones> HotZones
        {
            get { return _hotZones; }
            set
            {
                _hotZones = value;
                OnPropertyChanged("HotZones");
            }
        }

        public PlayerEntry()
        {
            SSList = new List<SignatureSkills>();
            Ratings = new List<byte>();
            Tendencies = new List<byte>();
            HotSpots = new List<byte>();
            HotZones = new List<HotZones>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
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

    internal enum HotZones
    {
        Cold = 0,
        Neutral = 1,
        Hot = 2,
        Burned = 3
    }

    internal enum Ratings : int
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

    internal enum Tendencies : int
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

    internal enum HotSpots : int
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
}