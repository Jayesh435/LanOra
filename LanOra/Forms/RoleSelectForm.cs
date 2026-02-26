using System;
using System.Windows.Forms;

namespace LanOra.Forms
{
    /// <summary>
    /// Startup screen. The user selects whether to act as a Host (share screen)
    /// or a Viewer (view a remote screen).
    /// </summary>
    public partial class RoleSelectForm : Form
    {
        public RoleSelectForm()
        {
            InitializeComponent();
        }

        private void btnHost_Click(object sender, EventArgs e)
        {
            Hide();
            using (HostForm form = new HostForm())
                form.ShowDialog(this);
            Show();
        }

        private void btnViewer_Click(object sender, EventArgs e)
        {
            Hide();
            using (ViewerForm form = new ViewerForm())
                form.ShowDialog(this);
            Show();
        }
    }
}
