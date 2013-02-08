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
using NBA_2K13_Roster_Editor.Annotations;

#endregion

namespace NBA_2K13_Roster_Editor.Data.Staff
{
    internal class StaffEntry : INotifyPropertyChanged
    {
        private int _cFID;
        private ObservableCollection<byte> _coachingProfile;
        private string _headCoachOf;
        private int _iD;
        private int _playbookID;
        private int _portraitID;

        public StaffEntry()
        {
            HeadCoachOf = "-1";
            CoachingProfile = new ObservableCollection<byte>();
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

        public ObservableCollection<byte> CoachingProfile
        {
            get { return _coachingProfile; }
            set
            {
                _coachingProfile = value;
                OnPropertyChanged("CoachingProfile");
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

        public string HeadCoachOf
        {
            get { return _headCoachOf; }
            set
            {
                _headCoachOf = value;
                OnPropertyChanged("HeadCoachOf");
            }
        }

        public int CFID
        {
            get { return _cFID; }
            set
            {
                _cFID = value;
                OnPropertyChanged("CFID");
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