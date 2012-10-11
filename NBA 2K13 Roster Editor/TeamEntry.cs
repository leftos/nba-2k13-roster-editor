using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace NBA_2K13_Roster_Editor
{
    class TeamEntry : INotifyPropertyChanged
    {
        private int _id;
        private string _name;
        private int _plNum;
        private List<int> _rosterSpots;

        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public int PlNum
        {
            get { return _plNum; }
            set { _plNum = value; OnPropertyChanged("PlNum");}
        }

        public List<int> RosterSpots
        {
            get { return _rosterSpots; }
            set { _rosterSpots = value; OnPropertyChanged("RosterSpots"); }
        }

        public TeamEntry()
        {
            RosterSpots = new List<int>();
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
