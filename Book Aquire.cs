using Guna.UI2.WinForms;
using Library_Final;
using LibraryCGC.Components;
using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic.Logging;
using Newtonsoft.Json.Linq;
using PdfSharp.Pdf.Content.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace LibraryCGC
{
    public partial class Book_Aquire : Form
    {
        private int currentEmployeeID;

        private DataTable booksTable = new DataTable();
        private int? editingRowIndex = null;

        private bool scannerMode = true; // default: Scanner Mode

        // for autocomplete
        private ListBox suggestionListBox;  // dropdown list for suggestions
        private string connectionString = " Data Source=(LocalDB)\\MSSQLLocalDB;\r\nInitial Catalog=LibraryDB;\r\nIntegrated Security=True;\r\nEncrypt=True;\r\nTrust Server Certificate=True;\r\n";

        private System.Windows.Forms.Timer isbnTimer; // add at class level
        public Book_Aquire()
        {
            InitializeComponent();
            LoadBooksGrid(); // refresh grid to show new record
            DataGridTotalBooks.CellBeginEdit += DataGridTotalBooks_CellBeginEdit;

            // Timer setup (to avoid multiple rapid API calls while typing)
            isbnTimer = new System.Windows.Forms.Timer();
            isbnTimer.Interval = 1000; // 1 seconds delay
            isbnTimer.Tick += IsbnTimer_Tick;

            ISBN.TextChanged += ISBN_TextChanged; // trigger when ISBN box changes




            //Source Combobox fill
            Source.Items.AddRange(new string[] { "Purchased ", "Donate " });
            Source.SelectedIndex = 0;
            BookConditioncmb.Items.AddRange(new string[] { "Good", "Damaged", "Minor Damaged" });
            BookConditioncmb.SelectedIndex = 0;

            //filtering combobox fill
            BookCondition.Items.AddRange(new string[] { "Good", "Damaged", "Minor Damaged"});


            cmbSource.DataSource = null;   // 👈 break data-binding
            cmbSource.Items.Clear();
            cmbSource.Items.Add("All");
            cmbSource.Items.Add("Purchased");
            cmbSource.Items.Add("Donate");
            cmbSource.SelectedIndex = 0; // default to All


        }


        // 🔹 Manual Mode setup





        private async void BookTitle_TextChanged(object sender, EventArgs e)
        {
            string searchText = BookTitle.Texts.Trim();

            if (string.IsNullOrEmpty(searchText))
            {
                suggestionListBox.Visible = false;
                return;
            }

            // Query the database for matching titles
            List<string> suggestions = new List<string>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                await con.OpenAsync();

                string query = "SELECT TOP 10 BookTitle FROM BooksAcq WHERE BookTitle LIKE @search + '%'";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@search", searchText);

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            suggestions.Add(reader.GetString(0));
                        }
                    }
                }
            }

            // If there are suggestions, display them
            if (suggestions.Count > 0)
            {
                suggestionListBox.BeginUpdate();
                suggestionListBox.Items.Clear();
                foreach (var s in suggestions)
                    suggestionListBox.Items.Add(s);
                suggestionListBox.EndUpdate();

                // Position below the BookTitle textbox
                var tbLocation = BookTitle.PointToScreen(Point.Empty);
                var formLocation = this.PointToClient(tbLocation);

                suggestionListBox.Location = new Point(formLocation.X, formLocation.Y + BookTitle.Height);
                suggestionListBox.Width = BookTitle.Width;
                suggestionListBox.Height = Math.Min(150, suggestions.Count * 25);

                suggestionListBox.Visible = true;
                suggestionListBox.BringToFront();
            }
            else
            {
                suggestionListBox.Visible = false;
            }
        }


        private async void IsbnTimer_Tick(object sender, EventArgs e)
        {
            isbnTimer.Stop();

            string isbn = ISBN.Texts.Trim();
            if (isbn.Length < 5) return;

            try
            {
                // 🧠 Detect the item type dynamically
                string category = Category.Text.ToLower();
                string type = DetectBookType(category); // will return Book / Magazine / Newspaper / Catalog

                // 🌐 Fetch metadata based on type
                JObject? json = await FetchMetadataByTypeAsync(isbn, type);
                if (json == null)
                {
                    MessageBox.Show("No metadata found.");
                    return;
                }

                // 🧩 Parse metadata based on type
                if (type == "Book")
                {
                    var book = json["items"]?[0]?["volumeInfo"];
                    if (book != null)
                    {
                        Invoke((Action)(() =>
                        {
                            BookTitle.Texts = book["title"]?.ToString() ?? "";
                            Author.Texts = book["authors"]?.First?.ToString() ?? "";
                            Publisher.Texts = book["publisher"]?.ToString() ?? "";
                            Published.Texts = book["publishedDate"]?.ToString() ?? "";
                            Category.Texts = book["categories"]?.First?.ToString() ?? "";

                        }));

                        // ✅ Load thumbnail
                        string thumbnail = book["imageLinks"]?["thumbnail"]?.ToString();
                        if (!string.IsNullOrEmpty(thumbnail))
                        {
                            using (HttpClient imageClient = new HttpClient())
                            {
                                var stream = await imageClient.GetStreamAsync(thumbnail);
                                using (var bmp = new Bitmap(stream))
                                {
                                    picCover.BackgroundImage = new Bitmap(bmp);
                                    picCover.BackgroundImageLayout = ImageLayout.Zoom;
                                    picCover.Refresh();
                                }
                            }
                        }
                        else picCover.BackgroundImage = null;
                    }
                }
                else if (type == "Magazine" || type == "Newspaper")
                {
                    var article = json["articles"]?[0];
                    if (article != null)
                    {
                        Invoke((Action)(() =>
                        {
                            BookTitle.Texts = article["title"]?.ToString() ?? "";
                            Author.Texts = article["source"]?["name"]?.ToString() ?? "";
                            Publisher.Texts = article["author"]?.ToString() ?? "";
                            Published.Texts = article["publishedAt"]?.ToString() ?? "";
                            Category.Texts = type;

                        }));
                        picCover.BackgroundImage = null; // NewsAPI doesn’t provide thumbnails
                    }
                }
                else if (type.Contains("Catalog"))
                {
                    var item = json["items"]?[0];
                    if (item != null)
                    {
                        Invoke((Action)(() =>
                        {
                            BookTitle.Texts = item["title"]?.ToString() ?? "";
                            Author.Texts = item["dataProvider"]?.ToString() ?? "";
                            Publisher.Texts = item["provider"]?.ToString() ?? "";
                            Published.Texts = item["year"]?.ToString() ?? "";
                            Category.Texts = "Catalog / Pamphlet";

                        }));
                        picCover.BackgroundImage = null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private string DetectBookType(string category)
        {
            category = category.ToLower();

            if (category.Contains("magazine") || category.Contains("journal"))
                return "Magazine";
            if (category.Contains("newspaper") || category.Contains("news"))
                return "Newspaper";
            if (category.Contains("catalog") || category.Contains("pamphlet") || category.Contains("brochure"))
                return "Catalog / Pamphlet";

            return "Book";
        }




        private void ISBN_TextChanged(object sender, EventArgs e)
        {
            string isbn = ISBN.Texts.Trim();

            // 🧹 If ISBN field is cleared, automatically clear all related fields
            if (string.IsNullOrEmpty(isbn))
            {
                BookTitle.Texts = "";
                Author.Texts = "";
                Publisher.Texts = "";
                Published.Texts = "";
                Category.Texts = "";

                //Quantity.Texts = ""; ------------------

                picCover.BackgroundImage = null; // clear image
                isbnTimer.Stop(); // no need to trigger API
                return;
            }

            isbnTimer.Stop();
            isbnTimer.Start(); // restart timer every time they type

        }


        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }


        private void Book_Aquire_Load(object sender, EventArgs e)
        {
            SetupDataGridView();  // 🔹 call this first
            LoadBooksGrid();
            DataGridTotalBooks.CellPainting += DataGridTotalBooks_CellPainting;


            scannerMode = true;
            // Set default button text
            btnManualMode.Text = "📡 Scanner Mode Active";

            Quantity.Value = 1; // numeric value default value
            LoadBooksGrid();
            btnManualMode.Text = "📡 Scanner Mode Active";

            // 🔹 List all textboxes you want to lock/unlock
            var boxes = new LibraryCGC.Components.ArthanTextBox[]
            {
        BookTitle,
        Publisher,
        Author,
        Category,
        Published,

            };

            // 🔹 Attach KeyPress event handler to all textboxes
            foreach (var box in boxes)
            {
                box.KeyPress -= AnyTextBox_KeyPress; // remove duplicates
                box.KeyPress += AnyTextBox_KeyPress;
                box.BackColor = Color.LightGray;
            }

        }

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private void arthanButton1_Load(object sender, EventArgs e)
        {

        }

        private void arthanButton1_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(
                "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True;"))
            {
                con.Open();

                // 🧠 Step 1: Check if ISBN already exists
                string checkQuery = "SELECT Quantity FROM BooksAcq WHERE ISBN = @ISBN";
                using (SqlCommand checkCmd = new SqlCommand(checkQuery, con))
                {
                    checkCmd.Parameters.AddWithValue("@ISBN", ISBN.Texts.Trim());

                    object existingQtyObj = checkCmd.ExecuteScalar();

                    if (existingQtyObj != null)
                    {
                        // ✅ ISBN exists → just update quantity
                        int existingQty = Convert.ToInt32(existingQtyObj);
                        int newQty = existingQty + Convert.ToInt32(Quantity.Value);

                        string updateQuery = "UPDATE BooksAcq SET Quantity = @Quantity WHERE ISBN = @ISBN";
                        using (SqlCommand updateCmd = new SqlCommand(updateQuery, con))
                        {
                            updateCmd.Parameters.AddWithValue("@Quantity", newQty);
                            updateCmd.Parameters.AddWithValue("@ISBN", ISBN.Texts.Trim());
                            updateCmd.ExecuteNonQuery();
                        }

                        MessageBox.Show("Quantity updated successfully (same ISBN found).",
                                        "Updated", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        // 🆕 ISBN not found → insert new record
                        string insertQuery = @"INSERT INTO BooksAcq 
(BookTitle, Author, ISBN, Publisher, Source, Quantity, Published, Category, BookType,BookCondition) 
VALUES (@BookTitle, @Author, @ISBN, @Publisher, @Source, @Quantity, @Published, @Category, @BookType,@BookCondition)";

                        using (SqlCommand insertCmd = new SqlCommand(insertQuery, con))
                        {
                            insertCmd.Parameters.AddWithValue("@BookTitle", BookTitle.Texts);
                            insertCmd.Parameters.AddWithValue("@Author", Author.Texts);
                            insertCmd.Parameters.AddWithValue("@ISBN", ISBN.Texts);
                            insertCmd.Parameters.AddWithValue("@Publisher", Publisher.Texts);
                            insertCmd.Parameters.AddWithValue("@Source", Source.Text);
                            insertCmd.Parameters.AddWithValue("@Quantity", Quantity.Value);
                            insertCmd.Parameters.AddWithValue("@Published", Published.Texts);
                            insertCmd.Parameters.AddWithValue("@Category", Category.Texts);
                            insertCmd.Parameters.AddWithValue("@BookCondition", BookConditioncmb.Text);

                            // Detect BookType
                            string typeOfBook;
                            string category = Category.Text.ToLower();
                            if (category.Contains("magazine") || category.Contains("journal"))
                                typeOfBook = "Magazine";
                            else if (category.Contains("newspaper") || category.Contains("news"))
                                typeOfBook = "Newspaper";
                            else if (category.Contains("report") || category.Contains("document") || category.Contains("paper"))
                                typeOfBook = "Report / Document";
                            else if (category.Contains("catalog") || category.Contains("pamphlet") || category.Contains("brochure"))
                                typeOfBook = "Catalog / Pamphlet";
                            else
                                typeOfBook = "Book";

                            insertCmd.Parameters.AddWithValue("@BookType", typeOfBook);
                            insertCmd.ExecuteNonQuery();

                            MessageBox.Show($"{typeOfBook} added successfully!");
                        }
                    }
                }
            }

            // 🔄 Refresh grid
            LoadBooksGrid();
            GlobalEvents.RaiseBooksDataChanged();

            ActivityLog.RecordActivity(
           SessionData.CurrentUserName,
           "Add Book",
           "Book Acquisition",
           $"Added book: {BookTitle.Texts}"
       );










            // 🧹 Clear input fields
            BookTitle.Texts = "";
            Author.Texts = "";
            ISBN.Texts = "";
            Publisher.Texts = "";
            Published.Texts = "";
            Category.Texts = "";

            picCover.BackgroundImage = null;
            Quantity.Value = 1;
            BookConditioncmb.Text = "";

            // 🔁 Update dashboard if open
            var dashboardForm = Application.OpenForms["Form1"] as Form1;
            if (dashboardForm != null)
                dashboardForm.UpdateTotalBooksLabel();
        }




        // All METHOD HERE ---------------------------------------------------------
        private void DataGridTotalBooks_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (DataGridTotalBooks.Columns[e.ColumnIndex].Name == "BookID")
            {
                MessageBox.Show("BookID cannot be edited.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = true; // prevent editing
            }

            string colName = DataGridTotalBooks.Columns[e.ColumnIndex].Name;
            // restricted columns
            string[] restrictedCols = { "BookID", "Author", "ISBN", "Publisher", "Category" };

            if (restrictedCols.Contains(colName))
            {
                MessageBox.Show($"{colName} cannot be edited.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = true; // prevent editing
            }




        }





        private void kryptonButton6_Click(object sender, EventArgs e)
        {//NOT ALREADY APPLY -------------------------------------------------------------------
            SqlConnection con = new SqlConnection(" Data Source=(LocalDB)\\MSSQLLocalDB;\r\nInitial Catalog=LibraryDB;\r\nIntegrated Security=True;\r\nEncrypt=True;\r\nTrust Server Certificate=True;\r\n");
            con.Open();
            //use WHERE to specify what record to UPDATE                                                                                              
            SqlCommand cmd = new SqlCommand("UPDATE BooksAcq SET BookTitle = @BookTitle, Author = @Author, ISBN = @ISBN, Publisher = @Publisher, Source = @Source, Quantity = @Quantity, Published = @Published, Category = @Category,BookCondition = @BookCondition WHERE BookID = @BookID", con);

            // this will be used in WHERE clause
            cmd.Parameters.AddWithValue("@BookTitle", BookTitle.Text);
            cmd.Parameters.AddWithValue("@Author", Author.Text);
            cmd.Parameters.AddWithValue("@ISBN", ISBN.Text);
            cmd.Parameters.AddWithValue("@Publisher", Publisher.Text);
            cmd.Parameters.AddWithValue("@Source", Source.Text);
            cmd.Parameters.AddWithValue("@Quantity", Quantity.Text);
            cmd.Parameters.AddWithValue("@Published", Published.Text);
            cmd.Parameters.AddWithValue("@Category", Category.Text);
            cmd.Parameters.AddWithValue("@BookCondition", BookConditioncmb.Text);

            cmd.ExecuteNonQuery();
            MessageBox.Show("Book updated successfully!");
            LoadBooksGrid(); // refresh grid to show new record
            con.Close();

            // Clear the textboxes after update

            BookTitle.Text = "";
            Author.Text = "";
            ISBN.Text = "";
            Publisher.Text = "";

            Quantity.Text = "";
            Published.Text = "";
            Category.Text = "";

        }


        private void ArchivedButton_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(
                "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True;"))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(@"
            INSERT INTO BooksArchive 
            (BookTitle, Author, ISBN, Publisher, Source, Quantity, Published, Category, BookType,BookCondition)
            VALUES 
            (@BookTitle, @Author, @ISBN, @Publisher, @Source, @Quantity, @Published, @Category, @BookType,@BookCondition)", con);

                cmd.Parameters.AddWithValue("@BookTitle", BookTitle.Text);
                cmd.Parameters.AddWithValue("@Author", Author.Text);
                cmd.Parameters.AddWithValue("@ISBN", ISBN.Text);
                cmd.Parameters.AddWithValue("@Publisher", Publisher.Text);
                cmd.Parameters.AddWithValue("@Source", Source.Text);
                cmd.Parameters.AddWithValue("@Quantity", Quantity.Text);
                cmd.Parameters.AddWithValue("@Published", Published.Text);
                cmd.Parameters.AddWithValue("@Category", Category.Text);
                cmd.Parameters.AddWithValue("@BookCondition", BookConditioncmb.Text);

                // 🟢 Detect BookType (same logic as Book_Acquire insert)
                string typeOfBook = "Book";
                string category = Category.Text.ToLower();

                if (category.Contains("magazine") || category.Contains("journal"))
                    typeOfBook = "Magazine";
                else if (category.Contains("newspaper") || category.Contains("news"))
                    typeOfBook = "Newspaper";
                else if (category.Contains("report") || category.Contains("document") || category.Contains("paper"))
                    typeOfBook = "Report / Document";
                else if (category.Contains("catalog") || category.Contains("pamphlet") || category.Contains("brochure"))
                    typeOfBook = "Catalog / Pamphlet";
                else
                    typeOfBook = "Book";

                cmd.Parameters.AddWithValue("@BookType", typeOfBook);

                cmd.ExecuteNonQuery();
                MessageBox.Show("Book archived successfully!");

                ActivityLog.RecordActivity(
       SessionData.CurrentUserName,
       "Archive Book",
       "Book Acquisition",
       $"Archived book: {BookTitle.Texts} (ISBN: {ISBN.Texts})"
   );







                LoadBooksGrid();
            }

            // Clear input fields after archiving
            BookTitle.Text = "";
            Author.Text = "";
            ISBN.Text = "";
            Publisher.Text = "";
            Quantity.Text = "";
            Published.Text = "";
            Category.Text = "";
            BookConditioncmb.Text = "";
        }


        private void ArchiveBookFromRow(DataGridViewRow row)
        {
            using (SqlConnection con = new SqlConnection(
                "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True;"))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand(@"
            INSERT INTO BooksArchive 
            (BookID, BookTitle, Author, ISBN, Publisher, Source, Quantity, Published, Category, ArchivedDate, BookType,BookCondition)
            VALUES 
            (@BookID, @BookTitle, @Author, @ISBN, @Publisher, @Source, @Quantity, @Published, @Category, @ArchivedDate, @BookType,@BookCondition)", con))
                {
                    cmd.Parameters.AddWithValue("@BookID", row.Cells["BookID"].Value ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@BookTitle", row.Cells["BookTitle"].Value ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Author", row.Cells["Author"].Value ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ISBN", row.Cells["ISBN"].Value ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Publisher", row.Cells["Publisher"].Value ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Source", row.Cells["Source"].Value ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@BookCondition", row.Cells["BookCondition"].Value ?? DBNull.Value);

                    if (int.TryParse(row.Cells["Quantity"].Value?.ToString(), out int qty))
                        cmd.Parameters.AddWithValue("@Quantity", qty);
                    else
                        cmd.Parameters.AddWithValue("@Quantity", DBNull.Value);

                    cmd.Parameters.AddWithValue("@Published", row.Cells["Published"].Value ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Category", row.Cells["Category"].Value ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ArchivedDate", DateTime.Now);

                    if (row.Cells["BookType"] != null)
                        cmd.Parameters.AddWithValue("@BookType", row.Cells["BookType"].Value ?? DBNull.Value);
                    else
                        cmd.Parameters.AddWithValue("@BookType", DBNull.Value);



                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Book archived successfully!");
                    string archivedTitle = row.Cells["BookTitle"].Value?.ToString() ?? "";
                    string archivedISBN = row.Cells["ISBN"].Value?.ToString() ?? "";

                    ActivityLog.RecordActivity(
                        SessionData.CurrentUserName,
                        "Archive Book",
                        "Book Acquisition",
                        $"Archived book: {archivedTitle} (ISBN: {archivedISBN})"
                    );


                    GlobalEvents.RaiseBooksDataChanged();
                    GlobalEvents.RaiseBorrowedDataChanged();
                    GlobalEvents.RaiseArchivedDataChanged();
                }
            }
        }


        private void DeleteFromBooksAcq(string bookID) //delete after archive
        {
            //not already apply thisssssssssssssssssss
            SqlConnection con = new SqlConnection("   Data Source=(LocalDB)\\MSSQLLocalDB;\r\nInitial Catalog=LibraryDB;\r\nIntegrated Security=True;\r\nEncrypt=True;\r\nTrust Server Certificate=True;\r\n");
            con.Open();

            SqlCommand cmd = new SqlCommand("DELETE FROM BooksAcq WHERE BookID = @BookID", con);
            cmd.Parameters.AddWithValue("@BookID", bookID);
            cmd.ExecuteNonQuery();

            con.Close();
            GlobalEvents.RaiseBooksDataChanged();
            GlobalEvents.RaiseBorrowedDataChanged();
            GlobalEvents.RaiseArchivedDataChanged();

        }


        private void LoadBooksGrid()
        {
            using (SqlConnection con = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True;"))
            {
                string query = "SELECT * FROM BooksAcq";
                SqlDataAdapter da = new SqlDataAdapter(query, con);

                booksTable.Clear();
                da.Fill(booksTable);

                // ✅ Must be right after Fill()
                booksTable.CaseSensitive = false;

                // ✅ Clear any old filters
                booksTable.DefaultView.RowFilter = "";

                // ✅ IMPORTANT: Bind to DefaultView (not the table)
                DataGridTotalBooks.DataSource = booksTable.DefaultView;




                // ✅ Fill ComboBoxes with distinct values
                FillComboBox(cmbBookTitle, "BookTitle");
                FillComboBox(cmbAuthor, "Author");
                FillComboBox(cmbPublisher, "Publisher");
                FillComboBox(cmbSource, "Source");
                FillComboBox(cmbPublished, "Published");
                FillComboBox(cmbCategory, "Category");
                FillComboBox(cmbBookType, "BookType");

                // Existing styling code remains the same...
                DataGridTotalBooks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                DataGridTotalBooks.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                DataGridTotalBooks.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                DataGridTotalBooks.Dock = DockStyle.Fill;
                DataGridTotalBooks.RowHeadersVisible = false;
                DataGridTotalBooks.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                DataGridTotalBooks.MultiSelect = false;
                DataGridTotalBooks.ReadOnly = true;
                DataGridTotalBooks.AllowUserToResizeRows = false;
                DataGridTotalBooks.AllowUserToResizeColumns = false;
                DataGridTotalBooks.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                // ✅ Allow typing in all Guna2ComboBoxes
                cmbAuthor.DropDownStyle = ComboBoxStyle.DropDown;
                cmbPublisher.DropDownStyle = ComboBoxStyle.DropDown;
                cmbSource.DropDownStyle = ComboBoxStyle.DropDown;
                cmbPublished.DropDownStyle = ComboBoxStyle.DropDown;
                cmbCategory.DropDownStyle = ComboBoxStyle.DropDown;
                cmbBookType.DropDownStyle = ComboBoxStyle.DropDown;

                // ✅ Fill them with distinct values
                FillComboBox(cmbAuthor, "Author");
                FillComboBox(cmbPublisher, "Publisher");
                FillComboBox(cmbSource, "Source");
                FillComboBox(cmbPublished, "Published");
                FillComboBox(cmbCategory, "Category");
                FillComboBox(cmbBookType, "BookType");

                // Bind grid
                DataGridTotalBooks.DataSource = booksTable.DefaultView;

                // ✅ Re-add button columns if they were removed
                if (!DataGridTotalBooks.Columns.Contains("Update"))
                {
                    DataGridViewButtonColumn updateBtn = new DataGridViewButtonColumn();
                    updateBtn.HeaderText = "Action";
                    updateBtn.Name = "Update";
                    updateBtn.Text = "Update";
                    updateBtn.UseColumnTextForButtonValue = true;
                    DataGridTotalBooks.Columns.Add(updateBtn);
                }

                if (!DataGridTotalBooks.Columns.Contains("Archive"))
                {
                    DataGridViewButtonColumn archiveBtn = new DataGridViewButtonColumn();
                    archiveBtn.HeaderText = " ";
                    archiveBtn.Name = "Archive";
                    archiveBtn.Text = "Archive";
                    archiveBtn.UseColumnTextForButtonValue = true;
                    DataGridTotalBooks.Columns.Add(archiveBtn);
                }


            }
        }

        private void FillComboBox(System.Windows.Forms.ComboBox comboBox, string columnName)
        {
            var distinctValues = booksTable.AsEnumerable()
                .Select(row => row.Field<string>(columnName))
                .Where(val => !string.IsNullOrEmpty(val))
                .Distinct()
                .OrderBy(val => val)
                .ToList();

            comboBox.DataSource = distinctValues;
            comboBox.SelectedIndex = -1;

            // 👇 Enable typing + auto-suggest
            comboBox.DropDownStyle = ComboBoxStyle.DropDown;
            comboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            comboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
        }




        private void ApplyComboBoxFilters()
        {
            try
            {
                DataView dv = booksTable.DefaultView;
                booksTable.CaseSensitive = false;

                List<string> filters = new List<string>();

                // Helper for LIKE filters
                void AddLikeFilter(string column, string value)
                {
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        string safe = value.Replace("'", "''");
                        filters.Add($"[{column}] LIKE '%{safe}%'");
                    }
                }

                // 🔹 Normal filters
                AddLikeFilter("BookTitle", cmbBookTitle.Text);
                AddLikeFilter("Author", cmbAuthor.Text);
                AddLikeFilter("Publisher", cmbPublisher.Text);
                AddLikeFilter("Published", cmbPublished.Text);
                AddLikeFilter("Category", cmbCategory.Text);
                AddLikeFilter("BookType", cmbBookType.Text);
                AddLikeFilter("BookCondition", BookCondition.Text);

                // 🔹 Special handling for Source
                if (cmbSource.SelectedItem != null && cmbSource.Text != "All")
                {
                    string selected = cmbSource.Text.Replace("'", "''");
                    // exact match (no LIKE)
                    filters.Add($"Source = '{selected}'");
                }

                // Apply all filters
                dv.RowFilter = string.Join(" AND ", filters);
                DataGridTotalBooks.DataSource = dv;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error applying filters: " + ex.Message);
            }
        }










        private void ApplyFilters()
        {
            try
            {
                DataView dv = booksTable.DefaultView;
                booksTable.CaseSensitive = false;

                List<string> filters = new List<string>();

                void AddFilter(string column, string value)
                {
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        string safe = value.Replace("'", "''");
                        filters.Add($"[{column}] LIKE '%{safe}%'");
                    }
                }

                AddFilter("BookTitle", cmbBookTitle.Text);
                AddFilter("Author", cmbAuthor.Text);
                AddFilter("Publisher", cmbPublisher.Text);
                AddFilter("Source", cmbSource.Text);
                AddFilter("Published", cmbPublished.Text);
                AddFilter("Category", cmbCategory.Text);
                AddFilter("BookType", cmbBookType.Text);
                AddFilter("BookCondition", BookCondition.Text);

                dv.RowFilter = string.Join(" AND ", filters);
                DataGridTotalBooks.DataSource = dv;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error applying filters: " + ex.Message);
            }
        }




        private void DataGridTotalBooks_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            DataGridViewRow row = DataGridTotalBooks.Rows[e.RowIndex];

            if (DataGridTotalBooks.Columns[e.ColumnIndex].Name == "BookID")
            {
                MessageBox.Show("BookID cannot be edited.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ✅ Archive logic (same as before)
            if (DataGridTotalBooks.Columns[e.ColumnIndex].Name == "Archive")
            {
                string bookID = row.Cells["BookID"].Value.ToString();
                var confirm = MessageBox.Show("Are you sure you want to archive this book?",
                                              "Confirm Archive",
                                              MessageBoxButtons.YesNo,
                                              MessageBoxIcon.Question);
                if (confirm == DialogResult.No)
                    return;

                ArchiveBookFromRow(row);
                DeleteFromBooksAcq(bookID);
                LoadBooksGrid();
                return;
            }

            // ✅ Update logic with change tracking
            if (DataGridTotalBooks.Columns[e.ColumnIndex].Name == "Update")
            {
                if (editingRowIndex == null)
                {
                    editingRowIndex = e.RowIndex;

                    // ✅ Allow the grid to be editable overall
                    DataGridTotalBooks.ReadOnly = false;

                    // 🔹 Allow editing only specific columns
                    foreach (DataGridViewColumn col in DataGridTotalBooks.Columns)
                    {
                        bool isEditable = col.Name == "Source" ||
                                          col.Name == "Quantity" ||
                                          col.Name == "BookType" ||
                                          col.Name == "BookCondition";
                        DataGridTotalBooks.Columns[col.Index].ReadOnly = !isEditable;
                    }

                    // 🔹 Disable editing for all other rows
                    foreach (DataGridViewRow r in DataGridTotalBooks.Rows)
                    {
                        if (r.Index != e.RowIndex)
                            r.ReadOnly = true;
                    }

                    // Optional visual feedback
                    foreach (DataGridViewCell cell in DataGridTotalBooks.Rows[e.RowIndex].Cells)
                    {
                        if (!cell.ReadOnly)
                            cell.Style.BackColor = Color.LightYellow; // highlight editable cells
                    }

                    // Change button text
                    DataGridTotalBooks.Rows[e.RowIndex].Cells["Update"].Value = "Save";

                    MessageBox.Show("You can now edit Source, Quantity, Book Type, and Book Condition only.",
                                    "Edit Mode", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    return;
                }

                if (editingRowIndex == e.RowIndex)
                {
                    var confirm = MessageBox.Show("Do you want to save the changes to this book?",
                                                  "Confirm Save",
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Question);
                    if (confirm == DialogResult.No)
                        return;

                    try
                    {
                        DataGridViewRow editRow = DataGridTotalBooks.Rows[e.RowIndex];

                        int bookID = Convert.ToInt32(editRow.Cells["BookID"].Value);
                        string bookTitle = editRow.Cells["BookTitle"].Value?.ToString() ?? "";
                        string author = editRow.Cells["Author"].Value?.ToString() ?? "";
                        string isbn = editRow.Cells["ISBN"].Value?.ToString() ?? "";
                        string publisher = editRow.Cells["Publisher"].Value?.ToString() ?? "";
                        string source = editRow.Cells["Source"].Value?.ToString() ?? "";
                        int quantity = Convert.ToInt32(editRow.Cells["Quantity"].Value);
                        string published = editRow.Cells["Published"].Value?.ToString() ?? "";
                        string category = editRow.Cells["Category"].Value?.ToString() ?? "";
                        string bookCondition = editRow.Cells["BookCondition"].Value?.ToString() ?? "";

                        string bookType = "Book";
                        string lowerCategory = category.ToLower();
                        if (lowerCategory.Contains("magazine") || lowerCategory.Contains("journal"))
                            bookType = "Magazine";
                        else if (lowerCategory.Contains("newspaper") || lowerCategory.Contains("news"))
                            bookType = "Newspaper";
                        else if (lowerCategory.Contains("catalog") || lowerCategory.Contains("pamphlet"))
                            bookType = "Catalog / Pamphlet";
                        else if (lowerCategory.Contains("report") || lowerCategory.Contains("document"))
                            bookType = "Report / Document";

                        // 🧩 Step 1: Get OLD values before updating
                        string oldSource = "";
                        int oldQuantity = 0;
                        string oldBookType = "";
                        string BookCondition = "";

                        using (SqlConnection conn = new SqlConnection(
                            @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=LibraryDB;
                      Integrated Security=True;Encrypt=True;TrustServerCertificate=True"))
                        {
                            conn.Open();
                            string selectQuery = "SELECT Source, Quantity, BookType,BookCondition FROM BooksAcq WHERE BookID = @BookID";
                            using (SqlCommand selectCmd = new SqlCommand(selectQuery, conn))
                            {
                                selectCmd.Parameters.AddWithValue("@BookID", bookID);
                                SqlDataReader reader = selectCmd.ExecuteReader();
                                if (reader.Read())
                                {
                                    oldSource = reader["Source"].ToString();
                                    oldQuantity = Convert.ToInt32(reader["Quantity"]);
                                    oldBookType = reader["BookType"].ToString();
                                    BookCondition = reader["BookCondition"].ToString();
                                }
                            }
                        }

                        foreach (DataGridViewColumn col in DataGridTotalBooks.Columns)
                        {
                            bool isEditable = col.Name == "Source" || col.Name == "Quantity" ||
                                              col.Name == "BookType" || col.Name == "BookCondition";
                            var cell = DataGridTotalBooks.Rows[e.RowIndex].Cells[col.Index];
                            cell.ReadOnly = !isEditable;
                            cell.Style.BackColor = isEditable ? Color.LightYellow : Color.White;
                        }

                        // 🧩 Step 2: Perform the UPDATE
                        using (SqlConnection con = new SqlConnection(
                            @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=LibraryDB;
                      Integrated Security=True;Encrypt=True;TrustServerCertificate=True"))
                        {
                            con.Open();
                            SqlCommand cmd = new SqlCommand(@"
UPDATE BooksAcq
SET 
    Source = @Source,
    Quantity = @Quantity,
    BookType = @BookType,
    BookCondition = @BookCondition
WHERE BookID = @BookID", con);


                            cmd.Parameters.AddWithValue("@Source", source);
                            cmd.Parameters.AddWithValue("@Quantity", quantity);
                            cmd.Parameters.AddWithValue("@BookType", bookType);
                            cmd.Parameters.AddWithValue("@BookCondition", bookCondition);
                            cmd.Parameters.AddWithValue("@BookID", bookID);

                            cmd.ExecuteNonQuery();
                        }

                        // 🧩 Step 3: Detect and record CHANGES
                        string changes = "";
                        if (oldSource != source)
                            changes += $"Source: {oldSource} → {source}; ";
                        if (oldQuantity != quantity)
                            changes += $"Quantity: {oldQuantity} → {quantity}; ";
                        if (oldBookType != bookType)
                            changes += $"Type: {oldBookType} → {bookType}; ";
                        if (BookCondition != bookCondition)
                            changes += $"Condition: {BookCondition} → {bookCondition}; ";


                        if (string.IsNullOrEmpty(changes))
                            changes = "No significant changes.";

                        ActivityLog.RecordActivity(
                            SessionData.CurrentUserName,
                            "Update Book",
                            "Book Acquisition",
                            $"Updated book: {bookTitle}. Changes: {changes}"
                        );

                        MessageBox.Show("Book updated successfully!",
                                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        editingRowIndex = null;
                        DataGridTotalBooks.ReadOnly = true;
                        DataGridTotalBooks.Rows[e.RowIndex].Cells["Update"].Value = "Edit";
                        LoadBooksGrid();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error updating book: {ex.Message}",
                                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }





        // Make sure this is the method assigned to the ISBN TextBox KeyDown event
        private async void ISBN_KeyDown(object sender, KeyEventArgs e)
        {
            // Only act when user presses Enter
            if (e.KeyCode != Keys.Enter) return;

            string isbn = ISBN.Text.Trim();
            if (string.IsNullOrEmpty(isbn))
            {
                MessageBox.Show("Please enter an ISBN.");
                return;
            }

            string apiUrl = $"https://www.googleapis.com/books/v1/volumes?q=isbn:{Uri.EscapeDataString(isbn)}";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var response = await client.GetStringAsync(apiUrl);
                    JObject json = JObject.Parse(response);

                    var book = json["items"]?[0]?["volumeInfo"];
                    if (book != null)
                    {
                        BookTitle.Texts = book["title"]?.ToString() ?? "";
                        Author.Texts = book["authors"]?.First?.ToString() ?? "";
                        Publisher.Texts = book["publisher"]?.ToString() ?? "";
                        Published.Texts = book["publishedDate"]?.ToString() ?? "";
                        Category.Texts = book["categories"]?.First?.ToString() ?? "";


                        string thumbnail = book["imageLinks"]?["thumbnail"]?.ToString();
                        if (!string.IsNullOrEmpty(thumbnail))
                        {
                            // wrap in try/catch if image might fail to load
                            try { picCover.Load(thumbnail); } catch { /* ignore load errors */ }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Book not found.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }

            // optionally prevent 'ding' sound on Enter
            e.SuppressKeyPress = true;
        }


        private async Task<JObject?> FetchMetadataByTypeAsync(string identifier, string type)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    if (type == "Book")
                    {
                        string url = $"https://www.googleapis.com/books/v1/volumes?q=isbn:{Uri.EscapeDataString(identifier)}";
                        string response = await client.GetStringAsync(url);
                        return JObject.Parse(response);
                    }
                    else if (type == "Magazine" || type == "Newspaper")
                    {
                        string apiKey = "YOUR_NEWSAPI_KEY"; // 🔑 Replace with your NewsAPI key
                        string url = $"https://newsapi.org/v2/everything?q={Uri.EscapeDataString(identifier)}&apiKey={apiKey}";
                        string response = await client.GetStringAsync(url);
                        return JObject.Parse(response);
                    }
                    else if (type.Contains("Catalog"))
                    {
                        string apiKey = "YOUR_EUROPEANA_KEY"; // 🔑 Replace with your Europeana key
                        string url = $"https://api.europeana.eu/record/v2/search.json?wskey={apiKey}&query={Uri.EscapeDataString(identifier)}";
                        string response = await client.GetStringAsync(url);
                        return JObject.Parse(response);
                    }
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }



        private void arthanButton1_HomeClick(object sender, EventArgs e)
        {
            foreach (Form openForm in Application.OpenForms)
            {
                if (openForm is Form1)
                {
                    openForm.Show();
                    this.Hide();
                    return;
                }
            }

            // If not open, create it
            Form1 form1 = new Form1();
            form1.Show();
            this.Hide();

        }


        private void arthanPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void picCover_Click(object sender, EventArgs e)
        {
            picCover.SizeMode = PictureBoxSizeMode.Zoom; // or CenterImage
        }

        private void arthanPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Author__TextChanged(object sender, EventArgs e)
        {

        }

        private void Source_SelectedIndexChanged(object sender, EventArgs e)
        {


        }

        private void kryptonButton1_Load(object sender, EventArgs e)
        {

        }

        private void btnManualMode_Click(object sender, EventArgs e)
        {
            scannerMode = !scannerMode; // toggle the mode

            // Update button text
            btnManualMode.Text = scannerMode
                ? "📡 Scanner Mode Active"
                : "⌨️ Manual Mode Active";

            // Optional: visual feedback for textbox background
            var boxes = new LibraryCGC.Components.ArthanTextBox[]
            {
        BookTitle,
        Publisher,
        Author,
        Category,
        Published,

            };

            foreach (var box in boxes)
            {
                box.BackColor = scannerMode ? Color.LightGray : Color.White;
            }
        }

        private void BookTitle_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void AnyTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (scannerMode)
            {
                e.Handled = true; // 🚫 Block manual keyboard typing
            }
        }


        private void SetupDataGridView()
        {
            DataGridTotalBooks.Columns.Clear();
            DataGridTotalBooks.AutoGenerateColumns = false;
            DataGridTotalBooks.ReadOnly = true;
            DataGridTotalBooks.RowHeadersVisible = false;
            DataGridTotalBooks.BorderStyle = BorderStyle.None;
            DataGridTotalBooks.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            DataGridTotalBooks.EnableHeadersVisualStyles = false;

            // --- Book ID (hidden) ---
            var colBookID = new DataGridViewTextBoxColumn();
            colBookID.HeaderText = "Book ID";
            colBookID.DataPropertyName = "BookID";
            colBookID.Name = "BookID";      // 🔹 must have this
            colBookID.Visible = false;
            DataGridTotalBooks.Columns.Add(colBookID);

            // --- Book Title ---
            var colBookTitle = new DataGridViewTextBoxColumn();
            colBookTitle.HeaderText = "Book Title";
            colBookTitle.DataPropertyName = "BookTitle";
            colBookTitle.Name = "BookTitle"; // 🔹 name must match the one you use in code
            colBookTitle.Width = 200;
            DataGridTotalBooks.Columns.Add(colBookTitle);

            // --- Author ---
            var colAuthor = new DataGridViewTextBoxColumn();
            colAuthor.HeaderText = "Author";
            colAuthor.DataPropertyName = "Author";
            colAuthor.Name = "Author";
            colAuthor.Width = 150;
            DataGridTotalBooks.Columns.Add(colAuthor);

            // --- ISBN ---
            var colISBN = new DataGridViewTextBoxColumn();
            colISBN.HeaderText = "ISBN";
            colISBN.DataPropertyName = "ISBN";
            colISBN.Name = "ISBN";
            colISBN.Width = 120;
            DataGridTotalBooks.Columns.Add(colISBN);

            // --- Publisher ---
            var colPublisher = new DataGridViewTextBoxColumn();
            colPublisher.HeaderText = "Publisher";
            colPublisher.DataPropertyName = "Publisher";
            colPublisher.Name = "Publisher";
            colPublisher.Width = 150;
            DataGridTotalBooks.Columns.Add(colPublisher);

            // --- Source ---
            var colSource = new DataGridViewTextBoxColumn();
            colSource.HeaderText = "Source";
            colSource.DataPropertyName = "Source";
            colSource.Name = "Source";
            colSource.Width = 100;
            DataGridTotalBooks.Columns.Add(colSource);

            // --- Quantity ---
            var colQuantity = new DataGridViewTextBoxColumn();
            colQuantity.HeaderText = "Quantity";
            colQuantity.DataPropertyName = "Quantity";
            colQuantity.Name = "Quantity";
            colQuantity.Width = 80;
            DataGridTotalBooks.Columns.Add(colQuantity);

            // --- Published ---
            var colPublished = new DataGridViewTextBoxColumn();
            colPublished.HeaderText = "Published";
            colPublished.DataPropertyName = "Published";
            colPublished.Name = "Published";
            colPublished.Width = 120;
            DataGridTotalBooks.Columns.Add(colPublished);

            // --- Category ---
            var colCategory = new DataGridViewTextBoxColumn();
            colCategory.HeaderText = "Category";
            colCategory.DataPropertyName = "Category";
            colCategory.Name = "Category";
            colCategory.Width = 150;
            DataGridTotalBooks.Columns.Add(colCategory);

            // --- Book Type ---
            var colBookType = new DataGridViewTextBoxColumn();
            colBookType.HeaderText = "Book Type";
            colBookType.DataPropertyName = "BookType";  // must match column name in DB
            colBookType.Name = "BookType";
            colBookType.Width = 130;
            DataGridTotalBooks.Columns.Add(colBookType);

            // --- Book Condition ---
            var BookCondition = new DataGridViewTextBoxColumn();
            BookCondition.HeaderText = "Book Condition";
            BookCondition.DataPropertyName = "BookCondition";  // must match column name in DB
            BookCondition.Name = "BookCondition";
            BookCondition.Width = 130;
            DataGridTotalBooks.Columns.Add(BookCondition);

            // --- Styling ---
            DataGridTotalBooks.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DataGridTotalBooks.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DataGridTotalBooks.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(253, 242, 194);
            DataGridTotalBooks.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            DataGridTotalBooks.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            DataGridTotalBooks.DefaultCellStyle.BackColor = Color.White;
            DataGridTotalBooks.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(255, 250, 230);


        }



        private void DataGridTotalBooks_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex == -1 && e.ColumnIndex >= 0)
            {
                using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                    e.CellBounds,
                    Color.FromArgb(255, 253, 242, 194), // light yellow
                    Color.FromArgb(255, 253, 231, 144), // darker yellow
                    System.Drawing.Drawing2D.LinearGradientMode.Horizontal))
                {
                    e.Graphics.FillRectangle(brush, e.CellBounds);
                    e.Graphics.DrawRectangle(Pens.LightGray, e.CellBounds);
                    e.Graphics.DrawString(
                        e.FormattedValue?.ToString(),
                        e.CellStyle.Font,
                        Brushes.Black,
                        e.CellBounds.X + 5,
                        e.CellBounds.Y + 5);
                    e.Handled = true;
                }
            }
        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void SearchTxtBox__TextChanged(object sender, EventArgs e)
        {

        }
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {

        }



        private void cmbAuthor_TextChanged(object sender, EventArgs e)
        {
            ApplyComboBoxFilters();
        }

        private void cmbPublisher_TextChanged(object sender, EventArgs e)
        {
            ApplyComboBoxFilters();
        }

        private void a(object sender, EventArgs e)
        {
            ApplyComboBoxFilters();
        }

        private void cmbPublished_TextChanged(object sender, EventArgs e)
        {
            ApplyComboBoxFilters();
        }

        private void cmbCategory_TextChanged(object sender, EventArgs e)
        {
            ApplyComboBoxFilters();
        }

        private void cmbBookType_TextChanged(object sender, EventArgs e)
        {
            ApplyComboBoxFilters();
        }

        private void cmbAuthor_DropDown(object sender, EventArgs e)
        {

        }





        private void btnFilter_Click(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void btnClearFilters_Click(object sender, EventArgs e)
        {
            // Reset combo boxes
            cmbBookTitle.SelectedIndex = -1;
            cmbAuthor.SelectedIndex = -1;
            cmbPublisher.SelectedIndex = -1;
            cmbSource.SelectedIndex = -1;
            cmbPublished.SelectedIndex = -1;
            cmbCategory.SelectedIndex = -1;
            cmbBookType.SelectedIndex = -1;

            // Safely handle both DataTable or DataView
            if (DataGridTotalBooks.DataSource is DataView dv)
            {
                dv.RowFilter = "";
                DataGridTotalBooks.DataSource = dv;
            }
            else if (DataGridTotalBooks.DataSource is DataTable dt)
            {
                dt.DefaultView.RowFilter = "";
                DataGridTotalBooks.DataSource = dt;
            }
        }

        private void cmbPublisher_DropDown(object sender, EventArgs e)
        {

        }

        private void cmbSource_TextChanged(object sender, EventArgs e)
        {
            ApplyComboBoxFilters();
        }

        private void cmbSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyComboBoxFilters();
        }

        private void DataGridTotalBooks_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            string colName = DataGridTotalBooks.Columns[e.ColumnIndex].Name;
            string[] restrictedCols = { "BookID", "Author", "ISBN", "Publisher", "Category", "BookTitle", "Published" };

            if (restrictedCols.Contains(colName))
            {
                MessageBox.Show($"{colName} cannot be edited.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }
    }


} //END OF MAIN METHOD


