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
        }
    }

    public class ProgressToVolumeConverter : IValueConverter
    {

        object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double test = (double)value;
            double test2 = test*1.0;
            double test3 = test2/2;
            return test3;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DoubleToPositionConverter : IValueConverter
    {

        object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
