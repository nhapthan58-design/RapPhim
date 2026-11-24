using RapPhim.Data;
using RapPhim.Models;
using RapPhim.Services;

namespace RapPhim
{
    public partial class FormSaoChepLich : Form
    {
        public bool DaSaoChepThanhCong { get; private set; }

        public FormSaoChepLich(DateTime? ngayNguonMacDinh = null)
        {
            InitializeComponent();

            DateTime tu = (ngayNguonMacDinh ?? DateTime.Today.AddDays(1)).Date;
            dtpTuNgay.Value = tu;
            dtpDenNgay.Value = tu.AddDays(1);

            var danhSachPhong = new List<PhongChieu>
            {
                new PhongChieu { MaPhong = 0, TenPhong = "-- All Rooms --" }
            };
            danhSachPhong.AddRange(DataManager.DanhSachPhongChieu);

            cboPhong.DisplayMember = "TenPhong";
            cboPhong.ValueMember = "MaPhong";
            cboPhong.DataSource = danhSachPhong;
        }

        private void btnSaoChep_Click(object? sender, EventArgs e)
        {
            int? maPhong = cboPhong.SelectedValue is int id && id != 0 ? id : null;
            var kq = LichChieuService.SaoChepLichNgay(dtpTuNgay.Value.Date, dtpDenNgay.Value.Date, maPhong);

            if (kq.SoTaoMoi > 0) DaSaoChepThanhCong = true;

            var lines = new List<string>
            {
                $"Created {kq.SoTaoMoi} showtime(s). Skipped {kq.BoQua.Count}."
            };
            if (kq.BoQua.Count > 0)
            {
                lines.Add("");
                lines.Add("Skipped details:");
                foreach (var reason in kq.BoQua)
                {
                    lines.Add("• " + reason);
                }
            }

            txtKetQua.Text = string.Join(Environment.NewLine, lines);
        }

        private void btnHuy_Click(object? sender, EventArgs e) => Close();
    }
}
