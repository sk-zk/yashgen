using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace yashgen
{
    /// <summary>
    /// Interop class for UnityMediaPlayer.dll; copy-pasted from ILSpy ¯\_(ツ)_/¯
    /// </summary>
    class MediaPlayer : IDisposable
    {
        private bool disposed;
        public float[] fft;
        private GCHandle fftHandle;
        private static MediaPlayer instance;

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
                this.fft = new float[0x200];
                this.fftHandle = GCHandle.Alloc(this.fft, GCHandleType.Pinned);
            }
        }

        [DllImport("UnityMediaPlayer", CallingConvention = CallingConvention.Cdecl)]
        private static extern void cppDispose();
        [DllImport("UnityMediaPlayer", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        private static extern int cppGetBytesPerSecond();
        [DllImport("UnityMediaPlayer", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        private static extern float cppGetDuration(string filePath);
        [DllImport("UnityMediaPlayer", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        private static extern int cppGetFFT512(IntPtr fft);
        [DllImport("UnityMediaPlayer", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        private static extern int cppGetFFT512KISS(IntPtr fft);
        [DllImport("UnityMediaPlayer", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr cppGetMusicFolder();
        [DllImport("UnityMediaPlayer", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        private static extern float cppGetPosition();
        [DllImport("UnityMediaPlayer", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool cppGetPrescanWholeSongSums(IntPtr fft, int count);
        [DllImport("UnityMediaPlayer", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool cppInit();
        [DllImport("UnityMediaPlayer", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool cppPause();
        [DllImport("UnityMediaPlayer", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool cppPlay(string filePath);
        [DllImport("UnityMediaPlayer", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        private static extern int cppPrescanWholeSong(string filePath);
        [DllImport("UnityMediaPlayer", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool cppResume();
        [DllImport("UnityMediaPlayer", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool cppSetPosition(float secs);
        [DllImport("UnityMediaPlayer", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        private static extern void cppSetVolume(float fVol);
        [DllImport("UnityMediaPlayer", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool cppStartPrescan(string filePath);
        [DllImport("UnityMediaPlayer", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool cppStop();
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.fftHandle.Free();
                }
                cppDispose();
                this.disposed = true;
            }
        }

        ~MediaPlayer()
        {
            this.Dispose(false);
        }

        public int GetBytesPerSecond()
        {
            return cppGetBytesPerSecond();
        }

        public float GetDuration(string filePath)
        {
            return cppGetDuration(filePath);
        }

        public int GetFFT512()
        {
            return cppGetFFT512(this.fftHandle.AddrOfPinnedObject());
        }

        public int GetFFT512KISS()
        {
            return cppGetFFT512KISS(this.fftHandle.AddrOfPinnedObject());
        }

        public static string GetMusicFolder()
        {
            return Marshal.PtrToStringUni(cppGetMusicFolder());
        }

        public float GetPosition()
        {
            return cppGetPosition();
        }

        public bool GetPrescanWholeSongSums(float[] sums, int count)
        {
            GCHandle handle = GCHandle.Alloc(sums, GCHandleType.Pinned);
            bool flag = cppGetPrescanWholeSongSums(handle.AddrOfPinnedObject(), count);
            handle.Free();
            return flag;
        }

        public void Pause()
        {
            cppPause();
        }

        public void PlaySongFile(string filePath)
        {
            cppPlay(filePath);
        }

        public int PrescanWholeSong(string filePath)
        {
            return cppPrescanWholeSong(filePath);
        }

        public void Reset()
        {
            cppDispose();
            cppInit();
        }

        public void Resume()
        {
            cppResume();
        }

        public bool SetPosition(float secs)
        {
            return cppSetPosition(secs);
        }

        public void SetVolume(float fVol)
        {
            cppSetVolume(fVol);
        }

        public void StartPrescan2(string filePath)
        {
            cppStartPrescan(filePath);
        }

        public void Stop()
        {
            cppStop();
        }

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
    }


}
