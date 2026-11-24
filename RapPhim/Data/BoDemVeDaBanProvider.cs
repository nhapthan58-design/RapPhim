namespace RapPhim.Data
{
    /// <summary>
    /// Default sold-ticket counter backed by <see cref="Models.LichChieu.SoVeDaBan"/>.
    /// </summary>
    public class BoDemVeDaBanProvider : IVeDaBanProvider
    {
        public int DemVeDaBan(int maLichChieu)
        {
            var l = DataManager.DanhSachLichChieu.FirstOrDefault(x => x.MaLichChieu == maLichChieu);
            return l?.SoVeDaBan ?? 0;
        }
    }
}
