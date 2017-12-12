using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace V2RayS
{
    class PacHttpServer
    {
        HttpListener listener;
        Thread listenThread;
        string pacfile = "";
        string lisenip = "";
        public void SetPacFile(string pac)
        {
            pacfile = pac;
        }
        public void SetLisenIP(string lisenipadress)
        {
            lisenip = lisenipadress;
        }
        public bool IsRunning
        {
            get { return (listener == null) ? false : listener.IsListening; }
        }
 
        public void StartListening()
        {
            listener = new HttpListener();
            if (listener != null)
            {
                //MessageBox.Show("litsenner is'nt  null");
                //return;
            }
            
            listener.Prefixes.Add("http://" + lisenip + "/");
            Console.WriteLine("当前PAC文件：" + pacfile);
            try
            {
                listener.Start(); //开始监听端口，接收客户端请求  
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }            
            listenThread = new Thread(AcceptClient);
            listenThread.Name = "httpserver";
            listenThread.Start();
            Console.WriteLine("开启服务：" + lisenip);
        }//开启端口并监听  
        public void StopListening()
        {
            if (listener != null)
            {
                listenThread.Abort();
                listener.Stop();
               
            }
        }
        void AcceptClient()
        {
            while (listener.IsListening)//while可以使得程序保持不退出
            {
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request; //客户端发送过来的消息  
                Console.Write("Headers:");
                Console.Write(request.Headers);
                Console.WriteLine("URL:");
                Console.WriteLine(request.Url);
                Console.WriteLine("InputStream:");
                Console.WriteLine(request.InputStream);

                //调用返回信息
                try
                {
                    new Thread(HandleRequest).Start(context);
                }
                catch
                {
                }
            }
        }
        void HandleRequest(object ctx)
        {
            HttpListenerContext context = ctx as HttpListenerContext;
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;
            //向客户端传输请求的文件  
            
            try
            {
                FileStream stream = File.OpenRead(pacfile);
                response.StatusCode = 200;
                response.ContentLength64 = stream.Length;
                response.ContentType = "application/x-ns-proxy-autoconfig";//这里是IE能否识别的关键
                int byteLength = (int)stream.Length;
                byte[] fileBytes = new byte[byteLength];
                stream.Read(fileBytes, 0, byteLength);
                stream.Close();
                stream.Dispose();
                response.ContentLength64 = byteLength;
                response.OutputStream.Write(fileBytes, 0, byteLength);
                response.OutputStream.Close();
            }
            catch
            {
                MessageBox.Show("PacHTTPServer:传输PAC文件出现异常");
            }
        }
    }
}
