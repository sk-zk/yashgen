using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace yashgen
{
    /// <summary>
    /// Interop class for Audiosurf 2/Audiosurf2_Data/Plugins/x86_64/libASMedia.so; 
    /// copy-pasted from ILSpy ¯\_(ツ)_/¯
    /// </summary>
    class MediaPlayer : IDisposable
    {
        private static MediaPlayer instance;

        private bool disposed;

        public float[] fft;

        private GCHandle fftHandle;

        public static MediaPlayer Instance
        {
            get
            {
                if (instance == null)
                {
                    new MediaPlayer();
                }
                return instance;
            }
        }

        public MediaPlayer()
        {
            if (instance != null)
            {
                //Debug.LogError("MediaPlayer.cs DOH: tried to create a second singleton");
            }
            else
            {
                cppInit();
                instance = this;
                fft = new float[512];
                fftHandle = GCHandle.Alloc(fft, GCHandleType.Pinned);
            }
        }

        [DllImport("ASMedia")]
        private static extern bool cppInit();

        [DllImport("ASMedia")]
        private static extern float cppGetDuration(string filePath);

        public float GetDuration(string filePath)
        {
            return cppGetDuration(filePath);
        }

        [DllImport("ASMedia")]
        private static extern bool cppPlay(string filePath);

        public void PlaySongFile(string filePath)
        {
            cppPlay(filePath);
        }

        [DllImport("ASMedia")]
        private static extern bool cppStartPrescan(string filePath);

        public void StartPrescan2(string filePath)
        {
            cppStartPrescan(filePath);
        }

        [DllImport("ASMedia")]
        private static extern int cppGetFFT512KISS(IntPtr fft);

        public int GetFFT512KISS()
        {
            return cppGetFFT512KISS(fftHandle.AddrOfPinnedObject());
        }

        [DllImport("ASMedia")]
        private static extern int cppGetFFT512(IntPtr fft);

        public int GetFFT512()
        {
            return cppGetFFT512(fftHandle.AddrOfPinnedObject());
        }

        [DllImport("ASMedia")]
        private static extern int cppPrescanWholeSong(string filePath);

        public int PrescanWholeSong(string filePath)
        {
            return cppPrescanWholeSong(filePath);
        }

        [DllImport("ASMedia")]
        private static extern bool cppGetPrescanWholeSongSums(IntPtr fft, int count);

        public bool GetPrescanWholeSongSums(float[] sums, int count)
        {
            GCHandle gCHandle = GCHandle.Alloc(sums, GCHandleType.Pinned);
            bool result = cppGetPrescanWholeSongSums(gCHandle.AddrOfPinnedObject(), count);
            gCHandle.Free();
            return result;
        }

        [DllImport("ASMedia")]
        private static extern float cppGetPosition();

        public float GetPosition()
        {
            return cppGetPosition();
        }

        [DllImport("ASMedia")]
        private static extern bool cppSetPosition(float secs);

        public bool SetPosition(float secs)
        {
            return cppSetPosition(secs);
        }

        [DllImport("ASMedia")]
        private static extern int cppGetBytesPerSecond();

        public int GetBytesPerSecond()
        {
            return cppGetBytesPerSecond();
        }

        [DllImport("ASMedia")]
        private static extern bool cppStop();

        public void Stop()
        {
            cppStop();
        }

        [DllImport("ASMedia")]
        private static extern bool cppPause();

        public void Pause()
        {
            cppPause();
        }

        [DllImport("ASMedia")]
        private static extern bool cppResume();

        public void Resume()
        {
            cppResume();
        }

        [DllImport("ASMedia")]
        private static extern void cppSetVolume(float fVol);

        public void SetVolume(float fVol)
        {
            cppSetVolume(fVol);
        }

        public static string GetMusicFolder()
        {
            return Environment.GetEnvironmentVariable("HOME") + "/Music";
        }

        [DllImport("ASMedia")]
        private static extern void cppDispose();

        public void Reset()
        {
            cppDispose();
            cppInit();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    fftHandle.Free();
                }
                cppDispose();
                disposed = true;
            }
        }

        ~MediaPlayer()
        {
            Dispose(false);
        }
    }


}
