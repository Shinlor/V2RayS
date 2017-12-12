using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace V2RayS
{
    class V2RaySconfig
    {

        //初始化
        string FILE_PATH;
        XmlDocument configfile = new XmlDocument();


        public int ReadServerNumber()
        {
            XmlNode servers = this.configfile.DocumentElement.SelectSingleNode("Servers");//当前选定节点为servers
            int i = servers.ChildNodes.Count;//获取server数量，即servers下的节点数
            return i;

        }
        public void RemoveServer(int i)
        {
            XmlNode servers = this.configfile.DocumentElement.SelectSingleNode("Servers");
            string servernum = "Server" + "[" + i + "]";
            XmlNode server = servers.SelectSingleNode(servernum);//选择指定的server
            servers.RemoveChild(server);

        }
        public void ClearServer()
        {
            XmlNode servers = this.configfile.DocumentElement.SelectSingleNode("Servers");
            servers.RemoveAll();
            configfile.Save(FILE_PATH);
        }
        public void ReadServer(out string IP, out string Port, out string UserId, out string AlterID, out string Security, int i)
        {
            XmlNode servers = this.configfile.DocumentElement.SelectSingleNode("Servers");
            int num = servers.ChildNodes.Count;
            if (i > num)
            {
                Console.WriteLine("没有这个server，返回空值");
                IP = "";
                Port = "";
                UserId = "";
                AlterID = "";
                Security = "";
            }
            else
            {
                string servernum = "Server" + "[" + i + "]";
                XmlNode server = servers.SelectSingleNode(servernum);//选择指定的server
                XmlNode serverIP = server.SelectSingleNode("IP");
                IP = serverIP.InnerText;
                XmlNode serverPort = server.SelectSingleNode("Port");
                Port = serverPort.InnerText;
                XmlNode serverUserId = server.SelectSingleNode("UserID");
                UserId = serverUserId.InnerText;
                XmlNode serverAlterID = server.SelectSingleNode("AlterID");
                AlterID = serverAlterID.InnerText;
                XmlNode serverSecurity = server.SelectSingleNode("Security");
                Security = serverSecurity.InnerText;
            }


        }
        public void WriteServer(string IP, string Port, string UserId, string AlterID, string Security, int i)
        {
            XmlNode servers = this.configfile.DocumentElement.SelectSingleNode("Servers");
            int num = servers.ChildNodes.Count;
            if (i > num)
            {
                Console.WriteLine("没有这个server，新建后排序为1");
                XmlElement server = configfile.CreateElement("Server");
                XmlElement serverIP = configfile.CreateElement("IP");
                serverIP.InnerText = IP;
                server.AppendChild(serverIP);
                XmlElement serverPort = configfile.CreateElement("Port");
                serverPort.InnerText = Port;
                server.AppendChild(serverPort);
                XmlElement serverUserId = configfile.CreateElement("UserID");
                serverUserId.InnerText = UserId;
                server.AppendChild(serverUserId);
                XmlElement serverAlterID = configfile.CreateElement("AlterID");
                serverAlterID.InnerText = AlterID;
                server.AppendChild(serverAlterID);
                XmlElement serverSecurity = configfile.CreateElement("Security");
                serverSecurity.InnerText = Security;
                server.AppendChild(serverSecurity);
                servers.AppendChild(server);
            }
            else
            {
                string servernum = "Server" + "[" + i + "]";
                //Console.WriteLine(servernum);
                XmlNode server = servers.SelectSingleNode(servernum);
                XmlNode serverIP = server.SelectSingleNode("IP");
                serverIP.InnerText = IP;
                XmlNode serverPort = server.SelectSingleNode("Port");
                serverPort.InnerText = Port;
                XmlNode serverUserId = server.SelectSingleNode("UserID");
                serverUserId.InnerText = UserId;
                XmlNode serverAlterID = server.SelectSingleNode("AlterID");
                serverAlterID.InnerText = AlterID;
                XmlNode serverSecurity = server.SelectSingleNode("Security");
                serverSecurity.InnerText = Security;

            }

            configfile.Save(FILE_PATH);

        }

        public void CreatFile()
        {
            XmlElement config = configfile.CreateElement("Config");//添加根元素
            config.SetAttribute("version", "1.0");
            config.SetAttribute("name", "V2RayS_ConfigFile");
            configfile.AppendChild(config);
            XmlElement port = configfile.CreateElement("Port");
            port.InnerText = "8080";
            config.AppendChild(port);
            XmlElement proxymode = configfile.CreateElement("ProxyMode");
            proxymode.InnerText = "HTTP";
            config.AppendChild(proxymode);
            XmlElement autostart = configfile.CreateElement("AutoStart");
            autostart.InnerText = "false";
            config.AppendChild(autostart);
            XmlElement proxyrange = configfile.CreateElement("ProxyRange");
            proxyrange.InnerText = "PAC";
            config.AppendChild(proxyrange);
            XmlElement enable = configfile.CreateElement("Enable");
            enable.InnerText = "true";
            config.AppendChild(enable);
            XmlElement servers = configfile.CreateElement("Servers");
            config.AppendChild(servers);
            configfile.Save(FILE_PATH);
        }
        
        //方法，读取配置文件
        public void LoadFile(string FilePath)//读取config文件，否则创建
        {
            FILE_PATH = FilePath;
            try
            {
                // Get the path that stores user documents.
                string myDocumentsPath = System.Environment.CurrentDirectory;
                //XmlDocument configfile = new XmlDocument();
                configfile.Load(FILE_PATH);

            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("文件不存在，按默认重新创建文件" + e.FileName);
                this.CreatFile(); //创建文件
                this.LoadFile(FILE_PATH);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        public string ReadPort()
        {
            return (this.configfile.DocumentElement.SelectSingleNode("Port").InnerText);
        }

        public string ReadProxymode()
        {
            return (this.configfile.DocumentElement.SelectSingleNode("ProxyMode").InnerText);
        }
        public string ReadAutoStart()
        {
            return (this.configfile.DocumentElement.SelectSingleNode("AutoStart").InnerText);
        }
        public string ReadProxyRange()
        {
            return (this.configfile.DocumentElement.SelectSingleNode("ProxyRange").InnerText);
        }
        public string ReadEnable()
        {
            return (this.configfile.DocumentElement.SelectSingleNode("Enable").InnerText);
        }
        public string ReadServers()
        {
            return (this.configfile.DocumentElement.SelectSingleNode("Servers").InnerText);
        }
        public void WritePort(string Port)
        {
            XmlNode temp = this.configfile.DocumentElement.SelectSingleNode("Port");
            temp.InnerText = Port;
            this.WriteFile();
        }
        public void WriteProxymode(string Proxymode)
        {
            XmlNode temp = this.configfile.DocumentElement.SelectSingleNode("ProxyMode");
            temp.InnerText = Proxymode;
            this.WriteFile();
        }
        public void WriteProxyRange(string ProxyRange)
        {
            XmlNode temp = this.configfile.DocumentElement.SelectSingleNode("ProxyRange");
            temp.InnerText = ProxyRange;
            this.WriteFile();
        }
        public void WriteAutoStart(string AutoStart)
        {
            XmlNode temp = this.configfile.DocumentElement.SelectSingleNode("AutoStart");
            temp.InnerText = AutoStart;
            this.WriteFile();
        }
        public void WriteEnable(string Enable)
        {
            XmlNode temp = this.configfile.DocumentElement.SelectSingleNode("Enable");
            temp.InnerText = Enable;
            this.WriteFile();
        }
        public void WriteFile()
        {
            configfile.Save(FILE_PATH);
        }



    }

}
