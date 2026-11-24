namespace RapPhim
{
    partial class FormDanhSachLich
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
            lblTitle = new Label();

            panelFilter = new Panel();
            lblLocPhong = new Label();
            cboLocPhong = new ComboBox();
            lblLocNgay = new Label();
            dtpLocNgay = new DateTimePicker();
            chkTatCaNgay = new CheckBox();
            btnLoc = new Button();
            txtTimPhim = new TextBox();
            lblLyDoKhoa = new Label();

            dgvDanhSach = new DataGridView();

            panelBottom = new Panel();
            btnSaoChep = new Button();
            btnSua = new Button();
            btnXoa = new Button();
            btnDong = new Button();
            lblTongSuat = new Label();

            SuspendLayout();
            panelHeader.SuspendLayout();
            panelFilter.SuspendLayout();
            panelBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvDanhSach).BeginInit();

            // =============================================
            // panelTopBar - Cinema Red Accent Line
            // =============================================
            panelTopBar.BackColor = Color.FromArgb(225, 29, 72);
            panelTopBar.Dock = DockStyle.Top;
            panelTopBar.Height = 3;

            // =============================================
            // panelHeader - Deep Theater Charcoal
            // =============================================
            panelHeader.BackColor = Color.FromArgb(24, 24, 28);
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Height = 55;
            panelHeader.Controls.Add(lblTitle);

            lblTitle.Text = "📋   SHOWTIME SCHEDULE LIST";
            lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(244, 244, 245);
            lblTitle.Dock = DockStyle.Fill;
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            // =============================================
            // panelFilter - Filter Bar
            // =============================================
            panelFilter.BackColor = Color.FromArgb(27, 27, 31);
            panelFilter.Location = new Point(0, 58);
            panelFilter.Size = new Size(1060, 52);
            panelFilter.Controls.AddRange(new Control[] {
                lblLocPhong, cboLocPhong, lblLocNgay, dtpLocNgay, chkTatCaNgay, btnLoc, txtTimPhim
            });

            lblLocPhong.Text = "Room:";
            lblLocPhong.Font = new Font("Segoe UI", 9.5F);
            lblLocPhong.ForeColor = Color.FromArgb(161, 161, 170);
            lblLocPhong.Location = new Point(20, 15);
            lblLocPhong.AutoSize = true;

            cboLocPhong.Font = new Font("Segoe UI", 10F);
            cboLocPhong.DropDownStyle = ComboBoxStyle.DropDownList;
            cboLocPhong.BackColor = Color.FromArgb(18, 18, 20);
            cboLocPhong.ForeColor = Color.FromArgb(244, 244, 245);
            cboLocPhong.FlatStyle = FlatStyle.Flat;
            cboLocPhong.Location = new Point(75, 12);
            cboLocPhong.Size = new Size(210, 30);

            lblLocNgay.Text = "Date:";
            lblLocNgay.Font = new Font("Segoe UI", 9.5F);
            lblLocNgay.ForeColor = Color.FromArgb(161, 161, 170);
            lblLocNgay.Location = new Point(310, 15);
            lblLocNgay.AutoSize = true;

            dtpLocNgay.Font = new Font("Segoe UI", 10F);
            dtpLocNgay.Format = DateTimePickerFormat.Short;
            dtpLocNgay.Location = new Point(355, 12);
            dtpLocNgay.Size = new Size(150, 30);

            chkTatCaNgay.Text = "All dates";
            chkTatCaNgay.Font = new Font("Segoe UI", 9.5F);
            chkTatCaNgay.ForeColor = Color.FromArgb(228, 228, 231);
            chkTatCaNgay.Location = new Point(530, 14);
            chkTatCaNgay.AutoSize = true;
            chkTatCaNgay.CheckedChanged += chkTatCaNgay_CheckedChanged;

            btnLoc.Text = "🔍  Filter";
            btnLoc.Font = new Font("Segoe UI Bold", 10F, FontStyle.Bold);
            btnLoc.BackColor = Color.FromArgb(225, 29, 72); // Cinema Crimson Red
            btnLoc.ForeColor = Color.White;
            btnLoc.FlatStyle = FlatStyle.Flat;
            btnLoc.FlatAppearance.BorderSize = 0;
            btnLoc.Location = new Point(660, 9);
            btnLoc.Size = new Size(100, 34);
            btnLoc.Cursor = Cursors.Hand;
            btnLoc.Click += btnLoc_Click;

            // txtTimPhim - live movie search
            txtTimPhim.Font = new Font("Segoe UI", 10F);
            txtTimPhim.PlaceholderText = "Search movie...";
            txtTimPhim.BackColor = Color.FromArgb(18, 18, 20);
            txtTimPhim.ForeColor = Color.FromArgb(244, 244, 245);
            txtTimPhim.BorderStyle = BorderStyle.FixedSingle;
            txtTimPhim.Location = new Point(780, 12);
            txtTimPhim.Size = new Size(250, 30);
            txtTimPhim.TextChanged += txtTimPhim_TextChanged;

            // =============================================
            // dgvDanhSach - Main Data Grid
            // =============================================
            dgvDanhSach.Location = new Point(15, 125);
            dgvDanhSach.Size = new Size(1030, 395);
            dgvDanhSach.Font = new Font("Segoe UI", 9.5F);
            dgvDanhSach.ReadOnly = true;
            dgvDanhSach.AllowUserToAddRows = false;
            dgvDanhSach.AllowUserToDeleteRows = false;
            dgvDanhSach.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvDanhSach.MultiSelect = false;
            dgvDanhSach.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvDanhSach.BackgroundColor = Color.FromArgb(18, 18, 20);
            dgvDanhSach.BorderStyle = BorderStyle.None;
            dgvDanhSach.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvDanhSach.GridColor = Color.FromArgb(45, 45, 50);
            dgvDanhSach.RowHeadersVisible = false;
            dgvDanhSach.EnableHeadersVisualStyles = false;

            dgvDanhSach.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(27, 27, 31);
            dgvDanhSach.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(244, 244, 245);
            dgvDanhSach.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            dgvDanhSach.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvDanhSach.ColumnHeadersHeight = 38;

            dgvDanhSach.DefaultCellStyle.BackColor = Color.FromArgb(18, 18, 20);
            dgvDanhSach.DefaultCellStyle.ForeColor = Color.FromArgb(228, 228, 231);
            dgvDanhSach.DefaultCellStyle.SelectionBackColor = Color.FromArgb(159, 18, 57);
            dgvDanhSach.DefaultCellStyle.SelectionForeColor = Color.White;

            dgvDanhSach.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(24, 24, 28);
            dgvDanhSach.AlternatingRowsDefaultCellStyle.ForeColor = Color.FromArgb(228, 228, 231);
            dgvDanhSach.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(159, 18, 57);
            dgvDanhSach.AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.White;

            dgvDanhSach.RowTemplate.Height = 36;
            dgvDanhSach.CellFormatting += dgvDanhSach_CellFormatting;
            dgvDanhSach.SelectionChanged += dgvDanhSach_SelectionChanged;
            dgvDanhSach.CellDoubleClick += dgvDanhSach_CellDoubleClick;
            dgvDanhSach.KeyDown += dgvDanhSach_KeyDown;

            // =============================================
            // panelBottom - Footer Action Bar
            // =============================================
            panelBottom.BackColor = Color.FromArgb(24, 24, 28);
            panelBottom.Location = new Point(0, 535);
            panelBottom.Size = new Size(1060, 60);
            panelBottom.Controls.AddRange(new Control[] { lblTongSuat, lblLyDoKhoa, btnSaoChep, btnSua, btnXoa, btnDong });

            lblLyDoKhoa.Text = "";
            lblLyDoKhoa.Font = new Font("Segoe UI", 9.5F, FontStyle.Italic);
            lblLyDoKhoa.ForeColor = Color.FromArgb(251, 191, 36);
            lblLyDoKhoa.Location = new Point(230, 21);
            lblLyDoKhoa.Size = new Size(300, 22);

            // btnSaoChep - Copy Day button
            btnSaoChep.Text = "📑 Copy Day";
            btnSaoChep.Font = new Font("Segoe UI Bold", 9.5F, FontStyle.Bold);
            btnSaoChep.BackColor = Color.FromArgb(79, 70, 229);
            btnSaoChep.ForeColor = Color.White;
            btnSaoChep.FlatStyle = FlatStyle.Flat;
            btnSaoChep.FlatAppearance.BorderSize = 0;
            btnSaoChep.Location = new Point(535, 10);
            btnSaoChep.Size = new Size(115, 40);
            btnSaoChep.Cursor = Cursors.Hand;
            btnSaoChep.Click += btnSaoChep_Click;

            lblTongSuat.Text = "Total: 0 showtimes";
            lblTongSuat.Font = new Font("Segoe UI Semibold", 10.5F, FontStyle.Bold);
            lblTongSuat.ForeColor = Color.FromArgb(244, 244, 245);
            lblTongSuat.Location = new Point(20, 20);
            lblTongSuat.AutoSize = true;

            // btnSua - Warm Gold / Amber Accent
            btnSua.Text = "✏️   Edit";
            btnSua.Font = new Font("Segoe UI Bold", 10F, FontStyle.Bold);
            btnSua.BackColor = Color.FromArgb(217, 119, 6); // Amber 600
            btnSua.ForeColor = Color.White;
            btnSua.FlatStyle = FlatStyle.Flat;
            btnSua.FlatAppearance.BorderSize = 0;
            btnSua.Location = new Point(660, 10);
            btnSua.Size = new Size(120, 40);
            btnSua.Cursor = Cursors.Hand;
            btnSua.Click += btnSua_Click;

            // btnXoa - Ruby Red Delete
            btnXoa.Text = "🗑️   Delete";
            btnXoa.Font = new Font("Segoe UI Bold", 10F, FontStyle.Bold);
            btnXoa.BackColor = Color.FromArgb(190, 18, 60); // Rose 700
            btnXoa.ForeColor = Color.White;
            btnXoa.FlatStyle = FlatStyle.Flat;
            btnXoa.FlatAppearance.BorderSize = 0;
            btnXoa.Location = new Point(790, 10);
            btnXoa.Size = new Size(120, 40);
            btnXoa.Cursor = Cursors.Hand;
            btnXoa.Click += btnXoa_Click;

            // btnDong - Dark Neutral Button
            btnDong.Text = "Close";
            btnDong.Font = new Font("Segoe UI", 10F);
            btnDong.BackColor = Color.FromArgb(63, 63, 70); // Zinc 700
            btnDong.ForeColor = Color.FromArgb(244, 244, 245);
            btnDong.FlatStyle = FlatStyle.Flat;
            btnDong.FlatAppearance.BorderSize = 0;
            btnDong.Location = new Point(920, 10);
            btnDong.Size = new Size(120, 40);
            btnDong.Cursor = Cursors.Hand;
            btnDong.Click += btnDong_Click;

            // =============================================
            // FormDanhSachLich
            // =============================================
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1060, 595);
            Controls.Add(panelHeader);
            Controls.Add(panelTopBar);
            Controls.Add(panelFilter);
            Controls.Add(dgvDanhSach);
            Controls.Add(panelBottom);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Showtime Schedule List - Cinema Pro";
            BackColor = Color.FromArgb(18, 18, 20);

            panelHeader.ResumeLayout(false);
            panelFilter.ResumeLayout(false);
            panelFilter.PerformLayout();
            panelBottom.ResumeLayout(false);
            panelBottom.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvDanhSach).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelTopBar;
        private Panel panelHeader;
        private Label lblTitle;

        private Panel panelFilter;
        private Label lblLocPhong;
        private ComboBox cboLocPhong;
        private Label lblLocNgay;
        private DateTimePicker dtpLocNgay;
        private CheckBox chkTatCaNgay;
        private Button btnLoc;
        private TextBox txtTimPhim;
        private Label lblLyDoKhoa;
        private DataGridView dgvDanhSach;

        private Panel panelBottom;
        private Button btnSaoChep;
        private Button btnSua;
        private Button btnXoa;
        private Button btnDong;
        private Label lblTongSuat;
    }
}
