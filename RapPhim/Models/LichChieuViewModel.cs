using System.ComponentModel;

namespace RapPhim.Models
{
    /// <summary>
    /// View model for the room schedule grid in FormXepLich
    /// </summary>
    public class PhongLichViewModel
    {
        public int ID { get; set; }
        public string Movie { get; set; } = string.Empty;
        public string Start { get; set; } = string.Empty;
        public string End { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public int Tickets { get; set; }
    }

    /// <summary>
    /// View model for the full schedule list in FormDanhSachLich
    /// </summary>
    public class LichChieuViewModel
    {
        public int ID { get; set; }
        public string Movie { get; set; } = string.Empty;
        public string Room { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string Start { get; set; } = string.Empty;
        public string End { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public int Tickets { get; set; }
        public string Status { get; set; } = string.Empty;

        [Browsable(false)]
        public DateTime GioBatDauGoc { get; set; }

        [Browsable(false)]
        public DateTime GioKetThucGoc { get; set; }

        [Browsable(false)]
        public int ThoiLuongPhut { get; set; }
    }
}
