using ICities;
using System.IO;
using MoreOutsideInteraction.Util;
using CitiesHarmony.API;

namespace MoreOutsideInteraction
{
    public class MoreOutsideInteraction : IUserMod
    {
        public static bool IsEnabled = false;
        public static bool updateOnce = false;
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
            HarmonyHelper.EnsureHarmonyInstalled();
        }

        public void OnDisabled()
        {
            IsEnabled = false;
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

                if (strLine == "True")
                {
                    deadFromOutside = true;
                }
                else
                {
                    deadFromOutside = false;
                }

                strLine = sr.ReadLine();

                if (strLine == "True")
                {
                    garbageFromOutside = true;
                }
                else
                {
                    garbageFromOutside = false;
                }

                strLine = sr.ReadLine();

                if (strLine == "True")
                {
                    garbageToOutside = true;
                }
                else
                {
                    garbageToOutside = false;
                }

                strLine = sr.ReadLine();

                if (strLine == "True")
                {
                    crimeToOutside = true;
                }
                else
                {
                    crimeToOutside = false;
                }

                strLine = sr.ReadLine();

                if (strLine == "True")
                {
                    sickToOutside = true;
                }
                else
                {
                    sickToOutside = false;
                }

                strLine = sr.ReadLine();

                if (strLine == "True")
                {
                    fireToOutside = true;
                }
                else
                {
                    fireToOutside = false;
                }

                sr.Close();
                fs.Close();
            }
        }

        public void OnSettingsUI(UIHelperBase helper)
        {
            LoadSetting();
            UIHelperBase group = helper.AddGroup(Localization.Get("SELECT_TO_ENABLE"));
            group.AddCheckbox(Localization.Get("DEAD_FROM_OUTSIDE"), deadFromOutside, (index) => deadFromOutsideConnection(index));
            group.AddCheckbox(Localization.Get("GARBAGE_FROM_OUTSIDE"), garbageFromOutside, (index) => garbageFromOutsideConnection(index));
            group.AddCheckbox(Localization.Get("GARBAGE_TO_OUTSIDE"), garbageToOutside, (index) => garbageToOutsideConnection(index));
            group.AddCheckbox(Localization.Get("POLICE_TO_OUTSIDE"), crimeToOutside, (index) => crimeToOutsideConnection(index));
            group.AddCheckbox(Localization.Get("HOSPITAL_TO_OUTSIDE"), sickToOutside, (index) => sickToOutsideConnection(index));
            group.AddCheckbox(Localization.Get("FIRESTATION_TO_OUTSIDE"), fireToOutside, (index) => fireToOutsideConnection(index));
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