using RapPhim.Data;

namespace RapPhim
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            // Load sample data on startup
            DataManager.TaoDuLieuMau();
        }

        private void btnXepLich_Click(object? sender, EventArgs e)
        {
            using var formXepLich = new FormXepLich();
            formXepLich.ShowDialog();
        }

        private void btnDanhSach_Click(object? sender, EventArgs e)
        {
            using var formDanhSach = new FormDanhSachLich();
            formDanhSach.ShowDialog();
        }

        private void btnThoat_Click(object? sender, EventArgs e)
        {
            var confirm = MessageBox.Show("Do you want to exit?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
    }
}
