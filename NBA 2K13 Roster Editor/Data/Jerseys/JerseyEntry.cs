using System.ComponentModel;

namespace NBA_2K13_Roster_Editor.Data.Jerseys
{
    internal class JerseyEntry : INotifyPropertyChanged
    {
        private int _id;
        private int _jerseyType;
        private string _teamColor1;
        private string _teamColor2;
        private string _teamColor3;
        private string _teamColor4;
        private string _teamColor5;
        private string _teamColor6;

        public int ID
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged("ID");
            }
        }

        public int JerseyType
        {
            get { return _jerseyType; }
            set
            {
                _jerseyType = value;
                OnPropertyChanged("JerseyType");
            }
        }

        public string TeamColor1
        {
            get { return _teamColor1; }
            set
            {
                _teamColor1 = value;
                OnPropertyChanged("TeamColor1");
            }
        }

        public string TeamColor2
        {
            get { return _teamColor2; }
            set
            {
                _teamColor2 = value;
                OnPropertyChanged("TeamColor2");
            }
        }

        public string TeamColor3
        {
            get { return _teamColor3; }
            set
            {
                _teamColor3 = value;
                OnPropertyChanged("TeamColor3");
            }
        }

        public string TeamColor4
        {
            get { return _teamColor4; }
            set
            {
                _teamColor4 = value;
                OnPropertyChanged("TeamColor4");
            }
        }

        public string TeamColor5
        {
            get { return _teamColor5; }
            set
            {
                _teamColor5 = value;
                OnPropertyChanged("TeamColor5");
            }
        }

        public string TeamColor6
        {
            get { return _teamColor6; }
            set
            {
                _teamColor6 = value;
                OnPropertyChanged("TeamColor6");
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