using RapPhim.Data;
using RapPhim.Models;
using RapPhim.Services;

namespace RapPhim
{
    public partial class FormXepLich : Form
    {
        private readonly LichChieu? _lichChieuDangSua = null;
        private readonly System.Windows.Forms.Timer _timerThongBao;
        private bool _dangTaiDuLieu = false;     // suppress change events while loading
        private int _maXungDot = -1;             // showtime id highlighted as conflicting in the grid

        public FormXepLich()
        {
            InitializeComponent();
            dgvLichPhong.CellFormatting += dgvLichPhong_CellFormatting;

            _timerThongBao = new System.Windows.Forms.Timer { Interval = 4000 };
            _timerThongBao.Tick += (_, _) =>
            {
                _timerThongBao.Stop();
                lblThongBao.Text = "";
            };
            FormClosed += (_, _) => _timerThongBao.Dispose();

            _dangTaiDuLieu = true;
            DataManager.TaiLai();
            DatGioMacDinhHopLe();
            LoadDuLieu();
            _dangTaiDuLieu = false;

            LamMoiTatCa();
        }

        /// <summary>
        /// Constructor for EDIT mode
        /// </summary>
        public FormXepLich(LichChieu lichChieuCanSua) : this()
        {
            _lichChieuDangSua = lichChieuCanSua;
            lblTitle.Text = "✏️  EDIT SHOWTIME";
            btnXepLich.Text = "💾  UPDATE";
            btnXepLich.BackColor = Color.FromArgb(217, 119, 6); // Warm Amber Gold
            btnXemDanhSach.Visible = false;
            chkTuNhayGio.Visible = false;

            NapGiaTriSuaVaoForm();
            LamMoiTatCa();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        // =====================================================
        // LOADING
        // =====================================================

        private void LoadDuLieu()
        {
            cboPhim.DisplayMember = "TenPhim";
            cboPhim.ValueMember = "MaPhim";
            cboPhim.DataSource = new List<Phim>(DataManager.DanhSachPhim);

            cboPhong.DisplayMember = "TenPhong";
            cboPhong.ValueMember = "MaPhong";
            cboPhong.DataSource = new List<PhongChieu>(DataManager.DanhSachPhongChieu);
        }

        private void NapGiaTriSuaVaoForm()
        {
            if (_lichChieuDangSua == null) return;

            _dangTaiDuLieu = true;
            // The date picker normally refuses past dates; an old showtime must still be loadable.
            dtpNgayChieu.MinDate = DateTimePicker.MinimumDateTime;
            cboPhim.SelectedValue = _lichChieuDangSua.MaPhim;
            cboPhong.SelectedValue = _lichChieuDangSua.MaPhong;
            dtpNgayChieu.Value = _lichChieuDangSua.GioBatDau.Date;
            dtpGioChieu.Value = _lichChieuDangSua.GioBatDau;
            _dangTaiDuLieu = false;
        }

        private static DateTime LamTronLen5Phut(DateTime d)
        {
            d = LichChieuService.CatGiay(d);
            int du = d.Minute % 5;
            return du == 0 ? d : d.AddMinutes(5 - du);
        }

        /// <summary>Default add-mode date &amp; time snapped inside cinema operating hours [08:00, 23:30].</summary>
        private void DatGioMacDinhHopLe()
        {
            DateTime macDinh = LamTronLen5Phut(LichChieuService.Now().AddMinutes(5));
            if (macDinh.TimeOfDay < TimeSpan.FromHours(LichChieuService.GIO_MO_CUA))
            {
                macDinh = macDinh.Date.AddHours(LichChieuService.GIO_MO_CUA);
            }
            else if (macDinh.TimeOfDay > LichChieuService.GIO_BAT_DAU_MUON_NHAT)
            {
                macDinh = macDinh.Date.AddDays(1).AddHours(LichChieuService.GIO_MO_CUA);
            }

            if (macDinh.Date < dtpNgayChieu.MinDate) dtpNgayChieu.MinDate = macDinh.Date;
            dtpNgayChieu.Value = macDinh.Date;
            dtpGioChieu.Value = macDinh;
        }

        private DateTime LayGioBatDau()
        {
            return dtpNgayChieu.Value.Date.Add(new TimeSpan(dtpGioChieu.Value.Hour, dtpGioChieu.Value.Minute, 0));
        }

        private int BoQuaMa => _lichChieuDangSua?.MaLichChieu ?? -1;

        private void HienThongBaoNhe(string text, Color? color = null)
        {
            lblThongBao.ForeColor = color ?? Color.FromArgb(52, 211, 153);
            lblThongBao.Text = text;
            _timerThongBao.Stop();
            _timerThongBao.Start();
        }

        // =====================================================
        // EVENTS
        // =====================================================

        private void cboPhim_SelectedIndexChanged(object? sender, EventArgs e) => LamMoiNeuCan();
        private void cboPhong_SelectedIndexChanged(object? sender, EventArgs e) => LamMoiNeuCan();
        private void dtpNgayChieu_ValueChanged(object? sender, EventArgs e) => LamMoiNeuCan();
        private void dtpGioChieu_ValueChanged(object? sender, EventArgs e) => LamMoiNeuCan();

        private void LamMoiNeuCan()
        {
            if (_dangTaiDuLieu) return;
            LamMoiTatCa();
        }

        // =====================================================
        // REFRESH UI
        // =====================================================

        private void LamMoiTatCa()
        {
            CapNhatThongTin();
            LoadLichChieuCuaPhong();
            CapNhatTrangThaiSlot();
        }

        private void CapNhatThongTin()
        {
            if (cboPhim.SelectedItem is Phim phim)
            {
                lblThoiLuongValue.Text = $"{phim.ThoiLuong} min";
                lblGioKetThucValue.Text = LayGioBatDau().AddMinutes(phim.ThoiLuong).ToString("HH:mm dd/MM/yyyy");
            }
        }

        /// <summary>Live conflict check: runs on every change of movie / room / date / time.</summary>
        private void CapNhatTrangThaiSlot()
        {
            if (cboPhim.SelectedValue is not int maPhim || cboPhong.SelectedValue is not int maPhong)
            {
                lblTrangThaiSlot.Text = "";
                btnXepLich.Enabled = false;
                return;
            }

            var kq = LichChieuService.KiemTraKhaDung(maPhim, maPhong, LayGioBatDau(), BoQuaMa);

            if (kq.ThanhCong)
            {
                lblTrangThaiSlot.Text = "✓ Slot available";
                lblTrangThaiSlot.ForeColor = Color.FromArgb(52, 211, 153);   // mint
            }
            else
            {
                lblTrangThaiSlot.Text = "✗ " + kq.ThongBao;
                lblTrangThaiSlot.ForeColor = Color.FromArgb(244, 63, 94);    // rose
            }

            btnXepLich.Enabled = kq.ThanhCong;

            int maMoi = kq.LichXungDot?.MaLichChieu ?? -1;
            if (maMoi != _maXungDot)
            {
                _maXungDot = maMoi;
                dgvLichPhong.Invalidate();
            }
        }

        private void LoadLichChieuCuaPhong()
        {
            if (cboPhong.SelectedValue is not int maPhong) return;

            DateTime ngay = dtpNgayChieu.Value.Date;
            DateTime ngayKeTiep = ngay.AddDays(1);

            // Include showtimes that started the previous night and run past midnight.
            var lichCuaPhong = DataManager.DanhSachLichChieu
                .Where(l => l.MaPhong == maPhong && l.GioBatDau < ngayKeTiep && l.GioKetThuc > ngay)
                .OrderBy(l => l.GioBatDau)
                .Select(l => new PhongLichViewModel
                {
                    ID = l.MaLichChieu,
                    Movie = l.TenPhim,
                    Start = l.GioBatDau.ToString("HH:mm"),
                    End = l.GioKetThuc.ToString("HH:mm"),
                    Duration = $"{l.ThoiLuongPhim} min",
                    Tickets = DataManager.VeDaBanProvider.DemVeDaBan(l.MaLichChieu)
                })
                .ToList();

            dgvLichPhong.DataSource = null;
            dgvLichPhong.DataSource = lichCuaPhong;

            var tenPhong = (cboPhong.SelectedItem as PhongChieu)?.TenPhong ?? "";
            grpLichPhong.Text = $" {tenPhong} - {ngay:dd/MM/yyyy} ({lichCuaPhong.Count} showtimes) ";
        }

        /// <summary>Highlight conflicting row (rose) and edited row (amber) using e.CellStyle only.</summary>
        private void dgvLichPhong_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dgvLichPhong.Rows.Count || e.CellStyle == null) return;
            if (dgvLichPhong.Rows[e.RowIndex].DataBoundItem is not PhongLichViewModel vm) return;

            if (vm.ID == _maXungDot)
            {
                e.CellStyle.BackColor = Color.FromArgb(76, 5, 25);
                e.CellStyle.ForeColor = Color.FromArgb(254, 205, 211);
            }
            else if (_lichChieuDangSua != null && vm.ID == _lichChieuDangSua.MaLichChieu)
            {
                e.CellStyle.BackColor = Color.FromArgb(59, 42, 7);       // #3B2A07 amber tint
                e.CellStyle.ForeColor = Color.FromArgb(252, 211, 77);    // #FCD34D warm gold
            }
        }

        // =====================================================
        // BUTTONS
        // =====================================================

        private void btnGoiYGio_Click(object? sender, EventArgs e)
        {
            if (cboPhim.SelectedValue is not int maPhim || cboPhong.SelectedValue is not int maPhong) return;

            var goiY = LichChieuService.GoiYGioTrong(maPhim, maPhong, LayGioBatDau(), BoQuaMa);
            if (goiY == null)
            {
                MessageBox.Show("No free slot left on this day in this room.", "No Slot Available",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            dtpGioChieu.Value = goiY.Value;   // triggers ValueChanged -> refresh
        }

        private void btnXepLich_Click(object? sender, EventArgs e)
        {
            if (cboPhim.SelectedValue is not int maPhim || cboPhong.SelectedValue is not int maPhong)
            {
                MessageBox.Show("Please select a movie and a room!", "Missing Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DateTime gioBatDau = LayGioBatDau();

            if (_lichChieuDangSua != null)
            {
                var kq = LichChieuService.CapNhat(_lichChieuDangSua.MaLichChieu, maPhim, maPhong, gioBatDau);

                if (kq.ThanhCong)
                {
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    MessageBox.Show(kq.ThongBao, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return;
            }

            // ADD MODE — non-blocking success + optional auto-jump to next free slot
            var kqThem = LichChieuService.Them(maPhim, maPhong, gioBatDau);
            if (!kqThem.ThanhCong)
            {
                MessageBox.Show(kqThem.ThongBao, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LamMoiTatCa();
                return;
            }

            var phim = cboPhim.SelectedItem as Phim;
            DateTime gioKetThuc = phim != null ? gioBatDau.AddMinutes(phim.ThoiLuong) : gioBatDau;
            string tenPhim = phim?.TenPhim ?? "Showtime";
            string thongBao = $"✓ Added: {tenPhim} ({gioBatDau:HH:mm}–{gioKetThuc:HH:mm})";

            if (chkTuNhayGio.Checked)
            {
                var tiepTheo = LichChieuService.GoiYGioTrong(maPhim, maPhong, gioKetThuc);
                if (tiepTheo.HasValue)
                {
                    _dangTaiDuLieu = true;
                    dtpGioChieu.Value = tiepTheo.Value;
                    _dangTaiDuLieu = false;
                }
                else
                {
                    thongBao += "  ·  No free slot left today in this room.";
                }
            }

            LamMoiTatCa();
            HienThongBaoNhe(thongBao);
        }

        private void btnXoaForm_Click(object? sender, EventArgs e)
        {
            _dangTaiDuLieu = true;
            if (_lichChieuDangSua != null)
            {
                NapGiaTriSuaVaoForm();
            }
            else
            {
                if (cboPhim.Items.Count > 0) cboPhim.SelectedIndex = 0;
                if (cboPhong.Items.Count > 0) cboPhong.SelectedIndex = 0;
                DatGioMacDinhHopLe();
            }
            _dangTaiDuLieu = false;
            LamMoiTatCa();
        }

        private void btnXemDanhSach_Click(object? sender, EventArgs e)
        {
            using var formDanhSach = new FormDanhSachLich();
            formDanhSach.ShowDialog();
            LamMoiTatCa();
        }
    }
}
