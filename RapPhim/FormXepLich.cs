using RapPhim.Data;
using RapPhim.Models;

namespace RapPhim
{
    public partial class FormXepLich : Form
    {
        private LichChieu? _lichChieuDangSua = null;

        public FormXepLich()
        {
            InitializeComponent();
            LoadDuLieu();
        }

        /// <summary>
        /// Constructor for EDIT mode
        /// </summary>
        public FormXepLich(LichChieu lichChieuCanSua) : this()
        {
            _lichChieuDangSua = lichChieuCanSua;
            lblTitle.Text = "✏️  EDIT SHOWTIME";
            btnXepLich.Text = "💾 UPDATE";
            btnXepLich.BackColor = Color.FromArgb(217, 119, 6); // Warm Amber Gold

            cboPhim.SelectedValue = lichChieuCanSua.MaPhim;
            cboPhong.SelectedValue = lichChieuCanSua.MaPhong;
            dtpNgayChieu.Value = lichChieuCanSua.GioBatDau.Date;
            dtpGioChieu.Value = lichChieuCanSua.GioBatDau;
        }

        private void LoadDuLieu()
        {
            cboPhim.DisplayMember = "TenPhim";
            cboPhim.ValueMember = "MaPhim";
            cboPhim.DataSource = new List<Phim>(DataManager.DanhSachPhim);

            cboPhong.DisplayMember = "TenPhong";
            cboPhong.ValueMember = "MaPhong";
            cboPhong.DataSource = new List<PhongChieu>(DataManager.DanhSachPhongChieu);

            CapNhatThongTin();
            LoadLichChieuCuaPhong();
        }

        private void cboPhim_SelectedIndexChanged(object? sender, EventArgs e)
        {
            CapNhatThongTin();
        }

        private void cboPhong_SelectedIndexChanged(object? sender, EventArgs e)
        {
            LoadLichChieuCuaPhong();
        }

        private void dtpNgayChieu_ValueChanged(object? sender, EventArgs e)
        {
            LoadLichChieuCuaPhong();
        }

        private void dtpGioChieu_ValueChanged(object? sender, EventArgs e)
        {
            CapNhatThongTin();
        }

        private void CapNhatThongTin()
        {
            if (cboPhim.SelectedItem is Phim phim)
            {
                lblThoiLuongValue.Text = $"{phim.ThoiLuong} min";

                DateTime gioBatDau = dtpNgayChieu.Value.Date.Add(dtpGioChieu.Value.TimeOfDay);
                DateTime gioKetThuc = gioBatDau.AddMinutes(phim.ThoiLuong);
                lblGioKetThucValue.Text = gioKetThuc.ToString("HH:mm dd/MM/yyyy");
            }
        }

        private void LoadLichChieuCuaPhong()
        {
            if (cboPhong.SelectedValue is not int maPhong) return;

            DateTime ngayChon = dtpNgayChieu.Value.Date;

            var lichCuaPhong = DataManager.DanhSachLichChieu
                .Where(l => l.MaPhong == maPhong && l.GioBatDau.Date == ngayChon)
                .OrderBy(l => l.GioBatDau)
                .Select(l => new PhongLichViewModel
                {
                    ID = l.MaLichChieu,
                    Movie = l.TenPhim,
                    Start = l.GioBatDau.ToString("HH:mm"),
                    End = l.GioKetThuc.ToString("HH:mm"),
                    Duration = $"{l.ThoiLuongPhim} min",
                    Tickets = l.SoVeDaBan
                })
                .ToList();

            dgvLichPhong.DataSource = null;
            dgvLichPhong.DataSource = lichCuaPhong;

            var tenPhong = (cboPhong.SelectedItem as PhongChieu)?.TenPhong ?? "";
            grpLichPhong.Text = $"Schedule for {tenPhong} - {ngayChon:dd/MM/yyyy} ({lichCuaPhong.Count} showtimes)";
        }

        private void btnXepLich_Click(object? sender, EventArgs e)
        {
            if (cboPhim.SelectedValue is not int maPhim || cboPhong.SelectedValue is not int maPhong)
            {
                MessageBox.Show("Please select a movie and a room!", "Missing Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DateTime gioBatDau = dtpNgayChieu.Value.Date.Add(dtpGioChieu.Value.TimeOfDay);

            if (_lichChieuDangSua != null)
            {
                var (thanhCong, thongBao) = DataManager.CapNhatLichChieu(
                    _lichChieuDangSua.MaLichChieu, maPhim, maPhong, gioBatDau);

                MessageBox.Show(thongBao, thanhCong ? "Success" : "Error",
                    MessageBoxButtons.OK, thanhCong ? MessageBoxIcon.Information : MessageBoxIcon.Error);

                if (thanhCong)
                {
                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
            else
            {
                var (thanhCong, thongBao, _) = DataManager.ThemLichChieu(maPhim, maPhong, gioBatDau);

                MessageBox.Show(thongBao, thanhCong ? "Success" : "Error",
                    MessageBoxButtons.OK, thanhCong ? MessageBoxIcon.Information : MessageBoxIcon.Error);

                if (thanhCong)
                {
                    LoadLichChieuCuaPhong();
                }
            }
        }

        private void btnXoaForm_Click(object? sender, EventArgs e)
        {
            if (cboPhim.Items.Count > 0) cboPhim.SelectedIndex = 0;
            if (cboPhong.Items.Count > 0) cboPhong.SelectedIndex = 0;
            dtpNgayChieu.Value = DateTime.Today;
            dtpGioChieu.Value = DateTime.Now;
            CapNhatThongTin();
            LoadLichChieuCuaPhong();
        }

        private void btnXemDanhSach_Click(object? sender, EventArgs e)
        {
            using var formDanhSach = new FormDanhSachLich();
            formDanhSach.ShowDialog();
            LoadLichChieuCuaPhong();
        }
    }
}
