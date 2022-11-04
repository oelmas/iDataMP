using System;
using System.Drawing;
using System.Drawing.Text;
using System.Globalization;
using BSE.Windows.Forms.Properties;

namespace BSE.Windows.Forms
{
	/// <summary>
	///     Set the TextRenderingHint.ClearTypeGridFit until instance disposed.
	/// </summary>
	public class UseClearTypeGridFit : IDisposable
    {
        #region MethodsProtected

        /// <summary>
        ///     Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                //Revert the TextRenderingHint back to original setting.
                m_graphics.TextRenderingHint = m_textRenderingHint;
        }

        #endregion

        #region FieldsPrivate

        private readonly Graphics m_graphics;
        private readonly TextRenderingHint m_textRenderingHint;

        #endregion

        #region MethodsPublic

        /// <summary>
        ///     Initialize a new instance of the UseClearTypeGridFit class.
        /// </summary>
        /// <param name="graphics">Graphics instance.</param>
        public UseClearTypeGridFit(Graphics graphics)
        {
            if (graphics == null)
                throw new ArgumentNullException("graphics",
                    string.Format(CultureInfo.InvariantCulture,
                        Resources.IDS_ArgumentException,
                        "graphics"));

            m_graphics = graphics;
            m_textRenderingHint = m_graphics.TextRenderingHint;
            m_graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        }

        /// <summary>
        ///     destructor of the UseClearTypeGridFit class.
        /// </summary>
        ~UseClearTypeGridFit()
        {
            Dispose(false);
        }

        /// <summary>
        ///     Releases all resources used by the class.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}