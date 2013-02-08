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
using System.Collections.ObjectModel;
using System.ComponentModel;

#endregion

namespace NBA_2K13_Roster_Editor.Data.Teams
{
    internal class TeamEntry : INotifyPropertyChanged
    {
        private Int16 _curPlaSta;
        private Int16 _curSeaSta;
        private int _plNum;
        private int _playbookID;
        private Int16 _prvPlaSta;
        private Int16 _prvSeaSta;
        private ObservableCollection<int> _rosterSpots;
        private short _stAsstCoach;
        private short _stHeadCoach;

        public TeamEntry()
        {
            RosterSpots = new ObservableCollection<int>();
        }

        public int ID { get; set; }

        public string Name { get; set; }

        public Int16 CurSeaSta
        {
            get { return _curSeaSta; }
            set
            {
                _curSeaSta = value;
                OnPropertyChanged("CurSeaSta");
            }
        }

        public Int16 CurPlaSta
        {
            get { return _curPlaSta; }
            set
            {
                _curPlaSta = value;
                OnPropertyChanged("CurPlaSta");
            }
        }

        public Int16 PrvSeaSta
        {
            get { return _prvSeaSta; }
            set
            {
                _prvSeaSta = value;
                OnPropertyChanged("PrvSeaSta");
            }
        }

        public Int16 PrvPlaSta
        {
            get { return _prvPlaSta; }
            set
            {
                _prvPlaSta = value;
                OnPropertyChanged("PrvPlaSta");
            }
        }

        public int PlaybookID
        {
            get { return _playbookID; }
            set
            {
                _playbookID = value;
                OnPropertyChanged("PlaybookID");
            }
        }

        public int PlNum
        {
            get { return _plNum; }
            set
            {
                _plNum = value;
                OnPropertyChanged("PlNum");
            }
        }

        public ObservableCollection<int> RosterSpots
        {
            get { return _rosterSpots; }
            set
            {
                _rosterSpots = value;
                OnPropertyChanged("RosterSpots");
            }
        }

        public short StHeadCoach
        {
            get { return _stHeadCoach; }
            set
            {
                _stHeadCoach = value;
                OnPropertyChanged("StHeadCoach");
            }
        }

        public short StAsstCoach
        {
            get { return _stAsstCoach; }
            set
            {
                _stAsstCoach = value;
                OnPropertyChanged("StAsstCoach");
            }
        }

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