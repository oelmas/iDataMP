using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using MissionPlanner.Utilities;
using ZedGraph;

namespace MissionPlanner.Controls
{
    public partial class fftui : Form
    {
        private bool indB = true;

        private double prevMouseX;
        private double prevMouseY;

        public fftui()
        {
            DoubleBuffered = true;
            InitializeComponent();
        }

        private void BUT_runwav_Click(object sender, EventArgs e)
        {
            var fft = new FFT2();

            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "*.wav|*.wav";

                ofd.ShowDialog();

                if (!File.Exists(ofd.FileName))
                    return;

                var st = File.OpenRead(ofd.FileName);

                var bins = (int)NUM_bins.Value;

                var avg = new List<double[]>
                {
                    new double[1 << bins]
                };
                Color[] color = { Color.Red, Color.Green, Color.Black, Color.Violet, Color.Blue, Color.Orange };

                var hz = 8000;
                InputBox.Show("fft sample rate", "nter source file sample rate", ref hz);

                var buffer = new double[1 << bins];

                var a = 0;
                var samples = 0;

                while (st.Position < st.Length)
                {
                    var temp = new byte[2];
                    var read = st.Read(temp, 0, temp.Length);

                    var val = (double)BitConverter.ToInt16(temp, 0);

                    buffer[a] = val;

                    a++;

                    if (a == 1 << bins)
                    {
                        samples++;

                        var fftanswer = fft.rin(buffer, (uint)bins, indB);

                        var freqt = fft.FreqTable(buffer.Length, hz);

                        var ppl = new PointPairList();

                        for (var b = 0; b < fftanswer.Length; b++) ppl.Add(freqt[b], fftanswer[b]);

                        double xMin, xMax, yMin, yMax;

                        var curve = new LineItem("FFT", ppl, Color.Red, SymbolType.Diamond);

                        curve.GetRange(out xMin, out xMax, out yMin, out yMax, true, false, zedGraphControl1.GraphPane);

                        zedGraphControl1.GraphPane.XAxis.Title.Text = "Freq Hz";
                        zedGraphControl1.GraphPane.YAxis.Title.Text = "Amplitude";
                        zedGraphControl1.GraphPane.Title.Text = "FFT - " + hz;
                        zedGraphControl1.GraphPane.Y2Axis.IsVisible = true;
                        zedGraphControl1.GraphPane.CurveList.Clear();
                        zedGraphControl1.GraphPane.CurveList.Add(curve);

                        for (var b = 0; b < (1 << bins) / 2; b++)
                            avg[0][b] = avg[0][b] * (1.0 - 1.0 / samples) + fftanswer[b] * (1.0 / samples);

                        // 0 out all data befor cutoff
                        for (var b = 0; b < 1 << (bins / 2); b++)
                        {
                            if (freqt[b] < (double)NUM_startfreq.Value)
                            {
                                avg[0][b] = 0;
                                continue;
                            }

                            break;
                        }

                        ppl = new PointPairList(freqt, avg[0]);
                        curve = new LineItem("Avg", ppl, color[1], SymbolType.None) { IsY2Axis = true };
                        zedGraphControl1.GraphPane.CurveList.Add(curve);

                        zedGraphControl1.Invalidate();
                        zedGraphControl1.AxisChange();

                        zedGraphControl1.Refresh();

                        // 50% overlap
                        st.Seek(-(1 << bins) / 2, SeekOrigin.Current);
                        a = 0;
                        buffer = new double[buffer.Length];
                    }
                }
            }
        }

        //FMT, 131, 43, IMU, IffffffIIf, TimeMS,GyrX,GyrY,GyrZ,AccX,AccY,AccZ,ErrG,ErrA,Temp
        //FMT, 135, 43, IMU2, IffffffIIf, TimeMS,GyrX,GyrY,GyrZ,AccX,AccY,AccZ,ErrG,ErrA,Temp
        //FMT, 149, 43, IMU3, IffffffIIf, TimeMS,GyrX,GyrY,GyrZ,AccX,AccY,AccZ,ErrG,ErrA,Temp

        //FMT, 172, 23, ACC1, IIfff, TimeMS,TimeUS,AccX,AccY,AccZ
        //FMT, 173, 23, ACC2, IIfff, TimeMS,TimeUS,AccX,AccY,AccZ
        //FMT, 174, 23, ACC3, IIfff, TimeMS,TimeUS,AccX,AccY,AccZ
        //FMT, 175, 23, GYR1, IIfff, TimeMS,TimeUS,GyrX,GyrY,GyrZ
        //FMT, 176, 23, GYR2, IIfff, TimeMS,TimeUS,GyrX,GyrY,GyrZ
        //FMT, 177, 23, GYR3, IIfff, TimeMS,TimeUS,GyrX,GyrY,GyrZ

        private void acc1gyr1myButton1_Click(object sender, EventArgs e)
        {
            var fft = new FFT2();

            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "*.log;*.bin|*.log;*.bin;*.BIN;*.LOG";

                ofd.ShowDialog();

                if (!File.Exists(ofd.FileName))
                    return;

                var file = new DFLogBuffer(File.OpenRead(ofd.FileName));

                var bins = (int)NUM_bins.Value;

                var N = 1 << bins;

                var datainGX = new double[N];
                var datainGY = new double[N];
                var datainGZ = new double[N];
                var datainAX = new double[N];
                var datainAY = new double[N];
                var datainAZ = new double[N];

                var avg = new List<double[]>
                {
                    // 6
                    new double[N / 2],
                    new double[N / 2],
                    new double[N / 2],
                    new double[N / 2],
                    new double[N / 2],
                    new double[N / 2]
                };

                object[] datas = { datainGX, datainGY, datainGZ, datainAX, datainAY, datainAZ };
                string[] datashead = { "GYR1-GyrX", "GYR1-GyrY", "GYR1-GyrZ", "ACC1-AccX", "ACC1-AccY", "ACC1-AccZ" };
                Color[] color = { Color.Red, Color.Green, Color.Black, Color.Violet, Color.Blue, Color.Orange };
                ZedGraphControl[] ctls =
                {
                    zedGraphControl1, zedGraphControl2, zedGraphControl3, zedGraphControl4, zedGraphControl5,
                    zedGraphControl6
                };

                var samplecounta = 0;
                var samplecountg = 0;

                double lasttime = 0;
                double timedelta = 0;
                double[] freqt = null;
                double samplerate = 0;

                foreach (var item in file.GetEnumeratorType(new[] { "ACC1", "GYR1" }))
                {
                    if (item.msgtype == "ACC1" || (item.msgtype == "ACC" && item.instance == "0"))
                    {
                        var offsetAX = file.dflog.FindMessageOffset(item.msgtype, "AccX");
                        var offsetAY = file.dflog.FindMessageOffset(item.msgtype, "AccY");
                        var offsetAZ = file.dflog.FindMessageOffset(item.msgtype, "AccZ");
                        var offsetTime = file.dflog.FindMessageOffset(item.msgtype, "TimeUS");

                        var time = double.Parse(item.items[offsetTime],
                            CultureInfo.InvariantCulture) / 1000.0;

                        timedelta = timedelta * 0.99 + (time - lasttime) * 0.01;

                        // we missed gyro data
                        if (samplecounta >= N)
                            continue;

                        datainAX[samplecounta] = double.Parse(item.items[offsetAX],
                            CultureInfo.InvariantCulture);
                        datainAY[samplecounta] = double.Parse(item.items[offsetAY],
                            CultureInfo.InvariantCulture);
                        datainAZ[samplecounta] = double.Parse(item.items[offsetAZ],
                            CultureInfo.InvariantCulture);

                        samplecounta++;

                        lasttime = time;
                    }
                    else if (item.msgtype == "GYR1" || (item.msgtype == "GYR" && item.instance == "0"))
                    {
                        var offsetGX = file.dflog.FindMessageOffset(item.msgtype, "GyrX");
                        var offsetGY = file.dflog.FindMessageOffset(item.msgtype, "GyrY");
                        var offsetGZ = file.dflog.FindMessageOffset(item.msgtype, "GyrZ");
                        var offsetTime = file.dflog.FindMessageOffset(item.msgtype, "TimeUS");

                        var time = double.Parse(item.items[offsetTime],
                            CultureInfo.InvariantCulture) / 1000.0;

                        // we missed accel data
                        if (samplecountg >= N)
                            continue;

                        datainGX[samplecountg] = double.Parse(item.items[offsetGX],
                            CultureInfo.InvariantCulture);
                        datainGY[samplecountg] = double.Parse(item.items[offsetGY],
                            CultureInfo.InvariantCulture);
                        datainGZ[samplecountg] = double.Parse(item.items[offsetGZ],
                            CultureInfo.InvariantCulture);

                        samplecountg++;
                    }

                    if (samplecounta >= N && samplecountg >= N)
                    {
                        var inputdataindex = 0;

                        foreach (var itemlist in datas)
                        {
                            var fftanswer = fft.rin((double[])itemlist, (uint)bins, indB);

                            for (var b = 0; b < N / 2; b++) avg[inputdataindex][b] += fftanswer[b] * (1.0 / (N / 2.0));

                            samplecounta = 0;
                            samplecountg = 0;
                            inputdataindex++;
                        }
                    }
                }

                if (freqt == null)
                {
                    samplerate = Math.Round(1000 / timedelta, 1);
                    freqt = fft.FreqTable(N, (int)samplerate);
                }

                // 0 out all data befor cutoff
                for (var inputdataindex = 0; inputdataindex < 6; inputdataindex++)
                for (var b = 0; b < N / 2; b++)
                {
                    if (freqt[b] < (double)NUM_startfreq.Value)
                    {
                        avg[inputdataindex][b] = 0;
                        continue;
                    }

                    break;
                }

                var controlindex = 0;
                foreach (var item in avg)
                {
                    var ppl = new PointPairList(freqt, item);

                    //double xMin, xMax, yMin, yMax;

                    var curve = new LineItem(datashead[controlindex], ppl, color[controlindex], SymbolType.None);

                    //curve.GetRange(out xMin, out xMax, out yMin, out  yMax, true, false, ctls[c].GraphPane);

                    ctls[controlindex].GraphPane.Legend.IsVisible = false;

                    ctls[controlindex].GraphPane.XAxis.Title.Text = "Freq Hz";
                    ctls[controlindex].GraphPane.YAxis.Title.Text = "Amplitude";
                    ctls[controlindex].GraphPane.Title.Text = "FFT " + datashead[controlindex] + " - " +
                                                              Path.GetFileName(ofd.FileName) + " - " + samplerate +
                                                              "hz input";

                    ctls[controlindex].GraphPane.CurveList.Clear();

                    ctls[controlindex].GraphPane.CurveList.Add(curve);

                    ctls[controlindex].Invalidate();
                    ctls[controlindex].AxisChange();

                    ctls[controlindex].Refresh();

                    controlindex++;
                }

                SetScale(ctls);
            }
        }


        private void BUT_accgyrall_Click(object sender, EventArgs e)
        {
            var fft = new FFT2();
            using (
                var ofd = new OpenFileDialog())
            {
                ofd.Filter = "*.log;*.bin|*.log;*.bin;*.BIN;*.LOG";

                ofd.ShowDialog();

                if (!File.Exists(ofd.FileName))
                    return;

                var file = new DFLogBuffer(File.OpenRead(ofd.FileName));

                var bins = (int)NUM_bins.Value;

                var N = 1 << bins;

                Color[] color = { Color.Red, Color.Green, Color.Blue, Color.Black, Color.Violet, Color.Orange };
                ZedGraphControl[] ctls =
                {
                    zedGraphControl1, zedGraphControl2, zedGraphControl3, zedGraphControl4, zedGraphControl5,
                    zedGraphControl6
                };

                // 3 imus * 2 sets of measurements(gyr/acc)
                var alldata = new FFT2.datastate[3 * 2];
                for (var a = 0; a < alldata.Length; a++)
                    alldata[a] = new FFT2.datastate();

                int offsetAX = 0, offsetAY = 0, offsetAZ = 0, offsetTimeacc = 0;
                int offsetGX = 0, offsetGY = 0, offsetGZ = 0, offsetTimegyr = 0;
                var second = DateTime.Now.Second;
                long linesdone = 0;

                foreach (var item in file.GetEnumeratorType(new[]
                             { "ACC1", "GYR1", "ACC2", "GYR2", "ACC3", "GYR3", "ACC4", "GYR4" }))
                {
                    linesdone++;
                    if (second != DateTime.Now.Second)
                    {
                        Console.WriteLine(linesdone);
                        second = DateTime.Now.Second;
                    }

                    if (item.msgtype == null) continue;

                    if (item.msgtype.StartsWith("ACC"))
                    {
                        var sensorno = item.instance == ""
                            ? int.Parse(item.msgtype.Substring(3),
                                CultureInfo.InvariantCulture) - 1 + 3
                            : int.Parse(item.instance) + 3;
                        alldata[sensorno].type = item.msgtype;

                        if (offsetAX == 0)
                            offsetAX = file.dflog.FindMessageOffset(item.msgtype, "AccX");
                        if (offsetAY == 0)
                            offsetAY = file.dflog.FindMessageOffset(item.msgtype, "AccY");
                        if (offsetAZ == 0)
                            offsetAZ = file.dflog.FindMessageOffset(item.msgtype, "AccZ");
                        if (offsetTimeacc == 0)
                            offsetTimeacc = file.dflog.FindMessageOffset(item.msgtype, "TimeUS");

                        var time = Convert.ToDouble(item.raw[offsetTimeacc],
                            CultureInfo.InvariantCulture) / 1000.0;

                        if (time < alldata[sensorno].lasttime)
                            continue;

                        if (time != alldata[sensorno].lasttime)
                            alldata[sensorno].timedelta = alldata[sensorno].timedelta * 0.99 +
                                                          (time - alldata[sensorno].lasttime) * 0.01;

                        alldata[sensorno].lasttime = time;

                        alldata[sensorno].datax.Add(Convert.ToDouble(item.raw[offsetAX],
                            CultureInfo.InvariantCulture));
                        alldata[sensorno].datay.Add(Convert.ToDouble(item.raw[offsetAY],
                            CultureInfo.InvariantCulture));
                        alldata[sensorno].dataz.Add(Convert.ToDouble(item.raw[offsetAZ],
                            CultureInfo.InvariantCulture));
                    }
                    else if (item.msgtype.StartsWith("GYR"))
                    {
                        var sensorno = item.instance == ""
                            ? int.Parse(item.msgtype.Substring(3),
                                CultureInfo.InvariantCulture) - 1
                            : int.Parse(item.instance);
                        alldata[sensorno].type = item.msgtype;

                        if (offsetGX == 0) offsetGX = file.dflog.FindMessageOffset(item.msgtype, "GyrX");
                        if (offsetGY == 0) offsetGY = file.dflog.FindMessageOffset(item.msgtype, "GyrY");
                        if (offsetGZ == 0) offsetGZ = file.dflog.FindMessageOffset(item.msgtype, "GyrZ");
                        if (offsetTimegyr == 0) offsetTimegyr = file.dflog.FindMessageOffset(item.msgtype, "TimeUS");

                        var time = Convert.ToDouble(item.raw[offsetTimegyr],
                            CultureInfo.InvariantCulture) / 1000.0;

                        if (time < alldata[sensorno].lasttime)
                            continue;

                        if (time != alldata[sensorno].lasttime)
                            alldata[sensorno].timedelta = alldata[sensorno].timedelta * 0.99 +
                                                          (time - alldata[sensorno].lasttime) * 0.01;

                        alldata[sensorno].lasttime = time;

                        alldata[sensorno].datax.Add(Convert.ToDouble(item.raw[offsetGX],
                            CultureInfo.InvariantCulture));
                        alldata[sensorno].datay.Add(Convert.ToDouble(item.raw[offsetGY],
                            CultureInfo.InvariantCulture));
                        alldata[sensorno].dataz.Add(Convert.ToDouble(item.raw[offsetGZ],
                            CultureInfo.InvariantCulture));
                    }
                }

                var controlindex = 0;

                foreach (var sensordata in alldata)
                {
                    if (sensordata.datax.Count <= N)
                        continue;

                    double samplerate = 0;

                    samplerate = Math.Round(1000 / sensordata.timedelta, 1);

                    var freqt = fft.FreqTable(N, (int)samplerate);

                    var avgx = new double[N / 2];
                    var avgy = new double[N / 2];
                    var avgz = new double[N / 2];

                    var totalsamples = sensordata.datax.Count;
                    var count = totalsamples / N;
                    var done = 0;
                    while (count > 1) // skip last part
                    {
                        var fftanswerx = fft.rin(sensordata.datax.AsSpan().Slice(N * done, N), (uint)bins, indB);
                        var fftanswery = fft.rin(sensordata.datay.AsSpan().Slice(N * done, N), (uint)bins, indB);
                        var fftanswerz = fft.rin(sensordata.dataz.AsSpan().Slice(N * done, N), (uint)bins, indB);

                        for (var b = 0; b < N / 2; b++)
                        {
                            if (freqt[b] < (double)NUM_startfreq.Value)
                                continue;

                            avgx[b] += fftanswerx[b] / (done + count);
                            avgy[b] += fftanswery[b] / (done + count);
                            avgz[b] += fftanswerz[b] / (done + count);
                        }

                        count--;
                        done++;
                    }

                    var pplx = new PointPairList(freqt, avgx);
                    var pply = new PointPairList(freqt, avgy);
                    var pplz = new PointPairList(freqt, avgz);

                    var curvex = new LineItem(sensordata.type + " x", pplx, color[0], SymbolType.None);
                    var curvey = new LineItem(sensordata.type + " y", pply, color[1], SymbolType.None);
                    var curvez = new LineItem(sensordata.type + " z", pplz, color[2], SymbolType.None);

                    ctls[controlindex].GraphPane.Legend.IsVisible = true;

                    ctls[controlindex].GraphPane.XAxis.Title.Text = "Freq Hz";
                    ctls[controlindex].GraphPane.YAxis.Title.Text = "Amplitude";
                    ctls[controlindex].GraphPane.Title.Text = "FFT " + sensordata.type + " - " +
                                                              Path.GetFileName(ofd.FileName) + " - " + samplerate +
                                                              "hz input";

                    ctls[controlindex].GraphPane.CurveList.Clear();

                    ctls[controlindex].GraphPane.CurveList.Add(curvex);
                    ctls[controlindex].GraphPane.CurveList.Add(curvey);
                    ctls[controlindex].GraphPane.CurveList.Add(curvez);

                    ctls[controlindex].Invalidate();
                    ctls[controlindex].AxisChange();

                    ctls[controlindex].GraphPane.XAxis.Scale.Max = samplerate / 2;

                    ctls[controlindex].Refresh();

                    controlindex++;
                }

                SetScale(ctls);
            }
        }

        private void SetScale(ZedGraphControl[] ctls)
        {
            // get the max scale
            double maxg = 0;
            double maxa = 0;
            foreach (var zedGraphControl in ctls)
                if (zedGraphControl.GraphPane.Title.Text.Contains("GYR"))
                    maxg = Math.Max(maxg, zedGraphControl.GraphPane.YAxis.Scale.Max);
                else if (zedGraphControl.GraphPane.Title.Text.Contains("ACC"))
                    maxa = Math.Max(maxa, zedGraphControl.GraphPane.YAxis.Scale.Max);
            // set the max scale
            foreach (var zedGraphControl in ctls)
                if (zedGraphControl.GraphPane.Title.Text.Contains("GYR"))
                    zedGraphControl.GraphPane.YAxis.Scale.Max = maxg;
                else if (zedGraphControl.GraphPane.Title.Text.Contains("ACC"))
                    zedGraphControl.GraphPane.YAxis.Scale.Max = maxa;
        }

        private string zedGraphControl_PointValueEvent(ZedGraphControl sender, GraphPane pane, CurveItem curve, int iPt)
        {
            return string.Format("{0} hz/{1} rpm", curve[iPt].X, curve[iPt].X * 60.0);
        }

        private void but_fftimu13_Click(object sender, EventArgs e)
        {
            var fft = new FFT2();
            using (
                var ofd = new OpenFileDialog())
            {
                ofd.Filter = "*.log;*.bin|*.log;*.bin;*.BIN;*.LOG";

                ofd.ShowDialog();

                if (!File.Exists(ofd.FileName))
                    return;

                var file = new DFLogBuffer(File.OpenRead(ofd.FileName));

                var bins = (int)NUM_bins.Value;

                var N = 1 << bins;

                Color[] color = { Color.Red, Color.Green, Color.Blue, Color.Black, Color.Violet, Color.Orange };
                ZedGraphControl[] ctls =
                {
                    zedGraphControl1, zedGraphControl2, zedGraphControl3, zedGraphControl4, zedGraphControl5,
                    zedGraphControl6
                };

                // 3 imus * 2 sets of measurements(gyr/acc)
                var alldata = new FFT2.datastate[3 * 2];
                for (var a = 0; a < alldata.Length; a++)
                    alldata[a] = new FFT2.datastate();

                foreach (var item in file.GetEnumeratorType(new[] { "IMU", "IMU2", "IMU3" }))
                {
                    if (item.msgtype == null) continue;

                    if (item.msgtype.StartsWith("IMU"))
                    {
                        var sensorno = 0;
                        if (item.msgtype == "IMU")
                            sensorno = 0;
                        if (item.msgtype == "IMU2")
                            sensorno = 1;
                        if (item.msgtype == "IMU3")
                            sensorno = 2;

                        alldata[sensorno + 3].type = item.msgtype + " ACC";

                        var offsetAX = file.dflog.FindMessageOffset(item.msgtype, "AccX");
                        var offsetAY = file.dflog.FindMessageOffset(item.msgtype, "AccY");
                        var offsetAZ = file.dflog.FindMessageOffset(item.msgtype, "AccZ");
                        var offsetTime = file.dflog.FindMessageOffset(item.msgtype, "TimeUS");

                        var time = double.Parse(item.items[offsetTime],
                            CultureInfo.InvariantCulture) / 1000.0;

                        if (time != alldata[sensorno + 3].lasttime)
                            alldata[sensorno + 3].timedelta = alldata[sensorno + 3].timedelta * 0.99 +
                                                              (time - alldata[sensorno + 3].lasttime) * 0.01;

                        alldata[sensorno + 3].lasttime = time;

                        alldata[sensorno + 3].datax.Add(double.Parse(item.items[offsetAX],
                            CultureInfo.InvariantCulture));
                        alldata[sensorno + 3].datay.Add(double.Parse(item.items[offsetAY],
                            CultureInfo.InvariantCulture));
                        alldata[sensorno + 3].dataz.Add(double.Parse(item.items[offsetAZ],
                            CultureInfo.InvariantCulture));

                        //gyro
                        alldata[sensorno].type = item.msgtype + " GYR";

                        var offsetGX = file.dflog.FindMessageOffset(item.msgtype, "GyrX");
                        var offsetGY = file.dflog.FindMessageOffset(item.msgtype, "GyrY");
                        var offsetGZ = file.dflog.FindMessageOffset(item.msgtype, "GyrZ");

                        if (time != alldata[sensorno].lasttime)
                            alldata[sensorno].timedelta = alldata[sensorno].timedelta * 0.99 +
                                                          (time - alldata[sensorno].lasttime) * 0.01;

                        alldata[sensorno].lasttime = time;

                        alldata[sensorno].datax.Add(double.Parse(item.items[offsetGX],
                            CultureInfo.InvariantCulture));
                        alldata[sensorno].datay.Add(double.Parse(item.items[offsetGY],
                            CultureInfo.InvariantCulture));
                        alldata[sensorno].dataz.Add(double.Parse(item.items[offsetGZ],
                            CultureInfo.InvariantCulture));
                    }
                }

                var controlindex = 0;

                foreach (var sensordata in alldata)
                {
                    if (sensordata.datax.Count <= N)
                        continue;

                    double samplerate = 0;

                    samplerate = Math.Round(1000 / sensordata.timedelta, 1);

                    var freqt = fft.FreqTable(N, (int)samplerate);

                    var avgx = new double[N / 2];
                    var avgy = new double[N / 2];
                    var avgz = new double[N / 2];

                    var totalsamples = sensordata.datax.Count;
                    var count = totalsamples / N;
                    var done = 0;
                    while (count > 1) // skip last part
                    {
                        var fftanswerx = fft.rin(sensordata.datax.AsSpan().Slice(N * done, N), (uint)bins, indB);
                        var fftanswery = fft.rin(sensordata.datay.AsSpan().Slice(N * done, N), (uint)bins, indB);
                        var fftanswerz = fft.rin(sensordata.dataz.AsSpan().Slice(N * done, N), (uint)bins, indB);

                        for (var b = 0; b < N / 2; b++)
                        {
                            if (freqt[b] < (double)NUM_startfreq.Value)
                                continue;

                            avgx[b] += fftanswerx[b] / (done + count);
                            avgy[b] += fftanswery[b] / (done + count);
                            avgz[b] += fftanswerz[b] / (done + count);
                        }

                        count--;
                        done++;
                    }

                    var pplx = new PointPairList(freqt, avgx);
                    var pply = new PointPairList(freqt, avgy);
                    var pplz = new PointPairList(freqt, avgz);

                    var curvex = new LineItem(sensordata.type + " x", pplx, color[0], SymbolType.None);
                    var curvey = new LineItem(sensordata.type + " y", pply, color[1], SymbolType.None);
                    var curvez = new LineItem(sensordata.type + " z", pplz, color[2], SymbolType.None);

                    ctls[controlindex].GraphPane.Legend.IsVisible = true;

                    ctls[controlindex].GraphPane.XAxis.Title.Text = "Freq Hz";
                    ctls[controlindex].GraphPane.YAxis.Title.Text = "Amplitude";
                    ctls[controlindex].GraphPane.Title.Text = "FFT " + sensordata.type + " - " +
                                                              Path.GetFileName(ofd.FileName) + " - " + samplerate +
                                                              "hz input";

                    ctls[controlindex].GraphPane.CurveList.Clear();

                    ctls[controlindex].GraphPane.CurveList.Add(curvex);
                    ctls[controlindex].GraphPane.CurveList.Add(curvey);
                    ctls[controlindex].GraphPane.CurveList.Add(curvez);

                    ctls[controlindex].Invalidate();
                    ctls[controlindex].AxisChange();

                    ctls[controlindex].GraphPane.XAxis.Scale.Max = samplerate / 2;

                    ctls[controlindex].Refresh();

                    controlindex++;
                }

                SetScale(ctls);
            }
        }

        private bool zedGraphControl1_MouseMoveEvent(ZedGraphControl sender, MouseEventArgs e)
        {
            // debounce for mousemove and tooltip label

            if (e.X == prevMouseX && e.Y == prevMouseY)
                return true;

            prevMouseX = e.X;
            prevMouseY = e.Y;

            // not handled
            return false;
        }

        private void but_ISBH_Click(object sender, EventArgs e)
        {
            var fft = new FFT2();
            using (
                var ofd = new OpenFileDialog())
            {
                ofd.Filter = "*.log;*.bin|*.log;*.bin;*.BIN;*.LOG";

                ofd.ShowDialog();

                if (!File.Exists(ofd.FileName))
                    return;

                var file = new DFLogBuffer(File.OpenRead(ofd.FileName));

                var bins = (int)NUM_bins.Value;

                var N = 1 << bins;

                Color[] color = { Color.Red, Color.Green, Color.Blue, Color.Black, Color.Violet, Color.Orange };
                ZedGraphControl[] ctls =
                {
                    zedGraphControl1, zedGraphControl2, zedGraphControl3, zedGraphControl4, zedGraphControl5,
                    zedGraphControl6
                };

                // 3 imus * 2 sets of measurements(gyr/acc)
                var alldata = new FFT2.datastate[3 * 2];
                for (var a = 0; a < alldata.Length; a++)
                    alldata[a] = new FFT2.datastate();

                // state cache
                var Ns = 0;
                var type = 0;
                var instance = 0;
                var sensorno = 0;
                double multiplier = -1;

                int offsetX = 0, offsetY = 0, offsetZ = 0, offsetTime = 0;

                foreach (var item in file.GetEnumeratorType(new[] { "ISBH", "ISBD" }))
                {
                    if (item.msgtype == null) continue;

                    if (item.msgtype.StartsWith("ISBH"))
                    {
                        Ns = int.Parse(item.items[file.dflog.FindMessageOffset(item.msgtype, "N")],
                            CultureInfo.InvariantCulture);
                        type = int.Parse(item.items[file.dflog.FindMessageOffset(item.msgtype, "type")],
                            CultureInfo.InvariantCulture);
                        instance = int.Parse(item.items[file.dflog.FindMessageOffset(item.msgtype, "instance")],
                            CultureInfo.InvariantCulture);

                        sensorno = type * 3 + instance;

                        alldata[sensorno].sample_rate = double.Parse(
                            item.items[file.dflog.FindMessageOffset(item.msgtype, "smp_rate")],
                            CultureInfo.InvariantCulture);

                        multiplier = double.Parse(
                            item.items[file.dflog.FindMessageOffset(item.msgtype, "mul")],
                            CultureInfo.InvariantCulture);

                        if (type == 0)
                            alldata[sensorno].type = "ACC" + instance;
                        if (type == 1)
                            alldata[sensorno].type = "GYR" + instance;
                    }
                    else if (item.msgtype.StartsWith("ISBD"))
                    {
                        var Nsdata = Convert.ToInt32(item.GetRaw("N"),
                            CultureInfo.InvariantCulture);

                        if (Ns != Nsdata)
                            continue;

                        if (offsetX == 0) offsetX = file.dflog.FindMessageOffset(item.msgtype, "x");
                        if (offsetY == 0) offsetY = file.dflog.FindMessageOffset(item.msgtype, "y");
                        if (offsetZ == 0) offsetZ = file.dflog.FindMessageOffset(item.msgtype, "z");
                        if (offsetTime == 0) offsetTime = file.dflog.FindMessageOffset(item.msgtype, "TimeUS");

                        var time = Convert.ToDouble(item.raw[offsetTime],
                            CultureInfo.InvariantCulture) / 1000.0;

                        if (time < alldata[sensorno].lasttime)
                            continue;

                        if (time != alldata[sensorno].lasttime)
                            alldata[sensorno].timedelta = alldata[sensorno].timedelta * 0.99 +
                                                          (time - alldata[sensorno].lasttime) * 0.01;

                        alldata[sensorno].lasttime = time;

                        var ua = (BinaryLog.UnionArray)item.raw[offsetX];
                        ua.Shorts.ForEach(aa => { alldata[sensorno].datax.Add(aa / multiplier); });
                        ua = (BinaryLog.UnionArray)item.raw[offsetY];
                        ua.Shorts.ForEach(aa => { alldata[sensorno].datay.Add(aa / multiplier); });
                        ua = (BinaryLog.UnionArray)item.raw[offsetZ];
                        ua.Shorts.ForEach(aa => { alldata[sensorno].dataz.Add(aa / multiplier); });
                    }
                }

                var controlindex = 0;

                foreach (var sensordata in alldata)
                {
                    if (sensordata.datax.Count <= N)
                        continue;

                    double samplerate = 0;

                    samplerate = sensordata.sample_rate; // Math.Round(1000 / sensordata.timedelta, 1);

                    var freqt = fft.FreqTable(N, (int)samplerate);

                    var avgx = new double[N / 2];
                    var avgy = new double[N / 2];
                    var avgz = new double[N / 2];

                    var totalsamples = sensordata.datax.Count;
                    var count = totalsamples / N;
                    var done = 0;
                    while (count > 1) // skip last part
                    {
                        var fftanswerx = fft.rin(sensordata.datax.AsSpan().Slice(N * done, N), (uint)bins, indB);
                        var fftanswery = fft.rin(sensordata.datay.AsSpan().Slice(N * done, N), (uint)bins, indB);
                        var fftanswerz = fft.rin(sensordata.dataz.AsSpan().Slice(N * done, N), (uint)bins, indB);

                        for (var b = 0; b < N / 2; b++)
                        {
                            if (freqt[b] < (double)NUM_startfreq.Value)
                                continue;

                            avgx[b] += fftanswerx[b] / (done + count);
                            avgy[b] += fftanswery[b] / (done + count);
                            avgz[b] += fftanswerz[b] / (done + count);
                        }

                        count--;
                        done++;
                    }

                    var pplx = new PointPairList(freqt, avgx);
                    var pply = new PointPairList(freqt, avgy);
                    var pplz = new PointPairList(freqt, avgz);

                    var curvex = new LineItem(sensordata.type + " x", pplx, color[0], SymbolType.None);
                    var curvey = new LineItem(sensordata.type + " y", pply, color[1], SymbolType.None);
                    var curvez = new LineItem(sensordata.type + " z", pplz, color[2], SymbolType.None);

                    ctls[controlindex].GraphPane.Legend.IsVisible = true;

                    ctls[controlindex].GraphPane.XAxis.Title.Text = "Freq Hz";
                    ctls[controlindex].GraphPane.YAxis.Title.Text = "Amplitude";
                    ctls[controlindex].GraphPane.Title.Text = "FFT " + sensordata.type + " - " +
                                                              Path.GetFileName(ofd.FileName) + " - " + samplerate +
                                                              "hz input";

                    ctls[controlindex].GraphPane.CurveList.Clear();

                    ctls[controlindex].GraphPane.CurveList.Add(curvex);
                    ctls[controlindex].GraphPane.CurveList.Add(curvey);
                    ctls[controlindex].GraphPane.CurveList.Add(curvez);

                    ctls[controlindex].Invalidate();
                    ctls[controlindex].AxisChange();

                    ctls[controlindex].GraphPane.XAxis.Scale.Max = samplerate / 2;

                    ctls[controlindex].Refresh();

                    controlindex++;
                }

                SetScale(ctls);
            }
        }

        private void chk_mag_CheckedChanged(object sender, EventArgs e)
        {
            indB = !chk_mag.Checked;
        }
    }
}