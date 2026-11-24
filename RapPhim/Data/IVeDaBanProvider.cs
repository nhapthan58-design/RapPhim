namespace RapPhim.Data
{
    /// <summary>
    /// Read-only provider that reports how many tickets/seats have been sold for a showtime.
    /// When Khôi's payment module stores sold seats in DB, swap to a DB-backed implementation in <see cref="CauHinhLuuTru"/>.
    /// </summary>
    public interface IVeDaBanProvider
    {
        int DemVeDaBan(int maLichChieu);
    }
}
