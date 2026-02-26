using System;
using System.Drawing;
using System.Windows.Forms;
using LanOra.Theme;

namespace LanOra.Forms
{
    /// <summary>
    /// Startup screen. The user selects whether to act as a Host (share screen)
    /// or a Viewer (view a remote screen).
    /// Custom title-bar, draggable form, card-style role buttons with hover glow.
    /// </summary>
    public partial class RoleSelectForm : Form
    {
        // Tracks which card the cursor is over so Paint can draw the correct border.
        private bool _hostCardHovered;
        private bool _viewerCardHovered;

        // Title-bar drag support
        private Point _dragOffset;

        public RoleSelectForm()
        {
            InitializeComponent();
            DoubleBuffered = true;
        }

        // ------------------------------------------------------------------ //
        // Title-bar drag                                                      //
        // ------------------------------------------------------------------ //

        private void TitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                _dragOffset = e.Location;
        }

        private void TitleBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                Location = new Point(
                    Location.X + e.X - _dragOffset.X,
                    Location.Y + e.Y - _dragOffset.Y);
        }

        // ------------------------------------------------------------------ //
        // Title-bar buttons                                                   //
        // ------------------------------------------------------------------ //

        private void btnClose_Click(object sender, EventArgs e)    => Close();
        private void btnMinimize_Click(object sender, EventArgs e) => WindowState = FormWindowState.Minimized;

        // ------------------------------------------------------------------ //
        // HOST card                                                           //
        // ------------------------------------------------------------------ //

        private void HostCard_MouseEnter(object sender, EventArgs e)
        {
            _hostCardHovered = true;
            pnlHostCard.Invalidate();
        }

        private void HostCard_MouseLeave(object sender, EventArgs e)
        {
            // Ignore if still inside the card (child control raised the event)
            if (pnlHostCard.ClientRectangle.Contains(pnlHostCard.PointToClient(Cursor.Position)))
                return;
            _hostCardHovered = false;
            pnlHostCard.Invalidate();
        }

        private void HostCard_Paint(object sender, PaintEventArgs e)
        {
            Color border = _hostCardHovered ? AppTheme.AccentBlue : AppTheme.CardBorder;
            int   width  = _hostCardHovered ? 3 : 2;
            using (Pen pen = new Pen(border, width))
                e.Graphics.DrawRectangle(pen, 1, 1, pnlHostCard.Width - 3, pnlHostCard.Height - 3);
        }

        private void HostCard_Click(object sender, EventArgs e)
        {
            Hide();
            using (HostForm form = new HostForm())
                form.ShowDialog(this);
            Show();
        }

        // ------------------------------------------------------------------ //
        // VIEWER card                                                         //
        // ------------------------------------------------------------------ //

        private void ViewerCard_MouseEnter(object sender, EventArgs e)
        {
            _viewerCardHovered = true;
            pnlViewerCard.Invalidate();
        }

        private void ViewerCard_MouseLeave(object sender, EventArgs e)
        {
            if (pnlViewerCard.ClientRectangle.Contains(pnlViewerCard.PointToClient(Cursor.Position)))
                return;
            _viewerCardHovered = false;
            pnlViewerCard.Invalidate();
        }

        private void ViewerCard_Paint(object sender, PaintEventArgs e)
        {
            Color border = _viewerCardHovered ? AppTheme.SuccessGreen : AppTheme.CardBorder;
            int   width  = _viewerCardHovered ? 3 : 2;
            using (Pen pen = new Pen(border, width))
                e.Graphics.DrawRectangle(pen, 1, 1, pnlViewerCard.Width - 3, pnlViewerCard.Height - 3);
        }

        private void ViewerCard_Click(object sender, EventArgs e)
        {
            Hide();
            using (ViewerForm form = new ViewerForm())
                form.ShowDialog(this);
            Show();
        }
    }
}
