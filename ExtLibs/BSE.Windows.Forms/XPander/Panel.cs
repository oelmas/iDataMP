using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using BSE.Windows.Forms.Properties;

namespace BSE.Windows.Forms
{
    #region Class Panel

    /// <summary>
    ///     Used to group collections of controls.
    /// </summary>
    /// <remarks>
    ///     The Panel is a control that contains other controls.
    ///     You can use a Panel to group collections of controls such as the XPanderPanelList control.
    ///     On top of the Panel there is the captionbar. This captionbar may contain an image and text.
    ///     According to it's dockstyle and properties the panel is collapsable and/or closable.
    /// </remarks>
    /// <copyright>
    ///     Copyright � 2006-2008 Uwe Eichkorn
    ///     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
    ///     KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
    ///     IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
    ///     PURPOSE. IT CAN BE DISTRIBUTED FREE OF CHARGE AS LONG AS THIS HEADER
    ///     REMAINS UNCHANGED.
    /// </copyright>
    [DefaultEvent("CloseClick")]
    [ToolboxBitmap(typeof(System.Windows.Forms.Panel))]
    public partial class Panel : BasePanel
    {
        #region FieldsPrivate

        private Rectangle m_restoreBounds;
        private bool m_bShowTransparentBackground;
        private bool m_bShowXPanderPanelProfessionalStyle;
        private bool m_bShowCaptionbar;
        private LinearGradientMode m_linearGradientMode;
        private Image m_imageClosePanel;
        private Image m_imgHoverBackground;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the associated Splitter. If there is a splitter associated to a panel,
        ///     the splitter visibility always changes when the visibilty of this panel changes.
        /// </summary>
        /// <value>The associated <see cref="Splitter" /></value>
        [Description("The associated Splitter.")]
        [Category("Behavior")]
        public virtual System.Windows.Forms.Splitter AssociatedSplitter { get; set; }

        /// <summary>
        ///     Gets the custom colors which are used for the panel.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description("The custom colors which are used for the panel.")]
        [Category("Appearance")]
        public CustomPanelColors CustomColors { get; }

        /// <summary>
        ///     Expands the panel.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool Expand
        {
            get => base.Expand;
            set => base.Expand = value;
        }

        /// <summary>
        ///     LinearGradientMode of the panels background
        /// </summary>
        [Description("LinearGradientMode of the Panels background")]
        [DefaultValue(1)]
        [Category("Appearance")]
        public LinearGradientMode LinearGradientMode
        {
            get => m_linearGradientMode;
            set
            {
                if (value.Equals(m_linearGradientMode) == false)
                {
                    m_linearGradientMode = value;
                    Invalidate(false);
                }
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the panels captionbar is displayed.
        /// </summary>
        /// <example>
        ///     <code>
        /// private void btnShowHideCaptionbar_Click(object sender, EventArgs e)
        /// {
        ///     //displaye or hides the captionbar at the top of the panel
        ///     panel6.ShowCaptionbar = !panel6.ShowCaptionbar;
        /// }
        /// </code>
        /// </example>
        [Description("A value indicating whether the panels captionbar is displayed.")]
        [DefaultValue(true)]
        [Category("Behavior")]
        public bool ShowCaptionbar
        {
            get => m_bShowCaptionbar;
            set
            {
                if (value.Equals(m_bShowCaptionbar) == false)
                {
                    m_bShowCaptionbar = value;
                    Invalidate(true);
                }
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the controls background is transparent.
        /// </summary>
        [Description("Gets or sets a value indicating whether the controls background is transparent")]
        [DefaultValue(true)]
        [Category("Behavior")]
        public bool ShowTransparentBackground
        {
            get => m_bShowTransparentBackground;
            set
            {
                if (value.Equals(m_bShowTransparentBackground) == false)
                {
                    m_bShowTransparentBackground = value;
                    Invalidate(false);
                }
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the controls caption professional colorscheme is the same then the
        ///     XPanderPanels
        /// </summary>
        [Description(
            "Gets or sets a value indicating whether the controls caption professional colorscheme is the same then the XPanderPanels")]
        [DefaultValue(false)]
        [Category("Behavior")]
        public bool ShowXPanderPanelProfessionalStyle
        {
            get => m_bShowXPanderPanelProfessionalStyle;
            set
            {
                if (value.Equals(m_bShowXPanderPanelProfessionalStyle) == false)
                {
                    m_bShowXPanderPanelProfessionalStyle = value;
                    Invalidate(false);
                }
            }
        }

        /// <summary>
        ///     Gets the size and location of the panel in it's normal expanded state.
        /// </summary>
        /// <remarks>
        ///     A Rect that specifies the size and location of a panel before being either collapsed
        /// </remarks>
        [Browsable(false)]
        public Rectangle RestoreBounds => m_restoreBounds;

        #endregion

        #region MethodsPublic

        /// <summary>
        ///     Initializes a new instance of the Panel class.
        /// </summary>
        public Panel()
        {
            InitializeComponent();

            CaptionFont = new Font(SystemFonts.CaptionFont.FontFamily, SystemFonts.CaptionFont.SizeInPoints + 2.75F,
                FontStyle.Bold);
            BackColor = Color.Transparent;
            ForeColor = SystemColors.ControlText;
            ShowTransparentBackground = true;
            ShowXPanderPanelProfessionalStyle = false;
            ColorScheme = ColorScheme.Professional;
            LinearGradientMode = LinearGradientMode.Vertical;
            Expand = true;
            CaptionHeight = 27;
            ImageSize = new Size(18, 18);
            m_bShowCaptionbar = true;
            CustomColors = new CustomPanelColors();
            CustomColors.CustomColorsChanged += OnCustomColorsChanged;
        }

        /// <summary>
        ///     Sets the PanelProperties for the Panel
        /// </summary>
        /// <param name="panelColors">The PanelColors table</param>
        public override void SetPanelProperties(PanelColors panelColors)
        {
            m_imgHoverBackground = null;
            base.SetPanelProperties(panelColors);
        }

        /// <summary>
        ///     Gets the rectangle that represents the display area of the Panel.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Rectangle DisplayRectangle
        {
            get
            {
                var padding = Padding;
                var displayRectangle = new Rectangle(
                    ClientRectangle.Left + padding.Left,
                    ClientRectangle.Top + padding.Top,
                    ClientRectangle.Width - padding.Left - padding.Right,
                    ClientRectangle.Height - padding.Top - padding.Bottom);

                if (m_bShowCaptionbar)
                    if (Controls.Count > 0)
                    {
                        var xpanderPanelList = Controls[0] as XPanderPanelList;
                        if (xpanderPanelList != null && xpanderPanelList.Dock == DockStyle.Fill)
                            displayRectangle = new Rectangle(
                                padding.Left,
                                CaptionHeight + padding.Top + Constants.BorderThickness,
                                ClientRectangle.Width - padding.Left - padding.Right,
                                ClientRectangle.Height - CaptionHeight - padding.Top - padding.Bottom -
                                2 * Constants.BorderThickness);
                        else
                            displayRectangle = new Rectangle(
                                padding.Left + Constants.BorderThickness,
                                CaptionHeight + padding.Top + Constants.BorderThickness,
                                ClientRectangle.Width - padding.Left - padding.Right - 2 * Constants.BorderThickness,
                                ClientRectangle.Height - CaptionHeight - padding.Top - padding.Bottom -
                                2 * Constants.BorderThickness);
                    }

                return displayRectangle;
            }
        }

        #endregion

        #region MethodsProtected

        /// <summary>
        ///     Raises the ExpandClick event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected override void OnExpandClick(object sender, EventArgs e)
        {
            Expand = !Expand;
            base.OnExpandClick(sender, e);
        }

        /// <summary>
        ///     Raises the ExpandIconHoverState changed event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A HoverStateChangeEventArgs that contains the event data.</param>
        protected override void OnExpandIconHoverStateChanged(object sender, HoverStateChangeEventArgs e)
        {
            Invalidate(RectangleExpandIcon);
            base.OnExpandIconHoverStateChanged(sender, e);
        }

        /// <summary>
        ///     Raises the CloseIconHoverStat changed event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A HoverStateChangeEventArgs that contains the event data.</param>
        protected override void OnCloseIconHoverStateChanged(object sender, HoverStateChangeEventArgs e)
        {
            Invalidate(RectangleCloseIcon);
            base.OnCloseIconHoverStateChanged(sender, e);
        }

        /// <summary>
        ///     Paints the background of the control.
        /// </summary>
        /// <param name="pevent">A PaintEventArgs that contains information about the control to paint.</param>
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            if (ShowTransparentBackground)
            {
                base.OnPaintBackground(pevent);
                BackColor = Color.Transparent;
            }
            else
            {
                var rectangleBounds = ClientRectangle;
                if (m_bShowCaptionbar)
                {
                    BackColor = Color.Transparent;
                    rectangleBounds = new Rectangle(
                        ClientRectangle.Left,
                        ClientRectangle.Top + CaptionHeight,
                        ClientRectangle.Width,
                        ClientRectangle.Height - CaptionHeight);
                }

                RenderBackgroundGradient(
                    pevent.Graphics,
                    rectangleBounds,
                    PanelColors.PanelContentGradientBegin,
                    PanelColors.PanelContentGradientEnd,
                    LinearGradientMode);
            }
        }

        /// <summary>
        ///     Raises the Paint event.
        /// </summary>
        /// <param name="e">A PaintEventArgs that contains the event data.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            var panelStyle = PanelStyle;
            if (m_bShowCaptionbar == false) return;

            using (var antiAlias = new UseAntiAlias(e.Graphics))
            {
                var graphics = e.Graphics;
                using (var clearTypeGridFit = new UseClearTypeGridFit(graphics))
                {
                    var captionRectangle = CaptionRectangle;
                    var colorGradientBegin = PanelColors.PanelCaptionGradientBegin;
                    var colorGradientEnd = PanelColors.PanelCaptionGradientEnd;
                    var colorGradientMiddle = PanelColors.PanelCaptionGradientMiddle;
                    var colorText = PanelColors.PanelCaptionText;
                    var bShowXPanderPanelProfessionalStyle = ShowXPanderPanelProfessionalStyle;
                    var colorSchema = ColorScheme;

                    if (bShowXPanderPanelProfessionalStyle
                        && colorSchema == ColorScheme.Professional
                        && panelStyle != PanelStyle.Office2007)
                    {
                        colorGradientBegin = PanelColors.XPanderPanelCaptionGradientBegin;
                        colorGradientEnd = PanelColors.XPanderPanelCaptionGradientEnd;
                        colorGradientMiddle = PanelColors.XPanderPanelCaptionGradientMiddle;
                        colorText = PanelColors.XPanderPanelCaptionText;
                    }

                    var image = Image;
                    var rightToLeft = RightToLeft;
                    var captionFont = CaptionFont;
                    var clientRectangle = ClientRectangle;
                    var strText = Text;
                    var dockStyle = Dock;
                    var bExpand = Expand;
                    if (m_imageClosePanel == null) m_imageClosePanel = Resources.closePanel;
                    var colorCloseIcon = PanelColors.PanelCaptionCloseIcon;
                    if (colorCloseIcon == Color.Empty) colorCloseIcon = colorText;
                    var bShowExpandIcon = ShowExpandIcon;
                    var bShowCloseIcon = ShowCloseIcon;

                    switch (panelStyle)
                    {
                        case PanelStyle.Default:
                        case PanelStyle.Office2007:
                            DrawStyleDefault(graphics,
                                captionRectangle,
                                colorGradientBegin,
                                colorGradientEnd,
                                colorGradientMiddle);
                            break;
                    }

                    DrawBorder(
                        graphics,
                        clientRectangle,
                        captionRectangle,
                        PanelColors.BorderColor,
                        PanelColors.InnerBorderColor);

                    if (dockStyle == DockStyle.Fill || dockStyle == DockStyle.None ||
                        (bShowExpandIcon == false && bShowCloseIcon == false))
                    {
                        DrawImagesAndText(
                            graphics,
                            captionRectangle,
                            CaptionSpacing,
                            ImageRectangle,
                            image,
                            rightToLeft,
                            captionFont,
                            colorText,
                            strText);

                        return;
                    }

                    if (bShowExpandIcon || bShowCloseIcon)
                    {
                        var imageExpandPanel = GetExpandImage(dockStyle, bExpand);

                        DrawImagesAndText(
                            graphics,
                            dockStyle,
                            CaptionSpacing,
                            captionRectangle,
                            clientRectangle,
                            ImageRectangle,
                            image,
                            rightToLeft,
                            bShowCloseIcon,
                            m_imageClosePanel,
                            colorCloseIcon,
                            ref RectangleCloseIcon,
                            bShowExpandIcon,
                            bExpand,
                            imageExpandPanel,
                            colorText,
                            ref RectangleExpandIcon,
                            captionFont,
                            colorText,
                            PanelColors.PanelCollapsedCaptionText,
                            strText);

                        if (m_imgHoverBackground == null)
                            m_imgHoverBackground = GetPanelIconBackground(
                                graphics,
                                ImageRectangle,
                                PanelColors.PanelCaptionSelectedGradientBegin,
                                PanelColors.PanelCaptionSelectedGradientEnd);
                        if (m_imgHoverBackground != null)
                        {
                            var rectangleCloseIcon = RectangleCloseIcon;
                            if (rectangleCloseIcon != Rectangle.Empty)
                                if (HoverStateCloseIcon == HoverState.Hover)
                                {
                                    graphics.DrawImage(m_imgHoverBackground, rectangleCloseIcon);
                                    DrawIcon(graphics, m_imageClosePanel, rectangleCloseIcon, colorCloseIcon,
                                        rectangleCloseIcon.Y);
                                }

                            var rectangleExpandIcon = RectangleExpandIcon;
                            if (rectangleExpandIcon != Rectangle.Empty)
                                if (HoverStateExpandIcon == HoverState.Hover)
                                {
                                    graphics.DrawImage(m_imgHoverBackground, rectangleExpandIcon);
                                    DrawIcon(graphics, imageExpandPanel, rectangleExpandIcon, colorText,
                                        rectangleExpandIcon.Y);
                                }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Raises the PanelCollapsing event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A XPanderStateChangeEventArgs that contains the event data.</param>
        protected override void OnPanelCollapsing(object sender, XPanderStateChangeEventArgs e)
        {
            if (Dock == DockStyle.Left || Dock == DockStyle.Right)
                foreach (Control control in Controls)
                    control.Hide();

            if (Dock == DockStyle.Left || Dock == DockStyle.Right)
            {
                if (ClientRectangle.Width > CaptionHeight) m_restoreBounds = ClientRectangle;
                Width = CaptionHeight;
            }

            if (Dock == DockStyle.Top || Dock == DockStyle.Bottom)
            {
                if (ClientRectangle.Height > CaptionHeight) m_restoreBounds = ClientRectangle;
                Height = CaptionHeight;
            }

            base.OnPanelCollapsing(sender, e);
        }

        /// <summary>
        ///     Raises the PanelExpanding event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A XPanderStateChangeEventArgs that contains the event data.</param>
        protected override void OnPanelExpanding(object sender, XPanderStateChangeEventArgs e)
        {
            if (Dock == DockStyle.Left || Dock == DockStyle.Right)
            {
                foreach (Control control in Controls) control.Show();

                //When ClientRectangle.Width > CaptionHeight the panel size has changed
                //otherwise the captionclick event was executed
                if (ClientRectangle.Width == CaptionHeight) Width = m_restoreBounds.Width;
            }

            if (Dock == DockStyle.Top || Dock == DockStyle.Bottom) Height = m_restoreBounds.Height;

            base.OnPanelExpanding(sender, e);
        }

        /// <summary>
        ///     Raises the PanelStyleChanged event
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A EventArgs that contains the event data.</param>
        protected override void OnPanelStyleChanged(object sender, PanelStyleChangeEventArgs e)
        {
            OnLayout(new LayoutEventArgs(this, null));
            base.OnPanelStyleChanged(sender, e);
        }

        /// <summary>
        ///     Raises the CreateControl method.
        /// </summary>
        protected override void OnCreateControl()
        {
            m_restoreBounds = ClientRectangle;
            MinimumSize = new Size(CaptionHeight, CaptionHeight);
            base.OnCreateControl();
        }

        /// <summary>
        ///     Raises the Resize event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected override void OnResize(EventArgs e)
        {
            if (ShowExpandIcon)
            {
                if (Expand == false)
                {
                    if (Dock == DockStyle.Left || Dock == DockStyle.Right)
                        if (Width > CaptionHeight)
                            Expand = true;
                    if (Dock == DockStyle.Top || Dock == DockStyle.Bottom)
                        if (Height > CaptionHeight)
                            Expand = true;
                }
                else
                {
                    if (Dock == DockStyle.Left || Dock == DockStyle.Right)
                        if (Width == CaptionHeight)
                            Expand = false;
                    if (Dock == DockStyle.Top || Dock == DockStyle.Bottom)
                        if (Height == CaptionHeight)
                            Expand = false;
                }
            }

            base.OnResize(e);
        }

        /// <summary>
        ///     Raises the <see cref="Control.VisibleChanged" />VisibleChanged event.
        /// </summary>
        /// <param name="e">An <see cref="System.EventArgs" /> that contains the event data.</param>
        protected override void OnVisibleChanged(EventArgs e)
        {
            var associatedSplitter = AssociatedSplitter;
            if (associatedSplitter != null) associatedSplitter.Visible = Visible;
            base.OnVisibleChanged(e);
        }

        #endregion

        #region MethodsPrivate

        /// <summary>
        ///     Gets the background for an panelicon image
        /// </summary>
        /// <param name="graphics">The Graphics to draw on.</param>
        /// <param name="rectanglePanelIcon"></param>
        /// <param name="backgroundColorBegin"></param>
        /// <param name="backgroundColorEnd"></param>
        /// <returns></returns>
        private static Image GetPanelIconBackground(Graphics graphics, Rectangle rectanglePanelIcon,
            Color backgroundColorBegin, Color backgroundColorEnd)
        {
            var rectangle = rectanglePanelIcon;
            rectangle.X = 0;
            rectangle.Y = 0;
            Image image = new Bitmap(rectanglePanelIcon.Width, rectanglePanelIcon.Height, graphics);
            using (var imageGraphics = Graphics.FromImage(image))
            {
                RenderBackgroundGradient(
                    imageGraphics,
                    rectangle,
                    backgroundColorBegin,
                    backgroundColorEnd,
                    LinearGradientMode.Vertical);
            }

            return image;
        }

        private static void DrawStyleDefault(Graphics graphics,
            Rectangle captionRectangle,
            Color colorGradientBegin,
            Color colorGradientEnd,
            Color colorGradientMiddle)
        {
            RenderDoubleBackgroundGradient(
                graphics,
                captionRectangle,
                colorGradientBegin,
                colorGradientMiddle,
                colorGradientEnd,
                LinearGradientMode.Vertical,
                true);
        }

        private static void DrawBorder(
            Graphics graphics,
            Rectangle panelRectangle,
            Rectangle captionRectangle,
            Color borderColor,
            Color innerBorderColor)
        {
            using (var borderPen = new Pen(borderColor))
            {
                // Draws the innerborder around the captionbar
                var innerBorderRectangle = captionRectangle;
                innerBorderRectangle.Width -= Constants.BorderThickness;
                innerBorderRectangle.Offset(Constants.BorderThickness, Constants.BorderThickness);
                ControlPaint.DrawBorder(
                    graphics,
                    innerBorderRectangle,
                    innerBorderColor,
                    ButtonBorderStyle.Solid);

                // Draws the outer border around the captionbar
                ControlPaint.DrawBorder(
                    graphics,
                    panelRectangle,
                    borderColor,
                    ButtonBorderStyle.Solid);

                // Draws the line below the captionbar
                graphics.DrawLine(
                    borderPen,
                    captionRectangle.X,
                    captionRectangle.Y + captionRectangle.Height,
                    captionRectangle.Width,
                    captionRectangle.Y + captionRectangle.Height);

                if (panelRectangle.Height == captionRectangle.Height) return;

                // Draws the border lines around the whole panel
                var panelBorderRectangle = panelRectangle;
                panelBorderRectangle.Y = captionRectangle.Height;
                panelBorderRectangle.Height -= captionRectangle.Height + (int)borderPen.Width;
                panelBorderRectangle.Width -= (int)borderPen.Width;
                Point[] points =
                {
                    new Point(panelBorderRectangle.X, panelBorderRectangle.Y),
                    new Point(panelBorderRectangle.X, panelBorderRectangle.Y + panelBorderRectangle.Height),
                    new Point(panelBorderRectangle.X + panelBorderRectangle.Width,
                        panelBorderRectangle.Y + panelBorderRectangle.Height),
                    new Point(panelBorderRectangle.X + panelBorderRectangle.Width, panelBorderRectangle.Y)
                };
                graphics.DrawLines(borderPen, points);
            }
        }

        private static Image GetExpandImage(DockStyle dockStyle, bool bIsExpanded)
        {
            Image image = null;
            if (dockStyle == DockStyle.Left && bIsExpanded)
                image = Resources.ChevronLeft;
            else if (dockStyle == DockStyle.Left && bIsExpanded == false)
                image = Resources.ChevronRight;
            else if (dockStyle == DockStyle.Right && bIsExpanded)
                image = Resources.ChevronRight;
            else if (dockStyle == DockStyle.Right && bIsExpanded == false)
                image = Resources.ChevronLeft;
            else if (dockStyle == DockStyle.Top && bIsExpanded)
                image = Resources.ChevronUp;
            else if (dockStyle == DockStyle.Top && bIsExpanded == false)
                image = Resources.ChevronDown;
            else if (dockStyle == DockStyle.Bottom && bIsExpanded)
                image = Resources.ChevronDown;
            else if (dockStyle == DockStyle.Bottom && bIsExpanded == false) image = Resources.ChevronUp;

            return image;
        }

        #endregion
    }

    #endregion
}