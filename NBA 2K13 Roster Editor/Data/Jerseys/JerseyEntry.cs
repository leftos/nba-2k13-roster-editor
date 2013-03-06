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
                                                                            {"Practice", "47CA3480"},
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
        // This makes sure jersey name show up in-game after a change
        public static readonly Dictionary<string, string> JerseyNameDisplay = new Dictionary<string, string>
                                                                            {
                                                                                {"PHI_PracticeHome", "0959"},
                                                                                {"PHI_PracticeAway", "0959"},
                                                                                {"CHA_PracticeHome", "0A11"},
                                                                                {"CHA_PracticeAway", "0A11"},
                                                                                {"MIL_PracticeHome", "0AC1"},
                                                                                {"MIL_PracticeAway", "0AC1"},
                                                                                {"CHI_PracticeHome", "0B61"},
                                                                                {"CHI_PracticeAway", "0B61"},
                                                                                {"CLE_PracticeHome", "0C21"},
                                                                                {"CLE_PracticeAway", "0C21"},
                                                                                {"BOS_PracticeHome", "0CC9"},
                                                                                {"BOS_PracticeAway", "0CC9"},
                                                                                {"LAC_PracticeHome", "0DF1"},
                                                                                {"LAC_PracticeAway", "0DF1"},
                                                                                {"MEM_PracticeHome", "0EC9"},
                                                                                {"MEM_PracticeAway", "0EC9"},
                                                                                {"ATL_PracticeHome", "0F89"},
                                                                                {"ATL_PracticeAway", "0F89"},
                                                                                {"MIA_PracticeHome", "1021"},
                                                                                {"MIA_PracticeAway", "1021"},
                                                                                {"NOH_PracticeHome", "10C9"},
                                                                                {"NOH_PracticeAway", "10C9"},
                                                                                {"UTA_PracticeHome", "1199"},
                                                                                {"UTA_PracticeAway", "1199"},
                                                                                {"SAC_PracticeHome", "1231"},
                                                                                {"SAC_PracticeAway", "1231"},
                                                                                {"NYK_PracticeHome", "12D9"},
                                                                                {"NYK_PracticeAway", "12D9"},
                                                                                {"LAL_PracticeHome", "13D9"},
                                                                                {"LAL_PracticeAway", "13D9"},
                                                                                {"ORL_PracticeHome", "1451"},
                                                                                {"ORL_PracticeAway", "1451"},
                                                                                {"DAL_PracticeHome", "1511"},
                                                                                {"DAL_PracticeAway", "1511"},
                                                                                {"BKN_PracticeHome", "15A1"},
                                                                                {"BKN_PracticeAway", "15A1"},
                                                                                {"DEN_PracticeHome", "1649"},
                                                                                {"DEN_PracticeAway", "1649"},
                                                                                {"IND_PracticeHome", "1741"},
                                                                                {"IND_PracticeAway", "1741"},
                                                                                {"DET_PracticeHome", "1839"},
                                                                                {"DET_PracticeAway", "1839"},
                                                                                {"TOR_PracticeHome", "18F9"},
                                                                                {"TOR_PracticeAway", "18F9"},
                                                                                {"HOU_PracticeHome", "19B9"},
                                                                                {"HOU_PracticeAway", "19B9"},
                                                                                {"SAS_PracticeHome", "1A69"},
                                                                                {"SAS_PracticeAway", "1A69"},
                                                                                {"PHX_PracticeHome", "1B01"},
                                                                                {"PHX_PracticeAway", "1B01"},
                                                                                {"OKC_PracticeHome", "1BA9"},
                                                                                {"OKC_PracticeAway", "1BA9"},
                                                                                {"MIN_PracticeHome", "1CE1"},
                                                                                {"MIN_PracticeAway", "1CE1"},
                                                                                {"POR_PracticeHome", "1DF9"},
                                                                                {"POR_PracticeAway", "1DF9"},
                                                                                {"GSW_PracticeHome", "1F29"},
                                                                                {"GSW_PracticeAway", "1F29"},
                                                                                {"WAS_PracticeHome", "1FF1"},
                                                                                {"WAS_PracticeAway", "1FF1"},
                                                                                {"PHI_Home", "095D"},
                                                                                {"PHI_Away", "095F"},
                                                                                {"PHI_Alternate", "0959"},
                                                                                {"PHI_ClassicAwayI", "095A"},
                                                                                {"PHI_ClassicHomeII", "0959"},
                                                                                {"PHI_ClassicAwayIII", "095E"},
                                                                                {"PHI_ClassicAwayIV", "095E"},
                                                                                {"PHI_ClassicAwayV", "095B"},
                                                                                {"PHI_ClassicHomeVI", "095F"},
                                                                                {"PHI_ClassicAwayVI", "0959"},
                                                                                {"PHI_ClassicAwayVIAlt", "095C"},
                                                                                {"PHI_ClassicHomeVII", "095B"},
                                                                                {"PHI_ClassicAwayVII", "095F"},
                                                                                {"CHA_Home", "0A15"},
                                                                                {"CHA_Away", "0A17"},
                                                                                {"CHA_Racing", "0A14"},
                                                                                {"CHA_NBAGreen", "0A15"},
                                                                                {"CHA_ClassicHomeI", "0A10"},
                                                                                {"CHA_ClassicAwayI", "0A12"},
                                                                                {"CHA_ClassAwayIAlt", "0A10"},
                                                                                {"CHA_ClassicHomeII", "0A11"},
                                                                                {"MIL_Home", "0AC5"},
                                                                                {"MIL_Away", "0AC7"},
                                                                                {"MIL_Alternate", "0AC1"},
                                                                                {"MIL_ClassicHomeI", "0AC0"},
                                                                                {"MIL_ClassicAwayI", "0AC2"},
                                                                                {"MIL_ClassicHomeII", "0AC1"},
                                                                                {"MIL_ClassicAwayII", "0AC7"},
                                                                                {"MIL_ClassicHomeIII", "0AC2"},
                                                                                {"MIL_ClassicAwayIV", "0AC6"},
                                                                                {"CHI_Home", "0B65"},
                                                                                {"CHI_Away", "0B67"},
                                                                                {"CHI_Alternate", "0B61"},
                                                                                {"CHI_LatinNights", "0B64"},
                                                                                {"CHI_StPatricks", "0B61"},
                                                                                {"CHI_NBAGreen", "0B65"},
                                                                                {"CHI_ClassicAwayI", "0B62"},
                                                                                {"CHI_ClassicAwayII", "0B67"},
                                                                                {"CHI_Christmas", "0B61"},
                                                                                {"CLE_Home", "0C25"},
                                                                                {"CLE_Away", "0C27"},
                                                                                {"CLE_Alternate", "0C21"},
                                                                                {"CLE_Cavfanatic", "0C26"},
                                                                                {"CLE_ClassicHomeI", "0C20"},
                                                                                {"CLE_ClassicAwayI", "0C22"},
                                                                                {"CLE_ClassicHomeII", "0C21"},
                                                                                {"CLE_ClassicAwayII", "0C27"},
                                                                                {"CLE_ClassicHomeIII", "0C22"},
                                                                                {"CLE_ClassicAwayIII", "0C26"},
                                                                                {"CLE_ClassicHomeIV", "0C20"},
                                                                                {"CLE_ClassicAwayIV", "0C26"},
                                                                                {"CLE_ClassicAwayIVAlt", "0C21"},
                                                                                {"CLE_ClassicHomeV", "0C21"},
                                                                                {"CLE_ClassicAwayV", "0C23"},
                                                                                {"CLE_ClassicAwayVAlt", "0C26"},
                                                                                {"BOS_Home", "0CCD"},
                                                                                {"BOS_Away", "0CCF"},
                                                                                {"BOS_Alternate", "0CC9"},
                                                                                {"BOS_StPatricks", "0CC9"},
                                                                                {"BOS_ClassicHomeI", "0CC8"},
                                                                                {"BOS_Christmas", "0CC9"},
                                                                                {"LAC_Home", "0DF5"},
                                                                                {"LAC_Away", "0DF7"},
                                                                                {"LAC_Alternate", "0DF1"},
                                                                                {"LAC_ClassicAwayI", "0DF2"},
                                                                                {"LAC_ClassicHomeII", "0DF1"},
                                                                                {"LAC_ClassicAwayII", "0DF7"},
                                                                                {"LAC_ClassicAwayIIAlt", "0DF7"},
                                                                                {"LAC_ClassicAwayIII", "0DF6"},
                                                                                {"LAC_Christmas", "0DF1"},
                                                                                {"MEM_Home", "0ECD"},
                                                                                {"MEM_Away", "0ECF"},
                                                                                {"MEM_Alternate", "0EC9"},
                                                                                {"MEM_ClassicHomeI", "0EC8"},
                                                                                {"MEM_ClassicAwayI", "0ECA"},
                                                                                {"MEM_ClassicAwayII", "0ECF"},
                                                                                {"ATL_Home", "0F8D"},
                                                                                {"ATL_Away", "0F8F"},
                                                                                {"ATL_Alternate", "0F89"},
                                                                                {"ATL_ClassicHomeI", "0F88"},
                                                                                {"ATL_ClassicAwayI", "0F8A"},
                                                                                {"ATL_ClassicHomeII", "0F89"},
                                                                                {"ATL_ClassicAwayII", "0F8F"},
                                                                                {"ATL_ClassicHomeIII", "0F8A"},
                                                                                {"ATL_ClassicAwayIII", "0F8E"},
                                                                                {"ATL_ClassicAwayIIIAtl", "0F8E"},
                                                                                {"ATL_ClassicAwayIV", "0F8E"},
                                                                                {"MIA_Home", "1025"},
                                                                                {"MIA_Away", "1027"},
                                                                                {"MIA_Alternate", "1021"},
                                                                                {"MIA_WhiteHot", "1027"},
                                                                                {"MIA_BackInBlack", "1023"},
                                                                                {"MIA_LatinNights", "1024"},
                                                                                {"MIA_ClassicHomeI", "1020"},
                                                                                {"MIA_ClassicAwayI", "1022"},
                                                                                {"MIA_ClassAwayIAlt", "1020"},
                                                                                {"MIA_ClassicHomeII", "1021"},
                                                                                {"MIA_Christmas", "1021"},
                                                                                {"NOH_Home", "10CD"},
                                                                                {"NOH_Away", "10CF"},
                                                                                {"NOH_Alternate", "10C9"},
                                                                                {"NOH_MardiGras", "10CB"},
                                                                                {"NOH_ClassicHomeI", "10C8"},
                                                                                {"NOH_ClassicAwayI", "10CA"},
                                                                                {"NOH_ClassAwayIAlt", "10C8"},
                                                                                {"NOH_ClassicHomeII", "10C9"},
                                                                                {"NOH_ClassicAwayII", "10CF"},
                                                                                {"NOH_ClassicHomeIII", "10CA"},
                                                                                {"NOH_ClassicAwayIII", "10CE"},
                                                                                {"NOH_ClassicAwayIIIAtl", "10CE"},
                                                                                {"UTA_Home", "119D"},
                                                                                {"UTA_Away", "119F"},
                                                                                {"UTA_Alternate", "1199"},
                                                                                {"UTA_ClassicHomeI", "1198"},
                                                                                {"UTA_ClassicAwayI", "119A"},
                                                                                {"UTA_ClassicHomeII", "1199"},
                                                                                {"UTA_ClassicAwayII", "119F"},
                                                                                {"UTA_ClassicAwayIII", "119E"},
                                                                                {"UTA_ClassicHomeIV", "1198"},
                                                                                {"UTA_ClassicAwayIV", "119E"},
                                                                                {"UTA_ClassicAwayIVAlt", "1199"},
                                                                                {"SAC_Home", "1235"},
                                                                                {"SAC_Away", "1237"},
                                                                                {"SAC_Alternate", "1231"},
                                                                                {"SAC_ClassicAwayI", "1232"},
                                                                                {"SAC_ClassicHomeII", "1231"},
                                                                                {"SAC_ClassicAwayII", "1237"},
                                                                                {"SAC_ClassicHomeIII", "1232"},
                                                                                {"SAC_ClassicAwayIII", "1236"},
                                                                                {"SAC_ClassicAwayIIIAtl", "1236"},
                                                                                {"SAC_ClassicHomeIV", "1230"},
                                                                                {"SAC_ClassicAwayV", "1233"},
                                                                                {"NYK_Home", "12DD"},
                                                                                {"NYK_Away", "12DF"},
                                                                                {"NYK_LatinNights", "12DC"},
                                                                                {"NYK_StPatricks", "12D9"},
                                                                                {"NYK_ClassicHomeI", "12D8"},
                                                                                {"NYK_ClassicAwayI", "12DA"},
                                                                                {"NYK_ClassicHomeII", "12D9"},
                                                                                {"NYK_ClassicAwayII", "12DF"},
                                                                                {"NYK_ClassicAwayIII", "12DE"},
                                                                                {"NYK_Christmas", "12D9"},
                                                                                {"LAL_Home", "13DD"},
                                                                                {"LAL_Alternate", "13D9"},
                                                                                {"LAL_Away", "13DF"},
                                                                                {"LAL_LatinNights", "13DC"},
                                                                                {"LAL_ClassicHomeI", "13D8"},
                                                                                {"LAL_ClassicAwayI", "13DA"},
                                                                                {"LAL_ClassicHomeII", "13D9"},
                                                                                {"LAL_ClassicAwayII", "13DF"},
                                                                                {"LAL_ClassicHomeIII", "13DA"},
                                                                                {"LAL_ClassicAwayIII", "13DE"},
                                                                                {"LAL_ClassicHomeIV", "13D8"},
                                                                                {"LAL_Christmas", "13D9"},
                                                                                {"ORL_Home", "1455"},
                                                                                {"ORL_Away", "1457"},
                                                                                {"ORL_Alternate", "1451"},
                                                                                {"ORL_LatinNights", "1454"},
                                                                                {"ORL_ClassicHomeI", "1450"},
                                                                                {"ORL_ClassicAwayI", "1452"},
                                                                                {"ORL_ClassAwayIAlt", "1450"},
                                                                                {"ORL_ClassicHomeII", "1451"},
                                                                                {"ORL_ClassicAwayII", "1457"},
                                                                                {"DAL_Home", "1515"},
                                                                                {"DAL_Away", "1517"},
                                                                                {"DAL_Alternate", "1511"},
                                                                                {"DAL_LatinNights", "1514"},
                                                                                {"DAL_ClassicHomeI", "1510"},
                                                                                {"DAL_ClassicAwayI", "1512"},
                                                                                {"BKN_Home", "15A5"},
                                                                                {"BKN_Away", "15A7"},
                                                                                {"BKN_ClassicAwayI", "15A2"},
                                                                                {"BKN_ClassicHomeII", "15A1"},
                                                                                {"BKN_ClassicAwayII", "15A7"},
                                                                                {"BKN_Christmas", "15A1"},
                                                                                {"DEN_Home", "164D"},
                                                                                {"DEN_Away", "164F"},
                                                                                {"DEN_Alternate", "1649"},
                                                                                {"DEN_NBAGreen", "164D"},
                                                                                {"DEN_ClassicHomeI", "1648"},
                                                                                {"DEN_ClassicAwayI", "164A"},
                                                                                {"DEN_ClassicHomeII", "1649"},
                                                                                {"DEN_ClassicHomeIII", "164A"},
                                                                                {"DEN_Christmas", "1649"},
                                                                                {"IND_Home", "1745"},
                                                                                {"IND_Away", "1747"},
                                                                                {"IND_Alternate", "1741"},
                                                                                {"IND_ClassicAwayI", "1742"},
                                                                                {"IND_ClassicHomeII", "1741"},
                                                                                {"IND_ClassicAwayII", "1747"},
                                                                                {"IND_ClassicAwayIIAlt", "1747"},
                                                                                {"IND_ClassicHomeIII", "1742"},
                                                                                {"IND_ClassicAwayIV", "1746"},
                                                                                {"DET_Home", "183D"},
                                                                                {"DET_Away", "183F"},
                                                                                {"DET_Alternate", "1839"},
                                                                                {"DET_ClassicHomeI", "1838"},
                                                                                {"DET_ClassicAwayI", "183A"},
                                                                                {"DET_ClassicHomeII", "1839"},
                                                                                {"DET_ClassicAwayII", "183F"},
                                                                                {"DET_ClassicHomeIII", "183A"},
                                                                                {"TOR_Home", "18FD"},
                                                                                {"TOR_Away", "18FF"},
                                                                                {"TOR_Alternate", "18F9"},
                                                                                {"TOR_MilitaryNight", "18FE"},
                                                                                {"TOR_StPatricks", "18F9"},
                                                                                {"TOR_ClassicHomeI", "18F8"},
                                                                                {"TOR_ClassicAwayI", "18FA"},
                                                                                {"TOR_ClassicHomeII", "18F9"},
                                                                                {"HOU_Home", "19BD"},
                                                                                {"HOU_Away", "19BF"},
                                                                                {"HOU_Alternate", "19B9"},
                                                                                {"HOU_LatinNights", "19BC"},
                                                                                {"HOU_ClassicHomeI", "19B8"},
                                                                                {"HOU_ClassicAwayI", "19BA"},
                                                                                {"HOU_ClassicHomeII", "19B9"},
                                                                                {"HOU_ClassicAwayII", "19BF"},
                                                                                {"HOU_Christmas", "19B9"},
                                                                                {"SAS_Home", "1A6D"},
                                                                                {"SAS_Away", "1A6F"},
                                                                                {"SAS_Alternate", "1A69"},
                                                                                {"SAS_LatinNights", "1A6C"},
                                                                                {"SAS_ClassicHomeI", "1A68"},
                                                                                {"SAS_ClassicHomeII", "1A69"},
                                                                                {"PHX_Home", "1B05"},
                                                                                {"PHX_Away", "1B07"},
                                                                                {"PHX_Alternate", "1B01"},
                                                                                {"PHX_LatinNights", "1B04"},
                                                                                {"PHX_ClassicHomeI", "1B00"},
                                                                                {"PHX_ClassicAwayI", "1B02"},
                                                                                {"PHX_ClassicAwayII", "1B07"},
                                                                                {"OKC_Home", "1BAD"},
                                                                                {"OKC_Away", "1BAF"},
                                                                                {"OKC_Alternate", "1BA9"},
                                                                                {"OKC_Christmas", "1BA9"},
                                                                                {"MIN_Home", "1CE5"},
                                                                                {"MIN_Away", "1CE7"},
                                                                                {"MIN_Alternate", "1CE1"},
                                                                                {"MIN_ClassicHomeI", "1CE0"},
                                                                                {"MIN_ClassicAwayI", "1CE2"},
                                                                                {"MIN_ClassicHomeII", "1CE1"},
                                                                                {"MIN_ClassicAwayII", "1CE7"},
                                                                                {"MIN_ClassicAwayIIAlt", "1CE7"},
                                                                                {"MIN_ClassicHomeIII", "1CE2"},
                                                                                {"POR_Home", "1DFD"},
                                                                                {"POR_Away", "1DFF"},
                                                                                {"POR_Alternate", "1DF9"},
                                                                                {"POR_RipCity", "1DFC"},
                                                                                {"POR_ClassicHomeI", "1DF8"},
                                                                                {"POR_ClassicAwayI", "1DFA"},
                                                                                {"POR_ClassicHomeII", "1DF9"},
                                                                                {"POR_ClassicAwayII", "1DFF"},
                                                                                {"GSW_Home", "1F2D"},
                                                                                {"GSW_Away", "1F2F"},
                                                                                {"GSW_ClassicHomeI", "1F28"},
                                                                                {"GSW_ClassicAwayI", "1F2A"},
                                                                                {"GSW_ClassicHomeII", "1F29"},
                                                                                {"GSW_ClassicHomeIII", "1F2A"},
                                                                                {"GSW_ClassicAwayIII", "1F2E"},
                                                                                {"GSW_ClassicHomeIV", "1F28"},
                                                                                {"GSW_ClassicAwayIV", "1F2E"},
                                                                                {"GSW_ClassicAwayIVAlt", "1F29"},
                                                                                {"GSW_ClassicHomeV", "1F29"},
                                                                                {"WAS_Home", "1FF5"},
                                                                                {"WAS_Away", "1FF7"},
                                                                                {"WAS_ClassicHomeI", "1FF0"},
                                                                                {"WAS_ClassicAwayI", "1FF2"},
                                                                                {"WAS_ClassicHomeII", "1FF1"},
                                                                                {"WAS_ClassicHomeIII", "1FF2"},
                                                                                {"WAS_ClassicHomeIV", "1FF0"},
                                                                                {"WAS_ClassicAwayIV", "1FF6"},
                                                                                {"WAS_ClassicAwayIVAlt", "1FF1"},
                                                                            };

        private JerseyType _gid;
        private int _id;
        private JerseyName _name;
        private JerseyArt _art;
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

        public JerseyArt Art
        {
            get { return _art; }
            set
            {
                _art = value;
                OnPropertyChanged("Art");
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