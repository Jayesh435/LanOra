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
            this.pnlHeader     = new System.Windows.Forms.Panel();
            this.lblTitle      = new System.Windows.Forms.Label();
            this.pnlLeft       = new System.Windows.Forms.Panel();
            this.lblHostsCap   = new System.Windows.Forms.Label();
            this.lstHosts      = new System.Windows.Forms.ListBox();
            this.btnRefresh    = new System.Windows.Forms.Button();
            this.lblPinCaption = new System.Windows.Forms.Label();
            this.txtPin        = new System.Windows.Forms.TextBox();
            this.btnConnect    = new System.Windows.Forms.Button();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.picScreen     = new System.Windows.Forms.PictureBox();
            this.pnlStatusBar  = new System.Windows.Forms.Panel();
            this.lblStatusDot  = new System.Windows.Forms.Label();
            this.lblStatus     = new System.Windows.Forms.Label();
            this.pnlHeader.SuspendLayout();
            this.pnlLeft.SuspendLayout();
            this.pnlStatusBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.picScreen).BeginInit();
            this.SuspendLayout();

            // ---- pnlHeader ----
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(30, 30, 45);
            this.pnlHeader.Dock      = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Height    = 55;
            this.pnlHeader.Controls.Add(this.lblTitle);

            // ---- lblTitle ----
            this.lblTitle.AutoSize  = false;
            this.lblTitle.Dock      = System.Windows.Forms.DockStyle.Fill;
            this.lblTitle.Font      = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Text      = "LanOra – Viewer";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // ---- pnlStatusBar ----
            this.pnlStatusBar.BackColor = System.Drawing.Color.FromArgb(20, 20, 35);
            this.pnlStatusBar.Dock      = System.Windows.Forms.DockStyle.Bottom;
            this.pnlStatusBar.Height    = 32;
            this.pnlStatusBar.Controls.Add(this.lblStatus);
            this.pnlStatusBar.Controls.Add(this.lblStatusDot);

            // ---- lblStatusDot ----
            this.lblStatusDot.AutoSize  = false;
            this.lblStatusDot.Font      = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblStatusDot.ForeColor = System.Drawing.Color.Red;
            this.lblStatusDot.Location  = new System.Drawing.Point(6, 4);
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

            // ---- pnlLeft ----
            this.pnlLeft.BackColor = System.Drawing.Color.FromArgb(30, 30, 50);
            this.pnlLeft.Dock      = System.Windows.Forms.DockStyle.Left;
            this.pnlLeft.Width     = 250;
            this.pnlLeft.Controls.Add(this.lblHostsCap);
            this.pnlLeft.Controls.Add(this.lstHosts);
            this.pnlLeft.Controls.Add(this.btnRefresh);
            this.pnlLeft.Controls.Add(this.lblPinCaption);
            this.pnlLeft.Controls.Add(this.txtPin);
            this.pnlLeft.Controls.Add(this.btnConnect);
            this.pnlLeft.Controls.Add(this.btnDisconnect);

            // ---- lblHostsCap ----
            this.lblHostsCap.AutoSize  = false;
            this.lblHostsCap.Font      = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblHostsCap.ForeColor = System.Drawing.Color.Silver;
            this.lblHostsCap.Location  = new System.Drawing.Point(10, 10);
            this.lblHostsCap.Size      = new System.Drawing.Size(225, 24);
            this.lblHostsCap.Text      = "Available Hosts:";

            // ---- lstHosts ----
            this.lstHosts.BackColor   = System.Drawing.Color.FromArgb(50, 50, 70);
            this.lstHosts.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstHosts.Font        = new System.Drawing.Font("Segoe UI", 9F);
            this.lstHosts.ForeColor   = System.Drawing.Color.White;
            this.lstHosts.Location    = new System.Drawing.Point(10, 38);
            this.lstHosts.Size        = new System.Drawing.Size(225, 180);

            // ---- btnRefresh ----
            this.btnRefresh.BackColor = System.Drawing.Color.FromArgb(60, 60, 90);
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.Font      = new System.Drawing.Font("Segoe UI", 9F);
            this.btnRefresh.ForeColor = System.Drawing.Color.Silver;
            this.btnRefresh.Location  = new System.Drawing.Point(10, 225);
            this.btnRefresh.Size      = new System.Drawing.Size(225, 28);
            this.btnRefresh.Text      = "Refresh List";
            this.btnRefresh.Click    += new System.EventHandler(this.btnRefresh_Click);

            // ---- lblPinCaption ----
            this.lblPinCaption.AutoSize  = false;
            this.lblPinCaption.Font      = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblPinCaption.ForeColor = System.Drawing.Color.Silver;
            this.lblPinCaption.Location  = new System.Drawing.Point(10, 268);
            this.lblPinCaption.Size      = new System.Drawing.Size(225, 24);
            this.lblPinCaption.Text      = "Enter PIN:";

            // ---- txtPin ----
            this.txtPin.BackColor   = System.Drawing.Color.FromArgb(50, 50, 70);
            this.txtPin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPin.Font        = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.txtPin.ForeColor   = System.Drawing.Color.Yellow;
            this.txtPin.Location    = new System.Drawing.Point(10, 296);
            this.txtPin.MaxLength   = 6;
            this.txtPin.Size        = new System.Drawing.Size(225, 38);
            this.txtPin.TextAlign   = System.Windows.Forms.HorizontalAlignment.Center;

            // ---- btnConnect ----
            this.btnConnect.BackColor = System.Drawing.Color.FromArgb(0, 153, 76);
            this.btnConnect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConnect.Font      = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnConnect.ForeColor = System.Drawing.Color.White;
            this.btnConnect.Location  = new System.Drawing.Point(10, 348);
            this.btnConnect.Size      = new System.Drawing.Size(225, 38);
            this.btnConnect.Text      = "Connect";
            this.btnConnect.Click    += new System.EventHandler(this.btnConnect_Click);

            // ---- btnDisconnect ----
            this.btnDisconnect.BackColor = System.Drawing.Color.FromArgb(180, 30, 30);
            this.btnDisconnect.Enabled   = false;
            this.btnDisconnect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDisconnect.Font      = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnDisconnect.ForeColor = System.Drawing.Color.White;
            this.btnDisconnect.Location  = new System.Drawing.Point(10, 396);
            this.btnDisconnect.Size      = new System.Drawing.Size(225, 38);
            this.btnDisconnect.Text      = "Disconnect";
            this.btnDisconnect.Click    += new System.EventHandler(this.btnDisconnect_Click);

            // ---- picScreen ----
            this.picScreen.BackColor = System.Drawing.Color.Black;
            this.picScreen.Dock      = System.Windows.Forms.DockStyle.Fill;
            this.picScreen.SizeMode  = System.Windows.Forms.PictureBoxSizeMode.Zoom;

            // ---- ViewerForm ----
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor           = System.Drawing.Color.Black;
            this.ClientSize          = new System.Drawing.Size(1060, 640);
            this.Controls.Add(this.picScreen);
            this.Controls.Add(this.pnlLeft);
            this.Controls.Add(this.pnlStatusBar);
            this.Controls.Add(this.pnlHeader);
            this.FormBorderStyle     = System.Windows.Forms.FormBorderStyle.Sizable;
            this.MinimumSize         = new System.Drawing.Size(700, 500);
            this.Name                = "ViewerForm";
            this.StartPosition       = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text                = "LanOra – Viewer";
            this.FormClosing        += new System.Windows.Forms.FormClosingEventHandler(this.ViewerForm_FormClosing);

            this.pnlHeader.ResumeLayout(false);
            this.pnlLeft.ResumeLayout(false);
            this.pnlLeft.PerformLayout();
            this.pnlStatusBar.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.picScreen).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel      pnlHeader;
        private System.Windows.Forms.Label      lblTitle;
        private System.Windows.Forms.Panel      pnlLeft;
        private System.Windows.Forms.Label      lblHostsCap;
        private System.Windows.Forms.ListBox    lstHosts;
        private System.Windows.Forms.Button     btnRefresh;
        private System.Windows.Forms.Label      lblPinCaption;
        private System.Windows.Forms.TextBox    txtPin;
        private System.Windows.Forms.Button     btnConnect;
        private System.Windows.Forms.Button     btnDisconnect;
        private System.Windows.Forms.PictureBox picScreen;
        private System.Windows.Forms.Panel      pnlStatusBar;
        private System.Windows.Forms.Label      lblStatusDot;
        private System.Windows.Forms.Label      lblStatus;
    }
}
