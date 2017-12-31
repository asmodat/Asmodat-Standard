using Microsoft.Win32;

namespace AsmodatStandard.IO
{
    public class WindowsRegistry
    {
        static RegistryKey RegBaseKey = Registry.LocalMachine;
        public readonly string RegSubKey = "SOFTWARE\\" + typeof(WindowsRegistry).Namespace;

        public WindowsRegistry(string mainFolder = "SOFTWARE\\")
        {
            if(mainFolder != null)
                RegSubKey = mainFolder;
        }

        public void AddKey(string sKeyName, string sValue) 
            => RegBaseKey.CreateSubKey(RegSubKey).SetValue(sKeyName, sValue);

        public string GetKey(string sKeyName)
        {
            RegistryKey RKSub = RegBaseKey.CreateSubKey(RegSubKey);
            return (RKSub == null) ? null : (string)RKSub.GetValue(sKeyName);
        }

        public bool RemoveKey(string sKeyName, bool bShowException = true)
        {
            RegistryKey RKSub = RegBaseKey.CreateSubKey(RegSubKey);

            if (RKSub == null)
                return false;

            RKSub.DeleteValue(sKeyName);
            return true;
        }

        /*public static bool LaunchAtStartup(bool removeAppFromRegistry = false, string appName = null, string patchExe = null)
        {
            if (appName == null)
                appName = Application.ProductName;
            if (patchExe == null)
                patchExe = Application.ExecutablePath.ToString();

            RegistryKey RKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (!removeAppFromRegistry)
                RKey.SetValue(appName, patchExe);
            else
                RKey.SetValue(appName, false);

            return true;
        }*/
    }
}
