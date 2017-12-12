using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms; // NotifyIcon control
using System.Drawing; // Icon
using System.Diagnostics;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Net.Sockets;


namespace V2RayS
{

    public delegate void OutputDataReceivedEventHandler(string Data);//声明关于V2Ray日志输出的委托  搜索委托与事件学习

        /// <summary>
        /// MainWindow.xaml 的交互逻辑
        /// </summary>
        /// 

    public partial class MainWindow : Window
    {
        //定义通知栏图标菜单的item
        System.Windows.Forms.NotifyIcon notifyIcon = new System.Windows.Forms.NotifyIcon();
        System.Windows.Forms.ContextMenuStrip V2rayMenuStrip = new System.Windows.Forms.ContextMenuStrip();
        System.Windows.Forms.ToolStripMenuItem v2rayuse = new System.Windows.Forms.ToolStripMenuItem();
        System.Windows.Forms.ToolStripMenuItem proxymode = new System.Windows.Forms.ToolStripMenuItem();
        System.Windows.Forms.ToolStripMenuItem pacitem = new System.Windows.Forms.ToolStripMenuItem();
        System.Windows.Forms.ToolStripMenuItem allproxy = new System.Windows.Forms.ToolStripMenuItem();
        System.Windows.Forms.ToolStripMenuItem config = new System.Windows.Forms.ToolStripMenuItem();
        System.Windows.Forms.ToolStripMenuItem logging = new System.Windows.Forms.ToolStripMenuItem();
        System.Windows.Forms.ToolStripMenuItem reload = new System.Windows.Forms.ToolStripMenuItem();
        System.Windows.Forms.ToolStripMenuItem exit = new System.Windows.Forms.ToolStripMenuItem();
        
        //定义初始变量，各类定义
        WindowLogging Logwindow = new WindowLogging();
        V2RayProcess v2Ray=new V2RayProcess();
        PacHttpServer Pacsrv = new PacHttpServer();
        SysProxy Proxy = new SysProxy();
        V2RaySconfig XMLConfig = new V2RaySconfig();
        ConfigV2RayJson V2rayJson = new ConfigV2RayJson();
        AutoStart AutoStartReg = new AutoStart();
        string IP, Port, UserID, AlterID, Security;
        string ServerIP, ServerPort,ServerUserID,ServerAlterID,ServerSecurity;//用于存储当前使用的Vrayserver的IP及Port等
        int CurrentUsedServerIndex=-1;//用于存储当前使用的server在listBox中的index
        string ProxyIP = @"127.0.0.1";
        string PacPort,ProxyPort;
        string FILE_PATH;//程序的启动路径
        string FILE_FULL_PATH;//包含文件的名的程序全路径


        public MainWindow()
        {
            InitializeComponent();


            

            V2RaySNotify();
            v2Ray.OutputDataReceivedEvent += Writelog;//这是委托，用于调用其它类中的方法，V2Ray发生output事件时调用Writelog
            INIT();
                      
            Pacsrv.SetLisenIP(ProxyIP+@":"+PacPort);
            Pacsrv.SetPacFile(FILE_PATH + @"\pac.txt");
            Pacsrv.StartListening();
            this.v2Ray.RunV2ray(FILE_PATH + @"\V2ray.exe");
        }
        public void INIT()//初始化
        {
            string FullPath = System.Windows.Forms.Application.ExecutablePath;
            FILE_FULL_PATH = System.IO.Path.GetFullPath(FullPath);
            FILE_PATH = System.IO.Path.GetDirectoryName(FullPath);

            XMLConfig.LoadFile(FILE_PATH + @"\V2RaySConfig.xml");//读入V2ryaS配置文件
            V2rayJson.ReadFile(FILE_PATH + @"\config.json");//读入 V2ray的配置文件
            this.PortTextBox.Text = XMLConfig.ReadPort();//读取PAC服务端口，在窗体显示


            //读取Vray本地代理的端口，在窗体显示
            ProxyPort = V2rayJson.ReadPort();
            this.V2RayListenPortTextBox.Text = ProxyPort;

            ServerIP = V2rayJson.ReadServerAddress();
            ServerPort = V2rayJson.ReadServerPort();
            ServerUserID = V2rayJson.ReadServerUserID();
            ServerSecurity = V2rayJson.ReadServerSecurity();
            ServerAlterID = V2rayJson.ReadServerAlterID();            
            PacPort = XMLConfig.ReadPort();//给Port赋值为配置文件中的端口
            //配置文件中是否启用代理
            if (XMLConfig.ReadEnable() == "true")
            {
                v2rayuse.Checked = true;
                this.notifyIcon.Icon = V2RayS.Properties.Resources.V2RayS;
            }
            else if (XMLConfig.ReadEnable() == "false")
            {
                v2rayuse.Checked = false;
                this.notifyIcon.Icon = V2RayS.Properties.Resources.V2RaySDisable;
            }
            //若是启用代理，读取配置文件判断是PAC还是全局
            if (XMLConfig.ReadEnable() == "true")
            {
                if (XMLConfig.ReadProxyRange() == "PAC")
                {
                    this.pacitem.Checked = true;
                    this.allproxy.Checked = false;
                    Proxy.SetPacProxy(@"http://" + ProxyIP + @":" + PacPort + @"/");
                }
                else if (XMLConfig.ReadProxyRange() == "ALL")
                {
                    this.pacitem.Checked = false;
                    this.allproxy.Checked = true;
                    Proxy.SetAllProxy(ProxyIP + @":" + ProxyPort);
                }
            }
            if (V2rayJson.Readloglevel()== "debug")
            {
                ComboBoxLogLevel.SelectedIndex = 0;
            }
            if (V2rayJson.Readloglevel() == "info")
            {
                ComboBoxLogLevel.SelectedIndex = 1;
            }
            if (V2rayJson.Readloglevel() == "warning")
            {
                ComboBoxLogLevel.SelectedIndex = 2;
            }
            if (V2rayJson.Readloglevel() == "error")
            {
                ComboBoxLogLevel.SelectedIndex = 3;
            }
            if (V2rayJson.Readloglevel() == "none")
            {
                ComboBoxLogLevel.SelectedIndex = 4;
            }
            if (XMLConfig.ReadAutoStart()=="true")
            {
                AutoStartCheckBox.IsChecked=true;
                AutoStartReg.SetStart(true, "V2RayS", FILE_FULL_PATH);
                
            }
            else if (XMLConfig.ReadAutoStart() == "false")
            {
                AutoStartCheckBox.IsChecked = false;
                AutoStartReg.SetStart(false, "V2RayS", FILE_FULL_PATH);
            }
            if (XMLConfig.ReadProxymode() == "HTTP")
            {
                ProxyModeComboBox.SelectedIndex = 0;
            }
            else if (XMLConfig.ReadProxymode() == "Socks")
            {
                ProxyModeComboBox.SelectedIndex = 1;
            }
            int servernum = XMLConfig.ReadServerNumber();
            RefreshListBox();
            if (CurrentUsedServerIndex==-1)
            {
                System.Windows.MessageBox.Show("当前程序没有V2Ray正在使用的Server，将自动添加");
                XMLConfig.WriteServer(ServerIP, ServerPort, ServerUserID, ServerAlterID, ServerSecurity, servernum + 1);
                XMLConfig.WriteFile();
                RefreshListBox();
            }


        }
        public void RefreshListBox()
        {
            ServerListBox.Items.Clear();
            int servernum = XMLConfig.ReadServerNumber();
            for (int i = 1; i <= servernum; i++)
            {
                XMLConfig.ReadServer(out IP, out Port, out UserID, out AlterID, out Security, i);
                ServerListBox.Items.Add(IP);
                if (IP == ServerIP && Port == ServerPort && UserID == ServerUserID && AlterID == ServerAlterID && Security == ServerSecurity)
                {
                    CurrentUsedServerIndex = i;
                }
            }
            //System.Windows.MessageBox.Show(CurrentUsedServerIndex.ToString());
            if (CurrentUsedServerIndex > 0)
            {
                ServerListBox.SelectedIndex = CurrentUsedServerIndex - 1;
            }

        }
        private void Writelog(string Data)//根据委托的设置，自动刷新日志窗口
        {
            
            this.Dispatcher.BeginInvoke((Action)delegate ()
            {
                Data = Data + "\r";
             
                Logwindow.V2RayLog.AppendText(Data);
                Logwindow.V2RayLog.ScrollToEnd();//光标自动滚动到最后
                
            });
        }

        //建立通知栏
        public void V2RaySNotify()
        {
            //定义notifyicon
            this.notifyIcon.Text = "V2RayS";
            //this.notifyIcon.Icon = V2RayS.Properties.Resources.V2RayS;
            this.notifyIcon.Visible = true;
            this.notifyIcon.ContextMenuStrip = this.V2rayMenuStrip;
            //this.notifyIcon.ShowBalloonTip(1000);
            notifyIcon.DoubleClick += new EventHandler(OnNotifyIconDoubleClick);//定义双击图标响应函数

            //生成菜单
            this.V2rayMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.v2rayuse,
            this.proxymode,
            this.config,
            this.logging,
            this.reload,
            this.exit,
            });


            //生成二级菜单
            this.proxymode.Name = "proxymode";
            this.proxymode.Text = "代理模式";
            this.proxymode.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pacitem,
            this.allproxy});
            //定义菜单Item
            //启用代理
            this.v2rayuse.Checked = true;
            this.v2rayuse.CheckState = System.Windows.Forms.CheckState.Checked;
            this.v2rayuse.Name = "v2rayuse";
            this.v2rayuse.Text = "启用代理";
            this.v2rayuse.Click += new System.EventHandler(this.v2rayuse_Click);
            //Exit
            this.exit.Name = "Exit";
            this.exit.Text = "退出";
            this.exit.Click += new System.EventHandler(this.Exit_Click);

            //PAC
            this.pacitem.Name = "Pac";
            this.pacitem.Text = "使用PAC";
            this.pacitem.Checked = true;
            this.pacitem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.pacitem.Click += new System.EventHandler(this.Pacitem_Click);
            //proxy all
            this.allproxy.Name = "All";
            this.allproxy.Text = "全部使用代理";
            this.allproxy.Checked = false;
            this.allproxy.CheckState = System.Windows.Forms.CheckState.Unchecked;
            this.allproxy.Click += new System.EventHandler(this.Allproxy_Click);

            //configItem
            this.config.Name = "config";
            this.config.Text = "设置";
            this.config.Click += new System.EventHandler(this.Config_Click);

            //状态
            this.reload.Name = "reload";
            this.reload.Text = "重载V2ray";
            this.reload.Click += new System.EventHandler(this.Reload_Click);

            //状态
            this.logging.Name = "logging";
            this.logging.Text = "状态日志";
            this.logging.Click += new System.EventHandler(this.logging_Click);

        }
        //check启用代理响应
        public void v2rayuse_Click(object sender, EventArgs e)
        {
            if (this.v2rayuse.CheckState == System.Windows.Forms.CheckState.Checked)
            {
                this.notifyIcon.Icon = V2RayS.Properties.Resources.V2RaySDisable;
                this.v2rayuse.Checked = false;
                this.v2rayuse.CheckState = System.Windows.Forms.CheckState.Unchecked;
                Proxy.RemoveProxy();

                XMLConfig.WriteEnable("false");
                XMLConfig.WriteFile();

            }
            else if (this.v2rayuse.CheckState == System.Windows.Forms.CheckState.Unchecked)
            {
                this.v2rayuse.Checked = true;
                this.v2rayuse.CheckState = System.Windows.Forms.CheckState.Checked;
                this.notifyIcon.Icon = V2RayS.Properties.Resources.V2RayS;
                if (XMLConfig.ReadProxyRange() == "PAC")
                {
                    this.pacitem.Checked = true;
                    this.allproxy.Checked = false;
                    Proxy.SetPacProxy(@"http://" + ProxyIP + @":" + PacPort + @"/");
                    //System.Windows.MessageBox.Show(@"http://" + ProxyIP + @":" + PacPort + @"/");

                }
                else if (XMLConfig.ReadProxyRange() == "ALL")
                {
                    this.pacitem.Checked = false;
                    this.allproxy.Checked = true;
                    Proxy.SetAllProxy(ProxyIP + @":" + PacPort);
                }
                XMLConfig.WriteEnable("true");
                XMLConfig.WriteFile();
            }
        }
        public void Pacitem_Click(object sender, EventArgs e)
        {
            if (this.pacitem.CheckState == System.Windows.Forms.CheckState.Checked)
            {
                return;
            }
            else
            {
                this.pacitem.Checked = true;
                this.allproxy.Checked = false;
                Proxy.SetPacProxy(@"http://" + ProxyIP + @":" + PacPort + @"/");
                XMLConfig.WriteProxyRange("PAC");
                XMLConfig.WriteFile();
            }
        }
        public void Allproxy_Click(object sender, EventArgs e)
        {
            if (this.allproxy.CheckState == System.Windows.Forms.CheckState.Checked)
            {
                return;
            }
            else
            {
                this.allproxy.Checked = true;
                this.pacitem.Checked = false;
                Proxy.SetAllProxy(ProxyIP + @":" + ProxyPort);
                XMLConfig.WriteProxyRange("ALL");
                XMLConfig.WriteFile();
            }
        }
        public void logging_Click(object sender, EventArgs e)
        {
            Logwindow.Show();
        }
        public void Config_Click(object sender, EventArgs e)
        {
            windowV2Ray.Show();
        }


        //双击通知栏图标响应
        void OnNotifyIconDoubleClick(object sender, EventArgs e)
        {
            windowV2Ray.Visibility = Visibility.Visible;
        }
        void Exit_Click(object sender, EventArgs e)
        {
            this.v2Ray.KillV2ray();
            this.notifyIcon.Dispose();
            this.Pacsrv.StopListening();
            //this.Pacsrv.KillPacProcess();
            //this.Proxy.KillSysProxy();
   
            System.Windows.Application.Current.Shutdown();
        }
        //关闭按钮后隐藏窗口
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            windowV2Ray.Visibility = Visibility.Hidden;
        }

        public void Reload_Click(object sender, EventArgs e)//重载V2ray的process
        {
            Logwindow.V2RayLog.Text = "Reloaded.."+"\r";
            this.v2Ray.ReloadV2ray();
        }
        

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            V2rayJson.Setloglevel(ComboBoxLogLevel.Text);
            
            ProxyPort = this.V2RayListenPortTextBox.Text;//更新V2ray监听的端口
            V2rayJson.SetPort(int.Parse(ProxyPort));//更新V2ray监听的端口入配置文件

            Logwindow.V2RayLog.Text = "Reloaded.." + "\r";
            this.v2Ray.ReloadV2ray();
            //autostart配置
            if (AutoStartCheckBox.IsChecked==true)
            {
                AutoStartReg.SetStart(true, "V2RayS", FILE_FULL_PATH);
                XMLConfig.WriteAutoStart("true");
            }
            else
            {
                AutoStartReg.SetStart(false, "V2RayS", FILE_FULL_PATH);
                XMLConfig.WriteAutoStart("false");
            }           
        }

        private void ServerListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int i=ServerListBox.SelectedIndex;
            if (i == -1)
            {
                return;
            }
            XMLConfig.ReadServer(out IP, out Port, out UserID, out AlterID, out Security, i+1);
            ServerIPTextBox.Text = IP;
            ServerPortTextBox.Text = Port;
            ServerUserIDTextBox.Text = UserID;
            ServerAlterIDTextBox.Text = AlterID;
            if (Security== "aes-128-cfb")
            {
                ServerSecurityComboBox.SelectedIndex = 0;
            }
            if (Security == "aes-128-gcm")
            {
                ServerSecurityComboBox.SelectedIndex = 1;
            }
            if (Security == "chacha20-poly1305")
            {
                ServerSecurityComboBox.SelectedIndex = 2;
            }
            if (Security == "auto")
            {
                ServerSecurityComboBox.SelectedIndex = 3;
            }
            if (Security == "none")
            {
                ServerSecurityComboBox.SelectedIndex = 4;
            }


        }

        private void UseToV2rayButton_Click(object sender, RoutedEventArgs e)
        {
            V2rayJson.SetServerAddress(this.ServerIPTextBox.Text);
            V2rayJson.SetServerPort(int.Parse(this.ServerPortTextBox.Text));
            V2rayJson.SetServerUserID(this.ServerUserIDTextBox.Text);
            V2rayJson.SetServeralterId(int.Parse(this.ServerAlterIDTextBox.Text));
            V2rayJson.SetServersecurity(this.ServerSecurityComboBox.Text);
            Logwindow.V2RayLog.Text = "";
            this.v2Ray.ReloadV2ray();
            System.Windows.MessageBox.Show("已重新加载V2Ray");

        }

        private void AddServerButton_Click(object sender, RoutedEventArgs e)
        {
            ServerListBox.Items.Add("New Server");
            ServerIPTextBox.Text = "";
            ServerPortTextBox.Text = "";
            ServerUserIDTextBox.Text = "";
            ServerAlterIDTextBox.Text = "";
            int i = ServerListBox.Items.Count;
            ServerListBox.SelectedIndex = i-1;

        }

        private void SaveServerButton_Click(object sender, RoutedEventArgs e)
        {
            int itemnum = ServerListBox.SelectedIndex;
            
            if (ServerSecurityComboBox.SelectedIndex == 0)
                {
                Security = "aes-128-cfb";
                }
            if (ServerSecurityComboBox.SelectedIndex == 1)
            {
                Security = "aes-128-gcm";
            }
            if (ServerSecurityComboBox.SelectedIndex == 2)
            {
                Security = "chacha20-poly1305";
            }
            if (ServerSecurityComboBox.SelectedIndex == 3)
            {
                Security = "auto";
            }
            if (ServerSecurityComboBox.SelectedIndex == 4)
            {
                Security = "none";
            }
            ServerListBox.SelectedIndex = itemnum;
            if (itemnum==-1)
            {
                System.Windows.MessageBox.Show("没有选择保存到哪一个Server");
                return;
            }
            XMLConfig.WriteServer(ServerIPTextBox.Text, ServerPortTextBox.Text, ServerUserIDTextBox.Text, ServerAlterIDTextBox.Text, Security, itemnum + 1);
            XMLConfig.WriteFile();
            ServerListBox.Items.Clear();
            
            int servernum = XMLConfig.ReadServerNumber();
            for (int i = 1; i <= servernum; i++)
            {
                XMLConfig.ReadServer(out IP, out Port, out UserID, out AlterID, out Security, i);
                ServerListBox.Items.Add(IP);
            }
        }

        private void RemoveServerButton_Click(object sender, RoutedEventArgs e)
        {
            int itemnum = ServerListBox.SelectedIndex;
            if (itemnum==-1)
            {
                System.Windows.MessageBox.Show("没有选择删除哪个Server！");
                return;
            }
            if (ServerListBox.SelectedItem.ToString()=="New Server")
            {
                ServerListBox.Items.RemoveAt(itemnum);
                ServerListBox.SelectedIndex = itemnum-1;
                return;
            }
            XMLConfig.RemoveServer(itemnum + 1);
            XMLConfig.WriteFile();
            ServerListBox.Items.Clear();
            ServerListBox.SelectedIndex = itemnum-1;
            int servernum = XMLConfig.ReadServerNumber();
            for (int i = 1; i <= servernum; i++)
            {
                XMLConfig.ReadServer(out IP, out Port, out UserID, out AlterID, out Security, i);
                ServerListBox.Items.Add(IP);
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void ComboBoxItem_Selected_2(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonQuit_Click(object sender, RoutedEventArgs e)
        {
            windowV2Ray.Visibility = Visibility.Hidden;
        }

        private void ComboBoxItem_Selected(object sender, RoutedEventArgs e)
        {

        }

        private void ComboBoxItem_Selected_1(object sender, RoutedEventArgs e)
        {

        }

        private void ShowLogButton_Click(object sender, RoutedEventArgs e)
        {
            Logwindow.Show();
        }
    }

}

