﻿using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Net.Sockets;

using System.Reflection;
using System.Resources;
using Newtonsoft.Json;
using DirectTorrent.Data.Yify.Models;
using System.Threading.Tasks;

namespace DirectTorrent.Data.Yify.ApiWrapper
{
    /// <summary>
    /// Represents a wrapper around the Yify /api/v2 APIs.
    /// </summary>
    public static class ApiWrapper
    {
        #region Enum Parsers

        // Parsers for parameter value enforcing enums
        private static string ParseFormat(Format format)
        {
            switch (format)
            {
                case Format.JSON:
                    return "json";
                case Format.JSONP:
                    return "jsonp";
                case Format.XML:
                    return "xml";
            }
            throw new ArgumentOutOfRangeException("format");
        }

        private static string ParseQuality(Quality quality)
        {
            switch (quality)
            {
                case Quality.HD:
                    return "720p";
                case Quality.FHD:
                    return "1080p";
                case Quality.ThreeD:
                    return "3D";
                case Quality.ALL:
                    return "ALL";
            }
            throw new ArgumentOutOfRangeException("quality");
        }

        private static string ParseSort(Sort sort)
        {
            switch (sort)
            {
                case Sort.Title:
                    return "title";
                case Sort.Year:
                    return "year";
                case Sort.Rating:
                    return "rating";
                case Sort.Peers:
                    return "peers";
                case Sort.Seeds:
                    return "seeds";
                case Sort.DownloadCount:
                    return "downloaded_count";
                case Sort.LikeCount:
                    return "like_count";
                case Sort.DateAdded:
                    return "date_added";
            }
            throw new ArgumentOutOfRangeException("sort");
        }

        private static string ParseOrder(Order order)
        {
            switch (order)
            {
                case Order.Ascending:
                    return "asc";
                case Order.Descending:
                    return "desc";
            }
            throw new ArgumentOutOfRangeException("order");
        }

        #endregion

        #region API Methods

        /// <summary>
        /// Provides an interface to the /api/v2/list_upcoming yify API.
        /// </summary>
        /// <param name="format">Sets the format in which to display the results in. DO NOT USE ANYTHING OTHER THAN JSON! (Experimental)</param>
        /// <returns>The ApiResponse representing the query result.</returns>
        public static Task<ApiResponse<UpcomingMoviesData>> ListUpcomingMovies(Format format = Format.JSON)
        {
            var query = string.Format("https://yts.to/api/v2/list_upcoming.{0}", ParseFormat(format));

            return Get<UpcomingMoviesData>(query);
        }

        /// <summary>
        /// Provides an interface to the /api/v2/list_movies yify API.
        /// </summary>
        /// <param name="format">Sets the format in which to display the results in. DO NOT USE ANYTHING OTHER THAN JSON! (Experimental)</param>
        /// <param name="limit">Sets the max amount of movie results (1-50, inclusive).</param>
        /// <param name="page">Sets the set of movies to display (eg limit=15 and set=2 will show you movies 15-30).</param>
        /// <param name="quality">Sets a quality type to filter by.</param>
        /// <param name="minimumRating">Sets minimum movie rating for display (0-9, inclusive).</param>
        /// <param name="queryTerm">Sets the keywords to search by (maybe be multiple keywords, eg. britney, spears).</param>
        /// <param name="genre">Sets the genre from which to display movies from.</param>
        /// <param name="sortBy">Sets the sorting parameter.</param>
        /// <param name="orderBy">Sets the order in which the movies will be displayed.</param>
        /// <exception cref="ArgumentOutOfRangeException">Limit or rating values were out of range.</exception>
        /// <returns>The ApiResponse representing the query result.</returns>
        public static Task<ApiResponse<ListMoviesData>> ListMovies(Format format = Format.JSON, byte limit = 20, uint page = 1,
            Quality quality = Quality.ALL, byte minimumRating = 0, string queryTerm = "", string genre = "ALL",
            Sort sortBy = Sort.DateAdded, Order orderBy = Order.Descending)
        {
            // Parameter value range checking
            if (limit > 50 || limit < 1)
                throw new ArgumentOutOfRangeException("limit", limit, "Limit must be between 1 - 50 (inclusive).");
            
            if (minimumRating > 9)
                throw new ArgumentOutOfRangeException("minimumRating", minimumRating,
                    "Must be between 0 - 9 (inclusive).");
            
            // Forming the request string
            string query = string.Format(
                    "limit={0}&page={1}&quality={2}&minimum_rating={3}&query_term={4}&genre={5}&sort_by={6}&order_by={7}",
                    limit, page, ParseQuality(quality), minimumRating, queryTerm, genre, ParseSort(sortBy), ParseOrder(orderBy));

            return Get<ListMoviesData>(string.Format("https://yts.to/api/v2/list_movies.{0}?{1}", ParseFormat(format), query));
        }

        /// <summary>
        /// Provides an interface to the /api/v2/movie_details yify API.
        /// </summary>
        /// <param name="movieId">Sets the id of the movie details which are to be queried.</param>
        /// <param name="withImages">Sets wether the response should hold images.</param>
        /// <param name="withCast">Sets wether the response should hold information about the cast.</param>
        /// <param name="format">Sets the format in which to display the results in. DO NOT USE ANYTHING OTHER THAN JSON! (Experimental)</param>
        /// <returns>The ApiResponse representing the query result.</returns>
        public static Task<ApiResponse<MovieDetailsData>> GetMovieDetails(int movieId, bool withImages = false, bool withCast = false,
            Format format = Format.JSON)
        {
            var query = string.Format("movie_id={0}&with_images={1}&with_cast={2}", movieId, withImages, withCast);

            return Get<MovieDetailsData>(string.Format("https://yts.to/api/v2/movie_details.{0}?{1}", ParseFormat(format), query));
        }

        private static async Task<ApiResponse<T>> Get<T>(string uri) where T : IDataModel
        {
            try
            {
                var request = WebRequest.Create(uri);
                using (var response = await request.GetResponseAsync())
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        return new ApiResponse<T>(JsonConvert.DeserializeObject<ApiResponseRaw>(await reader.ReadToEndAsync()));
                    }
                }
            }
            catch (WebException)
            {
                // No internet connection
                throw new Exception("No internet connection.");
            }
        }

        // Testing method
        public static ApiResponse<ListMoviesData> DummyMovieData()
        {
            StreamReader _textStreamReader;

            try
            {
                var _assembly = Assembly.GetExecutingAssembly();
                _textStreamReader =
                    new StreamReader(_assembly.GetManifestResourceStream("DirectTorrent.Data.Yify.ApiWrapper.list_movies.json"));
            }
            catch
            {
                throw new MissingManifestResourceException("Missing the dummy movie resource.");
            }
            System.Diagnostics.Stopwatch timer = System.Diagnostics.Stopwatch.StartNew();
            var text = _textStreamReader.ReadToEnd();
            Debug.WriteLine("Reading file: " + timer.Elapsed);
            var temp =
                new ApiResponse<ListMoviesData>(
                    JsonConvert.DeserializeObject<ApiResponseRaw>(text));
            Debug.WriteLine("Parsing JSON: " + timer.Elapsed);
            timer.Stop();
            return temp;
        }

        #endregion
    }
}