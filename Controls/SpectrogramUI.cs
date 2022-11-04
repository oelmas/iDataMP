using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MissionPlanner.Utilities;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using ZedGraph;
using Image = SixLabors.ImageSharp.Image;

namespace MissionPlanner.Controls
{
    public partial class SpectrogramUI : Form
    {
        private DFLogBuffer file;

        public SpectrogramUI()
        {
            InitializeComponent();

            zedGraphControl1.MasterPane[0].Title.Text = "X";
            zedGraphControl1.MasterPane[0].XAxis.Title.Text = "T";
            zedGraphControl1.MasterPane[0].YAxis.Title.Text = "Frequency";

            zedGraphControl1.MasterPane.Add(new GraphPane());
            zedGraphControl1.MasterPane[1].Title.Text = "Y";
            zedGraphControl1.MasterPane[1].XAxis.Title.Text = "T";
            zedGraphControl1.MasterPane[1].YAxis.Title.Text = "Frequency";

            zedGraphControl1.MasterPane.Add(new GraphPane());
            zedGraphControl1.MasterPane[2].Title.Text = "Z";
            zedGraphControl1.MasterPane[2].XAxis.Title.Text = "T";
            zedGraphControl1.MasterPane[2].YAxis.Title.Text = "Frequency";

            zedGraphControl1.AxisChange();

            ThemeManager.ApplyThemeTo(zedGraphControl1);

            zedGraphControl1.Invalidate();
        }

        private void but_loadlog_Click(object sender, EventArgs e)
        {
            using (
                var ofd = new OpenFileDialog())
            {
                ofd.Filter = "*.log;*.bin|*.log;*.bin;*.BIN;*.LOG";

                ofd.ShowDialog();

                if (!File.Exists(ofd.FileName))
                    return;

                file = new DFLogBuffer(File.OpenRead(ofd.FileName));
            }
        }

        private void SpectrogramUI_Resize(object sender, EventArgs e)
        {
        }

        private void cmb_sensor_SelectedIndexChanged(object sender, EventArgs e)
        {
            var field = new[] { "AccX", "AccY", "AccZ" };

            if (cmb_sensor.Text.Contains("GYR")) field = new[] { "GyrX", "GyrY", "GyrZ" };

            List<(double timeus, double[] value)> allfftdata;
            double[] freqt;

            // create X Y Z
            var img1 = Spectrogram.GenerateImage(file, out freqt, out allfftdata, cmb_sensor.Text, field[0],
                min: (int)num_min.Value, max: (int)num_max.Value);
            var img2 = Spectrogram.GenerateImage(file, out freqt, out allfftdata, cmb_sensor.Text, field[1],
                min: (int)num_min.Value, max: (int)num_max.Value);
            var img3 = Spectrogram.GenerateImage(file, out freqt, out allfftdata, cmb_sensor.Text, field[2],
                min: (int)num_min.Value, max: (int)num_max.Value);

            var mintime = allfftdata.Min(a => a.timeus) / 1000000.0;
            var maxtime = allfftdata.Max(a => a.timeus) / 1000000.0;

            var tdelta = maxtime - mintime;

            zedGraphControl1.MasterPane[0].GraphObjList.Clear();
            zedGraphControl1.MasterPane[0].GraphObjList.Add(
                new ImageObj(img1.ToBitmap(), mintime, freqt.Max(), tdelta, freqt.Max())
                    { ZOrder = ZOrder.F_BehindGrid });
            zedGraphControl1.MasterPane[0].XAxis.Scale.Min = mintime;
            zedGraphControl1.MasterPane[0].XAxis.Scale.Max = maxtime;
            zedGraphControl1.MasterPane[0].YAxis.Scale.Min = 0;
            zedGraphControl1.MasterPane[0].YAxis.Scale.Max = freqt.Max();

            zedGraphControl1.MasterPane[1].GraphObjList.Clear();
            zedGraphControl1.MasterPane[1].GraphObjList.Add(
                new ImageObj(img2.ToBitmap(), mintime, freqt.Max(), tdelta, freqt.Max())
                    { ZOrder = ZOrder.F_BehindGrid });
            zedGraphControl1.MasterPane[1].XAxis.Scale.Min = mintime;
            zedGraphControl1.MasterPane[1].XAxis.Scale.Max = maxtime;
            zedGraphControl1.MasterPane[1].YAxis.Scale.Min = 0;
            zedGraphControl1.MasterPane[1].YAxis.Scale.Max = freqt.Max();

            zedGraphControl1.MasterPane[2].GraphObjList.Clear();
            zedGraphControl1.MasterPane[2].GraphObjList.Add(
                new ImageObj(img3.ToBitmap(), mintime, freqt.Max(), tdelta, freqt.Max())
                    { ZOrder = ZOrder.F_BehindGrid });
            zedGraphControl1.MasterPane[2].XAxis.Scale.Min = mintime;
            zedGraphControl1.MasterPane[2].XAxis.Scale.Max = maxtime;
            zedGraphControl1.MasterPane[2].YAxis.Scale.Min = 0;
            zedGraphControl1.MasterPane[2].YAxis.Scale.Max = freqt.Max();

            zedGraphControl1.MasterPane[0].YAxis.Scale.MajorStep = 20;
            zedGraphControl1.MasterPane[0].YAxis.MajorGrid.IsVisible = true;
            zedGraphControl1.MasterPane[1].YAxis.Scale.MajorStep = 20;
            zedGraphControl1.MasterPane[1].YAxis.MajorGrid.IsVisible = true;
            zedGraphControl1.MasterPane[2].YAxis.Scale.MajorStep = 20;
            zedGraphControl1.MasterPane[2].YAxis.MajorGrid.IsVisible = true;

            var bitmap = new Bitmap(10, 10);
            var g = Graphics.FromImage(bitmap);
            zedGraphControl1.MasterPane.DoLayout(g);

            zedGraphControl1.AxisChange();

            zedGraphControl1.Invalidate();
        }

        private void num_min_ValueChanged(object sender, EventArgs e)
        {
            cmb_sensor_SelectedIndexChanged(null, null);
        }

        private void num_max_ValueChanged(object sender, EventArgs e)
        {
            cmb_sensor_SelectedIndexChanged(null, null);
        }

        private void but_redraw_Click(object sender, EventArgs e)
        {
            cmb_sensor_SelectedIndexChanged(null, null);
        }
    }

    public static class ImageSharpExtensions
    {
        public static Bitmap ToBitmap<TPixel>(this Image<TPixel> image) where TPixel : unmanaged, IPixel<TPixel>
        {
            using (var memoryStream = new MemoryStream())
            {
                var imageEncoder = image.GetConfiguration().ImageFormatsManager.FindEncoder(PngFormat.Instance);
                image.Save(memoryStream, imageEncoder);

                memoryStream.Seek(0, SeekOrigin.Begin);

                return new Bitmap(memoryStream);
            }
        }

        public static Image<TPixel> ToImageSharpImage<TPixel>(this Bitmap bitmap)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            using (var memoryStream = new MemoryStream())
            {
                bitmap.Save(memoryStream, ImageFormat.Png);

                memoryStream.Seek(0, SeekOrigin.Begin);

                return Image.Load<TPixel>(memoryStream);
            }
        }
    }
}