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
        // This makes sure jersey names show up in-game after a change
        #region Name Display
        
        public enum NameDisplay : ushort
        {
            PHI_PracticeHome = 0,
            PHI_PracticeAway = 0,
            CHA_PracticeHome = 184,
            CHA_PracticeAway = 184,
            MIL_PracticeHome = 360,
            MIL_PracticeAway = 360,
            CHI_PracticeHome = 520,
            CHI_PracticeAway = 520,
            CLE_PracticeHome = 712,
            CLE_PracticeAway = 712,
            BOS_PracticeHome = 880,
            BOS_PracticeAway = 880,
            LAC_PracticeHome = 1176,
            LAC_PracticeAway = 1176,
            MEM_PracticeHome = 1392,
            MEM_PracticeAway = 1392,
            ATL_PracticeHome = 1584,
            ATL_PracticeAway = 1584,
            MIA_PracticeHome = 1736,
            MIA_PracticeAway = 1736,
            NOH_PracticeHome = 1904,
            NOH_PracticeAway = 1904,
            UTA_PracticeHome = 2112,
            UTA_PracticeAway = 2112,
            SAC_PracticeHome = 2264,
            SAC_PracticeAway = 2264,
            NYK_PracticeHome = 2432,
            NYK_PracticeAway = 2432,
            LAL_PracticeHome = 2688,
            LAL_PracticeAway = 2688,
            ORL_PracticeHome = 2808,
            ORL_PracticeAway = 2808,
            DAL_PracticeHome = 3000,
            DAL_PracticeAway = 3000,
            BKN_PracticeHome = 3144,
            BKN_PracticeAway = 3144,
            DEN_PracticeHome = 3312,
            DEN_PracticeAway = 3312,
            IND_PracticeHome = 3560,
            IND_PracticeAway = 3560,
            DET_PracticeHome = 3808,
            DET_PracticeAway = 3808,
            TOR_PracticeHome = 4000,
            TOR_PracticeAway = 4000,
            HOU_PracticeHome = 4192,
            HOU_PracticeAway = 4192,
            SAS_PracticeHome = 4368,
            SAS_PracticeAway = 4368,
            PHX_PracticeHome = 4520,
            PHX_PracticeAway = 4520,
            OKC_PracticeHome = 4688,
            OKC_PracticeAway = 4688,
            MIN_PracticeHome = 5032,
            MIN_PracticeAway = 5032,
            POR_PracticeHome = 5312,
            POR_PracticeAway = 5312,
            GSW_PracticeHome = 5616,
            GSW_PracticeAway = 5616,
            WAS_PracticeHome = 5816,
            WAS_PracticeAway = 5816,
            PHI_Home = 4,
            PHI_Away = 6,
            PHI_Alternate = 0,
            PHI_ClassicAwayI = 1,
            PHI_ClassicHomeII = 0,
            PHI_ClassicAwayIII = 5,
            PHI_ClassicAwayIV = 5,
            PHI_ClassicAwayV = 2,
            PHI_ClassicHomeVI = 6,
            PHI_ClassicAwayVI = 0,
            PHI_ClassicAwayVIAlt = 3,
            PHI_ClassicHomeVII = 2,
            PHI_ClassicAwayVII = 6,
            CHA_Home = 188,
            CHA_Away = 190,
            CHA_Racing = 187,
            CHA_NBAGreen = 188,
            CHA_ClassicHomeI = 183,
            CHA_ClassicAwayI = 185,
            CHA_ClassAwayIAlt = 183,
            CHA_ClassicHomeII = 184,
            MIL_Home = 364,
            MIL_Away = 366,
            MIL_Alternate = 360,
            MIL_ClassicHomeI = 359,
            MIL_ClassicAwayI = 361,
            MIL_ClassicHomeII = 360,
            MIL_ClassicAwayII = 366,
            MIL_ClassicHomeIII = 361,
            MIL_ClassicAwayIV = 365,
            CHI_Home = 524,
            CHI_Away = 526,
            CHI_Alternate = 520,
            CHI_LatinNights = 523,
            CHI_StPatricks = 520,
            CHI_NBAGreen = 524,
            CHI_ClassicAwayI = 521,
            CHI_ClassicAwayII = 526,
            CHI_Christmas = 520,
            CLE_Home = 716,
            CLE_Away = 718,
            CLE_Alternate = 712,
            CLE_Cavfanatic = 717,
            CLE_ClassicHomeI = 711,
            CLE_ClassicAwayI = 713,
            CLE_ClassicHomeII = 712,
            CLE_ClassicAwayII = 718,
            CLE_ClassicHomeIII = 713,
            CLE_ClassicAwayIII = 717,
            CLE_ClassicHomeIV = 711,
            CLE_ClassicAwayIV = 717,
            CLE_ClassicAwayIVAlt = 712,
            CLE_ClassicHomeV = 712,
            CLE_ClassicAwayV = 714,
            CLE_ClassicAwayVAlt = 717,
            BOS_Home = 884,
            BOS_Away = 886,
            BOS_Alternate = 880,
            BOS_StPatricks = 880,
            BOS_ClassicHomeI = 879,
            BOS_Christmas = 880,
            LAC_Home = 1180,
            LAC_Away = 1182,
            LAC_Alternate = 1176,
            LAC_ClassicAwayI = 1177,
            LAC_ClassicHomeII = 1176,
            LAC_ClassicAwayII = 1182,
            LAC_ClassicAwayIIAlt = 1182,
            LAC_ClassicAwayIII = 1181,
            LAC_Christmas = 1176,
            MEM_Home = 1396,
            MEM_Away = 1398,
            MEM_Alternate = 1392,
            MEM_ClassicHomeI = 1391,
            MEM_ClassicAwayI = 1393,
            MEM_ClassicAwayII = 1398,
            ATL_Home = 1588,
            ATL_Away = 1590,
            ATL_Alternate = 1584,
            ATL_ClassicHomeI = 1583,
            ATL_ClassicAwayI = 1585,
            ATL_ClassicHomeII = 1584,
            ATL_ClassicAwayII = 1590,
            ATL_ClassicHomeIII = 1585,
            ATL_ClassicAwayIII = 1589,
            ATL_ClassicAwayIIIAtl = 1589,
            ATL_ClassicAwayIV = 1589,
            MIA_Home = 1740,
            MIA_Away = 1742,
            MIA_Alternate = 1736,
            MIA_WhiteHot = 1742,
            MIA_BackInBlack = 1738,
            MIA_LatinNights = 1739,
            MIA_ClassicHomeI = 1735,
            MIA_ClassicAwayI = 1737,
            MIA_ClassAwayIAlt = 1735,
            MIA_ClassicHomeII = 1736,
            MIA_Christmas = 1736,
            NOH_Home = 1908,
            NOH_Away = 1910,
            NOH_Alternate = 1904,
            NOH_MardiGras = 1906,
            NOH_ClassicHomeI = 1903,
            NOH_ClassicAwayI = 1905,
            NOH_ClassAwayIAlt = 1903,
            NOH_ClassicHomeII = 1904,
            NOH_ClassicAwayII = 1910,
            NOH_ClassicHomeIII = 1905,
            NOH_ClassicAwayIII = 1909,
            NOH_ClassicAwayIIIAtl = 1909,
            UTA_Home = 2116,
            UTA_Away = 2118,
            UTA_Alternate = 2112,
            UTA_ClassicHomeI = 2111,
            UTA_ClassicAwayI = 2113,
            UTA_ClassicHomeII = 2112,
            UTA_ClassicAwayII = 2118,
            UTA_ClassicAwayIII = 2117,
            UTA_ClassicHomeIV = 2111,
            UTA_ClassicAwayIV = 2117,
            UTA_ClassicAwayIVAlt = 2112,
            SAC_Home = 2268,
            SAC_Away = 2270,
            SAC_Alternate = 2264,
            SAC_ClassicAwayI = 2265,
            SAC_ClassicHomeII = 2264,
            SAC_ClassicAwayII = 2270,
            SAC_ClassicHomeIII = 2265,
            SAC_ClassicAwayIII = 2269,
            SAC_ClassicAwayIIIAtl = 2269,
            SAC_ClassicHomeIV = 2263,
            SAC_ClassicAwayV = 2266,
            NYK_Home = 2436,
            NYK_Away = 2438,
            NYK_LatinNights = 2435,
            NYK_StPatricks = 2432,
            NYK_ClassicHomeI = 2431,
            NYK_ClassicAwayI = 2433,
            NYK_ClassicHomeII = 2432,
            NYK_ClassicAwayII = 2438,
            NYK_ClassicAwayIII = 2437,
            NYK_Christmas = 2432,
            LAL_Home = 2692,
            LAL_Alternate = 2688,
            LAL_Away = 2694,
            LAL_LatinNights = 2691,
            LAL_ClassicHomeI = 2687,
            LAL_ClassicAwayI = 2689,
            LAL_ClassicHomeII = 2688,
            LAL_ClassicAwayII = 2694,
            LAL_ClassicHomeIII = 2689,
            LAL_ClassicAwayIII = 2693,
            LAL_ClassicHomeIV = 2687,
            LAL_Christmas = 2688,
            ORL_Home = 2812,
            ORL_Away = 2814,
            ORL_Alternate = 2808,
            ORL_LatinNights = 2811,
            ORL_ClassicHomeI = 2807,
            ORL_ClassicAwayI = 2809,
            ORL_ClassAwayIAlt = 2807,
            ORL_ClassicHomeII = 2808,
            ORL_ClassicAwayII = 2814,
            DAL_Home = 3004,
            DAL_Away = 3006,
            DAL_Alternate = 3000,
            DAL_LatinNights = 3003,
            DAL_ClassicHomeI = 2999,
            DAL_ClassicAwayI = 3001,
            BKN_Home = 3148,
            BKN_Away = 3150,
            BKN_ClassicAwayI = 3145,
            BKN_ClassicHomeII = 3144,
            BKN_ClassicAwayII = 3150,
            BKN_Christmas = 3144,
            DEN_Home = 3316,
            DEN_Away = 3318,
            DEN_Alternate = 3312,
            DEN_NBAGreen = 3316,
            DEN_ClassicHomeI = 3311,
            DEN_ClassicAwayI = 3313,
            DEN_ClassicHomeII = 3312,
            DEN_ClassicHomeIII = 3313,
            DEN_Christmas = 3312,
            IND_Home = 3564,
            IND_Away = 3566,
            IND_Alternate = 3560,
            IND_ClassicAwayI = 3561,
            IND_ClassicHomeII = 3560,
            IND_ClassicAwayII = 3566,
            IND_ClassicAwayIIAlt = 3566,
            IND_ClassicHomeIII = 3561,
            IND_ClassicAwayIV = 3565,
            DET_Home = 3812,
            DET_Away = 3814,
            DET_Alternate = 3808,
            DET_ClassicHomeI = 3807,
            DET_ClassicAwayI = 3809,
            DET_ClassicHomeII = 3808,
            DET_ClassicAwayII = 3814,
            DET_ClassicHomeIII = 3809,
            TOR_Home = 4004,
            TOR_Away = 4006,
            TOR_Alternate = 4000,
            TOR_MilitaryNight = 4005,
            TOR_StPatricks = 4000,
            TOR_ClassicHomeI = 3999,
            TOR_ClassicAwayI = 4001,
            TOR_ClassicHomeII = 4000,
            HOU_Home = 4196,
            HOU_Away = 4198,
            HOU_Alternate = 4192,
            HOU_LatinNights = 4195,
            HOU_ClassicHomeI = 4191,
            HOU_ClassicAwayI = 4193,
            HOU_ClassicHomeII = 4192,
            HOU_ClassicAwayII = 4198,
            HOU_Christmas = 4192,
            SAS_Home = 4372,
            SAS_Away = 4374,
            SAS_Alternate = 4368,
            SAS_LatinNights = 4371,
            SAS_ClassicHomeI = 4367,
            SAS_ClassicHomeII = 4368,
            PHX_Home = 4524,
            PHX_Away = 4526,
            PHX_Alternate = 4520,
            PHX_LatinNights = 4523,
            PHX_ClassicHomeI = 4519,
            PHX_ClassicAwayI = 4521,
            PHX_ClassicAwayII = 4526,
            OKC_Home = 4692,
            OKC_Away = 4694,
            OKC_Alternate = 4688,
            OKC_Christmas = 4688,
            MIN_Home = 5036,
            MIN_Away = 5038,
            MIN_Alternate = 5032,
            MIN_ClassicHomeI = 5031,
            MIN_ClassicAwayI = 5033,
            MIN_ClassicHomeII = 5032,
            MIN_ClassicAwayII = 5038,
            MIN_ClassicAwayIIAlt = 5038,
            MIN_ClassicHomeIII = 5033,
            POR_Home = 5316,
            POR_Away = 5318,
            POR_Alternate = 5312,
            POR_RipCity = 5315,
            POR_ClassicHomeI = 5311,
            POR_ClassicAwayI = 5313,
            POR_ClassicHomeII = 5312,
            POR_ClassicAwayII = 5318,
            GSW_Home = 5620,
            GSW_Away = 5622,
            GSW_ClassicHomeI = 5615,
            GSW_ClassicAwayI = 5617,
            GSW_ClassicHomeII = 5616,
            GSW_ClassicHomeIII = 5617,
            GSW_ClassicAwayIII = 5621,
            GSW_ClassicHomeIV = 5615,
            GSW_ClassicAwayIV = 5621,
            GSW_ClassicAwayIVAlt = 5616,
            GSW_ClassicHomeV = 5616,
            WAS_Home = 5820,
            WAS_Away = 5822,
            WAS_ClassicHomeI = 5815,
            WAS_ClassicAwayI = 5817,
            WAS_ClassicHomeII = 5816,
            WAS_ClassicHomeIII = 5817,
            WAS_ClassicHomeIV = 5815,
            WAS_ClassicAwayIV = 5821,
            WAS_ClassicAwayIVAlt = 5816,
        };
        #endregion
        #region Name Display Default
        public enum NameDisplayDefault : ushort
        {
            PHI_PracticeHome = 0,
            PHI_PracticeAway = 0,
            CHA_PracticeHome = 184,
            CHA_PracticeAway = 184,
            MIL_PracticeHome = 360,
            MIL_PracticeAway = 360,
            CHI_PracticeHome = 520,
            CHI_PracticeAway = 520,
            CLE_PracticeHome = 712,
            CLE_PracticeAway = 712,
            BOS_PracticeHome = 880,
            BOS_PracticeAway = 880,
            LAC_PracticeHome = 1176,
            LAC_PracticeAway = 1176,
            MEM_PracticeHome = 1392,
            MEM_PracticeAway = 1392,
            ATL_PracticeHome = 1584,
            ATL_PracticeAway = 1584,
            MIA_PracticeHome = 1736,
            MIA_PracticeAway = 1736,
            NOH_PracticeHome = 1904,
            NOH_PracticeAway = 1904,
            UTA_PracticeHome = 2112,
            UTA_PracticeAway = 2112,
            SAC_PracticeHome = 2264,
            SAC_PracticeAway = 2264,
            NYK_PracticeHome = 2432,
            NYK_PracticeAway = 2432,
            LAL_PracticeHome = 2688,
            LAL_PracticeAway = 2688,
            ORL_PracticeHome = 2808,
            ORL_PracticeAway = 2808,
            DAL_PracticeHome = 3000,
            DAL_PracticeAway = 3000,
            BKN_PracticeHome = 3144,
            BKN_PracticeAway = 3144,
            DEN_PracticeHome = 3312,
            DEN_PracticeAway = 3312,
            IND_PracticeHome = 3560,
            IND_PracticeAway = 3560,
            DET_PracticeHome = 3808,
            DET_PracticeAway = 3808,
            TOR_PracticeHome = 4000,
            TOR_PracticeAway = 4000,
            HOU_PracticeHome = 4192,
            HOU_PracticeAway = 4192,
            SAS_PracticeHome = 4368,
            SAS_PracticeAway = 4368,
            PHX_PracticeHome = 4520,
            PHX_PracticeAway = 4520,
            OKC_PracticeHome = 4688,
            OKC_PracticeAway = 4688,
            MIN_PracticeHome = 5000,
            MIN_PracticeAway = 5000,
            POR_PracticeHome = 5280,
            POR_PracticeAway = 5280,
            GSW_PracticeHome = 5584,
            GSW_PracticeAway = 5584,
            WAS_PracticeHome = 5784,
            WAS_PracticeAway = 5784,
            PHI_Home = 4,
            PHI_Away = 6,
            PHI_Alternate = 0,
            PHI_ClassicAwayI = 1,
            PHI_ClassicHomeII = 0,
            PHI_ClassicAwayIII = 5,
            PHI_ClassicAwayIV = 5,
            PHI_ClassicAwayV = 2,
            PHI_ClassicHomeVI = 6,
            PHI_ClassicAwayVI = 0,
            PHI_ClassicAwayVIAlt = 3,
            PHI_ClassicHomeVII = 2,
            PHI_ClassicAwayVII = 6,
            CHA_Home = 188,
            CHA_Away = 190,
            CHA_Racing = 187,
            CHA_NBAGreen = 188,
            CHA_ClassicHomeI = 183,
            CHA_ClassicAwayI = 185,
            CHA_ClassAwayIAlt = 183,
            CHA_ClassicHomeII = 184,
            MIL_Home = 364,
            MIL_Away = 366,
            MIL_Alternate = 360,
            MIL_ClassicHomeI = 359,
            MIL_ClassicAwayI = 361,
            MIL_ClassicHomeII = 360,
            MIL_ClassicAwayII = 366,
            MIL_ClassicHomeIII = 361,
            MIL_ClassicAwayIV = 365,
            CHI_Home = 524,
            CHI_Away = 526,
            CHI_Alternate = 520,
            CHI_LatinNights = 523,
            CHI_StPatricks = 520,
            CHI_NBAGreen = 524,
            CHI_ClassicAwayI = 521,
            CHI_ClassicAwayII = 526,
            CHI_Christmas = 520,
            CLE_Home = 716,
            CLE_Away = 718,
            CLE_Alternate = 712,
            CLE_Cavfanatic = 717,
            CLE_ClassicHomeI = 711,
            CLE_ClassicAwayI = 713,
            CLE_ClassicHomeII = 712,
            CLE_ClassicAwayII = 718,
            CLE_ClassicHomeIII = 713,
            CLE_ClassicAwayIII = 717,
            CLE_ClassicHomeIV = 711,
            CLE_ClassicAwayIV = 717,
            CLE_ClassicAwayIVAlt = 712,
            CLE_ClassicHomeV = 712,
            CLE_ClassicAwayV = 714,
            CLE_ClassicAwayVAlt = 717,
            BOS_Home = 884,
            BOS_Away = 886,
            BOS_Alternate = 880,
            BOS_StPatricks = 880,
            BOS_ClassicHomeI = 879,
            BOS_Christmas = 880,
            LAC_Home = 1180,
            LAC_Away = 1182,
            LAC_Alternate = 1176,
            LAC_ClassicAwayI = 1177,
            LAC_ClassicHomeII = 1176,
            LAC_ClassicAwayII = 1182,
            LAC_ClassicAwayIIAlt = 1182,
            LAC_ClassicAwayIII = 1181,
            LAC_Christmas = 1176,
            MEM_Home = 1396,
            MEM_Away = 1398,
            MEM_Alternate = 1392,
            MEM_ClassicHomeI = 1391,
            MEM_ClassicAwayI = 1393,
            MEM_ClassicAwayII = 1398,
            ATL_Home = 1588,
            ATL_Away = 1590,
            ATL_Alternate = 1584,
            ATL_ClassicHomeI = 1583,
            ATL_ClassicAwayI = 1585,
            ATL_ClassicHomeII = 1584,
            ATL_ClassicAwayII = 1590,
            ATL_ClassicHomeIII = 1585,
            ATL_ClassicAwayIII = 1589,
            ATL_ClassicAwayIIIAtl = 1589,
            ATL_ClassicAwayIV = 1589,
            MIA_Home = 1740,
            MIA_Away = 1742,
            MIA_Alternate = 1736,
            MIA_WhiteHot = 1742,
            MIA_BackInBlack = 1738,
            MIA_LatinNights = 1739,
            MIA_ClassicHomeI = 1735,
            MIA_ClassicAwayI = 1737,
            MIA_ClassAwayIAlt = 1735,
            MIA_ClassicHomeII = 1736,
            MIA_Christmas = 1736,
            NOH_Home = 1908,
            NOH_Away = 1910,
            NOH_Alternate = 1904,
            NOH_MardiGras = 1906,
            NOH_ClassicHomeI = 1903,
            NOH_ClassicAwayI = 1905,
            NOH_ClassAwayIAlt = 1903,
            NOH_ClassicHomeII = 1904,
            NOH_ClassicAwayII = 1910,
            NOH_ClassicHomeIII = 1905,
            NOH_ClassicAwayIII = 1909,
            NOH_ClassicAwayIIIAtl = 1909,
            UTA_Home = 2116,
            UTA_Away = 2118,
            UTA_Alternate = 2112,
            UTA_ClassicHomeI = 2111,
            UTA_ClassicAwayI = 2113,
            UTA_ClassicHomeII = 2112,
            UTA_ClassicAwayII = 2118,
            UTA_ClassicAwayIII = 2117,
            UTA_ClassicHomeIV = 2111,
            UTA_ClassicAwayIV = 2117,
            UTA_ClassicAwayIVAlt = 2112,
            SAC_Home = 2268,
            SAC_Away = 2270,
            SAC_Alternate = 2264,
            SAC_ClassicAwayI = 2265,
            SAC_ClassicHomeII = 2264,
            SAC_ClassicAwayII = 2270,
            SAC_ClassicHomeIII = 2265,
            SAC_ClassicAwayIII = 2269,
            SAC_ClassicAwayIIIAtl = 2269,
            SAC_ClassicHomeIV = 2263,
            SAC_ClassicAwayV = 2266,
            NYK_Home = 2436,
            NYK_Away = 2438,
            NYK_LatinNights = 2435,
            NYK_StPatricks = 2432,
            NYK_ClassicHomeI = 2431,
            NYK_ClassicAwayI = 2433,
            NYK_ClassicHomeII = 2432,
            NYK_ClassicAwayII = 2438,
            NYK_ClassicAwayIII = 2437,
            NYK_Christmas = 2432,
            LAL_Home = 2692,
            LAL_Alternate = 2688,
            LAL_Away = 2694,
            LAL_LatinNights = 2691,
            LAL_ClassicHomeI = 2687,
            LAL_ClassicAwayI = 2689,
            LAL_ClassicHomeII = 2688,
            LAL_ClassicAwayII = 2694,
            LAL_ClassicHomeIII = 2689,
            LAL_ClassicAwayIII = 2693,
            LAL_ClassicHomeIV = 2687,
            LAL_Christmas = 2688,
            ORL_Home = 2812,
            ORL_Away = 2814,
            ORL_Alternate = 2808,
            ORL_LatinNights = 2811,
            ORL_ClassicHomeI = 2807,
            ORL_ClassicAwayI = 2809,
            ORL_ClassAwayIAlt = 2807,
            ORL_ClassicHomeII = 2808,
            ORL_ClassicAwayII = 2814,
            DAL_Home = 3004,
            DAL_Away = 3006,
            DAL_Alternate = 3000,
            DAL_LatinNights = 3003,
            DAL_ClassicHomeI = 2999,
            DAL_ClassicAwayI = 3001,
            BKN_Home = 3148,
            BKN_Away = 3150,
            BKN_ClassicAwayI = 3145,
            BKN_ClassicHomeII = 3144,
            BKN_ClassicAwayII = 3150,
            BKN_Christmas = 3144,
            DEN_Home = 3316,
            DEN_Away = 3318,
            DEN_Alternate = 3312,
            DEN_NBAGreen = 3316,
            DEN_ClassicHomeI = 3311,
            DEN_ClassicAwayI = 3313,
            DEN_ClassicHomeII = 3312,
            DEN_ClassicHomeIII = 3313,
            DEN_Christmas = 3312,
            IND_Home = 3564,
            IND_Away = 3566,
            IND_Alternate = 3560,
            IND_ClassicAwayI = 3561,
            IND_ClassicHomeII = 3560,
            IND_ClassicAwayII = 3566,
            IND_ClassicAwayIIAlt = 3566,
            IND_ClassicHomeIII = 3561,
            IND_ClassicAwayIV = 3565,
            DET_Home = 3812,
            DET_Away = 3814,
            DET_Alternate = 3808,
            DET_ClassicHomeI = 3807,
            DET_ClassicAwayI = 3809,
            DET_ClassicHomeII = 3808,
            DET_ClassicAwayII = 3814,
            DET_ClassicHomeIII = 3809,
            TOR_Home = 4004,
            TOR_Away = 4006,
            TOR_Alternate = 4000,
            TOR_MilitaryNight = 4005,
            TOR_StPatricks = 4000,
            TOR_ClassicHomeI = 3999,
            TOR_ClassicAwayI = 4001,
            TOR_ClassicHomeII = 4000,
            HOU_Home = 4196,
            HOU_Away = 4198,
            HOU_Alternate = 4192,
            HOU_LatinNights = 4195,
            HOU_ClassicHomeI = 4191,
            HOU_ClassicAwayI = 4193,
            HOU_ClassicHomeII = 4192,
            HOU_ClassicAwayII = 4198,
            HOU_Christmas = 4192,
            SAS_Home = 4372,
            SAS_Away = 4374,
            SAS_Alternate = 4368,
            SAS_LatinNights = 4371,
            SAS_ClassicHomeI = 4367,
            SAS_ClassicHomeII = 4368,
            PHX_Home = 4524,
            PHX_Away = 4526,
            PHX_Alternate = 4520,
            PHX_LatinNights = 4523,
            PHX_ClassicHomeI = 4519,
            PHX_ClassicAwayI = 4521,
            PHX_ClassicAwayII = 4526,
            OKC_Home = 4692,
            OKC_Away = 4694,
            OKC_Alternate = 4688,
            OKC_Christmas = 4688,
            MIN_Home = 5004,
            MIN_Away = 5006,
            MIN_Alternate = 5000,
            MIN_ClassicHomeI = 4999,
            MIN_ClassicAwayI = 5001,
            MIN_ClassicHomeII = 5000,
            MIN_ClassicAwayII = 5006,
            MIN_ClassicAwayIIAlt = 5006,
            MIN_ClassicHomeIII = 5001,
            POR_Home = 5284,
            POR_Away = 5286,
            POR_Alternate = 5280,
            POR_RipCity = 5283,
            POR_ClassicHomeI = 5279,
            POR_ClassicAwayI = 5281,
            POR_ClassicHomeII = 5280,
            POR_ClassicAwayII = 5286,
            GSW_Home = 5588,
            GSW_Away = 5590,
            GSW_ClassicHomeI = 5583,
            GSW_ClassicAwayI = 5585,
            GSW_ClassicHomeII = 5584,
            GSW_ClassicHomeIII = 5585,
            GSW_ClassicAwayIII = 5589,
            GSW_ClassicHomeIV = 5583,
            GSW_ClassicAwayIV = 5589,
            GSW_ClassicAwayIVAlt = 5584,
            GSW_ClassicHomeV = 5584,
            WAS_Home = 5788,
            WAS_Away = 5790,
            WAS_ClassicHomeI = 5783,
            WAS_ClassicAwayI = 5785,
            WAS_ClassicHomeII = 5784,
            WAS_ClassicHomeIII = 5785,
            WAS_ClassicHomeIV = 5783,
            WAS_ClassicAwayIV = 5789,
            WAS_ClassicAwayIVAlt = 5784,
        };
        #endregion

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