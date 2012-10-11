using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace NBA_2K13_Roster_Editor
{
    class PlayerEntry : INotifyPropertyChanged
    {
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

        public string Name
        {
            get { return _name; }
            set { 
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public int ID
        {
            get { return _id; }
            set { _id = value; OnPropertyChanged("ID"); }
        }

        public int CFID
        {
            get { return _cfid; }
            set { _cfid = value; OnPropertyChanged("CFID"); }
        }

        public int PlType
        {
            get { return _plType; }
            set { _plType = value; OnPropertyChanged("PlType"); }
        }

        public bool GenericF
        {
            get { return _genericF; }
            set { _genericF = value; OnPropertyChanged("GenericF"); }
        }

        public int PortraitID
        {
            get { return _portraitID; }
            set { _portraitID = value; OnPropertyChanged("PortraitID"); }
        }

        public List<SignatureSkill> SSList
        {
            get { return _ssList; }
            set { _ssList = value; OnPropertyChanged("SSList"); }
        }

        public List<byte> Ratings
        {
            get { return _ratings; }
            set { _ratings = value;
                OnPropertyChanged("Ratings");
            }
        }

        public List<byte> Tendencies
        {
            get { return _tendencies; }
            set { _tendencies = value; OnPropertyChanged("Tendencies"); }
        }

        public List<byte> HotSpots
        {
            get { return _hotSpots; }
            set { _hotSpots = value; OnPropertyChanged("HotSpots"); }
        }

        public List<HotZone> HotZones
        {
            get { return _hotZones; }
            set { _hotZones = value;
                OnPropertyChanged("HotZones");
            }
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
}
