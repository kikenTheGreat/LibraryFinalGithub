namespace Library_Final
{
    partial class ArchiveBooks
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
            DataGridTotalBooks = new DataGridView();
            kryptonButton1 = new Krypton.Toolkit.KryptonButton();
            ((System.ComponentModel.ISupportInitialize)DataGridTotalBooks).BeginInit();
            SuspendLayout();
            // 
            // DataGridTotalBooks
            // 
            DataGridTotalBooks.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DataGridTotalBooks.Location = new Point(4, 87);
            DataGridTotalBooks.Name = "DataGridTotalBooks";
            DataGridTotalBooks.RowHeadersWidth = 51;
            DataGridTotalBooks.Size = new Size(1244, 355);
            DataGridTotalBooks.TabIndex = 0;
            DataGridTotalBooks.CellContentClick += dataGridView1_CellContentClick;
            // 
            // kryptonButton1
            // 
            kryptonButton1.Location = new Point(732, 9);
            kryptonButton1.Name = "kryptonButton1";
            kryptonButton1.Size = new Size(112, 31);
            kryptonButton1.TabIndex = 2;
            kryptonButton1.Values.DropDownArrowColor = Color.Empty;
            kryptonButton1.Values.Text = "Back";
            kryptonButton1.Click += kryptonButton1_Click;
            // 
            // ArchiveBooks
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1250, 454);
            Controls.Add(kryptonButton1);
            Controls.Add(DataGridTotalBooks);
            Name = "ArchiveBooks";
            Text = "ArchiveBooks";
            Load += ArchiveBooks_Load;
            ((System.ComponentModel.ISupportInitialize)DataGridTotalBooks).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView DataGridTotalBooks;
        private Krypton.Toolkit.KryptonButton kryptonButton1;
    }
}