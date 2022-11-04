using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using com.utils.xml;

namespace MissionPlanner.Controls
{
    [DefaultEvent("OnSelectedIndexChanged")]
    public partial class IDCombobox : UserControl
    {
        // Fields
        private Color _backColor = Color.FromArgb(31,31,78);
        private Color _iconColor = Color.FromArgb(112,81,157);
        private Color _listBackColor = Color.FromArgb(230, 228, 245);
        private Color _listTextColor = Color.DimGray; 
        private Color _borderColor  = Color.FromArgb(112, 31, 157);
        private int _borderSize = 1;

        // Items
        private ComboBox cmbList;
        private Label lblText;
        private Button btnIcon;



        // Events
        public event EventHandler OnSelectedIndexChanged;   // Default event



        public IDCombobox()
        {
            cmbList = new ComboBox();
            lblText = new Label();
            btnIcon = new Button();
            SuspendLayout();

            // ComboBox: Dropdown list
            cmbList.BackColor = _backColor;
            cmbList.Font = new Font(Font.Name, 10F);
            cmbList.ForeColor = _listTextColor;
            cmbList.SelectedIndexChanged += ComboBox_SelectedIndexChanged;   // Default event
            cmbList.TextChanged += ComboBox_TextChanged;    //refresh text

            // Button: Icon
            btnIcon.Dock = DockStyle.Right;
            btnIcon.FlatStyle = FlatStyle.Flat;
            btnIcon.FlatAppearance.BorderSize = 0;
            btnIcon.BackColor = _backColor;
            btnIcon.Size = new Size(30,30);
            btnIcon.Cursor = Cursors.Hand;
            btnIcon.Click += Icon_Click;    // open dorpdown list
            btnIcon.Paint += new PaintEventHandler( Icon_Paint);

            // Label: Text
            lblText.Dock = DockStyle.Fill;
            lblText.AutoSize = false;
            lblText.BackColor = _backColor;
            lblText.TextAlign = ContentAlignment.MiddleLeft;
            lblText.Padding = new Padding(8, 0, 0, 0);
            lblText.Font = new Font(Font.Name, 10F);
            lblText.Click += Surface_Click; // select combobox

            // User Control
            Controls.Add(lblText);//2
            Controls.Add(btnIcon);//1
            Controls.Add(cmbList);//0
            MinimumSize = new Size(200, 30);
            Size = new Size(200, 30);
            ForeColor = Color.WhiteSmoke;
            Padding = new Padding(_borderSize);
            base.BackColor = _borderColor;
            ResumeLayout(true);
            AdjustComboBoxDimensions();
            InitializeComponent();
        }

        private void AdjustComboBoxDimensions()
        {
            cmbList.Width = lblText.Width;
            cmbList.Location = new Point()
            {
                X = this.Width - this.Padding.Right - cmbList.Width,
                Y = lblText.Bottom - cmbList.Height
            };
        }

        // Event methods
        // -> Default event
        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (OnSelectedIndexChanged != null)
                OnSelectedIndexChanged.Invoke(sender, e);
        }

        private void Surface_Click(object sender, EventArgs e)
        {
            cmbList.Select();
            if (cmbList.DropDownStyle == ComboBoxStyle.DropDownList)
            {
                cmbList.DroppedDown = true; // open dropdown list
            }
        }

        private void Icon_Paint(object sender, PaintEventArgs e)
        {
            // fields
            int iconWidth = 14;
            int iconHeight = 8;
            var rectIcon = new Rectangle((btnIcon.Width - iconWidth)/2, (btnIcon.Height -iconHeight)/2, iconWidth, iconHeight);
            Graphics graph = e.Graphics;

            // Draw icon shape
            using (GraphicsPath path = new GraphicsPath())
            using (Pen pen = new Pen(_iconColor, 2))
            {
                graph.SmoothingMode = SmoothingMode.AntiAlias;
                path.AddLine(rectIcon.X, rectIcon.Y, rectIcon.X + (iconWidth / 2), rectIcon.Bottom);
                path.AddLine(rectIcon.X + (iconWidth / 2), rectIcon.Bottom, rectIcon.Right, rectIcon.Y);
                graph.DrawPath(pen, path);
                path.CloseFigure();
            }
        }

        private void Icon_Click(object sender, EventArgs e)
        {
            cmbList.Select();
            cmbList.DroppedDown = true;
        }


        private void ComboBox_TextChanged(object sender, EventArgs e)
        {
            // refresh text
            lblText.Text = cmbList.Text;
        }
    }
}
