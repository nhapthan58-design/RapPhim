namespace RapPhim
{
    partial class FormSaoChepLich
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

            lblTuNgay = new Label();
            dtpTuNgay = new DateTimePicker();
            lblDenNgay = new Label();
            dtpDenNgay = new DateTimePicker();
            lblPhong = new Label();
            cboPhong = new ComboBox();

            btnSaoChep = new Button();
            btnHuy = new Button();

            lblKetQua = new Label();
            txtKetQua = new TextBox();

            SuspendLayout();
            panelHeader.SuspendLayout();

            // panelTopBar
            panelTopBar.BackColor = Color.FromArgb(225, 29, 72);
            panelTopBar.Dock = DockStyle.Top;
            panelTopBar.Height = 3;

            // panelHeader
            panelHeader.BackColor = Color.FromArgb(24, 24, 28);
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Height = 52;
            panelHeader.Controls.Add(lblTitle);

            lblTitle.Text = "📑   COPY DAY SCHEDULE";
            lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(244, 244, 245);
            lblTitle.Dock = DockStyle.Fill;
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            // From Date
            lblTuNgay.Text = "From date:";
            lblTuNgay.Font = new Font("Segoe UI", 9.5F);
            lblTuNgay.ForeColor = Color.FromArgb(161, 161, 170);
            lblTuNgay.Location = new Point(24, 76);
            lblTuNgay.Size = new Size(95, 24);

            dtpTuNgay.Font = new Font("Segoe UI", 10F);
            dtpTuNgay.Format = DateTimePickerFormat.Short;
            dtpTuNgay.Location = new Point(125, 72);
            dtpTuNgay.Size = new Size(340, 28);

            // To Date
            lblDenNgay.Text = "To date:";
            lblDenNgay.Font = new Font("Segoe UI", 9.5F);
            lblDenNgay.ForeColor = Color.FromArgb(161, 161, 170);
            lblDenNgay.Location = new Point(24, 116);
            lblDenNgay.Size = new Size(95, 24);

            dtpDenNgay.Font = new Font("Segoe UI", 10F);
            dtpDenNgay.Format = DateTimePickerFormat.Short;
            dtpDenNgay.Location = new Point(125, 112);
            dtpDenNgay.Size = new Size(340, 28);

            // Room
            lblPhong.Text = "Room:";
            lblPhong.Font = new Font("Segoe UI", 9.5F);
            lblPhong.ForeColor = Color.FromArgb(161, 161, 170);
            lblPhong.Location = new Point(24, 156);
            lblPhong.Size = new Size(95, 24);

            cboPhong.Font = new Font("Segoe UI", 10F);
            cboPhong.DropDownStyle = ComboBoxStyle.DropDownList;
            cboPhong.BackColor = Color.FromArgb(27, 27, 31);
            cboPhong.ForeColor = Color.FromArgb(244, 244, 245);
            cboPhong.FlatStyle = FlatStyle.Flat;
            cboPhong.Location = new Point(125, 152);
            cboPhong.Size = new Size(340, 28);

            // Buttons
            btnSaoChep.Text = "📑  Copy Schedule";
            btnSaoChep.Font = new Font("Segoe UI Bold", 10F, FontStyle.Bold);
            btnSaoChep.BackColor = Color.FromArgb(225, 29, 72);
            btnSaoChep.ForeColor = Color.White;
            btnSaoChep.FlatStyle = FlatStyle.Flat;
            btnSaoChep.FlatAppearance.BorderSize = 0;
            btnSaoChep.Location = new Point(125, 198);
            btnSaoChep.Size = new Size(165, 38);
            btnSaoChep.Cursor = Cursors.Hand;
            btnSaoChep.Click += btnSaoChep_Click;

            btnHuy.Text = "Close";
            btnHuy.Font = new Font("Segoe UI", 10F);
            btnHuy.BackColor = Color.FromArgb(63, 63, 70);
            btnHuy.ForeColor = Color.FromArgb(244, 244, 245);
            btnHuy.FlatStyle = FlatStyle.Flat;
            btnHuy.FlatAppearance.BorderSize = 0;
            btnHuy.Location = new Point(305, 198);
            btnHuy.Size = new Size(160, 38);
            btnHuy.Cursor = Cursors.Hand;
            btnHuy.Click += btnHuy_Click;

            // Result summary
            lblKetQua.Text = "Result:";
            lblKetQua.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            lblKetQua.ForeColor = Color.FromArgb(161, 161, 170);
            lblKetQua.Location = new Point(24, 250);
            lblKetQua.AutoSize = true;

            txtKetQua.Font = new Font("Segoe UI", 9.5F);
            txtKetQua.Multiline = true;
            txtKetQua.ReadOnly = true;
            txtKetQua.ScrollBars = ScrollBars.Vertical;
            txtKetQua.BackColor = Color.FromArgb(24, 24, 28);
            txtKetQua.ForeColor = Color.FromArgb(228, 228, 231);
            txtKetQua.BorderStyle = BorderStyle.FixedSingle;
            txtKetQua.Location = new Point(24, 274);
            txtKetQua.Size = new Size(441, 130);

            // FormSaoChepLich
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(490, 425);
            Controls.Add(panelHeader);
            Controls.Add(panelTopBar);
            Controls.Add(lblTuNgay);
            Controls.Add(dtpTuNgay);
            Controls.Add(lblDenNgay);
            Controls.Add(dtpDenNgay);
            Controls.Add(lblPhong);
            Controls.Add(cboPhong);
            Controls.Add(btnSaoChep);
            Controls.Add(btnHuy);
            Controls.Add(lblKetQua);
            Controls.Add(txtKetQua);
            AcceptButton = btnSaoChep;
            CancelButton = btnHuy;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Copy Day Schedule - Cinema Pro";
            BackColor = Color.FromArgb(18, 18, 20);

            panelHeader.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panelTopBar;
        private Panel panelHeader;
        private Label lblTitle;

        private Label lblTuNgay;
        private DateTimePicker dtpTuNgay;
        private Label lblDenNgay;
        private DateTimePicker dtpDenNgay;
        private Label lblPhong;
        private ComboBox cboPhong;

        private Button btnSaoChep;
        private Button btnHuy;

        private Label lblKetQua;
        private TextBox txtKetQua;
    }
}
