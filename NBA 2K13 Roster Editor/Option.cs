using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace NBA_2K13_Roster_Editor
{
    class Option : INotifyPropertyChanged
    {
        private string _setting;
        private object _value;

        public string Setting
        {
            get { return _setting; }
            set { _setting = value; }
        }

        public object Value
        {
            get { return _value; }
            set { _value = value; OnPropertyChanged("Value");}
        }

        public Option()
        {
            
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
