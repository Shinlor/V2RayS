using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V2RayS
{
    class ConfigV2RayJson
    {
        string FILE_PATH;
        string Josn;
        dynamic Config;
        public void ReadFile(string FilePath)
        {
            FILE_PATH = FilePath;
            Josn = System.IO.File.ReadAllText(FILE_PATH);
            Config = Newtonsoft.Json.Linq.JObject.Parse(Josn);
            return;
        }
        public void SaveFile()
        {
            string js = Config.ToString();
            System.IO.File.WriteAllText(FILE_PATH, js);
            return;
        }
        public string Readloglevel()
        {
            return Config.log.loglevel;
        }
        public string ReadPort()
        {
            return Config.inbound.port;
        }
        public string Readprotocol()
        {
            return Config.inbound.protocol;
        }
        public string ReadServerAddress()
        {
            return Config.outbound.settings.vnext[0].address;
        }
        public string ReadServerPort()
        {
            return Config.outbound.settings.vnext[0].port;
        }
        public string ReadServerUserID()
        {
            return Config.outbound.settings.vnext[0].users[0].id;
        }
        public string ReadServerAlterID()
        {
            return Config.outbound.settings.vnext[0].users[0].alterId;
        }
        public string ReadServerSecurity()
        {
            return Config.outbound.settings.vnext[0].users[0].security;
        }
        public void Setloglevel(string loglevel)
        {
            Config.log.loglevel = loglevel;
            SaveFile();
        }
        public void SetPort(int port)
        {
            Config.inbound.port = port;
            SaveFile();
        }
        public void Setprotocol(string protocol)
        {
            Config.inbound.protocol = protocol;
            SaveFile();
        }
        public void SetServerAddress(string address)
        {
            Config.outbound.settings.vnext[0].address = address;
            SaveFile();
        }
        public void SetServerPort(int port)
        {
            Config.outbound.settings.vnext[0].port = port;
            SaveFile();
        }
        public void SetServerUserID(string serverid)
        {
            Config.outbound.settings.vnext[0].users[0].id = serverid;
            SaveFile();
        }
        public void SetServeralterId(int alterId)
        {
            Config.outbound.settings.vnext[0].users[0].alterId = alterId;
            SaveFile();
        }
        public void SetServersecurity(string security)
        {
            Config.outbound.settings.vnext[0].users[0].security = security;
            SaveFile();
        }
    }
}
