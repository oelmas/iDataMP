/* This class has been written by
 * Corinna John (Hannover, Germany)
 * cj@binary-universe.net
 * 
 * You may do with this code whatever you like,
 * except selling it or claiming any rights/ownership.
 * 
 * Please send me a little feedback about what you're
 * using this code for and what changes you'd like to
 * see in later versions. (And please excuse my bad english.)
 * 
 * WARNING: This is experimental code.
 * Please do not expect "Release Quality".
 * */

using System;
using System.Runtime.InteropServices;

namespace AviFile
{
    public class AudioStream : AviStream
    {
        /// <summary>the stream's format</summary>
        private readonly Avi.PCMWAVEFORMAT waveFormat;

        /// <summary>Initialize an AudioStream for an existing stream</summary>
        /// <param name="aviFile">The file that contains the stream</param>
        /// <param name="aviStream">An IAVISTREAM from [aviFile]</param>
        public AudioStream(int aviFile, IntPtr aviStream)
        {
            this.aviFile = aviFile;
            this.aviStream = aviStream;

            var size = Marshal.SizeOf(waveFormat);
            Avi.NativeMethods.AVIStreamReadFormat(aviStream, 0, ref waveFormat, ref size);
            var streamInfo = GetStreamInfo(aviStream);
        }

        public int CountBitsPerSample => waveFormat.wBitsPerSample;

        public int CountSamplesPerSecond => waveFormat.nSamplesPerSec;

        public int CountChannels => waveFormat.nChannels;

        /// <summary>Read the stream's header information</summary>
        /// <param name="aviStream">The IAVISTREAM to read from</param>
        /// <returns>AVISTREAMINFO</returns>
        private Avi.AVISTREAMINFO GetStreamInfo(IntPtr aviStream)
        {
            var streamInfo = new Avi.AVISTREAMINFO();
            var result = Avi.NativeMethods.AVIStreamInfo(aviStream, ref streamInfo, Marshal.SizeOf(streamInfo));
            if (result != 0) throw new Exception("Exception in AVIStreamInfo: " + result);
            return streamInfo;
        }

        /// <summary>Read the stream's header information</summary>
        /// <returns>AVISTREAMINFO</returns>
        public Avi.AVISTREAMINFO GetStreamInfo()
        {
            if (writeCompressed)
                return GetStreamInfo(compressedStream);
            return GetStreamInfo(aviStream);
        }

        /// <summary>Read the stream's format information</summary>
        /// <returns>PCMWAVEFORMAT</returns>
        public Avi.PCMWAVEFORMAT GetFormat()
        {
            var format = new Avi.PCMWAVEFORMAT();
            var size = Marshal.SizeOf(format);
            var result = Avi.NativeMethods.AVIStreamReadFormat(aviStream, 0, ref format, ref size);
            return format;
        }

        /// <summary>Returns all data needed to copy the stream</summary>
        /// <remarks>Do not forget to call Marshal.FreeHGlobal and release the raw data pointer</remarks>
        /// <param name="streamInfo">Receives the header information</param>
        /// <param name="format">Receives the format</param>
        /// <param name="streamLength">Receives the length of the stream</param>
        /// <returns>Pointer to the wave data</returns>
        public IntPtr GetStreamData(ref Avi.AVISTREAMINFO streamInfo, ref Avi.PCMWAVEFORMAT format,
            ref int streamLength)
        {
            streamInfo = GetStreamInfo();

            format = GetFormat();
            //length in bytes = length in samples * length of a sample
            streamLength = Avi.NativeMethods.AVIStreamLength(aviStream.ToInt32()) * streamInfo.dwSampleSize;
            var waveData = Marshal.AllocHGlobal(streamLength);

            var result = Avi.NativeMethods.AVIStreamRead(aviStream, 0, streamLength, waveData, streamLength, 0, 0);
            if (result != 0) throw new Exception("Exception in AVIStreamRead: " + result);

            return waveData;
        }

        /// <summary>Copy the stream into a new file</summary>
        /// <param name="fileName">Name of the new file</param>
        public override void ExportStream(string fileName)
        {
            var opts = new Avi.AVICOMPRESSOPTIONS_CLASS();
            opts.fccType = (uint)Avi.NativeMethods.mmioStringToFOURCC("auds", 0);
            opts.fccHandler = (uint)Avi.NativeMethods.mmioStringToFOURCC("CAUD", 0);
            opts.dwKeyFrameEvery = 0;
            opts.dwQuality = 0;
            opts.dwFlags = 0;
            opts.dwBytesPerSecond = 0;
            opts.lpFormat = new IntPtr(0);
            opts.cbFormat = 0;
            opts.lpParms = new IntPtr(0);
            opts.cbParms = 0;
            opts.dwInterleaveEvery = 0;

            Avi.NativeMethods.AVISaveV(fileName, 0, 0, 1, ref aviStream, ref opts);
        }
    }
}