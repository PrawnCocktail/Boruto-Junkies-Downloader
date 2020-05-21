using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto_Junkies
{
    class Player
    {
        public class Json
        {
            public Hostlist hostList { get; set; }
            public string videoUrl { get; set; }
            public string videoServer { get; set; }
            public string videoDisk { get; set; }
            public string videoPlayer { get; set; }
            public bool isJWPlayer8 { get; set; }
            public string jwPlayerKey { get; set; }
            public string jwPlayerURL { get; set; }
            public Logo logo { get; set; }
            public object[] tracks { get; set; }
            public Captions captions { get; set; }
            public string defaultImage { get; set; }
            public bool SubtitleManager { get; set; }
            public bool jwplayer8button1 { get; set; }
            public bool jwplayer8quality { get; set; }
            public string title { get; set; }
            public bool displaytitle { get; set; }
            public bool rememberPosition { get; set; }
            public Advertising advertising { get; set; }
            public Videodata videoData { get; set; }
        }

        public class Hostlist
        {
            [JsonProperty(PropertyName = "1")]
            public string[] One { get; set; }
        }

        public class Logo
        {
            public string file { get; set; }
            public string link { get; set; }
            public string position { get; set; }
            public bool hide { get; set; }
        }

        public class Captions
        {
            public string fontSize { get; set; }
            public string fontfamily { get; set; }
        }

        public class Advertising
        {
            public string client { get; set; }
            public string tag { get; set; }
        }

        public class Videodata
        {
            public object videoImage { get; set; }
            public Videosource[] videoSources { get; set; }
        }

        public class Videosource
        {
            public string file { get; set; }
            public string label { get; set; }
            public string type { get; set; }
        }

    }

    public class Streams
    {
        public int Bandwidth { get; set; }
        public string Resolution { get; set; }
        public string Playlist { get; set; }
    }

    public class PlayerParseReturn
    {
        public string playlistUrl { get; set; }
        public string videoTitle { get; set; }
    }

    public class VideoInfo
    {
        public class Streams
        {
            public int Bandwidth { get; set; }
            public string Resolution { get; set; }
            public string Playlist { get; set; }
        }

        public class Video
        {
            public string VideoName { get; set; }
            public string PlayerUrl { get; set; }
            public List<Streams> Streams { get; set; }
        }

        public class Chunks
        {
            public string Header { get; set; }
            public List<string> Streams { get; set; }
        }
    }
}
