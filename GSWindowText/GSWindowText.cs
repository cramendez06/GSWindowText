using Microsoft.Win32;
using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace GSWindowText
{
    public partial class GSWindowText : Form
    {
        ///----------------------------------------------------------------------/
        ///
        /// System name：GSWindowText
        /// Program title：GSWindowText
        /// Overview：Main Program for Get/Set Window Text
        ///
        ///      Author： M.Mendez    CREATE 2017/12/01          【P-10000】
        ///      
        ///     Copyright (C)
        ///----------------------------------------------------------------------/
        //=======================================================================
        //                        Declaration of Variables
        //=======================================================================
        public IniFile m_iniFile;
        public string m_iniPath;
        public IniFile.IniSection m_sSection;

        public static string m_ToLang;
        public static string m_FromLang;

        public static bool m_ATFLG;

        public static IntPtr m_mainHandle;

        /// <summary>
        /// Main Program Constructor
        /// </summary>
        /// <param name="control">Main Functions as UserControl</param>
        public GSWindowText(UserControl control)
        {
            InitializeComponent();

            m_iniPath = Application.StartupPath + "\\GSWTSettings.ini";

            //Checks if the cursor doesn't exist
            if (!File.Exists(m_iniPath))
            {
                //Create .ini file
                CreateIniFile();
                //Set the cursor file as hidden
                File.SetAttributes(m_iniPath, File.GetAttributes(m_iniPath) | FileAttributes.Hidden);
            }

            //Initialize .ini file settings
            InitializeIniFile();

            //Load all languages from .ini file
            LoadLanguages();

            //Add UserControl to the Main Panel
            this.pnlMain.Controls.Add(control);

            //Auto-translate boolean flag to false
            m_ATFLG = false;

            //Set Main handle as static variable
            m_mainHandle = this.Handle;
        }

        /// <summary>
        /// Initialization of .ini file settings
        /// </summary>
        public void InitializeIniFile()
        {
            //Initialization of .ini file
            m_iniFile = new IniFile();
            m_iniFile.Load(m_iniPath);
            m_sSection = m_iniFile.GetSection("Settings");

            //Get settings from .ini file to form
            TrayWhenClose.Checked = bool.Parse(m_iniFile.GetKeyValue("Settings", "TrayWhenClose"));
            TrayWhenMin.Checked = bool.Parse(m_iniFile.GetKeyValue("Settings", "TrayWhenMin"));
            StartupToolStripMenuItem.Checked = bool.Parse(m_iniFile.GetKeyValue("Settings", "OnStartUp"));
            alwaysOnTopToolStripMenuItem.Checked = bool.Parse(m_iniFile.GetKeyValue("Settings", "OnTop"));

            this.TopMost = alwaysOnTopToolStripMenuItem.Checked;
        }

        /// <summary>
        /// Creates .ini file when it is non-existent for program portability
        /// </summary>
        public void CreateIniFile()
        {
            m_iniFile = new IniFile(m_iniPath);

            m_iniFile.AddSection("Settings").AddKey("OnStartUp").Value = "False";
            m_iniFile.AddSection("Settings").AddKey("ToLanguage").Value = "English";
            m_iniFile.AddSection("Settings").AddKey("TrayWhenMin").Value = "False";
            m_iniFile.AddSection("Settings").AddKey("OnTop").Value = "False";
            m_iniFile.AddSection("Settings").AddKey("TrayWhenClose").Value = "False";
            m_iniFile.AddSection("Settings").AddKey("FromLanguage").Value = "Japanese";

            m_iniFile.AddSection("Language").AddKey("Find Language").Value = "auto";
            m_iniFile.AddSection("Language").AddKey("African").Value = "af";
            m_iniFile.AddSection("Language").AddKey("Albanian").Value = "sq";
            m_iniFile.AddSection("Language").AddKey("Amharic").Value = "am";
            m_iniFile.AddSection("Language").AddKey("Arabic").Value = "ar";
            m_iniFile.AddSection("Language").AddKey("Armenian").Value = "hy";
            m_iniFile.AddSection("Language").AddKey("Azerbaijani").Value = "az";
            m_iniFile.AddSection("Language").AddKey("Basque").Value = "eu";
            m_iniFile.AddSection("Language").AddKey("Belarusian").Value = "be";
            m_iniFile.AddSection("Language").AddKey("Bengali").Value = "bn";
            m_iniFile.AddSection("Language").AddKey("Bosnia").Value = "bs";
            m_iniFile.AddSection("Language").AddKey("Bulgarian").Value = "bg";
            m_iniFile.AddSection("Language").AddKey("Burmese").Value = "my";
            m_iniFile.AddSection("Language").AddKey("Catalan").Value = "ca";
            m_iniFile.AddSection("Language").AddKey("Cebuano").Value = "ceb";
            m_iniFile.AddSection("Language").AddKey("Chichewa").Value = "ny";
            m_iniFile.AddSection("Language").AddKey("Chinese (Simple)").Value = "zh-CN";
            m_iniFile.AddSection("Language").AddKey("Chinese (Traditional)").Value = "zh-TW";
            m_iniFile.AddSection("Language").AddKey("Corsican").Value = "co";
            m_iniFile.AddSection("Language").AddKey("Croatian").Value = "hr";
            m_iniFile.AddSection("Language").AddKey("Czech").Value = "cs";
            m_iniFile.AddSection("Language").AddKey("Danish").Value = "da";
            m_iniFile.AddSection("Language").AddKey("Dutch").Value = "nl";
            m_iniFile.AddSection("Language").AddKey("English").Value = "en";
            m_iniFile.AddSection("Language").AddKey("Esperanto").Value = "eo";
            m_iniFile.AddSection("Language").AddKey("Estonian").Value = "et";
            m_iniFile.AddSection("Language").AddKey("Filipino").Value = "tl";
            m_iniFile.AddSection("Language").AddKey("Finnish").Value = "fi";
            m_iniFile.AddSection("Language").AddKey("French").Value = "fr";
            m_iniFile.AddSection("Language").AddKey("Frisian").Value = "fy";
            m_iniFile.AddSection("Language").AddKey("Galician").Value = "gl";
            m_iniFile.AddSection("Language").AddKey("Georgian").Value = "ka";
            m_iniFile.AddSection("Language").AddKey("German").Value = "de";
            m_iniFile.AddSection("Language").AddKey("Greek").Value = "el";
            m_iniFile.AddSection("Language").AddKey("Gujarati").Value = "gu";
            m_iniFile.AddSection("Language").AddKey("Haitian Creole").Value = "ht";
            m_iniFile.AddSection("Language").AddKey("Hausa").Value = "ha";
            m_iniFile.AddSection("Language").AddKey("Hawaiian").Value = "haw";
            m_iniFile.AddSection("Language").AddKey("Hebrew").Value = "iw";
            m_iniFile.AddSection("Language").AddKey("Hindi").Value = "hi";
            m_iniFile.AddSection("Language").AddKey("Hmong").Value = "hmn";
            m_iniFile.AddSection("Language").AddKey("Hungarian").Value = "hu";
            m_iniFile.AddSection("Language").AddKey("Icelandic").Value = "is";
            m_iniFile.AddSection("Language").AddKey("Igbo").Value = "ig";
            m_iniFile.AddSection("Language").AddKey("Indonesian").Value = "id";
            m_iniFile.AddSection("Language").AddKey("Irish").Value = "ga";
            m_iniFile.AddSection("Language").AddKey("Italian").Value = "it";
            m_iniFile.AddSection("Language").AddKey("Japanese").Value = "ja";
            m_iniFile.AddSection("Language").AddKey("Javanese").Value = "jw";
            m_iniFile.AddSection("Language").AddKey("Kannada").Value = "kn";
            m_iniFile.AddSection("Language").AddKey("Kazakh").Value = "kk";
            m_iniFile.AddSection("Language").AddKey("Khmer").Value = "km";
            m_iniFile.AddSection("Language").AddKey("Korean").Value = "ko";
            m_iniFile.AddSection("Language").AddKey("Kurdish").Value = "ku";
            m_iniFile.AddSection("Language").AddKey("Kyrgyz").Value = "ky";
            m_iniFile.AddSection("Language").AddKey("Lao").Value = "lo";
            m_iniFile.AddSection("Language").AddKey("Latin").Value = "la";
            m_iniFile.AddSection("Language").AddKey("Latvian").Value = "lv";
            m_iniFile.AddSection("Language").AddKey("Lithuanian").Value = "lt";
            m_iniFile.AddSection("Language").AddKey("Luxembourgish").Value = "lb";
            m_iniFile.AddSection("Language").AddKey("Macedonian").Value = "mk";
            m_iniFile.AddSection("Language").AddKey("Malagasy").Value = "mg";
            m_iniFile.AddSection("Language").AddKey("Malay").Value = "ms";
            m_iniFile.AddSection("Language").AddKey("Malayalam").Value = "ml";
            m_iniFile.AddSection("Language").AddKey("Maltese").Value = "mt";
            m_iniFile.AddSection("Language").AddKey("Maori").Value = "mi";
            m_iniFile.AddSection("Language").AddKey("Marathi").Value = "mr";
            m_iniFile.AddSection("Language").AddKey("Mongolian").Value = "mn";
            m_iniFile.AddSection("Language").AddKey("Nepali").Value = "ne";
            m_iniFile.AddSection("Language").AddKey("Norwegian").Value = "no";
            m_iniFile.AddSection("Language").AddKey("Pashto").Value = "ps";
            m_iniFile.AddSection("Language").AddKey("Persian").Value = "fa";
            m_iniFile.AddSection("Language").AddKey("Polish").Value = "pl";
            m_iniFile.AddSection("Language").AddKey("Portugese").Value = "pt";
            m_iniFile.AddSection("Language").AddKey("Punjabi").Value = "pa";
            m_iniFile.AddSection("Language").AddKey("Romanian").Value = "ro";
            m_iniFile.AddSection("Language").AddKey("Russian").Value = "ru";
            m_iniFile.AddSection("Language").AddKey("Samoan").Value = "sm";
            m_iniFile.AddSection("Language").AddKey("Scots Gaelic ").Value = "gd";
            m_iniFile.AddSection("Language").AddKey("Serbian").Value = "sr";
            m_iniFile.AddSection("Language").AddKey("Sesotho").Value = "st";
            m_iniFile.AddSection("Language").AddKey("Shona").Value = "sn";
            m_iniFile.AddSection("Language").AddKey("Sindhi").Value = "sd";
            m_iniFile.AddSection("Language").AddKey("Sinhala").Value = "si";
            m_iniFile.AddSection("Language").AddKey("Slovak").Value = "sk";
            m_iniFile.AddSection("Language").AddKey("Slovenian").Value = "sl";
            m_iniFile.AddSection("Language").AddKey("Somali").Value = "so";
            m_iniFile.AddSection("Language").AddKey("Spanish").Value = "es";
            m_iniFile.AddSection("Language").AddKey("Sundanese").Value = "su";
            m_iniFile.AddSection("Language").AddKey("Swahili").Value = "sw";
            m_iniFile.AddSection("Language").AddKey("Swedish").Value = "sv";
            m_iniFile.AddSection("Language").AddKey("Tajik").Value = "tg";
            m_iniFile.AddSection("Language").AddKey("Tamil").Value = "ta";
            m_iniFile.AddSection("Language").AddKey("Telugu").Value = "te";
            m_iniFile.AddSection("Language").AddKey("Thai").Value = "th";
            m_iniFile.AddSection("Language").AddKey("Turkish").Value = "tr";
            m_iniFile.AddSection("Language").AddKey("Ukranian").Value = "uk";
            m_iniFile.AddSection("Language").AddKey("Urdu").Value = "ur";
            m_iniFile.AddSection("Language").AddKey("Uzbek").Value = "uz";
            m_iniFile.AddSection("Language").AddKey("Vietnamese").Value = "vi";
            m_iniFile.AddSection("Language").AddKey("Welsh").Value = "cy";
            m_iniFile.AddSection("Language").AddKey("Xhosa").Value = "xh";
            m_iniFile.AddSection("Language").AddKey("Yiddish").Value = "yi";
            m_iniFile.AddSection("Language").AddKey("Yoruba").Value = "yo";
            m_iniFile.AddSection("Language").AddKey("Zulu").Value = "zu ";

            m_iniFile.Save(m_iniPath);
        }

        /// <summary>
        /// Load all languages in .ini file
        /// </summary>
        private void LoadLanguages()
        {
            //Get all keys regarding the Language section
            string[] w_allkeys = (string[])m_iniFile.GetAllKeys("Language");

            //Sort Array
            Array.Sort(w_allkeys);

            //Add keys to both comboboxes
            for (int i = 0; i < w_allkeys.Length; i++)
            {
                cmbToLang.Items.Add(w_allkeys[i]);
                cmbFromLang.Items.Add(w_allkeys[i]);
            }

            //Set the comboboxes display based on the .ini settings
            cmbToLang.Text = m_iniFile.GetKeyValue("Settings", "ToLanguage");
            cmbFromLang.Text = m_iniFile.GetKeyValue("Settings", "FromLanguage");

            //Get Values of the keys specified
            GetLangKeyVal();
        }

        /// <summary>
        /// Get key value of Language selected
        /// </summary>
        private void GetLangKeyVal()
        {
            //Set the key value of the language to be translated into
            m_ToLang = m_iniFile.GetKeyValue("Language", m_iniFile.GetKeyValue("Settings", "ToLanguage"));
            //Set the key value of the language to be translated from
            m_FromLang = m_iniFile.GetKeyValue("Language", m_iniFile.GetKeyValue("Settings", "FromLanguage"));
        }

        /// <summary>
        /// Combobox To Languge Selected Index Change Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbToLang_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Remove the Key from the section
            m_sSection.RemoveKey("ToLanguage");
            //Add the key again with the new value
            m_sSection.AddKey("ToLanguage").Value = cmbToLang.SelectedItem.ToString();
            //Save settings
            m_iniFile.Save(m_iniPath);

            //Get key values
            GetLangKeyVal();
        }

        /// <summary>
        /// Combobox From Language Selected Index Change Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbFromLang_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Remove the Key from the section
            m_sSection.RemoveKey("FromLanguage");
            //Add the key again with the new value
            m_sSection.AddKey("FromLanguage").Value = cmbFromLang.SelectedItem.ToString();
            //Save settings
            m_iniFile.Save(m_iniPath);

            //Get key values
            GetLangKeyVal();
        }

        /// <summary>
        /// TrayWhenMin MenuItem Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TrayWhenMin_Click(object sender, EventArgs e)
        {
            //Declare flag variable
            bool w_TrayWhenMinFLG;

            //Checks if TrayWhenMin MenuStrip is checked
            if (TrayWhenMin.Checked)
            {
                TrayWhenMin.Checked = false;
                w_TrayWhenMinFLG = false;
            }
            else
            {
                TrayWhenMin.Checked = true;
                w_TrayWhenMinFLG = true;
            }

            //Change TrayWhenMin Settings and saves it
            m_sSection.RemoveKey("TrayWhenMin");
            m_sSection.AddKey("TrayWhenMin").Value = w_TrayWhenMinFLG + "";
            m_iniFile.Save(m_iniPath);
        }

        /// <summary>
        /// Tray When Close MenuItem Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TrayWhenClose_Click(object sender, EventArgs e)
        {
            //Declare flag variable
            bool w_TrayWhenCloseFLG;

            //Checks if TrayWhenClose MenuStrip is checked
            if (TrayWhenClose.Checked)
            {
                TrayWhenClose.Checked = false;
                w_TrayWhenCloseFLG = false;
            }
            else
            {
                TrayWhenClose.Checked = true;
                w_TrayWhenCloseFLG = true;
            }

            //Change TrayWhenClose Settings and saves it
            m_sSection.RemoveKey("TrayWhenClose");
            m_sSection.AddKey("TrayWhenClose").Value = w_TrayWhenCloseFLG + "";
            m_iniFile.Save(m_iniPath);
        }

        /// <summary>
        /// Startup MenuItem Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Declare flag variable
            bool w_onStartUpFLG;

            try
            {
                //Checks if StartupToolStripMenuItem is checked
                if (StartupToolStripMenuItem.Checked)
                {
                    StartupToolStripMenuItem.Checked = false;
                    RegisterInStartup(false);
                    w_onStartUpFLG = false;
                }
                else
                {
                    StartupToolStripMenuItem.Checked = true;
                    RegisterInStartup(true);
                    w_onStartUpFLG = true;
                }

                //Change OnStartUp Settings and saves it
                m_sSection.RemoveKey("OnStartUp");
                m_sSection.AddKey("OnStartUp").Value = w_onStartUpFLG + "";
                m_iniFile.Save(m_iniPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unexpected error occured: " + ex.Message, "GSWindowText", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Sets the Application in startup list
        /// </summary>
        /// <param name="isChecked"></param>
        private void RegisterInStartup(bool isChecked)
        {
            //Sets the Registry key
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey
                    ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            //Checks if to be inserted in startup
            if (isChecked)
            {
                registryKey.SetValue(Application.ProductName, Application.ExecutablePath);
            }
            else
            {
                registryKey.DeleteValue(Application.ProductName);
            }
        }

        /// <summary>
        /// Always on top MenuItem Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void alwaysOnTopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Declare flag variable
            bool w_onTopFLG;

            //Checks if alwaysOnTopToolStripMenuItem is checked
            if (alwaysOnTopToolStripMenuItem.Checked)
            {
                alwaysOnTopToolStripMenuItem.Checked = false;
                w_onTopFLG = false;
                this.TopMost = false;
            }
            else
            {
                alwaysOnTopToolStripMenuItem.Checked = true;
                w_onTopFLG = true;
                this.TopMost = true;
            }

            //Change OnTop Settings and saves it
            m_sSection.RemoveKey("OnTop");
            m_sSection.AddKey("OnTop").Value = w_onTopFLG + "";
            m_iniFile.Save(m_iniPath);
        }

        /// <summary>
        /// Windows Resize Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GSWindowText_SizeChanged(object sender, EventArgs e)
        {
            //Checks if the TrayWhenMin is checked and it is minimized
            if (TrayWhenMin.Checked && this.WindowState == FormWindowState.Minimized)
            {
                //Shows the System tray icon
                notifyIcon.Visible = true;
                notifyIcon.Icon = this.Icon;
                notifyIcon.ShowBalloonTip(0);

                //Hides the windows form
                this.FormBorderStyle = FormBorderStyle.SizableToolWindow;
                this.ShowInTaskbar = false;
                this.Hide();
            }
        }

        /// <summary>
        /// NotifyIconBalloon Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void notifyIcon_BalloonTipClicked(object sender, EventArgs e)
        {
            //Shows the windows form and hides the System Tray Icon
            this.Show();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.ShowInTaskbar = true;
            this.WindowState = FormWindowState.Normal;
            notifyIcon.Visible = false;
        }

        /// <summary>
        /// NotifyIcon Double-click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //Shows the windows form and hides the System Tray Icon
            this.Show();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.ShowInTaskbar = true;
            this.WindowState = FormWindowState.Normal;
            notifyIcon.Visible = false;
        }

        private void GSWindowText_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Checks if the TrayWhenClose is checked
            if (TrayWhenClose.Checked)
            {
                //Cancels the closing of form
                e.Cancel = true;

                //Shows the System tray icon
                notifyIcon.Visible = true;
                notifyIcon.Icon = this.Icon;
                notifyIcon.ShowBalloonTip(0);

                //Hides the windows form
                this.FormBorderStyle = FormBorderStyle.SizableToolWindow;
                this.ShowInTaskbar = false;
                this.Hide();
            }
        }

        /// <summary>
        /// Get All Items MenuItem Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void getAllItemsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Checks if the getAllItemsToolStripMenuItem is checked
            if (getAllItemsToolStripMenuItem.Checked)
            {
                getAllItemsToolStripMenuItem.Checked = false;
                Win32SDK.m_blnCmbAll = false;
            }
            else
            {
                getAllItemsToolStripMenuItem.Checked = true;
                Win32SDK.m_blnCmbAll = true;
            }
        }

        /// <summary>
        /// Copy MenuItem Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Gets the active control
            var w_box = this.ActiveControl as TextBoxBase;
            if (w_box != null) w_box.Copy();
        }

        /// <summary>
        /// Cut MenuItem Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Gets the active control
            var w_box = this.ActiveControl as TextBoxBase;
            if (w_box != null) w_box.Cut();
        }

        /// <summary>
        /// Paste MenuItem Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Gets the active control
            var w_box = this.ActiveControl as TextBoxBase;
            if (w_box != null) w_box.Paste();
        }

        /// <summary>
        /// Clear MenuItem Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Gets the active control
            var w_box = this.ActiveControl as TextBoxBase;
            if (w_box != null) w_box.Clear();
        }

        /// <summary>
        /// Select All MenuItem Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Gets the active control
            var w_box = this.ActiveControl as TextBoxBase;
            if (w_box != null) w_box.SelectAll();
        }

        /// <summary>
        /// Undo MenuItem Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var w_box = this.ActiveControl as TextBoxBase;
            if (w_box != null) w_box.Undo();
        }

        /// <summary>
        /// Exit MenuItem Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Instructions MenuItem Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void instructionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder w_inst = new StringBuilder();

            w_inst.AppendLine("How to use:\n");
            w_inst.AppendLine("Drag the icon using the Left mouse button then drop");
            w_inst.AppendLine("to the desired Window control to get its text.\n");
            w_inst.AppendLine("Drag the icon using the Right mouse button then drop ");
            w_inst.AppendLine("to the desired Window control to get its text with its");
            w_inst.AppendLine("corresponding translation. The window text will also be");
            w_inst.AppendLine("set temporarily with the translated text.\n");
            w_inst.AppendLine("Click the icon with the Middle mouse button to clear");
            w_inst.AppendLine("both textboxes.\n");
            w_inst.AppendLine("Note: Translation will require an Internet connection.");

            MessageBox.Show(w_inst.ToString(), "Instructions", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        /// <summary>
        /// About MenuItem Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder w_inst = new StringBuilder();

            w_inst.AppendLine("Name: GSWindowText");
            w_inst.AppendLine("Version: 1.0");
            w_inst.AppendLine("License: GPL");
            w_inst.AppendLine("Manufactured By: Nicart & Friends Inc.\n");

            w_inst.AppendLine("*DISCLAIMER*");
            w_inst.AppendLine("THIS MATERIAL IS PROVIDED \"AS IS\" WITHOUT WARRANTY");
            w_inst.AppendLine("OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING,");
            w_inst.AppendLine("BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF ");
            w_inst.AppendLine("MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE,");
            w_inst.AppendLine("OR NON - INFRINGEMENT. SOME JURISDICTIONS DO NOT ALLOW");
            w_inst.AppendLine("THE EXCLUSION OF IMPLIED WARRANTIES, SO THE ABOVE");
            w_inst.AppendLine("EXCLUSION MAY NOT APPLY TO YOU. IN NO EVENT WILL I BE");
            w_inst.AppendLine("LIABLE TO ANY PARTY FOR ANY DIRECT, INDIRECT, SPECIAL");
            w_inst.AppendLine("OR OTHER CONSEQUENTIAL DAMAGES FOR ANY USE OF THIS");
            w_inst.AppendLine("MATERIAL INCLUDING, WITHOUT LIMITATION, ANY LOST PROFITS,");
            w_inst.AppendLine("BUSINESS INTERRUPTION, LOSS OF PROGRAMS OR OTHER DATA ON");
            w_inst.AppendLine("YOUR INFORMATION HANDLING SYSTEM OR OTHERWISE, EVEN IF");
            w_inst.AppendLine("WE ARE EXPRESSLY ADVISED OF THE POSSIBILITY OF SUCH DAMAGES.");

            MessageBox.Show(w_inst.ToString(), "About", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }
        
        /// <summary>
        /// Load form Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GSWindowText_Load(object sender, EventArgs e)
        {
            //Checks if opened from startup
            if (StartupToolStripMenuItem.Checked)
            {
                //Resize to minimize
                this.WindowState = FormWindowState.Minimized;
            }
        }
    }
}
