using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;
using MissionPlanner.Properties;

namespace MissionPlanner.Controls
{
    public partial class DistanceBar : UserControl
    {
        private readonly Brush _brushbar = new SolidBrush(Color.FromArgb(50, Color.White));


        private readonly Bitmap icon = Resources.marker_05;

        private float _traveleddist;

        private Bitmap buffer = new Bitmap(640, 480);

        private readonly object locker = new object();
        private readonly List<float> wpdist = new List<float>();

        public DistanceBar()
        {
            //SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            //SetStyle(ControlStyles.Opaque, true);

            DoubleBuffered = false;

            InitializeComponent();

            totaldist = 100;


            //this.BackColor = Color.Transparent;

            ClearWPDist();
        }

        public float totaldist { get; set; }

        public float traveleddist
        {
            get => _traveleddist;
            set
            {
                _traveleddist = value;
                Invalidate();
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var parms = base.CreateParams;
                //parms.ExStyle |= 0x20;
                return parms;
            }
        }

        public void AddWPDist(float dist)
        {
            lock (locker)
            {
                wpdist.Add(dist);
                totaldist = wpdist.Sum();
            }
        }

        public void ClearWPDist()
        {
            lock (locker)
            {
                wpdist.Clear();
                wpdist.Add(0);
            }
        }

        public void DoPaintRemote(PaintEventArgs e)
        {
            var matrix = new Matrix();
            matrix.Translate(Left, Top);
            e.Graphics.Transform = matrix;
            OnPaint(e);
            e.Graphics.ResetTransform();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);

            if (Parent != null)
            {
                //Parent.Invalidate(this.Bounds, true);
            }

            try
            {
                using (var etemp = Graphics.FromImage(buffer))
                {
                    if (totaldist <= 0)
                        totaldist = 100;

                    // bar

                    var bar = new RectangleF(4, 4, Width - 8, Height - 8);

                    etemp.Clear(Color.Transparent);

                    etemp.FillRectangle(_brushbar, bar);

                    // draw bar traveled

                    var bartrav = new RectangleF(bar.X, bar.Y, bar.Width * (traveleddist / totaldist), bar.Height);

                    etemp.FillRectangle(_brushbar, bartrav);
                    etemp.FillRectangle(_brushbar, bartrav);
                    etemp.FillRectangle(_brushbar, bartrav);
                    etemp.FillRectangle(_brushbar, bartrav);
                    etemp.FillRectangle(_brushbar, bartrav);

                    // draw wp dist

                    lock (locker)
                    {
                        var iconwidth = Height / 4.0f;
                        float trav = 0;
                        foreach (var disttrav in wpdist)
                        {
                            trav += disttrav;

                            if (trav > totaldist)
                                trav = totaldist;

                            etemp.FillPie(Brushes.Yellow, bar.X + bar.Width * (trav / totaldist) - iconwidth / 2,
                                bar.Top,
                                bar.Height / 2, bar.Height, 0, 360);
                            //e.Graphics.DrawImage(icon, (bar.X + bar.Width * (trav / totaldist)) - iconwidth / 2, 1, iconwidth, bar.Height);
                        }
                    }

                    // draw dist traveled

                    var dist = traveleddist.ToString("0");

                    etemp.DrawString(dist, Font, new SolidBrush(ForeColor), bartrav.Right,
                        bartrav.Bottom - FontHeight);

                    e.Graphics.DrawImageUnscaled(buffer, 0, 0);
                }
            }
            catch (Exception)
            {
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (Width == 0 || Height == 0)
                return;

            buffer = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            // base.OnParentBackColorChanged(e);
        }
    }
}