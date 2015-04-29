using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;

using FirstFloor.ModernUI.Windows.Controls;
using DirectTorrent.Logic.Services;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Channels;
using System.Windows.Threading;
using DirectTorrent.Presentation.Clients.WPFClient.ViewModels;
using FirstFloor.ModernUI.Windows;

namespace DirectTorrent.Presentation.Clients.WPFClient.Views
{
    /// <summary>
    /// Interaction logic for MovieVideo.xaml
    /// </summary>
    public partial class MovieVideo : ModernWindow
    {
        private bool IsPlaying = false;
        private bool first = true;
        private DispatcherTimer timer;
        public MovieVideo()
        {
            InitializeComponent();
            //this.Player.Play();
            ((MovieVideoViewModel)this.DataContext).PlayRequested += (sender, args) =>
            {
                if (first)
                {
                    this.Player.Source = new Uri("http://localhost:1337", UriKind.Absolute);
                }

                IsPlaying = true;
                this.Player.Play();
                first = false;
            };

            ((MovieVideoViewModel)this.DataContext).PauseRequested += (sender, args) =>
            {
                IsPlaying = false;
                this.Player.Pause();
            };

            ((MovieVideoViewModel)this.DataContext).StopRequested += (sender, args) =>
            {
                IsPlaying = false;
                this.Player.Stop();
            };

            ((MovieVideoViewModel)this.DataContext).SeekRequested += (sender, args) =>
            {
                this.Player.Pause();
                this.Player.Position = TimeSpan.FromMilliseconds(args);
                this.Player.Play();
            };

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(900);
            timer.Start();
            timer.Tick += (sender, args) =>
            {
                if (IsPlaying)
                {
                    ((MovieVideoViewModel)this.DataContext).ManualSliderChange = false;
                    ((MovieVideoViewModel)this.DataContext).Position = this.Player.Position.Seconds;
                }
            };
        }

        //private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        //{
        //    this.Player.Position = new TimeSpan(0, 0, 0, 0, (int)e.NewValue);
        //}

        //private bool first = true;
        private void Player_OnMediaOpened(object sender, RoutedEventArgs e)
        {
            //if (first)
            //{
            //    this.Slider.Maximum = this.Player.NaturalDuration.TimeSpan.Seconds;
            //    this.Player.Pause();
            //    first = false;
            //    timer.Start();
            //}
            ((MovieVideoViewModel) this.DataContext).LoaderVisibility = Visibility.Collapsed;
            ((MovieVideoViewModel) this.DataContext).MovieVisibility = Visibility.Visible;
            this.Slider.Maximum = Data.Runtime * 60;
            timer.Start();
        }

        private void ModernWindow_Closing(object sender, CancelEventArgs e)
        {
            NodeServerManager.CloseServer();
        }
    }

    public class VolumeConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (double)value / 100;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
