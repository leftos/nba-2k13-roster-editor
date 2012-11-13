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