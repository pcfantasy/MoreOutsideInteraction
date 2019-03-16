using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MoreOutsideInteraction
{
    public class Language
    {
        public static string[] English =
        {
            "Select To Enable Outside Interaction",
            "Dead From Outside",                                                                                      //0
            "Garbage From Outside",                                                                               //1
            "Garbage Sevices To Outside",
            "Police Sevices To Outside",
            "First Aid Sevices To Outside",
            "Fire Sevices Outside",
        };



        public static string[] Chinese =
        {
            "选择要打开的外部交互",  //0
            "接受外部遗体",          //1
            "接受外部垃圾",          //2
            "去外部收集垃圾",        //3
            "去外部警务巡逻",        //4
            "去外部急救支援",        //5
            "去外部消防支援",        //6
        };

        public static string[] OptionUI = new string[English.Length];

        public static void LanguageSwitch(byte language)
        {
            if (language == 1)
            {
                for (int i = 0; i < English.Length; i++)
                {
                    OptionUI[i] = Chinese[i];
                }
            }
            else if (language == 0)
            {
                for (int i = 0; i < English.Length; i++)
                {
                    OptionUI[i] = English[i];
                }
            }
            else
            {
                DebugLog.LogToFileOnly("unknow language!! use English");
                for (int i = 0; i < English.Length; i++)
                {
                    OptionUI[i] = English[i];
                }
            }
        }
    }
}
