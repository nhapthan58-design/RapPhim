namespace RapPhim.Models
{
    public class Phim
    {
        public int MaPhim { get; set; }
        public string TenPhim { get; set; } = string.Empty;
        public int ThoiLuong { get; set; } // Thời lượng tính bằng phút
        public string TheLoai { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"{TenPhim} ({ThoiLuong} min)";
        }
    }
}
