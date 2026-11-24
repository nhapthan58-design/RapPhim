using System.Text.Json;
using RapPhim.Models;

namespace RapPhim.Data
{
    /// <summary>
    /// Stores showtimes in %AppData%\RapPhim\lichchieu.json (outside the git repo).
    /// Each CRUD call reads fresh data, mutates a single record, and writes atomically via a temp file.
    /// </summary>
    public class JsonLichChieuRepository : ILichChieuRepository
    {
        private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };
        private readonly string _filePath;

        public JsonLichChieuRepository(string? filePath = null)
        {
            // RAPPHIM_DATA_DIR lets tests / other environments redirect storage away from %AppData%.
            string baseDir = Environment.GetEnvironmentVariable("RAPPHIM_DATA_DIR")
                ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RapPhim");
            _filePath = filePath ?? Path.Combine(baseDir, "lichchieu.json");
        }

        public string FilePath => _filePath;

        public bool HasData() => File.Exists(_filePath);

        public List<LichChieu> LoadAll()
        {
            if (!File.Exists(_filePath)) return new List<LichChieu>();

            try
            {
                string json = File.ReadAllText(_filePath);
                return JsonSerializer.Deserialize<List<LichChieu>>(json, Options) ?? new List<LichChieu>();
            }
            catch (Exception ex) when (ex is JsonException || ex is IOException || ex is UnauthorizedAccessException)
            {
                // Corrupted / unreadable file: keep a backup and start with an empty list.
                try
                {
                    File.Move(_filePath, _filePath + ".bak", overwrite: true);
                }
                catch (Exception) { /* ignore backup failure */ }
                return new List<LichChieu>();
            }
        }

        public LichChieu Insert(LichChieu item)
        {
            var list = LoadAll();
            int nextId = list.Count == 0 ? 1 : list.Max(x => x.MaLichChieu) + 1;

            var stored = Clone(item, nextId);
            list.Add(stored);
            WriteAtomic(list);
            return Clone(stored, stored.MaLichChieu);
        }

        public void Update(LichChieu item)
        {
            var list = LoadAll();
            int idx = list.FindIndex(x => x.MaLichChieu == item.MaLichChieu);
            if (idx < 0)
                throw new KeyNotFoundException($"Showtime #{item.MaLichChieu} not found.");

            list[idx] = Clone(item, item.MaLichChieu);
            WriteAtomic(list);
        }

        public void Delete(int maLichChieu)
        {
            var list = LoadAll();
            int removed = list.RemoveAll(x => x.MaLichChieu == maLichChieu);
            if (removed > 0)
            {
                WriteAtomic(list);
            }
        }

        private void WriteAtomic(List<LichChieu> items)
        {
            string? dir = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);

            string tmp = _filePath + ".tmp";
            File.WriteAllText(tmp, JsonSerializer.Serialize(items, Options));
            File.Move(tmp, _filePath, overwrite: true);
        }

        private static LichChieu Clone(LichChieu src, int id) => new()
        {
            MaLichChieu = id,
            MaPhim = src.MaPhim,
            MaPhong = src.MaPhong,
            GioBatDau = src.GioBatDau,
            GioKetThuc = src.GioKetThuc,
            SoVeDaBan = src.SoVeDaBan,
            TenPhim = src.TenPhim,
            TenPhong = src.TenPhong,
            ThoiLuongPhim = src.ThoiLuongPhim
        };
    }
}
