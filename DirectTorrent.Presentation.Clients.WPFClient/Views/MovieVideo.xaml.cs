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
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;

namespace DirectTorrent.Presentation.Clients.WPFClient.Views
{
    /// <summary>
    /// Interaction logic for MovieVideo.xaml
    /// </summary>
    public partial class MovieVideo : ModernWindow
    {
        private string guid;
        private bool IsPlaying = false;
        private bool first = true;
        private DispatcherTimer timer;
        public MovieVideo()
        {
            InitializeComponent();
            Messenger.Default.Register<int>(this, "runtime", r => { this.Slider.Maximum = r * 60; });
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
                    ((MovieVideoViewModel)this.DataContext).Position = (int)this.Player.Position.TotalSeconds;
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
            ((MovieVideoViewModel)this.DataContext).LoaderVisibility = Visibility.Collapsed;
            ((MovieVideoViewModel)this.DataContext).MovieVisibility = Visibility.Visible;
            //this.Slider.Maximum = Data.Runtime * 60;
            timer.Start();
        }

        private void ModernWindow_Closing(object sender, CancelEventArgs e)
        {
            NodeServerManager.CloseServer();
            ((MovieVideoViewModel)this.DataContext).UnregisterViewModel();
            Messenger.Default.Unregister(this);
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

    public class DoubleToTimespan : IValueConverter
    {

        object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int dabl = (int)value;
            return TimeSpan.FromSeconds(dabl);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ListToString : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((List<string>)value).Aggregate((f, s) => f + Environment.NewLine + s);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class VideoHeightToFontSize : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Math.Max((double)value / 12, 12);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class HtmlParser
    {
        public static readonly DependencyProperty HtmlProperty = DependencyProperty.RegisterAttached(
            "Html",
            typeof(string),
            typeof(HtmlParser),
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsMeasure, HtmlPropertyChanged));

        public static void SetHtml(DependencyObject textBlock, string value)
        {
            textBlock.SetValue(HtmlProperty, value);
        }

        public static string GetHtml(DependencyObject textBlock)
        {
            return (string)textBlock.GetValue(HtmlProperty);
        }

        private static void HtmlPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textBlock = d as TextBlock;
            if (textBlock == null)
                return;

            var html = (string)e.NewValue ?? string.Empty;

            textBlock.Inlines.Clear();
            if (!string.IsNullOrEmpty(html))
                textBlock.Inlines.AddRange(ParseHtml(html));
        }

        private static IEnumerable<Inline> ParseHtml(string html)
        {
            int offset = 0;
            string text = "";
            string tag = "";

            while (offset < html.Length)
            {
                if (html[offset] == '<')
                {
                    if (tag == "" && text.Length > 0)
                    {
                        yield return new Run(text);
                        text = "";
                    }

                    offset++;
                    if (html[offset] == '/')
                    {
                        offset++;
                        tag = new String(html.Skip(offset).TakeWhile(c => c != '>').ToArray());
                        offset += tag.Length + 1;

                        yield return CreateInlineByTag(tag, text);

                        text = "";
                        tag = "";
                    }
                    else
                    {
                        tag = new String(html.Skip(offset).TakeWhile(c => c != '>').ToArray());
                        offset += tag.Length + 1;
                    }
                    continue;
                }

                text += html[offset++];
            }

            if (tag == "" && text.Length > 0)
            {
                yield return new Run(text);
            }
        }

        static Inline CreateInlineByTag(string tag, string text)
        {
            switch (tag[0])
            {
                case 'u':
                    return new Underline(new Run(text));

                case 'i':
                    return new Italic(new Run(text));

                case 'b':
                    return new Bold(new Run(text));

                default:
                    return new Run(text);
            }
        }
    }
}
