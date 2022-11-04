using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace BSE.Windows.Forms
{
    /// <summary>
    ///     Class for the custom colors at a XPanderPanel control.
    /// </summary>
    /// <copyright>
    ///     Copyright © 2008 Uwe Eichkorn
    ///     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
    ///     KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
    ///     IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
    ///     PURPOSE. IT CAN BE DISTRIBUTED FREE OF CHARGE AS LONG AS THIS HEADER
    ///     REMAINS UNCHANGED.
    /// </copyright>
    public class CustomXPanderPanelColors : CustomColors
    {
        #region FieldsPrivate

        private Color m_backColor = SystemColors.Control;
        private Color m_flatCaptionGradientBegin = ProfessionalColors.ToolStripGradientMiddle;
        private Color m_flatCaptionGradientEnd = ProfessionalColors.ToolStripGradientBegin;
        private Color m_captionPressedGradientBegin = ProfessionalColors.ButtonPressedGradientBegin;
        private Color m_captionPressedGradientEnd = ProfessionalColors.ButtonPressedGradientEnd;
        private Color m_captionPressedGradientMiddle = ProfessionalColors.ButtonPressedGradientMiddle;
        private Color m_captionCheckedGradientBegin = ProfessionalColors.ButtonCheckedGradientBegin;
        private Color m_captionCheckedGradientEnd = ProfessionalColors.ButtonCheckedGradientEnd;
        private Color m_captionCheckedGradientMiddle = ProfessionalColors.ButtonCheckedGradientMiddle;
        private Color m_captionSelectedGradientBegin = ProfessionalColors.ButtonSelectedGradientBegin;
        private Color m_captionSelectedGradientEnd = ProfessionalColors.ButtonSelectedGradientEnd;
        private Color m_captionSelectedGradientMiddle = ProfessionalColors.ButtonSelectedGradientMiddle;
        private Color m_captionSelectedText = SystemColors.ControlText;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the backcolor of a XPanderPanel.
        /// </summary>
        [Description("The backcolor of a XPanderPanel.")]
        public virtual Color BackColor
        {
            get => m_backColor;
            set
            {
                if (value.Equals(m_backColor) == false)
                {
                    m_backColor = value;
                    OnCustomColorsChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        ///     Gets or sets the starting color of the gradient on a flat XPanderPanel captionbar.
        /// </summary>
        [Description("The starting color of the gradient on a flat XPanderPanel captionbar.")]
        public virtual Color FlatCaptionGradientBegin
        {
            get => m_flatCaptionGradientBegin;
            set
            {
                if (value.Equals(m_flatCaptionGradientBegin) == false)
                {
                    m_flatCaptionGradientBegin = value;
                    OnCustomColorsChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        ///     Gets or sets the end color of the gradient on a flat XPanderPanel captionbar.
        /// </summary>
        [Description("The end color of the gradient on a flat XPanderPanel captionbar.")]
        public virtual Color FlatCaptionGradientEnd
        {
            get => m_flatCaptionGradientEnd;
            set
            {
                if (value.Equals(m_flatCaptionGradientEnd) == false)
                {
                    m_flatCaptionGradientEnd = value;
                    OnCustomColorsChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        ///     Gets or sets the starting color of the gradient used when the XPanderPanel is pressed down.
        /// </summary>
        [Description("The starting color of the gradient used when the XPanderPanel is pressed down.")]
        public virtual Color CaptionPressedGradientBegin
        {
            get => m_captionPressedGradientBegin;
            set
            {
                if (value.Equals(m_captionPressedGradientBegin) == false)
                {
                    m_captionPressedGradientBegin = value;
                    OnCustomColorsChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        ///     Gets or sets the end color of the gradient used when the XPanderPanel is pressed down.
        /// </summary>
        [Description("The end color of the gradient used when the XPanderPanel is pressed down.")]
        public virtual Color CaptionPressedGradientEnd
        {
            get => m_captionPressedGradientEnd;
            set
            {
                if (value.Equals(m_captionPressedGradientEnd) == false)
                {
                    m_captionPressedGradientEnd = value;
                    OnCustomColorsChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        ///     Gets or sets the middle color of the gradient used when the XPanderPanel is pressed down.
        /// </summary>
        [Description("The middle color of the gradient used when the XPanderPanel is pressed down.")]
        public virtual Color CaptionPressedGradientMiddle
        {
            get => m_captionPressedGradientMiddle;
            set
            {
                if (value.Equals(m_captionPressedGradientMiddle) == false)
                {
                    m_captionPressedGradientMiddle = value;
                    OnCustomColorsChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        ///     Gets or sets the starting color of the gradient used when the XPanderPanel is checked.
        /// </summary>
        [Description("The starting color of the gradient used when the XPanderPanel is checked.")]
        public virtual Color CaptionCheckedGradientBegin
        {
            get => m_captionCheckedGradientBegin;
            set
            {
                if (value.Equals(m_captionCheckedGradientBegin) == false)
                {
                    m_captionCheckedGradientBegin = value;
                    OnCustomColorsChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        ///     Gets or sets the end color of the gradient used when the XPanderPanel is checked.
        /// </summary>
        [Description("The end color of the gradient used when the XPanderPanel is checked.")]
        public virtual Color CaptionCheckedGradientEnd
        {
            get => m_captionCheckedGradientEnd;
            set
            {
                if (value.Equals(m_captionCheckedGradientEnd) == false)
                {
                    m_captionCheckedGradientEnd = value;
                    OnCustomColorsChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        ///     Gets or sets the middle color of the gradient used when the XPanderPanel is checked.
        /// </summary>
        [Description("The middle color of the gradient used when the XPanderPanel is checked.")]
        public virtual Color CaptionCheckedGradientMiddle
        {
            get => m_captionCheckedGradientMiddle;
            set
            {
                if (value.Equals(m_captionCheckedGradientMiddle) == false)
                {
                    m_captionCheckedGradientMiddle = value;
                    OnCustomColorsChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        ///     Gets or sets the starting color of the gradient used when the XPanderPanel is selected.
        /// </summary>
        [Description("The starting color of the gradient used when the XPanderPanel is selected.")]
        public virtual Color CaptionSelectedGradientBegin
        {
            get => m_captionSelectedGradientBegin;
            set
            {
                if (value.Equals(m_captionSelectedGradientBegin) == false)
                {
                    m_captionSelectedGradientBegin = value;
                    OnCustomColorsChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        ///     Gets or sets the end color of the gradient used when the XPanderPanel is selected.
        /// </summary>
        [Description("The end color of the gradient used when the XPanderPanel is selected.")]
        public virtual Color CaptionSelectedGradientEnd
        {
            get => m_captionSelectedGradientEnd;
            set
            {
                if (value.Equals(m_captionSelectedGradientEnd) == false)
                {
                    m_captionSelectedGradientEnd = value;
                    OnCustomColorsChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        ///     Gets or sets the middle color of the gradient used when the XPanderPanel is selected.
        /// </summary>
        [Description("The middle color of the gradient used when the XPanderPanel is selected.")]
        public virtual Color CaptionSelectedGradientMiddle
        {
            get => m_captionSelectedGradientMiddle;
            set
            {
                if (value.Equals(m_captionSelectedGradientMiddle) == false)
                {
                    m_captionSelectedGradientMiddle = value;
                    OnCustomColorsChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        ///     Gets or sets the text color used when the XPanderPanel is selected.
        /// </summary>
        [Description("The text color used when the XPanderPanel is selected.")]
        public virtual Color CaptionSelectedText
        {
            get => m_captionSelectedText;
            set
            {
                if (value.Equals(m_captionSelectedText) == false)
                {
                    m_captionSelectedText = value;
                    OnCustomColorsChanged(this, EventArgs.Empty);
                }
            }
        }

        #endregion
    }
}