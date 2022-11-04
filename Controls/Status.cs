using System;
using System.Drawing;
using System.Windows.Forms;
using Timer = System.Threading.Timer;

namespace MissionPlanner.Controls
{
    public partial class Status : UserControl
    {
        private readonly Timer _hidetimer;
        private double _percent = 50;

        public Status()
        {
            InitializeComponent();

            CreateHandle();

            _hidetimer = new Timer(state => { BeginInvoke((Action)delegate { Visible = false; }); }, null, 1, -1);
        }

        public double Percent
        {
            get => _percent;
            set
            {
                if (value < 0 || value > 100)
                    return;

                _percent = value;
                BeginInvoke((Action)delegate { Visible = true; });
                _hidetimer.Change(TimeSpan.FromSeconds(10), TimeSpan.Zero);
                Invalidate();
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x00000020; // WS_EX_TRANSPARENT
                return cp;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_percent > 100)
                _percent = 50;

            try
            {
                e.Graphics.FillRectangle(Brushes.Green, 0, 0, (float)(Width * (_percent / 100.0)), Height);
            }
            catch (OverflowException)
            {
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }
    }
}