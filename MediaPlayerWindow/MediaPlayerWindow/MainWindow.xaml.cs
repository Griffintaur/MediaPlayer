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

            _mediaPlayerViewModel.MoveForwardRequested += (sender,e) =>
            {
                if (this.PlayListView.SelectedIndex < PlayListView.Items.Count)
                {
                    this.PlayListView.SelectedIndex ++;
                    _mediaPlayerViewModel.OnItemSelectedInPlayListCommand.Execute(this.PlayListView.SelectedValue);
                }
            };
            _mediaPlayerViewModel.MoveBackwardRequested += (sender, e) =>
            {
                if (this.PlayListView.SelectedIndex < PlayListView.Items.Count && this.PlayListView.SelectedIndex >0)
                {
                    this.PlayListView.SelectedIndex--;
                    _mediaPlayerViewModel.OnItemSelectedInPlayListCommand.Execute(this.PlayListView.SelectedValue);
                }
            };
            _mediaPlayerViewModel.PauseRequested += (sender, e) =>
            {
                
                if (this.PlayButton.Content == FindResource("PauseImage"))
                {   this.MediaPlayerElement.Pause();
                    this.PlayButton.Content = FindResource("PlayImage");
                    this.PlayButton.IsChecked = true;
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
                    TotalTime = MediaPlayerElement.NaturalDuration.TimeSpan;

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
            
            
                this.MoviePositionSlider.Maximum = this.MediaPlayerElement.NaturalDuration.TimeSpan.TotalSeconds;
                this.MediaPlayerElement.Position = new TimeSpan(0, 0, (int)this.MoviePositionSlider.Value);  
            
            
        }

        

        private void PlayMethodView()
        {
            this.PlayButton.IsEnabled = true;
           this.Title = this.MediaPlayerElement.Source.ToString();
                if (this.PlayButton.Content==FindResource("PlayImage"))
                {
                    this.MediaPlayerElement.Play();
                    
                    if (this.MediaPlayerElement.HasVideo == false)
                    {
                        /* To be Put later on  */
                    }
                    this.PlayButton.Content = FindResource("PauseImage");
                    this.PlayButton.IsChecked = false;
                    

                }
               
        }
        void Timer_Tick(object sender, EventArgs e)
        {
            // Check if the movie finished calculate it's total time
            if (this.MediaPlayerElement.NaturalDuration.HasTimeSpan)
            {
                this.TimeToComplete.Content = this.MediaPlayerElement.NaturalDuration;
                this.Timer.Content = this.MediaPlayerElement.Position;
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
    }
}
