using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;

namespace yashgen
{
    class Program
    {
        static SongLoader songLoader = new SongLoader();

        private const int ExitUnspecified = -1;
        private const int ExitInvalidId = 1;
        private const int ExitYoutubeDlError = 2;
        private const int ExitNoArgs = 3;

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("yashgen video_id destination [-6]");
                Environment.Exit(ExitNoArgs);
            }

            var id = args[0];
            var path = args.Length > 1 ? args[1] : "";

            // force ytdl to use ipv6
            // fixes "This video is not available" for auto-generated videos
            var ipv6 = args.Contains("-6");
            
            #if DEBUG
                YoutubeDl.DebugPrintVersion();
            #endif

            try
            {
                if (IsYoutubeId(id))
                {
                    ProcessVideo(id, path, ipv6);
                    Console.WriteLine("Done\n");
                }
                else
                {
                    Console.Error.WriteLine("\"{0}\" doesn't appear to be a valid ID\n", id);
                    Environment.Exit(ExitInvalidId);
                }
            }
            catch (YoutubeDlException yex)
            {
                Console.Error.WriteLine("youtube-dl encountered an error:");
                Console.Error.WriteLine(yex.Message);
                Environment.Exit(ExitYoutubeDlError);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Something went wrong:");
                Console.Error.WriteLine(ex.ToString());
                Environment.Exit(ExitUnspecified);
            }
           
        }

        static bool IsYoutubeId(string input)
        {
            return Regex.IsMatch(input, "^[a-zA-Z0-9_-]{11}$");
        }

        static void ProcessVideo(string videoId, string path, bool ipv6 = false)
        {
            CreateAndSaveYash(videoId, path, ipv6);
        }

        static void CreateAndSaveYash(string videoId, string path, bool ipv6 = false)
        {
            Console.WriteLine("Processing {0} now", videoId);
            Console.WriteLine("Downloading audio");
            string ytAudioFile;
            try
            {
                ytAudioFile = YoutubeDl.CallYoutubeDl(videoId, ipv6);
            }
            catch (YoutubeDlException)
            {
                throw;
            }

            Console.WriteLine("Analyzing song");
            TagLib.File file = TagLib.File.Create(ytAudioFile);
            float duration = (float)file.Properties.Duration.TotalSeconds;
            file.Dispose();
            List<float> sums = songLoader.DecodeSongSums(ytAudioFile, duration);

            Console.WriteLine("Saving yash");
            try
            {
                var filename = $"youtube_{videoId}.yash";
                SaveYash(sums, duration, Path.Combine(path, filename));
            }
            catch (IOException)
            {
                throw;
            }

            try
            {
                File.Delete(ytAudioFile);
            }
            catch (IOException)
            {
                // oh well.
            }
        }

        static void SaveYash(List<float> sums, float duration, string path)
        {
            const int HEADER_SIZE = 3 * 4;
            var sumBytes = new byte[HEADER_SIZE + (sums.Count() * 4)];
            Buffer.BlockCopy(BitConverter.GetBytes(1), 0, sumBytes, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(duration), 0, sumBytes, 1 * 4, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(sums.Count), 0, sumBytes, 2 * 4, 4);
            Buffer.BlockCopy(sums.ToArray(), 0, sumBytes, 3 * 4, sums.Count * 4);

            File.WriteAllBytes(path, sumBytes);
        }

    }
}
