using LanOra.Theme;

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
            // ---- Controls ----
            this.pnlTitleBar    = new System.Windows.Forms.Panel();
            this.lblAppName     = new System.Windows.Forms.Label();
            this.btnClose       = new System.Windows.Forms.Button();
            this.btnMinimize    = new System.Windows.Forms.Button();
            this.pnlContent     = new System.Windows.Forms.Panel();
            this.lblTitle       = new System.Windows.Forms.Label();
            this.lblSubtitle    = new System.Windows.Forms.Label();
            this.pnlCardsArea   = new System.Windows.Forms.Panel();
            this.pnlHostCard    = new System.Windows.Forms.Panel();
            this.lblHostIcon    = new System.Windows.Forms.Label();
            this.lblHostTitle   = new System.Windows.Forms.Label();
            this.lblHostDesc    = new System.Windows.Forms.Label();
            this.pnlViewerCard  = new System.Windows.Forms.Panel();
            this.lblViewerIcon  = new System.Windows.Forms.Label();
            this.lblViewerTitle = new System.Windows.Forms.Label();
            this.lblViewDesc    = new System.Windows.Forms.Label();
            this.pnlStatusBar   = new System.Windows.Forms.Panel();
            this.lblStatusBar   = new System.Windows.Forms.Label();

            this.pnlTitleBar.SuspendLayout();
            this.pnlContent.SuspendLayout();
            this.pnlCardsArea.SuspendLayout();
            this.pnlHostCard.SuspendLayout();
            this.pnlViewerCard.SuspendLayout();
            this.pnlStatusBar.SuspendLayout();
            this.SuspendLayout();

            // ----------------------------------------------------------------
            // pnlTitleBar – draggable custom title bar
            // ----------------------------------------------------------------
            this.pnlTitleBar.BackColor = AppTheme.TitleBar;
            this.pnlTitleBar.Dock      = System.Windows.Forms.DockStyle.Top;
            this.pnlTitleBar.Height    = 44;
            this.pnlTitleBar.Controls.Add(this.btnClose);
            this.pnlTitleBar.Controls.Add(this.btnMinimize);
            this.pnlTitleBar.Controls.Add(this.lblAppName);
            this.pnlTitleBar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TitleBar_MouseDown);
            this.pnlTitleBar.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TitleBar_MouseMove);

            // lblAppName
            this.lblAppName.AutoSize  = false;
            this.lblAppName.Font      = new System.Drawing.Font(AppTheme.FontFamily, 12F, System.Drawing.FontStyle.Bold);
            this.lblAppName.ForeColor = AppTheme.TextPrimary;
            this.lblAppName.Location  = new System.Drawing.Point(12, 0);
            this.lblAppName.Size      = new System.Drawing.Size(200, 44);
            this.lblAppName.Text      = "LanOra";
            this.lblAppName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblAppName.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TitleBar_MouseDown);
            this.lblAppName.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TitleBar_MouseMove);

            // btnClose
            this.btnClose.BackColor               = AppTheme.TitleBar;
            this.btnClose.FlatStyle               = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatAppearance.MouseOverBackColor = AppTheme.ErrorRed;
            this.btnClose.Font      = new System.Drawing.Font(AppTheme.FontFamily, 11F);
            this.btnClose.ForeColor = AppTheme.TextSecondary;
            this.btnClose.Location  = new System.Drawing.Point(514, 0);
            this.btnClose.Size      = new System.Drawing.Size(46, 44);
            this.btnClose.Text      = "✕";
            this.btnClose.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.btnClose.Click    += new System.EventHandler(this.btnClose_Click);

            // btnMinimize
            this.btnMinimize.BackColor               = AppTheme.TitleBar;
            this.btnMinimize.FlatStyle               = System.Windows.Forms.FlatStyle.Flat;
            this.btnMinimize.FlatAppearance.BorderSize = 0;
            this.btnMinimize.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(0x40, 0x40, 0x40);
            this.btnMinimize.Font      = new System.Drawing.Font(AppTheme.FontFamily, 11F);
            this.btnMinimize.ForeColor = AppTheme.TextSecondary;
            this.btnMinimize.Location  = new System.Drawing.Point(468, 0);
            this.btnMinimize.Size      = new System.Drawing.Size(46, 44);
            this.btnMinimize.Text      = "─";
            this.btnMinimize.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.btnMinimize.Click    += new System.EventHandler(this.btnMinimize_Click);

            // ----------------------------------------------------------------
            // pnlStatusBar – bottom status strip
            // ----------------------------------------------------------------
            this.pnlStatusBar.BackColor = AppTheme.StatusBar;
            this.pnlStatusBar.Dock      = System.Windows.Forms.DockStyle.Bottom;
            this.pnlStatusBar.Height    = 26;
            this.pnlStatusBar.Controls.Add(this.lblStatusBar);

            // lblStatusBar
            this.lblStatusBar.AutoSize  = false;
            this.lblStatusBar.Dock      = System.Windows.Forms.DockStyle.Fill;
            this.lblStatusBar.Font      = new System.Drawing.Font(AppTheme.FontFamily, 8F);
            this.lblStatusBar.ForeColor = AppTheme.TextPrimary;
            this.lblStatusBar.Padding   = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.lblStatusBar.Text      = "Role Selection  |  " + AppTheme.Developer;
            this.lblStatusBar.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // ----------------------------------------------------------------
            // pnlContent – main body
            // ----------------------------------------------------------------
            this.pnlContent.BackColor = AppTheme.Background;
            this.pnlContent.Dock      = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.Controls.Add(this.pnlCardsArea);
            this.pnlContent.Controls.Add(this.lblSubtitle);
            this.pnlContent.Controls.Add(this.lblTitle);

            // lblTitle
            this.lblTitle.AutoSize  = false;
            this.lblTitle.Font      = new System.Drawing.Font(AppTheme.FontFamily, 18F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = AppTheme.TextPrimary;
            this.lblTitle.Location  = new System.Drawing.Point(0, 18);
            this.lblTitle.Size      = new System.Drawing.Size(560, 40);
            this.lblTitle.Text      = "Select Role";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // lblSubtitle
            this.lblSubtitle.AutoSize  = false;
            this.lblSubtitle.Font      = new System.Drawing.Font(AppTheme.FontFamily, 9.5F);
            this.lblSubtitle.ForeColor = AppTheme.TextSecondary;
            this.lblSubtitle.Location  = new System.Drawing.Point(0, 60);
            this.lblSubtitle.Size      = new System.Drawing.Size(560, 28);
            this.lblSubtitle.Text      = "Run LanOra on all computers. Choose this computer\u2019s role:";
            this.lblSubtitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // ----------------------------------------------------------------
            // pnlCardsArea – contains the two side-by-side role cards
            // ----------------------------------------------------------------
            this.pnlCardsArea.BackColor = AppTheme.Background;
            this.pnlCardsArea.Location  = new System.Drawing.Point(50, 96);
            this.pnlCardsArea.Size      = new System.Drawing.Size(460, 210);
            this.pnlCardsArea.Controls.Add(this.pnlHostCard);
            this.pnlCardsArea.Controls.Add(this.pnlViewerCard);

            // ----------------------------------------------------------------
            // pnlHostCard – HOST card
            // ----------------------------------------------------------------
            this.pnlHostCard.BackColor = AppTheme.Panel;
            this.pnlHostCard.Location  = new System.Drawing.Point(0, 0);
            this.pnlHostCard.Size      = new System.Drawing.Size(210, 210);
            this.pnlHostCard.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.pnlHostCard.Controls.Add(this.lblHostDesc);
            this.pnlHostCard.Controls.Add(this.lblHostTitle);
            this.pnlHostCard.Controls.Add(this.lblHostIcon);
            this.pnlHostCard.Paint       += new System.Windows.Forms.PaintEventHandler(this.HostCard_Paint);
            this.pnlHostCard.MouseEnter  += new System.EventHandler(this.HostCard_MouseEnter);
            this.pnlHostCard.MouseLeave  += new System.EventHandler(this.HostCard_MouseLeave);
            this.pnlHostCard.Click       += new System.EventHandler(this.HostCard_Click);

            // lblHostIcon
            this.lblHostIcon.AutoSize  = false;
            this.lblHostIcon.Font      = new System.Drawing.Font(AppTheme.FontFamily, 28F);
            this.lblHostIcon.ForeColor = AppTheme.AccentBlue;
            this.lblHostIcon.Location  = new System.Drawing.Point(0, 20);
            this.lblHostIcon.Size      = new System.Drawing.Size(210, 56);
            this.lblHostIcon.Text      = "\u25CE";
            this.lblHostIcon.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblHostIcon.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.lblHostIcon.MouseEnter += new System.EventHandler(this.HostCard_MouseEnter);
            this.lblHostIcon.MouseLeave += new System.EventHandler(this.HostCard_MouseLeave);
            this.lblHostIcon.Click      += new System.EventHandler(this.HostCard_Click);

            // lblHostTitle
            this.lblHostTitle.AutoSize  = false;
            this.lblHostTitle.Font      = new System.Drawing.Font(AppTheme.FontFamily, 15F, System.Drawing.FontStyle.Bold);
            this.lblHostTitle.ForeColor = AppTheme.TextPrimary;
            this.lblHostTitle.Location  = new System.Drawing.Point(0, 80);
            this.lblHostTitle.Size      = new System.Drawing.Size(210, 36);
            this.lblHostTitle.Text      = "HOST";
            this.lblHostTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblHostTitle.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.lblHostTitle.MouseEnter += new System.EventHandler(this.HostCard_MouseEnter);
            this.lblHostTitle.MouseLeave += new System.EventHandler(this.HostCard_MouseLeave);
            this.lblHostTitle.Click      += new System.EventHandler(this.HostCard_Click);

            // lblHostDesc
            this.lblHostDesc.AutoSize  = false;
            this.lblHostDesc.Font      = new System.Drawing.Font(AppTheme.FontFamily, 8.5F);
            this.lblHostDesc.ForeColor = AppTheme.TextSecondary;
            this.lblHostDesc.Location  = new System.Drawing.Point(10, 122);
            this.lblHostDesc.Size      = new System.Drawing.Size(190, 50);
            this.lblHostDesc.Text      = "Share this computer\u2019s screen with others";
            this.lblHostDesc.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblHostDesc.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.lblHostDesc.MouseEnter += new System.EventHandler(this.HostCard_MouseEnter);
            this.lblHostDesc.MouseLeave += new System.EventHandler(this.HostCard_MouseLeave);
            this.lblHostDesc.Click      += new System.EventHandler(this.HostCard_Click);

            // ----------------------------------------------------------------
            // pnlViewerCard – VIEWER card
            // ----------------------------------------------------------------
            this.pnlViewerCard.BackColor = AppTheme.Panel;
            this.pnlViewerCard.Location  = new System.Drawing.Point(250, 0);
            this.pnlViewerCard.Size      = new System.Drawing.Size(210, 210);
            this.pnlViewerCard.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.pnlViewerCard.Controls.Add(this.lblViewDesc);
            this.pnlViewerCard.Controls.Add(this.lblViewerTitle);
            this.pnlViewerCard.Controls.Add(this.lblViewerIcon);
            this.pnlViewerCard.Paint       += new System.Windows.Forms.PaintEventHandler(this.ViewerCard_Paint);
            this.pnlViewerCard.MouseEnter  += new System.EventHandler(this.ViewerCard_MouseEnter);
            this.pnlViewerCard.MouseLeave  += new System.EventHandler(this.ViewerCard_MouseLeave);
            this.pnlViewerCard.Click       += new System.EventHandler(this.ViewerCard_Click);

            // lblViewerIcon
            this.lblViewerIcon.AutoSize  = false;
            this.lblViewerIcon.Font      = new System.Drawing.Font(AppTheme.FontFamily, 28F);
            this.lblViewerIcon.ForeColor = AppTheme.SuccessGreen;
            this.lblViewerIcon.Location  = new System.Drawing.Point(0, 20);
            this.lblViewerIcon.Size      = new System.Drawing.Size(210, 56);
            this.lblViewerIcon.Text      = "\u25A3";
            this.lblViewerIcon.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblViewerIcon.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.lblViewerIcon.MouseEnter += new System.EventHandler(this.ViewerCard_MouseEnter);
            this.lblViewerIcon.MouseLeave += new System.EventHandler(this.ViewerCard_MouseLeave);
            this.lblViewerIcon.Click      += new System.EventHandler(this.ViewerCard_Click);

            // lblViewerTitle
            this.lblViewerTitle.AutoSize  = false;
            this.lblViewerTitle.Font      = new System.Drawing.Font(AppTheme.FontFamily, 15F, System.Drawing.FontStyle.Bold);
            this.lblViewerTitle.ForeColor = AppTheme.TextPrimary;
            this.lblViewerTitle.Location  = new System.Drawing.Point(0, 80);
            this.lblViewerTitle.Size      = new System.Drawing.Size(210, 36);
            this.lblViewerTitle.Text      = "VIEWER";
            this.lblViewerTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblViewerTitle.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.lblViewerTitle.MouseEnter += new System.EventHandler(this.ViewerCard_MouseEnter);
            this.lblViewerTitle.MouseLeave += new System.EventHandler(this.ViewerCard_MouseLeave);
            this.lblViewerTitle.Click      += new System.EventHandler(this.ViewerCard_Click);

            // lblViewDesc
            this.lblViewDesc.AutoSize  = false;
            this.lblViewDesc.Font      = new System.Drawing.Font(AppTheme.FontFamily, 8.5F);
            this.lblViewDesc.ForeColor = AppTheme.TextSecondary;
            this.lblViewDesc.Location  = new System.Drawing.Point(10, 122);
            this.lblViewDesc.Size      = new System.Drawing.Size(190, 50);
            this.lblViewDesc.Text      = "Connect to and monitor another computer";
            this.lblViewDesc.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblViewDesc.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.lblViewDesc.MouseEnter += new System.EventHandler(this.ViewerCard_MouseEnter);
            this.lblViewDesc.MouseLeave += new System.EventHandler(this.ViewerCard_MouseLeave);
            this.lblViewDesc.Click      += new System.EventHandler(this.ViewerCard_Click);

            // ----------------------------------------------------------------
            // RoleSelectForm
            // ----------------------------------------------------------------
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor           = AppTheme.Background;
            this.ClientSize          = new System.Drawing.Size(560, 390);
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlStatusBar);
            this.Controls.Add(this.pnlTitleBar);
            this.FormBorderStyle     = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox         = false;
            this.Name                = "RoleSelectForm";
            this.StartPosition       = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text                = "LanOra";

            this.pnlTitleBar.ResumeLayout(false);
            this.pnlContent.ResumeLayout(false);
            this.pnlCardsArea.ResumeLayout(false);
            this.pnlHostCard.ResumeLayout(false);
            this.pnlViewerCard.ResumeLayout(false);
            this.pnlStatusBar.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel  pnlTitleBar;
        private System.Windows.Forms.Label  lblAppName;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnMinimize;
        private System.Windows.Forms.Panel  pnlContent;
        private System.Windows.Forms.Label  lblTitle;
        private System.Windows.Forms.Label  lblSubtitle;
        private System.Windows.Forms.Panel  pnlCardsArea;
        private System.Windows.Forms.Panel  pnlHostCard;
        private System.Windows.Forms.Label  lblHostIcon;
        private System.Windows.Forms.Label  lblHostTitle;
        private System.Windows.Forms.Label  lblHostDesc;
        private System.Windows.Forms.Panel  pnlViewerCard;
        private System.Windows.Forms.Label  lblViewerIcon;
        private System.Windows.Forms.Label  lblViewerTitle;
        private System.Windows.Forms.Label  lblViewDesc;
        private System.Windows.Forms.Panel  pnlStatusBar;
        private System.Windows.Forms.Label  lblStatusBar;
    }
}
