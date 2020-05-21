using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Boruto_Junkies
{
    class Program
    {
        static ProgressBar progress;

        static void Main(string[] args)
        {
            string url = "";
            //if no arguments then prompt for url and assign. no checks in place though
            if (args.Length == 0)
            {
                Console.WriteLine("Please enter a url.");
                url = Console.ReadLine();
            }
            // if arguments is equal to 1, then assume its a url, again, no checks.
            else if (args.Length == 1)
            {
                url = args[0];
            }
            //if more than one argument thow this message and close
            else 
            {
                Console.WriteLine("To many arguments, please enter ONE url.");
                Console.WriteLine("Press any key to exit");
                Console.ReadLine();
                Environment.Exit(0);
            }

            var mainVideoUrls = parseUrl(url);

            foreach (var vidurl in mainVideoUrls)
            {
                VideoInfo.Video video = new VideoInfo.Video();
                string playerUrl = getPlayerUrl(vidurl);
                PlayerParseReturn playerInfo = parsePlayer(playerUrl);
                video.VideoName = playerInfo.videoTitle;
                video.Streams = masterPlaylist(playerInfo.playlistUrl, playerUrl);
                video.PlayerUrl = playerUrl;
                Console.WriteLine("Downloading; " + playerInfo.videoTitle);
                downloadVideo(video);
            }
        }

        //main series url
        static List<string> parseUrl(string url)
        {
            List<string> tmpurls = new List<string>();
            var web = new HtmlWeb();
            var doc = web.Load(url);
            var vhxembed = doc.DocumentNode.SelectNodes("//table[@class='ani-folgen']//tbody//tr[@class='mediaitem']//td[2]//a");

            foreach (var item in vhxembed)
            {
                var vidurl = item.Attributes["href"].Value;
                tmpurls.Add(vidurl);
            }
            return tmpurls;
        }

        //video page url
        static string getPlayerUrl(string url)
        {
            var web = new HtmlWeb();
            var doc = web.Load(url);
            return(doc.DocumentNode.SelectSingleNode("//div[@class='GTTabs_divs GTTabs_curr_div']//iframe").GetAttributeValue("src", null));
        }

        static PlayerParseReturn parsePlayer(string url)
        {
            var web = new HtmlWeb();
            var doc = web.Load(url);

            string playerjsonstring = doc.DocumentNode.SelectSingleNode("//script[contains(.,'function fireload(vhash)')]").InnerText
                .Split(new string[] { "(vhash, " }, StringSplitOptions.None)[1]
                .Split(new string[] { ", false);" }, StringSplitOptions.None)[0];

            string videoTitle = doc.DocumentNode.SelectSingleNode("//head/title").InnerText.Replace("Anime4You", "").Replace(".mp4", "");

            Player.Json playerJson = JsonConvert.DeserializeObject<Player.Json>(playerjsonstring);
            string playlistUrl = "https://anividz.org" + playerJson.videoUrl + "?s=" + playerJson.videoServer + "&d=" + Convert.ToBase64String(Encoding.UTF8.GetBytes(playerJson.videoDisk));

            PlayerParseReturn playerInfo = new PlayerParseReturn
            {
                videoTitle = videoTitle,
                playlistUrl = playlistUrl
            };

            return playerInfo;
        }

        static List<VideoInfo.Streams> masterPlaylist(string masterUrl, string playerUrl)
        {
            List<VideoInfo.Streams> progStreams = new List<VideoInfo.Streams>();

            WebClient client = new WebClient();
            client.Headers["Referer"] = playerUrl;
            client.Headers["Accept"] = "*/*";
            using (var stream = client.OpenRead(masterUrl))
            {
                string resolution = "";
                int bandwidth = 0;
                string streamUrl = "";

                string line;
                StreamReader file = new StreamReader(stream);
                while ((line = file.ReadLine()) != null)
                {
                    if (line.StartsWith("#EXT-X-STREAM-INF") || line.Contains("anividz.org"))
                    {
                        if (line.StartsWith("#EXT-X-STREAM-INF"))
                        {
                            string[] words = line.Split(',');

                            foreach (var word in words)
                            {
                                if (word.Contains("BANDWIDTH"))
                                {
                                    bandwidth = Convert.ToInt32(word.Split('=')[1]);
                                }
                                if (word.Contains("RESOLUTION"))
                                {
                                    resolution = word.Split('=')[1];
                                }
                            }
                        }
                        else if (line.Contains("anividz.org"))
                        {
                            streamUrl = line;

                            VideoInfo.Streams streams = new VideoInfo.Streams()
                            {
                                Bandwidth = bandwidth,
                                Resolution = resolution,
                                Playlist = streamUrl
                            };
                            progStreams.Add(streams);
                        }
                    }
                }
            }
            return progStreams;
        }

        static void downloadVideo(VideoInfo.Video video)
        {
            string winner = "";
            int bandwidth = 0;
            foreach (var item in video.Streams)
            {
                if (item.Bandwidth > bandwidth)
                {
                    bandwidth = item.Bandwidth;
                    winner = item.Playlist;
                }
            }

            var Chunks = videoStream(winner, video.PlayerUrl);

            try
            {
                string fileName = MakeValidFileName(video.VideoName);

                if (File.Exists(fileName + ".ts"))
                {
                    Console.WriteLine(fileName + " already downloaded, skipping.");
                    return;
                }

                using (var outputStream = File.Create(fileName + ".ts"))
                {
                    using (WebClient client = new WebClient())
                    {
                        client.Headers["Referer"] = video.PlayerUrl;
                        client.Headers["Accept"] = "*/*";

                        progress = new ProgressBar();

                        int totalTicks = Chunks.Count;
                        int currentTick = 0;

                        foreach (var chunk in Chunks)
                        {
                            progress.Report((double)currentTick / totalTicks);
                            using (MemoryStream stream = new MemoryStream(client.DownloadData(chunk)))
                            {
                                stream.CopyTo(outputStream);
                            }
                            currentTick++;
                        }
                    }
                }
            }
            catch (AmbiguousMatchException ex)
            {
                Console.WriteLine(ex);
            }

        }

        static List<string> videoStream(string videoPlaylist, string playerUrl)
        {
            List<string> tsChunks = new List<string>();

            using (var client = new WebClient())
            {
                client.Headers["Referer"] = playerUrl;
                client.Headers["Accept"] = "*/*";

                Stream playlist = client.OpenRead(videoPlaylist);
                StreamReader file = new StreamReader(playlist);
                
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    if (line.Contains(".jpg"))
                    {
                        tsChunks.Add(line);
                    }
                }
                file.Close();
            }
            return tsChunks;
        }

        private static string MakeValidFileName(string name)
        {
            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return System.Text.RegularExpressions.Regex.Replace(name, invalidRegStr, "");
        }
    }
}