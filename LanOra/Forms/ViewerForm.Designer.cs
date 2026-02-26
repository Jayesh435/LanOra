using LanOra.Theme;

namespace LanOra.Forms
{
    partial class ViewerForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.pnlTitleBar    = new System.Windows.Forms.Panel();
            this.lblAppName     = new System.Windows.Forms.Label();
            this.btnClose       = new System.Windows.Forms.Button();
            this.btnMinimize    = new System.Windows.Forms.Button();
            this.pnlLeft        = new System.Windows.Forms.Panel();
            this.lblHostsCap    = new System.Windows.Forms.Label();
            this.lstHosts       = new System.Windows.Forms.ListBox();
            this.btnRefresh     = new System.Windows.Forms.Button();
            this.pnlConnectSect = new System.Windows.Forms.Panel();
            this.lblConnectHdr  = new System.Windows.Forms.Label();
            this.lblPinCaption  = new System.Windows.Forms.Label();
            this.pnlPinBoxes    = new System.Windows.Forms.Panel();
            this.txtPin1        = new System.Windows.Forms.TextBox();
            this.txtPin2        = new System.Windows.Forms.TextBox();
            this.txtPin3        = new System.Windows.Forms.TextBox();
            this.txtPin4        = new System.Windows.Forms.TextBox();
            this.txtPin5        = new System.Windows.Forms.TextBox();
            this.txtPin6        = new System.Windows.Forms.TextBox();
            this.btnConnect     = new System.Windows.Forms.Button();
            this.btnDisconnect  = new System.Windows.Forms.Button();
            this.picScreen      = new System.Windows.Forms.PictureBox();
            this.lblPerfOverlay = new System.Windows.Forms.Label();
            this.pnlStatusBar   = new System.Windows.Forms.Panel();
            this.lblStatusDot   = new System.Windows.Forms.Label();
            this.lblStatusBar   = new System.Windows.Forms.Label();

            this.pnlTitleBar.SuspendLayout();
            this.pnlLeft.SuspendLayout();
            this.pnlConnectSect.SuspendLayout();
            this.pnlPinBoxes.SuspendLayout();
            this.pnlStatusBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.picScreen).BeginInit();
            this.SuspendLayout();

            // ----------------------------------------------------------------
            // pnlTitleBar
            // ----------------------------------------------------------------
            this.pnlTitleBar.BackColor = AppTheme.TitleBar;
            this.pnlTitleBar.Dock      = System.Windows.Forms.DockStyle.Top;
            this.pnlTitleBar.Height    = 44;
            this.pnlTitleBar.Controls.Add(this.btnClose);
            this.pnlTitleBar.Controls.Add(this.btnMinimize);
            this.pnlTitleBar.Controls.Add(this.lblAppName);
            this.pnlTitleBar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TitleBar_MouseDown);
            this.pnlTitleBar.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TitleBar_MouseMove);

            this.lblAppName.AutoSize  = false;
            this.lblAppName.Font      = new System.Drawing.Font(AppTheme.FontFamily, 12F, System.Drawing.FontStyle.Bold);
            this.lblAppName.ForeColor = AppTheme.TextPrimary;
            this.lblAppName.Location  = new System.Drawing.Point(12, 0);
            this.lblAppName.Size      = new System.Drawing.Size(300, 44);
            this.lblAppName.Text      = "LanOra  \u2013  Viewer Mode";
            this.lblAppName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblAppName.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TitleBar_MouseDown);
            this.lblAppName.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TitleBar_MouseMove);

            this.btnClose.BackColor               = AppTheme.TitleBar;
            this.btnClose.FlatStyle               = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatAppearance.MouseOverBackColor = AppTheme.ErrorRed;
            this.btnClose.Font      = new System.Drawing.Font(AppTheme.FontFamily, 11F);
            this.btnClose.ForeColor = AppTheme.TextSecondary;
            this.btnClose.Location  = new System.Drawing.Point(1014, 0);
            this.btnClose.Size      = new System.Drawing.Size(46, 44);
            this.btnClose.Text      = "✕";
            this.btnClose.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.btnClose.Click    += new System.EventHandler(this.btnClose_Click);

            this.btnMinimize.BackColor               = AppTheme.TitleBar;
            this.btnMinimize.FlatStyle               = System.Windows.Forms.FlatStyle.Flat;
            this.btnMinimize.FlatAppearance.BorderSize = 0;
            this.btnMinimize.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(0x40, 0x40, 0x40);
            this.btnMinimize.Font      = new System.Drawing.Font(AppTheme.FontFamily, 11F);
            this.btnMinimize.ForeColor = AppTheme.TextSecondary;
            this.btnMinimize.Location  = new System.Drawing.Point(968, 0);
            this.btnMinimize.Size      = new System.Drawing.Size(46, 44);
            this.btnMinimize.Text      = "─";
            this.btnMinimize.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.btnMinimize.Click    += new System.EventHandler(this.btnMinimize_Click);

            // ----------------------------------------------------------------
            // pnlStatusBar
            // ----------------------------------------------------------------
            this.pnlStatusBar.BackColor = AppTheme.StatusBar;
            this.pnlStatusBar.Dock      = System.Windows.Forms.DockStyle.Bottom;
            this.pnlStatusBar.Height    = 26;
            this.pnlStatusBar.Controls.Add(this.lblStatusBar);
            this.pnlStatusBar.Controls.Add(this.lblStatusDot);

            this.lblStatusDot.AutoSize  = false;
            this.lblStatusDot.Font      = new System.Drawing.Font(AppTheme.FontFamily, 11F, System.Drawing.FontStyle.Bold);
            this.lblStatusDot.ForeColor = AppTheme.ErrorRed;
            this.lblStatusDot.Location  = new System.Drawing.Point(6, 4);
            this.lblStatusDot.Size      = new System.Drawing.Size(20, 18);
            this.lblStatusDot.Text      = "\u25CF";

            this.lblStatusBar.AutoSize  = false;
            this.lblStatusBar.Dock      = System.Windows.Forms.DockStyle.Fill;
            this.lblStatusBar.Font      = new System.Drawing.Font(AppTheme.FontFamily, 8F);
            this.lblStatusBar.ForeColor = AppTheme.TextPrimary;
            this.lblStatusBar.Padding   = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.lblStatusBar.Text      = "Viewer Mode: Discovery  |  " + AppTheme.Developer;
            this.lblStatusBar.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // ----------------------------------------------------------------
            // pnlLeft – sidebar
            // ----------------------------------------------------------------
            this.pnlLeft.BackColor = AppTheme.Panel;
            this.pnlLeft.Dock      = System.Windows.Forms.DockStyle.Left;
            this.pnlLeft.Width     = 270;
            this.pnlLeft.Controls.Add(this.pnlConnectSect);
            this.pnlLeft.Controls.Add(this.btnRefresh);
            this.pnlLeft.Controls.Add(this.lstHosts);
            this.pnlLeft.Controls.Add(this.lblHostsCap);

            // ---- lblHostsCap ----
            this.lblHostsCap.AutoSize  = false;
            this.lblHostsCap.Font      = new System.Drawing.Font(AppTheme.FontFamily, 9F, System.Drawing.FontStyle.Bold);
            this.lblHostsCap.ForeColor = AppTheme.AccentBlue;
            this.lblHostsCap.Location  = new System.Drawing.Point(12, 12);
            this.lblHostsCap.Size      = new System.Drawing.Size(246, 20);
            this.lblHostsCap.Text      = "DISCOVERED HOSTS (UDP)";

            // ---- lstHosts ----
            this.lstHosts.BackColor   = AppTheme.InputBackground;
            this.lstHosts.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstHosts.Font        = new System.Drawing.Font(AppTheme.FontFamily, 9.5F);
            this.lstHosts.ForeColor   = AppTheme.TextPrimary;
            this.lstHosts.Location    = new System.Drawing.Point(12, 36);
            this.lstHosts.Size        = new System.Drawing.Size(246, 158);
            this.lstHosts.SelectionMode = System.Windows.Forms.SelectionMode.One;
            this.lstHosts.SelectedIndexChanged += new System.EventHandler(this.lstHosts_SelectedIndexChanged);

            // ---- btnRefresh ----
            this.btnRefresh.BackColor               = AppTheme.InputBackground;
            this.btnRefresh.FlatStyle               = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.FlatAppearance.BorderColor = AppTheme.CardBorder;
            this.btnRefresh.FlatAppearance.BorderSize  = 1;
            this.btnRefresh.Font      = new System.Drawing.Font(AppTheme.FontFamily, 8.5F);
            this.btnRefresh.ForeColor = AppTheme.TextSecondary;
            this.btnRefresh.Location  = new System.Drawing.Point(12, 200);
            this.btnRefresh.Size      = new System.Drawing.Size(246, 26);
            this.btnRefresh.Text      = "\u21BB  Refresh List";
            this.btnRefresh.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.btnRefresh.Click    += new System.EventHandler(this.btnRefresh_Click);

            // ---- pnlConnectSect ----
            this.pnlConnectSect.BackColor = AppTheme.Panel;
            this.pnlConnectSect.Location  = new System.Drawing.Point(0, 236);
            this.pnlConnectSect.Size      = new System.Drawing.Size(270, 310);
            this.pnlConnectSect.Controls.Add(this.btnDisconnect);
            this.pnlConnectSect.Controls.Add(this.btnConnect);
            this.pnlConnectSect.Controls.Add(this.pnlPinBoxes);
            this.pnlConnectSect.Controls.Add(this.lblPinCaption);
            this.pnlConnectSect.Controls.Add(this.lblConnectHdr);

            // lblConnectHdr
            this.lblConnectHdr.AutoSize  = false;
            this.lblConnectHdr.Font      = new System.Drawing.Font(AppTheme.FontFamily, 9F, System.Drawing.FontStyle.Bold);
            this.lblConnectHdr.ForeColor = AppTheme.AccentBlue;
            this.lblConnectHdr.Location  = new System.Drawing.Point(12, 12);
            this.lblConnectHdr.Size      = new System.Drawing.Size(246, 20);
            this.lblConnectHdr.Text      = "CONNECT TO SELECTED HOST";

            // lblPinCaption
            this.lblPinCaption.AutoSize  = false;
            this.lblPinCaption.Font      = new System.Drawing.Font(AppTheme.FontFamily, 9F);
            this.lblPinCaption.ForeColor = AppTheme.TextSecondary;
            this.lblPinCaption.Location  = new System.Drawing.Point(12, 38);
            this.lblPinCaption.Size      = new System.Drawing.Size(246, 20);
            this.lblPinCaption.Text      = "Enter PIN:";

            // ---- pnlPinBoxes – six individual digit inputs ----
            this.pnlPinBoxes.BackColor = AppTheme.Panel;
            this.pnlPinBoxes.Location  = new System.Drawing.Point(12, 62);
            this.pnlPinBoxes.Size      = new System.Drawing.Size(246, 56);
            this.pnlPinBoxes.Controls.Add(this.txtPin1);
            this.pnlPinBoxes.Controls.Add(this.txtPin2);
            this.pnlPinBoxes.Controls.Add(this.txtPin3);
            this.pnlPinBoxes.Controls.Add(this.txtPin4);
            this.pnlPinBoxes.Controls.Add(this.txtPin5);
            this.pnlPinBoxes.Controls.Add(this.txtPin6);

            // Helper locals for compact PIN-box setup
            int boxWidth = 32, boxHeight = 40, spacing = 7, initialX = 1;
            System.Drawing.Font pinFont = new System.Drawing.Font(AppTheme.FontFamily, 16F, System.Drawing.FontStyle.Bold);

            SetupPinBox(this.txtPin1, initialX + 0 * (boxWidth + spacing), 6, boxWidth, boxHeight, pinFont);
            SetupPinBox(this.txtPin2, initialX + 1 * (boxWidth + spacing), 6, boxWidth, boxHeight, pinFont);
            SetupPinBox(this.txtPin3, initialX + 2 * (boxWidth + spacing), 6, boxWidth, boxHeight, pinFont);
            SetupPinBox(this.txtPin4, initialX + 3 * (boxWidth + spacing) + 10, 6, boxWidth, boxHeight, pinFont);
            SetupPinBox(this.txtPin5, initialX + 4 * (boxWidth + spacing) + 10, 6, boxWidth, boxHeight, pinFont);
            SetupPinBox(this.txtPin6, initialX + 5 * (boxWidth + spacing) + 10, 6, boxWidth, boxHeight, pinFont);

            // ---- btnConnect ----
            this.btnConnect.BackColor               = AppTheme.AccentBlue;
            this.btnConnect.FlatStyle               = System.Windows.Forms.FlatStyle.Flat;
            this.btnConnect.FlatAppearance.BorderSize = 0;
            this.btnConnect.FlatAppearance.MouseOverBackColor = AppTheme.AccentBlueDark;
            this.btnConnect.Font      = new System.Drawing.Font(AppTheme.FontFamily, 11F, System.Drawing.FontStyle.Bold);
            this.btnConnect.ForeColor = AppTheme.TextPrimary;
            this.btnConnect.Location  = new System.Drawing.Point(12, 130);
            this.btnConnect.Size      = new System.Drawing.Size(246, 40);
            this.btnConnect.Text      = "Connect";
            this.btnConnect.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.btnConnect.Click    += new System.EventHandler(this.btnConnect_Click);

            // ---- btnDisconnect ----
            this.btnDisconnect.BackColor               = AppTheme.ErrorRed;
            this.btnDisconnect.Enabled                 = false;
            this.btnDisconnect.FlatStyle               = System.Windows.Forms.FlatStyle.Flat;
            this.btnDisconnect.FlatAppearance.BorderSize = 0;
            this.btnDisconnect.FlatAppearance.MouseOverBackColor = AppTheme.ErrorRedDark;
            this.btnDisconnect.Font      = new System.Drawing.Font(AppTheme.FontFamily, 11F, System.Drawing.FontStyle.Bold);
            this.btnDisconnect.ForeColor = AppTheme.TextPrimary;
            this.btnDisconnect.Location  = new System.Drawing.Point(12, 180);
            this.btnDisconnect.Size      = new System.Drawing.Size(246, 40);
            this.btnDisconnect.Text      = "Disconnect";
            this.btnDisconnect.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.btnDisconnect.Click    += new System.EventHandler(this.btnDisconnect_Click);

            // ----------------------------------------------------------------
            // picScreen – live stream area (fills remainder of form)
            // ----------------------------------------------------------------
            this.picScreen.BackColor = System.Drawing.Color.FromArgb(0x0A, 0x0A, 0x0A);
            this.picScreen.Dock      = System.Windows.Forms.DockStyle.Fill;
            this.picScreen.SizeMode  = System.Windows.Forms.PictureBoxSizeMode.Zoom;

            // ----------------------------------------------------------------
            // lblPerfOverlay – stats label anchored top-right of stream area
            // ----------------------------------------------------------------
            this.lblPerfOverlay.AutoSize  = false;
            this.lblPerfOverlay.Anchor    = System.Windows.Forms.AnchorStyles.Top
                                          | System.Windows.Forms.AnchorStyles.Right;
            this.lblPerfOverlay.BackColor = System.Drawing.Color.FromArgb(160, 0, 0, 0);
            this.lblPerfOverlay.Font      = new System.Drawing.Font("Courier New", 9F,
                                               System.Drawing.FontStyle.Bold);
            this.lblPerfOverlay.ForeColor = AppTheme.SuccessGreen;
            this.lblPerfOverlay.Location  = new System.Drawing.Point(790, 50);
            this.lblPerfOverlay.Size      = new System.Drawing.Size(260, 20);
            this.lblPerfOverlay.Text      = string.Empty;
            this.lblPerfOverlay.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblPerfOverlay.Visible   = false;

            // ----------------------------------------------------------------
            // ViewerForm
            // ----------------------------------------------------------------
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor           = AppTheme.Background;
            this.ClientSize          = new System.Drawing.Size(1060, 640);
            this.Controls.Add(this.lblPerfOverlay);
            this.Controls.Add(this.picScreen);
            this.Controls.Add(this.pnlLeft);
            this.Controls.Add(this.pnlStatusBar);
            this.Controls.Add(this.pnlTitleBar);
            this.FormBorderStyle     = System.Windows.Forms.FormBorderStyle.Sizable;
            this.MinimumSize         = new System.Drawing.Size(760, 520);
            this.Name                = "ViewerForm";
            this.StartPosition       = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text                = "LanOra \u2013 Viewer";
            this.FormClosing        += new System.Windows.Forms.FormClosingEventHandler(this.ViewerForm_FormClosing);

            this.pnlTitleBar.ResumeLayout(false);
            this.pnlLeft.ResumeLayout(false);
            this.pnlConnectSect.ResumeLayout(false);
            this.pnlPinBoxes.ResumeLayout(false);
            this.pnlPinBoxes.PerformLayout();
            this.pnlStatusBar.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.picScreen).EndInit();
            this.ResumeLayout(false);
        }

        /// <summary>Applies shared styling to one PIN digit TextBox.</summary>
        private static void SetupPinBox(System.Windows.Forms.TextBox tb,
                                        int x, int y, int w, int h,
                                        System.Drawing.Font font)
        {
            tb.BackColor   = AppTheme.InputBackground;
            tb.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            tb.Font        = font;
            tb.ForeColor   = AppTheme.WarningYellow;
            tb.Location    = new System.Drawing.Point(x, y);
            tb.MaxLength   = 1;
            tb.Multiline   = false;
            tb.Size        = new System.Drawing.Size(w, h);
            tb.TextAlign   = System.Windows.Forms.HorizontalAlignment.Center;
        }

        #endregion

        private System.Windows.Forms.Panel      pnlTitleBar;
        private System.Windows.Forms.Label      lblAppName;
        private System.Windows.Forms.Button     btnClose;
        private System.Windows.Forms.Button     btnMinimize;
        private System.Windows.Forms.Panel      pnlLeft;
        private System.Windows.Forms.Label      lblHostsCap;
        private System.Windows.Forms.ListBox    lstHosts;
        private System.Windows.Forms.Button     btnRefresh;
        private System.Windows.Forms.Panel      pnlConnectSect;
        private System.Windows.Forms.Label      lblConnectHdr;
        private System.Windows.Forms.Label      lblPinCaption;
        private System.Windows.Forms.Panel      pnlPinBoxes;
        private System.Windows.Forms.TextBox    txtPin1;
        private System.Windows.Forms.TextBox    txtPin2;
        private System.Windows.Forms.TextBox    txtPin3;
        private System.Windows.Forms.TextBox    txtPin4;
        private System.Windows.Forms.TextBox    txtPin5;
        private System.Windows.Forms.TextBox    txtPin6;
        private System.Windows.Forms.Button     btnConnect;
        private System.Windows.Forms.Button     btnDisconnect;
        private System.Windows.Forms.PictureBox picScreen;
        private System.Windows.Forms.Label      lblPerfOverlay;
        private System.Windows.Forms.Panel      pnlStatusBar;
        private System.Windows.Forms.Label      lblStatusDot;
        private System.Windows.Forms.Label      lblStatusBar;
    }
}

