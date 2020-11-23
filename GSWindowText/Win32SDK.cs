using System;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace GSWindowText
{
    class Win32SDK
    {
        ///----------------------------------------------------------------------/
        ///
        /// System name：GSWindowText
        /// Program title：Win32SDK
        /// Overview：Win32 API Class
        ///
        ///      Author： M.Mendez    CREATE 2017/12/01          【P-10000】
        ///      
        ///     Copyright (C)
        ///----------------------------------------------------------------------/
        //=======================================================================
        //                        Declaration of Variables
        //=======================================================================
        public static string m_oldDisplay;
        public static string m_newDisplay;
        public static int m_cmbCount;
        public static int m_cmbIdx;
        public static bool m_blnCmbAll;

        #region Win32 API Declarations

        #region Helpers
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            #region Helper methods

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }

            public static implicit operator System.Drawing.Point(POINT p)
            {
                return new System.Drawing.Point(p.X, p.Y);
            }

            public static implicit operator POINT(System.Drawing.Point p)
            {
                return new POINT(p.X, p.Y);
            }

            #endregion
        }
        #endregion

        //Declaration of Win32 Constants
        const int DSTINVERT = 0x00550009;
        const uint CB_GETCURSEL = 0x0147;
        const uint CB_SETCURSEL = 0x014E;
        const uint CB_GETLBTEXT = 0x0148;
        const uint WM_SETTEXT = 0x0C;
        const uint CB_INSERTSTRING = 0x014A;
        const uint CB_DELETESTRING = 0x0144;
        const uint CB_GETCOUNT = 0x0146;
        const int LB_GETTEXT = 0x0189;
        const int LB_GETTEXTLEN = 0x018A;
        const int LB_GETCOUNT = 0x018B;

        [DllImport("gdi32.dll")]
        static extern bool PatBlt(IntPtr hdc, int nXLeft, int nYLeft, int nWidth, int nHeight, uint dwRop);

        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr LoadCursorFromFile(string fileName);

        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr SetCursor(IntPtr hCursor);

        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(POINT Point);

        [DllImport("user32.dll")]
        static extern int GetWindowText(int hWnd, StringBuilder text, int count);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern uint GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        static extern bool OffsetRect(ref RECT lprc, int dx, int dy);

        [DllImport("user32.dll")]
        static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll")]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out System.Drawing.Point lpPoint);

        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        [DllImport("user32.dll")]
        static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall)]
        static extern uint SendMessageA(IntPtr hwnd, uint wMsg, uint wParam, string lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false, EntryPoint = "SendMessage", CallingConvention = CallingConvention.StdCall)]
        static extern IntPtr SendRefMessage(IntPtr hWnd, uint Msg, uint wParam, StringBuilder lParam);

        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern bool LockWindowUpdate(IntPtr hWndLock);

        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern bool InvalidateRect(IntPtr hWnd, IntPtr rect, bool clear);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool SetWindowText(IntPtr hwnd, String lpString);

        [DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Auto)]
        static extern int SendMessage3(IntPtr hwndControl, uint Msg, int wParam, StringBuilder strBuffer); // get text

        [DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Auto)]
        static extern int SendMessage4(IntPtr hwndControl, uint Msg, int wParam, int lParam);  // text length

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int RegisterWindowMessage(string lpString);

        [DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Auto)] //
        static extern bool SendMessage(IntPtr hWnd, uint Msg, int wParam, StringBuilder lParam);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SendMessage(int hWnd, int Msg, int wparam, int lparam);

        #endregion

        /// <summary>
        /// Refresh the rectangle area of the Handle
        /// </summary>
        /// <param name="p_hwnd">Handle to be refreshed</param>
        public static void _Refresh(IntPtr p_hwnd)
        {
            LockWindowUpdate(p_hwnd);
            InvalidateRect(p_hwnd, IntPtr.Zero, false);
            LockWindowUpdate(IntPtr.Zero);
        }

        /// <summary>
        /// Convert Pixels to Units 
        /// </summary>
        /// <param name="p_x">X pixels to be converted</param>
        /// <param name="p_y">Y pixels to be converted</param>
        /// <returns>Converted </returns>
        private static Point ConvertPixelsToUnits(double p_x, double p_y)
        {
            //Get the system DPI
            IntPtr w_dDC = GetDC(IntPtr.Zero); //Get desktop DC
            int w_dpi = GetDeviceCaps(w_dDC, 88);
            bool w_rv = ReleaseDC(IntPtr.Zero, w_dDC);

            //WPF's physical unit size is calculated by taking the 
            //"Device-Independant Unit Size" (always 1/96)
            //and scaling it by the system DPI
            double w_physicalUnitSize = (1d / 96d) * (double)w_dpi;
            Point w_wpfUnits = new Point(Convert.ToInt32(w_physicalUnitSize * (p_x)), Convert.ToInt32(w_physicalUnitSize * (p_y)));

            return w_wpfUnits;
        }

        /// <summary>
        /// Get the current Cursor Position in the screen
        /// </summary>
        /// <returns>Cursor location in Point</returns>
        public static POINT GetCursorPosition()
        {
            //Declate new Points
            Point w_lpPoint, w_wpfPoint = new Point();

            //Gets the current cursor position
            if (GetCursorPos(out w_lpPoint))
            {
                //Convert the pixel location to Point
                w_wpfPoint = ConvertPixelsToUnits(w_lpPoint.X, w_lpPoint.Y);
            }

            return w_wpfPoint;
        }

        /// <summary>
        /// Gets the Handle rectangle
        /// </summary>
        /// <param name="p_hwnd">Handle to get the rectangle from</param>
        /// <returns>Rectangle of the Handle</returns>
        public static Rectangle DrawRectangle(IntPtr p_hwnd)
        {
            //
            RECT rectVal = new RECT();
            Rectangle _rect = new Rectangle();

            GetWindowRect(p_hwnd, out rectVal);

            _rect.X = 0;
            _rect.Y = 0;
            _rect.Width = rectVal.Right - rectVal.Left;
            _rect.Height = rectVal.Bottom - rectVal.Top;

            return _rect;
        }

        /// <summary>
        /// Draw frame in the rectangle along with other 
        /// </summary>
        /// <param name="p_hWnd">Handle to draw the Inverted rectangle into</param>
        /// <param name="p_message">Translated or old text to set as window text</param>
        /// <param name="p_toRemove">Boolean flag to check if the draw rectangle is to be removed</param>
        public static void DrawFrame(IntPtr p_hWnd, string p_message, bool p_toRemove)
        {
            //Checks if Handle is non-existent
            if (p_hWnd == IntPtr.Zero)
                return;

            //Checks if Auto Translate is on
            if (GSWindowText.m_ATFLG)
            {
                //Checks if drawing handle
                if (!p_toRemove)
                {
                    //Checks if the handle is a button
                    if (GetClassName(p_hWnd).Contains("BUTTON"))
                    {
                        IntPtr w_hdc1 = GetWindowDC(p_hWnd);

                        //Create graphics and draw over the Handle with the translated counter-part of its window text
                        Graphics w_g = Graphics.FromHdc(w_hdc1);
                        w_g.FillRectangle(new SolidBrush(Color.FromArgb(150, 255, 255, 255)), DrawRectangle(p_hWnd));
                        w_g.DrawString(p_message, new Font("Calibri", 10, FontStyle.Regular), Brushes.Black, 0, 0);
                        w_g.Dispose();

                        ReleaseDC(p_hWnd, w_hdc1);
                    }
                    //Checks if not a button
                    else
                    {
                        //Directly sets the Window text
                        _SetWindowText(p_hWnd, p_message);
                        _Refresh(p_hWnd);
                        //Sleep thread to sync the rectangle drawing after refresh
                        System.Threading.Thread.Sleep(50);
                    }
                }
                //Checks if removing handle
                else
                {
                    //Checks if the Handle is a Button type class
                    if (GetClassName(p_hWnd).Contains("BUTTON"))
                    {
                        //Refreshes the Handle to remove drawn over graphics
                        _Refresh(p_hWnd);
                    }
                    else
                    {
                        //Resets the window text
                        _ResetWindowText(p_hWnd, p_message);
                        _Refresh(p_hWnd);
                    }
                }
            }

            //Create an Inverted Frame
            InvertedFrame(p_hWnd);
        }

        /// <summary>
        /// Create an Inverted Frame
        /// </summary>
        /// <param name="p_hWnd">Handles to be drawn with inverted frame</param>
        public static void InvertedFrame(IntPtr p_hWnd)
        {
            const int w_frameWidth = 3;

            //Get Handle's DC
            IntPtr w_hdc = GetWindowDC(p_hWnd);

            //Gets the rectangle's boundaries
            RECT w_rect;
            GetWindowRect(p_hWnd, out w_rect);
            OffsetRect(ref w_rect, -w_rect.Left, -w_rect.Top);

            //Draw the Inverted Lines
            PatBlt(w_hdc, w_rect.Left, w_rect.Top, w_rect.Right - w_rect.Left, w_frameWidth, DSTINVERT);
            PatBlt(w_hdc, w_rect.Left, w_rect.Bottom - w_frameWidth, w_frameWidth, -(w_rect.Bottom - w_rect.Top - 2 * w_frameWidth), DSTINVERT);
            PatBlt(w_hdc, w_rect.Right - w_frameWidth, w_rect.Top + w_frameWidth, w_frameWidth, w_rect.Bottom - w_rect.Top - 2 * w_frameWidth, DSTINVERT);
            PatBlt(w_hdc, w_rect.Right, w_rect.Bottom - w_frameWidth, -(w_rect.Right - w_rect.Left), w_frameWidth, DSTINVERT);

            //Release Handle's DC
            ReleaseDC(p_hWnd, w_hdc);
        }

        /// <summary>
        /// Main GetWindowText method
        /// </summary>
        /// <param name="p_hWnd">Handle to get the text from</param>
        /// <returns>Window Text in string</returns>
        public static string _GetWindowText(IntPtr p_hWnd)
        {
            int w_maxlength = ((int)GetWindowTextLength(p_hWnd) * 2) + 2;
            StringBuilder w_text = new StringBuilder(w_maxlength);
            string w_contType = GetClassName(p_hWnd);

            //Checks if Edit Control type
            if (w_contType.Contains("EDIT"))
            {
                return GetTextBoxText(p_hWnd);
            }
            //Checks if Combobox Control type
            else if (w_contType.Contains("COMBOBOX"))
            {
                //Checks if all items will be retrieved
                if (m_blnCmbAll)
                {
                    return GetAllItemsComboBox(p_hWnd);
                }
                else
                {
                    return GetComboBoxListItems(p_hWnd);
                }

            }
            //Checks if ListBox Control type
            else if (w_contType.Contains("LISTBOX"))
            {
                return GetListBoxContents(p_hWnd);
            }
            //Checks if ListView Control type
            else if (w_contType.Contains("SysListView32"))
            {
                return GetListTreeViews.GetListViewItems(p_hWnd);
            }
            //Checks if TreeView Control type
            else if (w_contType.Contains("SysTreeView32"))
            {
                return GetListTreeViews.GetTreeViewItems(p_hWnd);
            }
            //Checks if there is a text that can be extracted
            else if (GetWindowText(p_hWnd.ToInt32(), w_text, w_maxlength) > 0)
            {
                return w_text.ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Get the length of the string in an Edit Control
        /// </summary>
        /// <param name="p_hTextBox">Handle of to extract the text length from</param>
        /// <returns>Edit Control Text length in int</returns>
        static int GetTextBoxTextLength(IntPtr p_hTextBox)
        {
            //Helper for GetTextBoxText
            uint WM_GETTEXTLENGTH = 0x000E;
            int w_result = SendMessage4(p_hTextBox, WM_GETTEXTLENGTH,
              0, 0);
            return w_result;
        }

        /// <summary>
        /// Gets the Text in an Edit Control
        /// </summary>
        /// <param name="p_hTextBox">Handle of to extract the text from</param>
        /// <returns>Edit Control text in string</returns>
        static string GetTextBoxText(IntPtr p_hTextBox)
        {
            uint WM_GETTEXT = 0x000D;
            int w_len = GetTextBoxTextLength(p_hTextBox);

            if (w_len <= 0) return null;  //Checks if no text
            StringBuilder w_sb = new StringBuilder(w_len + 1);
            SendMessage3(p_hTextBox, WM_GETTEXT, w_len + 1, w_sb);
            return w_sb.ToString();
        }

        /// <summary>
        /// Extract the currently selected item in a combobox
        /// </summary>
        /// <param name="p_hWnd">Handle of to extract the text from</param>
        /// <returns>Combobox text in string</returns>
        public static string GetComboBoxListItems(IntPtr p_hWnd)
        {
            //Saves the current selected item's index in the combobox to a global variable
            m_cmbIdx = (int)SendMessageA(p_hWnd, CB_GETCURSEL, 0, "0");

            //Retrieve text
            StringBuilder w_ssb = new StringBuilder(256, 256);
            SendRefMessage(p_hWnd, CB_GETLBTEXT, (uint)m_cmbIdx, w_ssb);
            return w_ssb.ToString();
        }

        /// <summary>
        /// Extracts all items inside a combobox
        /// </summary>
        /// <param name="p_hWnd">Handle of to extract the list from</param>
        /// <returns>Combobox list in string</returns>
        public static string GetAllItemsComboBox(IntPtr p_hWnd)
        {
            //Save
            int w_cmbCount = (int)SendMessageA(p_hWnd, CB_GETCOUNT, 0, "0");
            StringBuilder outPut = new StringBuilder();

            for (int i = 0; i <= w_cmbCount - 1; i++)
            {
                StringBuilder ssb = new StringBuilder(256, 256);
                SendRefMessage(p_hWnd, CB_GETLBTEXT, (uint)i, ssb);
                outPut.AppendLine(ssb.ToString());
            }

            return outPut.ToString();
        }


        /// <summary>
        /// Extracts the ListBox contents
        /// </summary>
        /// <param name="p_hwnd">Handle of to extract the list from</param>
        /// <returns>List box contents in string</returns>
        public static string GetListBoxContents(IntPtr p_hwnd)
        {
            IntPtr hWnd = new IntPtr(0x1F04FC);
            StringBuilder title = new StringBuilder();

            // Get the size of the string required to hold the window title. 
            Int32 size = SendMessage((int)hWnd, LB_GETTEXTLEN, 0, 0).ToInt32();

            // If the return is 0, there is no title. 
            if (size > 0)
            {
                title = new StringBuilder(size + 1);

                SendMessage(hWnd, (int)LB_GETTEXT, 1, title);
            }

            return title.ToString();
        }

        /// <summary>
        /// Gets the Class name of the Handle
        /// </summary>
        /// <param name="p_hWnd">Handle to get the Class name from</param>
        /// <returns>Class name of handle in string</returns>
        public static string GetClassName(IntPtr p_hWnd)
        {
            StringBuilder w_className = new StringBuilder(100);
            if (GetClassName(p_hWnd, w_className, w_className.Capacity) > 0)
            {
                return w_className.ToString();
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the Application name where the handle is from
        /// </summary>
        /// <param name="p_hWnd">Handle to get the Application name from</param>
        /// <returns>Application name in string</returns>
        public static string GetApplication(IntPtr p_hWnd)
        {
            int w_procId;
            GetWindowThreadProcessId(p_hWnd, out w_procId);
            Process w_proc = Process.GetProcessById(w_procId);
            return w_proc.MainModule.ModuleName;
        }

        /// <summary>
        /// Sets the Window text to a Translated text directly to the handle
        /// </summary>
        /// <param name="p_hwnd">Handle to Set the window text</param>
        /// <param name="p_translatedMsg">Translated counter-part of the string</param>
        public static void _SetWindowText(IntPtr p_hwnd, string p_translatedMsg)
        {
            try
            {
                //Set combo box display
                m_cmbCount = (int)SendMessageA(p_hwnd, CB_GETCOUNT, 0, "0");

                SendMessageA(p_hwnd, CB_INSERTSTRING, (uint)m_cmbCount, p_translatedMsg);
                SendMessageA(p_hwnd, CB_SETCURSEL, (uint)m_cmbCount, "0");
            }
            catch (Exception)
            {
                //Do nothing
            }

            try
            {
                //Checks if the original window text variable is empty
                if (!string.IsNullOrEmpty(m_oldDisplay))
                {
                    //Set other window text
                    SetWindowText(p_hwnd, p_translatedMsg);
                }
            }
            catch (Exception)
            {
                //Do nothing
            }
        }

        /// <summary>
        /// Resets the Window text to the default
        /// </summary>
        /// <param name="p_hwndBef">Handle to reset the window text</param>
        /// <param name="p_oldMsg">Default text of the handle</param>
        public static void _ResetWindowText(IntPtr p_hwndBef, string p_oldMsg)
        {
            try
            {
                //Reset combo box display
                SendMessageA(p_hwndBef, CB_DELETESTRING, (uint)m_cmbCount, "0");
                SendMessageA(p_hwndBef, CB_SETCURSEL, (uint)m_cmbIdx, "0");
            }
            catch (Exception)
            {
                //Do nothing
            }

            try
            {
                //Resets other window text
                SetWindowText(p_hwndBef, p_oldMsg);
            }
            catch (Exception)
            {
                //Do nothing
            }
        }

        /// <summary>
        /// Main Language Translator Method
        /// </summary>
        /// <param name="p_getText">Non-Translated words to be translated</param>
        /// <param name="p_toLang">Language to be translated to</param>
        /// <param name="p_fromLang">Language to be translated from</param>
        /// <returns></returns>
        public static string AutoTranslate(string p_getText, string p_toLang, string p_fromLang)
        {
            //Initialize variables
            WebClient w_WebClient;
            string w_strEncodedText;
            string w_strFromLang = p_fromLang;
            string w_strToLang = p_toLang;
            string w_strUrl;
            string w_strHtmlRequest;
            string w_strTranslated = "<ERROR>";
            Int64 number;

            try
            {
                //Check if the value is an integer
                if (Int64.TryParse(p_getText.Normalize(NormalizationForm.FormKC), out number))
                {
                    w_strTranslated = number.ToString();
                }
                else
                {
                    //Create Web Client
                    w_WebClient = new WebClient();

                    w_WebClient.Proxy = null;
                    WebRequest.DefaultWebProxy = null;

                    w_WebClient.Encoding = Encoding.UTF8;

                    //Add headers
                    w_WebClient.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0");
                    w_WebClient.Headers.Add(HttpRequestHeader.AcceptCharset, "UTF-8");

                    //Create encoded counter-part of the text
                    w_strEncodedText = Uri.EscapeDataString(p_getText);

                    //Create URL
                    w_strUrl = string.Format("https://translate.googleapis.com/translate_a/single?client=gtx&sl={0}&tl={1}&dt=t&q={2}",
                                             w_strFromLang, w_strToLang, w_strEncodedText);

                    //Get the HTML Request
                    w_strHtmlRequest = w_WebClient.DownloadString(w_strUrl);

                    //Parse the result from the request
                    w_strTranslated = ResultParse(w_strHtmlRequest);
                }
            }
            catch (Exception ex)
            {
                //Do nothing
            }

            return System.Text.RegularExpressions.Regex.Unescape(w_strTranslated);
        }

        /// <summary>
        /// Main GET URL Parsing method
        /// </summary>
        /// <param name="p_result">HTML URL Request</param>
        /// <returns></returns>
        private static string ResultParse(string p_result)
        {
            //Initialize variables
            string w_functionReturnValue = null;
            Match w_match;
            string w_result;
            string[] w_splitted;

            try
            {
                //*** String manipulation
                do
                {
                    //Initialization
                    w_functionReturnValue = "";

                    //Extract the text using Regex
                    w_match = Regex.Match(p_result, "\\[(\".*\\d)\\]", RegexOptions.IgnoreCase);
                    w_result = w_match.Value;
                    w_splitted = w_result.Split(new string[] { "],[" }, StringSplitOptions.None);

                    foreach (string str in w_splitted)
                    {
                        string[] strsplit = str.Split(new string[] { "\"" }, StringSplitOptions.None);
                        for (int i = 1; i < strsplit.Length; i++)
                        {
                            w_functionReturnValue += strsplit[i];

                            if (!w_functionReturnValue.EndsWith("\\"))
                            {
                                break;
                            }
                            else
                            {
                                w_functionReturnValue = w_functionReturnValue.Remove(w_functionReturnValue.Length - 1, 1) + "\"";
                            }
                        }
                    }

                    w_functionReturnValue = w_functionReturnValue.Replace("\\n", "\n");
                    break;

                } while (true);

            }
            catch (Exception ex)
            {
                throw;
            }

            return w_functionReturnValue;
        }
    }
}
