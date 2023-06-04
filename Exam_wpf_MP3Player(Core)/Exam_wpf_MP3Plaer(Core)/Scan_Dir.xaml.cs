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
    /// Логика взаимодействия для Scan_Dir.xaml
    /// </summary>
    public partial class Scan_Dir : Window
    {
        FileInfo[] files = null;
        public List<string> found_songs = null;
        void ScanDir(DirectoryInfo dirInfo)
        {
        
            // Получаем все файлы в текущем каталоге
            try
            {
                files = dirInfo.GetFiles("*.*");
            }
            /*
            UnauthorizedAccessException — исключение возникает, 
            когда у текущего пользователя недостаточно 
            прав для просмотра каталога 
            */
            catch (UnauthorizedAccessException e)
            {
                MessageBox.Show(e.Message);
            }
            /*
                DirectoryNotFoundException — исключение возникает, 
                когда заданная папка не найдена по указанному пути.  
            */
            catch (DirectoryNotFoundException e)
            {
                MessageBox.Show(e.Message);
            }
            if (files != null)
            {
                
                foreach (FileInfo fi in files)
                {
                    string ext = System.IO.Path.GetExtension(fi.FullName);
                    if (ext == ".mp3" || ext == ".wav" || ext == ".wmv" || ext == ".mpeg" || ext == ".wmv")
                    {
                        found_songs.Add(fi.FullName);
                    }
                   

                }
                //получаем все подкаталоги
                DirectoryInfo[] filesAndDirs = dirInfo.GetDirectories("*.*");
                //проходим по каждому подкаталогу
                foreach (DirectoryInfo dir_info in filesAndDirs)
                {
                    ScanDir(dir_info);
                }
            }
        }

        public Scan_Dir()
        {
            InitializeComponent();
        }

       

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            found_songs = new List<string>();
        }

        private void Button_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Directory.Exists(nameFolder.Text))
            {
               
                DirectoryInfo directoryInfo = new DirectoryInfo(nameFolder.Text);
                ScanDir(directoryInfo);
                this.Close();
            }
            else
            {
                MessageBox.Show("Такой папки не существует");
                this.Close();
            }
            e.Handled = true;
        }
    }
}
