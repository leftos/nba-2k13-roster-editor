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

using System.Collections.Generic;
using System.ComponentModel;

#endregion

namespace NBA_2K13_Roster_Editor.Data.Jerseys
{
    internal class JerseyEntry : INotifyPropertyChanged
    {
        public static readonly Dictionary<string, string> JerseyNames = new Dictionary<string, string>
                                                                        {
                                                                            {"Home", "BF4718D0"},
                                                                            {"Away", "A2B39EF0"},
                                                                            {"Alternate", "CCBB71E8"},
                                                                            {"BackInBlack", "AC59B740"},
                                                                            {"Cavfanatic", "3C9CA188"},
                                                                            {"Christmas", "A2F5D7D8"},
                                                                            {"LatinNights", "49F9B0A0"},
                                                                            {"MardiGras", "51F9A520"},
                                                                            {"MilitaryNight", "EBDF3AD8"},
                                                                            {"NBAGreen", "EFDEB770"},
                                                                            {"Racing", "BE62D5B0"},
                                                                            {"RipCity", "2351B428"},
                                                                            {"StPatricks", "CD16A528"},
                                                                            {"WhiteHot", "F3992170"},
                                                                            {"ClassicHomeI", "3871FBA0"},
                                                                            {"ClassicAwayI", "C4BFEE20"},
                                                                            {"ClassAwayIAlt", "43963E40"},
                                                                            {"ClassicHomeII", "5EF2F550"},
                                                                            {"ClassicAwayII", "0703A720"},
                                                                            {"ClassicAwayIIAlt", "1B04E3B0"},
                                                                            {"ClassicHomeIII", "54CB88C0"},
                                                                            {"ClassicAwayIII", "D9B11D28"},
                                                                            {"ClassicAwayIIIAtl", "7A908530"},
                                                                            {"ClassicHomeIV", "537E3460"},
                                                                            {"ClassicAwayIV", "0A8F6610"},
                                                                            {"ClassicAwayIVAlt", "247053D8"},
                                                                            {"ClassicHomeV", "35FD3A90"},
                                                                            {"ClassicAwayV", "C9332F10"},
                                                                            {"ClassicAwayVAlt", "7CE28E28"},
                                                                            {"ClassicHomeVI", "CF935678"},
                                                                            {"ClassicAwayVI", "96620408"},
                                                                            {"ClassicAwayVIAlt", "E8B072E8"},
                                                                            {"ClassicHomeVII", "BC9E47D8"},
                                                                            {"ClassicAwayVII", "31E4D230"},
                                                                            {"j2012", "3B5EC208"},
                                                                            {"j2013", "0121C8D0"},
                                                                            {"Unknown1", "43E04D18"},
                                                                            {"j1990", "4A5A2B80"},
                                                                            {"j1991", "5C385408"},
                                                                            {"NBA2K13", "1BA16098"},
                                                                            {"j1992", "B341BC50"},
                                                                            {"j1986", "1DB2C458"},
                                                                            {"j1995", "882F9C28"},
                                                                            {"j1996", "125C4998"},
                                                                            {"j1998", "9B637C18"},
                                                                            {"j1965", "D8D25368"},
                                                                            {"j1971", "032B1688"},
                                                                            {"j1972", "9958C338"},
                                                                            {"j1985", "87C111E8"},
                                                                            {"Unknown2", "1764A778"},
                                                                            {"j1989", "20717B80"},
                                                                            {"Rookies", "307D6468"},
                                                                            {"Custom1", "307D646D"},
                                                                            {"Custom2", "307D646E"},
                                                                            {"Custom3", "307D646F"},
                                                                            {"j1993", "3633B018"},
                                                                            {"j2002", "589A1B50"},
                                                                            {"j1977", "BD373AB8"},
                                                                            {"j2001", "C2E9CEE0"},
                                                                            {"j1987", "EDCAF5F8"},
                                                                            {"j1994", "F2AC2AD8"},
                                                                            {"CAT", "00000000"},
                                                                        };

        private JerseyType _gid;
        private int _id;
        private JerseyName _name;
        private NeckType _neck;
        private SockColor _sockClr;
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

        public JerseyType GID
        {
            get { return _gid; }
            set
            {
                _gid = value;
                OnPropertyChanged("GID");
            }
        }

        public NeckType Neck
        {
            get { return _neck; }
            set
            {
                _neck = value;
                OnPropertyChanged("Neck");
            }
        }

        public SockColor SockClr
        {
            get { return _sockClr; }
            set
            {
                _sockClr = value;
                OnPropertyChanged("SockClr");
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

        public JerseyName Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
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