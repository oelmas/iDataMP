using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using BSE.Windows.Forms.Properties;

namespace BSE.Windows.Forms
{
    #region Class XPanderPanel

    /// <summary>
    ///     Used to group collections of controls.
    /// </summary>
    /// <remarks>
    ///     XPanderPanel controls represent the expandable and collapsable panels in XPanderPanelList.
    ///     The XpanderPanel is a control that contains other controls.
    ///     You can use a XPanderPanel to group collections of controls such as the XPanderPanelList.
    ///     The order of xpanderpanels in the XPanderPanelList.XPanderPanels collection reflects the order
    ///     of xpanderpanels controls. To change the order of tabs in the control, you must change
    ///     their positions in the collection by removing them and inserting them at new indexes.
    ///     You can change the xpanderpanel's appearance. For example, to make it appear flat,
    ///     set the CaptionStyle property to CaptionStyle.Flat.
    ///     On top of the XPanderPanel there is the captionbar.
    ///     This captionbar may contain an image and text. According to it's properties the panel is closable.
    /// </remarks>
    /// <copyright>
    ///     Copyright � 2006-2008 Uwe Eichkorn
    ///     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
    ///     KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
    ///     IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
    ///     PURPOSE. IT CAN BE DISTRIBUTED FREE OF CHARGE AS LONG AS THIS HEADER
    ///     REMAINS UNCHANGED.
    /// </copyright>
    [DesignTimeVisible(false)]
    public partial class XPanderPanel : BasePanel
    {
        #region EventsPublic

        /// <summary>
        ///     The CaptionStyleChanged event occurs when CaptionStyle flags have been changed.
        /// </summary>
        [Description("The CaptionStyleChanged event occurs when CaptionStyle flags have been changed.")]
        public event EventHandler<EventArgs> CaptionStyleChanged;

        #endregion

        #region Constants

        #endregion

        #region FieldsPrivate

        private Image m_imageChevron;
        private Image m_imageChevronUp;
        private Image m_imageChevronDown;
        private Image m_imageClosePanel;
        private bool m_bIsClosable = true;
        private CaptionStyle m_captionStyle;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets a value indicating whether the expand icon in a XPanderPanel is visible.
        /// </summary>
        [Description("Gets or sets a value indicating whether the expand icon in a XPanderPanel is visible.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DefaultValue(false)]
        [Browsable(false)]
        [Category("Appearance")]
        public override bool ShowExpandIcon
        {
            get => base.ShowExpandIcon;
            set => base.ShowExpandIcon = value;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the close icon in a XPanderPanel is visible.
        /// </summary>
        [Description("Gets or sets a value indicating whether the close icon in a XPanderPanel is visible.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DefaultValue(false)]
        [Browsable(false)]
        [Category("Appearance")]
        public override bool ShowCloseIcon
        {
            get => base.ShowCloseIcon;
            set => base.ShowCloseIcon = value;
        }

        /// <summary>
        ///     Gets the custom colors which are used for the XPanderPanel.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description("The custom colors which are used for the XPanderPanel.")]
        [Category("Appearance")]
        public CustomXPanderPanelColors CustomColors { get; }

        /// <summary>
        ///     Gets or sets the style of the caption (not for PanelStyle.Aqua).
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public CaptionStyle CaptionStyle
        {
            get => m_captionStyle;
            set
            {
                if (value.Equals(m_captionStyle) == false)
                {
                    m_captionStyle = value;
                    OnCaptionStyleChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this XPanderPanel is closable.
        /// </summary>
        [Description("Gets or sets a value indicating whether this XPanderPanel is closable.")]
        [DefaultValue(true)]
        [Category("Appearance")]
        public bool IsClosable
        {
            get => m_bIsClosable;
            set
            {
                if (value.Equals(m_bIsClosable) == false)
                {
                    m_bIsClosable = value;
                    Invalidate(false);
                }
            }
        }

        /// <summary>
        ///     Gets or sets the height and width of the XPanderPanel.
        /// </summary>
        [Browsable(false)]
        public new Size Size
        {
            get => base.Size;
            set => base.Size = value;
        }

        #endregion

        #region MethodsPublic

        /// <summary>
        ///     Initializes a new instance of the XPanderPanel class.
        /// </summary>
        public XPanderPanel()
        {
            InitializeComponent();

            BackColor = Color.Transparent;
            CaptionStyle = CaptionStyle.Normal;
            ForeColor = SystemColors.ControlText;
            Height = CaptionHeight;
            ShowBorder = true;
            CustomColors = new CustomXPanderPanelColors();
            CustomColors.CustomColorsChanged += OnCustomColorsChanged;
        }

        /// <summary>
        ///     Gets the rectangle that represents the display area of the XPanderPanel.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Rectangle DisplayRectangle
        {
            get
            {
                var padding = Padding;

                var displayRectangle = new Rectangle(
                    padding.Left + Constants.BorderThickness,
                    padding.Top + CaptionHeight,
                    ClientRectangle.Width - padding.Left - padding.Right - 2 * Constants.BorderThickness,
                    ClientRectangle.Height - CaptionHeight - padding.Top - padding.Bottom);

                if (Controls.Count > 0)
                {
                    var xpanderPanelList = Controls[0] as XPanderPanelList;
                    if (xpanderPanelList != null && xpanderPanelList.Dock == DockStyle.Fill)
                        displayRectangle = new Rectangle(
                            padding.Left,
                            padding.Top + CaptionHeight,
                            ClientRectangle.Width - padding.Left - padding.Right,
                            ClientRectangle.Height - CaptionHeight - padding.Top - padding.Bottom -
                            Constants.BorderThickness);
                }

                return displayRectangle;
            }
        }

        #endregion

        #region MethodsProtected

        /// <summary>
        ///     Paints the background of the control.
        /// </summary>
        /// <param name="pevent">A PaintEventArgs that contains information about the control to paint.</param>
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            base.OnPaintBackground(pevent);
            base.BackColor = Color.Transparent;
            var backColor = PanelColors.XPanderPanelBackColor;
            if (backColor != Color.Empty && backColor != Color.Transparent)
            {
                var rectangle = new Rectangle(
                    0,
                    CaptionHeight,
                    ClientRectangle.Width,
                    ClientRectangle.Height - CaptionHeight);

                using (var backgroundBrush = new SolidBrush(backColor))
                {
                    pevent.Graphics.FillRectangle(backgroundBrush, rectangle);
                }
            }
        }

        /// <summary>
        ///     Raises the Paint event.
        /// </summary>
        /// <param name="e">A PaintEventArgs that contains the event data.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (IsZeroWidthOrHeight(CaptionRectangle)) return;

            using (var antiAlias = new UseAntiAlias(e.Graphics))
            {
                var graphics = e.Graphics;
                using (var clearTypeGridFit = new UseClearTypeGridFit(graphics))
                {
                    var bExpand = Expand;
                    var bShowBorder = ShowBorder;
                    var borderColor = PanelColors.BorderColor;
                    var borderRectangle = ClientRectangle;

                    switch (PanelStyle)
                    {
                        case PanelStyle.Default:
                        case PanelStyle.Office2007:
                            DrawCaptionbar(graphics, bExpand, bShowBorder, PanelStyle);
                            CalculatePanelHeights();
                            DrawBorders(graphics, this);
                            break;
                    }
                }
            }
        }

        /// <summary>
        ///     Raises the PanelExpanding event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A XPanderStateChangeEventArgs that contains the event data.</param>
        protected override void OnPanelExpanding(object sender, XPanderStateChangeEventArgs e)
        {
            var bExpand = e.Expand;
            if (bExpand)
            {
                Expand = bExpand;
                Invalidate(false);
            }

            base.OnPanelExpanding(sender, e);
        }

        /// <summary>
        ///     Raises the CaptionStyleChanged event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected virtual void OnCaptionStyleChanged(object sender, EventArgs e)
        {
            Invalidate(CaptionRectangle);
            if (CaptionStyleChanged != null) CaptionStyleChanged(sender, e);
        }

        /// <summary>
        ///     Raises the MouseUp event.
        /// </summary>
        /// <param name="e">A MouseEventArgs that contains data about the OnMouseUp event.</param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (CaptionRectangle.Contains(e.X, e.Y))
            {
                if (ShowCloseIcon == false && ShowExpandIcon == false)
                    OnExpandClick(this, EventArgs.Empty);
                else if (ShowCloseIcon && ShowExpandIcon == false)
                    if (RectangleCloseIcon.Contains(e.X, e.Y) == false)
                        OnExpandClick(this, EventArgs.Empty);
                if (ShowExpandIcon)
                    if (RectangleExpandIcon.Contains(e.X, e.Y))
                        OnExpandClick(this, EventArgs.Empty);
                if (ShowCloseIcon && m_bIsClosable)
                    if (RectangleCloseIcon.Contains(e.X, e.Y))
                        OnCloseClick(this, EventArgs.Empty);
            }
        }

        /// <summary>
        ///     Raises the VisibleChanged event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (DesignMode) return;
            if (Visible == false)
                if (Expand)
                {
                    Expand = false;
                    foreach (Control control in Parent.Controls)
                    {
                        var xpanderPanel =
                            control as XPanderPanel;

                        if (xpanderPanel != null)
                            if (xpanderPanel.Visible)
                            {
                                xpanderPanel.Expand = true;
                                return;
                            }
                    }
                }
#if DEBUG
            //System.Diagnostics.Trace.WriteLine("Visibility: " + this.Name + this.Visible);
#endif
            CalculatePanelHeights();
        }

        #endregion

        #region MethodsPrivate

        private void DrawCaptionbar(Graphics graphics, bool bExpand, bool bShowBorder, PanelStyle panelStyle)
        {
            var captionRectangle = CaptionRectangle;
            var colorGradientBegin = PanelColors.XPanderPanelCaptionGradientBegin;
            var colorGradientEnd = PanelColors.XPanderPanelCaptionGradientEnd;
            var colorGradientMiddle = PanelColors.XPanderPanelCaptionGradientMiddle;
            var colorText = PanelColors.XPanderPanelCaptionText;
            var foreColorCloseIcon = PanelColors.XPanderPanelCaptionCloseIcon;
            var foreColorExpandIcon = PanelColors.XPanderPanelCaptionExpandIcon;
            var bHover = HoverStateCaptionBar == HoverState.Hover ? true : false;

            if (m_imageClosePanel == null) m_imageClosePanel = Resources.closePanel;
            if (m_imageChevronUp == null) m_imageChevronUp = Resources.ChevronUp;
            if (m_imageChevronDown == null) m_imageChevronDown = Resources.ChevronDown;

            m_imageChevron = m_imageChevronDown;
            if (bExpand) m_imageChevron = m_imageChevronUp;

            if (m_captionStyle == CaptionStyle.Normal)
            {
                if (bHover)
                {
                    colorGradientBegin = PanelColors.XPanderPanelSelectedCaptionBegin;
                    colorGradientEnd = PanelColors.XPanderPanelSelectedCaptionEnd;
                    colorGradientMiddle = PanelColors.XPanderPanelSelectedCaptionMiddle;
                    if (bExpand)
                    {
                        colorGradientBegin = PanelColors.XPanderPanelPressedCaptionBegin;
                        colorGradientEnd = PanelColors.XPanderPanelPressedCaptionEnd;
                        colorGradientMiddle = PanelColors.XPanderPanelPressedCaptionMiddle;
                    }

                    colorText = PanelColors.XPanderPanelSelectedCaptionText;
                    foreColorCloseIcon = colorText;
                    foreColorExpandIcon = colorText;
                }
                else
                {
                    if (bExpand)
                    {
                        colorGradientBegin = PanelColors.XPanderPanelCheckedCaptionBegin;
                        colorGradientEnd = PanelColors.XPanderPanelCheckedCaptionEnd;
                        colorGradientMiddle = PanelColors.XPanderPanelCheckedCaptionMiddle;
                        colorText = PanelColors.XPanderPanelSelectedCaptionText;
                        foreColorCloseIcon = colorText;
                        foreColorExpandIcon = colorText;
                    }
                }

                if (panelStyle != PanelStyle.Office2007)
                    RenderDoubleBackgroundGradient(
                        graphics,
                        captionRectangle,
                        colorGradientBegin,
                        colorGradientMiddle,
                        colorGradientEnd,
                        LinearGradientMode.Vertical,
                        false);
                else
                    RenderButtonBackground(
                        graphics,
                        captionRectangle,
                        colorGradientBegin,
                        colorGradientMiddle,
                        colorGradientEnd);
            }
            else
            {
                var colorFlatGradientBegin = PanelColors.XPanderPanelFlatCaptionGradientBegin;
                var colorFlatGradientEnd = PanelColors.XPanderPanelFlatCaptionGradientEnd;
                var colorInnerBorder = PanelColors.InnerBorderColor;
                colorText = PanelColors.XPanderPanelCaptionText;
                foreColorExpandIcon = colorText;

                RenderFlatButtonBackground(graphics, captionRectangle, colorFlatGradientBegin, colorFlatGradientEnd,
                    bHover);
                DrawInnerBorders(graphics, this);
            }

            DrawImagesAndText(
                graphics,
                captionRectangle,
                CaptionSpacing,
                ImageRectangle,
                Image,
                RightToLeft,
                m_bIsClosable,
                ShowCloseIcon,
                m_imageClosePanel,
                foreColorCloseIcon,
                ref RectangleCloseIcon,
                ShowExpandIcon,
                m_imageChevron,
                foreColorExpandIcon,
                ref RectangleExpandIcon,
                CaptionFont,
                colorText,
                Text);
        }

        private static void DrawBorders(Graphics graphics, XPanderPanel xpanderPanel)
        {
            if (xpanderPanel.ShowBorder)
                using (var graphicsPath = new GraphicsPath())
                {
                    using (var borderPen = new Pen(xpanderPanel.PanelColors.BorderColor, Constants.BorderThickness))
                    {
                        var captionRectangle = xpanderPanel.CaptionRectangle;
                        var borderRectangle = captionRectangle;

                        if (xpanderPanel.Expand)
                        {
                            borderRectangle = xpanderPanel.ClientRectangle;

                            graphics.DrawLine(
                                borderPen,
                                captionRectangle.Left,
                                captionRectangle.Top + captionRectangle.Height - Constants.BorderThickness,
                                captionRectangle.Left + captionRectangle.Width,
                                captionRectangle.Top + captionRectangle.Height - Constants.BorderThickness);
                        }

                        var xpanderPanelList = xpanderPanel.Parent as XPanderPanelList;
                        if (xpanderPanelList != null && xpanderPanelList.Dock == DockStyle.Fill)
                        {
                            var panel = xpanderPanelList.Parent as Panel;
                            var parentXPanderPanel = xpanderPanelList.Parent as XPanderPanel;
                            if ((panel != null && panel.Padding == new Padding(0)) ||
                                (parentXPanderPanel != null && parentXPanderPanel.Padding == new Padding(0)))
                            {
                                if (xpanderPanel.Top != 0)
                                    graphicsPath.AddLine(
                                        borderRectangle.Left,
                                        borderRectangle.Top,
                                        borderRectangle.Left + captionRectangle.Width,
                                        borderRectangle.Top);

                                // Left vertical borderline
                                graphics.DrawLine(borderPen,
                                    borderRectangle.Left,
                                    borderRectangle.Top,
                                    borderRectangle.Left,
                                    borderRectangle.Top + borderRectangle.Height);

                                // Right vertical borderline
                                graphics.DrawLine(borderPen,
                                    borderRectangle.Left + borderRectangle.Width - Constants.BorderThickness,
                                    borderRectangle.Top,
                                    borderRectangle.Left + borderRectangle.Width - Constants.BorderThickness,
                                    borderRectangle.Top + borderRectangle.Height);
                            }
                            else
                            {
                                // Upper horizontal borderline only at the top xpanderPanel
                                if (xpanderPanel.Top == 0)
                                    graphicsPath.AddLine(
                                        borderRectangle.Left,
                                        borderRectangle.Top,
                                        borderRectangle.Left + borderRectangle.Width,
                                        borderRectangle.Top);

                                // Left vertical borderline
                                graphicsPath.AddLine(
                                    borderRectangle.Left,
                                    borderRectangle.Top,
                                    borderRectangle.Left,
                                    borderRectangle.Top + borderRectangle.Height);

                                //Lower horizontal borderline
                                graphicsPath.AddLine(
                                    borderRectangle.Left,
                                    borderRectangle.Top + borderRectangle.Height - Constants.BorderThickness,
                                    borderRectangle.Left + borderRectangle.Width - Constants.BorderThickness,
                                    borderRectangle.Top + borderRectangle.Height - Constants.BorderThickness);

                                // Right vertical borderline
                                graphicsPath.AddLine(
                                    borderRectangle.Left + borderRectangle.Width - Constants.BorderThickness,
                                    borderRectangle.Top,
                                    borderRectangle.Left + borderRectangle.Width - Constants.BorderThickness,
                                    borderRectangle.Top + borderRectangle.Height);
                            }
                        }
                        else
                        {
                            // Upper horizontal borderline only at the top xpanderPanel
                            if (xpanderPanel.Top == 0)
                                graphicsPath.AddLine(
                                    borderRectangle.Left,
                                    borderRectangle.Top,
                                    borderRectangle.Left + borderRectangle.Width,
                                    borderRectangle.Top);

                            // Left vertical borderline
                            graphicsPath.AddLine(
                                borderRectangle.Left,
                                borderRectangle.Top,
                                borderRectangle.Left,
                                borderRectangle.Top + borderRectangle.Height);

                            //Lower horizontal borderline
                            graphicsPath.AddLine(
                                borderRectangle.Left,
                                borderRectangle.Top + borderRectangle.Height - Constants.BorderThickness,
                                borderRectangle.Left + borderRectangle.Width - Constants.BorderThickness,
                                borderRectangle.Top + borderRectangle.Height - Constants.BorderThickness);

                            // Right vertical borderline
                            graphicsPath.AddLine(
                                borderRectangle.Left + borderRectangle.Width - Constants.BorderThickness,
                                borderRectangle.Top,
                                borderRectangle.Left + borderRectangle.Width - Constants.BorderThickness,
                                borderRectangle.Top + borderRectangle.Height);
                        }
                    }

                    using (var borderPen = new Pen(xpanderPanel.PanelColors.BorderColor, Constants.BorderThickness))
                    {
                        graphics.DrawPath(borderPen, graphicsPath);
                    }
                }
        }


        private static void DrawInnerBorders(Graphics graphics, XPanderPanel xpanderPanel)
        {
            if (xpanderPanel.ShowBorder)
                using (var graphicsPath = new GraphicsPath())
                {
                    var captionRectangle = xpanderPanel.CaptionRectangle;
                    var xpanderPanelList = xpanderPanel.Parent as XPanderPanelList;
                    if (xpanderPanelList != null && xpanderPanelList.Dock == DockStyle.Fill)
                    {
                        var panel = xpanderPanelList.Parent as Panel;
                        var parentXPanderPanel = xpanderPanelList.Parent as XPanderPanel;
                        if ((panel != null && panel.Padding == new Padding(0)) ||
                            (parentXPanderPanel != null && parentXPanderPanel.Padding == new Padding(0)))
                        {
                            //Left vertical borderline
                            graphicsPath.AddLine(captionRectangle.X, captionRectangle.Y + captionRectangle.Height,
                                captionRectangle.X, captionRectangle.Y + Constants.BorderThickness);
                            if (xpanderPanel.Top == 0)
                                //Upper horizontal borderline
                                graphicsPath.AddLine(captionRectangle.X, captionRectangle.Y,
                                    captionRectangle.X + captionRectangle.Width, captionRectangle.Y);
                            else
                                //Upper horizontal borderline
                                graphicsPath.AddLine(captionRectangle.X, captionRectangle.Y + Constants.BorderThickness,
                                    captionRectangle.X + captionRectangle.Width,
                                    captionRectangle.Y + Constants.BorderThickness);
                        }
                    }
                    else
                    {
                        //Left vertical borderline
                        graphicsPath.AddLine(captionRectangle.X + Constants.BorderThickness,
                            captionRectangle.Y + captionRectangle.Height,
                            captionRectangle.X + Constants.BorderThickness, captionRectangle.Y);
                        if (xpanderPanel.Top == 0)
                            //Upper horizontal borderline
                            graphicsPath.AddLine(captionRectangle.X + Constants.BorderThickness,
                                captionRectangle.Y + Constants.BorderThickness,
                                captionRectangle.X + captionRectangle.Width - Constants.BorderThickness,
                                captionRectangle.Y + Constants.BorderThickness);
                        else
                            //Upper horizontal borderline
                            graphicsPath.AddLine(captionRectangle.X + Constants.BorderThickness, captionRectangle.Y,
                                captionRectangle.X + captionRectangle.Width - Constants.BorderThickness,
                                captionRectangle.Y);
                    }

                    using (var borderPen = new Pen(xpanderPanel.PanelColors.InnerBorderColor))
                    {
                        graphics.DrawPath(borderPen, graphicsPath);
                    }
                }
        }

        private void CalculatePanelHeights()
        {
            if (Parent == null) return;

            var iPanelHeight = Parent.Padding.Top;

            foreach (Control control in Parent.Controls)
            {
                var xpanderPanel =
                    control as XPanderPanel;

                if (xpanderPanel != null && xpanderPanel.Visible) iPanelHeight += xpanderPanel.CaptionHeight;
            }

            iPanelHeight += Parent.Padding.Bottom;

            foreach (Control control in Parent.Controls)
            {
                var xpanderPanel =
                    control as XPanderPanel;

                if (xpanderPanel != null)
                {
                    if (xpanderPanel.Expand)
                        xpanderPanel.Height = Parent.Height
                                              + xpanderPanel.CaptionHeight
                                              - iPanelHeight;
                    else
                        xpanderPanel.Height = xpanderPanel.CaptionHeight;
                }
            }

            var iTop = Parent.Padding.Top;
            foreach (Control control in Parent.Controls)
            {
                var xpanderPanel =
                    control as XPanderPanel;

                if (xpanderPanel != null && xpanderPanel.Visible)
                {
                    xpanderPanel.Top = iTop;
                    iTop += xpanderPanel.Height;
                }
            }
        }

        #endregion
    }

    #endregion
}