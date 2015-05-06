using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace DirectTorrent.Data.YifySubtitles.ApiWrapper
{
    public static class ApiWrapper
    {
        public static async Task<ApiResponse> GetSubtitlesByImdb(string imdbCode)
        {
            try
            {
                var request = WebRequest.Create(string.Format("http://api.yifysubtitles.com/subs/{0}", imdbCode));
                using (var response = await request.GetResponseAsync())
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        return new ApiResponse(JsonConvert.DeserializeObject<ApiResponseRaw>(await reader.ReadToEndAsync()));
                    }
                }
            }
            catch (WebException)
            {
                throw new WebException("No internet connection.");
            }
        }
    }
}
