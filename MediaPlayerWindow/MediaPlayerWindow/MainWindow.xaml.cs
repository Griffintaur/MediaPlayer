using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace MediaPlayerWindow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MediaPlayerViewModel.MediaPlayerMainWindowViewModel _mediaPlayerViewModel = new MediaPlayerViewModel.MediaPlayerMainWindowViewModel();
        private TimeSpan TotalTime;
        private DispatcherTimer timerVideoTime;
        public MainWindow()
        {
            InitializeComponent();

            DataContext = _mediaPlayerViewModel;

            SetUpEventsHandler();

        }

        private void SetUpEventsHandler()
        {
            _mediaPlayerViewModel.PropertyChanged += (sender, e) =>
            {
                PlayMethodView();

            };
            _mediaPlayerViewModel.PlayValueRequested += (sender, e) =>
            {
                PlayMethodView();
            };

            _mediaPlayerViewModel.VolumeDownEvent += (sender, e) =>
            {
                if (MediaPlayerElement.Volume > 0)
                    MediaPlayerElement.Volume -= 0.1;
            };
            _mediaPlayerViewModel.VolumeUpEvent += (sender, e) =>
            {
                if (MediaPlayerElement.Volume < 1)
                    MediaPlayerElement.Volume += 0.1;
            };
            _mediaPlayerViewModel.SpeedRatioUpEvent += (sender, e) =>
            {
                MediaPlayerElement.SpeedRatio += 0.25;
                MessageBox.Show(MediaPlayerElement.SpeedRatio.ToString(), "Speed Ratio");
            };
            _mediaPlayerViewModel.SpeedRatioDownEvent += (sender, e) =>
            {
                MediaPlayerElement.SpeedRatio -= 0.15;
                MessageBox.Show(MediaPlayerElement.SpeedRatio.ToString(), "Speed Ratio");
            };
            _mediaPlayerViewModel.MoveForwardRequested += (sender, e) =>
            {
                if (PlayListView.SelectedIndex < PlayListView.Items.Count && PlayListView.Items.Count > 0)
                {
                    PlayListView.SelectedIndex = (PlayListView.SelectedIndex + 1) % PlayListView.Items.Count; ;
                    _mediaPlayerViewModel.OnItemSelectedInPlayListCommand.Execute(PlayListView.SelectedValue);
                }
            };
            _mediaPlayerViewModel.MoveBackwardRequested += (sender, e) =>
            {
                if (PlayListView.SelectedIndex < PlayListView.Items.Count && PlayListView.Items.Count > 0)
                {
                    PlayListView.SelectedIndex = (PlayListView.SelectedIndex - 1 + PlayListView.Items.Count) % PlayListView.Items.Count;
                    _mediaPlayerViewModel.OnItemSelectedInPlayListCommand.Execute(PlayListView.SelectedValue);
                }
            };
            _mediaPlayerViewModel.PauseRequested += (sender, e) =>
            {

                if (PlayButton.Content == FindResource("PauseImage"))
                {
                    MediaPlayerElement.Pause();
                    PlayButton.Content = FindResource("PlayImage");
                    //  this.PlayButton.IsChecked = true;
                }

            };
            _mediaPlayerViewModel.FullScreenEvent += (sender, e) =>
            {
                if (WindowStyle != WindowStyle.None)
                {
                    // this.Visibility = Visibility.Collapsed;
                    WindowStyle = WindowStyle.None;
                    Taskbar.Visibility = Visibility.Collapsed;
                    MovieSliderBar.Visibility = Visibility.Collapsed;
                    MenuBar.Visibility = Visibility.Collapsed;
                    PlayListView.Visibility = Visibility.Collapsed;
                    MediaPlayerElement.Stretch = Stretch.Fill;
                    ResizeMode = ResizeMode.NoResize;
                    WindowState = WindowState.Maximized;
                    Visibility = Visibility.Visible;
                }
                else
                {
                    WindowStyle = WindowStyle.SingleBorderWindow;
                    WindowState = WindowState.Maximized;
                    Taskbar.Visibility = Visibility.Visible;
                    MovieSliderBar.Visibility = Visibility.Visible;
                    MenuBar.Visibility = Visibility.Visible;
                    PlayListView.Visibility = Visibility.Visible;
                    MediaPlayerElement.Stretch = Stretch.None;
                    ResizeMode = ResizeMode.CanResize;

                }
            };
            _mediaPlayerViewModel.StopRequested += (sender, e) =>
            {
                MediaPlayerElement.Position = new TimeSpan(0, 0, 0);
                MediaPlayerElement.Stop();
                MoviePositionSlider.Value = 0;
                PlayButton.IsChecked = true;
                PlayButton.Content = FindResource("PlayImage");
            };

            _mediaPlayerViewModel.MediaOpenedRequested += (sender, e) =>
            {
                if (MediaPlayerElement.IsLoaded)
                {
                    // Create a timer that will update the counters and the time slider
                    timerVideoTime = new DispatcherTimer();
                    timerVideoTime.Interval = new TimeSpan(0, 0, 1);
                    timerVideoTime.Tick += Timer_Tick;
                    timerVideoTime.Start();
                }
            };

            _mediaPlayerViewModel.ChangeBackgroundEvent += (sender, e) =>
            {
                Background = Background = new ImageBrush(new BitmapImage(new Uri(_mediaPlayerViewModel.ImageUri)));
            };
        }

        private void MoviePositionSlider_OnValueChanged(object sender, EventArgs e)
        {
            if (MediaPlayerElement.NaturalDuration.HasTimeSpan)
            {
                MoviePositionSlider.Maximum = MediaPlayerElement.NaturalDuration.TimeSpan.TotalSeconds;
                MediaPlayerElement.Position = new TimeSpan(0, 0, (int)MoviePositionSlider.Value);
            }
        }

        private void PlayMethodView()
        {
            PlayButton.IsEnabled = true;
            if (MediaPlayerElement.Source != null)
            {
                string[] splittedStrings = MediaPlayerElement.Source.ToString().Split('/');
                Title = splittedStrings[splittedStrings.Length - 1];
                if (PlayButton.Content == FindResource("PlayImage"))
                {
                    MediaPlayerElement.Play();

                    if (MediaPlayerElement.HasVideo == false)
                    {
                        /* To be Put later on  */
                    }
                    PlayButton.Content = FindResource("PauseImage");
                    //     this.PlayButton.IsChecked = false;
                }
            }

        }
        void Timer_Tick(object sender, EventArgs e)
        {
            // Check if the movie finished calculate it's total time
            if (MediaPlayerElement.NaturalDuration.HasTimeSpan)
            {
                TotalTime = MediaPlayerElement.NaturalDuration.TimeSpan;
                TimeToComplete.Content = MediaPlayerElement.NaturalDuration.TimeSpan.ToString(@"hh\:mm\:ss");
                Timer.Content = MediaPlayerElement.Position.ToString(@"hh\:mm\:ss");
                if (MediaPlayerElement.NaturalDuration.TimeSpan.TotalSeconds > 0)
                {
                    if (TotalTime.TotalSeconds > 0)
                    {
                        // Updating time slider
                        MoviePositionSlider.Maximum = MediaPlayerElement.NaturalDuration.TimeSpan.TotalSeconds;
                        MoviePositionSlider.Value = MediaPlayerElement.Position.TotalSeconds;

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
            PlayListView.SelectedIndex = (PlayListView.SelectedIndex + 1) % PlayListView.Items.Count;
            _mediaPlayerViewModel.ItemSelectedInPlayList(PlayListView.SelectedValue);
        }
    }
}
