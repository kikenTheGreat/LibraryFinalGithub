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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            label1 = new Label();
            pictureBox1 = new PictureBox();
            arthanPanel4 = new LibraryCGC.Components.ArthanPanel();
            arthanButton2 = new LibraryCGC.Components.ArthanButton();
            arthanPanel9 = new LibraryCGC.Components.ArthanPanel();
            RestoreQty = new Guna.UI2.WinForms.Guna2NumericUpDown();
            guna2HtmlLabel1 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            txtArchiveISBN = new Guna.UI2.WinForms.Guna2TextBox();
            btnArchiveBook = new Guna.UI2.WinForms.Guna2Button();
            DataGridTotalBooks = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            arthanPanel4.SuspendLayout();
            arthanPanel9.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)RestoreQty).BeginInit();
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
            arthanPanel4.Size = new Size(1861, 133);
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
            arthanButton2.BorderRadius = 10;
            arthanButton2.BottomLeftRadius = 10;
            arthanButton2.BottomRightRadius = 10;
            arthanButton2.CornerRadius = 10;
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
            arthanButton2.Location = new Point(1715, 36);
            arthanButton2.Margin = new Padding(3, 4, 3, 4);
            arthanButton2.Name = "arthanButton2";
            arthanButton2.Size = new Size(109, 53);
            arthanButton2.TabIndex = 6;
            arthanButton2.Text = "Home";
            arthanButton2.TextColor = Color.Black;
            arthanButton2.TopLeftRadius = 10;
            arthanButton2.TopRightRadius = 10;
            arthanButton2.UseVisualStyleBackColor = false;
            arthanButton2.Load += arthanButton2_Load;
            arthanButton2.Click += arthanButton2_Click;
            // 
            // arthanPanel9
            // 
            arthanPanel9.BackColor = Color.Transparent;
            arthanPanel9.BottomLeftRadius = 0;
            arthanPanel9.BottomRightRadius = 0;
            arthanPanel9.Controls.Add(RestoreQty);
            arthanPanel9.Controls.Add(guna2HtmlLabel1);
            arthanPanel9.Controls.Add(txtArchiveISBN);
            arthanPanel9.Controls.Add(btnArchiveBook);
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
            arthanPanel9.Size = new Size(727, 113);
            arthanPanel9.TabIndex = 27;
            arthanPanel9.TopLeftRadius = 15;
            arthanPanel9.TopRightRadius = 15;
            arthanPanel9.UseIndividualCorners = true;
            arthanPanel9.Paint += arthanPanel9_Paint;
            // 
            // RestoreQty
            // 
            RestoreQty.BackColor = Color.Transparent;
            RestoreQty.CustomizableEdges = customizableEdges1;
            RestoreQty.Font = new Font("Segoe UI", 9F);
            RestoreQty.Location = new Point(395, 33);
            RestoreQty.Margin = new Padding(3, 5, 3, 5);
            RestoreQty.Name = "RestoreQty";
            RestoreQty.ShadowDecoration.CustomizableEdges = customizableEdges2;
            RestoreQty.Size = new Size(125, 51);
            RestoreQty.TabIndex = 80;
            RestoreQty.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // guna2HtmlLabel1
            // 
            guna2HtmlLabel1.BackColor = Color.Transparent;
            guna2HtmlLabel1.Location = new Point(23, 21);
            guna2HtmlLabel1.Name = "guna2HtmlLabel1";
            guna2HtmlLabel1.Size = new Size(73, 22);
            guna2HtmlLabel1.TabIndex = 42;
            guna2HtmlLabel1.Text = "Enter ISBN";
            // 
            // txtArchiveISBN
            // 
            txtArchiveISBN.BorderColor = Color.FromArgb(224, 224, 224);
            txtArchiveISBN.BorderRadius = 4;
            txtArchiveISBN.BorderStyle = System.Drawing.Drawing2D.DashStyle.Custom;
            txtArchiveISBN.BorderThickness = 2;
            txtArchiveISBN.CustomizableEdges = customizableEdges3;
            txtArchiveISBN.DefaultText = "";
            txtArchiveISBN.DisabledState.BorderColor = Color.FromArgb(208, 208, 208);
            txtArchiveISBN.DisabledState.FillColor = Color.FromArgb(226, 226, 226);
            txtArchiveISBN.DisabledState.ForeColor = Color.FromArgb(138, 138, 138);
            txtArchiveISBN.DisabledState.PlaceholderForeColor = Color.FromArgb(138, 138, 138);
            txtArchiveISBN.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            txtArchiveISBN.Font = new Font("Segoe UI", 9F);
            txtArchiveISBN.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            txtArchiveISBN.Location = new Point(23, 51);
            txtArchiveISBN.Margin = new Padding(3, 5, 3, 5);
            txtArchiveISBN.Name = "txtArchiveISBN";
            txtArchiveISBN.PlaceholderText = "";
            txtArchiveISBN.SelectedText = "";
            txtArchiveISBN.ShadowDecoration.CustomizableEdges = customizableEdges4;
            txtArchiveISBN.Size = new Size(344, 39);
            txtArchiveISBN.TabIndex = 34;
            // 
            // btnArchiveBook
            // 
            btnArchiveBook.BorderRadius = 18;
            btnArchiveBook.BorderStyle = System.Drawing.Drawing2D.DashStyle.Custom;
            btnArchiveBook.CustomizableEdges = customizableEdges5;
            btnArchiveBook.DisabledState.BorderColor = Color.DarkGray;
            btnArchiveBook.DisabledState.CustomBorderColor = Color.DarkGray;
            btnArchiveBook.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnArchiveBook.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnArchiveBook.FillColor = Color.Orange;
            btnArchiveBook.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnArchiveBook.ForeColor = Color.Black;
            btnArchiveBook.Location = new Point(538, 27);
            btnArchiveBook.Name = "btnArchiveBook";
            btnArchiveBook.ShadowDecoration.CustomizableEdges = customizableEdges6;
            btnArchiveBook.Size = new Size(157, 63);
            btnArchiveBook.TabIndex = 41;
            btnArchiveBook.Text = "Unarchived Book";
            btnArchiveBook.Click += btnRestoreBook_Click;
            // 
            // DataGridTotalBooks
            // 
            DataGridTotalBooks.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DataGridTotalBooks.Location = new Point(30, 308);
            DataGridTotalBooks.Name = "DataGridTotalBooks";
            DataGridTotalBooks.RowHeadersWidth = 51;
            DataGridTotalBooks.Size = new Size(1861, 703);
            DataGridTotalBooks.TabIndex = 28;
            // 
            // Archive
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1902, 1033);
            Controls.Add(DataGridTotalBooks);
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
            arthanPanel9.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)RestoreQty).EndInit();
            ((System.ComponentModel.ISupportInitialize)DataGridTotalBooks).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Label label1;
        private PictureBox pictureBox1;
        private Components.ArthanPanel arthanPanel4;
        private Components.ArthanPanel arthanPanel9;
        private DataGridView DataGridTotalBooks;
        private Components.ArthanButton arthanButton2;
        private Guna.UI2.WinForms.Guna2Button btnArchiveBook;
        private Guna.UI2.WinForms.Guna2TextBox txtArchiveISBN;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel1;
        private Guna.UI2.WinForms.Guna2NumericUpDown RestoreQty;
    }
}