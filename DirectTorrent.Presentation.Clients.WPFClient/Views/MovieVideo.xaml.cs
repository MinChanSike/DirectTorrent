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
using DirectTorrent.Presentation.Clients.WPFClient.ViewModels;
using FirstFloor.ModernUI.Windows;

namespace DirectTorrent.Presentation.Clients.WPFClient.Views
{
    /// <summary>
    /// Interaction logic for MovieVideo.xaml
    /// </summary>
    public partial class MovieVideo : ModernWindow
    {
        public MovieVideo()
        {
            InitializeComponent();
            this.Player.Play();
            ((MovieVideoViewModel) this.DataContext).PlayRequested += (sender, args) =>
            {
                this.Player.Play();
            };

            ((MovieVideoViewModel)this.DataContext).PauseRequested += (sender, args) =>
            {
                this.Player.Pause();
            };

            ((MovieVideoViewModel)this.DataContext).StopRequested += (sender, args) =>
            {
                this.Player.Stop();
            };
        }

        //private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        //{
        //    this.Player.Position = new TimeSpan(0, 0, 0, 0, (int)e.NewValue);
        //}

        private bool first = true;
        private void Player_OnMediaOpened(object sender, RoutedEventArgs e)
        {
            if (first)
            {
                this.Slider.Maximum = this.Player.NaturalDuration.TimeSpan.TotalMilliseconds;
                this.Player.Pause();
                first = false;
            }
        }
    }

    public class VolumeConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (double) value/100;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
