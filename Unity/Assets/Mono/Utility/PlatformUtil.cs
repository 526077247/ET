using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ET
{
    public class PlatformUtil
    {

        public static int intPlatform
        {
            get
            {
                return GameUtility.GetIntPlatform();
            }
        }

        public static int GetIntPlatform()
        {
            return intPlatform;
        }

        public static string GetStrPlatform()
        {
            if (IsIphone())
                return "ios";
            else if (IsAndroid())
                return "android";
            else if (IsWindows())
                return "pc";
            else
                return "";
        }

        public static string GetStrPlatformOnlyMobile()
        {
            if(IsIphone())
                return "ios";
            else
                return "android";
        }

        public static string GetStrPlatformIgnoreEditor()
        {
            if (IsIphone())
                return "ios";
            else if (IsAndroid())
                return "android";
            else if (IsWindows())
                return "pc";
            return "pc";
        }

        public static bool IsEditor()
        {
            return intPlatform == (int)RuntimePlatform.WindowsEditor || intPlatform == (int)RuntimePlatform.OSXEditor;
        }

        public static bool IsWindowsEditor()
        {
            return intPlatform == (int)RuntimePlatform.WindowsEditor;
        }
        public static bool IsIphone()
        {
            return intPlatform == (int)RuntimePlatform.IPhonePlayer;
        }

        public static bool IsAndroid()
        {
            return intPlatform == (int)RuntimePlatform.Android;
        }

        public static bool IsWindows()
        {
            return intPlatform == (int)RuntimePlatform.WindowsPlayer;
        }

        public static bool IsMobile()
        {
            return IsAndroid() || IsIphone();
        }

        public static string GetAppChannel()
        {
            if (IsAndroid()) return "googleplay";
            else if (IsIphone()) return "applestore";
            else if (IsWindows()) return "pc";
            return "";
        }
    }
}
