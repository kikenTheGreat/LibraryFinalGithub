namespace Library_Final
{
    partial class ActivityLog
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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            DataGridActivity = new Guna.UI2.WinForms.Guna2DataGridView();
            arthanButton1 = new LibraryCGC.Components.ArthanButton();
            ((System.ComponentModel.ISupportInitialize)DataGridActivity).BeginInit();
            SuspendLayout();
            // 
            // DataGridActivity
            // 
            dataGridViewCellStyle1.BackColor = Color.White;
            DataGridActivity.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(100, 88, 255);
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle2.ForeColor = Color.White;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            DataGridActivity.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            DataGridActivity.ColumnHeadersHeight = 4;
            DataGridActivity.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = Color.White;
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle3.ForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle3.SelectionBackColor = Color.FromArgb(231, 229, 255);
            dataGridViewCellStyle3.SelectionForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            DataGridActivity.DefaultCellStyle = dataGridViewCellStyle3;
            DataGridActivity.GridColor = Color.FromArgb(231, 229, 255);
            DataGridActivity.Location = new Point(31, 190);
            DataGridActivity.Name = "DataGridActivity";
            DataGridActivity.RowHeadersVisible = false;
            DataGridActivity.RowHeadersWidth = 51;
            DataGridActivity.Size = new Size(1859, 470);
            DataGridActivity.TabIndex = 0;
            DataGridActivity.ThemeStyle.AlternatingRowsStyle.BackColor = Color.White;
            DataGridActivity.ThemeStyle.AlternatingRowsStyle.Font = null;
            DataGridActivity.ThemeStyle.AlternatingRowsStyle.ForeColor = Color.Empty;
            DataGridActivity.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = Color.Empty;
            DataGridActivity.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = Color.Empty;
            DataGridActivity.ThemeStyle.BackColor = Color.White;
            DataGridActivity.ThemeStyle.GridColor = Color.FromArgb(231, 229, 255);
            DataGridActivity.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(100, 88, 255);
            DataGridActivity.ThemeStyle.HeaderStyle.BorderStyle = DataGridViewHeaderBorderStyle.None;
            DataGridActivity.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 9F);
            DataGridActivity.ThemeStyle.HeaderStyle.ForeColor = Color.White;
            DataGridActivity.ThemeStyle.HeaderStyle.HeaightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            DataGridActivity.ThemeStyle.HeaderStyle.Height = 4;
            DataGridActivity.ThemeStyle.ReadOnly = false;
            DataGridActivity.ThemeStyle.RowsStyle.BackColor = Color.White;
            DataGridActivity.ThemeStyle.RowsStyle.BorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            DataGridActivity.ThemeStyle.RowsStyle.Font = new Font("Segoe UI", 9F);
            DataGridActivity.ThemeStyle.RowsStyle.ForeColor = Color.FromArgb(71, 69, 94);
            DataGridActivity.ThemeStyle.RowsStyle.Height = 29;
            DataGridActivity.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(231, 229, 255);
            DataGridActivity.ThemeStyle.RowsStyle.SelectionForeColor = Color.FromArgb(71, 69, 94);
            DataGridActivity.CellContentClick += DataGridActivity_CellContentClick;
            // 
            // arthanButton1
            // 
            arthanButton1.BackColor = Color.Transparent;
            arthanButton1.BackgroundColor = Color.AliceBlue;
            arthanButton1.BorderColor = Color.PaleVioletRed;
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
            arthanButton1.Location = new Point(1581, 62);
            arthanButton1.Margin = new Padding(3, 4, 3, 4);
            arthanButton1.Name = "arthanButton1";
            arthanButton1.Size = new Size(109, 53);
            arthanButton1.TabIndex = 6;
            arthanButton1.Text = "Home";
            arthanButton1.TextColor = Color.Black;
            arthanButton1.UseVisualStyleBackColor = false;
            arthanButton1.Click += arthanButton1_Click;
            // 
            // ActivityLog
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1902, 1033);
            Controls.Add(arthanButton1);
            Controls.Add(DataGridActivity);
            Name = "ActivityLog";
            Text = "ActivityLog";
            Load += ActivityLog_Load;
            ((System.ComponentModel.ISupportInitialize)DataGridActivity).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Guna.UI2.WinForms.Guna2DataGridView DataGridActivity;
        private LibraryCGC.Components.ArthanButton arthanButton1;
    }
}