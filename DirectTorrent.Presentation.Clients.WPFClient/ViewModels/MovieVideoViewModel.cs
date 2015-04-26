using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace DirectTorrent.Presentation.Clients.WPFClient.ViewModels
{
    class MovieVideoViewModel : GalaSoft.MvvmLight.ViewModelBase
    {
        public GalaSoft.MvvmLight.CommandWpf.RelayCommand PlayButtonClicked { get; set; }
        public GalaSoft.MvvmLight.CommandWpf.RelayCommand PauseButtonClicked { get; set; }
        public GalaSoft.MvvmLight.CommandWpf.RelayCommand StopButtonClicked { get; set; }
        public GalaSoft.MvvmLight.CommandWpf.RelayCommand<DragStartedEventArgs> SliderDragStarted { get; set; }
        public GalaSoft.MvvmLight.CommandWpf.RelayCommand<DragCompletedEventArgs> SliderDragCompleted { get; set; }
        public GalaSoft.MvvmLight.CommandWpf.RelayCommand<MouseWheelEventArgs> MouseWheelMove { get; set; } 
        public double Volume { get; set; }
        public TimeSpan Position { get; set; }
        public MovieVideoViewModel()
        {
            this.Volume = 1; // TODO: Fix Progress to volume conversion, remove this
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
