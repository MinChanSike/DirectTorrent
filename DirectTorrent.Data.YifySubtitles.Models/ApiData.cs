using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DirectTorrent.Data.YifySubtitles.Models
{
    public class Subtitle
    {
        //id
        public int Id { get; private set; }
        //hi
        public int HearingImpaired { get; private set; }
        //rating
        public int Rating { get; private set; }
        //url
        public Uri Url { get; private set; }

        [JsonConstructor]
        internal Subtitle(int id, int hi, int rating, string url)
        {
            this.Id = id;
            this.HearingImpaired = hi;
            this.Rating = rating;
            this.Url = new Uri("http://www.yifysubtitles.com/subtitle-api" + url, UriKind.Absolute);
        }
    }

    public class SubtitleGroup
    {
        public string Language { get; private set; }
        public List<Subtitle> Subtitles { get; private set; }

        public SubtitleGroup(string language, IEnumerable subtitles)
        {
            this.Subtitles = new List<Subtitle>();
            this.Language = language;
            foreach (JObject subtitle in subtitles)
            {
                this.Subtitles.Add(subtitle.ToObject<Subtitle>());
            }
        }
    }
}
