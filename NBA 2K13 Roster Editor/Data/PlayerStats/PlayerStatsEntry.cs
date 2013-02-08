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
using System.Collections.Generic;
using System.ComponentModel;
using NBA_2K13_Roster_Editor.Annotations;

#endregion

namespace NBA_2K13_Roster_Editor.Data.PlayerStats
{
    public class PlayerStatsEntry : INotifyPropertyChanged
    {
        private UInt16 _aST;
        private UInt16 _bLK;
        private UInt16 _dREB;
        private List<UInt16> _experimental;
        private UInt16 _fGA;
        private UInt16 _fGM;
        private UInt16 _fOUL;
        private UInt16 _fTA;
        private UInt16 _fTM;
        private UInt16 _gP;
        private UInt16 _gS;
        private int _iD;
        private UInt16 _mINS;
        private UInt16 _oREB;
        private UInt16 _pTS;
        private UInt16 _sTL;
        private UInt16 _tOS;
        private UInt16 _tPA;
        private UInt16 _tPM;
        private int _teamFin;

        private int _teamSta;

        public PlayerStatsEntry()
        {
            Experimental = new List<ushort>();
        }

        public int ID
        {
            get { return _iD; }
            set
            {
                _iD = value;
                OnPropertyChanged("ID");
            }
        }

        public int TeamSta
        {
            get { return _teamSta; }
            set
            {
                _teamSta = value;
                OnPropertyChanged("TeamSta");
            }
        }

        public int TeamFin
        {
            get { return _teamFin; }
            set
            {
                _teamFin = value;
                OnPropertyChanged("TeamFin");
            }
        }

        public UInt16 GP
        {
            get { return _gP; }
            set
            {
                _gP = value;
                OnPropertyChanged("GP");
            }
        }

        public UInt16 GS
        {
            get { return _gS; }
            set
            {
                _gS = value;
                OnPropertyChanged("GS");
            }
        }

        public UInt16 MINS
        {
            get { return _mINS; }
            set
            {
                _mINS = value;
                OnPropertyChanged("MINS");
            }
        }

        public UInt16 FGM
        {
            get { return _fGM; }
            set
            {
                _fGM = value;
                OnPropertyChanged("FGM");
            }
        }

        public UInt16 FGA
        {
            get { return _fGA; }
            set
            {
                _fGA = value;
                OnPropertyChanged("FGA");
            }
        }

        public UInt16 TPM
        {
            get { return _tPM; }
            set
            {
                _tPM = value;
                OnPropertyChanged("TPM");
            }
        }

        public UInt16 TPA
        {
            get { return _tPA; }
            set
            {
                _tPA = value;
                OnPropertyChanged("TPA");
            }
        }

        public UInt16 FTM
        {
            get { return _fTM; }
            set
            {
                _fTM = value;
                OnPropertyChanged("FTM");
            }
        }

        public UInt16 FTA
        {
            get { return _fTA; }
            set
            {
                _fTA = value;
                OnPropertyChanged("FTA");
            }
        }

        public UInt16 OREB
        {
            get { return _oREB; }
            set
            {
                _oREB = value;
                OnPropertyChanged("OREB");
            }
        }

        public UInt16 DREB
        {
            get { return _dREB; }
            set
            {
                _dREB = value;
                OnPropertyChanged("DREB");
            }
        }

        public UInt16 STL
        {
            get { return _sTL; }
            set
            {
                _sTL = value;
                OnPropertyChanged("STL");
            }
        }

        public UInt16 TOS
        {
            get { return _tOS; }
            set
            {
                _tOS = value;
                OnPropertyChanged("TOS");
            }
        }

        public UInt16 BLK
        {
            get { return _bLK; }
            set
            {
                _bLK = value;
                OnPropertyChanged("BLK");
            }
        }

        public UInt16 AST
        {
            get { return _aST; }
            set
            {
                _aST = value;
                OnPropertyChanged("AST");
            }
        }

        public UInt16 FOUL
        {
            get { return _fOUL; }
            set
            {
                _fOUL = value;
                OnPropertyChanged("FOUL");
            }
        }

        public UInt16 PTS
        {
            get { return _pTS; }
            set
            {
                _pTS = value;
                OnPropertyChanged("PTS");
            }
        }

        public List<UInt16> Experimental
        {
            get { return _experimental; }
            set
            {
                _experimental = value;
                OnPropertyChanged("Experimental");
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}