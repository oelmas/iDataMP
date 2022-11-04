using System.Drawing;
using System.Drawing.Drawing2D;

namespace MissionPlanner.Controls.Icon
{
    public abstract class Icon
    {
        private Color _backColor = Color.Black;
        private Color _backColorSelected = Color.SeaGreen;
        private Color _foreColor = Color.WhiteSmoke;
        private int _lineWidth = 1;

        public Icon()
        {
            updateColor();
        }

        public Pen LinePen { get; set; }
        public SolidBrush BgSolidBrush { get; set; }
        public SolidBrush BgSelectedSolidBrush { get; set; }

        public Color BackColor
        {
            get => _backColor;
            set
            {
                _backColor = value;
                updateColor();
            }
        }

        public Color BackColorSelected
        {
            get => _backColorSelected;
            set
            {
                _backColorSelected = value;
                updateColor();
            }
        }

        public Color ForeColor
        {
            get => _foreColor;
            set
            {
                _foreColor = value;
                updateColor();
            }
        }

        public int LineWidth
        {
            get => _lineWidth;
            set
            {
                _lineWidth = value;
                updateColor();
            }
        }

        public int Width { get; set; } = 30;

        public int Height { get; set; } = 30;

        public bool IsSelected { get; set; } = false;

        public Point Location { get; set; } = new Point(0, 0);

        public Rectangle Rectangle => new Rectangle(Location.X, Location.Y, Width, Height);

        private void updateColor()
        {
            BgSolidBrush = new SolidBrush(_backColor);
            BgSelectedSolidBrush = new SolidBrush(_backColorSelected);
            LinePen = new Pen(_foreColor, _lineWidth);
        }

        public void Paint(Graphics g)
        {
            // move 0,0 to out start location - no clipping is used, so we can draw anywhere on the parent control
            g.TranslateTransform(Location.X, Location.Y);
            g.InterpolationMode = InterpolationMode.High;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = new Rectangle(0, 0, Width, Height);

            if (IsSelected)
                g.FillPie(BgSelectedSolidBrush, rect, 0, 360);
            else
                g.FillPie(BgSolidBrush, rect, 0, 360);
            g.DrawArc(LinePen, rect, 0, 360);

            doPaint(g);
        }

        internal abstract void doPaint(Graphics g);
    }
}