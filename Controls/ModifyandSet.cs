using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace MissionPlanner.Controls
{
    public partial class ModifyandSet : UserControl
    {
        public ModifyandSet()
        {
            InitializeComponent();
        }

        [Browsable(false)] public NumericUpDown NumericUpDown => numericUpDown1;

        [Browsable(false)] public MyButton Button => myButton1;

        [Browsable(true)]
        public string ButtonText
        {
            get => Button.Text;
            set => Button.Text = value;
        }

        [Browsable(true)]
        public decimal Increment
        {
            get => NumericUpDown.Increment;
            set => NumericUpDown.Increment = value;
        }

        [Browsable(true)]
        public int DecimalPlaces
        {
            get => NumericUpDown.DecimalPlaces;
            set => NumericUpDown.DecimalPlaces = value;
        }

        [Browsable(true)]
        public decimal Value
        {
            get => NumericUpDown.Value;
            set => NumericUpDown.Value = value;
        }

        [Browsable(true)]
        public decimal Minimum
        {
            get => NumericUpDown.Minimum;
            set => NumericUpDown.Minimum = value;
        }

        [Browsable(true)]
        public decimal Maximum
        {
            get => NumericUpDown.Maximum;
            set => NumericUpDown.Maximum = value;
        }

        public new event EventHandler Click;
        public event EventHandler ValueChanged;

        private void myButton1_Click(object sender, EventArgs e)
        {
            if (Click != null)
                Click(sender, e);
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (ValueChanged != null)
                ValueChanged(sender, e);
        }
    }
}