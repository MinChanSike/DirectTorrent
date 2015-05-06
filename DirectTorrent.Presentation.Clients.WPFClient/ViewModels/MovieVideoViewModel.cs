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
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using System.IO.Compression;
using System.Net;
using System.Threading;
using SubtitlesParser.Classes;
using SubtitlesParser.Classes.Parsers;

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
        public event EventHandler<int> SeekRequested;

        public bool ManualSliderChange = true;

        // from 0 to 100, value converter scales it to 0 to 1
        private double _volume = 50;
        public double Volume
        {
            get { return this._volume; }
            private set { this.Set(ref this._volume, value); }
        }

        // in miliseconds
        private int _position;
        public int Position
        {
            get { return this._position; }
            set { this.Set(ref this._position, value); }
        }

        private SubtitleItem _currentSubtitle = null;
        public SubtitleItem CurrentSubtitle
        {
            get { return this._currentSubtitle; }
            private set { this.Set(ref this._currentSubtitle, value); }
        }

        private Visibility _loaderVisibility = Visibility.Visible;
        public Visibility LoaderVisibility
        {
            get { return this._loaderVisibility; }
            set { this.Set(ref this._loaderVisibility, value); }

        }

        private Visibility _movieVisibility = Visibility.Collapsed;
        public Visibility MovieVisibility
        {
            get { return this._movieVisibility; }
            set { this.Set(ref this._movieVisibility, value); }
        }

        private Visibility _subtitleVisibility = Visibility.Collapsed;
        public Visibility SubtitleVisibility
        {
            get { return this._subtitleVisibility; }
            private set { this.Set(ref this._subtitleVisibility, value); }
        }

        public string MagnetUri = string.Empty;
        private string guid;
        private List<SubtitleItem> subs = new List<SubtitleItem>();
        private SrtParser srtParser = new SrtParser();

        public MovieVideoViewModel()
        {
            Messenger.Default.Register<string>(this, "magnetUri", uri => { NodeServerManager.StartServer(uri); });
            Messenger.Default.Register<string>(this, "guid", r => { this.guid = r; });
            Messenger.Default.Register<string>(this, "subtitle", async (s) =>
            {
                this.SubtitleVisibility = Visibility.Visible;

                this.subs.Clear();
                this.subs.AddRange(await DownloadSubtitle(s));
            });

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
                else
                {
                    if (this.CurrentSubtitle != null && (this.CurrentSubtitle.EndTime / 1000) == this.Position)
                        this.CurrentSubtitle = null;

                    var sub = this.subs.FirstOrDefault(x => x.StartTime / 1000 == this.Position);
                    if (sub != null)
                        this.CurrentSubtitle = sub;
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

            // NodeServerManager.StartServer(this.MagnetUri);
            // Custom autoplay hack
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

        private async Task<IEnumerable<SubtitleItem>> DownloadSubtitle(string address)
        {
            try
            {
                var request = FileWebRequest.Create(address);
                using (var response = await request.GetResponseAsync())
                {
                    using (var archive = new ZipArchive(response.GetResponseStream(), ZipArchiveMode.Read))
                    {
                        var entry = archive.Entries.FirstOrDefault();
                        if (entry != null)
                        {
                            using (var stream = entry.Open())
                            {
                                using (var downloaded = new MemoryStream())
                                {
                                    await stream.CopyToAsync(downloaded);
                                    return srtParser.ParseStream(downloaded, Encoding.Default);
                                }
                            }
                        }
                    }
                }
            }
            catch { }

            return new SubtitleItem[] { };
        }

        public void UnregisterViewModel()
        {
            Messenger.Default.Unregister(this);
            SimpleIoc.Default.Unregister(guid);
        }
    }
}