using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms.VisualStyles;

namespace BSE.Windows.Forms
{
    internal static class DisplayInformation
    {
        #region Properties

        [field: ThreadStatic] internal static bool IsThemed { get; private set; }

        #endregion

        private static class NativeMethods
        {
            [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
            public static extern int GetCurrentThemeName(StringBuilder pszThemeFileName, int dwMaxNameChars,
                StringBuilder pszColorBuff, int dwMaxColorChars, StringBuilder pszSizeBuff, int cchMaxSizeChars);
        }

        #region FieldsPrivate

        private const string m_strRegExpression = @".*\.msstyles$";

        #endregion

        #region MethodsPrivate

        static DisplayInformation()
        {
            //SystemEvents.UserPreferenceChanged += new UserPreferenceChangedEventHandler(DisplayInformation.OnUserPreferenceChanged);
            SetScheme();
        }

        /*
        private static void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
	        DisplayInformation.SetScheme();
        }
        */
        private static void SetScheme()
        {
            if (VisualStyleRenderer.IsSupported)
            {
                if (!VisualStyleInformation.IsEnabledByUser) return;
                var stringBuilder = new StringBuilder(0x200);
                var iResult =
                    NativeMethods.GetCurrentThemeName(stringBuilder, stringBuilder.Capacity, null, 0, null, 0);
                if (iResult == 0)
                {
                    var regex = new Regex(m_strRegExpression);
                    IsThemed = regex.IsMatch(Path.GetFileName(stringBuilder.ToString()));
                }
            }
        }

        #endregion
    }
}