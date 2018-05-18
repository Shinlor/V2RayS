using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;

namespace V2RayS
{
    /// <summary>
    /// EditPacFile.xaml 的交互逻辑
    /// </summary>
    public partial class WindowEditPacFile : Window
    {
        EditPac pac = new EditPac();
        string pacfile;
        public WindowEditPacFile()
        {
            InitializeComponent();
            PACListBox.ItemsSource = pac.PAClist;
            string FullPath = System.Windows.Forms.Application.ExecutablePath;
            pacfile = System.IO.Path.GetDirectoryName(FullPath)+@"\pac.txt";
            pac.ReadPACList(pacfile);
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            pac.AddDomain(DomainTextBox.Text);

            PACListBox.SelectedItem = DomainTextBox.Text;
            PACListBox.ScrollIntoView(PACListBox.SelectedItem);//滚动到选择位置            
        }
        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            int i = PACListBox.SelectedIndex;
            if (i == -1)
            {
                MessageBox.Show("不存在");
                return;
            }
            else
            {
                pac.RemoveDomain(i); //PACListBox.Items.Remove(PACListBox.SelectedItem); 
            }
            PACListBox.SelectedIndex = i-1;

        }
        private void PACListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PACListBox.SelectedIndex == -1)
            {
                DomainTextBox.Text = "";
            }
            else
            {
                DomainTextBox.Text = PACListBox.SelectedItem.ToString();
            }

        }
        private void EditdomainButton_Click(object sender, RoutedEventArgs e)
        {
            int i = PACListBox.SelectedIndex;
            if (PACListBox.SelectedIndex != -1)
            {

                pac.EditDomain(PACListBox.SelectedIndex, DomainTextBox.Text);
                PACListBox.SelectedIndex = i;
            }
            else
            {
                MessageBox.Show("没有选择保存Domain到哪一个位置");
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            pac.SavePac(pacfile);

        }
    }
}
