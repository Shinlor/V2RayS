using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace V2RayS
{
    class EditPac
    {
        public BindingList<string> PAClist = new BindingList<string>();
        public string beforepac;
        public string afterpac="]"+"\r\n";
        public void ReadPACList(string FilePath)
        {
            string pac = File.ReadAllText(FilePath);
            //var pacsplit = pac.Split(new char[] { '[', ']' });
            string[] pacsplit = Regex.Split(pac, "// rules split", RegexOptions.IgnoreCase);//使用特殊标记的字符串分割
            beforepac = pacsplit[0];
            string domain = pacsplit[1];
            afterpac = pacsplit[2];
            Regex re = new Regex("(?<=\").*?(?=\")", RegexOptions.None);//正则表达式，提取""中的字符串；
            MatchCollection mc = re.Matches(domain);
            int i = 0;
            string[] paclist = new string[mc.Count];
            foreach (Match ma in mc)
            {
                paclist[i] = ma.Value;
                PAClist.Add(ma.Value);
                i++;
                //PACListBox.Items.Add(ma);//ma.Value就是你要的值
            }
        }
        public void AddDomain(string domain)
        {
            if (PAClist.IndexOf(domain) != -1)
            {
                MessageBox.Show("已存在");
                return;
            }
            if (domain == "")
            {
                MessageBox.Show("不可为空");
                return;
            }
            PAClist.Add(domain);

        }
        public void RemoveDomain(int i)
        {
            if (i == -1)
            {
                MessageBox.Show("不存在");
                return;
            }
            PAClist.RemoveAt(i);
        }
        public void SavePac(string FilePath)
        {
            string domains = "";
            foreach (string s in PAClist)
            {
                domains = domains + "\"" + s + "\",\r\n";
            }
            //去除最后的逗号
            domains=domains.Remove(domains.Length - 3, 3);
            string pacstring = beforepac + "// rules split" +"\r\n"+  domains + "\r\n" + "// rules split" + afterpac;
            try
            {
                File.WriteAllText(FilePath, pacstring);
            }
            catch
            {
                MessageBox.Show("写入PAC文件失败");
            }


        }
        public void EditDomain(int i, string pac)
        {
            PAClist[i] = pac;
        }
    }
}
