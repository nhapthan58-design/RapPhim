namespace RapPhim.Services
{
    /// <summary>
    /// Summary of a Copy-Day operation: how many showtimes were created and reasons for any skipped ones.
    /// </summary>
    public record KetQuaSaoChep(int SoTaoMoi, List<string> BoQua);
}
