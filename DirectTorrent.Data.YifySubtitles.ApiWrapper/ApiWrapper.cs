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
        public static ApiResponse GetSubtitlesByImdb(string imdbCode)
        {
            Stream stream;
            try
            {
                stream =
                    WebRequest.Create(string.Format("http://api.yifysubtitles.com/subs/{0}", imdbCode))
                        .GetResponse()
                        .GetResponseStream();
                using (StreamReader sr = new StreamReader(stream))
                {
                    return new ApiResponse(JsonConvert.DeserializeObject<ApiResponseRaw>(sr.ReadToEnd()));
                }
            }
            catch (WebException)
            {
                throw new WebException("No internet connection.");
            }
        }
    }
}
