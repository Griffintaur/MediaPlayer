using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MediaPlayerWindow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private  MediaPlayerViewModel.MediaPlayerMainWindowViewModel _mediaPlayerViewModel=new MediaPlayerViewModel.MediaPlayerMainWindowViewModel();
        private TimeSpan TotalTime;
        private DispatcherTimer timerVideoTime;
        public MainWindow()
        {
            InitializeComponent();
          
            this.DataContext = _mediaPlayerViewModel;
            //_mediaPlayerViewModel.PlayFileButtonObj
          //  this.Timer.Content = timerVideoTime.ToString();
            _mediaPlayerViewModel.PropertyChanged += (sender, e) =>
            {
               PlayMethodView();

            };
            _mediaPlayerViewModel.PlayValueRequested += (sender, e) =>
            {
                PlayMethodView();
            };

            _mediaPlayerViewModel.VolumeDownEvent += (sender, e) =>
            {if(this.MediaPlayerElement.Volume>0)
                this.MediaPlayerElement.Volume -= 0.1;
            };
            _mediaPlayerViewModel.VolumeUpEvent += (sender, e) =>
            {
                if (this.MediaPlayerElement.Volume < 1)
                    this.MediaPlayerElement.Volume += 0.1;
            };
            _mediaPlayerViewModel.SpeedRatioUpEvent += (sender, e) =>
            {
                this.MediaPlayerElement.SpeedRatio += 0.25;
                MessageBox.Show(this.MediaPlayerElement.SpeedRatio.ToString(),"Speed Ratio");
            };
            _mediaPlayerViewModel.SpeedRatioDownEvent += (sender, e) =>
            {
                this.MediaPlayerElement.SpeedRatio -= 0.15;
                MessageBox.Show(this.MediaPlayerElement.SpeedRatio.ToString(), "Speed Ratio");
            };
            _mediaPlayerViewModel.MoveForwardRequested += (sender,e) =>
            {
                if (this.PlayListView.SelectedIndex < PlayListView.Items.Count && PlayListView.Items.Count>0)
                {
                    this.PlayListView.SelectedIndex = (this.PlayListView.SelectedIndex + 1) % this.PlayListView.Items.Count; ;
                    _mediaPlayerViewModel.OnItemSelectedInPlayListCommand.Execute(this.PlayListView.SelectedValue);
                }
            };
            _mediaPlayerViewModel.MoveBackwardRequested += (sender, e) =>
            {
                if (this.PlayListView.SelectedIndex < PlayListView.Items.Count && PlayListView.Items.Count>0)
                {
                    this.PlayListView.SelectedIndex = (this.PlayListView.SelectedIndex - 1 + this.PlayListView.Items.Count) % this.PlayListView.Items.Count;
                    _mediaPlayerViewModel.OnItemSelectedInPlayListCommand.Execute(this.PlayListView.SelectedValue);
                }
            };
            _mediaPlayerViewModel.PauseRequested += (sender, e) =>
            {
                
                if (this.PlayButton.Content == FindResource("PauseImage"))
                {   this.MediaPlayerElement.Pause();
                    this.PlayButton.Content = FindResource("PlayImage");
                  //  this.PlayButton.IsChecked = true;
                }
                
            };
            _mediaPlayerViewModel.FullScreenEvent += (sender, e) =>
            {
                if (this.WindowStyle != WindowStyle.None)
                {
                   // this.Visibility = Visibility.Collapsed;
                    this.WindowStyle = WindowStyle.None;
                    this.Taskbar.Visibility = System.Windows.Visibility.Collapsed;
                    this.MovieSliderBar.Visibility = System.Windows.Visibility.Collapsed;
                    this.MenuBar.Visibility = System.Windows.Visibility.Collapsed;
                    this.PlayListView.Visibility = System.Windows.Visibility.Collapsed;
                    this.MediaPlayerElement.Stretch = Stretch.Fill;
                    this.ResizeMode = ResizeMode.NoResize;
                    this.WindowState = WindowState.Maximized;
                    this.Visibility = Visibility.Visible;
                }
                else
                {

                   
                    this.WindowStyle = WindowStyle.SingleBorderWindow;
                    this.WindowState = WindowState.Maximized;
                    this.Taskbar.Visibility = System.Windows.Visibility.Visible;
                    this.MovieSliderBar.Visibility = System.Windows.Visibility.Visible;
                    this.MenuBar.Visibility = System.Windows.Visibility.Visible;
                    this.PlayListView.Visibility = System.Windows.Visibility.Visible;
                    this.MediaPlayerElement.Stretch = Stretch.None;
                    this.ResizeMode = ResizeMode.CanResize;
                   
                }
            };
            _mediaPlayerViewModel.StopRequested += (sender, e) =>
            {
                this.MediaPlayerElement.Position = new TimeSpan(0, 0, 0);
                this.MediaPlayerElement.Stop();
                this.MoviePositionSlider.Value = 0;
                this.PlayButton.IsChecked = true;
                this.PlayButton.Content = FindResource("PlayImage");
                
            };

            
            _mediaPlayerViewModel.MediaOpenedRequested += (sender, e) =>
            {
               

               
                            
                if(this.MediaPlayerElement.IsLoaded)
                {
                   

                    // Create a timer that will update the counters and the time slider
                     timerVideoTime = new DispatcherTimer();
                    timerVideoTime.Interval = new TimeSpan(0, 0, 1);
                    timerVideoTime.Tick += Timer_Tick;
                    timerVideoTime.Start();
                }
            };

     
        }

        private void MoviePositionSlider_OnValueChanged(object sender, DragCompletedEventArgs e)
        {
            
             if (this.MediaPlayerElement.NaturalDuration.HasTimeSpan)
            {
                this.MoviePositionSlider.Maximum = this.MediaPlayerElement.NaturalDuration.TimeSpan.TotalSeconds;
                this.MediaPlayerElement.Position = new TimeSpan(0, 0, (int)this.MoviePositionSlider.Value);  
            
            }
        }

        

        private void PlayMethodView()
        {
            this.PlayButton.IsEnabled = true;
            if (this.MediaPlayerElement.Source != null)
            {
                string[] splittedStrings = this.MediaPlayerElement.Source.ToString().Split('/');
                this.Title = splittedStrings[splittedStrings.Length - 1];
                if (this.PlayButton.Content == FindResource("PlayImage"))
                {
                    this.MediaPlayerElement.Play();

                    if (this.MediaPlayerElement.HasVideo == false)
                    {
                        /* To be Put later on  */
                    }
                    this.PlayButton.Content = FindResource("PauseImage");
                    //     this.PlayButton.IsChecked = false;
                   


                }
            }

        }
        void Timer_Tick(object sender, EventArgs e)
        {
            // Check if the movie finished calculate it's total time
            if (this.MediaPlayerElement.NaturalDuration.HasTimeSpan)
            {
                TotalTime = MediaPlayerElement.NaturalDuration.TimeSpan;
                this.TimeToComplete.Content = this.MediaPlayerElement.NaturalDuration.TimeSpan.ToString(@"hh\:mm\:ss");
                this.Timer.Content = this.MediaPlayerElement.Position.ToString(@"hh\:mm\:ss");
                if (this.MediaPlayerElement.NaturalDuration.TimeSpan.TotalSeconds > 0)
                {
                    if (this.TotalTime.TotalSeconds > 0)
                    {
                        // Updating time slider
                        this.MoviePositionSlider.Maximum = this.MediaPlayerElement.NaturalDuration.TimeSpan.TotalSeconds;
                        this.MoviePositionSlider.Value = this.MediaPlayerElement.Position.TotalSeconds;

                    }
                }
            }
            else
            {
                timerVideoTime.Stop();
            }
        }

        private void MediaPlayerElement_OnMediaOpened(object sender, RoutedEventArgs e)
        {
            _mediaPlayerViewModel.MediaOpened = true;

        }

        private void MediaPlayerElement_OnMediaEnded(object sender, RoutedEventArgs e)
        {
            
            {
                this.PlayListView.SelectedIndex = (this.PlayListView.SelectedIndex+1) % this.PlayListView.Items.Count;
                _mediaPlayerViewModel.ItemSelectedInPlayList(this.PlayListView.SelectedValue);
            }
        }
    }
}
