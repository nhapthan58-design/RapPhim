using RapPhim.Data;
using RapPhim.Models;
using RapPhim.Services;

namespace RapPhim
{
    public partial class FormDanhSachLich : Form
    {
        private const string TRANG_THAI_CON_TRONG = "Available";
        private const string TRANG_THAI_KHOA = "Locked";
        private const string TRANG_THAI_DA_CHIEU = "Ended";

        public FormDanhSachLich()
        {
            InitializeComponent();

            // Showing every date by default so newly created showtimes are always visible.
            chkTatCaNgay.Checked = true;
            dtpLocNgay.Enabled = false;

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

        private void btnLoc_Click(object? sender, EventArgs e) => LoadDanhSach();

        private void txtTimPhim_TextChanged(object? sender, EventArgs e) => LoadDanhSach();

        private static string TinhTrangThai(LichChieu l, DateTime now)
        {
            if (l.GioKetThuc <= now) return TRANG_THAI_DA_CHIEU;
            if (!LichChieuService.CoTheSuaXoa(l, now, out _)) return TRANG_THAI_KHOA;
            return TRANG_THAI_CON_TRONG;
        }

        private void LoadDanhSach()
        {
            // Remember the selection so it survives the reload.
            int? maDangChon = null;
            int chiSoDangChon = -1;
            if (dgvDanhSach.SelectedRows.Count > 0)
            {
                chiSoDangChon = dgvDanhSach.SelectedRows[0].Index;
                if (dgvDanhSach.SelectedRows[0].DataBoundItem is LichChieuViewModel cu) maDangChon = cu.ID;
            }

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

            string tuKhoa = txtTimPhim.Text.Trim();
            if (tuKhoa.Length > 0)
            {
                query = query.Where(l => l.TenPhim.Contains(tuKhoa, StringComparison.OrdinalIgnoreCase));
            }

            DateTime now = LichChieuService.Now();
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
                    Status = TinhTrangThai(l, now)
                })
                .ToList();

            dgvDanhSach.DataSource = null;
            dgvDanhSach.DataSource = danhSach;

            // Restore the selection (same item, otherwise the neighbouring row).
            if (danhSach.Count > 0)
            {
                int chiSoMoi = maDangChon.HasValue ? danhSach.FindIndex(x => x.ID == maDangChon.Value) : -1;
                if (chiSoMoi < 0 && chiSoDangChon >= 0) chiSoMoi = Math.Min(chiSoDangChon, danhSach.Count - 1);
                if (chiSoMoi >= 0)
                {
                    dgvDanhSach.ClearSelection();
                    dgvDanhSach.Rows[chiSoMoi].Selected = true;
                    dgvDanhSach.CurrentCell = dgvDanhSach.Rows[chiSoMoi].Cells[0];
                }
            }

            lblTongSuat.Text = $"Total: {danhSach.Count} showtimes";
            CapNhatNutTheoDongChon();
        }

        /// <summary>Colour rows by status. Uses e.CellStyle only (touching row styles here causes repaint loops).</summary>
        private void dgvDanhSach_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dgvDanhSach.Rows.Count || e.CellStyle == null) return;
            if (dgvDanhSach.Rows[e.RowIndex].DataBoundItem is not LichChieuViewModel vm) return;

            switch (vm.Status)
            {
                case TRANG_THAI_KHOA:
                    e.CellStyle.BackColor = Color.FromArgb(48, 16, 24);        // deep wine
                    e.CellStyle.ForeColor = Color.FromArgb(254, 205, 211);     // soft rose
                    break;
                case TRANG_THAI_DA_CHIEU:
                    e.CellStyle.ForeColor = Color.FromArgb(113, 113, 122);     // muted grey
                    break;
                default:
                    e.CellStyle.BackColor = Color.FromArgb(14, 40, 30);        // deep forest
                    e.CellStyle.ForeColor = Color.FromArgb(167, 243, 208);     // soft mint
                    break;
            }
        }

        // =====================================================
        // SELECTION
        // =====================================================

        private LichChieu? LayLichChieuDangChon(bool baoNeuKhongChon = true)
        {
            if (dgvDanhSach.SelectedRows.Count == 0 ||
                dgvDanhSach.SelectedRows[0].DataBoundItem is not LichChieuViewModel vm)
            {
                if (baoNeuKhongChon)
                {
                    MessageBox.Show("Please select a showtime!", "Notice",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                return null;
            }

            return DataManager.DanhSachLichChieu.FirstOrDefault(l => l.MaLichChieu == vm.ID);
        }

        private void dgvDanhSach_SelectionChanged(object? sender, EventArgs e) => CapNhatNutTheoDongChon();

        /// <summary>Enable Edit/Delete only for showtimes that may be changed; explain why otherwise.</summary>
        private void CapNhatNutTheoDongChon()
        {
            var l = LayLichChieuDangChon(baoNeuKhongChon: false);
            if (l == null)
            {
                btnSua.Enabled = btnXoa.Enabled = false;
                lblLyDoKhoa.Text = "";
                return;
            }

            bool duocPhep = LichChieuService.CoTheSuaXoa(l, out string lyDo);
            btnSua.Enabled = btnXoa.Enabled = duocPhep;
            lblLyDoKhoa.Text = duocPhep ? "" : "🔒 " + lyDo;
        }

        private void dgvDanhSach_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || !btnSua.Enabled) return;
            btnSua_Click(sender, EventArgs.Empty);
        }

        private void dgvDanhSach_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && btnXoa.Enabled)
            {
                e.Handled = true;
                btnXoa_Click(sender, EventArgs.Empty);
            }
        }

        // =====================================================
        // ACTIONS
        // =====================================================

        private void btnSua_Click(object? sender, EventArgs e)
        {
            var lichChieu = LayLichChieuDangChon();
            if (lichChieu == null) return;

            if (!LichChieuService.CoTheSuaXoa(lichChieu, out string lyDo))
            {
                MessageBox.Show($"Cannot edit this showtime!\n\nMovie: {lichChieu.TenPhim}\n{lyDo}",
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

            if (!LichChieuService.CoTheSuaXoa(lichChieu, out string lyDo))
            {
                MessageBox.Show($"Cannot delete this showtime!\n\nMovie: {lichChieu.TenPhim}\n{lyDo}",
                    "Cannot Delete", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show(
                $"Are you sure you want to delete this showtime?\n\n" +
                $"Movie: {lichChieu.TenPhim}\n" +
                $"Room: {lichChieu.TenPhong}\n" +
                $"Time: {lichChieu.GioBatDau:HH:mm} - {lichChieu.GioKetThuc:HH:mm dd/MM/yyyy}",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            var kq = LichChieuService.Xoa(lichChieu.MaLichChieu);
            MessageBox.Show(kq.ThongBao, kq.ThanhCong ? "Success" : "Error",
                MessageBoxButtons.OK, kq.ThanhCong ? MessageBoxIcon.Information : MessageBoxIcon.Error);

            if (kq.ThanhCong) LoadDanhSach();
        }

        private void btnDong_Click(object? sender, EventArgs e) => Close();
    }
}
