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
            this.pnlHeader     = new System.Windows.Forms.Panel();
            this.lblTitle      = new System.Windows.Forms.Label();
            this.pnlContent    = new System.Windows.Forms.Panel();
            this.lblIpCaption  = new System.Windows.Forms.Label();
            this.lblIpValue    = new System.Windows.Forms.Label();
            this.lblPort       = new System.Windows.Forms.Label();
            this.lblPinCaption = new System.Windows.Forms.Label();
            this.lblPinValue   = new System.Windows.Forms.Label();
            this.lblPinNote    = new System.Windows.Forms.Label();
            this.btnStart      = new System.Windows.Forms.Button();
            this.btnStop       = new System.Windows.Forms.Button();
            this.pnlStatusBar  = new System.Windows.Forms.Panel();
            this.lblStatusDot  = new System.Windows.Forms.Label();
            this.lblStatus     = new System.Windows.Forms.Label();
            this.pnlHeader.SuspendLayout();
            this.pnlContent.SuspendLayout();
            this.pnlStatusBar.SuspendLayout();
            this.SuspendLayout();

            // ---- pnlHeader ----
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(30, 30, 45);
            this.pnlHeader.Dock      = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Height    = 60;
            this.pnlHeader.Controls.Add(this.lblTitle);

            // ---- lblTitle ----
            this.lblTitle.AutoSize  = false;
            this.lblTitle.Dock      = System.Windows.Forms.DockStyle.Fill;
            this.lblTitle.Font      = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Text      = "LanOra – Host Mode";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // ---- pnlContent ----
            this.pnlContent.BackColor = System.Drawing.Color.FromArgb(40, 40, 60);
            this.pnlContent.Dock      = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.Padding   = new System.Windows.Forms.Padding(20);
            this.pnlContent.Controls.Add(this.lblPinNote);
            this.pnlContent.Controls.Add(this.lblPinValue);
            this.pnlContent.Controls.Add(this.lblPinCaption);
            this.pnlContent.Controls.Add(this.btnStop);
            this.pnlContent.Controls.Add(this.btnStart);
            this.pnlContent.Controls.Add(this.lblPort);
            this.pnlContent.Controls.Add(this.lblIpValue);
            this.pnlContent.Controls.Add(this.lblIpCaption);

            // ---- lblIpCaption ----
            this.lblIpCaption.AutoSize  = false;
            this.lblIpCaption.Font      = new System.Drawing.Font("Segoe UI", 11F);
            this.lblIpCaption.ForeColor = System.Drawing.Color.Silver;
            this.lblIpCaption.Location  = new System.Drawing.Point(20, 20);
            this.lblIpCaption.Size      = new System.Drawing.Size(160, 28);
            this.lblIpCaption.Text      = "Your IP Address:";

            // ---- lblIpValue ----
            this.lblIpValue.AutoSize  = false;
            this.lblIpValue.Font      = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblIpValue.ForeColor = System.Drawing.Color.Cyan;
            this.lblIpValue.Location  = new System.Drawing.Point(20, 52);
            this.lblIpValue.Size      = new System.Drawing.Size(280, 36);
            this.lblIpValue.Text      = "Detecting…";

            // ---- lblPort ----
            this.lblPort.AutoSize  = false;
            this.lblPort.Font      = new System.Drawing.Font("Segoe UI", 11F);
            this.lblPort.ForeColor = System.Drawing.Color.Silver;
            this.lblPort.Location  = new System.Drawing.Point(20, 96);
            this.lblPort.Size      = new System.Drawing.Size(200, 28);
            this.lblPort.Text      = "Port: 5000";

            // ---- lblPinCaption ----
            this.lblPinCaption.AutoSize  = false;
            this.lblPinCaption.Font      = new System.Drawing.Font("Segoe UI", 11F);
            this.lblPinCaption.ForeColor = System.Drawing.Color.Silver;
            this.lblPinCaption.Location  = new System.Drawing.Point(20, 132);
            this.lblPinCaption.Size      = new System.Drawing.Size(160, 28);
            this.lblPinCaption.Text      = "Connection PIN:";

            // ---- lblPinValue ----
            this.lblPinValue.AutoSize  = false;
            this.lblPinValue.Font      = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.lblPinValue.ForeColor = System.Drawing.Color.Yellow;
            this.lblPinValue.Location  = new System.Drawing.Point(20, 164);
            this.lblPinValue.Size      = new System.Drawing.Size(200, 50);
            this.lblPinValue.Text      = "------";

            // ---- lblPinNote ----
            this.lblPinNote.AutoSize  = false;
            this.lblPinNote.Font      = new System.Drawing.Font("Segoe UI", 9F);
            this.lblPinNote.ForeColor = System.Drawing.Color.FromArgb(160, 160, 160);
            this.lblPinNote.Location  = new System.Drawing.Point(20, 218);
            this.lblPinNote.Size      = new System.Drawing.Size(360, 20);
            this.lblPinNote.Text      = "Share this PIN with the viewer to allow connection.";

            // ---- btnStart ----
            this.btnStart.BackColor = System.Drawing.Color.FromArgb(0, 153, 76);
            this.btnStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStart.Font      = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnStart.ForeColor = System.Drawing.Color.White;
            this.btnStart.Location  = new System.Drawing.Point(20, 252);
            this.btnStart.Size      = new System.Drawing.Size(150, 40);
            this.btnStart.Text      = "Start Hosting";
            this.btnStart.Click    += new System.EventHandler(this.btnStart_Click);

            // ---- btnStop ----
            this.btnStop.BackColor = System.Drawing.Color.FromArgb(180, 30, 30);
            this.btnStop.Enabled   = false;
            this.btnStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStop.Font      = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnStop.ForeColor = System.Drawing.Color.White;
            this.btnStop.Location  = new System.Drawing.Point(185, 252);
            this.btnStop.Size      = new System.Drawing.Size(120, 40);
            this.btnStop.Text      = "Stop";
            this.btnStop.Click    += new System.EventHandler(this.btnStop_Click);

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

            // ---- HostForm ----
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor           = System.Drawing.Color.FromArgb(40, 40, 60);
            this.ClientSize          = new System.Drawing.Size(430, 370);
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlStatusBar);
            this.Controls.Add(this.pnlHeader);
            this.FormBorderStyle     = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox         = false;
            this.Name                = "HostForm";
            this.StartPosition       = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text                = "LanOra – Host";
            this.FormClosing        += new System.Windows.Forms.FormClosingEventHandler(this.HostForm_FormClosing);

            this.pnlHeader.ResumeLayout(false);
            this.pnlContent.ResumeLayout(false);
            this.pnlStatusBar.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel  pnlHeader;
        private System.Windows.Forms.Label  lblTitle;
        private System.Windows.Forms.Panel  pnlContent;
        private System.Windows.Forms.Label  lblIpCaption;
        private System.Windows.Forms.Label  lblIpValue;
        private System.Windows.Forms.Label  lblPort;
        private System.Windows.Forms.Label  lblPinCaption;
        private System.Windows.Forms.Label  lblPinValue;
        private System.Windows.Forms.Label  lblPinNote;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Panel  pnlStatusBar;
        private System.Windows.Forms.Label  lblStatusDot;
        private System.Windows.Forms.Label  lblStatus;
    }
}
