namespace RapPhim
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            panelTopBar = new Panel();
            panelHeader = new Panel();
            lblTieuDe = new Label();
            lblSubTitle = new Label();

            panelCenter = new Panel();
            btnXepLich = new Button();
            btnDanhSach = new Button();
            btnThoat = new Button();

            lblFooter = new Label();

            SuspendLayout();
            panelHeader.SuspendLayout();
            panelCenter.SuspendLayout();

            // =============================================
            // panelTopBar - Cinema Red Accent Line
            // =============================================
            panelTopBar.BackColor = Color.FromArgb(225, 29, 72); // Cinema Crimson Red
            panelTopBar.Dock = DockStyle.Top;
            panelTopBar.Height = 4;

            // =============================================
            // panelHeader - Deep Theater Charcoal
            // =============================================
            panelHeader.BackColor = Color.FromArgb(24, 24, 28);
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Height = 85;
            panelHeader.Controls.Add(lblSubTitle);
            panelHeader.Controls.Add(lblTieuDe);

            lblTieuDe.Text = "🎬  CINEMA MANAGEMENT";
            lblTieuDe.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblTieuDe.ForeColor = Color.FromArgb(244, 244, 245);
            lblTieuDe.Dock = DockStyle.Top;
            lblTieuDe.Height = 45;
            lblTieuDe.TextAlign = ContentAlignment.BottomCenter;

            lblSubTitle.Text = "Showtime Scheduling & Ticketing System";
            lblSubTitle.Font = new Font("Segoe UI", 9.5F);
            lblSubTitle.ForeColor = Color.FromArgb(161, 161, 170);
            lblSubTitle.Dock = DockStyle.Bottom;
            lblSubTitle.Height = 30;
            lblSubTitle.TextAlign = ContentAlignment.TopCenter;

            // =============================================
            // panelCenter - Card Container
            // =============================================
            panelCenter.BackColor = Color.Transparent;
            panelCenter.Location = new Point(120, 115);
            panelCenter.Size = new Size(360, 275);
            panelCenter.Controls.AddRange(new Control[] { btnXepLich, btnDanhSach, btnThoat });

            // btnXepLich - Primary Cinema Red CTA
            btnXepLich.Text = "📅   Schedule Showtime";
            btnXepLich.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnXepLich.BackColor = Color.FromArgb(225, 29, 72); // Velvet Crimson
            btnXepLich.ForeColor = Color.White;
            btnXepLich.FlatStyle = FlatStyle.Flat;
            btnXepLich.FlatAppearance.BorderSize = 0;
            btnXepLich.Location = new Point(0, 10);
            btnXepLich.Size = new Size(360, 68);
            btnXepLich.Cursor = Cursors.Hand;
            btnXepLich.Click += btnXepLich_Click;

            // btnDanhSach - Modern Charcoal Card Button
            btnDanhSach.Text = "📋   View Schedule List";
            btnDanhSach.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnDanhSach.BackColor = Color.FromArgb(39, 39, 42); // Zinc 800
            btnDanhSach.ForeColor = Color.FromArgb(244, 244, 245);
            btnDanhSach.FlatStyle = FlatStyle.Flat;
            btnDanhSach.FlatAppearance.BorderColor = Color.FromArgb(63, 63, 70);
            btnDanhSach.FlatAppearance.BorderSize = 1;
            btnDanhSach.Location = new Point(0, 95);
            btnDanhSach.Size = new Size(360, 68);
            btnDanhSach.Cursor = Cursors.Hand;
            btnDanhSach.Click += btnDanhSach_Click;

            // btnThoat - Subtle Exit Button
            btnThoat.Text = "Exit";
            btnThoat.Font = new Font("Segoe UI", 10.5F);
            btnThoat.BackColor = Color.FromArgb(24, 24, 28);
            btnThoat.ForeColor = Color.FromArgb(161, 161, 170);
            btnThoat.FlatStyle = FlatStyle.Flat;
            btnThoat.FlatAppearance.BorderColor = Color.FromArgb(39, 39, 42);
            btnThoat.FlatAppearance.BorderSize = 1;
            btnThoat.Location = new Point(0, 185);
            btnThoat.Size = new Size(360, 46);
            btnThoat.Cursor = Cursors.Hand;
            btnThoat.Click += btnThoat_Click;

            // =============================================
            // lblFooter
            // =============================================
            lblFooter.Text = "Cinema Pro System  •  Showtime Management";
            lblFooter.Font = new Font("Segoe UI", 8.5F);
            lblFooter.ForeColor = Color.FromArgb(113, 113, 122);
            lblFooter.Dock = DockStyle.Bottom;
            lblFooter.Height = 35;
            lblFooter.TextAlign = ContentAlignment.MiddleCenter;

            // =============================================
            // Form1
            // =============================================
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(600, 440);
            Controls.Add(panelHeader);
            Controls.Add(panelTopBar);
            Controls.Add(panelCenter);
            Controls.Add(lblFooter);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Cinema Ticket Management";
            BackColor = Color.FromArgb(18, 18, 20); // Cinema Deep Black

            panelHeader.ResumeLayout(false);
            panelCenter.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel panelTopBar;
        private Panel panelHeader;
        private Label lblTieuDe;
        private Label lblSubTitle;

        private Panel panelCenter;
        private Button btnXepLich;
        private Button btnDanhSach;
        private Button btnThoat;

        private Label lblFooter;
    }
}
