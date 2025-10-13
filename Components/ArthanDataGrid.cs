using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryCGC.Components
{
    public class ArthanDataGrid : DataGridView
    {
        private Color _headerBackColor = Color.FromArgb(248, 249, 250);
        private Color _headerForeColor = Color.FromArgb(108, 117, 125);
        private Color _alternateRowColor = Color.FromArgb(252, 253, 254);
        private Color _rowBackColor = Color.White;
        private Color _gridLineColor = Color.FromArgb(233, 236, 239);
        private Color _selectionBackColor = Color.FromArgb(230, 247, 255);
        private Color _selectionForeColor = Color.FromArgb(33, 37, 41);
        private Font _headerFont = new Font("Segoe UI", 9F, FontStyle.Regular);
        private Font _cellFont = new Font("Segoe UI", 9F, FontStyle.Regular);
        private bool _showStatusColumn = true;

        public ArthanDataGrid()
        {
            InitializeComponent();
            ApplyModernStyling();
        }

        #region Custom Properties

        [Category("Appearance")]
        [Description("Background color of the header")]
        public Color HeaderBackColor
        {
            get { return _headerBackColor; }
            set { _headerBackColor = value; ApplyHeaderStyling(); }
        }

        [Category("Appearance")]
        [Description("Text color of the header")]
        public Color HeaderForeColor
        {
            get { return _headerForeColor; }
            set { _headerForeColor = value; ApplyHeaderStyling(); }
        }

        [Category("Appearance")]
        [Description("Background color of alternate rows")]
        public Color AlternateRowColor
        {
            get { return _alternateRowColor; }
            set { _alternateRowColor = value; AlternatingRowsDefaultCellStyle.BackColor = value; }
        }

        [Category("Appearance")]
        [Description("Background color of regular rows")]
        public Color RowBackColor
        {
            get { return _rowBackColor; }
            set { _rowBackColor = value; DefaultCellStyle.BackColor = value; }
        }

        [Category("Appearance")]
        [Description("Color of grid lines")]
        public Color GridLineColor
        {
            get { return _gridLineColor; }
            set { _gridLineColor = value; GridColor = value; }
        }

        [Category("Appearance")]
        [Description("Show status column with colored indicators")]
        public bool ShowStatusColumn
        {
            get { return _showStatusColumn; }
            set { _showStatusColumn = value; }
        }

        #endregion

        private void InitializeComponent()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.DoubleBuffer |
                     ControlStyles.ResizeRedraw, true);
        }

        private void ApplyModernStyling()
        {
            // Basic appearance
            BorderStyle = BorderStyle.None;
            CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            GridColor = _gridLineColor;
            BackgroundColor = Color.White;

            // Header styling
            EnableHeadersVisualStyles = false;
            ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            ColumnHeadersHeight = 40;

            // Row styling
            RowHeadersVisible = false;
            AllowUserToAddRows = false;
            AllowUserToDeleteRows = false;
            AllowUserToResizeRows = false;
            RowTemplate.Height = 45;

            // Selection styling
            SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            MultiSelect = false;

            // Fonts
            Font = _cellFont;
            ColumnHeadersDefaultCellStyle.Font = _headerFont;

            ApplyHeaderStyling();
            ApplyCellStyling();
        }

        private void ApplyHeaderStyling()
        {
            ColumnHeadersDefaultCellStyle.BackColor = _headerBackColor;
            ColumnHeadersDefaultCellStyle.ForeColor = _headerForeColor;
            ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            ColumnHeadersDefaultCellStyle.Padding = new Padding(15, 0, 0, 0);
            ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
        }

        private void ApplyCellStyling()
        {
            // Default cell style
            DefaultCellStyle.BackColor = _rowBackColor;
            DefaultCellStyle.ForeColor = Color.FromArgb(33, 37, 41);
            DefaultCellStyle.SelectionBackColor = _selectionBackColor;
            DefaultCellStyle.SelectionForeColor = _selectionForeColor;
            DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            DefaultCellStyle.Padding = new Padding(15, 8, 8, 8);
            DefaultCellStyle.WrapMode = DataGridViewTriState.False;

            // Alternate row style
            AlternatingRowsDefaultCellStyle.BackColor = _alternateRowColor;
            AlternatingRowsDefaultCellStyle.ForeColor = Color.FromArgb(33, 37, 41);
            AlternatingRowsDefaultCellStyle.SelectionBackColor = _selectionBackColor;
            AlternatingRowsDefaultCellStyle.SelectionForeColor = _selectionForeColor;
            AlternatingRowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            AlternatingRowsDefaultCellStyle.Padding = new Padding(15, 8, 8, 8);
        }

        protected override void OnCellPainting(DataGridViewCellPaintingEventArgs e)
        {
            // Custom painting for status column (if enabled)
            if (_showStatusColumn && e.ColumnIndex == 0 && e.RowIndex >= 0)
            {
                // Paint the cell background
                e.PaintBackground(e.ClipBounds, true);

                // Draw status indicator
                if (e.Value != null)
                {
                    string status = e.Value.ToString().ToLower();
                    Color statusColor = GetStatusColor(status);

                    Rectangle statusRect = new Rectangle(
                        e.CellBounds.X + 15,
                        e.CellBounds.Y + (e.CellBounds.Height - 20) / 2,
                        60,
                        20);

                    using (SolidBrush brush = new SolidBrush(statusColor))
                    using (SolidBrush textBrush = new SolidBrush(Color.White))
                    {
                        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        e.Graphics.FillRoundedRectangle(brush, statusRect, 10);

                        string displayText = status.Substring(0, 1).ToUpper() + status.Substring(1);
                        SizeF textSize = e.Graphics.MeasureString(displayText, new Font("Segoe UI", 7F, FontStyle.Bold));
                        float textX = statusRect.X + (statusRect.Width - textSize.Width) / 2;
                        float textY = statusRect.Y + (statusRect.Height - textSize.Height) / 2;

                        e.Graphics.DrawString(displayText, new Font("Segoe UI", 7F, FontStyle.Bold),
                                            textBrush, textX, textY);
                    }
                }

                e.Handled = true;
                return;
            }

            base.OnCellPainting(e);
        }

        private Color GetStatusColor(string status)
        {
            switch (status)
            {
                case "overdue":
                    return Color.FromArgb(220, 53, 69);  // Red
                case "due soon":
                    return Color.FromArgb(255, 193, 7);  // Yellow
                case "returned":
                    return Color.FromArgb(25, 135, 84);  // Green
                case "active":
                    return Color.FromArgb(13, 110, 253); // Blue
                default:
                    return Color.FromArgb(108, 117, 125); // Gray
            }
        }

        /// <summary>
        /// Add sample data for demonstration
        /// </summary>
        public void LoadSampleData()
        {
            // Clear existing data
            DataSource = null;
            Columns.Clear();

            // Add columns
            if (_showStatusColumn)
            {
                DataGridViewTextBoxColumn statusColumn = new DataGridViewTextBoxColumn();
                statusColumn.Name = "Status";
                statusColumn.HeaderText = "";
                statusColumn.Width = 100;
                statusColumn.ReadOnly = true;
                Columns.Add(statusColumn);
            }

            DataGridViewTextBoxColumn nameColumn = new DataGridViewTextBoxColumn();
            nameColumn.Name = "BorrowerName";
            nameColumn.HeaderText = "Borrower Name";
            nameColumn.Width = 150;
            nameColumn.ReadOnly = true;
            Columns.Add(nameColumn);

            DataGridViewTextBoxColumn bookColumn = new DataGridViewTextBoxColumn();
            bookColumn.Name = "BookTitle";
            bookColumn.HeaderText = "Book Title";
            bookColumn.Width = 200;
            bookColumn.ReadOnly = true;
            Columns.Add(bookColumn);

            DataGridViewTextBoxColumn isbnColumn = new DataGridViewTextBoxColumn();
            isbnColumn.Name = "ISBN";
            isbnColumn.HeaderText = "ISBN";
            isbnColumn.Width = 120;
            isbnColumn.ReadOnly = true;
            Columns.Add(isbnColumn);

            DataGridViewTextBoxColumn dueDateColumn = new DataGridViewTextBoxColumn();
            dueDateColumn.Name = "DueDate";
            dueDateColumn.HeaderText = "Due Date";
            dueDateColumn.Width = 100;
            dueDateColumn.ReadOnly = true;
            Columns.Add(dueDateColumn);

            DataGridViewTextBoxColumn daysOverdueColumn = new DataGridViewTextBoxColumn();
            daysOverdueColumn.Name = "DaysOverdue";
            daysOverdueColumn.HeaderText = "Days Overdue";
            daysOverdueColumn.Width = 100;
            daysOverdueColumn.ReadOnly = true;
            Columns.Add(daysOverdueColumn);

            // Add sample rows
            List<object[]> sampleData = new List<object[]>
            {
                new object[] { "Overdue", "Kent Lusdoc", "Harry Potter", "000000000", "March 9, 2025", "30" },
                new object[] { "Due Soon", "Maria Santos", "The Great Gatsby", "9780743273565", "August 20, 2025", "2" },
                new object[] { "Active", "John Smith", "To Kill a Mockingbird", "9780061120084", "August 25, 2025", "0" },
                new object[] { "Overdue", "Lisa Chen", "1984", "9780451524935", "March 1, 2025", "38" },
                new object[] { "Active", "Michael Johnson", "Pride and Prejudice", "9780141439518", "August 30, 2025", "0" }
            };

            foreach (object[] row in sampleData)
            {
                Rows.Add(row);
            }

            // Auto-size columns to fill the width
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
    }

    // Extension method for rounded rectangles
    public static class GraphicsExtensions
    {
        public static void FillRoundedRectangle(this Graphics graphics, Brush brush, Rectangle bounds, int radius)
        {
            using (GraphicsPath path = CreateRoundedRectangle(bounds, radius))
            {
                graphics.FillPath(brush, path);
            }
        }

        private static GraphicsPath CreateRoundedRectangle(Rectangle bounds, int radius)
        {
            GraphicsPath path = new GraphicsPath();

            if (radius <= 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            int diameter = radius * 2;
            Rectangle arc = new Rectangle(bounds.Location, new Size(diameter, diameter));

            // Top left arc
            path.AddArc(arc, 180, 90);

            // Top right arc
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);

            // Bottom right arc
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // Bottom left arc
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }
    }

    // Data model classes
    public class BookPenalty
    {
        public string Status { get; set; }
        public string BorrowerName { get; set; }
        public string BookTitle { get; set; }
        public string ISBN { get; set; }
        public string DueDate { get; set; }
        public int DaysOverdue { get; set; }
    }

    public class LibraryDataService
    {
        public static List<BookPenalty> GetBookPenalties()
        {
            return new List<BookPenalty>
            {
                new BookPenalty
                {
                    Status = "Overdue",
                    BorrowerName = "Kent Lusdoc",
                    BookTitle = "Harry Potter",
                    ISBN = "000000000",
                    DueDate = "March 9, 2025",
                    DaysOverdue = 30
                },
                new BookPenalty
                {
                    Status = "Due Soon",
                    BorrowerName = "Maria Santos",
                    BookTitle = "The Great Gatsby",
                    ISBN = "9780743273565",
                    DueDate = "August 20, 2025",
                    DaysOverdue = 0
                },
                new BookPenalty
                {
                    Status = "Active",
                    BorrowerName = "John Smith",
                    BookTitle = "To Kill a Mockingbird",
                    ISBN = "9780061120084",
                    DueDate = "August 25, 2025",
                    DaysOverdue = 0
                }
            };
        }

        /// <summary>
        /// Bind data to the grid using data binding
        /// </summary>
        /// <param name="dataGrid">The data grid to bind to</param>
        public static void BindPenaltiesToGrid(ArthanDataGrid dataGrid)
        {
            var penalties = GetBookPenalties();
            dataGrid.DataSource = penalties;

            // Customize column headers and widths after binding
            if (dataGrid.Columns["Status"] != null)
            {
                dataGrid.Columns["Status"].HeaderText = "";
                dataGrid.Columns["Status"].Width = 80;
            }

            if (dataGrid.Columns["BorrowerName"] != null)
            {
                dataGrid.Columns["BorrowerName"].HeaderText = "Borrower Name";
                dataGrid.Columns["BorrowerName"].Width = 150;
            }

            if (dataGrid.Columns["BookTitle"] != null)
            {
                dataGrid.Columns["BookTitle"].HeaderText = "Book Title";
                dataGrid.Columns["BookTitle"].Width = 200;
            }

            if (dataGrid.Columns["ISBN"] != null)
            {
                dataGrid.Columns["ISBN"].HeaderText = "ISBN";
                dataGrid.Columns["ISBN"].Width = 120;
            }

            if (dataGrid.Columns["DueDate"] != null)
            {
                dataGrid.Columns["DueDate"].HeaderText = "Due Date";
                dataGrid.Columns["DueDate"].Width = 100;
            }

            if (dataGrid.Columns["DaysOverdue"] != null)
            {
                dataGrid.Columns["DaysOverdue"].HeaderText = "Days Overdue";
                dataGrid.Columns["DaysOverdue"].Width = 100;
            }
        }
    }

    // Example usage in a form
    public partial class ExampleFormWithDataGrid : Form
    {
        private ArthanDataGrid _dataGrid;

        public ExampleFormWithDataGrid()
        {
            InitializeComponent();
            CreateDataGrid();
        }

        private void CreateDataGrid()
        {
            // Create the simple data grid
            _dataGrid = new ArthanDataGrid()
            {
                Location = new Point(20, 20),
                Size = new Size(660, 300),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };

            // Load sample data
            _dataGrid.LoadSampleData();

            // Or use data binding:
            // LibraryDataService.BindPenaltiesToGrid(_dataGrid);

            Controls.Add(_dataGrid);

            // Example: Add to your existing ArthanPanel
            /*
            ArthanPanel myPanel = new ArthanPanel()
            {
                Size = new Size(700, 350),
                Location = new Point(50, 50),
                CornerRadius = 15
            };

            SimpleDataGrid gridForPanel = new SimpleDataGrid()
            {
                Location = new Point(10, 10),
                Size = new Size(680, 330),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            gridForPanel.LoadSampleData();
            
            myPanel.Controls.Add(gridForPanel);
            this.Controls.Add(myPanel);
            */
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(700, 400);
            Text = "Simple DataGrid Example";
            BackColor = Color.FromArgb(248, 249, 250);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular);

            ResumeLayout(false);
        }
    }
}

