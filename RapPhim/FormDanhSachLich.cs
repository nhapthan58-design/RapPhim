using RapPhim.Data;
using RapPhim.Models;

namespace RapPhim
{
    public partial class FormDanhSachLich : Form
    {
        public FormDanhSachLich()
        {
            InitializeComponent();
            LoadBoLoc();
            LoadDanhSach();
        }

        private void LoadBoLoc()
        {
            var danhSachPhong = new List<PhongChieu>();
            danhSachPhong.Add(new PhongChieu { MaPhong = 0, TenPhong = "-- All Rooms --" });
            danhSachPhong.AddRange(DataManager.DanhSachPhongChieu);

            cboLocPhong.DisplayMember = "TenPhong";
            cboLocPhong.ValueMember = "MaPhong";
            cboLocPhong.DataSource = danhSachPhong;
        }

        private void chkTatCaNgay_CheckedChanged(object? sender, EventArgs e)
        {
            dtpLocNgay.Enabled = !chkTatCaNgay.Checked;
        }

        private void btnLoc_Click(object? sender, EventArgs e)
        {
            LoadDanhSach();
        }

        private void LoadDanhSach()
        {
            var query = DataManager.DanhSachLichChieu.AsEnumerable();

            if (cboLocPhong.SelectedValue is int maPhong && maPhong != 0)
            {
                query = query.Where(l => l.MaPhong == maPhong);
            }

            if (!chkTatCaNgay.Checked)
            {
                DateTime ngayLoc = dtpLocNgay.Value.Date;
                query = query.Where(l => l.GioBatDau.Date == ngayLoc);
            }

            var danhSach = query
                .OrderBy(l => l.GioBatDau)
                .Select(l => new LichChieuViewModel
                {
                    ID = l.MaLichChieu,
                    Movie = l.TenPhim,
                    Room = l.TenPhong,
                    Date = l.GioBatDau.ToString("dd/MM/yyyy"),
                    Start = l.GioBatDau.ToString("HH:mm"),
                    End = l.GioKetThuc.ToString("HH:mm"),
                    Duration = $"{l.ThoiLuongPhim} min",
                    Tickets = l.SoVeDaBan,
                    Status = l.SoVeDaBan > 0 ? "Sold" : "Available"
                })
                .ToList();

            dgvDanhSach.DataSource = null;
            dgvDanhSach.DataSource = danhSach;

            lblTongSuat.Text = $"Total: {danhSach.Count} showtimes";
        }

        private void dgvDanhSach_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dgvDanhSach.Rows.Count) return;

            var row = dgvDanhSach.Rows[e.RowIndex];
            if (dgvDanhSach.Columns.Contains("Status") && row.Cells["Status"] != null)
            {
                var status = row.Cells["Status"].Value?.ToString();
                if (status == "Sold")
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(48, 16, 24); // Deep Wine Red
                    row.DefaultCellStyle.ForeColor = Color.FromArgb(254, 205, 211); // Soft Rose Text
                }
                else
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(14, 40, 30); // Deep Forest Green
                    row.DefaultCellStyle.ForeColor = Color.FromArgb(167, 243, 208); // Soft Mint Text
                }
            }
        }

        private LichChieu? LayLichChieuDangChon()
        {
            if (dgvDanhSach.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a showtime!", "Notice",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return null;
            }

            var selectedRow = dgvDanhSach.SelectedRows[0];
            if (dgvDanhSach.Columns.Contains("ID") && selectedRow.Cells["ID"]?.Value is int maLichChieu)
            {
                return DataManager.DanhSachLichChieu.FirstOrDefault(l => l.MaLichChieu == maLichChieu);
            }

            return null;
        }

        private void btnSua_Click(object? sender, EventArgs e)
        {
            var lichChieu = LayLichChieuDangChon();
            if (lichChieu == null) return;

            if (lichChieu.SoVeDaBan > 0)
            {
                MessageBox.Show(
                    $"Cannot edit this showtime!\n\nMovie: {lichChieu.TenPhim}\nTickets sold: {lichChieu.SoVeDaBan}",
                    "Cannot Edit", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var formSua = new FormXepLich(lichChieu);
            if (formSua.ShowDialog() == DialogResult.OK)
            {
                LoadDanhSach();
            }
        }

        private void btnXoa_Click(object? sender, EventArgs e)
        {
            var lichChieu = LayLichChieuDangChon();
            if (lichChieu == null) return;

            if (lichChieu.SoVeDaBan > 0)
            {
                MessageBox.Show(
                    $"Cannot delete this showtime!\n\nMovie: {lichChieu.TenPhim}\nTickets sold: {lichChieu.SoVeDaBan}",
                    "Cannot Delete", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show(
                $"Are you sure you want to delete this showtime?\n\n" +
                $"Movie: {lichChieu.TenPhim}\n" +
                $"Room: {lichChieu.TenPhong}\n" +
                $"Time: {lichChieu.GioBatDau:HH:mm} - {lichChieu.GioKetThuc:HH:mm dd/MM/yyyy}",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                var (thanhCong, thongBao) = DataManager.XoaLichChieu(lichChieu.MaLichChieu);
                MessageBox.Show(thongBao, thanhCong ? "Success" : "Error",
                    MessageBoxButtons.OK, thanhCong ? MessageBoxIcon.Information : MessageBoxIcon.Error);

                if (thanhCong)
                {
                    LoadDanhSach();
                }
            }
        }

        private void btnDong_Click(object? sender, EventArgs e)
        {
            Close();
        }
    }
}
