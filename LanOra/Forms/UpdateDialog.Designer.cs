namespace LanOra.Forms
{
    partial class UpdateDialog
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlTitleBar        = new System.Windows.Forms.Panel();
            this.lblTitle           = new System.Windows.Forms.Label();
            this.btnClose           = new System.Windows.Forms.Button();
            this.pnlBody            = new System.Windows.Forms.Panel();
            this.lblHeading         = new System.Windows.Forms.Label();
            this.lblCurrentVersion  = new System.Windows.Forms.Label();
            this.lblNewVersion      = new System.Windows.Forms.Label();
            this.lblMandatory       = new System.Windows.Forms.Label();
            this.lblNotesHeader     = new System.Windows.Forms.Label();
            this.txtReleaseNotes    = new System.Windows.Forms.TextBox();
            this.progressBar        = new System.Windows.Forms.ProgressBar();
            this.lblStatus          = new System.Windows.Forms.Label();
            this.pnlButtons         = new System.Windows.Forms.Panel();
            this.btnUpdate          = new System.Windows.Forms.Button();
            this.btnSkip            = new System.Windows.Forms.Button();
            this.pnlTitleBar.SuspendLayout();
            this.pnlBody.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.SuspendLayout();

            // ---------------------------------------------------------------
            // Form
            // ---------------------------------------------------------------
            this.ClientSize         = new System.Drawing.Size(480, 400);
            this.FormBorderStyle    = System.Windows.Forms.FormBorderStyle.None;
            this.StartPosition      = System.Windows.Forms.FormStartPosition.CenterParent;
            this.BackColor          = LanOra.Theme.AppTheme.Background;
            this.Text               = "LanOra Update Available";
            this.MinimumSize        = new System.Drawing.Size(480, 400);
            this.Controls.Add(this.pnlTitleBar);
            this.Controls.Add(this.pnlBody);
            this.Controls.Add(this.pnlButtons);

            // ---------------------------------------------------------------
            // Title bar
            // ---------------------------------------------------------------
            this.pnlTitleBar.BackColor   = LanOra.Theme.AppTheme.TitleBar;
            this.pnlTitleBar.Dock        = System.Windows.Forms.DockStyle.Top;
            this.pnlTitleBar.Height      = 36;
            this.pnlTitleBar.Controls.Add(this.lblTitle);
            this.pnlTitleBar.Controls.Add(this.btnClose);
            this.pnlTitleBar.MouseDown  += new System.Windows.Forms.MouseEventHandler(this.TitleBar_MouseDown);
            this.pnlTitleBar.MouseMove  += new System.Windows.Forms.MouseEventHandler(this.TitleBar_MouseMove);

            this.lblTitle.AutoSize   = false;
            this.lblTitle.Dock       = System.Windows.Forms.DockStyle.Fill;
            this.lblTitle.ForeColor  = LanOra.Theme.AppTheme.TextPrimary;
            this.lblTitle.Font       = new System.Drawing.Font("Segoe UI", 10f, System.Drawing.FontStyle.Bold);
            this.lblTitle.Text       = "  Update Available";
            this.lblTitle.TextAlign  = System.Drawing.ContentAlignment.MiddleLeft;

            this.btnClose.FlatStyle   = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.BackColor   = System.Drawing.Color.Transparent;
            this.btnClose.ForeColor   = LanOra.Theme.AppTheme.TextPrimary;
            this.btnClose.Font        = new System.Drawing.Font("Segoe UI", 11f, System.Drawing.FontStyle.Bold);
            this.btnClose.Text        = "✕";
            this.btnClose.Size        = new System.Drawing.Size(36, 36);
            this.btnClose.Dock        = System.Windows.Forms.DockStyle.Right;
            this.btnClose.Cursor      = System.Windows.Forms.Cursors.Hand;
            this.btnClose.Click      += new System.EventHandler(this.btnClose_Click);

            // ---------------------------------------------------------------
            // Body panel
            // ---------------------------------------------------------------
            this.pnlBody.Dock        = System.Windows.Forms.DockStyle.Fill;
            this.pnlBody.Padding     = new System.Windows.Forms.Padding(20, 10, 20, 0);
            this.pnlBody.Controls.Add(this.lblHeading);
            this.pnlBody.Controls.Add(this.lblCurrentVersion);
            this.pnlBody.Controls.Add(this.lblNewVersion);
            this.pnlBody.Controls.Add(this.lblMandatory);
            this.pnlBody.Controls.Add(this.lblNotesHeader);
            this.pnlBody.Controls.Add(this.txtReleaseNotes);
            this.pnlBody.Controls.Add(this.progressBar);
            this.pnlBody.Controls.Add(this.lblStatus);

            this.lblHeading.AutoSize  = false;
            this.lblHeading.Dock      = System.Windows.Forms.DockStyle.Top;
            this.lblHeading.Height    = 36;
            this.lblHeading.ForeColor = LanOra.Theme.AppTheme.AccentBlue;
            this.lblHeading.Font      = new System.Drawing.Font("Segoe UI", 13f, System.Drawing.FontStyle.Bold);
            this.lblHeading.Text      = "A new version of LanOra is available!";
            this.lblHeading.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.lblCurrentVersion.AutoSize  = false;
            this.lblCurrentVersion.Dock      = System.Windows.Forms.DockStyle.Top;
            this.lblCurrentVersion.Height    = 22;
            this.lblCurrentVersion.ForeColor = LanOra.Theme.AppTheme.TextSecondary;
            this.lblCurrentVersion.Font      = new System.Drawing.Font("Segoe UI", 9f);
            this.lblCurrentVersion.Text      = "Current version:";

            this.lblNewVersion.AutoSize   = false;
            this.lblNewVersion.Dock       = System.Windows.Forms.DockStyle.Top;
            this.lblNewVersion.Height     = 22;
            this.lblNewVersion.ForeColor  = LanOra.Theme.AppTheme.SuccessGreen;
            this.lblNewVersion.Font       = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Bold);
            this.lblNewVersion.Text       = "New version:";

            this.lblMandatory.AutoSize   = false;
            this.lblMandatory.Dock       = System.Windows.Forms.DockStyle.Top;
            this.lblMandatory.Height     = 22;
            this.lblMandatory.ForeColor  = LanOra.Theme.AppTheme.ErrorRed;
            this.lblMandatory.Font       = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Bold);
            this.lblMandatory.Text       = "⚠  This update is mandatory.";
            this.lblMandatory.Visible    = false;

            this.lblNotesHeader.AutoSize  = false;
            this.lblNotesHeader.Dock      = System.Windows.Forms.DockStyle.Top;
            this.lblNotesHeader.Height    = 22;
            this.lblNotesHeader.ForeColor = LanOra.Theme.AppTheme.TextPrimary;
            this.lblNotesHeader.Font      = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Bold);
            this.lblNotesHeader.Text      = "What's new:";

            this.txtReleaseNotes.Dock        = System.Windows.Forms.DockStyle.Top;
            this.txtReleaseNotes.Height      = 80;
            this.txtReleaseNotes.Multiline   = true;
            this.txtReleaseNotes.ReadOnly    = true;
            this.txtReleaseNotes.ScrollBars  = System.Windows.Forms.ScrollBars.Vertical;
            this.txtReleaseNotes.BackColor   = LanOra.Theme.AppTheme.Panel;
            this.txtReleaseNotes.ForeColor   = LanOra.Theme.AppTheme.TextSecondary;
            this.txtReleaseNotes.Font        = new System.Drawing.Font("Segoe UI", 9f);
            this.txtReleaseNotes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            this.progressBar.Dock    = System.Windows.Forms.DockStyle.Bottom;
            this.progressBar.Height  = 18;
            this.progressBar.Minimum = 0;
            this.progressBar.Maximum = 100;
            this.progressBar.Visible = false;

            this.lblStatus.AutoSize   = false;
            this.lblStatus.Dock       = System.Windows.Forms.DockStyle.Bottom;
            this.lblStatus.Height     = 20;
            this.lblStatus.ForeColor  = LanOra.Theme.AppTheme.TextSecondary;
            this.lblStatus.Font       = new System.Drawing.Font("Segoe UI", 8.5f);
            this.lblStatus.Text       = string.Empty;
            this.lblStatus.Visible    = false;

            // ---------------------------------------------------------------
            // Button bar
            // ---------------------------------------------------------------
            this.pnlButtons.Dock       = System.Windows.Forms.DockStyle.Bottom;
            this.pnlButtons.Height     = 54;
            this.pnlButtons.Padding    = new System.Windows.Forms.Padding(16, 10, 16, 10);
            this.pnlButtons.BackColor  = LanOra.Theme.AppTheme.Background;
            this.pnlButtons.Controls.Add(this.btnSkip);
            this.pnlButtons.Controls.Add(this.btnUpdate);

            this.btnUpdate.FlatStyle             = System.Windows.Forms.FlatStyle.Flat;
            this.btnUpdate.FlatAppearance.BorderColor = LanOra.Theme.AppTheme.AccentBlue;
            this.btnUpdate.BackColor             = LanOra.Theme.AppTheme.AccentBlue;
            this.btnUpdate.ForeColor             = System.Drawing.Color.White;
            this.btnUpdate.Font                  = new System.Drawing.Font("Segoe UI", 9.5f, System.Drawing.FontStyle.Bold);
            this.btnUpdate.Text                  = "Update Now";
            this.btnUpdate.Size                  = new System.Drawing.Size(120, 34);
            this.btnUpdate.Dock                  = System.Windows.Forms.DockStyle.Right;
            this.btnUpdate.Cursor                = System.Windows.Forms.Cursors.Hand;
            this.btnUpdate.Click                += new System.EventHandler(this.btnUpdate_Click);

            this.btnSkip.FlatStyle               = System.Windows.Forms.FlatStyle.Flat;
            this.btnSkip.FlatAppearance.BorderColor = LanOra.Theme.AppTheme.CardBorder;
            this.btnSkip.BackColor               = LanOra.Theme.AppTheme.Panel;
            this.btnSkip.ForeColor               = LanOra.Theme.AppTheme.TextSecondary;
            this.btnSkip.Font                    = new System.Drawing.Font("Segoe UI", 9.5f);
            this.btnSkip.Text                    = "Skip This Version";
            this.btnSkip.Size                    = new System.Drawing.Size(140, 34);
            this.btnSkip.Dock                    = System.Windows.Forms.DockStyle.Left;
            this.btnSkip.Cursor                  = System.Windows.Forms.Cursors.Hand;
            this.btnSkip.Click                  += new System.EventHandler(this.btnSkip_Click);

            this.pnlTitleBar.ResumeLayout(false);
            this.pnlBody.ResumeLayout(false);
            this.pnlButtons.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        // Controls
        private System.Windows.Forms.Panel       pnlTitleBar;
        private System.Windows.Forms.Label       lblTitle;
        private System.Windows.Forms.Button      btnClose;
        private System.Windows.Forms.Panel       pnlBody;
        private System.Windows.Forms.Label       lblHeading;
        private System.Windows.Forms.Label       lblCurrentVersion;
        private System.Windows.Forms.Label       lblNewVersion;
        private System.Windows.Forms.Label       lblMandatory;
        private System.Windows.Forms.Label       lblNotesHeader;
        private System.Windows.Forms.TextBox     txtReleaseNotes;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label       lblStatus;
        private System.Windows.Forms.Panel       pnlButtons;
        private System.Windows.Forms.Button      btnUpdate;
        private System.Windows.Forms.Button      btnSkip;
    }
}
