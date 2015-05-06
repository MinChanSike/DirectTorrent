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
        public static async Task<IEnumerable<SubtitleGroup>> GetSubtitlesByImdbCode(string imdbCode)
        {
            try
            {
                var response = await ApiWrapper.GetSubtitlesByImdb(imdbCode);
                if (response.Success && response.SubtitleCount > 0)
                    return response.SubtitleGroups.Select(x => new SubtitleGroup(x));
            }
            catch { }

            return new List<SubtitleGroup>();
        }
    }
}
