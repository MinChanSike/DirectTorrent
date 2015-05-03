using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DirectTorrent.Data.YifySubtitles.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DirectTorrent.Data.YifySubtitles.ApiWrapper
{
    internal class ApiResponseRaw
    {
        [JsonProperty(PropertyName = "success")]
        internal bool Success { get; set; }
        [JsonProperty(PropertyName = "lastModified")]
        internal int LastModified { get; set; }
        [JsonProperty(PropertyName = "subtitles")]
        internal int SubtitleCount { get; set; }
        [JsonProperty(PropertyName = "subs")]
        internal JObject Subtitles { get; set; }
    }

    public class ApiResponse
    {
        public bool Success { get; private set; }
        public int LastModified { get; private set; }
        public int SubtitleCount { get; private set; }
        public List<SubtitleGroup> SubtitleGroups { get; private set; }

        internal ApiResponse(ApiResponseRaw rawResponse)
        {
            this.SubtitleGroups = new List<SubtitleGroup>();
            JObject subs;
            this.Success = rawResponse.Success;
            this.LastModified = rawResponse.LastModified;
            this.SubtitleCount = rawResponse.SubtitleCount;
            try
            {
                subs = rawResponse.Subtitles.First.First as JObject;
            }
            catch (Exception)
            {
                throw new KeyNotFoundException("No subtitles were found.");
            }
            
            var langs = subs.Properties().Select(x => x.Name).ToList();
            foreach (var lang in langs)
            {
                SubtitleGroups.Add(new SubtitleGroup(lang, subs[lang.ToString()].ToList()));
            }
        }
    }
}
