using LanOra.Theme;

namespace LanOra.Forms
{
    partial class HostForm
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
            this.pnlContent     = new System.Windows.Forms.Panel();
            this.lblNetInfoHdr  = new System.Windows.Forms.Label();
            this.pnlNetInfo     = new System.Windows.Forms.Panel();
            this.lblIpCaption   = new System.Windows.Forms.Label();
            this.lblIpValue     = new System.Windows.Forms.Label();
            this.lblStatusCaption = new System.Windows.Forms.Label();
            this.lblStatusDot   = new System.Windows.Forms.Label();
            this.lblIpValue2    = new System.Windows.Forms.Label();
            this.lblPort        = new System.Windows.Forms.Label();
            this.lblAuthHdr     = new System.Windows.Forms.Label();
            this.pnlAuthSection = new System.Windows.Forms.Panel();
            this.lblPinCaption  = new System.Windows.Forms.Label();
            this.lblPinValue    = new System.Windows.Forms.Label();
            this.lblPinNote     = new System.Windows.Forms.Label();
            this.btnStart       = new System.Windows.Forms.Button();
            this.btnStop        = new System.Windows.Forms.Button();
            this.lblViewerCount = new System.Windows.Forms.Label();
            this.pnlStatusBar   = new System.Windows.Forms.Panel();
            this.lblStatusBar   = new System.Windows.Forms.Label();

            this.pnlTitleBar.SuspendLayout();
            this.pnlContent.SuspendLayout();
            this.pnlNetInfo.SuspendLayout();
            this.pnlAuthSection.SuspendLayout();
            this.pnlStatusBar.SuspendLayout();
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
            this.lblAppName.Text      = "LanOra  \u2013  Host Mode";
            this.lblAppName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblAppName.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TitleBar_MouseDown);
            this.lblAppName.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TitleBar_MouseMove);

            this.btnClose.BackColor               = AppTheme.TitleBar;
            this.btnClose.FlatStyle               = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatAppearance.MouseOverBackColor = AppTheme.ErrorRed;
            this.btnClose.Font      = new System.Drawing.Font(AppTheme.FontFamily, 11F);
            this.btnClose.ForeColor = AppTheme.TextSecondary;
            this.btnClose.Location  = new System.Drawing.Point(434, 0);
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
            this.btnMinimize.Location  = new System.Drawing.Point(388, 0);
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

            this.lblStatusBar.AutoSize  = false;
            this.lblStatusBar.Dock      = System.Windows.Forms.DockStyle.Fill;
            this.lblStatusBar.Font      = new System.Drawing.Font(AppTheme.FontFamily, 8F);
            this.lblStatusBar.ForeColor = AppTheme.TextPrimary;
            this.lblStatusBar.Padding   = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.lblStatusBar.Text      = "Host Mode: Idle  |  " + AppTheme.Developer;
            this.lblStatusBar.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // ----------------------------------------------------------------
            // pnlContent
            // ----------------------------------------------------------------
            this.pnlContent.BackColor = AppTheme.Background;
            this.pnlContent.Dock      = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.Padding   = new System.Windows.Forms.Padding(20, 14, 20, 10);
            this.pnlContent.Controls.Add(this.lblViewerCount);
            this.pnlContent.Controls.Add(this.btnStop);
            this.pnlContent.Controls.Add(this.btnStart);
            this.pnlContent.Controls.Add(this.pnlAuthSection);
            this.pnlContent.Controls.Add(this.lblAuthHdr);
            this.pnlContent.Controls.Add(this.pnlNetInfo);
            this.pnlContent.Controls.Add(this.lblNetInfoHdr);

            // ---- Section 1 header ----
            this.lblNetInfoHdr.AutoSize  = false;
            this.lblNetInfoHdr.Font      = new System.Drawing.Font(AppTheme.FontFamily, 9F, System.Drawing.FontStyle.Bold);
            this.lblNetInfoHdr.ForeColor = AppTheme.AccentBlue;
            this.lblNetInfoHdr.Location  = new System.Drawing.Point(20, 14);
            this.lblNetInfoHdr.Size      = new System.Drawing.Size(440, 22);
            this.lblNetInfoHdr.Text      = "LOCAL NETWORK INFORMATION";

            // ---- pnlNetInfo (section 1 body) ----
            this.pnlNetInfo.BackColor   = AppTheme.Panel;
            this.pnlNetInfo.Location    = new System.Drawing.Point(20, 40);
            this.pnlNetInfo.Size        = new System.Drawing.Size(440, 90);
            this.pnlNetInfo.Controls.Add(this.lblPort);
            this.pnlNetInfo.Controls.Add(this.lblIpValue2);
            this.pnlNetInfo.Controls.Add(this.lblStatusDot);
            this.pnlNetInfo.Controls.Add(this.lblStatusCaption);
            this.pnlNetInfo.Controls.Add(this.lblIpValue);
            this.pnlNetInfo.Controls.Add(this.lblIpCaption);

            this.lblIpCaption.AutoSize  = false;
            this.lblIpCaption.Font      = new System.Drawing.Font(AppTheme.FontFamily, 9F);
            this.lblIpCaption.ForeColor = AppTheme.TextSecondary;
            this.lblIpCaption.Location  = new System.Drawing.Point(14, 12);
            this.lblIpCaption.Size      = new System.Drawing.Size(100, 20);
            this.lblIpCaption.Text      = "IP Address:";

            this.lblIpValue.AutoSize  = false;
            this.lblIpValue.Font      = new System.Drawing.Font(AppTheme.FontFamily, 12F, System.Drawing.FontStyle.Bold);
            this.lblIpValue.ForeColor = System.Drawing.Color.FromArgb(0x4E, 0xC9, 0xE0);
            this.lblIpValue.Location  = new System.Drawing.Point(120, 8);
            this.lblIpValue.Size      = new System.Drawing.Size(200, 28);
            this.lblIpValue.Text      = "Detecting\u2026";

            this.lblStatusCaption.AutoSize  = false;
            this.lblStatusCaption.Font      = new System.Drawing.Font(AppTheme.FontFamily, 9F);
            this.lblStatusCaption.ForeColor = AppTheme.TextSecondary;
            this.lblStatusCaption.Location  = new System.Drawing.Point(14, 48);
            this.lblStatusCaption.Size      = new System.Drawing.Size(100, 20);
            this.lblStatusCaption.Text      = "Status:";

            this.lblStatusDot.AutoSize  = false;
            this.lblStatusDot.Font      = new System.Drawing.Font(AppTheme.FontFamily, 14F, System.Drawing.FontStyle.Bold);
            this.lblStatusDot.ForeColor = AppTheme.ErrorRed;
            this.lblStatusDot.Location  = new System.Drawing.Point(118, 40);
            this.lblStatusDot.Size      = new System.Drawing.Size(22, 28);
            this.lblStatusDot.Text      = "\u25CF";

            this.lblIpValue2.AutoSize  = false;
            this.lblIpValue2.Font      = new System.Drawing.Font(AppTheme.FontFamily, 11F);
            this.lblIpValue2.ForeColor = AppTheme.TextPrimary;
            this.lblIpValue2.Location  = new System.Drawing.Point(144, 44);
            this.lblIpValue2.Size      = new System.Drawing.Size(200, 28);
            this.lblIpValue2.Text      = "Not Hosting";

            this.lblPort.AutoSize  = false;
            this.lblPort.Font      = new System.Drawing.Font(AppTheme.FontFamily, 9F);
            this.lblPort.ForeColor = AppTheme.TextSecondary;
            this.lblPort.Location  = new System.Drawing.Point(360, 12);
            this.lblPort.Size      = new System.Drawing.Size(70, 20);
            this.lblPort.Text      = "Port: 5000";
            this.lblPort.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            // ---- Section 2 header ----
            this.lblAuthHdr.AutoSize  = false;
            this.lblAuthHdr.Font      = new System.Drawing.Font(AppTheme.FontFamily, 9F, System.Drawing.FontStyle.Bold);
            this.lblAuthHdr.ForeColor = AppTheme.AccentBlue;
            this.lblAuthHdr.Location  = new System.Drawing.Point(20, 146);
            this.lblAuthHdr.Size      = new System.Drawing.Size(440, 22);
            this.lblAuthHdr.Text      = "SESSION AUTHENTICATION";

            // ---- pnlAuthSection ----
            this.pnlAuthSection.BackColor = AppTheme.Panel;
            this.pnlAuthSection.Location  = new System.Drawing.Point(20, 172);
            this.pnlAuthSection.Size      = new System.Drawing.Size(440, 140);
            this.pnlAuthSection.Controls.Add(this.lblPinNote);
            this.pnlAuthSection.Controls.Add(this.lblPinValue);
            this.pnlAuthSection.Controls.Add(this.lblPinCaption);

            this.lblPinCaption.AutoSize  = false;
            this.lblPinCaption.Font      = new System.Drawing.Font(AppTheme.FontFamily, 9F);
            this.lblPinCaption.ForeColor = AppTheme.TextSecondary;
            this.lblPinCaption.Location  = new System.Drawing.Point(14, 14);
            this.lblPinCaption.Size      = new System.Drawing.Size(412, 20);
            this.lblPinCaption.Text      = "Shared PIN:";

            this.lblPinValue.AutoSize  = false;
            this.lblPinValue.Font      = new System.Drawing.Font(AppTheme.FontFamily, 36F, System.Drawing.FontStyle.Bold);
            this.lblPinValue.ForeColor = AppTheme.WarningYellow;
            this.lblPinValue.Location  = new System.Drawing.Point(0, 36);
            this.lblPinValue.Size      = new System.Drawing.Size(440, 68);
            this.lblPinValue.Text      = "\u2014\u2014\u2014 \u2014\u2014\u2014";
            this.lblPinValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            this.lblPinNote.AutoSize  = false;
            this.lblPinNote.Font      = new System.Drawing.Font(AppTheme.FontFamily, 8.5F);
            this.lblPinNote.ForeColor = AppTheme.TextSecondary;
            this.lblPinNote.Location  = new System.Drawing.Point(0, 108);
            this.lblPinNote.Size      = new System.Drawing.Size(440, 22);
            this.lblPinNote.Text      = "Share this PIN with the viewer to allow connection.";
            this.lblPinNote.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // ---- btnStart ----
            this.btnStart.BackColor               = AppTheme.AccentBlue;
            this.btnStart.FlatStyle               = System.Windows.Forms.FlatStyle.Flat;
            this.btnStart.FlatAppearance.BorderSize = 0;
            this.btnStart.FlatAppearance.MouseOverBackColor = AppTheme.AccentBlueDark;
            this.btnStart.Font      = new System.Drawing.Font(AppTheme.FontFamily, 12F, System.Drawing.FontStyle.Bold);
            this.btnStart.ForeColor = AppTheme.TextPrimary;
            this.btnStart.Location  = new System.Drawing.Point(20, 328);
            this.btnStart.Size      = new System.Drawing.Size(440, 44);
            this.btnStart.Text      = "Start Hosting";
            this.btnStart.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.btnStart.Click    += new System.EventHandler(this.btnStart_Click);

            // ---- btnStop ----
            this.btnStop.BackColor               = AppTheme.ErrorRed;
            this.btnStop.Enabled                 = false;
            this.btnStop.FlatStyle               = System.Windows.Forms.FlatStyle.Flat;
            this.btnStop.FlatAppearance.BorderSize = 0;
            this.btnStop.FlatAppearance.MouseOverBackColor = AppTheme.ErrorRedDark;
            this.btnStop.Font      = new System.Drawing.Font(AppTheme.FontFamily, 12F, System.Drawing.FontStyle.Bold);
            this.btnStop.ForeColor = AppTheme.TextPrimary;
            this.btnStop.Location  = new System.Drawing.Point(20, 328);
            this.btnStop.Size      = new System.Drawing.Size(440, 44);
            this.btnStop.Text      = "Stop Hosting";
            this.btnStop.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.btnStop.Visible   = false;
            this.btnStop.Click    += new System.EventHandler(this.btnStop_Click);

            // ---- lblViewerCount ----
            this.lblViewerCount.AutoSize  = false;
            this.lblViewerCount.Font      = new System.Drawing.Font(AppTheme.FontFamily, 9F);
            this.lblViewerCount.ForeColor = AppTheme.TextSecondary;
            this.lblViewerCount.Location  = new System.Drawing.Point(20, 382);
            this.lblViewerCount.Size      = new System.Drawing.Size(440, 22);
            this.lblViewerCount.Text      = "Connected Viewers: 0";
            this.lblViewerCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblViewerCount.Visible   = false;

            // ----------------------------------------------------------------
            // HostForm
            // ----------------------------------------------------------------
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor           = AppTheme.Background;
            this.ClientSize          = new System.Drawing.Size(480, 468);
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlStatusBar);
            this.Controls.Add(this.pnlTitleBar);
            this.FormBorderStyle     = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox         = false;
            this.Name                = "HostForm";
            this.StartPosition       = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text                = "LanOra \u2013 Host";
            this.FormClosing        += new System.Windows.Forms.FormClosingEventHandler(this.HostForm_FormClosing);

            this.pnlTitleBar.ResumeLayout(false);
            this.pnlContent.ResumeLayout(false);
            this.pnlNetInfo.ResumeLayout(false);
            this.pnlAuthSection.ResumeLayout(false);
            this.pnlStatusBar.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel  pnlTitleBar;
        private System.Windows.Forms.Label  lblAppName;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnMinimize;
        private System.Windows.Forms.Panel  pnlContent;
        private System.Windows.Forms.Label  lblNetInfoHdr;
        private System.Windows.Forms.Panel  pnlNetInfo;
        private System.Windows.Forms.Label  lblIpCaption;
        private System.Windows.Forms.Label  lblIpValue;
        private System.Windows.Forms.Label  lblStatusCaption;
        private System.Windows.Forms.Label  lblStatusDot;
        private System.Windows.Forms.Label  lblIpValue2;
        private System.Windows.Forms.Label  lblPort;
        private System.Windows.Forms.Label  lblAuthHdr;
        private System.Windows.Forms.Panel  pnlAuthSection;
        private System.Windows.Forms.Label  lblPinCaption;
        private System.Windows.Forms.Label  lblPinValue;
        private System.Windows.Forms.Label  lblPinNote;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Label  lblViewerCount;
        private System.Windows.Forms.Panel  pnlStatusBar;
        private System.Windows.Forms.Label  lblStatusBar;
    }
}
