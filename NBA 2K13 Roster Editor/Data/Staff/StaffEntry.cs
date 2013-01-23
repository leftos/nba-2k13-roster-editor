using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using NBA_2K13_Roster_Editor.Annotations;

namespace NBA_2K13_Roster_Editor.Data.Staff
{
    class StaffEntry : INotifyPropertyChanged
    {
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

        private int _iD;

        public ObservableCollection<byte> CoachingProfile
        {
            get { return _coachingProfile; }
            set
            {
                _coachingProfile = value;
                OnPropertyChanged("CoachingProfile");
            }
        }

        private ObservableCollection<byte> _coachingProfile;
        
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

        private int _cFID;

        public int PortraitID
        {
            get { return _portraitID; }
            set
            {
                _portraitID = value;
                OnPropertyChanged("PortraitID");
            }
        }

        private int _portraitID;

        private string _headCoachOf;


        private int _playbookID;

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
