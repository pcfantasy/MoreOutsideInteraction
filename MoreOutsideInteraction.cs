using System;
using ColossalFramework;
using ColossalFramework.UI;
using ICities;
using System.Reflection;
using System.IO;
using System.Linq;
using ColossalFramework.Math;
using UnityEngine;
using ColossalFramework.Globalization;

namespace MoreOutsideInteraction
{
    public class MoreOutsideInteraction : IUserMod
    {
        public static bool IsEnabled = false;
        public static bool updateOnce = false;
        public static int languageIdex = 0;

        public static bool deadFromOutside = false;
        public static bool garbageFromOutside = false;
        public static bool garbageToOutside = false;
        public static bool crimeToOutside = false;
        public static bool sickToOutside = false;
        public static bool fireToOutside = false;


        public string Name
        {
            get { return "More Outside Interaction"; }
        }

        public string Description
        {
            get { return "Add policecar & ambulance & hearse & garbagetruck interaction with outside"; }
        }

        public void OnEnabled()
        {
            IsEnabled = true;
            FileStream fs = File.Create("MoreOutsideInteraction.txt");
            fs.Close();
            LoadSetting();
            SaveSetting();
            Language.LanguageSwitch((byte)languageIdex);
        }

        public void OnDisabled()
        {
            IsEnabled = false;
            Language.LanguageSwitch((byte)languageIdex);
        }


        public static void SaveSetting()
        {
            //save langugae
            FileStream fs = File.Create("MoreOutsideInteraction_setting.txt");
            StreamWriter streamWriter = new StreamWriter(fs);
            streamWriter.WriteLine(deadFromOutside);
            streamWriter.WriteLine(garbageFromOutside);
            streamWriter.WriteLine(garbageToOutside);
            streamWriter.WriteLine(crimeToOutside);
            streamWriter.WriteLine(sickToOutside);
            streamWriter.WriteLine(fireToOutside);
            streamWriter.Flush();
            fs.Close();
        }

        public static void LoadSetting()
        {
            if (File.Exists("MoreOutsideInteraction_setting.txt"))
            {
                FileStream fs = new FileStream("MoreOutsideInteraction_setting.txt", FileMode.Open);
                StreamReader sr = new StreamReader(fs);
                string strLine = sr.ReadLine();

                if (strLine == "False")
                {
                    deadFromOutside = false;
                }
                else
                {
                    deadFromOutside = true;
                }

                strLine = sr.ReadLine();

                if (strLine == "False")
                {
                    garbageFromOutside = false;
                }
                else
                {
                    garbageFromOutside = true;
                }

                strLine = sr.ReadLine();

                if (strLine == "False")
                {
                    garbageToOutside = false;
                }
                else
                {
                    garbageToOutside = true;
                }

                strLine = sr.ReadLine();

                if (strLine == "False")
                {
                    crimeToOutside = false;
                }
                else
                {
                    crimeToOutside = true;
                }

                strLine = sr.ReadLine();

                if (strLine == "False")
                {
                    sickToOutside = false;
                }
                else
                {
                    sickToOutside = true;
                }

                strLine = sr.ReadLine();

                if (strLine == "False")
                {
                    fireToOutside = false;
                }
                else
                {
                    fireToOutside = true;
                }

                sr.Close();
                fs.Close();
            }
        }


        public void OnSettingsUI(UIHelperBase helper)
        {

            LoadSetting();
            if (SingletonLite<LocaleManager>.instance.language.Contains("zh"))
            {
                Language.LanguageSwitch(1);
            }
            else
            {
                Language.LanguageSwitch(0);
            }

            UIHelperBase group1 = helper.AddGroup(Language.OptionUI[0]);
            group1.AddCheckbox(Language.OptionUI[1], deadFromOutside, (index) => deadFromOutsideConnection(index));
            group1.AddCheckbox(Language.OptionUI[2], garbageFromOutside, (index) => garbageFromOutsideConnection(index));
            group1.AddCheckbox(Language.OptionUI[3], garbageToOutside, (index) => garbageToOutsideConnection(index));
            group1.AddCheckbox(Language.OptionUI[4], crimeToOutside, (index) => crimeToOutsideConnection(index));
            group1.AddCheckbox(Language.OptionUI[5], sickToOutside, (index) => sickToOutsideConnection(index));
            group1.AddCheckbox(Language.OptionUI[6], fireToOutside, (index) => fireToOutsideConnection(index));
            SaveSetting();
        }

        public void deadFromOutsideConnection(bool index)
        {
            deadFromOutside = index;
            SaveSetting();
        }

        public void garbageFromOutsideConnection(bool index)
        {
            garbageFromOutside = index;
            SaveSetting();
        }

        public void garbageToOutsideConnection(bool index)
        {
            garbageToOutside = index;
            SaveSetting();
        }

        public void crimeToOutsideConnection(bool index)
        {
            crimeToOutside = index;
            SaveSetting();
        }

        public void sickToOutsideConnection(bool index)
        {
            sickToOutside = index;
            SaveSetting();
        }

        public void fireToOutsideConnection(bool index)
        {
            fireToOutside = index;
            SaveSetting();
        }

    }
}