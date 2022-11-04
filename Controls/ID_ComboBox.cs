using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MissionPlanner.Controls
{
    [DefaultEvent("OnSelectedIndexChanged")]
    [DefaultProperty("Items")]
    public class ID_ComboBox : MyUserControl
    {
        // Fields
        private Color _backColor = Color.FromArgb(31, 31, 78);
        
        private Color _iconColor = Color.FromArgb( 23, 53, 78);//Color.MediumSlateBlue;
        private Color _listBackColor = Color.FromArgb(22,20,68);
        private Color _listTextColor = Color.DimGray;
        private Color _borderColor = Color.FromArgb(71, 119, 134);//Color.MediumSlateBlue;
        private int _borderSize = 1;

        // Items
        private readonly ComboBox cmbList;
        private readonly Label lblText;
        private readonly Button btnIcon;

        // Properties
        public new Color BackColor
        {
            get
            {
                return _backColor;
            }

            set
            {
                _backColor = value;
                lblText.BackColor = _backColor;
                btnIcon.BackColor = _backColor;
            }
        }
        public Color IconColor
        {
            get => _iconColor;
            set
            {
                _iconColor = value;
                btnIcon.Invalidate(); 
            }
        }
        public Color ListBackColor
        {
            get => _listBackColor;
            set
            {
                _listBackColor = value;
                cmbList.BackColor = _listBackColor;
            }
        }
        public Color ListTextColor
        {
            get => _listTextColor;
            set
            {
                _listTextColor = value;
                cmbList.ForeColor = _listTextColor;
            }
        }
        public Color BorderColor
        {
            get => _borderColor;
            set
            {
                _borderColor = value;
                base.BackColor = _borderColor;
            }
        }
        public int BorderSize
        {
            get => _borderSize;
            set
            {
                _borderSize = value;
                this.Padding = new Padding(_borderSize);
                AdjustComboBoxDimensions();
            }
        }
        public override Color ForeColor
        {
            get => base.ForeColor;
            set
            {
                base.ForeColor = value;
                lblText.ForeColor = value;
                cmbList.ForeColor = value;
            }
        }
        public override Font Font
        {
            get => base.Font;
            set
            {
                base.Font = value;
                lblText.Font = value;
                cmbList.Font = value;
            }
        }
        public override string Text
        {
            get => lblText.Text;
            set => lblText.Text = value;
        }
        public ComboBox.ObjectCollection Items => cmbList.Items;
        public object SelectedItem => cmbList.SelectedItem;
        
        public int SelectedIndex
        {
            get => cmbList.SelectedIndex;
            set => cmbList.SelectedIndex = value;
        }
        public ComboBoxStyle DropdownStyle
        {
            get => cmbList.DropDownStyle;
            set
            {
                if(cmbList.DropDownStyle !=  ComboBoxStyle.Simple)
                    cmbList.DropDownStyle = value;
            }
        }

        public int DropDownWidth { get; set; }
        public object DataSource { get; set; }

        public void Clear()
        {
            this.Items.Clear(); // Clear items of combo box
        }

        public int FindString(string s)
        {
            return cmbList.FindString(s);
        }

      
        // Constructor
        public ID_ComboBox()
        {
            cmbList = new ComboBox();
            lblText = new Label();
            btnIcon = new Button();
            //SuspendLayout();
            InitializeComponent();

            // ComboBox: Dropdown List
            cmbList.BackColor = ListBackColor;
            cmbList.Font = new Font(Font.Name, 10f);
            cmbList.ForeColor = ListTextColor;
            cmbList.SelectedIndexChanged += ComboBox_SelectedIndexChanged; // Default event
            cmbList.TextChanged += ComboBox_TextChanged; // Refresh text

            // Button: Icon
            btnIcon.Dock = DockStyle.Right;
            btnIcon.FlatStyle = FlatStyle.Flat;
            btnIcon.FlatAppearance.BorderSize = 0;
            btnIcon.BackColor = BackColor;
            btnIcon.Size = new Size(30, 30);
            btnIcon.Cursor = Cursors.Hand;
            btnIcon.Click += Icon_Click; // Open dropdown list
            btnIcon.Paint += Icon_Paint;

            // Label: Text
            lblText.Dock = DockStyle.Fill;
            lblText.AutoSize = false;
            lblText.BackColor = BackColor;
            lblText.TextAlign = ContentAlignment.MiddleLeft;
            lblText.Padding = new Padding(8, 0, 8, 0);
            lblText.Click += Surface_Click; // Select ComboBox

            // User Control
            Controls.Add(lblText); // 2
            Controls.Add(btnIcon); // 1
            Controls.Add(cmbList); // 0
            //MinimumSize = new Size(200, 30);
            //Size = new Size(200, 30);
            ForeColor = Color.DimGray;
            Padding = new Padding(BorderSize);
            base.BackColor = BorderColor;

            ResumeLayout();
            AdjustComboBoxDimensions();
            
            
        }
        

        // Events

        // private methods
        private void AdjustComboBoxDimensions()
        {
            
            cmbList.Width = lblText.Width;
            cmbList.Location = new Point()
            {
                X = this.Width - this.Padding.Right - this.cmbList.Width,
                Y = this.lblText.Bottom - this.cmbList.Height
            };
        }

        // -> Default Event
        public void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (OnSelectedIndexChanged != null) OnSelectedIndexChanged.Invoke(sender, e);
            lblText.Text = cmbList.Text;
        }

        //
        private void Surface_Click(object sender, EventArgs e)
        {
            // Select combobox
            cmbList.Select();
            if (cmbList.DropDownStyle == ComboBoxStyle.DropDownList) cmbList.DroppedDown = true; // Open dropdown list
            AdjustComboBoxDimensions();
        }

        private void Icon_Paint(object sender, PaintEventArgs e)
        {
            // Fileds
            var iconWidth = 14;
            var iconHeight = 6;
            var rectIcon = new Rectangle((btnIcon.Width - iconWidth) / 2, (btnIcon.Height - iconHeight) / 2, iconWidth,
                iconHeight);

            var graph = e.Graphics;
            // Draw arrow down icon
            using (var path = new GraphicsPath())
            using (var pen = new Pen(IconColor, 2))
            {
                graph.SmoothingMode = SmoothingMode.AntiAlias;
                path.AddLine(rectIcon.X, rectIcon.Y, rectIcon.X + iconWidth / 2, rectIcon.Bottom);
                path.AddLine(rectIcon.X + iconWidth / 2, rectIcon.Bottom, rectIcon.Right, rectIcon.Y);
                graph.DrawPath(pen, path);
            }
        }

        private void Icon_Click(object sender, EventArgs e)
        {
            cmbList.Select();
            cmbList.DroppedDown = true; // Open dropdown list
            AdjustComboBoxDimensions();
        }

        private void ComboBox_TextChanged(object sender, EventArgs e)
        {
            // Refresh text
            lblText.Text = cmbList.Text;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ID_ComboBox
            // 
            this.Name = "ID_ComboBox";
            this.Size = new System.Drawing.Size(255, 150);
            this.ResumeLayout(false);

        }

        public event EventHandler<EventArgs> SelectedIndexChanged;

        protected virtual void OnSelectedIndexChanged()
        {
            SelectedIndexChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
