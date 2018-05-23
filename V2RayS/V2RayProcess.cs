using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V2RayS
{
    public class V2RayProcess
    {       // These are the Win32 error code for file not found or access denied.
        const int ERROR_FILE_NOT_FOUND = 2;
        const int ERROR_ACCESS_DENIED = 5;
        public event OutputDataReceivedEventHandler OutputDataReceivedEvent;
        string FILE_PATH;
        Process myProcess = new Process();
        public void RunV2ray(string FilePath)
        {
            FILE_PATH = FilePath;
            try
            {
                // Get the path that stores user documents.
                string myDocumentsPath = System.Environment.CurrentDirectory;

                myProcess.StartInfo.FileName = FILE_PATH;
                myProcess.StartInfo.UseShellExecute = false;
                myProcess.StartInfo.RedirectStandardInput = true;
                myProcess.StartInfo.CreateNoWindow = true;
                myProcess.StartInfo.RedirectStandardOutput = true;
                myProcess.StartInfo.RedirectStandardError = true;
                myProcess.OutputDataReceived += new DataReceivedEventHandler(OutputDataReceived);
                myProcess.Start();
                myProcess.BeginOutputReadLine();
            }
            catch (Win32Exception e)
            {
                if (e.NativeErrorCode == ERROR_FILE_NOT_FOUND)
                {
                    System.Windows.MessageBox.Show("未找到V2Ray.exe文件！");
                }

                else if (e.NativeErrorCode == ERROR_ACCESS_DENIED)
                {
                    // Note that if your word processor might generate exceptions
                    // such as this, which are handled first.
                    Console.WriteLine(e.Message +
                        ". You do not have permission.");
                }
            }
        }

        void OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Data))
            {
                if (e.Data.IndexOf("failed to read config file") > -1)
                { System.Windows.MessageBox.Show("V2Ray.json配置文件错误"); }
                OutputDataReceivedEvent(e.Data);
            }
        }
        public void ReloadV2ray()
        {
            try
            {
                myProcess.Kill();
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);

            }
            finally
            {
                myProcess.CancelOutputRead();
                myProcess.Start();
                myProcess.BeginOutputReadLine();
            }

        }
        public void KillV2ray()
        {
            try
            {
                myProcess.Kill();
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
            finally
            {
                myProcess.CancelOutputRead();
            }
        }

    }
}
