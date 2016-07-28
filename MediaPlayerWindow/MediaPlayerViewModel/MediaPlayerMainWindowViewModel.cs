using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Xml.Serialization;
using MediaPlayerModels;
using MediaPlayerViewModel.Annotations;
using MediaPlayerWindow.Commands;
using System.Windows.Input;
using Button = System.Windows.Controls.Button;
using System.Runtime.Serialization;
namespace MediaPlayerViewModel
{
    public class MediaPlayerMainWindowViewModel:INotifyPropertyChanged
    {
        private Uri _mediaSource;
        private bool _playRequested=false;
        private bool _mediaOpened = false;
        private System.Windows.Visibility listVisibility;
        private Dictionary<string, Uri> playlistLookupDictionary=new Dictionary<string, Uri>();
      // private  ObservableCollection<Uri> folder=new ObservableCollection<Uri>(); 
       private ObservableCollection<Uri> playListViewCollection=new ObservableCollection<Uri>();
       private ObservableCollection<ObservableCollection<Uri>> albumsViewCollection = new ObservableCollection<ObservableCollection<Uri>>();

       

        public ObservableCollection<Uri> PlayListViewCollection
        {
            get { return playListViewCollection; }
            set { playListViewCollection = value; }
        }

        public System.Windows.Visibility ListVisibility
        {
            get { return listVisibility; }
            set { listVisibility = value;OnPropertyChanged("ListVisibility"); }
        }
        //public event PlayRequestedValueChanged;
        public event EventHandler PlayValueRequested;
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler PauseRequested;
        public event EventHandler StopRequested;
        public event EventHandler MoveForwardRequested;
        public event EventHandler MoveBackwardRequested;
        public event EventHandler MediaOpenedRequested;
        public event EventHandler VolumeUpEvent;
        public event EventHandler VolumeDownEvent;

        public ObservableCollection<ObservableCollection<Uri>> AlbumsViewCollection
        {
            get { return albumsViewCollection; }
            set { albumsViewCollection = value; }
        }

        public bool PlayRequested
        {
            get { return _playRequested; }
            set { _playRequested = value; OnPropertyChanged("PlayRequested");
               
            }
            
        }

        public bool MediaOpened
        {
            get { return _mediaOpened; }
            set { _mediaOpened = value;OnMediaOpenedRequested(); }
        }

        #region label
        public Uri MediaSource
        {
            get { return _mediaSource; }
            set
            {
                _mediaSource = value;
                OnPropertyChanged("MediaSource");
                
            }
        }


        public ICommand  OpenFileCommand
        {
            get { return new DelegateCommand(OpenFileMethod, IsValid); }

        }

        public ICommand ShowHidePlayListCommand
        {
            get { return new DelegateCommand(ShowHidePlayListMethod,IsValid);}
        }

        public ICommand PlayFileCommand
        {
            get { return new DelegateCommand(PlayMediaFileMethod,IsValid);}
        }

        public ICommand VolumeUpCommand
        {
           get { return new DelegateCommand(VolumeUpMethod,IsValid);}
        }

        public ICommand VolumeDownCommand
        {
            get { return new DelegateCommand(VolumeDownMethod,IsValid);}
        }
        public ICommand PlayListCommand
        {
            get { return new DelegateCommand(PlayListMethod,IsValid);}
        }

        public ICommand OnItemSelectedInPlayListCommand
        {
            get { return new DelegateCommand(ItemSelectedInPlayList,IsValid);}
        }
        public ICommand StopFileCommand
        {
            get { return new DelegateCommand(StopMediaFileMethod, IsValid); }
        }

        public ICommand SavePlayListCommand
        {
            get { return new DelegateCommand(SavePlaylist,IsValid);}
        }

        public ICommand MediaOpenedCommand
        {
            get { return new DelegateCommand(MediaOpenedMethod,IsValid);}
        }
        public ICommand OpenPlayListCommand
        {
            get { return new DelegateCommand(OpenPlayList,IsValid);}
        }
        public ICommand MoveForwardFileCommand
        {
            get { return new DelegateCommand(MoveForwardMediaFileMethod, IsValid); }
        }
        public ICommand MoveBackwardFileCommand
        {
            get { return new DelegateCommand(MoveBackwardMediaFileMethod, IsValid); }
        }


        #endregion
        #region Events
        protected virtual void OnPropertyChanged(string propertyName)
        {
            // PropertyChangedEventHandler handler = PropertyChanged;
            // if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
            if (PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        
        protected virtual void OnPauseRequested()
        {
            if (PauseRequested != null)
            {
                this.PauseRequested(this,new EventArgs());
            }
        }
        protected virtual void OnPlayValueRequested()
        {
            if (PlayValueRequested != null)
            {
                this.PlayValueRequested(this, new EventArgs());
            }
        }

        protected virtual void OnVolumeUpEvent()
        {
            if (VolumeUpEvent != null)
            {
                this.VolumeUpEvent(this,new EventArgs());
            }
        }

        protected virtual void OnVolumeDownEvent()
        {
            if (VolumeDownEvent != null)
            {
                this.VolumeDownEvent(this,new EventArgs());
            }
        }
        protected virtual void OnMediaOpenedRequested()
        {
            if (MediaOpenedRequested != null)
            {
                this.MediaOpenedRequested(this,new EventArgs());
            }
        }
        protected virtual void OnStopRequested()
        {
            if (StopRequested != null)
            {
                this.StopRequested(this, new EventArgs());
            }
        }
        protected virtual void OnMoveForwardRequested()
        {
            if (MoveForwardRequested != null)
            {
                this.MoveForwardRequested(this, EventArgs.Empty);
            }
        }
        protected virtual void OnMoveBackwardRequested()
        {
            if (MoveBackwardRequested != null)
            {
                this.MoveBackwardRequested(this, new EventArgs());
            }
        }

        #endregion

        #region Methods
        private void OpenFileMethod(object obj)
        {
           
            System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();

           // MediaPlayerButtonObj = obj as MediaElement;

            System.Windows.Forms.DialogResult dialogResult = dialog.ShowDialog();
            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                MediaSource = new Uri(dialog.FileName);
                playlistLookupDictionary.Add(dialog.SafeFileName,MediaSource);
                PlayListViewCollection.Add(new Uri(dialog.SafeFileName,UriKind.Relative));
              
            }

       }

        private void VolumeUpMethod(object o)
        {
            OnVolumeUpEvent();
        }

        private void VolumeDownMethod(Object o)
        {
            OnVolumeDownEvent();
        }

        private void MediaOpenedMethod(Object obj)
        {
            MediaOpened = true;
        }
        private void SavePlaylist(Object obj)
        {
           
            XmlSerializer _xmlSerializer = new XmlSerializer(typeof(PlayListDirectory));
            SaveFileDialog _playListSaveDialog=new SaveFileDialog();
            _playListSaveDialog.Title = "Save PlayList";
            if (_playListSaveDialog.ShowDialog() == DialogResult.OK)
            {
                string filename = _playListSaveDialog.FileName;
                using (TextWriter _textWriter = new StreamWriter(filename))
                {
                    PlayListDirectory playListDirectory=new PlayListDirectory();
                    
                   
                    List<PlayListClass> _mediaList = new List<PlayListClass>();
                    foreach (Uri file in PlayListViewCollection )
                    {
                        PlayListClass temp = new PlayListClass();
                        temp.FileName = file.ToString();
                        temp.Filepath = playlistLookupDictionary[file.ToString()].ToString();
                        _mediaList.Add(temp);
                        
                    }
                    playListDirectory.PlayListCollection = _mediaList;
                    _xmlSerializer.Serialize(_textWriter, playListDirectory);
                }
            }

        }

        private void OpenPlayList(Object obj)
        {
            playlistLookupDictionary.Clear();
            playListViewCollection.Clear();
            XmlSerializer _xmlDeSerializer=new XmlSerializer(typeof(PlayListDirectory));
            OpenFileDialog _playListOpenFileDialog=new OpenFileDialog();
            _playListOpenFileDialog.Title = "Open PlayList";
            if (_playListOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filename = _playListOpenFileDialog.FileName;
                TextReader _textReader=new StreamReader(filename);
                Object xmlObject =_xmlDeSerializer.Deserialize(_textReader);
                PlayListDirectory xmlDataCollection = (PlayListDirectory) xmlObject;
                _textReader.Close();
                foreach (PlayListClass tempList in xmlDataCollection.PlayListCollection)
                {
                     playlistLookupDictionary.Add(tempList.FileName,new Uri(tempList.Filepath,UriKind.Absolute));
                     playListViewCollection.Add(new Uri(tempList.FileName,UriKind.Relative));
                }
               


                

            }

        }

        private void ShowHidePlayListMethod(Object obj)
        {
            System.Windows.Controls.ListBox _list = obj as System.Windows.Controls.ListBox;
            if (ListVisibility == System.Windows.Visibility.Visible)
            {
                ListVisibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                ListVisibility = System.Windows.Visibility.Visible;
            }
        }
       

        private void StopMediaFileMethod(Object Obj)
        {
             OnStopRequested();
            
        }

       
        private void MoveForwardMediaFileMethod(Object Obj)
        {
            OnMoveForwardRequested();
        }

        private void MoveBackwardMediaFileMethod(Object obj)
        {
            OnMoveBackwardRequested();
        }
        private void PlayListMethod(Object obj)
        {
            System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.Multiselect = true;
           // dialog.Filter = "*.mp4";
            // MediaPlayerButtonObj = obj as MediaElement;
            dialog.Title = "Add Media Files To PlayList";
            System.Windows.Forms.DialogResult dialogResult = dialog.ShowDialog();
         
            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                int count_files = 0;
                foreach (string file in dialog.FileNames)
                {
                   // string [] words;
                    //words=file.Split('/');
                    MediaSource = new Uri(file);
                    playlistLookupDictionary.Add(dialog.SafeFileNames[count_files], MediaSource);
                    PlayListViewCollection.Add(new Uri(dialog.SafeFileNames[count_files],UriKind.Relative));
                     count_files ++;
                    //  MessageBox.Show(PlayFileButtonObj.IsEnabled.ToString());
                    // MediaPlayerButtonObj.LoadedBehavior = MediaState.Manual;
                    // MediaTimeline timeline = new MediaTimeline(new Uri("MediaMainWindow\\download.jpg", UriKind.Relative));
                    // ButtonObj.IsEnabled = true;
                    // MediaPlayer.Clock = timeline.CreateClock();
                    // MediaPlayerButtonObj.Play();
                    // MessageBox.Show(MediaPlayerButtonObj.LoadedBehavior.ToString());
                }
            }
        }

        private void PlayMediaFileMethod(Object obj)
        {
            
            ToggleButton  PlayFileButtonObj = obj as ToggleButton;
            PlayFileButtonObj.IsChecked = PlayFileButtonObj.IsChecked.Value  ? false : true;
            if (PlayFileButtonObj.IsEnabled)
            {
                if (PlayFileButtonObj.IsChecked.Value)
                {
                    OnPlayValueRequested();
                   

                }
                else
                {
                    OnPauseRequested();
                   
                }
            }
        }

        private void ItemSelectedInPlayList(object obj)
        {
          //  TextBlock textBlockViewModelObj = obj as TextBlock;
            
            string sourceText = obj.ToString();

          //  MediaSource = playlistLookupDictionary[textBlockViewModelObj.Text];
            MediaSource = playlistLookupDictionary[sourceText];
            PlayRequested = true;
        }

        private void AlbumsCreateMethod()
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowserControl = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = folderBrowserControl.ShowDialog();
            if (result == DialogResult.OK)
            {
                
            }
        }


        private bool IsValid()
        {
           // if (obj == null) throw new ArgumentNullException("obj");
            return true;
        }

#endregion
       

       
       
    }
}
