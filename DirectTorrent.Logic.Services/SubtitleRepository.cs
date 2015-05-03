using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DirectTorrent.Data.YifySubtitles.ApiWrapper;
using DirectTorrent.Logic.Models;

namespace DirectTorrent.Logic.Services
{
    public static class SubtitleRepository
    {
        public static List<SubtitleGroup> GetSubtitlesByImdbCode(string imdbCode)
        {
            var temp = new List<SubtitleGroup>();
            var source = ApiWrapper.GetSubtitlesByImdb(imdbCode);
            source.SubtitleGroups.ForEach(x=>temp.Add(new SubtitleGroup(x)));
            return temp;
        }
    }
}
