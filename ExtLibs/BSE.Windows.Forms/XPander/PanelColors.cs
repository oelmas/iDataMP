using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace BSE.Windows.Forms
{
	/// <summary>
	///     Provides <see cref="Color" /> structures that are colors of a Panel or XPanderPanel display element.
	/// </summary>
	/// <copyright>
	///     Copyright � 2006-2008 Uwe Eichkorn
	///     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
	///     KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
	///     IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
	///     PURPOSE. IT CAN BE DISTRIBUTED FREE OF CHARGE AS LONG AS THIS HEADER
	///     REMAINS UNCHANGED.
	/// </copyright>
	public class PanelColors
    {
        #region Enums

        /// <summary>
        ///     Gets or sets the KnownColors appearance of the ProfessionalColorTable.
        /// </summary>
        public enum KnownColors
        {
	        /// <summary>
	        ///     The border color of the panel.
	        /// </summary>
	        BorderColor,

	        /// <summary>
	        ///     The forecolor of a close icon in a Panel.
	        /// </summary>
	        PanelCaptionCloseIcon,

	        /// <summary>
	        ///     The forecolor of a expand icon in a Panel.
	        /// </summary>
	        PanelCaptionExpandIcon,

	        /// <summary>
	        ///     The starting color of the gradient of the Panel.
	        /// </summary>
	        PanelCaptionGradientBegin,

	        /// <summary>
	        ///     The end color of the gradient of the Panel.
	        /// </summary>
	        PanelCaptionGradientEnd,

	        /// <summary>
	        ///     The middle color of the gradient of the Panel.
	        /// </summary>
	        PanelCaptionGradientMiddle,

	        /// <summary>
	        ///     The starting color of the gradient used when the hover icon in the captionbar on the Panel is selected.
	        /// </summary>
	        PanelCaptionSelectedGradientBegin,

	        /// <summary>
	        ///     The end color of the gradient used when the hover icon in the captionbar on the Panel is selected.
	        /// </summary>
	        PanelCaptionSelectedGradientEnd,

	        /// <summary>
	        ///     The starting color of the gradient used in the Panel.
	        /// </summary>
	        PanelContentGradientBegin,

	        /// <summary>
	        ///     The end color of the gradient used in the Panel.
	        /// </summary>
	        PanelContentGradientEnd,

	        /// <summary>
	        ///     The text color of a Panel.
	        /// </summary>
	        PanelCaptionText,

	        /// <summary>
	        ///     The text color of a Panel when it's collapsed.
	        /// </summary>
	        PanelCollapsedCaptionText,

	        /// <summary>
	        ///     The inner border color of a Panel.
	        /// </summary>
	        InnerBorderColor,

	        /// <summary>
	        ///     The backcolor of a XPanderPanel.
	        /// </summary>
	        XPanderPanelBackColor,

	        /// <summary>
	        ///     The forecolor of a close icon in a XPanderPanel.
	        /// </summary>
	        XPanderPanelCaptionCloseIcon,

	        /// <summary>
	        ///     The forecolor of a expand icon in a XPanderPanel.
	        /// </summary>
	        XPanderPanelCaptionExpandIcon,

	        /// <summary>
	        ///     The text color of a XPanderPanel.
	        /// </summary>
	        XPanderPanelCaptionText,

	        /// <summary>
	        ///     The starting color of the gradient of the XPanderPanel.
	        /// </summary>
	        XPanderPanelCaptionGradientBegin,

	        /// <summary>
	        ///     The end color of the gradient of the XPanderPanel.
	        /// </summary>
	        XPanderPanelCaptionGradientEnd,

	        /// <summary>
	        ///     The middle color of the gradient of the XPanderPanel.
	        /// </summary>
	        XPanderPanelCaptionGradientMiddle,

	        /// <summary>
	        ///     The starting color of the gradient of a flat XPanderPanel.
	        /// </summary>
	        XPanderPanelFlatCaptionGradientBegin,

	        /// <summary>
	        ///     The end color of the gradient of a flat XPanderPanel.
	        /// </summary>
	        XPanderPanelFlatCaptionGradientEnd,

	        /// <summary>
	        ///     The starting color of the gradient used when the XPanderPanel is pressed down.
	        /// </summary>
	        XPanderPanelPressedCaptionBegin,

	        /// <summary>
	        ///     The end color of the gradient used when the XPanderPanel is pressed down.
	        /// </summary>
	        XPanderPanelPressedCaptionEnd,

	        /// <summary>
	        ///     The middle color of the gradient used when the XPanderPanel is pressed down.
	        /// </summary>
	        XPanderPanelPressedCaptionMiddle,

	        /// <summary>
	        ///     The starting color of the gradient used when the XPanderPanel is checked.
	        /// </summary>
	        XPanderPanelCheckedCaptionBegin,

	        /// <summary>
	        ///     The end color of the gradient used when the XPanderPanel is checked.
	        /// </summary>
	        XPanderPanelCheckedCaptionEnd,

	        /// <summary>
	        ///     The middle color of the gradient used when the XPanderPanel is checked.
	        /// </summary>
	        XPanderPanelCheckedCaptionMiddle,

	        /// <summary>
	        ///     The starting color of the gradient used when the XPanderPanel is selected.
	        /// </summary>
	        XPanderPanelSelectedCaptionBegin,

	        /// <summary>
	        ///     The end color of the gradient used when the XPanderPanel is selected.
	        /// </summary>
	        XPanderPanelSelectedCaptionEnd,

	        /// <summary>
	        ///     The middle color of the gradient used when the XPanderPanel is selected.
	        /// </summary>
	        XPanderPanelSelectedCaptionMiddle,

	        /// <summary>
	        ///     The text color used when the XPanderPanel is selected.
	        /// </summary>
	        XPanderPanelSelectedCaptionText
        }

        #endregion

        #region MethodsProtected

        /// <summary>
        ///     Initialize a color Dictionary with defined colors
        /// </summary>
        /// <param name="rgbTable">Dictionary with defined colors</param>
        protected virtual void InitColors(Dictionary<KnownColors, Color> rgbTable)
        {
            InitBaseColors(rgbTable);
        }

        #endregion

        #region FieldsPrivate

        private readonly System.Windows.Forms.ProfessionalColorTable m_professionalColorTable;
        private Dictionary<KnownColors, Color> m_dictionaryRGBTable;
        private bool m_bUseSystemColors;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the border color of a Panel or XPanderPanel.
        /// </summary>
        public virtual Color BorderColor => FromKnownColor(KnownColors.BorderColor);

        /// <summary>
        ///     Gets the forecolor of a close icon in a Panel.
        /// </summary>
        public virtual Color PanelCaptionCloseIcon => FromKnownColor(KnownColors.PanelCaptionCloseIcon);

        /// <summary>
        ///     Gets the forecolor of an expand icon in a Panel.
        /// </summary>
        public virtual Color PanelCaptionExpandIcon => FromKnownColor(KnownColors.PanelCaptionExpandIcon);

        /// <summary>
        ///     Gets the starting color of the gradient of the Panel.
        /// </summary>
        public virtual Color PanelCaptionGradientBegin => FromKnownColor(KnownColors.PanelCaptionGradientBegin);

        /// <summary>
        ///     Gets the end color of the gradient of the Panel.
        /// </summary>
        public virtual Color PanelCaptionGradientEnd => FromKnownColor(KnownColors.PanelCaptionGradientEnd);

        /// <summary>
        ///     Gets the middle color of the gradient of the Panel.
        /// </summary>
        public virtual Color PanelCaptionGradientMiddle => FromKnownColor(KnownColors.PanelCaptionGradientMiddle);

        /// <summary>
        ///     Gets the starting color of the gradient used when the hover icon in the captionbar on the Panel is selected.
        /// </summary>
        public virtual Color PanelCaptionSelectedGradientBegin =>
            FromKnownColor(KnownColors.PanelCaptionSelectedGradientBegin);

        /// <summary>
        ///     Gets the end color of the gradient used when the hover icon in the captionbar on the Panel is selected.
        /// </summary>
        public virtual Color PanelCaptionSelectedGradientEnd =>
            FromKnownColor(KnownColors.PanelCaptionSelectedGradientEnd);

        /// <summary>
        ///     Gets the text color of a Panel.
        /// </summary>
        public virtual Color PanelCaptionText => FromKnownColor(KnownColors.PanelCaptionText);

        /// <summary>
        ///     Gets the text color of a Panel when it's collapsed.
        /// </summary>
        public virtual Color PanelCollapsedCaptionText => FromKnownColor(KnownColors.PanelCollapsedCaptionText);

        /// <summary>
        ///     Gets the starting color of the gradient used in the Panel.
        /// </summary>
        public virtual Color PanelContentGradientBegin => FromKnownColor(KnownColors.PanelContentGradientBegin);

        /// <summary>
        ///     Gets the end color of the gradient used in the Panel.
        /// </summary>
        public virtual Color PanelContentGradientEnd => FromKnownColor(KnownColors.PanelContentGradientEnd);

        /// <summary>
        ///     Gets the inner border color of a Panel.
        /// </summary>
        public virtual Color InnerBorderColor => FromKnownColor(KnownColors.InnerBorderColor);

        /// <summary>
        ///     Gets the backcolor of a XPanderPanel.
        /// </summary>
        public virtual Color XPanderPanelBackColor => FromKnownColor(KnownColors.XPanderPanelBackColor);

        /// <summary>
        ///     Gets the forecolor of a close icon in a XPanderPanel.
        /// </summary>
        public virtual Color XPanderPanelCaptionCloseIcon => FromKnownColor(KnownColors.XPanderPanelCaptionCloseIcon);

        /// <summary>
        ///     Gets the forecolor of an expand icon in a XPanderPanel.
        /// </summary>
        public virtual Color XPanderPanelCaptionExpandIcon => FromKnownColor(KnownColors.XPanderPanelCaptionExpandIcon);

        /// <summary>
        ///     Gets the starting color of the gradient of the XPanderPanel.
        /// </summary>
        public virtual Color XPanderPanelCaptionGradientBegin =>
            FromKnownColor(KnownColors.XPanderPanelCaptionGradientBegin);

        /// <summary>
        ///     Gets the end color of the gradient of the XPanderPanel.
        /// </summary>
        public virtual Color XPanderPanelCaptionGradientEnd =>
            FromKnownColor(KnownColors.XPanderPanelCaptionGradientEnd);

        /// <summary>
        ///     Gets the middle color of the gradient on the XPanderPanel captionbar.
        /// </summary>
        public virtual Color XPanderPanelCaptionGradientMiddle =>
            FromKnownColor(KnownColors.XPanderPanelCaptionGradientMiddle);

        /// <summary>
        ///     Gets the text color of a XPanderPanel.
        /// </summary>
        public virtual Color XPanderPanelCaptionText => FromKnownColor(KnownColors.XPanderPanelCaptionText);

        /// <summary>
        ///     Gets the starting color of the gradient on a flat XPanderPanel captionbar.
        /// </summary>
        public virtual Color XPanderPanelFlatCaptionGradientBegin =>
            FromKnownColor(KnownColors.XPanderPanelFlatCaptionGradientBegin);

        /// <summary>
        ///     Gets the end color of the gradient on a flat XPanderPanel captionbar.
        /// </summary>
        public virtual Color XPanderPanelFlatCaptionGradientEnd =>
            FromKnownColor(KnownColors.XPanderPanelFlatCaptionGradientEnd);

        /// <summary>
        ///     Gets the starting color of the gradient used when the XPanderPanel is pressed down.
        /// </summary>
        public virtual Color XPanderPanelPressedCaptionBegin =>
            FromKnownColor(KnownColors.XPanderPanelPressedCaptionBegin);

        /// <summary>
        ///     Gets the end color of the gradient used when the XPanderPanel is pressed down.
        /// </summary>
        public virtual Color XPanderPanelPressedCaptionEnd => FromKnownColor(KnownColors.XPanderPanelPressedCaptionEnd);

        /// <summary>
        ///     Gets the middle color of the gradient used when the XPanderPanel is pressed down.
        /// </summary>
        public virtual Color XPanderPanelPressedCaptionMiddle =>
            FromKnownColor(KnownColors.XPanderPanelPressedCaptionMiddle);

        /// <summary>
        ///     Gets the starting color of the gradient used when the XPanderPanel is checked.
        /// </summary>
        public virtual Color XPanderPanelCheckedCaptionBegin =>
            FromKnownColor(KnownColors.XPanderPanelCheckedCaptionBegin);

        /// <summary>
        ///     Gets the end color of the gradient used when the XPanderPanel is checked.
        /// </summary>
        public virtual Color XPanderPanelCheckedCaptionEnd => FromKnownColor(KnownColors.XPanderPanelCheckedCaptionEnd);

        /// <summary>
        ///     Gets the middle color of the gradient used when the XPanderPanel is checked.
        /// </summary>
        public virtual Color XPanderPanelCheckedCaptionMiddle =>
            FromKnownColor(KnownColors.XPanderPanelCheckedCaptionMiddle);

        /// <summary>
        ///     Gets the starting color of the gradient used when the XPanderPanel is selected.
        /// </summary>
        public virtual Color XPanderPanelSelectedCaptionBegin =>
            FromKnownColor(KnownColors.XPanderPanelSelectedCaptionBegin);

        /// <summary>
        ///     Gets the end color of the gradient used when the XPanderPanel is selected.
        /// </summary>
        public virtual Color XPanderPanelSelectedCaptionEnd =>
            FromKnownColor(KnownColors.XPanderPanelSelectedCaptionEnd);

        /// <summary>
        ///     Gets the middle color of the gradient used when the XPanderPanel is selected.
        /// </summary>
        public virtual Color XPanderPanelSelectedCaptionMiddle =>
            FromKnownColor(KnownColors.XPanderPanelSelectedCaptionMiddle);

        /// <summary>
        ///     Gets the text color used when the XPanderPanel is selected.
        /// </summary>
        public virtual Color XPanderPanelSelectedCaptionText =>
            FromKnownColor(KnownColors.XPanderPanelSelectedCaptionText);

        /// <summary>
        ///     Gets the associated PanelStyle for the XPanderControls
        /// </summary>
        public virtual PanelStyle PanelStyle => PanelStyle.Default;

        /// <summary>
        ///     Gets or sets a value indicating whether to use System.Drawing.SystemColors rather than colors that match the
        ///     current visual style.
        /// </summary>
        public bool UseSystemColors
        {
            get => m_bUseSystemColors;
            set
            {
                if (value.Equals(m_bUseSystemColors) == false)
                {
                    m_bUseSystemColors = value;
                    m_professionalColorTable.UseSystemColors = m_bUseSystemColors;
                    Clear();
                }
            }
        }

        /// <summary>
        ///     Gets or sets the panel or xpanderpanel
        /// </summary>
        public BasePanel Panel { get; set; }

        internal Color FromKnownColor(KnownColors color)
        {
            return ColorTable[color];
        }

        private Dictionary<KnownColors, Color> ColorTable
        {
            get
            {
                if (m_dictionaryRGBTable == null)
                {
                    m_dictionaryRGBTable = new Dictionary<KnownColors, Color>(0xd4);
                    if (Panel != null && Panel.ColorScheme == ColorScheme.Professional)
                    {
                        if (m_bUseSystemColors || ToolStripManager.VisualStylesEnabled == false)
                            InitBaseColors(m_dictionaryRGBTable);
                        else
                            InitColors(m_dictionaryRGBTable);
                    }
                    else
                    {
                        InitCustomColors(m_dictionaryRGBTable);
                    }
                }

                return m_dictionaryRGBTable;
            }
        }

        #endregion

        #region MethodsPublic

        /// <summary>
        ///     Initializes a new instance of the PanelColors class.
        /// </summary>
        public PanelColors()
        {
            m_professionalColorTable = new System.Windows.Forms.ProfessionalColorTable();
        }

        /// <summary>
        ///     Initialize a new instance of the PanelColors class.
        /// </summary>
        /// <param name="basePanel">Base class for the panel or xpanderpanel control.</param>
        public PanelColors(BasePanel basePanel) : this()
        {
            Panel = basePanel;
        }

        /// <summary>
        ///     Clears the current color table
        /// </summary>
        public void Clear()
        {
            ResetRGBTable();
        }

        #endregion

        #region MethodsPrivate

        private void InitBaseColors(Dictionary<KnownColors, Color> rgbTable)
        {
            rgbTable[KnownColors.BorderColor] = m_professionalColorTable.GripDark;
            rgbTable[KnownColors.InnerBorderColor] = m_professionalColorTable.GripLight;
            rgbTable[KnownColors.PanelCaptionCloseIcon] = SystemColors.ControlText;
            rgbTable[KnownColors.PanelCaptionExpandIcon] = SystemColors.ControlText;
            rgbTable[KnownColors.PanelCaptionGradientBegin] = m_professionalColorTable.ToolStripGradientBegin;
            rgbTable[KnownColors.PanelCaptionGradientEnd] = m_professionalColorTable.ToolStripGradientEnd;
            rgbTable[KnownColors.PanelCaptionGradientMiddle] = m_professionalColorTable.ToolStripGradientMiddle;
            rgbTable[KnownColors.PanelCaptionSelectedGradientBegin] =
                m_professionalColorTable.ButtonSelectedGradientBegin;
            rgbTable[KnownColors.PanelCaptionSelectedGradientEnd] = m_professionalColorTable.ButtonSelectedGradientEnd;
            rgbTable[KnownColors.PanelContentGradientBegin] =
                m_professionalColorTable.ToolStripContentPanelGradientBegin;
            rgbTable[KnownColors.PanelContentGradientEnd] = m_professionalColorTable.ToolStripContentPanelGradientEnd;
            rgbTable[KnownColors.PanelCaptionText] = SystemColors.ControlText;
            rgbTable[KnownColors.PanelCollapsedCaptionText] = SystemColors.ControlText;
            rgbTable[KnownColors.XPanderPanelBackColor] = m_professionalColorTable.ToolStripContentPanelGradientBegin;
            rgbTable[KnownColors.XPanderPanelCaptionCloseIcon] = SystemColors.ControlText;
            rgbTable[KnownColors.XPanderPanelCaptionExpandIcon] = SystemColors.ControlText;
            rgbTable[KnownColors.XPanderPanelCaptionText] = SystemColors.ControlText;
            rgbTable[KnownColors.XPanderPanelCaptionGradientBegin] = m_professionalColorTable.ToolStripGradientBegin;
            rgbTable[KnownColors.XPanderPanelCaptionGradientEnd] = m_professionalColorTable.ToolStripGradientEnd;
            rgbTable[KnownColors.XPanderPanelCaptionGradientMiddle] = m_professionalColorTable.ToolStripGradientMiddle;
            rgbTable[KnownColors.XPanderPanelFlatCaptionGradientBegin] =
                m_professionalColorTable.ToolStripGradientMiddle;
            rgbTable[KnownColors.XPanderPanelFlatCaptionGradientEnd] = m_professionalColorTable.ToolStripGradientBegin;
            rgbTable[KnownColors.XPanderPanelPressedCaptionBegin] = m_professionalColorTable.ButtonPressedGradientBegin;
            rgbTable[KnownColors.XPanderPanelPressedCaptionEnd] = m_professionalColorTable.ButtonPressedGradientEnd;
            rgbTable[KnownColors.XPanderPanelPressedCaptionMiddle] =
                m_professionalColorTable.ButtonPressedGradientMiddle;
            rgbTable[KnownColors.XPanderPanelCheckedCaptionBegin] = m_professionalColorTable.ButtonCheckedGradientBegin;
            rgbTable[KnownColors.XPanderPanelCheckedCaptionEnd] = m_professionalColorTable.ButtonCheckedGradientEnd;
            rgbTable[KnownColors.XPanderPanelCheckedCaptionMiddle] =
                m_professionalColorTable.ButtonCheckedGradientMiddle;
            rgbTable[KnownColors.XPanderPanelSelectedCaptionBegin] =
                m_professionalColorTable.ButtonSelectedGradientBegin;
            rgbTable[KnownColors.XPanderPanelSelectedCaptionEnd] = m_professionalColorTable.ButtonSelectedGradientEnd;
            rgbTable[KnownColors.XPanderPanelSelectedCaptionMiddle] =
                m_professionalColorTable.ButtonSelectedGradientMiddle;
            rgbTable[KnownColors.XPanderPanelSelectedCaptionText] = SystemColors.ControlText;
        }

        private void InitCustomColors(Dictionary<KnownColors, Color> rgbTable)
        {
            var panel = Panel as Panel;
            if (panel != null)
            {
                rgbTable[KnownColors.BorderColor] = panel.CustomColors.BorderColor;
                rgbTable[KnownColors.InnerBorderColor] = panel.CustomColors.InnerBorderColor;
                rgbTable[KnownColors.PanelCaptionCloseIcon] = panel.CustomColors.CaptionCloseIcon;
                rgbTable[KnownColors.PanelCaptionExpandIcon] = panel.CustomColors.CaptionExpandIcon;
                rgbTable[KnownColors.PanelCaptionGradientBegin] = panel.CustomColors.CaptionGradientBegin;
                rgbTable[KnownColors.PanelCaptionGradientEnd] = panel.CustomColors.CaptionGradientEnd;
                rgbTable[KnownColors.PanelCaptionGradientMiddle] = panel.CustomColors.CaptionGradientMiddle;
                rgbTable[KnownColors.PanelCaptionSelectedGradientBegin] =
                    panel.CustomColors.CaptionSelectedGradientBegin;
                rgbTable[KnownColors.PanelCaptionSelectedGradientEnd] = panel.CustomColors.CaptionSelectedGradientEnd;
                rgbTable[KnownColors.PanelContentGradientBegin] = panel.CustomColors.ContentGradientBegin;
                rgbTable[KnownColors.PanelContentGradientEnd] = panel.CustomColors.ContentGradientEnd;
                rgbTable[KnownColors.PanelCaptionText] = panel.CustomColors.CaptionText;
                rgbTable[KnownColors.PanelCollapsedCaptionText] = panel.CustomColors.CollapsedCaptionText;
            }

            var xpanderPanel = Panel as XPanderPanel;
            if (xpanderPanel != null)
            {
                rgbTable[KnownColors.BorderColor] = xpanderPanel.CustomColors.BorderColor;
                rgbTable[KnownColors.InnerBorderColor] = xpanderPanel.CustomColors.InnerBorderColor;
                rgbTable[KnownColors.XPanderPanelBackColor] = xpanderPanel.CustomColors.BackColor;
                rgbTable[KnownColors.XPanderPanelCaptionCloseIcon] = xpanderPanel.CustomColors.CaptionCloseIcon;
                rgbTable[KnownColors.XPanderPanelCaptionExpandIcon] = xpanderPanel.CustomColors.CaptionExpandIcon;
                rgbTable[KnownColors.XPanderPanelCaptionText] = xpanderPanel.CustomColors.CaptionText;
                rgbTable[KnownColors.XPanderPanelCaptionGradientBegin] = xpanderPanel.CustomColors.CaptionGradientBegin;
                rgbTable[KnownColors.XPanderPanelCaptionGradientEnd] = xpanderPanel.CustomColors.CaptionGradientEnd;
                rgbTable[KnownColors.XPanderPanelCaptionGradientMiddle] =
                    xpanderPanel.CustomColors.CaptionGradientMiddle;
                rgbTable[KnownColors.XPanderPanelFlatCaptionGradientBegin] =
                    xpanderPanel.CustomColors.FlatCaptionGradientBegin;
                rgbTable[KnownColors.XPanderPanelFlatCaptionGradientEnd] =
                    xpanderPanel.CustomColors.FlatCaptionGradientEnd;
                rgbTable[KnownColors.XPanderPanelPressedCaptionBegin] =
                    xpanderPanel.CustomColors.CaptionPressedGradientBegin;
                rgbTable[KnownColors.XPanderPanelPressedCaptionEnd] =
                    xpanderPanel.CustomColors.CaptionPressedGradientEnd;
                rgbTable[KnownColors.XPanderPanelPressedCaptionMiddle] =
                    xpanderPanel.CustomColors.CaptionPressedGradientMiddle;
                rgbTable[KnownColors.XPanderPanelCheckedCaptionBegin] =
                    xpanderPanel.CustomColors.CaptionCheckedGradientBegin;
                rgbTable[KnownColors.XPanderPanelCheckedCaptionEnd] =
                    xpanderPanel.CustomColors.CaptionCheckedGradientEnd;
                rgbTable[KnownColors.XPanderPanelCheckedCaptionMiddle] =
                    xpanderPanel.CustomColors.CaptionCheckedGradientMiddle;
                rgbTable[KnownColors.XPanderPanelSelectedCaptionBegin] =
                    xpanderPanel.CustomColors.CaptionSelectedGradientBegin;
                rgbTable[KnownColors.XPanderPanelSelectedCaptionEnd] =
                    xpanderPanel.CustomColors.CaptionSelectedGradientEnd;
                rgbTable[KnownColors.XPanderPanelSelectedCaptionMiddle] =
                    xpanderPanel.CustomColors.CaptionSelectedGradientMiddle;
                rgbTable[KnownColors.XPanderPanelSelectedCaptionText] = xpanderPanel.CustomColors.CaptionSelectedText;
            }
        }

        private void ResetRGBTable()
        {
            if (m_dictionaryRGBTable != null) m_dictionaryRGBTable.Clear();
            m_dictionaryRGBTable = null;
        }

        #endregion
    }
}