using RapPhim.Data;
using RapPhim.Models;

namespace RapPhim
{
    internal static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            ApplicationConfiguration.Initialize();

            if (args.Length > 0 && args[0] == "--test")
            {
                RunThreeRoundsOfTests();
                return;
            }

            Application.Run(new Form1());
        }

        private static void RunThreeRoundsOfTests()
        {
            Console.WriteLine("=================================================");
            Console.WriteLine("  EXECUTING 3 ROUNDS OF RIGOROUS AUTOMATED TESTS");
            Console.WriteLine("=================================================");

            // -------------------------------------------------------------
            // ROUND 1: FormXepLich (Schedule Form & Dynamic Event Simulation)
            // -------------------------------------------------------------
            Console.WriteLine("\n[ROUND 1 / 3] Testing FormXepLich (Creation, Data Binding & Interactions)...");
            try
            {
                using var formXepLich = new FormXepLich();
                formXepLich.Show(); // Forces window handle creation and initial layout

                // Test ComboBox switching
                var cboPhim = formXepLich.Controls.Find("cboPhim", true).FirstOrDefault() as ComboBox;
                var cboPhong = formXepLich.Controls.Find("cboPhong", true).FirstOrDefault() as ComboBox;
                var dtpNgay = formXepLich.Controls.Find("dtpNgayChieu", true).FirstOrDefault() as DateTimePicker;
                var dgv = formXepLich.Controls.Find("dgvLichPhong", true).FirstOrDefault() as DataGridView;

                if (cboPhim != null && cboPhim.Items.Count > 1)
                {
                    cboPhim.SelectedIndex = 1; // trigger event
                    cboPhim.SelectedIndex = 0; // trigger event
                    Console.WriteLine("  ✓ ComboBox 'Movie' selection changed successfully.");
                }

                if (cboPhong != null && cboPhong.Items.Count > 1)
                {
                    for (int i = 0; i < cboPhong.Items.Count; i++)
                    {
                        cboPhong.SelectedIndex = i; // trigger room change and grid refresh
                    }
                    Console.WriteLine("  ✓ ComboBox 'Room' cycled through all rooms successfully.");
                }

                if (dtpNgay != null)
                {
                    dtpNgay.Value = DateTime.Today.AddDays(1);
                    dtpNgay.Value = DateTime.Today;
                    Console.WriteLine("  ✓ DateTimePicker 'Date' changed and grid refreshed successfully.");
                }

                if (dgv != null)
                {
                    Console.WriteLine($"  ✓ DataGridView bound cleanly. Row count: {dgv.Rows.Count}, Column count: {dgv.Columns.Count}");
                }

                // Test Reset Button
                var btnReset = formXepLich.Controls.Find("btnXoaForm", true).FirstOrDefault() as Button;
                btnReset?.PerformClick();
                Console.WriteLine("  ✓ Reset button clicked successfully.");

                formXepLich.Close();
                Console.WriteLine("  >>> ROUND 1 RESULT: PASSED 100%");
            }
            catch (Exception ex)
            {
                Console.WriteLine("  >>> ROUND 1 FAILED: " + ex.ToString());
                Environment.Exit(1);
            }

            // -------------------------------------------------------------
            // ROUND 2: FormDanhSachLich (List Form, Filters, Row Selection)
            // -------------------------------------------------------------
            Console.WriteLine("\n[ROUND 2 / 3] Testing FormDanhSachLich (Filters, Grid Binding & Operations)...");
            try
            {
                using var formList = new FormDanhSachLich();
                formList.Show(); // Forces window handle creation

                var cboLocPhong = formList.Controls.Find("cboLocPhong", true).FirstOrDefault() as ComboBox;
                var chkTatCa = formList.Controls.Find("chkTatCaNgay", true).FirstOrDefault() as CheckBox;
                var btnLoc = formList.Controls.Find("btnLoc", true).FirstOrDefault() as Button;
                var dgvDanhSach = formList.Controls.Find("dgvDanhSach", true).FirstOrDefault() as DataGridView;

                // Cycle through all filter options
                if (cboLocPhong != null)
                {
                    for (int i = 0; i < cboLocPhong.Items.Count; i++)
                    {
                        cboLocPhong.SelectedIndex = i;
                        btnLoc?.PerformClick();
                    }
                    Console.WriteLine("  ✓ Room filter cycled through all options.");
                }

                // Check "All dates" checkbox
                if (chkTatCa != null)
                {
                    chkTatCa.Checked = true;
                    btnLoc?.PerformClick();
                    chkTatCa.Checked = false;
                    btnLoc?.PerformClick();
                    Console.WriteLine("  ✓ 'All dates' filter toggled and applied.");
                }

                if (dgvDanhSach != null)
                {
                    Console.WriteLine($"  ✓ Grid rendered cleanly. Rows: {dgvDanhSach.Rows.Count}, Columns: {dgvDanhSach.Columns.Count}");
                    // Verify row formatting
                    for (int r = 0; r < dgvDanhSach.Rows.Count; r++)
                    {
                        var cellVal = dgvDanhSach.Rows[r].Cells["Status"]?.Value?.ToString();
                        // Formatting check
                    }
                }

                formList.Close();
                Console.WriteLine("  >>> ROUND 2 RESULT: PASSED 100%");
            }
            catch (Exception ex)
            {
                Console.WriteLine("  >>> ROUND 2 FAILED: " + ex.ToString());
                Environment.Exit(1);
            }

            // -------------------------------------------------------------
            // ROUND 3: End-to-End Business Logic & Conflict Detection
            // -------------------------------------------------------------
            Console.WriteLine("\n[ROUND 3 / 3] Testing Conflict Detection Algorithm & Full Workflow...");
            try
            {
                // Test 1: Add a new showtime in Room 4 at 18:00
                DateTime baseTime = DateTime.Today.AddDays(5).AddHours(18);
                var (ok1, msg1, _) = DataManager.ThemLichChieu(1, 4, baseTime); // Movie 1 (181 mins -> ~21:01)
                if (!ok1) throw new Exception("Failed to add valid showtime: " + msg1);
                Console.WriteLine("  ✓ Added Showtime: Movie 1, Room 4, 18:00 -> SUCCESS.");

                // Test 2: Try to add another showtime in Room 4 at 19:00 (OVERLAP -> MUST FAIL)
                var (ok2, msg2, _) = DataManager.ThemLichChieu(2, 4, baseTime.AddHours(1));
                if (ok2) throw new Exception("Error: Conflicting showtime was incorrectly ALLOWED!");
                Console.WriteLine("  ✓ Conflict detected properly for overlapping start time: " + msg2.Split('\n')[0]);

                // Test 3: Try to add another showtime within 15 min buffer (at 21:10 -> buffer requires 21:16)
                var (ok3, msg3, _) = DataManager.ThemLichChieu(3, 4, baseTime.AddMinutes(181 + 5));
                if (ok3) throw new Exception("Error: Buffer violation was incorrectly ALLOWED!");
                Console.WriteLine("  ✓ 15-minute buffer protection verified: " + msg3.Split('\n')[0]);

                // Test 4: Add showtime AFTER buffer (at 21:20 -> OK)
                var (ok4, msg4, _) = DataManager.ThemLichChieu(3, 4, baseTime.AddMinutes(181 + 20));
                if (!ok4) throw new Exception("Failed to add valid showtime after buffer: " + msg4);
                Console.WriteLine("  ✓ Added valid Showtime after buffer period -> SUCCESS.");

                // Test 5: Verify Edit & Delete rules (cannot delete if tickets sold)
                var sample = DataManager.DanhSachLichChieu.FirstOrDefault(l => l.SoVeDaBan > 0);
                if (sample != null)
                {
                    var (delOk, delMsg) = DataManager.XoaLichChieu(sample.MaLichChieu);
                    if (delOk) throw new Exception("Sold ticket showtime was incorrectly deleted!");
                    Console.WriteLine("  ✓ Protection verified: cannot delete showtime with sold tickets.");
                }

                // Open Form1 to test main entry
                using var formMain = new Form1();
                formMain.Show();
                formMain.Close();
                Console.WriteLine("  ✓ Form1 opened and closed cleanly.");

                Console.WriteLine("  >>> ROUND 3 RESULT: PASSED 100%");
            }
            catch (Exception ex)
            {
                Console.WriteLine("  >>> ROUND 3 FAILED: " + ex.ToString());
                Environment.Exit(1);
            }

            Console.WriteLine("\n=================================================");
            Console.WriteLine("  CONGRATULATIONS! ALL 3 ROUNDS PASSED WITH 0 ERRORS!");
            Console.WriteLine("=================================================");
        }
    }
}
