namespace RapPhim
{
    partial class FormXepLich
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

            grpThongTin = new GroupBox();
            lblPhim = new Label();
            cboPhim = new ComboBox();
            lblPhong = new Label();
            cboPhong = new ComboBox();
            lblNgayChieu = new Label();
            dtpNgayChieu = new DateTimePicker();
            lblGioChieu = new Label();
            dtpGioChieu = new DateTimePicker();
            lblThoiLuong = new Label();
            lblThoiLuongValue = new Label();
            lblGioKetThuc = new Label();
            lblGioKetThucValue = new Label();
            btnGoiYGio = new Button();
            lblTrangThaiSlot = new Label();
            chkTuNhayGio = new CheckBox();
            lblThongBao = new Label();

            panelButtons = new Panel();
            btnXepLich = new Button();
            btnXoaForm = new Button();
            btnXemDanhSach = new Button();

            grpLichPhong = new GroupBox();
            dgvLichPhong = new DataGridView();

            SuspendLayout();
            grpThongTin.SuspendLayout();
            panelHeader.SuspendLayout();
            panelButtons.SuspendLayout();
            grpLichPhong.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvLichPhong).BeginInit();

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
            panelHeader.Height = 58;
            panelHeader.Controls.Add(lblTitle);

            lblTitle.Text = "🎬   SCHEDULE SHOWTIME";
            lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(244, 244, 245);
            lblTitle.Dock = DockStyle.Fill;
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            // =============================================
            // grpThongTin - Card Box
            // =============================================
            grpThongTin.Text = " Showtime Details ";
            grpThongTin.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            grpThongTin.ForeColor = Color.FromArgb(228, 228, 231);
            grpThongTin.BackColor = Color.FromArgb(27, 27, 31);
            grpThongTin.Location = new Point(20, 75);
            grpThongTin.Size = new Size(450, 355);
            grpThongTin.Controls.AddRange(new Control[] {
                lblPhim, cboPhim,
                lblPhong, cboPhong,
                lblNgayChieu, dtpNgayChieu,
                lblGioChieu, dtpGioChieu,
                lblThoiLuong, lblThoiLuongValue,
                lblGioKetThuc, lblGioKetThucValue,
                btnGoiYGio, lblTrangThaiSlot, chkTuNhayGio
            });

            // --- Select Movie ---
            lblPhim.Text = "Movie:";
            lblPhim.Font = new Font("Segoe UI", 9.5F);
            lblPhim.ForeColor = Color.FromArgb(161, 161, 170);
            lblPhim.Location = new Point(20, 38);
            lblPhim.Size = new Size(95, 25);

            cboPhim.Font = new Font("Segoe UI", 10F);
            cboPhim.DropDownStyle = ComboBoxStyle.DropDownList;
            cboPhim.BackColor = Color.FromArgb(18, 18, 20);
            cboPhim.ForeColor = Color.FromArgb(244, 244, 245);
            cboPhim.FlatStyle = FlatStyle.Flat;
            cboPhim.Location = new Point(125, 35);
            cboPhim.Size = new Size(305, 30);
            cboPhim.SelectedIndexChanged += cboPhim_SelectedIndexChanged;

            // --- Select Room ---
            lblPhong.Text = "Room:";
            lblPhong.Font = new Font("Segoe UI", 9.5F);
            lblPhong.ForeColor = Color.FromArgb(161, 161, 170);
            lblPhong.Location = new Point(20, 83);
            lblPhong.Size = new Size(95, 25);

            cboPhong.Font = new Font("Segoe UI", 10F);
            cboPhong.DropDownStyle = ComboBoxStyle.DropDownList;
            cboPhong.BackColor = Color.FromArgb(18, 18, 20);
            cboPhong.ForeColor = Color.FromArgb(244, 244, 245);
            cboPhong.FlatStyle = FlatStyle.Flat;
            cboPhong.Location = new Point(125, 80);
            cboPhong.Size = new Size(305, 30);
            cboPhong.SelectedIndexChanged += cboPhong_SelectedIndexChanged;

            // --- Select Date ---
            lblNgayChieu.Text = "Date:";
            lblNgayChieu.Font = new Font("Segoe UI", 9.5F);
            lblNgayChieu.ForeColor = Color.FromArgb(161, 161, 170);
            lblNgayChieu.Location = new Point(20, 128);
            lblNgayChieu.Size = new Size(95, 25);

            dtpNgayChieu.Font = new Font("Segoe UI", 10F);
            dtpNgayChieu.Format = DateTimePickerFormat.Short;
            dtpNgayChieu.Location = new Point(125, 125);
            dtpNgayChieu.Size = new Size(305, 30);
            dtpNgayChieu.MinDate = DateTime.Today;
            dtpNgayChieu.ValueChanged += dtpNgayChieu_ValueChanged;

            // --- Select Time ---
            lblGioChieu.Text = "Start Time:";
            lblGioChieu.Font = new Font("Segoe UI", 9.5F);
            lblGioChieu.ForeColor = Color.FromArgb(161, 161, 170);
            lblGioChieu.Location = new Point(20, 173);
            lblGioChieu.Size = new Size(95, 25);

            dtpGioChieu.Font = new Font("Segoe UI", 10F);
            dtpGioChieu.Format = DateTimePickerFormat.Custom;
            dtpGioChieu.CustomFormat = "HH:mm";
            dtpGioChieu.ShowUpDown = true;
            dtpGioChieu.Location = new Point(125, 170);
            dtpGioChieu.Size = new Size(190, 30);
            dtpGioChieu.ValueChanged += dtpGioChieu_ValueChanged;

            // --- Suggest next free slot ---
            btnGoiYGio.Text = "Next free slot";
            btnGoiYGio.Font = new Font("Segoe UI", 9F);
            btnGoiYGio.BackColor = Color.FromArgb(39, 39, 42);
            btnGoiYGio.ForeColor = Color.FromArgb(228, 228, 231);
            btnGoiYGio.FlatStyle = FlatStyle.Flat;
            btnGoiYGio.FlatAppearance.BorderColor = Color.FromArgb(63, 63, 70);
            btnGoiYGio.FlatAppearance.BorderSize = 1;
            btnGoiYGio.Location = new Point(325, 168);
            btnGoiYGio.Size = new Size(105, 32);
            btnGoiYGio.Cursor = Cursors.Hand;
            btnGoiYGio.Click += btnGoiYGio_Click;

            // --- Live slot status ---
            lblTrangThaiSlot.Text = "";
            lblTrangThaiSlot.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            lblTrangThaiSlot.ForeColor = Color.FromArgb(161, 161, 170);
            lblTrangThaiSlot.Location = new Point(20, 294);
            lblTrangThaiSlot.Size = new Size(410, 22);

            // --- Auto-jump checkbox ---
            chkTuNhayGio.Text = "Auto-jump to next free slot after adding";
            chkTuNhayGio.Font = new Font("Segoe UI", 9.5F);
            chkTuNhayGio.ForeColor = Color.FromArgb(228, 228, 231);
            chkTuNhayGio.Checked = true;
            chkTuNhayGio.AutoSize = true;
            chkTuNhayGio.Location = new Point(20, 322);

            // --- Non-blocking status toast ---
            lblThongBao.Text = "";
            lblThongBao.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            lblThongBao.ForeColor = Color.FromArgb(52, 211, 153);
            lblThongBao.Location = new Point(20, 496);
            lblThongBao.Size = new Size(940, 24);

            // --- Duration ---
            lblThoiLuong.Text = "Duration:";
            lblThoiLuong.Font = new Font("Segoe UI", 9.5F);
            lblThoiLuong.ForeColor = Color.FromArgb(161, 161, 170);
            lblThoiLuong.Location = new Point(20, 222);
            lblThoiLuong.Size = new Size(95, 25);

            lblThoiLuongValue.Text = "-- min";
            lblThoiLuongValue.Font = new Font("Segoe UI Semibold", 10.5F, FontStyle.Bold);
            lblThoiLuongValue.ForeColor = Color.FromArgb(56, 189, 248); // Sky Cyan
            lblThoiLuongValue.Location = new Point(125, 222);
            lblThoiLuongValue.Size = new Size(305, 25);

            // --- End Time ---
            lblGioKetThuc.Text = "End Time:";
            lblGioKetThuc.Font = new Font("Segoe UI", 9.5F);
            lblGioKetThuc.ForeColor = Color.FromArgb(161, 161, 170);
            lblGioKetThuc.Location = new Point(20, 265);
            lblGioKetThuc.Size = new Size(95, 25);

            lblGioKetThucValue.Text = "--:--";
            lblGioKetThucValue.Font = new Font("Segoe UI Bold", 11F, FontStyle.Bold);
            lblGioKetThucValue.ForeColor = Color.FromArgb(52, 211, 153); // Mint Green
            lblGioKetThucValue.Location = new Point(125, 265);
            lblGioKetThucValue.Size = new Size(305, 25);

            // =============================================
            // panelButtons - Action Bar
            // =============================================
            panelButtons.Location = new Point(20, 442);
            panelButtons.Size = new Size(450, 50);
            panelButtons.Controls.AddRange(new Control[] { btnXepLich, btnXoaForm, btnXemDanhSach });

            // btnXepLich - Primary Cinema Crimson
            btnXepLich.Text = "📅  SCHEDULE";
            btnXepLich.Font = new Font("Segoe UI Bold", 10.5F, FontStyle.Bold);
            btnXepLich.BackColor = Color.FromArgb(225, 29, 72);
            btnXepLich.ForeColor = Color.White;
            btnXepLich.FlatStyle = FlatStyle.Flat;
            btnXepLich.FlatAppearance.BorderSize = 0;
            btnXepLich.Location = new Point(0, 0);
            btnXepLich.Size = new Size(150, 45);
            btnXepLich.Cursor = Cursors.Hand;
            btnXepLich.Click += btnXepLich_Click;

            // btnXoaForm - Sleek Dark Button
            btnXoaForm.Text = "🔄  Reset";
            btnXoaForm.Font = new Font("Segoe UI", 10F);
            btnXoaForm.BackColor = Color.FromArgb(39, 39, 42);
            btnXoaForm.ForeColor = Color.FromArgb(228, 228, 231);
            btnXoaForm.FlatStyle = FlatStyle.Flat;
            btnXoaForm.FlatAppearance.BorderColor = Color.FromArgb(63, 63, 70);
            btnXoaForm.FlatAppearance.BorderSize = 1;
            btnXoaForm.Location = new Point(160, 0);
            btnXoaForm.Size = new Size(125, 45);
            btnXoaForm.Cursor = Cursors.Hand;
            btnXoaForm.Click += btnXoaForm_Click;

            // btnXemDanhSach - Indigo Accent Button
            btnXemDanhSach.Text = "📋  View List";
            btnXemDanhSach.Font = new Font("Segoe UI", 10F);
            btnXemDanhSach.BackColor = Color.FromArgb(79, 70, 229);
            btnXemDanhSach.ForeColor = Color.White;
            btnXemDanhSach.FlatStyle = FlatStyle.Flat;
            btnXemDanhSach.FlatAppearance.BorderSize = 0;
            btnXemDanhSach.Location = new Point(295, 0);
            btnXemDanhSach.Size = new Size(155, 45);
            btnXemDanhSach.Cursor = Cursors.Hand;
            btnXemDanhSach.Click += btnXemDanhSach_Click;

            // =============================================
            // grpLichPhong - Right Side Grid Container
            // =============================================
            grpLichPhong.Text = " Room Schedule ";
            grpLichPhong.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            grpLichPhong.ForeColor = Color.FromArgb(228, 228, 231);
            grpLichPhong.BackColor = Color.FromArgb(27, 27, 31);
            grpLichPhong.Location = new Point(490, 75);
            grpLichPhong.Size = new Size(470, 412);
            grpLichPhong.Controls.Add(dgvLichPhong);

            // dgvLichPhong - Modern Dark Table
            dgvLichPhong.Font = new Font("Segoe UI", 9.5F);
            dgvLichPhong.Location = new Point(15, 30);
            dgvLichPhong.Size = new Size(440, 367);
            dgvLichPhong.ReadOnly = true;
            dgvLichPhong.AllowUserToAddRows = false;
            dgvLichPhong.AllowUserToDeleteRows = false;
            dgvLichPhong.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvLichPhong.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvLichPhong.BackgroundColor = Color.FromArgb(18, 18, 20);
            dgvLichPhong.BorderStyle = BorderStyle.None;
            dgvLichPhong.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvLichPhong.GridColor = Color.FromArgb(45, 45, 50);
            dgvLichPhong.RowHeadersVisible = false;
            dgvLichPhong.EnableHeadersVisualStyles = false;

            dgvLichPhong.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(24, 24, 28);
            dgvLichPhong.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(244, 244, 245);
            dgvLichPhong.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            dgvLichPhong.ColumnHeadersHeight = 36;

            dgvLichPhong.DefaultCellStyle.BackColor = Color.FromArgb(18, 18, 20);
            dgvLichPhong.DefaultCellStyle.ForeColor = Color.FromArgb(228, 228, 231);
            dgvLichPhong.DefaultCellStyle.SelectionBackColor = Color.FromArgb(159, 18, 57);
            dgvLichPhong.DefaultCellStyle.SelectionForeColor = Color.White;

            dgvLichPhong.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(24, 24, 28);
            dgvLichPhong.AlternatingRowsDefaultCellStyle.ForeColor = Color.FromArgb(228, 228, 231);
            dgvLichPhong.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(159, 18, 57);
            dgvLichPhong.AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.White;
            dgvLichPhong.RowTemplate.Height = 34;

            // =============================================
            // FormXepLich
            // =============================================
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(980, 530);
            Controls.Add(panelHeader);
            Controls.Add(panelTopBar);
            Controls.Add(grpThongTin);
            Controls.Add(panelButtons);
            Controls.Add(grpLichPhong);
            Controls.Add(lblThongBao);
            AcceptButton = btnXepLich;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Schedule Showtime - Cinema Pro";
            BackColor = Color.FromArgb(18, 18, 20);

            panelHeader.ResumeLayout(false);
            grpThongTin.ResumeLayout(false);
            panelButtons.ResumeLayout(false);
            grpLichPhong.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvLichPhong).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelTopBar;
        private Panel panelHeader;
        private Label lblTitle;

        private GroupBox grpThongTin;
        private Label lblPhim;
        private ComboBox cboPhim;
        private Label lblPhong;
        private ComboBox cboPhong;
        private Label lblNgayChieu;
        private DateTimePicker dtpNgayChieu;
        private Label lblGioChieu;
        private DateTimePicker dtpGioChieu;
        private Label lblThoiLuong;
        private Label lblThoiLuongValue;
        private Label lblGioKetThuc;
        private Label lblGioKetThucValue;
        private Button btnGoiYGio;
        private Label lblTrangThaiSlot;
        private CheckBox chkTuNhayGio;
        private Label lblThongBao;

        private Panel panelButtons;
        private Button btnXepLich;
        private Button btnXoaForm;
        private Button btnXemDanhSach;

        private GroupBox grpLichPhong;
        private DataGridView dgvLichPhong;
    }
}
