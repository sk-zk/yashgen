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
        readonly static string OUTPUT_FOLDER = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "yashgen");

        static string errors = "";

        /// <summary>
        /// Downloads an audio track from a YouTube video as m4a.
        /// </summary>
        /// <param name="videoId">The ID of the video.</param>
        /// <returns>The path to the downloaded file.</returns>
        public static string CallYoutubeDl(string videoId, bool ipv6 = false)
        {
            errors = "";
            string tempFile = Path.Combine(OUTPUT_FOLDER, videoId + ".m4a");

            // workaround for "invalid retry count" bug with leading dashes in IDs
            var videoUrl = $"https://www.youtube.com/watch?v={videoId}";

            var info = new ProcessStartInfo("youtube-dl",
                 $"--ignore-config -f bestaudio -x --audio-format m4a --add-metadata " +
                 $"-o {OUTPUT_FOLDER}/%(id)s.%(ext)s \"{videoUrl}\""
                 );

            if (ipv6) info.Arguments += " -6"; // force ipv6

            #if DEBUG
                Console.WriteLine("videoUrl: " + videoUrl);
                Console.WriteLine("args: " + info.Arguments);
            #endif

            info.CreateNoWindow = true;
            info.UseShellExecute = false;
            info.RedirectStandardOutput = true;
            info.RedirectStandardError = true;
            Process process = new Process();
            process.StartInfo = info;
            process.OutputDataReceived += Process_OutputDataReceived;
            process.ErrorDataReceived += Process_ErrorDataReceived;
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
            process.Dispose();
            if(errors != "")
            {
                throw new YoutubeDlException(errors);
            }
            return tempFile;
        }

        public static void DebugPrintVersion()
        {
            var info = new ProcessStartInfo("youtube-dl", "--version");
            info.CreateNoWindow = true;
            info.UseShellExecute = false;
            Process process = new Process();
            process.StartInfo = info;
            process.Start();
            process.WaitForExit();
            process.Dispose();
        }

        private static void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            #if DEBUG
                Console.WriteLine(e.Data);
            #endif
        }

        private static void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null && e.Data.Trim() != "")
            {
                errors += e.Data;
            }
        }

    }
}
