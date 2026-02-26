namespace LANMonitor.Viewer.Forms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.pnlHeader      = new System.Windows.Forms.Panel();
            this.lblTitle       = new System.Windows.Forms.Label();
            this.pnlToolbar     = new System.Windows.Forms.Panel();
            this.lblIpLabel     = new System.Windows.Forms.Label();
            this.txtIpAddress   = new System.Windows.Forms.TextBox();
            this.btnConnect     = new System.Windows.Forms.Button();
            this.btnDisconnect  = new System.Windows.Forms.Button();
            this.picScreen      = new System.Windows.Forms.PictureBox();
            this.pnlStatusBar   = new System.Windows.Forms.Panel();
            this.lblStatusDot   = new System.Windows.Forms.Label();
            this.lblStatus      = new System.Windows.Forms.Label();
            this.pnlHeader.SuspendLayout();
            this.pnlToolbar.SuspendLayout();
            this.pnlStatusBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picScreen)).BeginInit();
            this.SuspendLayout();

            // ---- pnlHeader ----
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(30, 30, 45);
            this.pnlHeader.Dock      = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Height    = 50;
            this.pnlHeader.Controls.Add(this.lblTitle);

            // ---- lblTitle ----
            this.lblTitle.AutoSize  = false;
            this.lblTitle.Dock      = System.Windows.Forms.DockStyle.Fill;
            this.lblTitle.Font      = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Text      = "LAN Screen Viewer";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // ---- pnlToolbar ----
            this.pnlToolbar.BackColor = System.Drawing.Color.FromArgb(45, 45, 65);
            this.pnlToolbar.Dock      = System.Windows.Forms.DockStyle.Top;
            this.pnlToolbar.Height    = 46;
            this.pnlToolbar.Padding   = new System.Windows.Forms.Padding(8, 6, 8, 6);
            this.pnlToolbar.Controls.Add(this.btnDisconnect);
            this.pnlToolbar.Controls.Add(this.btnConnect);
            this.pnlToolbar.Controls.Add(this.txtIpAddress);
            this.pnlToolbar.Controls.Add(this.lblIpLabel);

            // ---- lblIpLabel ----
            this.lblIpLabel.AutoSize  = false;
            this.lblIpLabel.Font      = new System.Drawing.Font("Segoe UI", 10F);
            this.lblIpLabel.ForeColor = System.Drawing.Color.Silver;
            this.lblIpLabel.Location  = new System.Drawing.Point(8, 10);
            this.lblIpLabel.Size      = new System.Drawing.Size(80, 24);
            this.lblIpLabel.Text      = "Server IP:";
            this.lblIpLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // ---- txtIpAddress ----
            this.txtIpAddress.BackColor   = System.Drawing.Color.FromArgb(60, 60, 80);
            this.txtIpAddress.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtIpAddress.Font        = new System.Drawing.Font("Segoe UI", 10F);
            this.txtIpAddress.ForeColor   = System.Drawing.Color.White;
            this.txtIpAddress.Location    = new System.Drawing.Point(92, 10);
            this.txtIpAddress.Size        = new System.Drawing.Size(160, 24);
            this.txtIpAddress.Text        = "192.168.1.";

            // ---- btnConnect ----
            this.btnConnect.BackColor = System.Drawing.Color.FromArgb(0, 153, 76);
            this.btnConnect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConnect.Font      = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnConnect.ForeColor = System.Drawing.Color.White;
            this.btnConnect.Location  = new System.Drawing.Point(262, 8);
            this.btnConnect.Size      = new System.Drawing.Size(100, 28);
            this.btnConnect.Text      = "Connect";
            this.btnConnect.Click    += new System.EventHandler(this.btnConnect_Click);

            // ---- btnDisconnect ----
            this.btnDisconnect.BackColor = System.Drawing.Color.FromArgb(180, 30, 30);
            this.btnDisconnect.Enabled   = false;
            this.btnDisconnect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDisconnect.Font      = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnDisconnect.ForeColor = System.Drawing.Color.White;
            this.btnDisconnect.Location  = new System.Drawing.Point(372, 8);
            this.btnDisconnect.Size      = new System.Drawing.Size(100, 28);
            this.btnDisconnect.Text      = "Disconnect";
            this.btnDisconnect.Click    += new System.EventHandler(this.btnDisconnect_Click);

            // ---- picScreen ----
            this.picScreen.BackColor  = System.Drawing.Color.Black;
            this.picScreen.Dock       = System.Windows.Forms.DockStyle.Fill;
            this.picScreen.SizeMode   = System.Windows.Forms.PictureBoxSizeMode.Zoom;

            // ---- pnlStatusBar ----
            this.pnlStatusBar.BackColor = System.Drawing.Color.FromArgb(20, 20, 35);
            this.pnlStatusBar.Dock      = System.Windows.Forms.DockStyle.Bottom;
            this.pnlStatusBar.Height    = 28;
            this.pnlStatusBar.Controls.Add(this.lblStatus);
            this.pnlStatusBar.Controls.Add(this.lblStatusDot);

            // ---- lblStatusDot ----
            this.lblStatusDot.AutoSize  = false;
            this.lblStatusDot.Font      = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblStatusDot.ForeColor = System.Drawing.Color.Red;
            this.lblStatusDot.Location  = new System.Drawing.Point(6, 2);
            this.lblStatusDot.Size      = new System.Drawing.Size(20, 24);
            this.lblStatusDot.Text      = "●";

            // ---- lblStatus ----
            this.lblStatus.AutoSize  = false;
            this.lblStatus.Dock      = System.Windows.Forms.DockStyle.Fill;
            this.lblStatus.Font      = new System.Drawing.Font("Segoe UI", 9F);
            this.lblStatus.ForeColor = System.Drawing.Color.Silver;
            this.lblStatus.Padding   = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.lblStatus.Text      = "Status: Idle";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // ---- MainForm ----
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor           = System.Drawing.Color.Black;
            this.ClientSize          = new System.Drawing.Size(1024, 668);
            this.Controls.Add(this.picScreen);
            this.Controls.Add(this.pnlStatusBar);
            this.Controls.Add(this.pnlToolbar);
            this.Controls.Add(this.pnlHeader);
            this.FormBorderStyle     = System.Windows.Forms.FormBorderStyle.Sizable;
            this.MinimumSize         = new System.Drawing.Size(700, 500);
            this.Name                = "MainForm";
            this.StartPosition       = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text                = "LAN Screen Viewer";
            this.FormClosing        += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);

            this.pnlHeader.ResumeLayout(false);
            this.pnlToolbar.ResumeLayout(false);
            this.pnlToolbar.PerformLayout();
            this.pnlStatusBar.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picScreen)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel      pnlHeader;
        private System.Windows.Forms.Label      lblTitle;
        private System.Windows.Forms.Panel      pnlToolbar;
        private System.Windows.Forms.Label      lblIpLabel;
        private System.Windows.Forms.TextBox    txtIpAddress;
        private System.Windows.Forms.Button     btnConnect;
        private System.Windows.Forms.Button     btnDisconnect;
        private System.Windows.Forms.PictureBox picScreen;
        private System.Windows.Forms.Panel      pnlStatusBar;
        private System.Windows.Forms.Label      lblStatusDot;
        private System.Windows.Forms.Label      lblStatus;
    }
}
