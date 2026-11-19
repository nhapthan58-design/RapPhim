namespace RapPhim.Models
{
    public class LichChieu
    {
        public int MaLichChieu { get; set; }
        public int MaPhim { get; set; }
        public int MaPhong { get; set; }
        public DateTime GioBatDau { get; set; }
        public DateTime GioKetThuc { get; set; }
        public int SoVeDaBan { get; set; } // Số vé đã bán cho suất chiếu này

        // Navigation properties (để hiển thị)
        public string TenPhim { get; set; } = string.Empty;
        public string TenPhong { get; set; } = string.Empty;
        public int ThoiLuongPhim { get; set; }
    }
}
