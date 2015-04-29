using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.IO;
using DirectTorrent.Logic.Services;

namespace DirectTorrent.Presentation.Clients.WPFClient.ViewModels
{
    class MovieVideoViewModel : GalaSoft.MvvmLight.ViewModelBase
    {
        // TODO: Add subtitles
        public GalaSoft.MvvmLight.CommandWpf.RelayCommand PlayButtonClicked { get; private set; }
        public GalaSoft.MvvmLight.CommandWpf.RelayCommand PauseButtonClicked { get; private set; }
        public GalaSoft.MvvmLight.CommandWpf.RelayCommand StopButtonClicked { get; private set; }
        public GalaSoft.MvvmLight.CommandWpf.RelayCommand<RoutedPropertyChangedEventArgs<double>> SliderValueChanged { get; private set; }
        public GalaSoft.MvvmLight.CommandWpf.RelayCommand<MouseWheelEventArgs> MouseWheelMove { get; private set; }

        public event EventHandler PlayRequested;
        public event EventHandler StopRequested;
        public event EventHandler PauseRequested;
        public event EventHandler<int> SeekRequested;

        public bool ManualSliderChange = true;

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
        private int _position;
        public int Position
        {
            get { return this._position; }
            set
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

        private Visibility _loaderVisibility = Visibility.Visible;
        public Visibility LoaderVisibility
        {
            get { return this._loaderVisibility; }
            set
            {
                if (this._loaderVisibility != value)
                {
                    this._loaderVisibility = value;
                    RaisePropertyChanged("LoaderVisibility");
                }
            }
            
        }

        private Visibility _movieVisibility = Visibility.Collapsed;
        public Visibility MovieVisibility
        {
            get { return this._movieVisibility; }
            set
            {
                if (this._movieVisibility != value)
                {
                    this._movieVisibility = value;
                    RaisePropertyChanged("MovieVisibility");
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
                if (ManualSliderChange)
                {
                    this.Position = (int)e.NewValue;
                    if (this.SeekRequested != null)
                        this.SeekRequested(this, this.Position);
                }

                ManualSliderChange = true;
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

            if (File.Exists("hash.txt"))
                File.Delete("hash.txt");
            NodeServerManager.StartServer(Data.MagnetUri);
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (sender, args) =>
            {
                while (true)
                {
                    if (File.Exists("hash.txt"))
                        break;
                }
            };
            worker.RunWorkerCompleted += (sender, args) =>
            {
                if (this.PlayRequested != null)
                {
                    File.Delete("hash.txt");
                    this.PlayRequested(this, EventArgs.Empty);
                }
            };
            worker.RunWorkerAsync();
        }
    }
}
