using System;
using System.Collections.Generic;
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

        public int CPRunPlays
        {
            get { return _cPRunPlays; }
            set
            {
                _cPRunPlays = value;
                OnPropertyChanged("CPRunPlays");
            }
        }

        private int _cPRunPlays;

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
