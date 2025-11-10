namespace Library_Final
{
    partial class NotificationDashboard
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges7 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges8 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges9 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges10 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            dgvEmailLog = new DataGridView();
            btnRefresh = new Guna.UI2.WinForms.Guna2Button();
            btnSendNow = new Guna.UI2.WinForms.Guna2Button();
            btnExport = new Guna.UI2.WinForms.Guna2Button();
            lblEmailCount = new Label();
            lblLastRun = new Label();
            dtpFrom = new Guna.UI2.WinForms.Guna2DateTimePicker();
            dtpTo = new Guna.UI2.WinForms.Guna2DateTimePicker();
            label1 = new Label();
            label2 = new Label();
            lblFilteredCount = new Label();
            panel1 = new Panel();
            ((System.ComponentModel.ISupportInitialize)dgvEmailLog).BeginInit();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // dgvEmailLog
            // 
            dgvEmailLog.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvEmailLog.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvEmailLog.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvEmailLog.Dock = DockStyle.Fill;
            dgvEmailLog.Location = new Point(0, 0);
            dgvEmailLog.Name = "dgvEmailLog";
            dgvEmailLog.RowHeadersWidth = 51;
            dgvEmailLog.Size = new Size(1880, 584);
            dgvEmailLog.TabIndex = 0;
            dgvEmailLog.CellContentClick += dgvEmailLog_CellContentClick;
            // 
            // btnRefresh
            // 
            btnRefresh.CustomizableEdges = customizableEdges1;
            btnRefresh.DisabledState.BorderColor = Color.DarkGray;
            btnRefresh.DisabledState.CustomBorderColor = Color.DarkGray;
            btnRefresh.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnRefresh.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnRefresh.Font = new Font("Segoe UI", 9F);
            btnRefresh.ForeColor = Color.White;
            btnRefresh.Location = new Point(21, 366);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.ShadowDecoration.CustomizableEdges = customizableEdges2;
            btnRefresh.Size = new Size(225, 56);
            btnRefresh.TabIndex = 3;
            btnRefresh.Text = "Refresh";
            btnRefresh.Click += btnRefresh_Click;
            // 
            // btnSendNow
            // 
            btnSendNow.CustomizableEdges = customizableEdges3;
            btnSendNow.DisabledState.BorderColor = Color.DarkGray;
            btnSendNow.DisabledState.CustomBorderColor = Color.DarkGray;
            btnSendNow.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnSendNow.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnSendNow.Font = new Font("Segoe UI", 9F);
            btnSendNow.ForeColor = Color.White;
            btnSendNow.Location = new Point(21, 12);
            btnSendNow.Name = "btnSendNow";
            btnSendNow.ShadowDecoration.CustomizableEdges = customizableEdges4;
            btnSendNow.Size = new Size(225, 56);
            btnSendNow.TabIndex = 4;
            btnSendNow.Text = "Send Email  Notifications";
            btnSendNow.Click += btnSendNow_Click;
            // 
            // btnExport
            // 
            btnExport.CustomizableEdges = customizableEdges5;
            btnExport.DisabledState.BorderColor = Color.DarkGray;
            btnExport.DisabledState.CustomBorderColor = Color.DarkGray;
            btnExport.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnExport.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnExport.Font = new Font("Segoe UI", 9F);
            btnExport.ForeColor = Color.White;
            btnExport.Location = new Point(267, 366);
            btnExport.Name = "btnExport";
            btnExport.ShadowDecoration.CustomizableEdges = customizableEdges6;
            btnExport.Size = new Size(225, 56);
            btnExport.TabIndex = 5;
            btnExport.Text = "Export to PDF";
            btnExport.Click += btnExport_Click;
            // 
            // lblEmailCount
            // 
            lblEmailCount.AutoSize = true;
            lblEmailCount.Location = new Point(519, 30);
            lblEmailCount.Name = "lblEmailCount";
            lblEmailCount.Size = new Size(89, 20);
            lblEmailCount.TabIndex = 6;
            lblEmailCount.Text = "Email Count";
            // 
            // lblLastRun
            // 
            lblLastRun.AutoSize = true;
            lblLastRun.Location = new Point(519, 73);
            lblLastRun.Name = "lblLastRun";
            lblLastRun.Size = new Size(64, 20);
            lblLastRun.TabIndex = 8;
            lblLastRun.Text = "Last Run";
            // 
            // dtpFrom
            // 
            dtpFrom.Checked = true;
            dtpFrom.CustomizableEdges = customizableEdges7;
            dtpFrom.Font = new Font("Segoe UI", 9F);
            dtpFrom.Format = DateTimePickerFormat.Long;
            dtpFrom.Location = new Point(21, 116);
            dtpFrom.MaxDate = new DateTime(9998, 12, 31, 0, 0, 0, 0);
            dtpFrom.MinDate = new DateTime(1753, 1, 1, 0, 0, 0, 0);
            dtpFrom.Name = "dtpFrom";
            dtpFrom.ShadowDecoration.CustomizableEdges = customizableEdges8;
            dtpFrom.Size = new Size(250, 45);
            dtpFrom.TabIndex = 9;
            dtpFrom.Value = new DateTime(2025, 11, 10, 16, 28, 19, 306);
            // 
            // dtpTo
            // 
            dtpTo.Checked = true;
            dtpTo.CustomizableEdges = customizableEdges9;
            dtpTo.Font = new Font("Segoe UI", 9F);
            dtpTo.Format = DateTimePickerFormat.Long;
            dtpTo.Location = new Point(21, 207);
            dtpTo.MaxDate = new DateTime(9998, 12, 31, 0, 0, 0, 0);
            dtpTo.MinDate = new DateTime(1753, 1, 1, 0, 0, 0, 0);
            dtpTo.Name = "dtpTo";
            dtpTo.ShadowDecoration.CustomizableEdges = customizableEdges10;
            dtpTo.Size = new Size(250, 45);
            dtpTo.TabIndex = 10;
            dtpTo.Value = new DateTime(2025, 11, 10, 16, 28, 19, 306);
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(21, 93);
            label1.Name = "label1";
            label1.Size = new Size(77, 20);
            label1.TabIndex = 11;
            label1.Text = "Date from";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(21, 184);
            label2.Name = "label2";
            label2.Size = new Size(61, 20);
            label2.TabIndex = 12;
            label2.Text = "Date To";
            // 
            // lblFilteredCount
            // 
            lblFilteredCount.AutoSize = true;
            lblFilteredCount.Location = new Point(21, 288);
            lblFilteredCount.Name = "lblFilteredCount";
            lblFilteredCount.Size = new Size(145, 20);
            lblFilteredCount.TabIndex = 16;
            lblFilteredCount.Text = "Email on that period";
            // 
            // panel1
            // 
            panel1.Controls.Add(dgvEmailLog);
            panel1.Location = new Point(10, 437);
            panel1.Name = "panel1";
            panel1.Size = new Size(1880, 584);
            panel1.TabIndex = 17;
            panel1.Paint += panel1_Paint;
            // 
            // NotificationDashboard
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1902, 1033);
            Controls.Add(panel1);
            Controls.Add(lblFilteredCount);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(dtpTo);
            Controls.Add(dtpFrom);
            Controls.Add(lblLastRun);
            Controls.Add(lblEmailCount);
            Controls.Add(btnExport);
            Controls.Add(btnSendNow);
            Controls.Add(btnRefresh);
            Name = "NotificationDashboard";
            Text = "NotificationDashboard";
            Load += NotificationDashboard_Load;
            ((System.ComponentModel.ISupportInitialize)dgvEmailLog).EndInit();
            panel1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dgvEmailLog;
        private Guna.UI2.WinForms.Guna2Button btnRefresh;
        private Guna.UI2.WinForms.Guna2Button btnSendNow;
        private Guna.UI2.WinForms.Guna2Button btnExport;
        private Label lblEmailCount;
        private Label lblLastRun;
        private Guna.UI2.WinForms.Guna2DateTimePicker dtpFrom;
        private Guna.UI2.WinForms.Guna2DateTimePicker dtpTo;
        private Label label1;
        private Label label2;
        private Label lblFilteredCount;
        private Panel panel1;
    }
}