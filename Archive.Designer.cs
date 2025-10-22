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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            label1 = new Label();
            pictureBox1 = new PictureBox();
            arthanPanel4 = new LibraryCGC.Components.ArthanPanel();
            arthanButton2 = new LibraryCGC.Components.ArthanButton();
            arthanPanel9 = new LibraryCGC.Components.ArthanPanel();
            txtISBNRestore = new Guna.UI2.WinForms.Guna2TextBox();
            btnRestoreBook = new Guna.UI2.WinForms.Guna2Button();
            pnlActiveBooks = new LibraryCGC.Components.ArthanPanel();
            label2 = new Label();
            DataGridTotalBooks = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            arthanPanel4.SuspendLayout();
            arthanPanel9.SuspendLayout();
            pnlActiveBooks.SuspendLayout();
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
            // arthanPanel4
            // 
            arthanPanel4.BackColor = Color.Transparent;
            arthanPanel4.BottomLeftRadius = 15;
            arthanPanel4.BottomRightRadius = 15;
            arthanPanel4.Controls.Add(arthanButton2);
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
            arthanPanel4.Size = new Size(1860, 133);
            arthanPanel4.TabIndex = 24;
            arthanPanel4.TopLeftRadius = 15;
            arthanPanel4.TopRightRadius = 15;
            arthanPanel4.UseIndividualCorners = false;
            // 
            // arthanButton2
            // 
            arthanButton2.BackColor = Color.Transparent;
            arthanButton2.BackgroundColor = Color.AliceBlue;
            arthanButton2.BorderColor = Color.PaleVioletRed;
            arthanButton2.FlatAppearance.BorderColor = Color.Empty;
            arthanButton2.FlatAppearance.BorderSize = 0;
            arthanButton2.FlatAppearance.MouseDownBackColor = Color.Empty;
            arthanButton2.FlatAppearance.MouseOverBackColor = Color.Empty;
            arthanButton2.FlatStyle = FlatStyle.Flat;
            arthanButton2.Font = new Font("Sans Serif Collection", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            arthanButton2.ForeColor = Color.Black;
            arthanButton2.GradientEndColor = Color.White;
            arthanButton2.GradientStartColor = Color.White;
            arthanButton2.HoverEndColor = Color.FromArgb(147, 211, 251);
            arthanButton2.HoverStartColor = Color.FromArgb(86, 143, 190);
            arthanButton2.Image = null;
            arthanButton2.Location = new Point(1735, 37);
            arthanButton2.Margin = new Padding(3, 4, 3, 4);
            arthanButton2.Name = "arthanButton2";
            arthanButton2.Size = new Size(109, 53);
            arthanButton2.TabIndex = 6;
            arthanButton2.Text = "Home";
            arthanButton2.TextColor = Color.Black;
            arthanButton2.UseVisualStyleBackColor = false;
            arthanButton2.Load += arthanButton2_Load;
            arthanButton2.Click += arthanButton2_Click;
            // 
            // arthanPanel9
            // 
            arthanPanel9.BackColor = Color.Transparent;
            arthanPanel9.BottomLeftRadius = 0;
            arthanPanel9.BottomRightRadius = 0;
            arthanPanel9.Controls.Add(txtISBNRestore);
            arthanPanel9.Controls.Add(btnRestoreBook);
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
            arthanPanel9.Size = new Size(1860, 113);
            arthanPanel9.TabIndex = 27;
            arthanPanel9.TopLeftRadius = 15;
            arthanPanel9.TopRightRadius = 15;
            arthanPanel9.UseIndividualCorners = true;
            arthanPanel9.Paint += arthanPanel9_Paint;
            // 
            // txtISBNRestore
            // 
            txtISBNRestore.BorderColor = Color.FromArgb(224, 224, 224);
            txtISBNRestore.BorderRadius = 4;
            txtISBNRestore.BorderStyle = System.Drawing.Drawing2D.DashStyle.Custom;
            txtISBNRestore.BorderThickness = 2;
            txtISBNRestore.CustomizableEdges = customizableEdges1;
            txtISBNRestore.DefaultText = "";
            txtISBNRestore.DisabledState.BorderColor = Color.FromArgb(208, 208, 208);
            txtISBNRestore.DisabledState.FillColor = Color.FromArgb(226, 226, 226);
            txtISBNRestore.DisabledState.ForeColor = Color.FromArgb(138, 138, 138);
            txtISBNRestore.DisabledState.PlaceholderForeColor = Color.FromArgb(138, 138, 138);
            txtISBNRestore.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            txtISBNRestore.Font = new Font("Segoe UI", 9F);
            txtISBNRestore.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            txtISBNRestore.Location = new Point(1239, 50);
            txtISBNRestore.Margin = new Padding(3, 4, 3, 4);
            txtISBNRestore.Name = "txtISBNRestore";
            txtISBNRestore.PlaceholderText = "";
            txtISBNRestore.SelectedText = "";
            txtISBNRestore.ShadowDecoration.CustomizableEdges = customizableEdges2;
            txtISBNRestore.Size = new Size(344, 39);
            txtISBNRestore.TabIndex = 34;
            // 
            // btnRestoreBook
            // 
            btnRestoreBook.BorderRadius = 18;
            btnRestoreBook.BorderStyle = System.Drawing.Drawing2D.DashStyle.Custom;
            btnRestoreBook.CustomizableEdges = customizableEdges3;
            btnRestoreBook.DisabledState.BorderColor = Color.DarkGray;
            btnRestoreBook.DisabledState.CustomBorderColor = Color.DarkGray;
            btnRestoreBook.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnRestoreBook.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnRestoreBook.FillColor = Color.Orange;
            btnRestoreBook.Font = new Font("Arial Rounded MT Bold", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnRestoreBook.ForeColor = Color.Black;
            btnRestoreBook.Location = new Point(852, 25);
            btnRestoreBook.Name = "btnRestoreBook";
            btnRestoreBook.ShadowDecoration.CustomizableEdges = customizableEdges4;
            btnRestoreBook.Size = new Size(157, 63);
            btnRestoreBook.TabIndex = 41;
            btnRestoreBook.Text = "Restore Book";
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
            pnlActiveBooks.Location = new Point(30, 171);
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
            label2.Size = new Size(150, 58);
            label2.TabIndex = 0;
            label2.Text = "Restore Books";
            // 
            // DataGridTotalBooks
            // 
            DataGridTotalBooks.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DataGridTotalBooks.Location = new Point(30, 308);
            DataGridTotalBooks.Name = "DataGridTotalBooks";
            DataGridTotalBooks.RowHeadersWidth = 51;
            DataGridTotalBooks.Size = new Size(1860, 703);
            DataGridTotalBooks.TabIndex = 28;
            // 
            // Archive
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1902, 1033);
            Controls.Add(DataGridTotalBooks);
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
            arthanPanel9.ResumeLayout(false);
            pnlActiveBooks.ResumeLayout(false);
            pnlActiveBooks.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)DataGridTotalBooks).EndInit();
            ResumeLayout(false);

        }

        #endregion

        private Label label1;
        private PictureBox pictureBox1;
        private Components.ArthanPanel arthanPanel4;
        private Components.ArthanPanel arthanPanel9;
        private Components.ArthanPanel pnlActiveBooks;
        private Label label2;
        private DataGridView DataGridTotalBooks;
        private Components.ArthanButton arthanButton2;
        private Guna.UI2.WinForms.Guna2Button btnRestoreBook;
        private Guna.UI2.WinForms.Guna2TextBox txtISBNRestore;
    }
}