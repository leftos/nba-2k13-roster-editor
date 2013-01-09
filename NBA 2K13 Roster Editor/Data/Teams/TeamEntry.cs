using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace NBA_2K13_Roster_Editor.Data.Teams
{
    internal class TeamEntry : INotifyPropertyChanged
    {
        private int _plNum;
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

        private Int16 _curSeaSta;

        public Int16 CurPlaSta
        {
            get { return _curPlaSta; }
            set
            {
                _curPlaSta = value;
                OnPropertyChanged("CurPlaSta");
            }
        }

        private Int16 _curPlaSta;

        public Int16 PrvSeaSta
        {
            get { return _prvSeaSta; }
            set
            {
                _prvSeaSta = value;
                OnPropertyChanged("PrvSeaSta");
            }
        }

        private Int16 _prvSeaSta;

        public Int16 PrvPlaSta
        {
            get { return _prvPlaSta; }
            set
            {
                _prvPlaSta = value;
                OnPropertyChanged("PrvPlaSta");
            }
        }

        private Int16 _prvPlaSta;

        public int PlaybookID
        {
            get { return _playbookID; }
            set
            {
                _playbookID = value;
                OnPropertyChanged("PlaybookID");
            }
        }

        private int _playbookID;

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