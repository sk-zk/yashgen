﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;
using Mono.Options;

namespace yashgen
{
    class Program
    {
        private static SongLoader songLoader = new SongLoader();

        private const int ExitUnspecified = -1;
        private const int ExitInvalidId = 1;
        private const int ExitYoutubeDlError = 2;
        private const int ExitNoArgs = 3;

        static void Main(string[] args)
        {
            string id = "";
            string destination = ".";
            string ydlPath = "./youtube-dl";
            bool forceIpv6 = false;
            bool verbose = false;

            var p = new OptionSet()
            {
                { "d=|dest=",
                    $"The output folder.\nDefault: {destination}", 
                    x => { destination = x; } },
                { "p=|proc=", 
                    $"Path to youtube-dl or a compatible fork.\nDefault: {ydlPath}", 
                    x => { ydlPath = x; } },
                { "6", 
                    "Forces the downloader to use IPv6.", 
                    x => { forceIpv6 = true; } },
                { "v|verbose",
                    "Displays downloader console output.",
                    x => { verbose = true; }},
            };

            if (args.Length == 0)
            {
                PrintUsage(p);
            }

            id = args[0];
            p.Parse(args.Skip(1));

            #if DEBUG
                YoutubeDl.PrintVersion(ydlPath);
            #endif

            try
            {
                if (IsYoutubeId(id))
                {
                    CreateAndSaveYash(id, destination, ydlPath, forceIpv6, verbose);
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

        private static void PrintUsage(OptionSet p)
        {
            Console.WriteLine("yashgen video_id [options]\n");
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
            Environment.Exit(ExitNoArgs);
        }

        static bool IsYoutubeId(string input) 
            => Regex.IsMatch(input, "^[a-zA-Z0-9_-]{11}$");

        static void CreateAndSaveYash(string videoId, string destination, string ydlPath,
            bool forceIpv6, bool verbose)
        {
            Console.WriteLine("Processing {0}", videoId);
            Console.WriteLine("Downloading audio");
            string ytAudioFile;
            try
            {
                ytAudioFile = YoutubeDl.CallYoutubeDl(videoId, ydlPath, forceIpv6, verbose);
            } 
            catch (YoutubeDlException)
            {
                throw;
            }

            Console.WriteLine("Analyzing song");
            float duration;
            using (var file = TagLib.File.Create(ytAudioFile))
            {
                duration = (float)file.Properties.Duration.TotalSeconds;
            }
            var sums = songLoader.DecodeSongSums(ytAudioFile);

            Console.WriteLine("Saving yash");
            try
            {
                var filename = $"youtube_{videoId}.yash";
                SaveYash(sums, duration, Path.Combine(destination, filename));
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
