using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace V2RayS
{
    class AutoStart
    {
        public void SetStart(bool Started, string name, string path)
        {
            RegistryKey AutoStart = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (Started == true)
            {
                try
                {
                    AutoStart.SetValue(name, path);
                    AutoStart.Close();
                }
                catch
                {
                    MessageBox.Show("增加启动项，修改注册表出现异常！");
                }
            }
            else
            {
                try
                {
                    AutoStart.DeleteValue(name);
                    AutoStart.Close();
                }
                catch
                {
                    MessageBox.Show("删除启动项，注册表出现异常！");
                }
            }
        }
    }
}
