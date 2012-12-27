﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using NBA_2K13_Roster_Editor.Annotations;

namespace NBA_2K13_Roster_Editor.Data.Playbooks
{
    public class PlaybookEntry : INotifyPropertyChanged
    {
        public PlaybookEntry()
        {
            Plays = new ObservableCollection<string>();
        }

        //1099333 +3
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

        public ObservableCollection<string> Plays
        {
            get { return _plays; }
            set
            {
                _plays = value;
                OnPropertyChanged("Plays");
            }
        }

        private ObservableCollection<string> _plays;

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