using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yashgen
{
    /// <summary>
    /// Stuff I've adapted from the SongLoader class in AS2
    /// </summary>
    class SongLoader
    {
        List<float> ThreadAccessedDecodeSums;

        /// <summary>
        /// Returns ASH sums of an audio file.
        /// </summary>
        /// <param name="filename">The path to the file.</param>
        /// <param name="duration">The duration of the file in seconds.</param>
        /// <returns></returns>
        public List<float> DecodeSongSums(string filename, float duration)
        {

            MediaPlayer.Instance.Reset();  
            MediaPlayer.Instance.StartPrescan2(filename);
            ThreadAccessedDecodeSums = new List<float>();
            float[] fft = MediaPlayer.Instance.fft;
            float num2 = (duration * 44100f) / 1024f;

            float item;
            while (true)
            {
                item = FastDecodeStepAsync(fft);
                if (item < 0f)
                {
                    break;
                }
                ThreadAccessedDecodeSums.Add(item);
            }
            return ThreadAccessedDecodeSums;
        }

        private struct ThreadedDecodeParams
        {
            public string path;
            public float durationSeconds;
            public ThreadedDecodeParams(string filepath, float durSecs)
            {
                this.path = filepath;
                this.durationSeconds = durSecs;
            }
        }

        private float FastDecodeStepAsync(float[] fft)
        {
            if (MediaPlayer.Instance.GetFFT512KISS() < 1)
            {
                return -1f;
            }
            float b = 0f;
            for (int i = 1; i < 0x200; i++)
            {
                b += (float)(Math.Sqrt(Math.Max(0f, fft[i])));
            }
            return Math.Max(0f, b);
        }

    }
}
