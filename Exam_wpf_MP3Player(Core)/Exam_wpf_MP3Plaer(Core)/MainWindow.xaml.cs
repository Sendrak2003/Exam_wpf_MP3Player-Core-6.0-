using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.WebRequestMethods;
using Clock = System.Windows.Media.Animation.Clock;
using File = System.IO.File;
using Path = System.IO.Path;

namespace Exam_wpf_MP3Plaer_Core_
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool WwndowState = false;
        bool IsToggle = false;
        bool randomSwapSongs=false;
        bool repeatSongs = false;
        int countClickRepeatSongs=0;
        int countClickRandomSwapSongs=0;
        int index = 0;
        bool playNow = false;
        string[] arr_song = null;
        List<string> songs = null;
        List<string> Sourcesongs = null;
        Storyboard storyboard = null;
        MediaTimeline mediaTimeline = null;
        private object dummyNode = null;
        string folderPlayList = Directory.GetCurrentDirectory() + "\\PlayList";
        public string SelectedImagePath { get; set; }
        FileInfo[] files = null;
        PackIcon PlayIcon = null;
        PackIcon StopIcon = null;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Close_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

  
        private void Button_Scale_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!WwndowState)
            {
                this.WindowState = WindowState.Maximized;
                PlayList.Height = 540;
                WwndowState = true;
            }
            else
            {
                this.WindowState = WindowState.Normal;
                PlayList.Height = 190;
                WwndowState = false;
            }
        }

        private void ButtonСollapse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

       

        private void OpenButtonMenu_PreviewMouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            IsToggle = true;
           
            LabelMenu.Visibility = Visibility.Visible;
            OpenButtonMenu.Visibility = Visibility.Collapsed;
            CloseButtonMenu.Visibility = Visibility.Visible;
        }

        private void CloseButtonMenu_PreviewMouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            IsToggle = false;
            LabelMenu.Visibility=Visibility.Collapsed;
            OpenButtonMenu.Visibility = Visibility.Visible;
            CloseButtonMenu.Visibility = Visibility.Collapsed;
        }
       
        private void PlayLists_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            LabelMenu.Visibility = Visibility.Collapsed;
            OpenButtonMenu.Visibility = Visibility.Visible;
            CloseButtonMenu.Visibility = Visibility.Collapsed;
            if (IsToggle)
            {
                Storyboard sb = this.FindResource("CloseMenu") as Storyboard;
                Storyboard.SetTarget(sb, this.GridMenu);
                sb.Begin();
                IsToggle = false;
            }
            Player.Visibility = Visibility.Collapsed;
            PagePlayList.Visibility = Visibility.Visible;
         


        }

        private void MainGrid_Loaded(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(folderPlayList))
            {
                Directory.CreateDirectory(folderPlayList);
            }
            PagePlayList.Margin = new Thickness(55, 0, 0, 0);
            Player.Margin = new Thickness(55, 0, 0, 0);
        }

      

        private void TextBlock_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
      

        public void Update(string scanFolder)
        {
            foreach (string s in Directory.GetDirectories(scanFolder))
            {
                TreeViewItem item = new TreeViewItem();
                item.Header = s;
                item.Tag = s;
                item.FontWeight = FontWeights.Normal;
                item.Items.Add(dummyNode);
                item.Expanded += new RoutedEventHandler(folder_Expanded);
                fileName.Text = Path.GetDirectoryName(s);
                foldersItem.Items.Add(item);
            }
        }
        void folder_Expanded(object sender, RoutedEventArgs e)
        {
            if (SelectedImagePath != folderPlayList)
            {
                AddPlayListButton.Visibility = Visibility.Collapsed;
                AddSongButton.Visibility = Visibility.Visible;
            }
            TreeViewItem item = (TreeViewItem)sender;
            if (item.Items.Count == 1 && item.Items[0] == dummyNode)
            {

                item.Items.Clear();
                try
                {
                    foreach (string s in Directory.GetFiles(item.Tag.ToString()))
                    {
                        TreeViewItem subitem = new TreeViewItem();
                        subitem.Header = System.IO.Path.GetFileNameWithoutExtension(s);
                        subitem.Tag = s;
                        subitem.FontWeight = FontWeights.Normal;
                        subitem.Items.Add(dummyNode);
                        subitem.Expanded += new RoutedEventHandler(folder_Expanded);
                        item.Items.Add(subitem);
                        fileName.Text = Path.GetFileName(s);
                    }
                }
                catch (Exception) { }
            }
        }

        private void foldersItem_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

            TreeView tree = (TreeView)sender;
            TreeViewItem temp = ((TreeViewItem)tree.SelectedItem);
            tree.PreviewMouseRightButtonDown += temp_PreviewMouseRightButtonDown;
            if (temp == null)
                return;
            SelectedImagePath = "";
            string temp1 = "";
            string temp2 = "";
            while (true)
            {
                temp1 = temp.Header.ToString();

                SelectedImagePath = temp1 + temp2 + SelectedImagePath;
                if (temp.Parent.GetType().Equals(typeof(TreeView)))
                {
                    break;
                }
                temp = ((TreeViewItem)temp.Parent);
                temp2 = @"\";

            }

            //show user selected path
            // MessageBox.Show(SelectedImagePath);
        }


        private void Maib_Loaded(object sender, RoutedEventArgs e)
        {
            foldersItem.Items.Clear();
            Update(folderPlayList);

        }


        private void temp_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeView tree = (TreeView)sender;
            TreeViewItem temp = ((TreeViewItem)tree.SelectedItem);
            ContextMenu contextMenu = new ContextMenu();
            MenuItem delete = new MenuItem();
            delete.Header = "Удвлить";
            delete.PreviewMouseLeftButtonDown += Delete_PreviewMouseLeftButtonDown;
            contextMenu.Items.Add(delete);
            MenuItem PlayFolder = new MenuItem();
            PlayFolder.Header = "Воспроизвести";
            PlayFolder.PreviewMouseLeftButtonDown += PlayFolder_PreviewMouseLeftButtonDown;
            contextMenu.Items.Add(PlayFolder);
            temp.ContextMenu = contextMenu;
         
        }

        private void PlayFolder_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            TreeViewItem temp = ((TreeViewItem)foldersItem.SelectedItem);
            if (Directory.Exists(temp.Header.ToString()))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(temp.Header.ToString());
                index=0;
                if (songs.Count > 0)
                {
                    songs.Clear();
                    Sourcesongs.Clear();
                    PlayList.Items.Clear();
                }
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
                catch (UnauthorizedAccessException e1)
                {
                    MessageBox.Show(e1.Message);
                }
                /*
                    DirectoryNotFoundException — исключение возникает, 
                    когда заданная папка не найдена по указанному пути.  
                */
                catch (DirectoryNotFoundException e2)
                {
                    MessageBox.Show(e2.Message);
                }
                if (files != null)
                {
                    foreach (FileInfo fi in files)
                    {
                        string ext = System.IO.Path.GetExtension(fi.FullName);
                        if (ext == ".mp3" || ext == ".wav" || ext == ".wmv" || ext == ".mpeg" || ext == ".wmv")
                        {
                            songs.Add(fi.FullName);
                            Sourcesongs.Add(fi.FullName);
                            PlayList.Items.Add(System.IO.Path.GetFileNameWithoutExtension(fi.FullName));
                        }

                    }

                    if (randomSwapSongs)
                    {
                        RandomSwapSongs();
                    }
                    if(index < songs.Count)
                    {
                        Start(songs[index]);
                    }
                   

                }

            }
            else
            {
                string ext = System.IO.Path.GetExtension(temp.Tag.ToString());
                if (ext == ".mp3" || ext == ".wav" || ext == ".wmv" || ext == ".mpeg" || ext == ".wmv")
                {
                    songs.Add(temp.Tag.ToString());
                    Sourcesongs.Add(temp.Tag.ToString());
                    PlayList.Items.Add(System.IO.Path.GetFileNameWithoutExtension(temp.Tag.ToString()));
                    index = songs.Count - 1;
                    if (randomSwapSongs)
                    {
                        RandomSwapSongs();
                    }
                    Start(songs[index]);
                }

            }
            e.Handled = true;
           
        }

        private void Delete_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            bool isDir = System.IO.Directory.Exists(SelectedImagePath);

            if (isDir)
            {
                Directory.Delete(SelectedImagePath, true);
                SelectedImagePath = folderPlayList;

                foldersItem.Items.Clear();

                Update(SelectedImagePath);
                if (SelectedImagePath == folderPlayList)
                {
                    AddPlayListButton.Visibility = Visibility.Visible;
                    AddSongButton.Visibility = Visibility.Collapsed;
                }
            }
            else
            {

                File.Delete(SelectedImagePath);
                foldersItem.Items.Clear();
                Update(folderPlayList);

            }
          
        }
        private void AddPlayListButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CreatePlatList createPlatList = new CreatePlatList();


            createPlatList.ShowDialog();
            if (createPlatList.update == true)
            {
                foldersItem.Items.Clear();
                Update(folderPlayList);
            }
           

        }




     

        private void AddSongButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".mp3",
                Multiselect = true,
                Filter = "All Supported File Types(*.mp3, *.wav, *.mpeg, *.wmv, *.avi) | *.mp3; *.wav; *.mpeg; *.wmv; *.avi",
                CheckFileExists = true
            };

            if (dlg.ShowDialog() == true)
            {

                foreach (String file in dlg.FileNames)
                {
                    string oath = SelectedImagePath + "\\" + System.IO.Path.GetFileName(file);
                    if (!File.Exists(oath))
                    {
                        File.Copy(file, oath);
                    }
                    else
                    {
                        continue;
                    }

                }
                foldersItem.Items.Clear();
                AddSongButton.Visibility = Visibility.Collapsed;
                AddPlayListButton.Visibility = Visibility.Visible;
                Update(Path.GetDirectoryName(SelectedImagePath));
                //Play(songs[index]);
            }
           
        }

        private void backButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            foldersItem.Items.Clear();
            SelectedImagePath = folderPlayList;
            if (SelectedImagePath == folderPlayList)
            {
                AddPlayListButton.Visibility = Visibility.Visible;
                AddSongButton.Visibility = Visibility.Collapsed;
            }
            Update(SelectedImagePath);
           
        }



        private void Button_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            StView.Orientation = Orientation.Horizontal;
            e.Handled = true;
        }
       
        void Start(string song)
        {
            playNow = true;
            Play.Content = StopIcon;
            mediaTimeline.Source = new Uri(song);
            NameSong.Content = System.IO.Path.GetFileNameWithoutExtension(song);
            storyboard.Begin(Audio, true);
            PlayList.SelectedIndex = index;
            if (nextSong.IsEnabled == false)
            {
                nextSong.IsEnabled = true;
            }
            if (prevSong.IsEnabled == false)
            {
                prevSong.IsEnabled = true;
            }
            if (Play.IsEnabled == false)
            {
                Play.IsEnabled = true;
            }
            if (RepeatSong.IsEnabled == false)
            {
                RepeatSong.IsEnabled = true;
            }
            if (ShuffleVariant.IsEnabled == false)
            {
                ShuffleVariant.IsEnabled = true;
            }

        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            if (!playNow)
            {
                Play.Content = StopIcon;
                storyboard.Resume(Audio);
                playNow=true;
            }
            else
            {
                Play.Content = PlayIcon;
                storyboard.Pause(Audio);
                playNow = false;
            }
            
        }
       
     
        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            // BasedOn="{StaticResource MaterialDesignFloatingActionMiniButton}"
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".mp3",
                Multiselect = true,
                Filter = "All Supported File Types(*.mp3, *.wav, *.mpeg, *.wmv, *.avi) | *.mp3; *.wav; *.mpeg; *.wmv; *.avi",
                CheckFileExists = true
            };

            if (dlg.ShowDialog(this) == true)
            {
                foreach (String file in dlg.FileNames)
                {
                    songs.Add(file);
                    Sourcesongs.Add(file);
                    PlayList.Items.Add(System.IO.Path.GetFileNameWithoutExtension(file));
                }
                if (!playNow)
                {
                    if (randomSwapSongs)
                    {
                        RandomSwapSongs();
                    }
                    Start(songs[index]);
                }



            }
        }

        private void Video_MediaOpened(object sender, RoutedEventArgs e)
        {
            TimerSlider.Maximum = Audio.NaturalDuration.TimeSpan.TotalSeconds;
        }


        private bool go_player;

        private void Storyboard_CurrentTimeInvalidated(object sender, EventArgs e)
        {

            //Получаем значение времени для раскадровки
            Clock storyboardClock = (Clock)sender;

            if (storyboardClock.CurrentProgress == null)
            {
                lblTime.Text = "";

            }
            else
            {
                lblTime.Text = storyboardClock.CurrentTime.ToString();
                go_player = true;
                TimerSlider.Value = storyboardClock.CurrentTime.Value.TotalSeconds;
                go_player = false;
            }
        }

        private void TimerSlider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            storyboard.Pause(Audio);
        }

        private void TimerSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            storyboard.Seek(Audio, TimeSpan.FromSeconds(TimerSlider.Value), TimeSeekOrigin.BeginTime);
            storyboard.Resume(Audio);
        }



        private void TimerSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!go_player)
            {
                storyboard.Seek(Audio, TimeSpan.FromSeconds(TimerSlider.Value), TimeSeekOrigin.BeginTime);
             
            }
          
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {

            songs = new List<string>();
            Sourcesongs=new List<string>();
            mediaTimeline = new MediaTimeline();
            storyboard = new Storyboard();
            storyboard.CurrentTimeInvalidated += Storyboard_CurrentTimeInvalidated;
            storyboard.Children.Add(mediaTimeline);
            Balance.Value = Balance.Maximum / 2;
            Volume.Value = Volume.Maximum / 2;
            Play.IsEnabled = false;
            nextSong.IsEnabled = false;
            prevSong.IsEnabled = false;
            RepeatSong.IsEnabled = false;
            ShuffleVariant.IsEnabled = false;
            PlayIcon= new PackIcon();
            PlayIcon.Kind = PackIconKind.Play;
            StopIcon = new PackIcon();
            StopIcon.Kind = PackIconKind.Stop;

        }



        private void Audio_MediaEnded_1(object sender, RoutedEventArgs e)
        {
            if (repeatSongs)
            {
                Start(songs[index]);
            }
            else
            {
                index++;
                if (index < songs.Count)
                {

                    storyboard.Resume(Audio);
                    mediaTimeline.Source = null;
                    Start(songs[index]);
                }
                else
                {
                    playNow = false;
                    storyboard.Stop(Audio);
                    mediaTimeline.Source = null;
                    nextSong.IsEnabled = false;
                    prevSong.IsEnabled = true;

                }
            }
        }
       


        private void Grid_PreviewDragEnter(object sender, DragEventArgs e)
        {
            arr_song = (string[])e.Data.GetData(DataFormats.FileDrop);
            e.Handled = true;
        }

        private void Grid_PreviewDrop(object sender, DragEventArgs e)
        {
            foreach (var item in arr_song)
            {
                string ext = System.IO.Path.GetExtension(item);
                if (ext == ".mp3" || ext == ".wav" || ext == ".wmv" || ext == ".mpeg" || ext == ".wmv")
                {
                    songs.Add(item);
                    Sourcesongs.Add(item);
                    PlayList.Items.Add(System.IO.Path.GetFileNameWithoutExtension(item));
                }
            }
            if (!playNow)
            {
                if (randomSwapSongs)
                {
                    RandomSwapSongs();
                }
                index = 0;
                Start(songs[index]);
            }
            e.Handled = true;
        }


        private void PlayList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }



        private void PlayList_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var item = ItemsControl.ContainerFromElement(PlayList, e.OriginalSource as DependencyObject) as ListBoxItem;
            if (item != null)
            {
                index = PlayList.ItemContainerGenerator.IndexFromContainer(item);
                if (PlayList.SelectedIndex != index)
                {
                    storyboard.Stop(Audio);
                    Start(songs[index]);
                }
            }

            e.Handled = true;
        }



        private void BtnVolumeOFF_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Volume.Value = Volume.Minimum;
            BtnVolumeOFF.Visibility = Visibility.Collapsed;
            BtnVolumeON.Visibility = Visibility.Visible;
            e.Handled = true;
        }

        private void BalanceResume_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Balance.Value = Volume.Maximum / 2;
            e.Handled = true;
        }

        private void BtnVolumeON_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Volume.Value = Volume.Maximum / 2;
            BtnVolumeOFF.Visibility = Visibility.Visible;
            BtnVolumeON.Visibility = Visibility.Collapsed;
            e.Handled = true;
        }

        private void ScanDir_Click(object sender, RoutedEventArgs e)
        {
            Scan_Dir scan_Dir = new Scan_Dir();
            scan_Dir.ShowDialog();
            if (scan_Dir.found_songs.Count > 0)
            {
                //index = 0;
                //if(songs.Count > 0)
                //{
                //    PlayList.Items.Clear();
                //    songs.Clear();
                //}
                foreach (var item in scan_Dir.found_songs)
                {
                   
                    songs.Add(item);
                    Sourcesongs.Add(item);
                    PlayList.Items.Add(System.IO.Path.GetFileNameWithoutExtension(item));
                    
                }
                if (!playNow)
                {
                    index = 0;
                    Start(songs[index]);
                }
            }
        }

        private void Player1_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            LabelMenu.Visibility = Visibility.Collapsed;
            OpenButtonMenu.Visibility = Visibility.Visible;
            CloseButtonMenu.Visibility = Visibility.Collapsed;
            if (IsToggle)
            {
                Storyboard sb = this.FindResource("CloseMenu") as Storyboard;
                Storyboard.SetTarget(sb, this.GridMenu);
                sb.Begin();
                IsToggle = false;
            }
            Player.Visibility = Visibility.Visible;
            PagePlayList.Visibility = Visibility.Collapsed;
        }

        private void Volume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Volume.Value == 0)
            {
                BtnVolumeOFF.Visibility = Visibility.Collapsed;
                BtnVolumeON.Visibility = Visibility.Visible;
            }
            else
            {
                BtnVolumeOFF.Visibility = Visibility.Visible;
                BtnVolumeON.Visibility = Visibility.Collapsed;
            }
        }
        void RandomSwapSongs()
        {
            var rand = new Random();
            for (int i = songs.Count - 1; i > 0; i--)
            {
                int j = rand.Next(i);
                var t = songs[i];
                songs[i] = songs[j];
                songs[j] = t;
            }
            PlayList.Items.Clear();
            foreach (var s in songs)
            {
                PlayList.Items.Add(System.IO.Path.GetFileNameWithoutExtension(s));
            }
        }
       

        private void RepeatSong_Click(object sender, RoutedEventArgs e)
        {
            countClickRepeatSongs++;
            if (countClickRepeatSongs % 2 != 0)
            {
                 RepeatSongIcon.Foreground=System.Windows.Media.Brushes.Red;
                 repeatSongs = true;
            }
            else
            {
                RepeatSongIcon.Foreground = System.Windows.Media.Brushes.White;
                repeatSongs = false;
            }
        }

        private void ShuffleVariant_Click(object sender, RoutedEventArgs e)
        {
            index=0;
            countClickRandomSwapSongs++;
            if (countClickRandomSwapSongs % 2 != 0)
            {
                ShuffleVariantIcon.Foreground=System.Windows.Media.Brushes.Red;
                randomSwapSongs = true;
                RandomSwapSongs();
            }
            else
            {
                ShuffleVariantIcon.Foreground = System.Windows.Media.Brushes.White;
                randomSwapSongs = false;
                songs.Clear();
                PlayList.Items.Clear();
                foreach (var item in Sourcesongs)
                {
                    songs.Add(item);
                    PlayList.Items.Add(System.IO.Path.GetFileNameWithoutExtension(item));
                }

            }
        }

        private void nextSong_Click(object sender, RoutedEventArgs e)
        {
            index++;


            if (index < songs.Count)
            {

                // Play(PlayList.Items[index].ToString());
                Start(songs[index]);
            }
            else
            {
                playNow = false;
                TimerSlider.Value = 0;
                storyboard.Stop(Audio);
                Play.Content = PlayIcon;
                nextSong.IsEnabled = false;
                prevSong.IsEnabled = true;
                index--;
            }
        }

        private void prevSong_Click(object sender, RoutedEventArgs e)
        {
           
            if (index > 0)
            {
                index--;
                Start(songs[index]);
            }
            else
            {
                playNow = false;
                TimerSlider.Value = 0;
                storyboard.Pause(Audio);
                Play.Content = PlayIcon;
                index=0;
                prevSong.IsEnabled = false;
                nextSong.IsEnabled = true;

            }
        }
      
        private void Audio_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            //MessageBox.Show((string)e);
        }

        private void Button_PreviewMouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            StView.Orientation = Orientation.Vertical;
            e.Handled = true;
        }
    }
    


}

