using System;

namespace MissionPlanner.Controls
{
    partial class ConnectionControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConnectionControl));
            this.panel4 = new System.Windows.Forms.Panel();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.cmb_sysid = new MissionPlanner.Controls.ID_ComboBox();
            this.lblSysId = new System.Windows.Forms.Label();
            this.panel6 = new System.Windows.Forms.Panel();
            this.cmb_Connection = new MissionPlanner.Controls.ID_ComboBox();
            this.lConnection = new System.Windows.Forms.Label();
            this.pBaud = new System.Windows.Forms.Panel();
            this.lblBoud = new System.Windows.Forms.Label();
            this.cmb_Baud = new MissionPlanner.Controls.ID_ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lblConnection = new System.Windows.Forms.Label();
            this.labelBaud = new System.Windows.Forms.Label();
            this.panel4.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel6.SuspendLayout();
            this.pBaud.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(53)))), ((int)(((byte)(78)))));
            this.panel4.Controls.Add(this.linkLabel1);
            resources.ApplyResources(this.panel4, "panel4");
            this.panel4.Name = "panel4";
            // 
            // linkLabel1
            // 
            resources.ApplyResources(this.linkLabel1, "linkLabel1");
            this.linkLabel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(53)))), ((int)(((byte)(78)))));
            this.linkLabel1.Image = global::MissionPlanner.Properties.Resources.bgdark;
            this.linkLabel1.LinkColor = System.Drawing.Color.White;
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.TabStop = true;
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(53)))), ((int)(((byte)(78)))));
            this.panel5.Controls.Add(this.panel7);
            this.panel5.Controls.Add(this.panel6);
            this.panel5.Controls.Add(this.pBaud);
            this.panel5.Controls.Add(this.panel4);
            resources.ApplyResources(this.panel5, "panel5");
            this.panel5.Name = "panel5";
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.cmb_sysid);
            this.panel7.Controls.Add(this.lblSysId);
            resources.ApplyResources(this.panel7, "panel7");
            this.panel7.Name = "panel7";
            // 
            // cmb_sysid
            // 
            this.cmb_sysid.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(78)))));
            this.cmb_sysid.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(119)))), ((int)(((byte)(134)))));
            this.cmb_sysid.BorderSize = 1;
            this.cmb_sysid.DataSource = null;
            resources.ApplyResources(this.cmb_sysid, "cmb_sysid");
            this.cmb_sysid.DropdownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            this.cmb_sysid.DropDownWidth = 0;
            this.cmb_sysid.ForeColor = System.Drawing.Color.DimGray;
            this.cmb_sysid.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(119)))), ((int)(((byte)(134)))));
            this.cmb_sysid.ListBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(20)))), ((int)(((byte)(68)))));
            this.cmb_sysid.ListTextColor = System.Drawing.Color.DimGray;
            this.cmb_sysid.Name = "cmb_sysid";
            this.cmb_sysid.SelectedIndex = -1;
            // 
            // lblSysId
            // 
            resources.ApplyResources(this.lblSysId, "lblSysId");
            this.lblSysId.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(198)))), ((int)(((byte)(0)))));
            this.lblSysId.Name = "lblSysId";
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.cmb_Connection);
            this.panel6.Controls.Add(this.lConnection);
            resources.ApplyResources(this.panel6, "panel6");
            this.panel6.Name = "panel6";
            // 
            // cmb_Connection
            // 
            this.cmb_Connection.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(78)))));
            this.cmb_Connection.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(119)))), ((int)(((byte)(134)))));
            this.cmb_Connection.BorderSize = 1;
            this.cmb_Connection.DataSource = null;
            resources.ApplyResources(this.cmb_Connection, "cmb_Connection");
            this.cmb_Connection.DropdownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            this.cmb_Connection.DropDownWidth = 0;
            this.cmb_Connection.ForeColor = System.Drawing.Color.DimGray;
            this.cmb_Connection.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(119)))), ((int)(((byte)(134)))));
            this.cmb_Connection.ListBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(20)))), ((int)(((byte)(68)))));
            this.cmb_Connection.ListTextColor = System.Drawing.Color.DimGray;
            this.cmb_Connection.Name = "cmb_Connection";
            this.cmb_Connection.SelectedIndex = -1;
            // 
            // lConnection
            // 
            resources.ApplyResources(this.lConnection, "lConnection");
            this.lConnection.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(198)))), ((int)(((byte)(0)))));
            this.lConnection.Name = "lConnection";
            // 
            // pBaud
            // 
            this.pBaud.Controls.Add(this.lblBoud);
            this.pBaud.Controls.Add(this.cmb_Baud);
            resources.ApplyResources(this.pBaud, "pBaud");
            this.pBaud.Name = "pBaud";
            // 
            // lblBoud
            // 
            resources.ApplyResources(this.lblBoud, "lblBoud");
            this.lblBoud.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(198)))), ((int)(((byte)(0)))));
            this.lblBoud.Name = "lblBoud";
            // 
            // cmb_Baud
            // 
            this.cmb_Baud.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(78)))));
            this.cmb_Baud.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(119)))), ((int)(((byte)(134)))));
            this.cmb_Baud.BorderSize = 1;
            this.cmb_Baud.DataSource = null;
            resources.ApplyResources(this.cmb_Baud, "cmb_Baud");
            this.cmb_Baud.DropdownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            this.cmb_Baud.DropDownWidth = 0;
            this.cmb_Baud.ForeColor = System.Drawing.Color.DimGray;
            this.cmb_Baud.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(119)))), ((int)(((byte)(134)))));
            this.cmb_Baud.ListBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(20)))), ((int)(((byte)(68)))));
            this.cmb_Baud.ListTextColor = System.Drawing.Color.DimGray;
            this.cmb_Baud.Name = "cmb_Baud";
            this.cmb_Baud.SelectedIndex = -1;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // lblConnection
            // 
            resources.ApplyResources(this.lblConnection, "lblConnection");
            this.lblConnection.Name = "lblConnection";
            // 
            // labelBaud
            // 
            resources.ApplyResources(this.labelBaud, "labelBaud");
            this.labelBaud.Name = "labelBaud";
            // 
            // ConnectionControl
            // 
            this.BackgroundImage = global::MissionPlanner.Properties.Resources.bgdark;
            this.Controls.Add(this.panel5);
            resources.ApplyResources(this, "$this");
            this.Name = "ConnectionControl";
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.pBaud.ResumeLayout(false);
            this.pBaud.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        //private System.Windows.Forms.ComboBox cmb_Baud;
        //private MissionPlanner.Controls.ID_ComboBox cmb_Connection;
        //public MissionPlanner.Controls.ID_ComboBox cmb_sysid;

        // ID_Custom Combo Box
        private MissionPlanner.Controls.ID_ComboBox cmb_Baud;
        private MissionPlanner.Controls.ID_ComboBox cmb_Connection;

        private System.Windows.Forms.Label lblSysId;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Panel panel5;

        private System.Windows.Forms.Label labelBaud;
        //private System.Windows.Forms.Panel panel8;

        private System.Windows.Forms.Label label2;
        //private System.Windows.Forms.Panel panel7;

        private System.Windows.Forms.Label lblConnection;
        private System.Windows.Forms.Panel pBaud;
        private System.Windows.Forms.Label lblBoud;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Label lSysID;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Label lConnection;
        public ID_ComboBox cmb_sysid;
    }
}
