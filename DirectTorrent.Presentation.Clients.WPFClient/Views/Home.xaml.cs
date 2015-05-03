using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using DirectTorrent.Logic.Models;
using DirectTorrent.Presentation.Clients.WPFClient.ViewModels;
using FirstFloor.ModernUI.Presentation;
using FirstFloor.ModernUI.Windows;
using GalaSoft.MvvmLight.Messaging;

namespace DirectTorrent.Presentation.Clients.WPFClient.Views
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : UserControl
    {
        public Home()
        {
            InitializeComponent();
        }

        private void userControl_Loaded(object sender, RoutedEventArgs e)
        {
            //CbQuality.ItemsSource = Enum.GetValues(typeof(Quality)).Cast<Quality>().ToList();
            CbSort.ItemsSource = Enum.GetValues(typeof(Sort)).Cast<Sort>().ToList();
            CbOrder.ItemsSource = Enum.GetValues(typeof(Order)).Cast<Order>().ToList();
        }

        private void TbQuery_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                ((TextBox)sender).MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
        }
    }

    public class EnumStringConverter : IValueConverter
    {

        object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string enumString = value.ToString();
            for (int i = 1; i < enumString.Length; i++)
            {
                if (Char.IsUpper(enumString[i]))
                    return enumString.Insert(i, " ");
            }
            return value;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
