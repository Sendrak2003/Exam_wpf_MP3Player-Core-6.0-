using System;
using System.Collections.Generic;
using System.IO;
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

namespace Exam_wpf_MP3Plaer_Core_
{
    /// <summary>
    /// Логика взаимодействия для CreatePlatList.xaml
    /// </summary>
    
    public partial class CreatePlatList : Window
    {
       public bool update=false;
       public string path = "";
        public CreatePlatList()
        {
            InitializeComponent();
        }
        private void Button_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (namePlaylist.Text != "")
            {
                update=true;
                path = Directory.GetCurrentDirectory() + "\\PlayList" + "\\" + namePlaylist.Text;
                Directory.CreateDirectory(path);

                this.Close();
            }
            else
            {
                MessageBox.Show("Введите название или закройте окно");
            }
        }
    }
}
