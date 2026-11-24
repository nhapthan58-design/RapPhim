using RapPhim.Models;

namespace RapPhim.Data
{
    /// <summary>
    /// Default sample catalog used until Mạnh's movie/room database tables are connected.
    /// </summary>
    public class MauDanhMucProvider : IDanhMucProvider
    {
        private static readonly List<Phim> _phim = new()
        {
            new Phim { MaPhim = 1, TenPhim = "Avengers: Endgame", ThoiLuong = 181, TheLoai = "Action" },
            new Phim { MaPhim = 2, TenPhim = "Lat Mat 7", ThoiLuong = 132, TheLoai = "Action" },
            new Phim { MaPhim = 3, TenPhim = "Mai", ThoiLuong = 131, TheLoai = "Drama" },
            new Phim { MaPhim = 4, TenPhim = "Doraemon: Nobita's Sky Utopia", ThoiLuong = 108, TheLoai = "Animation" },
            new Phim { MaPhim = 5, TenPhim = "Nha Ba Nu", ThoiLuong = 112, TheLoai = "Comedy" },
            new Phim { MaPhim = 6, TenPhim = "Oppenheimer", ThoiLuong = 180, TheLoai = "Drama" },
            new Phim { MaPhim = 7, TenPhim = "Spider-Man: Across the Spider-Verse", ThoiLuong = 140, TheLoai = "Animation" },
            new Phim { MaPhim = 8, TenPhim = "Dat Rung Phuong Nam", ThoiLuong = 110, TheLoai = "Adventure" }
        };

        private static readonly List<PhongChieu> _phong = new()
        {
            new PhongChieu { MaPhong = 1, TenPhong = "Room 1 - 2D", SoGhe = 120 },
            new PhongChieu { MaPhong = 2, TenPhong = "Room 2 - 3D", SoGhe = 80 },
            new PhongChieu { MaPhong = 3, TenPhong = "Room 3 - IMAX", SoGhe = 200 },
            new PhongChieu { MaPhong = 4, TenPhong = "Room 4 - VIP", SoGhe = 40 },
            new PhongChieu { MaPhong = 5, TenPhong = "Room 5 - 2D", SoGhe = 100 }
        };

        public IReadOnlyList<Phim> LayDanhSachPhim() => _phim;
        public IReadOnlyList<PhongChieu> LayDanhSachPhong() => _phong;
    }
}
