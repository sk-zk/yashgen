using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yashgen
{
    /// <summary>
    /// youtube-dl handler
    /// </summary>
    class YoutubeDl
    {
        private readonly static string OUTPUT_FOLDER = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "yashgen");

        private static string errors = "";

        /// <summary>
        /// Downloads an audio track from a YouTube video as wav.
        /// </summary>
        /// <param name="videoId">The ID of the video.</param>
        /// <returns>The path to the downloaded file.</returns>
        public static string CallYoutubeDl(string videoId, string ydlPath, 
            bool forceIpv6, bool verbose)
        {
            errors = "";
            var tempFile = Path.Combine(OUTPUT_FOLDER, videoId + ".wav");
            
            // workaround for "invalid retry count" bug with leading dashes in IDs
            var videoUrl = $"https://www.youtube.com/watch?v={videoId}";
            
            var info = new ProcessStartInfo(ydlPath, 
                 $"--no-progress " +
                 $"-f bestaudio -x --audio-format wav --add-metadata " +
                 $"-o {OUTPUT_FOLDER}/%(id)s.%(ext)s \"{videoUrl}\""
                );

            if (forceIpv6) 
                info.Arguments += " -6";

            if (verbose)
            {
                Console.WriteLine("videoUrl: " + videoUrl);
                Console.WriteLine("args: " + info.Arguments);
            }

            info.CreateNoWindow = true;
            info.UseShellExecute = false;
            info.RedirectStandardOutput = true;
            info.RedirectStandardError = true;
            var process = new Process();
            process.StartInfo = info;
            if (verbose)
            {
                process.OutputDataReceived += (_, e) => Console.WriteLine(e.Data);
            }
            process.ErrorDataReceived += ErrorDataReceived;
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
            process.Dispose();

            if (!string.IsNullOrEmpty(errors))
                throw new YoutubeDlException(errors);

            return tempFile;
        }

        public static void PrintVersion(string ydlPath)
        {
            var info = new ProcessStartInfo(ydlPath, "--version");
            info.CreateNoWindow = true;
            info.UseShellExecute = false;
            var process = new Process();
            process.StartInfo = info;
            process.Start();
            process.WaitForExit();
            process.Dispose();
        }

        private static void ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data?.Trim()))
                errors += e.Data + "\n";
        }

    }

}
