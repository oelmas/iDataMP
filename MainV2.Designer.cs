using Onvif.Core.Client.Common;
using System;
using System.IO;

namespace MissionPlanner
{
    partial class MainV2
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
            Console.WriteLine("mainv2_Dispose");
            if (PluginThreadrunner != null)
                PluginThreadrunner.Dispose();
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainV2));
            this.MainMenu = new System.Windows.Forms.MenuStrip();
            this.CTX_mainmenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.autoHideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fullScreenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.readonlyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectionOptionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectionListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuFlightData = new System.Windows.Forms.ToolStripButton();
            this.MenuFlightPlanner = new System.Windows.Forms.ToolStripButton();
            this.MenuInitConfig = new System.Windows.Forms.ToolStripButton();
            this.MenuConfigTune = new System.Windows.Forms.ToolStripButton();
            this.MenuSimulation = new System.Windows.Forms.ToolStripButton();
            this.MenuHelp = new System.Windows.Forms.ToolStripButton();
            this.MenuConnect = new System.Windows.Forms.ToolStripButton();
            this.toolStripConnectionControl = new MissionPlanner.Controls.ToolStripConnectionControl();
            this.MenuArduPilot = new System.Windows.Forms.ToolStripButton();
            this.menu = new MissionPlanner.Controls.MyButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.status1 = new MissionPlanner.Controls.Status();
            this.btnHelp = new FontAwesome.Sharp.IconButton();
            this.btnSimulation = new FontAwesome.Sharp.IconButton();
            this.btnConfig = new FontAwesome.Sharp.IconButton();
            this.btnSetup = new FontAwesome.Sharp.IconButton();
            this.btnPlan = new FontAwesome.Sharp.IconButton();
            this.btnData = new FontAwesome.Sharp.IconButton();
            this.panelSideMenu = new System.Windows.Forms.Panel();
            this.panelLogo = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.lblEmail = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblFirstName = new System.Windows.Forms.Label();
            this.iconPictureBox1 = new FontAwesome.Sharp.IconPictureBox();
            this.btnLogout = new FontAwesome.Sharp.IconButton();
            this.materialPictureBox1 = new FontAwesome.Sharp.Material.MaterialPictureBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnHideSideMenu = new FontAwesome.Sharp.Material.MaterialButton();
            this.panelSideMenuMin = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.iconPictureBox2 = new FontAwesome.Sharp.IconPictureBox();
            this.btnLogoutMin = new FontAwesome.Sharp.IconButton();
            this.materialPictureBox2 = new FontAwesome.Sharp.Material.MaterialPictureBox();
            this.btnHelpMin = new FontAwesome.Sharp.IconButton();
            this.btnSimulationMin = new FontAwesome.Sharp.IconButton();
            this.btnConfigMin = new FontAwesome.Sharp.IconButton();
            this.btnSetupMin = new FontAwesome.Sharp.IconButton();
            this.btnPlanMin = new FontAwesome.Sharp.IconButton();
            this.btnDataMin = new FontAwesome.Sharp.IconButton();
            this.panel5 = new System.Windows.Forms.Panel();
            this.btnShowSideMenu = new FontAwesome.Sharp.Material.MaterialButton();
            this.panelTop = new System.Windows.Forms.Panel();
            this.lblVersion = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnMinimize = new FontAwesome.Sharp.IconButton();
            this.btnFullView = new FontAwesome.Sharp.IconButton();
            this.btnClose = new FontAwesome.Sharp.IconButton();
            this.panelView = new System.Windows.Forms.Panel();
            this.connectionControl1 = new MissionPlanner.Controls.ConnectionControl();
            this.MainMenu.SuspendLayout();
            this.CTX_mainmenu.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panelSideMenu.SuspendLayout();
            this.panelLogo.SuspendLayout();
            this.panel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.iconPictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.materialPictureBox1)).BeginInit();
            this.panel3.SuspendLayout();
            this.panelSideMenuMin.SuspendLayout();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.iconPictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.materialPictureBox2)).BeginInit();
            this.panel5.SuspendLayout();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainMenu
            // 
            resources.ApplyResources(this.MainMenu, "MainMenu");
            this.MainMenu.ContextMenuStrip = this.CTX_mainmenu;
            this.MainMenu.GripMargin = new System.Windows.Forms.Padding(0);
            this.MainMenu.ImageScalingSize = new System.Drawing.Size(45, 39);
            this.MainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.MenuFlightData, this.MenuFlightPlanner, this.MenuInitConfig, this.MenuConfigTune, this.MenuSimulation, this.MenuHelp, this.MenuConnect, this.toolStripConnectionControl, this.MenuArduPilot });
            this.MainMenu.Name = "MainMenu";
            this.MainMenu.ShowItemToolTips = true;
            this.MainMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.MainMenu_ItemClicked);
            this.MainMenu.MouseLeave += new System.EventHandler(this.MainMenu_MouseLeave);
            // 
            // CTX_mainmenu
            // 
            this.CTX_mainmenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.CTX_mainmenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.autoHideToolStripMenuItem, this.fullScreenToolStripMenuItem, this.readonlyToolStripMenuItem, this.connectionOptionsToolStripMenuItem, this.connectionListToolStripMenuItem });
            this.CTX_mainmenu.Name = "CTX_mainmenu";
            resources.ApplyResources(this.CTX_mainmenu, "CTX_mainmenu");
            // 
            // autoHideToolStripMenuItem
            // 
            this.autoHideToolStripMenuItem.CheckOnClick = true;
            this.autoHideToolStripMenuItem.Name = "autoHideToolStripMenuItem";
            resources.ApplyResources(this.autoHideToolStripMenuItem, "autoHideToolStripMenuItem");
            this.autoHideToolStripMenuItem.Click += new System.EventHandler(this.autoHideToolStripMenuItem_Click);
            // 
            // fullScreenToolStripMenuItem
            // 
            this.fullScreenToolStripMenuItem.CheckOnClick = true;
            this.fullScreenToolStripMenuItem.Name = "fullScreenToolStripMenuItem";
            resources.ApplyResources(this.fullScreenToolStripMenuItem, "fullScreenToolStripMenuItem");
            this.fullScreenToolStripMenuItem.Click += new System.EventHandler(this.fullScreenToolStripMenuItem_Click);
            // 
            // readonlyToolStripMenuItem
            // 
            this.readonlyToolStripMenuItem.CheckOnClick = true;
            this.readonlyToolStripMenuItem.Name = "readonlyToolStripMenuItem";
            resources.ApplyResources(this.readonlyToolStripMenuItem, "readonlyToolStripMenuItem");
            this.readonlyToolStripMenuItem.Click += new System.EventHandler(this.readonlyToolStripMenuItem_Click);
            // 
            // connectionOptionsToolStripMenuItem
            // 
            this.connectionOptionsToolStripMenuItem.Name = "connectionOptionsToolStripMenuItem";
            resources.ApplyResources(this.connectionOptionsToolStripMenuItem, "connectionOptionsToolStripMenuItem");
            this.connectionOptionsToolStripMenuItem.Click += new System.EventHandler(this.connectionOptionsToolStripMenuItem_Click);
            // 
            // connectionListToolStripMenuItem
            // 
            this.connectionListToolStripMenuItem.Name = "connectionListToolStripMenuItem";
            resources.ApplyResources(this.connectionListToolStripMenuItem, "connectionListToolStripMenuItem");
            this.connectionListToolStripMenuItem.Click += new System.EventHandler(this.connectionListToolStripMenuItem_Click);
            // 
            // MenuFlightData
            // 
            this.MenuFlightData.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(227)))), ((int)(((byte)(227)))));
            resources.ApplyResources(this.MenuFlightData, "MenuFlightData");
            this.MenuFlightData.Margin = new System.Windows.Forms.Padding(0);
            this.MenuFlightData.Name = "MenuFlightData";
            this.MenuFlightData.Click += new System.EventHandler(this.MenuFlightData_Click);
            // 
            // MenuFlightPlanner
            // 
            this.MenuFlightPlanner.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(227)))), ((int)(((byte)(227)))));
            resources.ApplyResources(this.MenuFlightPlanner, "MenuFlightPlanner");
            this.MenuFlightPlanner.Margin = new System.Windows.Forms.Padding(0);
            this.MenuFlightPlanner.Name = "MenuFlightPlanner";
            this.MenuFlightPlanner.Click += new System.EventHandler(this.MenuFlightPlanner_Click);
            // 
            // MenuInitConfig
            // 
            this.MenuInitConfig.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(227)))), ((int)(((byte)(227)))));
            resources.ApplyResources(this.MenuInitConfig, "MenuInitConfig");
            this.MenuInitConfig.Margin = new System.Windows.Forms.Padding(0);
            this.MenuInitConfig.Name = "MenuInitConfig";
            this.MenuInitConfig.Click += new System.EventHandler(this.MenuSetup_Click);
            // 
            // MenuConfigTune
            // 
            this.MenuConfigTune.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(227)))), ((int)(((byte)(227)))));
            resources.ApplyResources(this.MenuConfigTune, "MenuConfigTune");
            this.MenuConfigTune.Margin = new System.Windows.Forms.Padding(0);
            this.MenuConfigTune.Name = "MenuConfigTune";
            this.MenuConfigTune.Click += new System.EventHandler(this.MenuTuning_Click);
            // 
            // MenuSimulation
            // 
            this.MenuSimulation.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(227)))), ((int)(((byte)(227)))));
            resources.ApplyResources(this.MenuSimulation, "MenuSimulation");
            this.MenuSimulation.Margin = new System.Windows.Forms.Padding(0);
            this.MenuSimulation.Name = "MenuSimulation";
            this.MenuSimulation.Click += new System.EventHandler(this.MenuSimulation_Click);
            // 
            // MenuHelp
            // 
            this.MenuHelp.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(227)))), ((int)(((byte)(227)))));
            resources.ApplyResources(this.MenuHelp, "MenuHelp");
            this.MenuHelp.Margin = new System.Windows.Forms.Padding(0);
            this.MenuHelp.Name = "MenuHelp";
            this.MenuHelp.Click += new System.EventHandler(this.MenuHelp_Click);
            // 
            // MenuConnect
            // 
            this.MenuConnect.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.MenuConnect.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(227)))), ((int)(((byte)(227)))));
            resources.ApplyResources(this.MenuConnect, "MenuConnect");
            this.MenuConnect.Margin = new System.Windows.Forms.Padding(0);
            this.MenuConnect.Name = "MenuConnect";
            this.MenuConnect.Click += new System.EventHandler(this.MenuConnect_Click);
            // 
            // toolStripConnectionControl
            // 
            this.toolStripConnectionControl.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            resources.ApplyResources(this.toolStripConnectionControl, "toolStripConnectionControl");
            this.toolStripConnectionControl.ForeColor = System.Drawing.Color.Black;
            this.toolStripConnectionControl.Margin = new System.Windows.Forms.Padding(0);
            this.toolStripConnectionControl.Name = "toolStripConnectionControl";
            this.toolStripConnectionControl.MouseLeave += new System.EventHandler(this.MainMenu_MouseLeave);
            // 
            // MenuArduPilot
            // 
            this.MenuArduPilot.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            resources.ApplyResources(this.MenuArduPilot, "MenuArduPilot");
            this.MenuArduPilot.BackColor = System.Drawing.Color.Transparent;
            this.MenuArduPilot.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.MenuArduPilot.ForeColor = System.Drawing.Color.White;
            this.MenuArduPilot.Margin = new System.Windows.Forms.Padding(0);
            this.MenuArduPilot.Name = "MenuArduPilot";
            this.MenuArduPilot.Click += new System.EventHandler(this.MenuArduPilot_Click);
            // 
            // menu
            // 
            this.menu.BGGradBot = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.menu.BGGradTop = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            resources.ApplyResources(this.menu, "menu");
            this.menu.Name = "menu";
            this.menu.UseVisualStyleBackColor = true;
            this.menu.MouseEnter += new System.EventHandler(this.menu_MouseEnter);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.status1);
            this.panel1.Controls.Add(this.MainMenu);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            this.panel1.MouseLeave += new System.EventHandler(this.MainMenu_MouseLeave);
            // 
            // status1
            // 
            resources.ApplyResources(this.status1, "status1");
            this.status1.Name = "status1";
            this.status1.Percent = 0D;
            // 
            // btnHelp
            // 
            this.btnHelp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(53)))), ((int)(((byte)(75)))));
            resources.ApplyResources(this.btnHelp, "btnHelp");
            this.btnHelp.FlatAppearance.BorderSize = 0;
            this.btnHelp.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(40)))), ((int)(((byte)(56)))));
            this.btnHelp.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(198)))), ((int)(((byte)(0)))));
            this.btnHelp.IconChar = FontAwesome.Sharp.IconChar.Question;
            this.btnHelp.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(198)))), ((int)(((byte)(0)))));
            this.btnHelp.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnHelp.IconSize = 32;
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.UseVisualStyleBackColor = false;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // btnSimulation
            // 
            this.btnSimulation.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(53)))), ((int)(((byte)(75)))));
            resources.ApplyResources(this.btnSimulation, "btnSimulation");
            this.btnSimulation.FlatAppearance.BorderSize = 0;
            this.btnSimulation.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(40)))), ((int)(((byte)(56)))));
            this.btnSimulation.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(198)))), ((int)(((byte)(0)))));
            this.btnSimulation.IconChar = FontAwesome.Sharp.IconChar.PlaneDeparture;
            this.btnSimulation.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(198)))), ((int)(((byte)(0)))));
            this.btnSimulation.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnSimulation.IconSize = 32;
            this.btnSimulation.Name = "btnSimulation";
            this.btnSimulation.UseVisualStyleBackColor = false;
            this.btnSimulation.Click += new System.EventHandler(this.btnSimulation_Click);
            // 
            // btnConfig
            // 
            this.btnConfig.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(53)))), ((int)(((byte)(75)))));
            resources.ApplyResources(this.btnConfig, "btnConfig");
            this.btnConfig.FlatAppearance.BorderSize = 0;
            this.btnConfig.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(40)))), ((int)(((byte)(56)))));
            this.btnConfig.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(198)))), ((int)(((byte)(0)))));
            this.btnConfig.IconChar = FontAwesome.Sharp.IconChar.Clapperboard;
            this.btnConfig.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(198)))), ((int)(((byte)(0)))));
            this.btnConfig.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnConfig.IconSize = 32;
            this.btnConfig.Name = "btnConfig";
            this.btnConfig.UseVisualStyleBackColor = false;
            this.btnConfig.Click += new System.EventHandler(this.btnConfig_Click);
            // 
            // btnSetup
            // 
            this.btnSetup.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(53)))), ((int)(((byte)(75)))));
            resources.ApplyResources(this.btnSetup, "btnSetup");
            this.btnSetup.FlatAppearance.BorderSize = 0;
            this.btnSetup.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(40)))), ((int)(((byte)(56)))));
            this.btnSetup.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(198)))), ((int)(((byte)(0)))));
            this.btnSetup.IconChar = FontAwesome.Sharp.IconChar.Wrench;
            this.btnSetup.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(198)))), ((int)(((byte)(0)))));
            this.btnSetup.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnSetup.IconSize = 32;
            this.btnSetup.Name = "btnSetup";
            this.btnSetup.UseVisualStyleBackColor = false;
            this.btnSetup.Click += new System.EventHandler(this.btnSetup_Click);
            // 
            // btnPlan
            // 
            this.btnPlan.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(53)))), ((int)(((byte)(75)))));
            resources.ApplyResources(this.btnPlan, "btnPlan");
            this.btnPlan.FlatAppearance.BorderSize = 0;
            this.btnPlan.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(40)))), ((int)(((byte)(56)))));
            this.btnPlan.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(198)))), ((int)(((byte)(0)))));
            this.btnPlan.IconChar = FontAwesome.Sharp.IconChar.Map;
            this.btnPlan.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(198)))), ((int)(((byte)(0)))));
            this.btnPlan.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnPlan.IconSize = 32;
            this.btnPlan.Name = "btnPlan";
            this.btnPlan.UseVisualStyleBackColor = false;
            this.btnPlan.Click += new System.EventHandler(this.btnPlan_Click);
            // 
            // btnData
            // 
            resources.ApplyResources(this.btnData, "btnData");
            this.btnData.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(53)))), ((int)(((byte)(75)))));
            this.btnData.FlatAppearance.BorderSize = 0;
            this.btnData.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(40)))), ((int)(((byte)(56)))));
            this.btnData.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(198)))), ((int)(((byte)(0)))));
            this.btnData.IconChar = FontAwesome.Sharp.IconChar.PencilSquare;
            this.btnData.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(198)))), ((int)(((byte)(0)))));
            this.btnData.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnData.IconSize = 32;
            this.btnData.Name = "btnData";
            this.btnData.UseVisualStyleBackColor = false;
            // 
            // panelSideMenu
            // 
            this.panelSideMenu.Controls.Add(this.panelLogo);
            this.panelSideMenu.Controls.Add(this.btnHelp);
            this.panelSideMenu.Controls.Add(this.btnSimulation);
            this.panelSideMenu.Controls.Add(this.btnConfig);
            this.panelSideMenu.Controls.Add(this.btnSetup);
            this.panelSideMenu.Controls.Add(this.btnPlan);
            this.panelSideMenu.Controls.Add(this.btnData);
            this.panelSideMenu.Controls.Add(this.panel3);
            resources.ApplyResources(this.panelSideMenu, "panelSideMenu");
            this.panelSideMenu.Name = "panelSideMenu";
            // 
            // panelLogo
            // 
            this.panelLogo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(53)))), ((int)(((byte)(75)))));
            this.panelLogo.Controls.Add(this.connectionControl1);
            this.panelLogo.Controls.Add(this.panel6);
            this.panelLogo.Controls.Add(this.btnLogout);
            this.panelLogo.Controls.Add(this.materialPictureBox1);
            resources.ApplyResources(this.panelLogo, "panelLogo");
            this.panelLogo.ForeColor = System.Drawing.Color.Cyan;
            this.panelLogo.Name = "panelLogo";
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.lblEmail);
            this.panel6.Controls.Add(this.lblTitle);
            this.panel6.Controls.Add(this.lblFirstName);
            this.panel6.Controls.Add(this.iconPictureBox1);
            resources.ApplyResources(this.panel6, "panel6");
            this.panel6.Name = "panel6";
            // 
            // lblEmail
            // 
            resources.ApplyResources(this.lblEmail, "lblEmail");
            this.lblEmail.ForeColor = System.Drawing.Color.Coral;
            this.lblEmail.Name = "lblEmail";
            // 
            // lblTitle
            // 
            resources.ApplyResources(this.lblTitle, "lblTitle");
            this.lblTitle.ForeColor = System.Drawing.Color.Coral;
            this.lblTitle.Name = "lblTitle";
            // 
            // lblFirstName
            // 
            resources.ApplyResources(this.lblFirstName, "lblFirstName");
            this.lblFirstName.ForeColor = System.Drawing.Color.Coral;
            this.lblFirstName.Name = "lblFirstName";
            // 
            // iconPictureBox1
            // 
            this.iconPictureBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(53)))), ((int)(((byte)(75)))));
            resources.ApplyResources(this.iconPictureBox1, "iconPictureBox1");
            this.iconPictureBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(198)))), ((int)(((byte)(0)))));
            this.iconPictureBox1.IconChar = FontAwesome.Sharp.IconChar.PeopleGroup;
            this.iconPictureBox1.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(198)))), ((int)(((byte)(0)))));
            this.iconPictureBox1.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iconPictureBox1.IconSize = 65;
            this.iconPictureBox1.Name = "iconPictureBox1";
            this.iconPictureBox1.TabStop = false;
            // 
            // btnLogout
            // 
            this.btnLogout.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(53)))), ((int)(((byte)(75)))));
            resources.ApplyResources(this.btnLogout, "btnLogout");
            this.btnLogout.FlatAppearance.BorderSize = 0;
            this.btnLogout.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(40)))), ((int)(((byte)(56)))));
            this.btnLogout.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(198)))), ((int)(((byte)(0)))));
            this.btnLogout.IconChar = FontAwesome.Sharp.IconChar.PowerOff;
            this.btnLogout.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(198)))), ((int)(((byte)(0)))));
            this.btnLogout.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnLogout.IconSize = 32;
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.UseVisualStyleBackColor = false;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            // 
            // materialPictureBox1
            // 
            this.materialPictureBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(78)))));
            this.materialPictureBox1.BackgroundImage = global::MissionPlanner.Properties.Resources.InterData_Big;
            resources.ApplyResources(this.materialPictureBox1, "materialPictureBox1");
            this.materialPictureBox1.ForeColor = System.Drawing.Color.Cyan;
            this.materialPictureBox1.IconChar = FontAwesome.Sharp.MaterialIcons.None;
            this.materialPictureBox1.IconColor = System.Drawing.Color.Cyan;
            this.materialPictureBox1.Name = "materialPictureBox1";
            this.materialPictureBox1.TabStop = false;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(53)))), ((int)(((byte)(75)))));
            this.panel3.Controls.Add(this.btnHideSideMenu);
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Name = "panel3";
            // 
            // btnHideSideMenu
            // 
            resources.ApplyResources(this.btnHideSideMenu, "btnHideSideMenu");
            this.btnHideSideMenu.FlatAppearance.BorderSize = 0;
            this.btnHideSideMenu.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(40)))), ((int)(((byte)(56)))));
            this.btnHideSideMenu.IconChar = FontAwesome.Sharp.MaterialIcons.ArrowCollapseAll;
            this.btnHideSideMenu.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(198)))), ((int)(((byte)(0)))));
            this.btnHideSideMenu.IconSize = 36;
            this.btnHideSideMenu.Name = "btnHideSideMenu";
            this.btnHideSideMenu.UseVisualStyleBackColor = true;
            this.btnHideSideMenu.Click += new System.EventHandler(this.btnHideSideMenu_Click_1);
            // 
            // panelSideMenuMin
            // 
            this.panelSideMenuMin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(78)))));
            this.panelSideMenuMin.Controls.Add(this.panel4);
            this.panelSideMenuMin.Controls.Add(this.btnHelpMin);
            this.panelSideMenuMin.Controls.Add(this.btnSimulationMin);
            this.panelSideMenuMin.Controls.Add(this.btnConfigMin);
            this.panelSideMenuMin.Controls.Add(this.btnSetupMin);
            this.panelSideMenuMin.Controls.Add(this.btnPlanMin);
            this.panelSideMenuMin.Controls.Add(this.btnDataMin);
            this.panelSideMenuMin.Controls.Add(this.panel5);
            resources.ApplyResources(this.panelSideMenuMin, "panelSideMenuMin");
            this.panelSideMenuMin.Name = "panelSideMenuMin";
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(53)))), ((int)(((byte)(75)))));
            this.panel4.Controls.Add(this.iconPictureBox2);
            this.panel4.Controls.Add(this.btnLogoutMin);
            this.panel4.Controls.Add(this.materialPictureBox2);
            resources.ApplyResources(this.panel4, "panel4");
            this.panel4.Name = "panel4";
            // 
            // iconPictureBox2
            // 
            this.iconPictureBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(53)))), ((int)(((byte)(75)))));
            resources.ApplyResources(this.iconPictureBox2, "iconPictureBox2");
            this.iconPictureBox2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(198)))), ((int)(((byte)(0)))));
            this.iconPictureBox2.IconChar = FontAwesome.Sharp.IconChar.PeopleGroup;
            this.iconPictureBox2.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(198)))), ((int)(((byte)(0)))));
            this.iconPictureBox2.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iconPictureBox2.IconSize = 50;
            this.iconPictureBox2.Name = "iconPictureBox2";
            this.iconPictureBox2.TabStop = false;
            // 
            // btnLogoutMin
            // 
            this.btnLogoutMin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(53)))), ((int)(((byte)(75)))));
            resources.ApplyResources(this.btnLogoutMin, "btnLogoutMin");
            this.btnLogoutMin.FlatAppearance.BorderSize = 0;
            this.btnLogoutMin.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(40)))), ((int)(((byte)(56)))));
            this.btnLogoutMin.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(198)))), ((int)(((byte)(0)))));
            this.btnLogoutMin.IconChar = FontAwesome.Sharp.IconChar.PowerOff;
            this.btnLogoutMin.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(198)))), ((int)(((byte)(0)))));
            this.btnLogoutMin.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnLogoutMin.IconSize = 32;
            this.btnLogoutMin.Name = "btnLogoutMin";
            this.btnLogoutMin.UseVisualStyleBackColor = false;
            this.btnLogoutMin.Click += new System.EventHandler(this.btnLogoutMin_Click);
            // 
            // materialPictureBox2
            // 
            this.materialPictureBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(78)))));
            this.materialPictureBox2.BackgroundImage = global::MissionPlanner.Properties.Resources.InterData_D;
            resources.ApplyResources(this.materialPictureBox2, "materialPictureBox2");
            this.materialPictureBox2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.materialPictureBox2.IconChar = FontAwesome.Sharp.MaterialIcons.None;
            this.materialPictureBox2.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.materialPictureBox2.Name = "materialPictureBox2";
            this.materialPictureBox2.TabStop = false;
            // 
            // btnHelpMin
            // 
            this.btnHelpMin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(53)))), ((int)(((byte)(75)))));
            resources.ApplyResources(this.btnHelpMin, "btnHelpMin");
            this.btnHelpMin.FlatAppearance.BorderSize = 0;
            this.btnHelpMin.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(40)))), ((int)(((byte)(56)))));
            this.btnHelpMin.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(198)))), ((int)(((byte)(0)))));
            this.btnHelpMin.IconChar = FontAwesome.Sharp.IconChar.Question;
            this.btnHelpMin.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(198)))), ((int)(((byte)(0)))));
            this.btnHelpMin.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnHelpMin.IconSize = 32;
            this.btnHelpMin.Name = "btnHelpMin";
            this.btnHelpMin.UseVisualStyleBackColor = false;
            this.btnHelpMin.Click += new System.EventHandler(this.btnHelpMin_Click);
            // 
            // btnSimulationMin
            // 
            this.btnSimulationMin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(53)))), ((int)(((byte)(75)))));
            resources.ApplyResources(this.btnSimulationMin, "btnSimulationMin");
            this.btnSimulationMin.FlatAppearance.BorderSize = 0;
            this.btnSimulationMin.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(40)))), ((int)(((byte)(56)))));
            this.btnSimulationMin.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(198)))), ((int)(((byte)(0)))));
            this.btnSimulationMin.IconChar = FontAwesome.Sharp.IconChar.PlaneDeparture;
            this.btnSimulationMin.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(198)))), ((int)(((byte)(0)))));
            this.btnSimulationMin.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnSimulationMin.IconSize = 32;
            this.btnSimulationMin.Name = "btnSimulationMin";
            this.btnSimulationMin.UseVisualStyleBackColor = false;
            this.btnSimulationMin.Click += new System.EventHandler(this.btnSimulationMin_Click);
            // 
            // btnConfigMin
            // 
            this.btnConfigMin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(53)))), ((int)(((byte)(75)))));
            resources.ApplyResources(this.btnConfigMin, "btnConfigMin");
            this.btnConfigMin.FlatAppearance.BorderSize = 0;
            this.btnConfigMin.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(40)))), ((int)(((byte)(56)))));
            this.btnConfigMin.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(198)))), ((int)(((byte)(0)))));
            this.btnConfigMin.IconChar = FontAwesome.Sharp.IconChar.Clapperboard;
            this.btnConfigMin.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(198)))), ((int)(((byte)(0)))));
            this.btnConfigMin.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnConfigMin.IconSize = 32;
            this.btnConfigMin.Name = "btnConfigMin";
            this.btnConfigMin.UseVisualStyleBackColor = false;
            this.btnConfigMin.Click += new System.EventHandler(this.btnConfigMin_Click);
            // 
            // btnSetupMin
            // 
            this.btnSetupMin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(53)))), ((int)(((byte)(75)))));
            resources.ApplyResources(this.btnSetupMin, "btnSetupMin");
            this.btnSetupMin.FlatAppearance.BorderSize = 0;
            this.btnSetupMin.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(40)))), ((int)(((byte)(56)))));
            this.btnSetupMin.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(198)))), ((int)(((byte)(0)))));
            this.btnSetupMin.IconChar = FontAwesome.Sharp.IconChar.Wrench;
            this.btnSetupMin.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(198)))), ((int)(((byte)(0)))));
            this.btnSetupMin.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnSetupMin.IconSize = 32;
            this.btnSetupMin.Name = "btnSetupMin";
            this.btnSetupMin.UseVisualStyleBackColor = false;
            this.btnSetupMin.Click += new System.EventHandler(this.btnSetupMin_Click);
            // 
            // btnPlanMin
            // 
            this.btnPlanMin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(53)))), ((int)(((byte)(75)))));
            resources.ApplyResources(this.btnPlanMin, "btnPlanMin");
            this.btnPlanMin.FlatAppearance.BorderSize = 0;
            this.btnPlanMin.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(40)))), ((int)(((byte)(56)))));
            this.btnPlanMin.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(198)))), ((int)(((byte)(0)))));
            this.btnPlanMin.IconChar = FontAwesome.Sharp.IconChar.Map;
            this.btnPlanMin.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(198)))), ((int)(((byte)(0)))));
            this.btnPlanMin.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnPlanMin.IconSize = 32;
            this.btnPlanMin.Name = "btnPlanMin";
            this.btnPlanMin.UseVisualStyleBackColor = false;
            this.btnPlanMin.Click += new System.EventHandler(this.btnPlanMin_Click);
            // 
            // btnDataMin
            // 
            this.btnDataMin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(53)))), ((int)(((byte)(75)))));
            resources.ApplyResources(this.btnDataMin, "btnDataMin");
            this.btnDataMin.FlatAppearance.BorderSize = 0;
            this.btnDataMin.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(40)))), ((int)(((byte)(56)))));
            this.btnDataMin.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(198)))), ((int)(((byte)(0)))));
            this.btnDataMin.IconChar = FontAwesome.Sharp.IconChar.PencilSquare;
            this.btnDataMin.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(198)))), ((int)(((byte)(0)))));
            this.btnDataMin.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnDataMin.IconSize = 32;
            this.btnDataMin.Name = "btnDataMin";
            this.btnDataMin.UseVisualStyleBackColor = false;
            this.btnDataMin.Click += new System.EventHandler(this.btnDataMin_Click);
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(53)))), ((int)(((byte)(75)))));
            this.panel5.Controls.Add(this.btnShowSideMenu);
            resources.ApplyResources(this.panel5, "panel5");
            this.panel5.Name = "panel5";
            // 
            // btnShowSideMenu
            // 
            resources.ApplyResources(this.btnShowSideMenu, "btnShowSideMenu");
            this.btnShowSideMenu.FlatAppearance.BorderSize = 0;
            this.btnShowSideMenu.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(40)))), ((int)(((byte)(56)))));
            this.btnShowSideMenu.IconChar = FontAwesome.Sharp.MaterialIcons.ArrowExpandAll;
            this.btnShowSideMenu.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(198)))), ((int)(((byte)(0)))));
            this.btnShowSideMenu.IconSize = 36;
            this.btnShowSideMenu.Name = "btnShowSideMenu";
            this.btnShowSideMenu.UseVisualStyleBackColor = true;
            this.btnShowSideMenu.Click += new System.EventHandler(this.btnShowSideMenu_Click);
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(53)))), ((int)(((byte)(75)))));
            this.panelTop.Controls.Add(this.lblVersion);
            this.panelTop.Controls.Add(this.pictureBox1);
            this.panelTop.Controls.Add(this.panel2);
            resources.ApplyResources(this.panelTop, "panelTop");
            this.panelTop.Name = "panelTop";
            this.panelTop.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelTop_MouseDown);
            // 
            // lblVersion
            // 
            resources.ApplyResources(this.lblVersion, "lblVersion");
            this.lblVersion.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(198)))), ((int)(((byte)(0)))));
            this.lblVersion.Name = "lblVersion";
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Image = global::MissionPlanner.Properties.Resources.gokalp_blue_eagle_2;
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(53)))), ((int)(((byte)(75)))));
            this.panel2.Controls.Add(this.btnMinimize);
            this.panel2.Controls.Add(this.btnFullView);
            this.panel2.Controls.Add(this.btnClose);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // btnMinimize
            // 
            this.btnMinimize.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.btnMinimize, "btnMinimize");
            this.btnMinimize.FlatAppearance.BorderSize = 0;
            this.btnMinimize.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(198)))), ((int)(((byte)(0)))));
            this.btnMinimize.IconChar = FontAwesome.Sharp.IconChar.WindowMinimize;
            this.btnMinimize.IconColor = System.Drawing.Color.WhiteSmoke;
            this.btnMinimize.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnMinimize.IconSize = 18;
            this.btnMinimize.Name = "btnMinimize";
            this.btnMinimize.UseVisualStyleBackColor = true;
            this.btnMinimize.Click += new System.EventHandler(this.btnMinimize_Click_1);
            // 
            // btnFullView
            // 
            this.btnFullView.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.btnFullView, "btnFullView");
            this.btnFullView.FlatAppearance.BorderSize = 0;
            this.btnFullView.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(198)))), ((int)(((byte)(0)))));
            this.btnFullView.IconChar = FontAwesome.Sharp.IconChar.WindowMaximize;
            this.btnFullView.IconColor = System.Drawing.Color.WhiteSmoke;
            this.btnFullView.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnFullView.IconSize = 18;
            this.btnFullView.Name = "btnFullView";
            this.btnFullView.UseVisualStyleBackColor = true;
            this.btnFullView.Click += new System.EventHandler(this.btnFullView_Click);
            // 
            // btnClose
            // 
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.btnClose, "btnClose");
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Red;
            this.btnClose.IconChar = FontAwesome.Sharp.IconChar.Multiply;
            this.btnClose.IconColor = System.Drawing.Color.WhiteSmoke;
            this.btnClose.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnClose.IconSize = 18;
            this.btnClose.Name = "btnClose";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // panelView
            // 
            resources.ApplyResources(this.panelView, "panelView");
            this.panelView.Name = "panelView";
            // 
            // connectionControl1
            // 
            resources.ApplyResources(this.connectionControl1, "connectionControl1");
            this.connectionControl1.Name = "connectionControl1";
            // 
            // MainV2
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelView);
            this.Controls.Add(this.panelSideMenuMin);
            this.Controls.Add(this.panelSideMenu);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menu);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.MainMenuStrip = this.MainMenu;
            this.Name = "MainV2";
            this.ShowIcon = false;
            this.Load += new System.EventHandler(this.MainV2_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainV2_KeyDown);
            this.Resize += new System.EventHandler(this.MainV2_Resize);
            this.MainMenu.ResumeLayout(false);
            this.MainMenu.PerformLayout();
            this.CTX_mainmenu.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panelSideMenu.ResumeLayout(false);
            this.panelSideMenu.PerformLayout();
            this.panelLogo.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.iconPictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.materialPictureBox1)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panelSideMenuMin.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.iconPictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.materialPictureBox2)).EndInit();
            this.panel5.ResumeLayout(false);
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private FontAwesome.Sharp.IconButton btnLogoutMin;

        #endregion

        public System.Windows.Forms.ToolStripButton MenuFlightData;
        public System.Windows.Forms.ToolStripButton MenuFlightPlanner;
        public System.Windows.Forms.ToolStripButton MenuInitConfig;
        public System.Windows.Forms.ToolStripButton MenuSimulation;
        public System.Windows.Forms.ToolStripButton MenuConfigTune;
        public System.Windows.Forms.ToolStripButton MenuConnect;
        private MissionPlanner.Controls.ToolStripConnectionControl toolStripConnectionControl;
        private MissionPlanner.Controls.MyButton menu;
        public System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ContextMenuStrip CTX_mainmenu;
        private System.Windows.Forms.ToolStripMenuItem autoHideToolStripMenuItem;
        public System.Windows.Forms.MenuStrip MainMenu;
        private System.Windows.Forms.ToolStripMenuItem fullScreenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem readonlyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectionOptionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectionListToolStripMenuItem;
        public System.Windows.Forms.ToolStripButton MenuHelp;
        public System.Windows.Forms.ToolStripButton MenuArduPilot;
        public Controls.Status status1;
        private FontAwesome.Sharp.IconButton btnData;
        private FontAwesome.Sharp.IconButton btnHelp;
        private FontAwesome.Sharp.IconButton btnSimulation;
        private FontAwesome.Sharp.IconButton btnConfig;
        private FontAwesome.Sharp.IconButton btnSetup;
        private FontAwesome.Sharp.IconButton btnPlan;
        private System.Windows.Forms.Panel panelSideMenu;
        private System.Windows.Forms.Panel panelLogo;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panelSideMenuMin;
        private System.Windows.Forms.Panel panel4;
        private FontAwesome.Sharp.IconButton btnHelpMin;
        private FontAwesome.Sharp.IconButton btnSimulationMin;
        private FontAwesome.Sharp.IconButton btnConfigMin;
        private FontAwesome.Sharp.IconButton btnSetupMin;
        private FontAwesome.Sharp.IconButton btnPlanMin;
        private FontAwesome.Sharp.IconButton btnDataMin;
        private System.Windows.Forms.Panel panel5;
        private FontAwesome.Sharp.Material.MaterialPictureBox materialPictureBox2;
        private FontAwesome.Sharp.Material.MaterialPictureBox materialPictureBox1;
        private System.Windows.Forms.Panel panelTop;
        private FontAwesome.Sharp.IconButton btnMinimize;
        private FontAwesome.Sharp.IconButton btnFullView;
        private FontAwesome.Sharp.IconButton btnClose;
        private FontAwesome.Sharp.Material.MaterialButton btnHideSideMenu;
        private FontAwesome.Sharp.Material.MaterialButton btnShowSideMenu;
        private System.Windows.Forms.Panel pTitleBarButton;
        private System.Windows.Forms.Panel panelView;
        private System.Windows.Forms.Panel panel2;
        private FontAwesome.Sharp.IconButton btnLogout;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblFirstName;
        private FontAwesome.Sharp.IconPictureBox iconPictureBox1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private FontAwesome.Sharp.IconPictureBox iconPictureBox2;
        private System.Windows.Forms.Label lblVersion;
        private MissionPlanner.Controls.ConnectionControl connectionControl1;
    }
}