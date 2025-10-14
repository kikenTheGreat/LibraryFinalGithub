namespace LibraryCGC
{
    partial class Archive
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Archive));
            label1 = new Label();
            pictureBox1 = new PictureBox();
            arthanButton1 = new LibraryCGC.Components.ArthanButton();
            arthanPanel4 = new LibraryCGC.Components.ArthanPanel();
            arthanPanel9 = new LibraryCGC.Components.ArthanPanel();
            pnlActiveBooks = new LibraryCGC.Components.ArthanPanel();
            label2 = new Label();
            pnlArchivedBooks = new LibraryCGC.Components.ArthanPanel();
            label3 = new Label();
            DataGridTotalBooks = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            arthanPanel4.SuspendLayout();
            pnlActiveBooks.SuspendLayout();
            pnlArchivedBooks.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DataGridTotalBooks).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Sans Serif Collection", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.White;
            label1.Location = new Point(96, 25);
            label1.Name = "label1";
            label1.Size = new Size(248, 102);
            label1.TabIndex = 0;
            label1.Text = "Archive Books";
            // 
            // pictureBox1
            // 
            pictureBox1.BackgroundImage = (Image)resources.GetObject("pictureBox1.BackgroundImage");
            pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox1.Location = new Point(11, 13);
            pictureBox1.Margin = new Padding(3, 4, 3, 4);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(78, 91);
            pictureBox1.TabIndex = 5;
            pictureBox1.TabStop = false;
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
            arthanButton1.Location = new Point(1650, 34);
            arthanButton1.Margin = new Padding(3, 4, 3, 4);
            arthanButton1.Name = "arthanButton1";
            arthanButton1.Size = new Size(109, 53);
            arthanButton1.TabIndex = 5;
            arthanButton1.Text = "Home";
            arthanButton1.TextColor = Color.Black;
            arthanButton1.UseVisualStyleBackColor = false;
            arthanButton1.Load += arthanButton1_Load;
            arthanButton1.Click += arthanButton1_Click;
            // 
            // arthanPanel4
            // 
            arthanPanel4.BackColor = Color.Transparent;
            arthanPanel4.BottomLeftRadius = 15;
            arthanPanel4.BottomRightRadius = 15;
            arthanPanel4.Controls.Add(arthanButton1);
            arthanPanel4.Controls.Add(pictureBox1);
            arthanPanel4.Controls.Add(label1);
            arthanPanel4.CornerRadius = 15;
            arthanPanel4.EnableDragging = false;
            arthanPanel4.EnableDropShadow = true;
            arthanPanel4.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            arthanPanel4.GradientEndColor = Color.FromArgb(238, 66, 66);
            arthanPanel4.GradientStartColor = Color.FromArgb(220, 38, 38);
            arthanPanel4.Location = new Point(30, 16);
            arthanPanel4.Margin = new Padding(3, 4, 3, 4);
            arthanPanel4.Name = "arthanPanel4";
            arthanPanel4.ShadowBlur = 5;
            arthanPanel4.ShadowColor = Color.FromArgb(50, 0, 0, 0);
            arthanPanel4.ShadowOffset = 3;
            arthanPanel4.Size = new Size(1779, 133);
            arthanPanel4.TabIndex = 24;
            arthanPanel4.TopLeftRadius = 15;
            arthanPanel4.TopRightRadius = 15;
            arthanPanel4.UseIndividualCorners = false;
            // 
            // arthanPanel9
            // 
            arthanPanel9.BackColor = Color.Transparent;
            arthanPanel9.BottomLeftRadius = 0;
            arthanPanel9.BottomRightRadius = 0;
            arthanPanel9.CornerRadius = 0;
            arthanPanel9.EnableDragging = false;
            arthanPanel9.EnableDropShadow = true;
            arthanPanel9.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            arthanPanel9.GradientEndColor = Color.WhiteSmoke;
            arthanPanel9.GradientStartColor = Color.WhiteSmoke;
            arthanPanel9.Location = new Point(30, 175);
            arthanPanel9.Margin = new Padding(3, 4, 3, 4);
            arthanPanel9.Name = "arthanPanel9";
            arthanPanel9.ShadowBlur = 5;
            arthanPanel9.ShadowColor = Color.FromArgb(50, 0, 0, 0);
            arthanPanel9.ShadowOffset = 0;
            arthanPanel9.Size = new Size(1704, 113);
            arthanPanel9.TabIndex = 27;
            arthanPanel9.TopLeftRadius = 15;
            arthanPanel9.TopRightRadius = 15;
            arthanPanel9.UseIndividualCorners = true;
            // 
            // pnlActiveBooks
            // 
            pnlActiveBooks.BackColor = Color.Transparent;
            pnlActiveBooks.BottomLeftRadius = 0;
            pnlActiveBooks.BottomRightRadius = 0;
            pnlActiveBooks.Controls.Add(label2);
            pnlActiveBooks.CornerRadius = 0;
            pnlActiveBooks.EnableDragging = false;
            pnlActiveBooks.EnableDropShadow = true;
            pnlActiveBooks.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            pnlActiveBooks.GradientEndColor = Color.White;
            pnlActiveBooks.GradientStartColor = Color.White;
            pnlActiveBooks.Location = new Point(58, 175);
            pnlActiveBooks.Margin = new Padding(3, 4, 3, 4);
            pnlActiveBooks.Name = "pnlActiveBooks";
            pnlActiveBooks.ShadowBlur = 0;
            pnlActiveBooks.ShadowColor = Color.FromArgb(50, 0, 0, 0);
            pnlActiveBooks.ShadowOffset = 0;
            pnlActiveBooks.Size = new Size(210, 113);
            pnlActiveBooks.TabIndex = 0;
            pnlActiveBooks.TopLeftRadius = 0;
            pnlActiveBooks.TopRightRadius = 0;
            pnlActiveBooks.UseIndividualCorners = false;
            pnlActiveBooks.Paint += pnlActiveBooks_Paint;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Sans Serif Collection", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.Location = new Point(21, 35);
            label2.Name = "label2";
            label2.Size = new Size(135, 58);
            label2.TabIndex = 0;
            label2.Text = "Active Books";
            // 
            // pnlArchivedBooks
            // 
            pnlArchivedBooks.BackColor = Color.Transparent;
            pnlArchivedBooks.BottomLeftRadius = 0;
            pnlArchivedBooks.BottomRightRadius = 0;
            pnlArchivedBooks.Controls.Add(label3);
            pnlArchivedBooks.CornerRadius = 0;
            pnlArchivedBooks.EnableDragging = false;
            pnlArchivedBooks.EnableDropShadow = true;
            pnlArchivedBooks.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            pnlArchivedBooks.GradientEndColor = Color.White;
            pnlArchivedBooks.GradientStartColor = Color.White;
            pnlArchivedBooks.Location = new Point(286, 175);
            pnlArchivedBooks.Margin = new Padding(3, 4, 3, 4);
            pnlArchivedBooks.Name = "pnlArchivedBooks";
            pnlArchivedBooks.ShadowBlur = 0;
            pnlArchivedBooks.ShadowColor = Color.FromArgb(50, 0, 0, 0);
            pnlArchivedBooks.ShadowOffset = 0;
            pnlArchivedBooks.Size = new Size(239, 113);
            pnlArchivedBooks.TabIndex = 1;
            pnlArchivedBooks.TopLeftRadius = 0;
            pnlArchivedBooks.TopRightRadius = 0;
            pnlArchivedBooks.UseIndividualCorners = false;
            pnlArchivedBooks.Paint += pnlArchivedBooks_Paint;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Sans Serif Collection", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.Location = new Point(21, 35);
            label3.Name = "label3";
            label3.Size = new Size(159, 58);
            label3.TabIndex = 0;
            label3.Text = "Archived Books";
            // 
            // DataGridTotalBooks
            // 
            DataGridTotalBooks.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DataGridTotalBooks.Location = new Point(30, 308);
            DataGridTotalBooks.Name = "DataGridTotalBooks";
            DataGridTotalBooks.RowHeadersWidth = 51;
            DataGridTotalBooks.Size = new Size(1704, 536);
            DataGridTotalBooks.TabIndex = 28;
            DataGridTotalBooks.CellContentClick += DataGridTotalBooks_CellContentClick;
            // 
            // Archive
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1821, 1055);
            Controls.Add(DataGridTotalBooks);
            Controls.Add(pnlArchivedBooks);
            Controls.Add(pnlActiveBooks);
            Controls.Add(arthanPanel9);
            Controls.Add(arthanPanel4);
            Margin = new Padding(3, 4, 3, 4);
            Name = "Archive";
            Text = "Archive";
            Load += Archive_Load_1;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            arthanPanel4.ResumeLayout(false);
            arthanPanel4.PerformLayout();
            pnlActiveBooks.ResumeLayout(false);
            pnlActiveBooks.PerformLayout();
            pnlArchivedBooks.ResumeLayout(false);
            pnlArchivedBooks.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)DataGridTotalBooks).EndInit();
            ResumeLayout(false);

        }

        #endregion

        private Label label1;
        private PictureBox pictureBox1;
        private Components.ArthanButton arthanButton1;
        private Components.ArthanPanel arthanPanel4;
        private Components.ArthanPanel arthanPanel9;
        private Components.ArthanPanel pnlActiveBooks;
        private Label label2;
        private Components.ArthanPanel pnlArchivedBooks;
        private Label label3;
        private DataGridView DataGridTotalBooks;
    }
}