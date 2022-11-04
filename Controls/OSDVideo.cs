using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AviFile;
using DirectShowLib;
using DirectShowLib.DES;
using MissionPlanner.Utilities;

namespace MissionPlanner.Controls
{
    public partial class OSDVideo : Form, ISampleGrabberCB
    {
        private static int frame;

        //double framerate = 0;
        private static int framecount;

        private static Thread th;

        private readonly object avienclock = new object();
        private CurrentState cs = new CurrentState();

        public bool DSplugin = false;

        public Dictionary<DateTime, CurrentState> flightdata = new Dictionary<DateTime, CurrentState>();
        //static bool play = true;
        //static bool stop = false;

        private bool fullresolution;

        public Bitmap image;
        private IntPtr ip = IntPtr.Zero;
        private long m_avgtimeperframe;

        /// <summary> Set by async routine when it captures an image </summary>
        private volatile bool m_bGotOne;

        /// <summary> Indicates the status of the graph </summary>
        private bool m_bRunning;

        public int m_Dropped = 0;

        /// <summary> graph builder interface. </summary>
        private IFilterGraph2 m_FilterGraph;

        /// <summary> Dimensions of the image, calculated once in constructor. </summary>
        private IntPtr m_handle = IntPtr.Zero;

        private IMediaControl m_mediaCtrl;

        private IAMExtendedSeeking m_mediaextseek;
        private IMediaPosition m_mediapos;
        private IMediaSeeking m_mediaseek;

        /// <summary> so we can wait for the async job to finish </summary>
        private ManualResetEvent m_PictureReady;

        private int m_videoHeight;

        private int m_videoWidth;

        private AviManager newManager;
        private VideoStream newStream;
        public DateTime startlogtime = DateTime.MinValue;
        private Thread timer1;
        private readonly Bitmap trans = new Bitmap(5, 5, PixelFormat.Format32bppArgb);
        public DateTime videopos = DateTime.MinValue;

        public OSDVideo()
        {
            InitializeComponent();

            CheckForIllegalCrossThreadCalls = false;

            hud1.SixteenXNine = true;

            Tracking.AddPage(GetType().ToString(), Text);
        }

        public string textbox
        {
            set => textBox1.Text = value;
        }

        public int Stride { get; private set; }

        /// <summary> sample callback, NOT USED. </summary>
        int ISampleGrabberCB.SampleCB(double SampleTime, IMediaSample pSample)
        {
            if (!m_bGotOne)
            {
                // Set bGotOne to prevent further calls until we
                // request a new bitmap.
                m_bGotOne = true;
                IntPtr pBuffer;

                pSample.GetPointer(out pBuffer);
                var iBufferLen = pSample.GetSize();

                if (pSample.GetSize() > Stride * m_videoHeight) throw new Exception("Buffer is wrong size");

                NativeMethods.CopyMemory(m_handle, pBuffer, Stride * m_videoHeight);

                // Picture is ready.
                m_PictureReady.Set();
            }

            Marshal.ReleaseComObject(pSample);
            return 0;
        }

        /// <summary> buffer callback, COULD BE FROM FOREIGN THREAD. </summary>
        int ISampleGrabberCB.BufferCB(double SampleTime, IntPtr pBuffer, int BufferLen)
        {
            Console.WriteLine("BufferCB " + DateTime.Now.Millisecond + " pbtime " + SampleTime);
            framecount++;

            videopos = startlogtime.AddSeconds(SampleTime);

            trackBar_mediapos.Value = (int)SampleTime;

            // The buffer should be long enought
            if (BufferLen <= Stride * m_videoHeight)
                // Copy the frame to the buffer
                // CopyMemory(m_handle, pBuffer, m_stride * m_videoHeight);
                m_handle = pBuffer;
            else
                throw new Exception("Buffer is wrong size");

            try
            {
                Console.WriteLine("1 " + DateTime.Now.Millisecond);
                //ip = this.GetBitMap();
                image = new Bitmap(m_videoWidth, m_videoHeight, Stride, PixelFormat.Format24bppRgb, m_handle);
                Console.WriteLine("1a " + DateTime.Now.Millisecond);

                image.RotateFlip(RotateFlipType.RotateNoneFlipY);

                Console.WriteLine("1b " + DateTime.Now.Millisecond);


                hud1.HoldInvalidation = true;
                hud1.opengl = true;
                hud1.bgimage = image;
                hud1.streamjpgenable = true;
                if (fullresolution)
                {
                    hud1.Width = image.Width;
                    hud1.Height = image.Height;
                }

                Console.WriteLine("1c " + DateTime.Now.Millisecond);

                hud1.Refresh();

                Console.WriteLine("1d " + DateTime.Now.Millisecond);

                var bmp = (Bitmap)hud1.objBitmap.Clone();

                //  bmp.Save(framecount+".bmp");

                Console.WriteLine("1e " + DateTime.Now.Millisecond);

                if (newStream == null)
                {
                    //double frate = GetFrameRate(txtAviFileName.Text);

                    var frate = Math.Round(10000000.0 / m_avgtimeperframe, 0);

                    newStream = newManager.AddVideoStream(true, frate, bmp);
                }

                Console.WriteLine("2 " + DateTime.Now.Millisecond);

                addframe(bmp);
                lock (avienclock)
                {
                    //    System.Threading.ThreadPool.QueueUserWorkItem(addframe, bmp);
                }


                Console.WriteLine("3 " + DateTime.Now.Millisecond);
            }
            //System.Windows.Forms.CustomMessageBox.Show("Problem with capture device, grabbing frame took longer than 5 sec");
            catch (Exception ex)
            {
                Console.WriteLine("Grab bmp failed " + ex);
            }


            return 0;
        }

        private void OSDVideo_camimage(Image camimage)
        {
            //  camimage = new Bitmap(640, 480, PixelFormat.Format32bppArgb);

            hud1.bgimage = camimage; // new Bitmap(camimage, hud1.Size);
            //Application.DoEvents();
        }

        private void saveconfig()
        {
            try
            {
                using (
                    var sw =
                    new StreamWriter(Path.GetDirectoryName(txtAviFileName.Text) + Path.DirectorySeparatorChar +
                                     "osdvideo.txt"))
                {
                    sw.WriteLine(txtAviFileName.Text);
                    sw.WriteLine(txt_tlog.Text);
                    sw.WriteLine(trackBar1.Value);
                }
            }
            catch
            {
            }
        }

        private void loadconfig()
        {
            try
            {
                using (
                    var sr =
                    new StreamReader(Path.GetDirectoryName(txtAviFileName.Text) + Path.DirectorySeparatorChar +
                                     "osdvideo.txt"))
                {
                    txtAviFileName.Text = sr.ReadLine();
                    txt_tlog.Text = sr.ReadLine();
                    trackBar1.Value = int.Parse(sr.ReadLine());
                }
            }
            catch
            {
            }
        }

        private async Task startup()
        {
            await dolog().ConfigureAwait(true);

            if (DSplugin)
            {
                timer1 = new Thread(timer);
                timer1.IsBackground = true;
                timer1.Start();

                return;
            }

            try
            {
                if (th != null)
                    th.Abort();
            }
            catch
            {
            }

            th = new Thread(delegate() { StartCapture(); });
            th.Name = "Video Thread";
            th.Start();
        }

        public void writeconsole(string input)
        {
            Console.WriteLine(input);
        }

        public Bitmap gethud(Bitmap bmpin, double time)
        {
            // stop it from drawing
            hud1.HoldInvalidation = true;
            // update bg to be trans
            hud1.bgimage = trans;
            // use gdi
            hud1.opengl = false;
            // resize
            hud1.Width = bmpin.Width;
            hud1.Height = bmpin.Height;
            // makesure we can grab an image
            hud1.streamjpgenable = true;
            // redraw
            hud1.Refresh();
            // clone current screen with trans
            return (Bitmap)hud1.objBitmap.Clone();
        }

        private void StartCapture()
        {
            int hr;

            ISampleGrabber sampGrabber = null;
            IBaseFilter capFilter = null;
            ICaptureGraphBuilder2 capGraph = null;

            if (File.Exists(txtAviFileName.Text))
            {
                // Get the graphbuilder object
                m_FilterGraph = (IFilterGraph2)new FilterGraph();
                m_mediaCtrl = m_FilterGraph as IMediaControl;

                // Get the ICaptureGraphBuilder2
                capGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();

                // Get the SampleGrabber interface
                sampGrabber = (ISampleGrabber)new SampleGrabber();

                // Start building the graph
                hr = capGraph.SetFiltergraph(m_FilterGraph);
                DsError.ThrowExceptionForHR(hr);

                // Add the video source
                hr = m_FilterGraph.AddSourceFilter(txtAviFileName.Text, "File Source (Async.)", out capFilter);
                DsError.ThrowExceptionForHR(hr);

                //add AVI Decompressor
                var pAVIDecompressor = (IBaseFilter)new AVIDec();
                hr = m_FilterGraph.AddFilter(pAVIDecompressor, "AVI Decompressor");
                DsError.ThrowExceptionForHR(hr);


                //
                var baseGrabFlt = (IBaseFilter)sampGrabber;
                ConfigureSampleGrabber(sampGrabber);

                // Add the frame grabber to the graph
                hr = m_FilterGraph.AddFilter(baseGrabFlt, "Ds.NET Grabber");
                DsError.ThrowExceptionForHR(hr);


                var vidrender = (IBaseFilter)new VideoRenderer();
                hr = m_FilterGraph.AddFilter(vidrender, "Render");
                DsError.ThrowExceptionForHR(hr);

                var captpin = DsFindPin.ByDirection(capFilter, PinDirection.Output, 0);

                var samppin = DsFindPin.ByName(baseGrabFlt, "Input");

                hr = m_FilterGraph.Connect(captpin, samppin);
                DsError.ThrowExceptionForHR(hr);

                var filewritter = new FileWriter();
                var filemux = (IFileSinkFilter)filewritter;
                //filemux.SetFileName("test.avi",);

                //hr = capGraph.RenderStream(null, MediaType.Video, capFilter, null, vidrender);
                // DsError.ThrowExceptionForHR(hr); 

                SaveSizeInfo(sampGrabber);

                // setup buffer
                if (m_handle == IntPtr.Zero)
                    m_handle = Marshal.AllocCoTaskMem(Stride * m_videoHeight);

                // tell the callback to ignore new images
                m_PictureReady = new ManualResetEvent(false);
                m_bGotOne = false;
                m_bRunning = false;

                timer1 = new Thread(timer);
                timer1.IsBackground = true;
                timer1.Start();

                m_mediaextseek = m_FilterGraph as IAMExtendedSeeking;
                m_mediapos = m_FilterGraph as IMediaPosition;
                m_mediaseek = m_FilterGraph as IMediaSeeking;
                double length = 0;
                m_mediapos.get_Duration(out length);
                trackBar_mediapos.Minimum = 0;
                trackBar_mediapos.Maximum = (int)length;

                Start();
            }
            else
            {
                MessageBox.Show("File does not exist");
            }
        }

        private async Task dolog()
        {
            flightdata.Clear();

            using (var mine = new MAVLinkInterface())
            {
                try
                {
                    mine.logplaybackfile =
                        new BinaryReader(File.Open(txt_tlog.Text, FileMode.Open, FileAccess.Read, FileShare.Read));
                }
                catch
                {
                    CustomMessageBox.Show("Log Can not be opened. Are you still connected?");
                    return;
                }

                mine.logreadmode = true;
                mine.speechenabled = false;

                await mine.readPacketAsync().ConfigureAwait(true);

                startlogtime = mine.lastlogread;

                double oldlatlngsum = 0;

                var appui = 0;

                while (mine.logplaybackfile.BaseStream.Position < mine.logplaybackfile.BaseStream.Length)
                {
                    var packet = await mine.readPacketAsync().ConfigureAwait(true);

                    cs.datetime = mine.lastlogread;

                    cs.UpdateCurrentSettings(null, true, mine);

                    if (appui != DateTime.Now.Second)
                    {
                        // cant do entire app as mixes with flightdata timer
                        Refresh();
                        appui = DateTime.Now.Second;
                    }

                    // ignore because of this Exception System.PlatformNotSupportedException: No voice installed on the system or none available with the current security setting.

                    // if ((float)(cs.lat + cs.lng + cs.alt) != oldlatlngsum
                    //     && cs.lat != 0 && cs.lng != 0)

                    var nexttime = mine.lastlogread.AddMilliseconds(-(mine.lastlogread.Millisecond % 100));

                    if (!flightdata.ContainsKey(nexttime))
                    {
                        Console.WriteLine(cs.lat + " " + cs.lng + " " + cs.alt + "   lah " +
                                          (float)(cs.lat + cs.lng + cs.alt) + "!=" + oldlatlngsum);
                        var cs2 = (CurrentState)cs.Clone();

                        try
                        {
                            flightdata.Add(nexttime, cs2);
                        }
                        catch
                        {
                        }

                        oldlatlngsum = cs.lat + cs.lng + cs.alt;
                    }
                }

                mine.logreadmode = false;
                mine.logplaybackfile.Close();
                mine.logplaybackfile = null;
            }
        }

        public void timer()
        {
            CheckForIllegalCrossThreadCalls = false;

            videopos = startlogtime.AddMilliseconds(-startlogtime.Millisecond);
            hud1.SixteenXNine = true;

            while (true)
                try
                {
                    //   GC.Collect();
                    //  long mem = GC.GetTotalMemory(true) / 1024 / 1024;
                    // Console.WriteLine("mem "+mem);
                    Thread.Sleep(20); // 25 fps

                    if (flightdata.Count == 0)
                        break;


                    //  videopos = videopos.AddMilliseconds(1000 / 25);
                    //                    m_mediaseek = m_FilterGraph as IMediaSeeking;
                    //  m_mediapos.put_CurrentPosition((vidpos - startlogtime).TotalSeconds);
                    //m_mediaseek.SetTimeFormat(TimeFormat.MediaTime);
                    //long poscurrent = 0;
                    //long posend = 0;
                    //                    m_mediaseek.GetPositions(out poscurrent,out posend);
                    var cstimestamp =
                        videopos.AddSeconds(trackBar1.Value).AddMilliseconds(-(videopos.Millisecond % 20));

                    var tb = trackBar1.Value;

                    if (flightdata.ContainsKey(cstimestamp))
                        cs = flightdata[cstimestamp];
                    else if (flightdata.ContainsKey(cstimestamp.AddMilliseconds(-20)))
                        cs = flightdata[cstimestamp.AddMilliseconds(-20)];
                    else if (flightdata.ContainsKey(cstimestamp.AddMilliseconds(-40)))
                        cs = flightdata[cstimestamp.AddMilliseconds(-40)];

                    //  Console.WriteLine("Update CS");
                    Console.WriteLine("log " + cs.datetime);

                    hud1.datetime = cs.datetime;

                    //cs.UpdateCurrentSettings(bindingSource1,true,MainV2.comPort);
                    bindingSource1.DataSource = cs;
                    bindingSource1.ResetBindings(false);
                }
                catch (ThreadAbortException)
                {
                    break;
                }
                catch
                {
                }
        }

        /// <summary> capture the next image </summary>
        public IntPtr GetBitMap()
        {
            if (m_handle == IntPtr.Zero)
                m_handle = Marshal.AllocCoTaskMem(Stride * m_videoHeight);

            try
            {
                // Start waiting
                //  if (!m_PictureReady.WaitOne(5000, false))
                {
                    //throw new Exception("Timeout waiting to get picture");
                }
            }
            catch
            {
                Marshal.FreeCoTaskMem(m_handle);
                throw;
            }

            // Got one
            return m_handle;
        }

        // Start the capture graph
        public void Start()
        {
            if (!m_bRunning)
            {
                var hr = m_mediaCtrl.Run();
                DsError.ThrowExceptionForHR(hr);

                m_bRunning = true;
            }
        }

        // Pause the capture graph.
        // Running the graph takes up a lot of resources.  Pause it when it
        // isn't needed.
        public void Pause()
        {
            if (m_bRunning)
            {
                var hr = m_mediaCtrl.Pause();
                DsError.ThrowExceptionForHR(hr);

                m_bRunning = false;
            }
        }

        private double GetFrameRate(string filename)
        {
            var md = new MediaDet() as IMediaDet;
            Guid streamType;
            var mt = new AMMediaType();
            int hr, nStreams;

            md.put_Filename(filename);
            md.get_OutputStreams(out nStreams);

            for (var i = 0; i < nStreams; i++)
            {
                hr = md.put_CurrentStream(i);
                DsError.ThrowExceptionForHR(hr);

                hr = md.get_StreamType(out streamType);
                DsError.ThrowExceptionForHR(hr);
                if (streamType == MediaType.Video)
                {
                    md.put_CurrentStream(0);

                    double frate = 30;

                    md.get_FrameRate(out frate);
                    return frate;
                }
            }

            return 30;
        }

        private List<int> GetAudioStreams(string filename)
        {
            var md = new MediaDet() as IMediaDet;
            Guid streamType;
            var mt = new AMMediaType();
            int hr, nStreams;
            var streamList = new List<int>();

            md.put_Filename(filename);
            md.get_OutputStreams(out nStreams);

            for (var i = 0; i < nStreams; i++)
            {
                hr = md.put_CurrentStream(i);
                DsError.ThrowExceptionForHR(hr);

                hr = md.get_StreamType(out streamType);
                DsError.ThrowExceptionForHR(hr);
                if (streamType == MediaType.Audio)
                    streamList.Add(i);
            }

            return streamList;
        }


        private void SaveSizeInfo(ISampleGrabber sampGrabber)
        {
            int hr;

            // Get the media type from the SampleGrabber
            var media = new AMMediaType();
            hr = sampGrabber.GetConnectedMediaType(media);
            DsError.ThrowExceptionForHR(hr);

            if (media.formatType != FormatType.VideoInfo || media.formatPtr == IntPtr.Zero)
                throw new NotSupportedException("Unknown Grabber Media Format");

            // Grab the size info
            var videoInfoHeader =
                (VideoInfoHeader)Marshal.PtrToStructure(media.formatPtr, typeof(VideoInfoHeader));
            m_videoWidth = videoInfoHeader.BmiHeader.Width;
            m_videoHeight = videoInfoHeader.BmiHeader.Height;
            Stride = m_videoWidth * (videoInfoHeader.BmiHeader.BitCount / 8);
            m_avgtimeperframe = videoInfoHeader.AvgTimePerFrame;

            DsUtils.FreeAMMediaType(media);
            media = null;
        }

        private void ConfigureSampleGrabber(ISampleGrabber sampGrabber)
        {
            AMMediaType media;
            int hr;

            // Set the media type to Video/RBG24
            media = new AMMediaType();
            media.majorType = MediaType.Video;
            media.subType = MediaSubType.RGB24;
            media.formatType = FormatType.VideoInfo;
            sampGrabber.SetBufferSamples(false);
            sampGrabber.SetOneShot(false);
            hr = sampGrabber.SetMediaType(media);
            DsError.ThrowExceptionForHR(hr);

            DsUtils.FreeAMMediaType(media);
            media = null;

            // Configure the samplegrabber
            hr = sampGrabber.SetCallback(this, 1);
            DsError.ThrowExceptionForHR(hr);
        }

        private void BUT_vidfile_Click(object sender, EventArgs e)
        {
            var fileName = GetFileName("Videos (*.avi)|*.avi;*.mpe;*.mpeg;*.mp4", txtAviFileName);
            if (fileName != null)
            {
                // update name before calling next function
                txtAviFileName.Text = fileName;
                // load setting if they exist
                loadconfig();
                // force file we just picked as video
                txtAviFileName.Text = fileName;
            }
        }

        private string GetFileName(string filter, Control ctl)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Filter = filter;
                dlg.RestoreDirectory = true;
                if (ctl.Text.Length > 0) dlg.InitialDirectory = GetCurrentFilePath(ctl);
                if (dlg.ShowDialog(this) == DialogResult.OK)
                    return dlg.FileName;
                return null;
            }
        }

        private string GetCurrentFilePath(Control ctl)
        {
            return ctl.Text.Substring(0, ctl.Text.LastIndexOf("\\") + 1);
        }

        private async void BUT_start_Click(object sender, EventArgs e)
        {
            saveconfig();
            try
            {
                if (m_mediaCtrl != null)
                    m_mediaCtrl.Stop();
            }
            catch
            {
            }

            try
            {
                frame = framecount;
                if (th != null)
                    th.Abort();
            }
            catch
            {
            }

            try
            {
                newManager =
                    new AviManager(
                        Path.GetDirectoryName(txtAviFileName.Text) + Path.DirectorySeparatorChar +
                        Path.GetFileNameWithoutExtension(txtAviFileName.Text) + "-overlay.avi", false);
            }
            catch
            {
                CustomMessageBox.Show(Strings.InvalidFileName, Strings.ERROR);
                return;
            }


            //newManager.Close();

            await startup().ConfigureAwait(true);

            MaximumSize = Size;
            MinimumSize = Size;
        }


        private void addframe(object bmp)
        {
            lock (avienclock)
            {
                newStream.AddFrame((Bitmap)bmp);
                ((Bitmap)bmp).Dispose();
            }
        }

        private void BUT_tlogfile_Click(object sender, EventArgs e)
        {
            var fileName = GetFileName("Tlog (*.tlog)|*.tlog", txt_tlog);
            if (fileName != null) txt_tlog.Text = fileName;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            hud1.Invalidate();

            label1.Text = "time offset in seconds " + trackBar1.Value;

            saveconfig();
        }

        private void trackBar_mediapos_Scroll(object sender, EventArgs e)
        {
            m_mediapos.put_CurrentPosition(trackBar_mediapos.Value);
        }

        private void OSDVideo_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                flightdata.Clear();

                th.Abort();
            }
            catch
            {
            }

            try
            {
                m_mediaCtrl.Stop();
            }
            catch
            {
            }

            try
            {
                Thread.Sleep(500);
                newManager.Close();

                th.Abort();
            }
            catch
            {
            }
        }

        private void CHK_fullres_CheckedChanged(object sender, EventArgs e)
        {
            fullresolution = CHK_fullres.Checked;
        }

        private static class NativeMethods
        {
            [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory")]
            internal static extern void CopyMemory(IntPtr Destination, IntPtr Source, int Length);
        }
    }
}