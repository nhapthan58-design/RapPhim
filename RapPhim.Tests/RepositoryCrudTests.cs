using RapPhim.Data;
using RapPhim.Models;
using RapPhim.Services;

namespace RapPhim.Tests
{
    /// <summary>In-memory fake repository for fast, isolated unit tests.</summary>
    public class InMemoryLichChieuRepository : ILichChieuRepository
    {
        private readonly List<LichChieu> _items = new();
        private int _nextId = 1;
        public bool ThrowOnWrite { get; set; }
        public int InsertCount { get; private set; }
        public int UpdateCount { get; private set; }
        public int DeleteCount { get; private set; }

        public bool HasData() => true;

        public List<LichChieu> LoadAll() => _items.Select(Clone).ToList();

        public LichChieu Insert(LichChieu item)
        {
            if (ThrowOnWrite) throw new IOException("Simulated write error");
            var stored = Clone(item);
            stored.MaLichChieu = _nextId++;
            _items.Add(stored);
            InsertCount++;
            return Clone(stored);
        }

        public void Update(LichChieu item)
        {
            if (ThrowOnWrite) throw new IOException("Simulated write error");
            int idx = _items.FindIndex(x => x.MaLichChieu == item.MaLichChieu);
            if (idx < 0) throw new KeyNotFoundException($"Showtime #{item.MaLichChieu} not found.");
            _items[idx] = Clone(item);
            UpdateCount++;
        }

        public void Delete(int maLichChieu)
        {
            if (ThrowOnWrite) throw new IOException("Simulated write error");
            _items.RemoveAll(x => x.MaLichChieu == maLichChieu);
            DeleteCount++;
        }

        private static LichChieu Clone(LichChieu s) => new()
        {
            MaLichChieu = s.MaLichChieu,
            MaPhim = s.MaPhim,
            MaPhong = s.MaPhong,
            GioBatDau = s.GioBatDau,
            GioKetThuc = s.GioKetThuc,
            SoVeDaBan = s.SoVeDaBan,
            TenPhim = s.TenPhim,
            TenPhong = s.TenPhong,
            ThoiLuongPhim = s.ThoiLuongPhim
        };
    }

    [Collection("Sequential")]
    public class RepositoryCrudTests : IDisposable
    {
        private readonly string _dir = Path.Combine(Path.GetTempPath(), "RapPhimCrud_" + Guid.NewGuid().ToString("N"));

        public void Dispose()
        {
            if (Directory.Exists(_dir)) Directory.Delete(_dir, recursive: true);
        }

        private JsonLichChieuRepository NewRepo() => new(Path.Combine(_dir, "lichchieu.json"));

        private static LichChieu Sample(int phim = 1, int phong = 1, int hour = 10) => new()
        {
            MaPhim = phim,
            MaPhong = phong,
            GioBatDau = new DateTime(2030, 1, 15, hour, 0, 0),
            GioKetThuc = new DateTime(2030, 1, 15, hour + 2, 0, 0),
            SoVeDaBan = 0,
            TenPhim = "Movie " + phim,
            TenPhong = "Room " + phong,
            ThoiLuongPhim = 120
        };

        [Fact]
        public void Insert_AllocatesIncrementingIds_AcrossRepoInstances()
        {
            var repo1 = NewRepo();
            var a = repo1.Insert(Sample(1, 1, 9));
            var b = repo1.Insert(Sample(2, 1, 13));

            // New instance reading the same file must continue from max + 1
            var repo2 = NewRepo();
            var c = repo2.Insert(Sample(3, 2, 10));

            Assert.Equal(1, a.MaLichChieu);
            Assert.Equal(2, b.MaLichChieu);
            Assert.Equal(3, c.MaLichChieu);
        }

        [Fact]
        public void Update_ModifiesOnlyTargetRecord()
        {
            var repo = NewRepo();
            var a = repo.Insert(Sample(1, 1, 9));
            var b = repo.Insert(Sample(2, 2, 13));

            a.SoVeDaBan = 15;
            a.TenPhim = "Updated";
            repo.Update(a);

            var all = repo.LoadAll();
            Assert.Equal(2, all.Count);
            Assert.Equal(15, all.First(x => x.MaLichChieu == a.MaLichChieu).SoVeDaBan);
            Assert.Equal("Updated", all.First(x => x.MaLichChieu == a.MaLichChieu).TenPhim);
            Assert.Equal(0, all.First(x => x.MaLichChieu == b.MaLichChieu).SoVeDaBan);
        }

        [Fact]
        public void Update_MissingId_ThrowsKeyNotFound()
        {
            var repo = NewRepo();
            var ghost = Sample();
            ghost.MaLichChieu = 999;
            Assert.Throws<KeyNotFoundException>(() => repo.Update(ghost));
        }

        [Fact]
        public void Delete_MissingId_DoesNotCorruptFile()
        {
            var repo = NewRepo();
            var a = repo.Insert(Sample());

            repo.Delete(999);
            var loaded = Assert.Single(repo.LoadAll());
            Assert.Equal(a.MaLichChieu, loaded.MaLichChieu);
        }

        [Fact]
        public void TwoRepoInstances_SameFile_SecondSeesFirstInsert()
        {
            var repoA = NewRepo();
            var repoB = NewRepo();

            var inserted = repoA.Insert(Sample(1, 1, 10));
            var fromB = repoB.LoadAll();

            var found = Assert.Single(fromB);
            Assert.Equal(inserted.MaLichChieu, found.MaLichChieu);
        }

        [Fact]
        public void DungRepository_RoutesServiceWritesToFake_NotJson()
        {
            var prevRepo = DataManager.Repository;
            var prevNow = LichChieuService.Now;
            var fake = new InMemoryLichChieuRepository();
            try
            {
                LichChieuService.Now = () => new DateTime(2030, 1, 15, 8, 0, 0);
                DataManager.DungRepository(fake);

                var kq = LichChieuService.Them(1, 1, new DateTime(2030, 1, 15, 10, 0, 0));
                Assert.True(kq.ThanhCong, kq.ThongBao);
                Assert.Equal(1, fake.InsertCount);
                Assert.Single(fake.LoadAll());
            }
            finally
            {
                LichChieuService.Now = prevNow;
                DataManager.DungRepository(prevRepo);
            }
        }

        [Fact]
        public void WriteFailure_ReturnsErrorKetQua_DoesNotCrash()
        {
            var prevRepo = DataManager.Repository;
            var prevNow = LichChieuService.Now;
            var fake = new InMemoryLichChieuRepository { ThrowOnWrite = true };
            try
            {
                LichChieuService.Now = () => new DateTime(2030, 1, 15, 8, 0, 0);
                DataManager.DungRepository(fake);

                var kq = LichChieuService.Them(1, 1, new DateTime(2030, 1, 15, 10, 0, 0));
                Assert.False(kq.ThanhCong);
                Assert.Contains("Could not save", kq.ThongBao);
            }
            finally
            {
                LichChieuService.Now = prevNow;
                DataManager.DungRepository(prevRepo);
            }
        }
    }
}
