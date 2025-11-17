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
            lblEmailCount = new Label();
            lblLastRun = new Label();
            dtpFrom = new Guna.UI2.WinForms.Guna2DateTimePicker();
            dtpTo = new Guna.UI2.WinForms.Guna2DateTimePicker();
            label1 = new Label();
            label2 = new Label();
            lblFilteredCount = new Label();
            panel1 = new Panel();
            guna2CustomGradientPanel1 = new Guna.UI2.WinForms.Guna2CustomGradientPanel();
            arthanButton1 = new LibraryCGC.Components.ArthanButton();
            guna2HtmlLabel2 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            guna2HtmlLabel1 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            ((System.ComponentModel.ISupportInitialize)dgvEmailLog).BeginInit();
            panel1.SuspendLayout();
            guna2CustomGradientPanel1.SuspendLayout();
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
            btnRefresh.Location = new Point(266, 375);
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
            btnSendNow.Location = new Point(12, 375);
            btnSendNow.Name = "btnSendNow";
            btnSendNow.ShadowDecoration.CustomizableEdges = customizableEdges4;
            btnSendNow.Size = new Size(225, 56);
            btnSendNow.TabIndex = 4;
            btnSendNow.Text = "Send Email  Notifications";
            btnSendNow.Click += btnSendNow_Click;
            // 
            // lblEmailCount
            // 
            lblEmailCount.AutoSize = true;
            lblEmailCount.Font = new Font("Segoe UI Emoji", 13.8F, FontStyle.Bold);
            lblEmailCount.Location = new Point(350, 186);
            lblEmailCount.Name = "lblEmailCount";
            lblEmailCount.Size = new Size(148, 31);
            lblEmailCount.TabIndex = 6;
            lblEmailCount.Text = "Email Count";
            // 
            // lblLastRun
            // 
            lblLastRun.AutoSize = true;
            lblLastRun.Font = new Font("Segoe UI Emoji", 13.8F, FontStyle.Bold);
            lblLastRun.Location = new Point(350, 229);
            lblLastRun.Name = "lblLastRun";
            lblLastRun.Size = new Size(109, 31);
            lblLastRun.TabIndex = 8;
            lblLastRun.Text = "Last Run";
            // 
            // dtpFrom
            // 
            dtpFrom.Checked = true;
            dtpFrom.CustomizableEdges = customizableEdges5;
            dtpFrom.Font = new Font("Segoe UI", 9F);
            dtpFrom.Format = DateTimePickerFormat.Long;
            dtpFrom.Location = new Point(12, 186);
            dtpFrom.MaxDate = new DateTime(9998, 12, 31, 0, 0, 0, 0);
            dtpFrom.MinDate = new DateTime(1753, 1, 1, 0, 0, 0, 0);
            dtpFrom.Name = "dtpFrom";
            dtpFrom.ShadowDecoration.CustomizableEdges = customizableEdges6;
            dtpFrom.Size = new Size(250, 45);
            dtpFrom.TabIndex = 9;
            dtpFrom.Value = new DateTime(2025, 11, 10, 16, 28, 19, 306);
            // 
            // dtpTo
            // 
            dtpTo.Checked = true;
            dtpTo.CustomizableEdges = customizableEdges7;
            dtpTo.Font = new Font("Segoe UI", 9F);
            dtpTo.Format = DateTimePickerFormat.Long;
            dtpTo.Location = new Point(10, 273);
            dtpTo.MaxDate = new DateTime(9998, 12, 31, 0, 0, 0, 0);
            dtpTo.MinDate = new DateTime(1753, 1, 1, 0, 0, 0, 0);
            dtpTo.Name = "dtpTo";
            dtpTo.ShadowDecoration.CustomizableEdges = customizableEdges8;
            dtpTo.Size = new Size(250, 45);
            dtpTo.TabIndex = 10;
            dtpTo.Value = new DateTime(2025, 11, 10, 16, 28, 19, 306);
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 163);
            label1.Name = "label1";
            label1.Size = new Size(77, 20);
            label1.TabIndex = 11;
            label1.Text = "Date from";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(21, 234);
            label2.Name = "label2";
            label2.Size = new Size(61, 20);
            label2.TabIndex = 12;
            label2.Text = "Date To";
            // 
            // lblFilteredCount
            // 
            lblFilteredCount.AutoSize = true;
            lblFilteredCount.Font = new Font("Segoe UI Emoji", 13.8F, FontStyle.Bold);
            lblFilteredCount.Location = new Point(350, 273);
            lblFilteredCount.Name = "lblFilteredCount";
            lblFilteredCount.Size = new Size(242, 31);
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
            // guna2CustomGradientPanel1
            // 
            guna2CustomGradientPanel1.BorderRadius = 20;
            guna2CustomGradientPanel1.Controls.Add(arthanButton1);
            guna2CustomGradientPanel1.Controls.Add(guna2HtmlLabel2);
            guna2CustomGradientPanel1.Controls.Add(guna2HtmlLabel1);
            guna2CustomGradientPanel1.CustomizableEdges = customizableEdges9;
            guna2CustomGradientPanel1.FillColor = Color.FromArgb(220, 38, 38);
            guna2CustomGradientPanel1.FillColor2 = Color.FromArgb(220, 38, 38);
            guna2CustomGradientPanel1.FillColor3 = Color.FromArgb(238, 66, 66);
            guna2CustomGradientPanel1.FillColor4 = Color.FromArgb(238, 66, 66);
            guna2CustomGradientPanel1.Location = new Point(12, 13);
            guna2CustomGradientPanel1.Margin = new Padding(3, 4, 3, 4);
            guna2CustomGradientPanel1.Name = "guna2CustomGradientPanel1";
            guna2CustomGradientPanel1.ShadowDecoration.CustomizableEdges = customizableEdges10;
            guna2CustomGradientPanel1.Size = new Size(1878, 127);
            guna2CustomGradientPanel1.TabIndex = 18;
            // 
            // arthanButton1
            // 
            arthanButton1.BackColor = Color.Transparent;
            arthanButton1.BackgroundColor = Color.AliceBlue;
            arthanButton1.BorderColor = Color.PaleVioletRed;
            arthanButton1.BorderRadius = 10;
            arthanButton1.BottomLeftRadius = 10;
            arthanButton1.BottomRightRadius = 10;
            arthanButton1.CornerRadius = 10;
            arthanButton1.FlatAppearance.BorderColor = Color.Empty;
            arthanButton1.FlatAppearance.BorderSize = 0;
            arthanButton1.FlatAppearance.MouseDownBackColor = Color.Empty;
            arthanButton1.FlatAppearance.MouseOverBackColor = Color.Empty;
            arthanButton1.FlatStyle = FlatStyle.Flat;
            arthanButton1.Font = new Font("Sans Serif Collection", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            arthanButton1.ForeColor = Color.Black;
            arthanButton1.GradientEndColor = Color.White;
            arthanButton1.GradientStartColor = Color.White;
            arthanButton1.HoverEndColor = Color.FromArgb(147, 211, 251);
            arthanButton1.HoverStartColor = Color.FromArgb(86, 143, 190);
            arthanButton1.Image = null;
            arthanButton1.Location = new Point(1756, 35);
            arthanButton1.Margin = new Padding(3, 4, 3, 4);
            arthanButton1.Name = "arthanButton1";
            arthanButton1.Size = new Size(109, 53);
            arthanButton1.TabIndex = 19;
            arthanButton1.Text = "Home";
            arthanButton1.TextColor = Color.Black;
            arthanButton1.TopLeftRadius = 10;
            arthanButton1.TopRightRadius = 10;
            arthanButton1.UseVisualStyleBackColor = false;
            arthanButton1.Click += arthanButton1_Click;
            // 
            // guna2HtmlLabel2
            // 
            guna2HtmlLabel2.BackColor = Color.Transparent;
            guna2HtmlLabel2.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            guna2HtmlLabel2.ForeColor = Color.White;
            guna2HtmlLabel2.Location = new Point(22, 71);
            guna2HtmlLabel2.Margin = new Padding(3, 4, 3, 4);
            guna2HtmlLabel2.Name = "guna2HtmlLabel2";
            guna2HtmlLabel2.Size = new Size(246, 30);
            guna2HtmlLabel2.TabIndex = 1;
            guna2HtmlLabel2.Text = "See all the notifications sent";
            // 
            // guna2HtmlLabel1
            // 
            guna2HtmlLabel1.BackColor = Color.Transparent;
            guna2HtmlLabel1.Font = new Font("Segoe UI", 24F, FontStyle.Regular, GraphicsUnit.Point, 0);
            guna2HtmlLabel1.ForeColor = Color.White;
            guna2HtmlLabel1.Location = new Point(22, 17);
            guna2HtmlLabel1.Margin = new Padding(3, 4, 3, 4);
            guna2HtmlLabel1.Name = "guna2HtmlLabel1";
            guna2HtmlLabel1.Size = new Size(415, 56);
            guna2HtmlLabel1.TabIndex = 0;
            guna2HtmlLabel1.Text = "Dashboard Notification";
            // 
            // NotificationDashboard
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1902, 1033);
            Controls.Add(guna2CustomGradientPanel1);
            Controls.Add(panel1);
            Controls.Add(lblFilteredCount);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(dtpTo);
            Controls.Add(dtpFrom);
            Controls.Add(lblLastRun);
            Controls.Add(lblEmailCount);
            Controls.Add(btnSendNow);
            Controls.Add(btnRefresh);
            Name = "NotificationDashboard";
            Text = "NotificationDashboard";
            Load += NotificationDashboard_Load;
            ((System.ComponentModel.ISupportInitialize)dgvEmailLog).EndInit();
            panel1.ResumeLayout(false);
            guna2CustomGradientPanel1.ResumeLayout(false);
            guna2CustomGradientPanel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dgvEmailLog;
        private Guna.UI2.WinForms.Guna2Button btnRefresh;
        private Guna.UI2.WinForms.Guna2Button btnSendNow;
        private Label lblEmailCount;
        private Label lblLastRun;
        private Guna.UI2.WinForms.Guna2DateTimePicker dtpFrom;
        private Guna.UI2.WinForms.Guna2DateTimePicker dtpTo;
        private Label label1;
        private Label label2;
        private Label lblFilteredCount;
        private Panel panel1;
        private Guna.UI2.WinForms.Guna2CustomGradientPanel guna2CustomGradientPanel1;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel2;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel1;
        private LibraryCGC.Components.ArthanButton arthanButton1;
    }
}