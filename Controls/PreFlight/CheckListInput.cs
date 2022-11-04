using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MissionPlanner.Controls.PreFlight
{
    public class CheckListInput : UserControl
    {
        private readonly CheckListControl _parent;
        private MyButton but_addchild;
        private MyButton but_remove;
        public ComboBox CMB_colour1;
        public ComboBox CMB_colour2;

        private ComboBox CMB_condition;
        private ComboBox CMB_Source;
        private NumericUpDown NUM_trigger;
        public TextBox TXT_desc;
        public TextBox TXT_text;

        public CheckListInput(CheckListControl parent)
        {
            _parent = parent;

            InitializeComponent();
        }

        public CheckListInput(CheckListControl parent, CheckListItem item)
        {
            _parent = parent;

            InitializeComponent();

            CheckListItem.defaultsrc = MainV2.comPort.MAV.cs;
            item.SetField(item.Name);

            CMB_condition.DataSource = Enum.GetNames(typeof(CheckListItem.Conditional));

            CMB_Source.DataSource = item.GetOptions();

            CMB_colour1.DataSource = Enum.GetNames(typeof(KnownColor));
            CMB_colour2.DataSource = Enum.GetNames(typeof(KnownColor));

            CheckListItem = item;

            updateDisplay();
        }

        public CheckListItem CheckListItem { get; set; }

        public event EventHandler ReloadList;

        public void updateDisplay()
        {
            CMB_condition.Text = CheckListItem.ConditionType.ToString();
            CMB_Source.Text = CheckListItem.Name;
            NUM_trigger.Value = (decimal)CheckListItem.TriggerValue;
            TXT_text.Text = CheckListItem.Text;
            TXT_desc.Text = CheckListItem.Description;
            CMB_colour1.SelectedItem = CheckListItem.TrueColor;
            CMB_colour2.SelectedItem = CheckListItem.FalseColor;
        }

        private void InitializeComponent()
        {
            CMB_Source = new ComboBox();
            CMB_condition = new ComboBox();
            NUM_trigger = new NumericUpDown();
            TXT_text = new TextBox();
            but_addchild = new MyButton();
            but_remove = new MyButton();
            CMB_colour1 = new ComboBox();
            CMB_colour2 = new ComboBox();
            TXT_desc = new TextBox();
            ((ISupportInitialize)NUM_trigger).BeginInit();
            SuspendLayout();
            // 
            // CMB_Source
            // 
            CMB_Source.DropDownStyle = ComboBoxStyle.DropDownList;
            CMB_Source.FormattingEnabled = true;
            CMB_Source.Location = new Point(3, 2);
            CMB_Source.Name = "CMB_Source";
            CMB_Source.Size = new Size(121, 21);
            CMB_Source.TabIndex = 0;
            CMB_Source.SelectedIndexChanged += CMB_Source_SelectedIndexChanged;
            // 
            // CMB_condition
            // 
            CMB_condition.DropDownStyle = ComboBoxStyle.DropDownList;
            CMB_condition.FormattingEnabled = true;
            CMB_condition.Location = new Point(130, 2);
            CMB_condition.Name = "CMB_condition";
            CMB_condition.Size = new Size(54, 21);
            CMB_condition.TabIndex = 1;
            CMB_condition.SelectedIndexChanged += CMB_condition_SelectedIndexChanged;
            // 
            // NUM_trigger
            // 
            NUM_trigger.DecimalPlaces = 2;
            NUM_trigger.Location = new Point(190, 3);
            NUM_trigger.Maximum = new decimal(new[]
            {
                99999,
                0,
                0,
                0
            });
            NUM_trigger.Minimum = new decimal(new[]
            {
                99999,
                0,
                0,
                -2147483648
            });
            NUM_trigger.Name = "NUM_trigger";
            NUM_trigger.Size = new Size(65, 20);
            NUM_trigger.TabIndex = 2;
            NUM_trigger.ValueChanged += NUM_warning_ValueChanged;
            // 
            // TXT_text
            // 
            TXT_text.Location = new Point(261, 29);
            TXT_text.Name = "TXT_text";
            TXT_text.Size = new Size(236, 20);
            TXT_text.TabIndex = 4;
            TXT_text.Text = "{name} is {value}";
            TXT_text.TextChanged += TXT_warningtext_TextChanged;
            // 
            // but_addchild
            // 
            but_addchild.Location = new Point(631, 1);
            but_addchild.Name = "but_addchild";
            but_addchild.Size = new Size(25, 20);
            but_addchild.TabIndex = 7;
            but_addchild.Text = "+";
            but_addchild.UseVisualStyleBackColor = true;
            but_addchild.Click += but_addchild_Click;
            // 
            // but_remove
            // 
            but_remove.Location = new Point(662, 1);
            but_remove.Name = "but_remove";
            but_remove.Size = new Size(25, 20);
            but_remove.TabIndex = 8;
            but_remove.Text = "-";
            but_remove.UseVisualStyleBackColor = true;
            but_remove.Click += but_remove_Click;
            // 
            // CMB_colour1
            // 
            CMB_colour1.DropDownStyle = ComboBoxStyle.DropDownList;
            CMB_colour1.DropDownWidth = 100;
            CMB_colour1.FormattingEnabled = true;
            CMB_colour1.Location = new Point(503, 1);
            CMB_colour1.Name = "CMB_colour1";
            CMB_colour1.Size = new Size(50, 21);
            CMB_colour1.TabIndex = 5;
            CMB_colour1.SelectedIndexChanged += CMB_colour1_SelectedIndexChanged;
            // 
            // CMB_colour2
            // 
            CMB_colour2.DropDownStyle = ComboBoxStyle.DropDownList;
            CMB_colour2.DropDownWidth = 100;
            CMB_colour2.FormattingEnabled = true;
            CMB_colour2.Location = new Point(559, 1);
            CMB_colour2.Name = "CMB_colour2";
            CMB_colour2.Size = new Size(50, 21);
            CMB_colour2.TabIndex = 6;
            CMB_colour2.SelectedIndexChanged += CMB_colour2_SelectedIndexChanged;
            // 
            // TXT_desc
            // 
            TXT_desc.Location = new Point(261, 3);
            TXT_desc.Name = "TXT_desc";
            TXT_desc.Size = new Size(236, 20);
            TXT_desc.TabIndex = 3;
            TXT_desc.Text = "GPS Hdop";
            TXT_desc.TextChanged += TXT_desc_TextChanged;
            // 
            // CheckListInput
            // 
            Controls.Add(TXT_desc);
            Controls.Add(CMB_colour2);
            Controls.Add(CMB_colour1);
            Controls.Add(but_remove);
            Controls.Add(but_addchild);
            Controls.Add(TXT_text);
            Controls.Add(NUM_trigger);
            Controls.Add(CMB_condition);
            Controls.Add(CMB_Source);
            Name = "CheckListInput";
            Size = new Size(695, 51);
            ((ISupportInitialize)NUM_trigger).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private void CMB_Source_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CheckListItem != null)
                CheckListItem.SetField(CMB_Source.Text);
        }

        private void CMB_condition_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CheckListItem != null)
                CheckListItem.ConditionType =
                    (CheckListItem.Conditional)Enum.Parse(typeof(CheckListItem.Conditional), CMB_condition.Text);
        }

        private void NUM_warning_ValueChanged(object sender, EventArgs e)
        {
            if (CheckListItem != null)
                CheckListItem.TriggerValue = (double)NUM_trigger.Value;
        }

        private void TXT_warningtext_TextChanged(object sender, EventArgs e)
        {
            if (CheckListItem != null)
                CheckListItem.Text = TXT_text.Text;
        }

        private void but_addchild_Click(object sender, EventArgs e)
        {
            CheckListItem.Child = new CheckListItem();

            if (ReloadList != null)
                ReloadList(this, null);
        }

        private void but_remove_Click(object sender, EventArgs e)
        {
            lock (_parent.CheckListItems)
            {
                _parent.CheckListItems.Remove(CheckListItem);

                foreach (var item in _parent.CheckListItems) removewarning(item, CheckListItem);
            }

            if (ReloadList != null)
                ReloadList(this, null);
        }

        private void removewarning(CheckListItem lookin, CheckListItem removeme)
        {
            // depth first check children
            if (lookin.Child != null)
                removewarning(lookin.Child, removeme);

            if (lookin.Child == removeme)
            {
                if (lookin.Child.Child != null)
                    lookin.Child = lookin.Child.Child;
                else
                    lookin.Child = null;
            }
        }

        private void TXT_desc_TextChanged(object sender, EventArgs e)
        {
            if (CheckListItem != null)
                CheckListItem.Description = TXT_desc.Text;
        }

        private void CMB_colour1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CheckListItem != null)
                CheckListItem.TrueColor = CMB_colour1.SelectedValue.ToString();
        }

        private void CMB_colour2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CheckListItem != null)
                CheckListItem.FalseColor = CMB_colour2.SelectedValue.ToString();
        }
    }
}