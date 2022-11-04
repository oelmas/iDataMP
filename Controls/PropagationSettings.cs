using System;
using System.ComponentModel;
using System.Windows.Forms;
using MissionPlanner.Maps;
using MissionPlanner.Utilities;

namespace MissionPlanner.Controls
{
    public class PropagationSettings : Form
    {
        private CheckBox chk_dronedist;
        private CheckBox chk_ele;
        private CheckBox chk_homedist;
        private CheckBox chk_rf;
        private CheckBox chk_terrain;
        private NumericUpDown Clearance;
        private ComboBox CMB_Angular;
        private ComboBox CMB_Resolution;
        private ComboBox CMB_Rotational;
        private GroupBox groupBox1;
        private Label label100;
        private Label label109;
        private Label label110;
        private Label label111;
        private Label label112;
        private Label label113;
        private Label label114;
        private Label label89;
        private Label label90;
        private Label label91;
        private NumericUpDown NUM_height;
        private NumericUpDown NUM_range;
        private NumericUpDown Tolerance;

        public PropagationSettings()
        {
            InitializeComponent();

            ThemeManager.ApplyThemeTo(this);

            Clearance.Value = (decimal)Settings.Instance.GetFloat("Propagation_Clearance", 5);
            CMB_Resolution.Text = Settings.Instance.GetInt32("Propagation_Resolution", 4).ToString();
            CMB_Rotational.Text = Settings.Instance.GetInt32("Propagation_Rotational", 1).ToString();
            CMB_Angular.Text = Settings.Instance.GetInt32("Propagation_Converge", 1).ToString();
            NUM_range.Value = (decimal)Settings.Instance.GetFloat("Propagation_Range", 2.0f);
            NUM_height.Value = (decimal)Settings.Instance.GetFloat("Propagation_Height", 2.0f);
            Tolerance.Value = (decimal)Settings.Instance.GetFloat("Propagation_Tolerance", 0.8f);

            chk_ele.Checked = Propagation.ele_run;
            chk_terrain.Checked = Propagation.ter_run;
            chk_rf.Checked = Propagation.rf_run;
            chk_homedist.Checked = Propagation.home_kmleft;
            chk_dronedist.Checked = Propagation.drone_kmleft;
        }

        private void InitializeComponent()
        {
            var resources = new ComponentResourceManager(typeof(PropagationSettings));
            chk_ele = new CheckBox();
            chk_terrain = new CheckBox();
            chk_rf = new CheckBox();
            chk_dronedist = new CheckBox();
            chk_homedist = new CheckBox();
            groupBox1 = new GroupBox();
            label91 = new Label();
            label89 = new Label();
            Clearance = new NumericUpDown();
            label100 = new Label();
            label113 = new Label();
            label109 = new Label();
            CMB_Rotational = new ComboBox();
            label90 = new Label();
            CMB_Resolution = new ComboBox();
            label110 = new Label();
            CMB_Angular = new ComboBox();
            label111 = new Label();
            NUM_range = new NumericUpDown();
            label112 = new Label();
            NUM_height = new NumericUpDown();
            label114 = new Label();
            Tolerance = new NumericUpDown();
            groupBox1.SuspendLayout();
            ((ISupportInitialize)Clearance).BeginInit();
            ((ISupportInitialize)NUM_range).BeginInit();
            ((ISupportInitialize)NUM_height).BeginInit();
            ((ISupportInitialize)Tolerance).BeginInit();
            SuspendLayout();
            // 
            // chk_ele
            // 
            resources.ApplyResources(chk_ele, "chk_ele");
            chk_ele.Checked = true;
            chk_ele.CheckState = CheckState.Indeterminate;
            chk_ele.Name = "chk_ele";
            chk_ele.UseVisualStyleBackColor = true;
            chk_ele.CheckedChanged += chk_ele_CheckedChanged;
            // 
            // chk_terrain
            // 
            resources.ApplyResources(chk_terrain, "chk_terrain");
            chk_terrain.Checked = true;
            chk_terrain.CheckState = CheckState.Indeterminate;
            chk_terrain.Name = "chk_terrain";
            chk_terrain.UseVisualStyleBackColor = true;
            chk_terrain.CheckedChanged += chk_terrain_CheckedChanged;
            // 
            // chk_rf
            // 
            resources.ApplyResources(chk_rf, "chk_rf");
            chk_rf.Checked = true;
            chk_rf.CheckState = CheckState.Indeterminate;
            chk_rf.Name = "chk_rf";
            chk_rf.UseVisualStyleBackColor = true;
            chk_rf.CheckedChanged += chk_rf_CheckedChanged;
            // 
            // chk_dronedist
            // 
            resources.ApplyResources(chk_dronedist, "chk_dronedist");
            chk_dronedist.Checked = true;
            chk_dronedist.CheckState = CheckState.Indeterminate;
            chk_dronedist.Name = "chk_dronedist";
            chk_dronedist.UseVisualStyleBackColor = true;
            chk_dronedist.CheckedChanged += chk_dronedist_CheckedChanged;
            // 
            // chk_homedist
            // 
            resources.ApplyResources(chk_homedist, "chk_homedist");
            chk_homedist.Checked = true;
            chk_homedist.CheckState = CheckState.Indeterminate;
            chk_homedist.Name = "chk_homedist";
            chk_homedist.UseVisualStyleBackColor = true;
            chk_homedist.CheckedChanged += chk_homedist_CheckedChanged;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(label91);
            groupBox1.Controls.Add(label89);
            groupBox1.Controls.Add(Clearance);
            groupBox1.Controls.Add(label100);
            groupBox1.Controls.Add(label113);
            groupBox1.Controls.Add(label109);
            groupBox1.Controls.Add(CMB_Rotational);
            groupBox1.Controls.Add(label90);
            groupBox1.Controls.Add(CMB_Resolution);
            groupBox1.Controls.Add(label110);
            groupBox1.Controls.Add(CMB_Angular);
            groupBox1.Controls.Add(label111);
            groupBox1.Controls.Add(NUM_range);
            groupBox1.Controls.Add(label112);
            groupBox1.Controls.Add(NUM_height);
            groupBox1.Controls.Add(label114);
            groupBox1.Controls.Add(Tolerance);
            resources.ApplyResources(groupBox1, "groupBox1");
            groupBox1.Name = "groupBox1";
            groupBox1.TabStop = false;
            // 
            // label91
            // 
            resources.ApplyResources(label91, "label91");
            label91.Name = "label91";
            // 
            // label89
            // 
            resources.ApplyResources(label89, "label89");
            label89.Name = "label89";
            // 
            // Clearance
            // 
            Clearance.DecimalPlaces = 1;
            Clearance.Increment = new decimal(new[]
            {
                1,
                0,
                0,
                65536
            });
            resources.ApplyResources(Clearance, "Clearance");
            Clearance.Maximum = new decimal(new[]
            {
                2000,
                0,
                0,
                0
            });
            Clearance.Name = "Clearance";
            Clearance.ValueChanged += Clearance_ValueChanged;
            // 
            // label100
            // 
            resources.ApplyResources(label100, "label100");
            label100.Name = "label100";
            // 
            // label113
            // 
            resources.ApplyResources(label113, "label113");
            label113.Name = "label113";
            // 
            // label109
            // 
            resources.ApplyResources(label109, "label109");
            label109.Name = "label109";
            // 
            // CMB_Rotational
            // 
            CMB_Rotational.DropDownStyle = ComboBoxStyle.DropDownList;
            CMB_Rotational.FormattingEnabled = true;
            CMB_Rotational.Items.AddRange(new object[]
            {
                resources.GetString("CMB_Rotational.Items"),
                resources.GetString("CMB_Rotational.Items1"),
                resources.GetString("CMB_Rotational.Items2"),
                resources.GetString("CMB_Rotational.Items3"),
                resources.GetString("CMB_Rotational.Items4")
            });
            resources.ApplyResources(CMB_Rotational, "CMB_Rotational");
            CMB_Rotational.Name = "CMB_Rotational";
            CMB_Rotational.SelectedIndexChanged += CMB_Rotational_SelectedIndexChanged;
            // 
            // label90
            // 
            resources.ApplyResources(label90, "label90");
            label90.Name = "label90";
            // 
            // CMB_Resolution
            // 
            CMB_Resolution.DropDownStyle = ComboBoxStyle.DropDownList;
            CMB_Resolution.FormattingEnabled = true;
            CMB_Resolution.Items.AddRange(new object[]
            {
                resources.GetString("CMB_Resolution.Items"),
                resources.GetString("CMB_Resolution.Items1"),
                resources.GetString("CMB_Resolution.Items2"),
                resources.GetString("CMB_Resolution.Items3"),
                resources.GetString("CMB_Resolution.Items4")
            });
            resources.ApplyResources(CMB_Resolution, "CMB_Resolution");
            CMB_Resolution.Name = "CMB_Resolution";
            CMB_Resolution.SelectedIndexChanged += CMB_Resolution_SelectedIndexChanged;
            // 
            // label110
            // 
            resources.ApplyResources(label110, "label110");
            label110.Name = "label110";
            // 
            // CMB_Angular
            // 
            CMB_Angular.DropDownStyle = ComboBoxStyle.DropDownList;
            CMB_Angular.FormattingEnabled = true;
            CMB_Angular.Items.AddRange(new object[]
            {
                resources.GetString("CMB_Angular.Items"),
                resources.GetString("CMB_Angular.Items1"),
                resources.GetString("CMB_Angular.Items2"),
                resources.GetString("CMB_Angular.Items3"),
                resources.GetString("CMB_Angular.Items4")
            });
            resources.ApplyResources(CMB_Angular, "CMB_Angular");
            CMB_Angular.Name = "CMB_Angular";
            CMB_Angular.SelectedIndexChanged += CMB_Angular_SelectedIndexChanged;
            // 
            // label111
            // 
            resources.ApplyResources(label111, "label111");
            label111.Name = "label111";
            // 
            // NUM_range
            // 
            NUM_range.DecimalPlaces = 1;
            NUM_range.Increment = new decimal(new[]
            {
                1,
                0,
                0,
                65536
            });
            resources.ApplyResources(NUM_range, "NUM_range");
            NUM_range.Maximum = new decimal(new[]
            {
                2000,
                0,
                0,
                0
            });
            NUM_range.Name = "NUM_range";
            NUM_range.ValueChanged += NUM_range_ValueChanged;
            // 
            // label112
            // 
            resources.ApplyResources(label112, "label112");
            label112.Name = "label112";
            // 
            // NUM_height
            // 
            NUM_height.DecimalPlaces = 1;
            NUM_height.Increment = new decimal(new[]
            {
                1,
                0,
                0,
                65536
            });
            resources.ApplyResources(NUM_height, "NUM_height");
            NUM_height.Maximum = new decimal(new[]
            {
                2000,
                0,
                0,
                0
            });
            NUM_height.Name = "NUM_height";
            NUM_height.ValueChanged += NUM_height_ValueChanged;
            // 
            // label114
            // 
            resources.ApplyResources(label114, "label114");
            label114.Name = "label114";
            // 
            // Tolerance
            // 
            Tolerance.DecimalPlaces = 1;
            Tolerance.Increment = new decimal(new[]
            {
                1,
                0,
                0,
                65536
            });
            resources.ApplyResources(Tolerance, "Tolerance");
            Tolerance.Maximum = new decimal(new[]
            {
                1,
                0,
                0,
                0
            });
            Tolerance.Name = "Tolerance";
            Tolerance.ValueChanged += Tolerance_ValueChanged;
            // 
            // PropagationSettings
            // 
            resources.ApplyResources(this, "$this");
            Controls.Add(groupBox1);
            Controls.Add(chk_homedist);
            Controls.Add(chk_dronedist);
            Controls.Add(chk_rf);
            Controls.Add(chk_terrain);
            Controls.Add(chk_ele);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "PropagationSettings";
            groupBox1.ResumeLayout(false);
            ((ISupportInitialize)Clearance).EndInit();
            ((ISupportInitialize)NUM_range).EndInit();
            ((ISupportInitialize)NUM_height).EndInit();
            ((ISupportInitialize)Tolerance).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private void chk_ele_CheckedChanged(object sender, EventArgs e)
        {
            Propagation.ele_run = chk_ele.Checked;
        }

        private void chk_terrain_CheckedChanged(object sender, EventArgs e)
        {
            Propagation.ter_run = chk_terrain.Checked;
        }

        private void chk_rf_CheckedChanged(object sender, EventArgs e)
        {
            Propagation.rf_run = chk_rf.Checked;
        }

        private void chk_homedist_CheckedChanged(object sender, EventArgs e)
        {
            Propagation.home_kmleft = chk_homedist.Checked;
        }

        private void chk_dronedist_CheckedChanged(object sender, EventArgs e)
        {
            Propagation.drone_kmleft = chk_dronedist.Checked;
        }

        private void Clearance_ValueChanged(object sender, EventArgs e)
        {
            Settings.Instance["Propagation_Clearance"] = Clearance.Value.ToString();
        }

        private void CMB_Resolution_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings.Instance["Propagation_Resolution"] = CMB_Resolution.Text;
        }

        private void CMB_Rotational_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings.Instance["Propagation_Rotational"] = CMB_Rotational.Text;
        }

        private void CMB_Angular_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings.Instance["Propagation_Converge"] = CMB_Angular.Text;
        }

        private void NUM_range_ValueChanged(object sender, EventArgs e)
        {
            Settings.Instance["Propagation_Range"] = NUM_range.Value.ToString();
        }

        private void NUM_height_ValueChanged(object sender, EventArgs e)
        {
            Settings.Instance["Propagation_Height"] = NUM_height.Value.ToString();
        }

        private void Tolerance_ValueChanged(object sender, EventArgs e)
        {
            Settings.Instance["Propagation_Tolerance"] = Tolerance.Value.ToString();
        }
    }
}