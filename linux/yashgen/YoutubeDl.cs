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
        /// Downloads an audio track from a YouTube video as wav.
        /// </summary>
        /// <param name="videoId">The ID of the video.</param>
        /// <returns>The path to the downloaded file.</returns>
        public static string CallYoutubeDl(string videoId)
        {
            errors = "";
            string tempFile = Path.Combine(OUTPUT_FOLDER, videoId + ".wav");
            
            // workaround for "invalid retry count" bug with leading dashes in IDs
            var videoUrl = $"https://www.youtube.com/watch?v={videoId}";
            
            var info = new ProcessStartInfo(
                 "youtube-dl", 
                 $"--ignore-config -f bestaudio -x --audio-format wav --add-metadata " +
                 $"-o {OUTPUT_FOLDER}/%(id)s.%(ext)s \"{videoUrl}\""
                );

            #if DEBUG
                Console.WriteLine("videoUrl: " + videoUrl);
                Console.WriteLine("args: " + info.Arguments);
            #endif

            info.CreateNoWindow = true;
            info.UseShellExecute = false;
            #if RELEASE
                info.RedirectStandardOutput = true;
            #endif
            info.RedirectStandardError = true;
            Process process = new Process();
            process.StartInfo = info;
            process.ErrorDataReceived += Process_ErrorDataReceived;
            process.Start();
            #if RELEASE
                process.BeginOutputReadLine();
            #endif
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

        private static void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null && e.Data.Trim() != "")
            {
                errors += e.Data;
            }
        }

    }


}
