using System;
using System.Collections.Generic;
using System.ComponentModel;
using NBA_2K13_Roster_Editor.Annotations;

namespace NBA_2K13_Roster_Editor.Data.PlayerStats
{
    public class PlayerStatsEntry : INotifyPropertyChanged
    {
        public int ID
        {
            get { return _iD; }
            set
            {
                _iD = value;
                OnPropertyChanged("ID");
            }
        }

        private int _iD;

        public int TeamSta
        {
            get { return _teamSta; }
            set
            {
                _teamSta = value;
                OnPropertyChanged("TeamSta");
            }
        }

        private int _teamSta;

        public int TeamFin
        {
            get { return _teamFin; }
            set
            {
                _teamFin = value;
                OnPropertyChanged("TeamFin");
            }
        }

        private int _teamFin;

        public UInt16 GP
        {
            get { return _gP; }
            set
            {
                _gP = value;
                OnPropertyChanged("GP");
            }
        }

        private UInt16 _gP;

        public UInt16 GS
        {
            get { return _gS; }
            set
            {
                _gS = value;
                OnPropertyChanged("GS");
            }
        }

        private UInt16 _gS;

        public UInt16 MINS
        {
            get { return _mINS; }
            set
            {
                _mINS = value;
                OnPropertyChanged("MINS");
            }
        }

        private UInt16 _mINS;

        public UInt16 FGM
        {
            get { return _fGM; }
            set
            {
                _fGM = value;
                OnPropertyChanged("FGM");
            }
        }

        private UInt16 _fGM;

        public UInt16 FGA
        {
            get { return _fGA; }
            set
            {
                _fGA = value;
                OnPropertyChanged("FGA");
            }
        }

        private UInt16 _fGA;

        public UInt16 TPM
        {
            get { return _tPM; }
            set
            {
                _tPM = value;
                OnPropertyChanged("TPM");
            }
        }

        private UInt16 _tPM;

        public UInt16 TPA
        {
            get { return _tPA; }
            set
            {
                _tPA = value;
                OnPropertyChanged("TPA");
            }
        }

        private UInt16 _tPA;

        public UInt16 FTM
        {
            get { return _fTM; }
            set
            {
                _fTM = value;
                OnPropertyChanged("FTM");
            }
        }

        private UInt16 _fTM;

        public UInt16 FTA
        {
            get { return _fTA; }
            set
            {
                _fTA = value;
                OnPropertyChanged("FTA");
            }
        }

        private UInt16 _fTA;

        public UInt16 OREB
        {
            get { return _oREB; }
            set
            {
                _oREB = value;
                OnPropertyChanged("OREB");
            }
        }

        private UInt16 _oREB;

        public UInt16 DREB
        {
            get { return _dREB; }
            set
            {
                _dREB = value;
                OnPropertyChanged("DREB");
            }
        }

        private UInt16 _dREB;

        public UInt16 STL
        {
            get { return _sTL; }
            set
            {
                _sTL = value;
                OnPropertyChanged("STL");
            }
        }

        private UInt16 _sTL;

        public UInt16 TOS
        {
            get { return _tOS; }
            set
            {
                _tOS = value;
                OnPropertyChanged("TOS");
            }
        }

        private UInt16 _tOS;

        public UInt16 BLK
        {
            get { return _bLK; }
            set
            {
                _bLK = value;
                OnPropertyChanged("BLK");
            }
        }

        private UInt16 _bLK;

        public UInt16 AST
        {
            get { return _aST; }
            set
            {
                _aST = value;
                OnPropertyChanged("AST");
            }
        }

        private UInt16 _aST;

        public UInt16 FOUL
        {
            get { return _fOUL; }
            set
            {
                _fOUL = value;
                OnPropertyChanged("FOUL");
            }
        }

        private UInt16 _fOUL;

        public UInt16 PTS
        {
            get { return _pTS; }
            set
            {
                _pTS = value;
                OnPropertyChanged("PTS");
            }
        }

        private UInt16 _pTS;

        public List<UInt16> Experimental
        {
            get { return _experimental; }
            set
            {
                _experimental = value;
                OnPropertyChanged("Experimental");
            }
        }

        private List<UInt16> _experimental;

        public PlayerStatsEntry()
        {
            Experimental = new List<ushort>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}