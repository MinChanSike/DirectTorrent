using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Navigation;

using FirstFloor.ModernUI.Presentation;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace DirectTorrent.Presentation.Clients.WPFClient.ViewModels
{
    class MovieVideoViewModel : GalaSoft.MvvmLight.ViewModelBase
    {
        public GalaSoft.MvvmLight.CommandWpf.RelayCommand PlayButtonClicked { get; private set; }
        public GalaSoft.MvvmLight.CommandWpf.RelayCommand PauseButtonClicked { get; private set; }
        public GalaSoft.MvvmLight.CommandWpf.RelayCommand StopButtonClicked { get; private set; }
        public GalaSoft.MvvmLight.CommandWpf.RelayCommand<RoutedPropertyChangedEventArgs<double>> SliderValueChanged { get; private set; }
        public GalaSoft.MvvmLight.CommandWpf.RelayCommand<MouseWheelEventArgs> MouseWheelMove { get; private set; }

        public event EventHandler PlayRequested;
        public event EventHandler StopRequested;
        public event EventHandler PauseRequested;

        // from 0 to 100, value converter scales it to 0 to 1
        private double _volume = 50;
        public double Volume
        {
            get { return this._volume; }
            private set
            {
                if (this._volume != value)
                {
                    this._volume = value;
                    RaisePropertyChanged("Volume");
                }
            }
        }

        // in miliseconds
        private double _position;
        public double Position
        {
            get { return this._position; }
            private set
            {
                if (this._position != value)
                {
                    this._position = value;
                    RaisePropertyChanged("Position");
                }
            }
        }

        private string _currentSubtitle = "TEST";
        public string CurrentSubtitle
        {
            get { return this._currentSubtitle; }
            private set
            {
                if (this._currentSubtitle != value)
                {
                    this._currentSubtitle = value;
                    RaisePropertyChanged("CurrentSubtitle");
                }
            }
        }

        public MovieVideoViewModel()
        {
            this.MouseWheelMove = new RelayCommand<MouseWheelEventArgs>((e) =>
            {
                this.Volume += e.Delta * 1.0 / 100;
                if (this.Volume > 100)
                    this.Volume = 100;
                if (this.Volume < 0)
                    this.Volume = 0;
            });

            this.SliderValueChanged = new RelayCommand<RoutedPropertyChangedEventArgs<double>>((e) =>
            {
                this.Position = e.NewValue;
            });

            this.PlayButtonClicked = new GalaSoft.MvvmLight.CommandWpf.RelayCommand(() =>
            {
                if (this.PlayRequested != null)
                {
                    this.PlayRequested(this, EventArgs.Empty);
                }
            });

            this.StopButtonClicked = new GalaSoft.MvvmLight.CommandWpf.RelayCommand(() =>
            {
                if (this.StopRequested != null)
                {
                    this.StopRequested(this, EventArgs.Empty);
                }
            });

            this.PauseButtonClicked = new GalaSoft.MvvmLight.CommandWpf.RelayCommand(() =>
            {
                if (this.PauseRequested != null)
                {
                    this.PauseRequested(this, EventArgs.Empty);
                }
            });
            //AxWMPLib.AxWindowsMediaPlayer player = new AxWindowsMediaPlayer();
            //Host.Child = player;
            //    if (magnetUri != string.Empty)
            //    {
            //        if (File.Exists("hash.txt"))
            //            File.Delete("hash.txt");
            //        NodeServerManager.StartServer(magnetUri);
            //        BackgroundWorker worker = new BackgroundWorker();
            //        worker.DoWork += (sender, e) =>
            //        {
            //            while (true)
            //            {
            //                if (File.Exists("hash.txt"))
            //                    break;
            //            }
            //        };
            //        worker.RunWorkerCompleted += (sender, e) =>
            //        {
            //            File.Delete("hash.txt");
            //            //this.Player.closedCaption.SAMIFileName=
            //            //this.Player.closedCaption.SAMIFileName = "sample.sami";
            //            //this.Player.URL = "http://localhost:1337";
            //            //Player.BufferingStarted += (obj, args) => { Player.Play(); };
            //            Player.Play();
            //            Player.MediaOpened += (obj, args) => { MessageBox.Show("ready!"); };
            //            var parser = new SubtitlesParser.Classes.Parsers.SrtParser();
            //            using (var fileStream = File.OpenRead("sample.srt"))
            //            {
            //                var items = parser.ParseStream(fileStream, UTF8Encoding.UTF8);
            //                Subtitles.Text = items[0].Lines[0];
            //            }
            //        };
            //        worker.RunWorkerAsync();

            //        //this.Player.URL = "http://localhost:1337";
            //        //axWmp.URL = "http://localhost:1337";
            //    }
            //}

            //private void ModernWindow_Closing(object sender, CancelEventArgs e)
            //{
            //    NodeServerManager.CloseServer();
            //}

            //private void Button_Click(object sender, RoutedEventArgs e)
            //{
            //    //MessageBox.Show(Player.BufferingProgress.ToString("F1"));
            //    Player.Play();
            //}
        }
    }
}
