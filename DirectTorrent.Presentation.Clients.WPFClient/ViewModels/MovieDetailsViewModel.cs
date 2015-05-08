using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

using DirectTorrent.Logic.Models;
using DirectTorrent.Logic.Services;
using DirectTorrent.Presentation.Clients.WPFClient.Views;
using FirstFloor.ModernUI.Presentation;
using FirstFloor.ModernUI.Windows.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using MovieDetails = DirectTorrent.Logic.Models.MovieDetails;

namespace DirectTorrent.Presentation.Clients.WPFClient.ViewModels
{

    public class MovieDetailsViewModel : ViewModelBase
    {
        public enum TorrentHealth
        {
            Bad,
            Good,
            Excellent
        };

        //public enum ReproductionMethod
        //{
        //    WMP,
        //    Browser,
        //    Local
        //};

        private BitmapImage _movieImage;
        private string _movieTitle;
        private string _movieDescription;
        private int _movieYear;
        private int _movieDuration;
        private string _movieGenre;
        private Uri _imdbLink;
        private double _movieRating;
        private string _magnetUri;
        private Visibility _loaderVisibility = Visibility.Visible;
        private Visibility _movieVisibility = Visibility.Collapsed;
        private TorrentHealth _movieHealth;
        private bool _hasFhd = false;
        private Quality _selectedQuality = Quality.HD;
        private Torrent[] torrents = new Torrent[3];
        private List<SubtitleGroup> _subtitles = new List<SubtitleGroup>();
        private SubtitleGroup _selectedSubtitle;
        private Visibility _subtitleVisibility = Visibility.Collapsed;

        public int MovieId { get; private set; }

        public GalaSoft.MvvmLight.CommandWpf.RelayCommand PlayButtonClicked { get; private set; }

        public string MovieTitle
        {
            get { return this._movieTitle; }
            private set { this.Set(ref this._movieTitle, value); }
        }

        public string MovieDescription
        {
            get { return this._movieDescription; }
            private set { this.Set(ref this._movieDescription, value); }
        }
        public int MovieYear
        {
            get { return this._movieYear; }
            private set { this.Set(ref this._movieYear, value); }
        }
        public int MovieDuration
        {
            get { return this._movieDuration; }
            private set { this.Set(ref this._movieDuration, value); }
        }
        public double MovieRating
        {
            get { return this._movieRating; }
            private set { this.Set(ref this._movieRating, value); }
        }
        public string MovieGenre
        {
            get { return this._movieGenre; }
            private set { this.Set(ref this._movieGenre, value); }
        }
        public BitmapImage MovieImage
        {
            get { return this._movieImage; }
            private set { this.Set(ref this._movieImage, value); }
        }
        public Uri ImdbLink
        {
            get { return this._imdbLink; }
            private set { this.Set(ref this._imdbLink, value); }
        }
        public Visibility LoaderVisibility
        {
            get { return this._loaderVisibility; }
            private set { this.Set(ref this._loaderVisibility, value); }
        }
        public Visibility MovieVisibility
        {
            get { return this._movieVisibility; }
            private set { this.Set(ref this._movieVisibility, value); }
        }
        public TorrentHealth MovieHealth
        {
            get { return this._movieHealth; }
            private set { this.Set(ref this._movieHealth, value); }
        }
        public bool HasFhd
        {
            get { return this._hasFhd; }
            private set { this.Set(ref this._hasFhd, value); }
        }
        public Quality SelectedQuality
        {
            get { return this._selectedQuality; }
            private set
            {
                // TODO: Set torrent health via messenger?
                if (this.Set(ref this._selectedQuality, value))
                {
                    if (value == Quality.HD)
                        SetTorrentHealth(0);
                    else if (value == Quality.FHD)
                        SetTorrentHealth(1);
                }
            }
        }
        public List<SubtitleGroup> Subtitles
        {
            get { return this._subtitles; }
            private set { this.Set(ref this._subtitles, value); }
        }
        public Visibility SubtitleVisibility
        {
            get { return this._subtitleVisibility; }
            private set { this.Set(ref this._subtitleVisibility, value); }
        }

        public SubtitleGroup SelectedSubtitle
        {
            get { return this._selectedSubtitle; }
            set { this.Set(ref this._selectedSubtitle, value); }
        }

        public MovieDetailsViewModel(/*int movieId*/)
        {
            this.MovieId = -1;

            Messenger.Default.Register<int>(this, "movieId", async (id) =>
            {
                this.MovieId = id;
                await LoadMovie();
            });

            this.PlayButtonClicked = new GalaSoft.MvvmLight.CommandWpf.RelayCommand(() =>
            {
                string magnetUri = String.Empty;
                switch (SelectedQuality)
                {
                    case Quality.HD:
                        _magnetUri = MovieRepository.GetTorrentMagnetUri(this.torrents[0].Hash, this.MovieTitle);
                        break;
                    case Quality.FHD:
                        _magnetUri = MovieRepository.GetTorrentMagnetUri(this.torrents[1].Hash, this.MovieTitle);
                        break;
                }
                var wind = new MovieVideo();
                wind.Title = this.MovieTitle + " (" + this.MovieYear + ")";
                Messenger.Default.Send<int>(this.MovieDuration, "runtime");
                Messenger.Default.Send<string>(this._magnetUri, "magnetUri");
                if (this.Subtitles.Count != 0)
                {
                    var subUrl = this.SelectedSubtitle.Subtitles.Aggregate((i1, i2) => i1.Rating > i2.Rating ? i1 : i2).Url.ToString();
                    Messenger.Default.Send<string>(subUrl, "subtitle");
                }
                wind.ShowDialog();
            });
        }

        private async Task LoadMovie()
        {
            this.LoaderVisibility = Visibility.Visible;
            this.MovieVisibility = Visibility.Collapsed;
            this.SubtitleVisibility = Visibility.Collapsed;

            try
            {
                var movie = await MovieRepository.Yify.GetMovieDetails(MovieId);

                this.MovieTitle = movie.Title;
                this.MovieDescription = movie.DescriptionFull;
                this.MovieYear = movie.Year;
                this.MovieDuration = movie.Runtime;
                this.MovieRating = movie.Rating;
                StringBuilder genres = new StringBuilder();
                movie.Genres.ForEach(x => genres.Append(x + "/"));
                var genre = genres.ToString();
                genre = genre.Remove(genre.Length - 1);
                this.MovieGenre = genre;
                //Messenger.Default.Send(movie.Runtime, "runtime");
                //Data.Runtime = movie.Runtime;
                this.MovieImage = new BitmapImage(new Uri(movie.Images.LargeCoverImage, UriKind.Absolute));
                this.ImdbLink = new Uri("http://www.imdb.com/title/" + movie.ImdbCode + "/", UriKind.Absolute);
                movie.Torrents.CopyTo(this.torrents, 0);
                if (movie.Torrents.Count > 1)
                {
                    this.HasFhd = true;
                    this.SelectedQuality = Quality.FHD;
                    SetTorrentHealth(1);
                }
                else
                    SetTorrentHealth(0);

                this.LoaderVisibility = Visibility.Collapsed;
                this.MovieVisibility = Visibility.Visible;

                await LoadSubtitles(movie);
            }
            catch (KeyNotFoundException) { }
        }

        private async Task LoadSubtitles(MovieDetails movie)
        {
            Subtitles.Clear();
            try
            {
                Subtitles.AddRange((await SubtitleRepository.GetSubtitlesByImdbCode(movie.ImdbCode)).OrderBy(x => x.Language));
            }
            catch (NullReferenceException) { }
            catch (KeyNotFoundException) { }
            catch { }
            finally
            {
                SelectedSubtitle = Subtitles.SingleOrDefault(x => x.Language.Equals(System.Globalization.CultureInfo.CurrentUICulture.EnglishName.Split(',', ' ').First(), StringComparison.InvariantCultureIgnoreCase)) ?? Subtitles.FirstOrDefault();
                SubtitleVisibility = (Subtitles.Count > 0) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void SetTorrentHealth(int torrentId)
        {
            double ratio = 0;
            try
            {
                ratio = this.torrents[torrentId].Seeds / this.torrents[torrentId].Peers;
            }
            catch
            {

            }
            if (ratio < 1)
                this.MovieHealth = TorrentHealth.Bad;
            else if (ratio >= 1 && ratio <= 1.5)
                this.MovieHealth = TorrentHealth.Good;
            else if (ratio > 1.5)
                this.MovieHealth = TorrentHealth.Excellent;
        }
    }
}
