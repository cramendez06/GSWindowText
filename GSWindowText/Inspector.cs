using System;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace GSWindowText
{
    public partial class Inspector : UserControl
    {
        ///----------------------------------------------------------------------/
        ///
        /// System name：GSWindowText
        /// Program title：Inspector UserControl
        /// Overview：Main Control of the System
        ///
        ///      Author： M.Mendez    CREATE 2017/12/01          【P-10000】
        ///      
        ///     Copyright (C)
        ///----------------------------------------------------------------------/
        //=======================================================================
        //                        Declaration of Variables
        //=======================================================================
        private bool m_dragging;
        private IntPtr m_hWndBef;
        private IntPtr m_curCross;
        private Image m_imgAppCross;
        private Image m_imgApp;

        /// <summary>
        /// Main Program Constructor
        /// </summary>
        public Inspector()
        {
            InitializeComponent();
            //Gets image resources
            m_imgAppCross = global::GSWindowText.Properties.Resources.iconSM;
            m_imgApp = global::GSWindowText.Properties.Resources.iconSM_ck;
            //Initialize cursor path
            string w_curPath = Application.StartupPath + "\\grab.cur";

            //Checks if the cursor doesn't exist
            if (!File.Exists(w_curPath)) {
                //Recreate the cursor file from resources
                File.WriteAllBytes(w_curPath, Properties.Resources.grab);
                //Set the cursor file as hidden
                File.SetAttributes(w_curPath, File.GetAttributes(w_curPath) | FileAttributes.Hidden);
            }
            
            //Loads resources to variables
            m_curCross = Win32SDK.LoadCursorFromFile("grab.cur");
            dragPictureBox.Image = m_imgAppCross;
            dragPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        /// <summary>
        /// Picturebox MouseDown Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dragPictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            //Checks if the button used is LMB
            if (e.Button == MouseButtons.Left)
            {
                GSWindowText.m_ATFLG = false;
                m_dragging = true;
                Win32SDK.SetCursor(m_curCross);
                dragPictureBox.Image = m_imgApp;
            }
            //Checks if the button used is RMB
            else if (e.Button == MouseButtons.Right)
            {
                //Set Auto-translate flag to true
                GSWindowText.m_ATFLG = true;
                m_dragging = true;
                Win32SDK.SetCursor(m_curCross);
                dragPictureBox.Image = m_imgApp;
            }
            else if (e.Button == MouseButtons.Middle)
            {
                txtWindowText.Clear();
                tbDetails.Clear();
            }
        }

        /// <summary>
        /// Picturebox MouseUp Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dragPictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            //Checks if the mouse button is still pressed
            if (m_dragging)
            {
                m_dragging = false;
                System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;

                //Checks if the previous handle is empty
                if (m_hWndBef != IntPtr.Zero)
                {
                    Win32SDK.DrawFrame(m_hWndBef, Win32SDK.m_oldDisplay, true);
                    m_hWndBef = IntPtr.Zero;
                    //The image in the dragPictureBox will be restored when the context menu is closed
                }
                else
                {
                    dragPictureBox.Image = m_imgAppCross;
                }
                dragPictureBox.Image = m_imgAppCross;
            }
        }

        /// <summary>
        /// Picturebox MouseMove Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dragPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_dragging)
            {
                IntPtr p_hWnd = Win32SDK.WindowFromPoint(Win32SDK.GetCursorPosition());
                if (p_hWnd == dragPictureBox.Handle || p_hWnd == Handle || p_hWnd == txtWindowText.Handle || p_hWnd == tbDetails.Handle || p_hWnd == GSWindowText.menuStrip1.Handle || p_hWnd == GSWindowText.m_mainHandle)
                {
                    //Drawing a border around itself (where we start
                    //dragging) doesn't look nice, so we ignore this window
                    p_hWnd = IntPtr.Zero;
                }

                //Checks if the current Handle is the same as the previous handle
                if (p_hWnd != m_hWndBef)
                {
                    //Checks if there is a previous handle
                    if (m_hWndBef != null)
                    {
                        //Resets the inverted rectangle and window text
                        Win32SDK.DrawFrame(m_hWndBef, Win32SDK.m_oldDisplay, true);
                    }

                    //Checks if the current Handle is non-existent
                    if (p_hWnd != IntPtr.Zero)
                    {
                        tbDetails.Text = string.Empty;

                        //Checks if the Auto-translate flag is true
                        if (GSWindowText.m_ATFLG)
                        {
                            //Store the Window Text's old and new strings
                            Win32SDK.m_oldDisplay = Win32SDK._GetWindowText(p_hWnd);
                            Win32SDK.m_newDisplay = Win32SDK.AutoTranslate(Win32SDK.m_oldDisplay, GSWindowText.m_ToLang, GSWindowText.m_FromLang);

                            //Displays the texts to the textbox
                            txtWindowText.Text = Win32SDK.m_oldDisplay + Environment.NewLine + Environment.NewLine + Win32SDK.m_newDisplay;
                        }
                        else
                        {
                            //Displays the text to the textbox
                            txtWindowText.Text = Win32SDK._GetWindowText(p_hWnd);
                        }

                        //Displays all the other details gathered
                        tbDetails.Text += "[Handle:" + p_hWnd.ToString() + "]";
                        tbDetails.Text += "[Class:" + Win32SDK.GetClassName(p_hWnd) + "]";
                        try
                        {
                            tbDetails.Text += "[App:" + Win32SDK.GetApplication(p_hWnd) + "]";
                        }
                        catch (Exception)
                        {
                        }
                        
                    }
                    else
                    {
                        //Return empty when handle is empty
                        tbDetails.Text = string.Empty;
                        txtWindowText.Text = string.Empty;
                    }

                    //Draw the inverted frame and set window text
                    Win32SDK.DrawFrame(p_hWnd, Win32SDK.m_newDisplay, false);

                    //Save the current handle as previous handle
                    m_hWndBef = p_hWnd;
                }
            }
        }
    }
}
