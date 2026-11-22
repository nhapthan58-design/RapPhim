using System.Text.Json;
using RapPhim.Models;

namespace RapPhim.Data
{
    /// <summary>
    /// Stores showtimes in %AppData%\RapPhim\lichchieu.json (outside the git repo).
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

        public void SaveAll(IEnumerable<LichChieu> items)
        {
            string? dir = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);

            // Write to a temp file first, then replace: avoids a half-written file if the app crashes.
            string tmp = _filePath + ".tmp";
            File.WriteAllText(tmp, JsonSerializer.Serialize(items, Options));
            File.Move(tmp, _filePath, overwrite: true);
        }
    }
}
