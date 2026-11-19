namespace RapPhim.Models
{
    public class PhongChieu
    {
        public int MaPhong { get; set; }
        public string TenPhong { get; set; } = string.Empty;
        public int SoGhe { get; set; }

        public override string ToString()
        {
            return $"{TenPhong} ({SoGhe} seats)";
        }
    }
}
