using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace BSE.Windows.Forms
{
    /// <summary>
    ///     Class for the custom colors at a Panel control.
    /// </summary>
    /// <copyright>
    ///     Copyright © 2008 Uwe Eichkorn
    ///     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
    ///     KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
    ///     IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
    ///     PURPOSE. IT CAN BE DISTRIBUTED FREE OF CHARGE AS LONG AS THIS HEADER
    ///     REMAINS UNCHANGED.
    /// </copyright>
    public class CustomPanelColors : CustomColors
    {
        #region FieldsPrivate

        private Color m_captionSelectedGradientBegin = ProfessionalColors.ButtonSelectedGradientBegin;
        private Color m_captionSelectedGradientEnd = ProfessionalColors.ButtonSelectedGradientEnd;
        private Color m_collapsedCaptionText = SystemColors.ControlText;
        private Color m_contentGradientBegin = ProfessionalColors.ToolStripContentPanelGradientBegin;
        private Color m_contentGradientEnd = ProfessionalColors.ToolStripContentPanelGradientEnd;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the starting color of the gradient used when the hover icon in the captionbar on the Panel is
        ///     selected.
        /// </summary>
        [Description("The starting color of the hover icon in the captionbar on the Panel.")]
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
        ///     Gets or sets the end color of the gradient used when the hover icon in the captionbar on the Panel is selected.
        /// </summary>
        [Description("The end color of the hover icon in the captionbar on the Panel.")]
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
        ///     Gets or sets the text color of a Panel when it's collapsed.
        /// </summary>
        [Description("The text color of a Panel when it's collapsed.")]
        public virtual Color CollapsedCaptionText
        {
            get => m_collapsedCaptionText;
            set
            {
                if (value.Equals(m_collapsedCaptionText) == false)
                {
                    m_collapsedCaptionText = value;
                    OnCustomColorsChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        ///     Gets or sets the starting color of the gradient used in the Panel.
        /// </summary>
        [Description("The starting color of the gradient used in the Panel.")]
        public virtual Color ContentGradientBegin
        {
            get => m_contentGradientBegin;
            set
            {
                if (value.Equals(m_contentGradientBegin) == false)
                {
                    m_contentGradientBegin = value;
                    OnCustomColorsChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        ///     Gets or sets the end color of the gradient used in the Panel.
        /// </summary>
        [Description("The end color of the gradient used in the Panel.")]
        public virtual Color ContentGradientEnd
        {
            get => m_contentGradientEnd;
            set
            {
                if (value.Equals(m_contentGradientEnd) == false)
                {
                    m_contentGradientEnd = value;
                    OnCustomColorsChanged(this, EventArgs.Empty);
                }
            }
        }

        #endregion
    }
}