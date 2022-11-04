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
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace AviFile
{
    public class AviManager
    {
        private readonly int aviFile;
        private readonly ArrayList streams = new ArrayList();

        /// <summary>Open or create an AVI file</summary>
        /// <param name="fileName">Name of the AVI file</param>
        /// <param name="open">true: Open the file; false: Create or overwrite the file</param>
        public AviManager(string fileName, bool open)
        {
            Avi.NativeMethods.AVIFileInit();
            int result;

            if (open) //open existing file
                result = Avi.NativeMethods.AVIFileOpen(
                    ref aviFile, fileName,
                    Avi.OF_READWRITE, 0);
            else //create empty file
                result = Avi.NativeMethods.AVIFileOpen(
                    ref aviFile, fileName,
                    Avi.OF_WRITE | Avi.OF_CREATE, 0);

            if (result != 0) throw new Exception("Exception in AVIFileOpen: " + result);
        }

        private AviManager(int aviFile)
        {
            this.aviFile = aviFile;
        }

        /// <summary>Get the first video stream - usually there is only one video stream</summary>
        /// <returns>VideoStream object for the stream</returns>
        public VideoStream GetVideoStream()
        {
            IntPtr aviStream;

            var result = Avi.NativeMethods.AVIFileGetStream(
                aviFile,
                out aviStream,
                Avi.streamtypeVIDEO, 0);

            if (result != 0) throw new Exception("Exception in AVIFileGetStream: " + result);

            var stream = new VideoStream(aviFile, aviStream);
            streams.Add(stream);
            return stream;
        }

        /// <summary>Getthe first wave audio stream</summary>
        /// <returns>AudioStream object for the stream</returns>
        public AudioStream GetWaveStream()
        {
            IntPtr aviStream;

            var result = Avi.NativeMethods.AVIFileGetStream(
                aviFile,
                out aviStream,
                Avi.streamtypeAUDIO, 0);

            if (result != 0) throw new Exception("Exception in AVIFileGetStream: " + result);

            var stream = new AudioStream(aviFile, aviStream);
            streams.Add(stream);
            return stream;
        }

        /// <summary>Get a stream from the internal list of opened streams</summary>
        /// <param name="index">Index of the stream. The streams are not sorted, the first stream is the one that was opened first.</param>
        /// <returns>VideoStream at position [index]</returns>
        /// <remarks>
        ///     Use this method after DecompressToNewFile,
        ///     to get the copied stream from the new AVI file
        /// </remarks>
        /// <example>
        ///     //streams cannot be edited - copy to a new file
        ///     AviManager newManager = aviStream.DecompressToNewFile(@"..\..\testdata\temp.avi", true);
        ///     //there is only one stream in the new file - get it and add a frame
        ///     VideoStream aviStream = newManager.GetOpenStream(0);
        ///     aviStream.AddFrame(bitmap);
        /// </example>
        public VideoStream GetOpenStream(int index)
        {
            return (VideoStream)streams[index];
        }

        /// <summary>Add an empty video stream to the file</summary>
        /// <param name="isCompressed">true: Create a compressed stream before adding frames</param>
        /// <param name="frameRate">Frames per second</param>
        /// <param name="frameSize">Size of one frame in bytes</param>
        /// <param name="width">Width of each image</param>
        /// <param name="height">Height of each image</param>
        /// <param name="format">PixelFormat of the images</param>
        /// <returns>VideoStream object for the new stream</returns>
        public VideoStream AddVideoStream(bool isCompressed, double frameRate, int frameSize, int width, int height,
            PixelFormat format)
        {
            var stream = new VideoStream(aviFile, isCompressed, frameRate, frameSize, width, height, format);
            streams.Add(stream);
            return stream;
        }

        /// <summary>Add an empty video stream to the file</summary>
        /// <remarks>Compresses the stream without showing the codecs dialog</remarks>
        /// <param name="compressOptions">Compression options</param>
        /// <param name="frameRate">Frames per second</param>
        /// <param name="firstFrame">Image to write into the stream as the first frame</param>
        /// <returns>VideoStream object for the new stream</returns>
        public VideoStream AddVideoStream(Avi.AVICOMPRESSOPTIONS compressOptions, double frameRate, Bitmap firstFrame)
        {
            var stream = new VideoStream(aviFile, compressOptions, frameRate, firstFrame);
            streams.Add(stream);
            return stream;
        }

        /// <summary>Add an empty video stream to the file</summary>
        /// <param name="isCompressed">true: Create a compressed stream before adding frames</param>
        /// <param name="frameRate">Frames per second</param>
        /// <param name="firstFrame">Image to write into the stream as the first frame</param>
        /// <returns>VideoStream object for the new stream</returns>
        public VideoStream AddVideoStream(bool isCompressed, double frameRate, Bitmap firstFrame)
        {
            var stream = new VideoStream(aviFile, isCompressed, frameRate, firstFrame);
            streams.Add(stream);
            return stream;
        }

        /// <summary>Add a wave audio stream from another file to this file</summary>
        /// <param name="waveFileName">Name of the wave file to add</param>
        /// <param name="startAtFrameIndex">Index of the video frame at which the sound is going to start</param>
        public void AddAudioStream(string waveFileName, int startAtFrameIndex)
        {
            var audioManager = new AviManager(waveFileName, true);
            var newStream = audioManager.GetWaveStream();
            AddAudioStream(newStream, startAtFrameIndex);
            audioManager.Close();
        }

        private IntPtr InsertSilence(int countSilentSamples, IntPtr waveData, int lengthWave,
            ref Avi.AVISTREAMINFO streamInfo)
        {
            //initialize silence
            var lengthSilence = countSilentSamples * streamInfo.dwSampleSize;
            var silence = new byte[lengthSilence];

            //initialize new sound
            var lengthNewStream = lengthSilence + lengthWave;
            var newWaveData = Marshal.AllocHGlobal(lengthNewStream);

            //copy silence
            Marshal.Copy(silence, 0, newWaveData, lengthSilence);

            //copy sound
            var sound = new byte[lengthWave];
            Marshal.Copy(waveData, sound, 0, lengthWave);
            var startOfSound = new IntPtr(newWaveData.ToInt32() + lengthSilence);
            Marshal.Copy(sound, 0, startOfSound, lengthWave);

            Marshal.FreeHGlobal(newWaveData);

            streamInfo.dwLength = lengthNewStream;
            return newWaveData;
        }

        /// <summary>Add an existing wave audio stream to the file</summary>
        /// <param name="newStream">The stream to add</param>
        /// <param name="startAtFrameIndex">
        ///     The index of the video frame at which the sound is going to start.
        ///     '0' inserts the sound at the beginning of the video.
        /// </param>
        public void AddAudioStream(AudioStream newStream, int startAtFrameIndex)
        {
            var streamInfo = new Avi.AVISTREAMINFO();
            var streamFormat = new Avi.PCMWAVEFORMAT();
            var streamLength = 0;

            var rawData = newStream.GetStreamData(ref streamInfo, ref streamFormat, ref streamLength);
            var waveData = rawData;

            if (startAtFrameIndex > 0)
            {
                //not supported
                //streamInfo.dwStart = startAtFrameIndex;

                var framesPerSecond = GetVideoStream().FrameRate;
                double samplesPerSecond = newStream.CountSamplesPerSecond;
                var startAtSecond = startAtFrameIndex / framesPerSecond;
                var startAtSample = (int)(samplesPerSecond * startAtSecond);

                waveData = InsertSilence(startAtSample - 1, waveData, streamLength, ref streamInfo);
            }

            IntPtr aviStream;
            var result = Avi.NativeMethods.AVIFileCreateStream(aviFile, out aviStream, ref streamInfo);
            if (result != 0) throw new Exception("Exception in AVIFileCreateStream: " + result);

            result = Avi.NativeMethods.AVIStreamSetFormat(aviStream, 0, ref streamFormat, Marshal.SizeOf(streamFormat));
            if (result != 0) throw new Exception("Exception in AVIStreamSetFormat: " + result);

            result = Avi.NativeMethods.AVIStreamWrite(aviStream, 0, streamLength, waveData, streamLength,
                Avi.AVIIF_KEYFRAME, 0, 0);
            if (result != 0) throw new Exception("Exception in AVIStreamWrite: " + result);

            result = Avi.NativeMethods.AVIStreamRelease(aviStream);
            if (result != 0) throw new Exception("Exception in AVIStreamRelease: " + result);

            Marshal.FreeHGlobal(waveData);
        }

        /// <summary>Add an existing wave audio stream to the file</summary>
        /// <param name="waveData">The new stream's data</param>
        /// <param name="streamInfo">Header info for the new stream</param>
        /// <param name="streamFormat">The new stream' format info</param>
        /// <param name="streamLength">Length of the new stream</param>
        public void AddAudioStream(IntPtr waveData, Avi.AVISTREAMINFO streamInfo, Avi.PCMWAVEFORMAT streamFormat,
            int streamLength)
        {
            IntPtr aviStream;
            var result = Avi.NativeMethods.AVIFileCreateStream(aviFile, out aviStream, ref streamInfo);
            if (result != 0) throw new Exception("Exception in AVIFileCreateStream: " + result);

            result = Avi.NativeMethods.AVIStreamSetFormat(aviStream, 0, ref streamFormat, Marshal.SizeOf(streamFormat));
            if (result != 0) throw new Exception("Exception in AVIStreamSetFormat: " + result);

            result = Avi.NativeMethods.AVIStreamWrite(aviStream, 0, streamLength, waveData, streamLength,
                Avi.AVIIF_KEYFRAME, 0, 0);
            if (result != 0) throw new Exception("Exception in AVIStreamWrite: " + result);

            result = Avi.NativeMethods.AVIStreamRelease(aviStream);
            if (result != 0) throw new Exception("Exception in AVIStreamRelease: " + result);
        }

        /// <summary>Copy a piece of video and wave sound int a new file</summary>
        /// <param name="newFileName">File name</param>
        /// <param name="startAtSecond">Start copying at second x</param>
        /// <param name="stopAtSecond">Stop copying at second y</param>
        /// <returns>AviManager for the new video</returns>
        public AviManager CopyTo(string newFileName, float startAtSecond, float stopAtSecond)
        {
            var newFile = new AviManager(newFileName, false);

            try
            {
                //copy video stream

                var videoStream = GetVideoStream();

                var startFrameIndex = (int)(videoStream.FrameRate * startAtSecond);
                var stopFrameIndex = (int)(videoStream.FrameRate * stopAtSecond);

                videoStream.GetFrameOpen();
                var bmp = videoStream.GetBitmap(startFrameIndex);
                var newStream = newFile.AddVideoStream(false, videoStream.FrameRate, bmp);
                for (var n = startFrameIndex + 1; n <= stopFrameIndex; n++)
                {
                    bmp = videoStream.GetBitmap(n);
                    newStream.AddFrame(bmp);
                }

                videoStream.GetFrameClose();

                //copy audio stream

                var waveStream = GetWaveStream();

                var streamInfo = new Avi.AVISTREAMINFO();
                var streamFormat = new Avi.PCMWAVEFORMAT();
                var streamLength = 0;
                var ptrRawData = waveStream.GetStreamData(
                    ref streamInfo,
                    ref streamFormat,
                    ref streamLength);

                var startByteIndex = (int)(startAtSecond * (waveStream.CountSamplesPerSecond * streamFormat.nChannels *
                                                            waveStream.CountBitsPerSample) / 8);
                var stopByteIndex = (int)(stopAtSecond * (waveStream.CountSamplesPerSecond * streamFormat.nChannels *
                                                          waveStream.CountBitsPerSample) / 8);

                var ptrWavePart = new IntPtr(ptrRawData.ToInt32() + startByteIndex);

                var rawData = new byte[stopByteIndex - startByteIndex];
                Marshal.Copy(ptrWavePart, rawData, 0, rawData.Length);
                Marshal.FreeHGlobal(ptrRawData);

                streamInfo.dwLength = rawData.Length;
                streamInfo.dwStart = 0;

                var unmanagedRawData = Marshal.AllocHGlobal(rawData.Length);
                Marshal.Copy(rawData, 0, unmanagedRawData, rawData.Length);
                newFile.AddAudioStream(unmanagedRawData, streamInfo, streamFormat, rawData.Length);
                Marshal.FreeHGlobal(unmanagedRawData);
            }
            catch (Exception ex)
            {
                newFile.Close();
                throw ex;
            }

            return newFile;
        }

        /// <summary>Release all ressources</summary>
        public void Close()
        {
            foreach (AviStream stream in streams) stream.Close();

            Avi.NativeMethods.AVIFileRelease(aviFile);
            Avi.NativeMethods.AVIFileExit();
        }

        public static void MakeFileFromStream(string fileName, AviStream stream)
        {
            var newFile = IntPtr.Zero;
            var streamPointer = stream.StreamPointer;

            var opts = new Avi.AVICOMPRESSOPTIONS_CLASS();
            opts.fccType = (uint)Avi.streamtypeVIDEO;
            opts.lpParms = IntPtr.Zero;
            opts.lpFormat = IntPtr.Zero;
            Avi.NativeMethods.AVISaveOptions(IntPtr.Zero, Avi.ICMF_CHOOSE_KEYFRAME | Avi.ICMF_CHOOSE_DATARATE, 1,
                ref streamPointer, ref opts);
            Avi.NativeMethods.AVISaveOptionsFree(1, ref opts);

            Avi.NativeMethods.AVISaveV(fileName, 0, 0, 1, ref streamPointer, ref opts);
        }
    }
}