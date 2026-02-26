namespace LanOra.Forms
{
    partial class RoleSelectForm
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
            this.pnlHeader   = new System.Windows.Forms.Panel();
            this.lblTitle    = new System.Windows.Forms.Label();
            this.pnlContent  = new System.Windows.Forms.Panel();
            this.lblSubtitle = new System.Windows.Forms.Label();
            this.btnHost     = new System.Windows.Forms.Button();
            this.lblHostDesc = new System.Windows.Forms.Label();
            this.btnViewer   = new System.Windows.Forms.Button();
            this.lblViewDesc = new System.Windows.Forms.Label();
            this.pnlHeader.SuspendLayout();
            this.pnlContent.SuspendLayout();
            this.SuspendLayout();

            // ---- pnlHeader ----
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(30, 30, 45);
            this.pnlHeader.Dock      = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Height    = 70;
            this.pnlHeader.Controls.Add(this.lblTitle);

            // ---- lblTitle ----
            this.lblTitle.AutoSize  = false;
            this.lblTitle.Dock      = System.Windows.Forms.DockStyle.Fill;
            this.lblTitle.Font      = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Text      = "LanOra";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // ---- pnlContent ----
            this.pnlContent.BackColor = System.Drawing.Color.FromArgb(40, 40, 60);
            this.pnlContent.Dock      = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.Controls.Add(this.lblViewDesc);
            this.pnlContent.Controls.Add(this.lblHostDesc);
            this.pnlContent.Controls.Add(this.btnViewer);
            this.pnlContent.Controls.Add(this.btnHost);
            this.pnlContent.Controls.Add(this.lblSubtitle);

            // ---- lblSubtitle ----
            this.lblSubtitle.AutoSize  = false;
            this.lblSubtitle.Font      = new System.Drawing.Font("Segoe UI", 11F);
            this.lblSubtitle.ForeColor = System.Drawing.Color.Silver;
            this.lblSubtitle.Location  = new System.Drawing.Point(0, 12);
            this.lblSubtitle.Size      = new System.Drawing.Size(500, 28);
            this.lblSubtitle.Text      = "Select your role to get started";
            this.lblSubtitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // ---- btnHost ----
            this.btnHost.BackColor = System.Drawing.Color.FromArgb(0, 153, 76);
            this.btnHost.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHost.Font      = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.btnHost.ForeColor = System.Drawing.Color.White;
            this.btnHost.Location  = new System.Drawing.Point(40, 58);
            this.btnHost.Size      = new System.Drawing.Size(185, 60);
            this.btnHost.Text      = "Host";
            this.btnHost.Click    += new System.EventHandler(this.btnHost_Click);

            // ---- lblHostDesc ----
            this.lblHostDesc.AutoSize  = false;
            this.lblHostDesc.Font      = new System.Drawing.Font("Segoe UI", 9F);
            this.lblHostDesc.ForeColor = System.Drawing.Color.Silver;
            this.lblHostDesc.Location  = new System.Drawing.Point(40, 123);
            this.lblHostDesc.Size      = new System.Drawing.Size(185, 36);
            this.lblHostDesc.Text      = "Share your screen with viewers on the network";
            this.lblHostDesc.TextAlign = System.Drawing.ContentAlignment.TopCenter;

            // ---- btnViewer ----
            this.btnViewer.BackColor = System.Drawing.Color.FromArgb(0, 102, 204);
            this.btnViewer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnViewer.Font      = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.btnViewer.ForeColor = System.Drawing.Color.White;
            this.btnViewer.Location  = new System.Drawing.Point(275, 58);
            this.btnViewer.Size      = new System.Drawing.Size(185, 60);
            this.btnViewer.Text      = "Viewer";
            this.btnViewer.Click    += new System.EventHandler(this.btnViewer_Click);

            // ---- lblViewDesc ----
            this.lblViewDesc.AutoSize  = false;
            this.lblViewDesc.Font      = new System.Drawing.Font("Segoe UI", 9F);
            this.lblViewDesc.ForeColor = System.Drawing.Color.Silver;
            this.lblViewDesc.Location  = new System.Drawing.Point(275, 123);
            this.lblViewDesc.Size      = new System.Drawing.Size(185, 36);
            this.lblViewDesc.Text      = "View a shared screen from another machine";
            this.lblViewDesc.TextAlign = System.Drawing.ContentAlignment.TopCenter;

            // ---- RoleSelectForm ----
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor           = System.Drawing.Color.FromArgb(40, 40, 60);
            this.ClientSize          = new System.Drawing.Size(500, 240);
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlHeader);
            this.FormBorderStyle     = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox         = false;
            this.Name                = "RoleSelectForm";
            this.StartPosition       = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text                = "LanOra – Screen Sharing";

            this.pnlHeader.ResumeLayout(false);
            this.pnlContent.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel  pnlHeader;
        private System.Windows.Forms.Label  lblTitle;
        private System.Windows.Forms.Panel  pnlContent;
        private System.Windows.Forms.Label  lblSubtitle;
        private System.Windows.Forms.Button btnHost;
        private System.Windows.Forms.Label  lblHostDesc;
        private System.Windows.Forms.Button btnViewer;
        private System.Windows.Forms.Label  lblViewDesc;
    }
}
