using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using DirectTorrent.Logic.Models;
using DirectTorrent.Presentation.Clients.WPFClient.ViewModels;
using FirstFloor.ModernUI.Windows;
using FirstFloor.ModernUI.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;

namespace DirectTorrent.Presentation.Clients.WPFClient.Views
{
    /// <summary>
    /// Interaction logic for MovieDetails.xaml
    /// </summary>
    public partial class MovieDetails : UserControl, IContent
    {
        public MovieDetails()
        {
            InitializeComponent();
        }

        void IContent.OnFragmentNavigation(FirstFloor.ModernUI.Windows.Navigation.FragmentNavigationEventArgs e)
        {
        }

        void IContent.OnNavigatedFrom(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
        }

        void IContent.OnNavigatedTo(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
            if ((this.DataContext as MovieDetailsViewModel).MovieId == -1)
            {
                (Application.Current.MainWindow as MainWindow).ContentSource = new Uri("/Views/Home.xaml", UriKind.Relative);
                ModernDialog.ShowMessage("Please select a movie first!", "No movie selected.", MessageBoxButton.OK);
            }
        }

        void IContent.OnNavigatingFrom(FirstFloor.ModernUI.Windows.Navigation.NavigatingCancelEventArgs e)
        {
        }
    }

    public class QualityToRadioButtonConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.Equals(parameter);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //throw new NotImplementedException();
            return value.Equals(true) ? parameter : Binding.DoNothing;
        }
    }

    public class CapitalizeLanguageConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string language = (string)value;
            language = language.Insert(0, Char.ToUpper(language[0]).ToString());
            language = language.Remove(1, 1);
            if (language.Contains("-"))
            {
                language = language.Insert(language.IndexOf("-") + 1, Char.ToUpper(language[language.IndexOf("-") + 1]).ToString());
                language = language.Remove(language.IndexOf("-") + 2, 1);
            }
            return language;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
