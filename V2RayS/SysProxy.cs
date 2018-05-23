using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace V2RayS
{
    public class SysProxy
    {       // These are the Win32 error code for file not found or access denied.

        [DllImport("wininet.dll")]
        static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);
        const int INTERNET_OPTION_SETTINGS_CHANGED = 39;
        const int INTERNET_OPTION_REFRESH = 37;
        
        RegistryKey registry = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);
        //https://social.msdn.microsoft.com/Forums/vstudio/en-US/19517edf-8348-438a-a3da-5fbe7a46b61a/how-to-change-global-windows-proxy-using-c-net-with-immediate-effect?forum=csharpgeneral
        public void RefreshIEOption()

        { // They cause the OS to refresh the settings, causing IP to realy update
            bool settingsReturn, refreshReturn;
            settingsReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0);
            refreshReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);            
        }
        public bool IsRegeditKeyExist(RegistryKey RegBoot, string RegKeyName)
        {
            string[] subkeyNames;
            subkeyNames = RegBoot.GetValueNames();
            foreach (string keyName in subkeyNames)
            {

                if (keyName == RegKeyName)  //判断键值的名称
                {
                    return true;
                }
            }
            return false;
        }

        public void SetPacProxy(string proxy)
        {
            bool keyexist = IsRegeditKeyExist(registry, "AutoConfigURL");
            if (keyexist == true)
            {
                registry.SetValue("AutoConfigURL", proxy);
                registry.SetValue("ProxyOverride", "<local>;localhost;127.*;10.*;172.16.*;172.17.*;172.18.*;172.19.*;172.20.*;172.21.*;172.22.*;172.23.*;172.24.*;172.25.*;172.26.*;172.27.*;172.28.*;172.29.*;172.30.*;172.31.*;172.32.*;192.168.*");
                registry.SetValue("ProxyEnable",0);
            }
            if (keyexist==false)
            {
                registry.SetValue("AutoConfigURL", proxy);
                registry.SetValue("ProxyOverride", "<local>;localhost;127.*;10.*;172.16.*;172.17.*;172.18.*;172.19.*;172.20.*;172.21.*;172.22.*;172.23.*;172.24.*;172.25.*;172.26.*;172.27.*;172.28.*;172.29.*;172.30.*;172.31.*;172.32.*;192.168.*");
                registry.SetValue("ProxyEnable", 0);                
            }
            RefreshIEOption();
            return;
        }
        public void SetAllProxy(string proxy)
        {
            bool keyexist = IsRegeditKeyExist(registry, "AutoConfigURL");
            if (keyexist == true)
            {
                registry.DeleteValue("AutoConfigURL");
                registry.SetValue("ProxyOverride", "<local>;localhost;127.*;10.*;172.16.*;172.17.*;172.18.*;172.19.*;172.20.*;172.21.*;172.22.*;172.23.*;172.24.*;172.25.*;172.26.*;172.27.*;172.28.*;172.29.*;172.30.*;172.31.*;172.32.*;192.168.*");
                registry.SetValue("ProxyServer", proxy);
                registry.SetValue("ProxyEnable", 1);
            }            
            if (keyexist == false)
            {
                
                registry.SetValue("ProxyOverride", "<local>;localhost;127.*;10.*;172.16.*;172.17.*;172.18.*;172.19.*;172.20.*;172.21.*;172.22.*;172.23.*;172.24.*;172.25.*;172.26.*;172.27.*;172.28.*;172.29.*;172.30.*;172.31.*;172.32.*;192.168.*");
                registry.SetValue("ProxyServer", proxy);
                registry.SetValue("ProxyEnable", 1);
            }
            RefreshIEOption();
            return;
        }
        public void RemoveProxy()
        {
            bool keyexist = IsRegeditKeyExist(registry, "AutoConfigURL");
            if (keyexist == true)
            {
                registry.DeleteValue("AutoConfigURL");
                registry.SetValue("ProxyOverride", "<local>;localhost;127.*;10.*;172.16.*;172.17.*;172.18.*;172.19.*;172.20.*;172.21.*;172.22.*;172.23.*;172.24.*;172.25.*;172.26.*;172.27.*;172.28.*;172.29.*;172.30.*;172.31.*;172.32.*;192.168.*");
                //registry.SetValue("ProxyServer", proxy);
                registry.SetValue("ProxyEnable", 0);
            }
            if (keyexist == false)
            {

                registry.SetValue("ProxyOverride", "<local>;localhost;127.*;10.*;172.16.*;172.17.*;172.18.*;172.19.*;172.20.*;172.21.*;172.22.*;172.23.*;172.24.*;172.25.*;172.26.*;172.27.*;172.28.*;172.29.*;172.30.*;172.31.*;172.32.*;192.168.*");
                //registry.SetValue("ProxyServer", proxy);
                registry.SetValue("ProxyEnable", 0);
            }
            RefreshIEOption();
            return;
        }
    }
}
