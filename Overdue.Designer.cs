namespace Library_Final
{
    partial class Overdue
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
            dataGridView1 = new DataGridView();
            BorrowId = new DataGridViewTextBoxColumn();
            StudentName = new DataGridViewTextBoxColumn();
            BookTitle = new DataGridViewTextBoxColumn();
            DueDate = new DataGridViewTextBoxColumn();
            DateReturned = new DataGridViewTextBoxColumn();
            OverdueDays = new DataGridViewTextBoxColumn();
            Penalty = new DataGridViewTextBoxColumn();
            Status = new DataGridViewTextBoxColumn();
            kryptonButton5 = new Krypton.Toolkit.KryptonButton();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Columns.AddRange(new DataGridViewColumn[] { BorrowId, StudentName, BookTitle, DueDate, DateReturned, OverdueDays, Penalty, Status });
            dataGridView1.Location = new Point(27, 48);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersWidth = 51;
            dataGridView1.Size = new Size(1189, 472);
            dataGridView1.TabIndex = 0;
            dataGridView1.CellContentClick += dataGridView1_CellContentClick;
            // 
            // BorrowId
            // 
            BorrowId.HeaderText = "BorrowId";
            BorrowId.MinimumWidth = 6;
            BorrowId.Name = "BorrowId";
            BorrowId.Width = 125;
            // 
            // StudentName
            // 
            StudentName.HeaderText = "Student Name";
            StudentName.MinimumWidth = 6;
            StudentName.Name = "StudentName";
            StudentName.Width = 125;
            // 
            // BookTitle
            // 
            BookTitle.HeaderText = "Book Title";
            BookTitle.MinimumWidth = 6;
            BookTitle.Name = "BookTitle";
            BookTitle.Width = 125;
            // 
            // DueDate
            // 
            DueDate.HeaderText = "DueDate";
            DueDate.MinimumWidth = 6;
            DueDate.Name = "DueDate";
            DueDate.Width = 125;
            // 
            // DateReturned
            // 
            DateReturned.HeaderText = "Date Returned";
            DateReturned.MinimumWidth = 6;
            DateReturned.Name = "DateReturned";
            DateReturned.Width = 125;
            // 
            // OverdueDays
            // 
            OverdueDays.HeaderText = "Overdue Days";
            OverdueDays.MinimumWidth = 6;
            OverdueDays.Name = "OverdueDays";
            OverdueDays.Width = 125;
            // 
            // Penalty
            // 
            Penalty.HeaderText = "Penalty";
            Penalty.MinimumWidth = 6;
            Penalty.Name = "Penalty";
            Penalty.Width = 125;
            // 
            // Status
            // 
            Status.HeaderText = "Status";
            Status.MinimumWidth = 6;
            Status.Name = "Status";
            Status.Width = 125;
            // 
            // kryptonButton5
            // 
            kryptonButton5.Location = new Point(1124, 9);
            kryptonButton5.Margin = new Padding(3, 4, 3, 4);
            kryptonButton5.Name = "kryptonButton5";
            kryptonButton5.Size = new Size(82, 32);
            kryptonButton5.StateCommon.Back.Color1 = Color.Teal;
            kryptonButton5.StateCommon.Back.Color2 = Color.Teal;
            kryptonButton5.StateCommon.Border.Rounding = 15F;
            kryptonButton5.StateCommon.Content.ShortText.Color1 = Color.White;
            kryptonButton5.StateCommon.Content.ShortText.Font = new Font("Rockwell", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            kryptonButton5.TabIndex = 68;
            kryptonButton5.Values.DropDownArrowColor = Color.Empty;
            kryptonButton5.Values.Text = "Home";
            kryptonButton5.Click += kryptonButton5_Click;
            // 
            // Overdue
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1242, 546);
            Controls.Add(kryptonButton5);
            Controls.Add(dataGridView1);
            Name = "Overdue";
            Text = "Overdue";
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dataGridView1;
        private DataGridViewTextBoxColumn BorrowId;
        private DataGridViewTextBoxColumn StudentName;
        private DataGridViewTextBoxColumn BookTitle;
        private DataGridViewTextBoxColumn DueDate;
        private DataGridViewTextBoxColumn DateReturned;
        private DataGridViewTextBoxColumn OverdueDays;
        private DataGridViewTextBoxColumn Penalty;
        private DataGridViewTextBoxColumn Status;
        private Krypton.Toolkit.KryptonButton kryptonButton5;
    }
}