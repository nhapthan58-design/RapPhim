using System.Runtime.CompilerServices;

namespace RapPhim.Tests
{
    internal static class TestEnvironment
    {
        /// <summary>
        /// Runs before any test: redirect JSON storage to a temp folder so tests never touch the real data.
        /// (Must run before DataManager's static constructor.)
        /// </summary>
        [ModuleInitializer]
        internal static void Init()
        {
            string dir = Path.Combine(Path.GetTempPath(), "RapPhimTests_" + Guid.NewGuid().ToString("N"));
            Environment.SetEnvironmentVariable("RAPPHIM_DATA_DIR", dir);
        }
    }
}
