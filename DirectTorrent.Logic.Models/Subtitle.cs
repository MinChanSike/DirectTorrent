using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectTorrent.Logic.Models
{
    public class Subtitle
    {
        public int Id { get; private set; }
        public int HearingImpaired { get; private set; }
        public int Rating { get; private set; }
        public Uri Url { get; private set; }
    }

    public class SubtitleGroup
    {
        public string Language { get; private set; }
        public List<Subtitle> Subtitles { get; private set; }

        public SubtitleGroup(Data.YifySubtitles.Models.SubtitleGroup source)
        {
            SubtitleGroup temp = null;
            AutoMapper.Mapper.CreateMap<Data.YifySubtitles.Models.SubtitleGroup, SubtitleGroup>();
            AutoMapper.Mapper.CreateMap<Data.YifySubtitles.Models.Subtitle, Subtitle>();
            temp = AutoMapper.Mapper.Map<SubtitleGroup>(source);

            this.Language = temp.Language;
            this.Subtitles = temp.Subtitles;
        }

        private SubtitleGroup()
        {
            
        }
    }
}
