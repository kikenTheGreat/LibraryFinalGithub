using Library_Final;
using LibraryCGC.Components;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json.Linq;
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
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace LibraryCGC
{
    public partial class Book_Aquire : Form
    {

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


            // 🔍 Initialize suggestion list for search
            suggestionListBox = new ListBox();
            suggestionListBox.Visible = false;
            suggestionListBox.Font = SearchTxtBox.Font;
            suggestionListBox.BackColor = Color.White;
            suggestionListBox.ForeColor = Color.Black;
            suggestionListBox.BorderStyle = BorderStyle.FixedSingle;

            // Add to form
            this.Controls.Add(suggestionListBox);

            // Click on suggestion → fill the textbox
            suggestionListBox.Click += (s, e) =>
            {
                if (suggestionListBox.SelectedItem != null)
                {
                    SearchTxtBox.Texts = suggestionListBox.SelectedItem.ToString();
                    suggestionListBox.Visible = false;
                }
            };

            // 👇 Connect event to ArthanTextBox’s custom event (_TextChanged)
            SearchTxtBox._TextChanged += SearchTxtBox_TextChanged;
            SearchTxtBox.Leave += (s, e) => suggestionListBox.Visible = false;

            //Source Combobox fill
            Source.Items.AddRange(new string[] { "Purchased ", "Donate " });
            Source.SelectedIndex = 0;


        }


        // 🔹 Manual Mode setup



        private async void SearchTxtBox_TextChanged(object sender, EventArgs e)
        {
            string searchText = SearchTxtBox.Texts.Trim();

            if (string.IsNullOrEmpty(searchText))
            {
                suggestionListBox.Visible = false;
                return;
            }

            List<string> suggestions = new List<string>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                await con.OpenAsync();
                // 🔍 match anywhere in title, not just starting letters
                string query = "SELECT TOP 10 BookTitle FROM BooksAcq WHERE BookTitle LIKE '%' + @search + '%'";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@search", searchText);
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                            suggestions.Add(reader.GetString(0));
                    }
                }
            }

            if (suggestions.Count > 0)
            {
                suggestionListBox.BeginUpdate();
                suggestionListBox.Items.Clear();
                foreach (var s in suggestions)
                    suggestionListBox.Items.Add(s);
                suggestionListBox.EndUpdate();

                // Position below the search box
                var tbLocation = SearchTxtBox.PointToScreen(Point.Empty);
                var formLocation = this.PointToClient(tbLocation);

                suggestionListBox.Location = new Point(formLocation.X, formLocation.Y + SearchTxtBox.Height);
                suggestionListBox.Width = SearchTxtBox.Width;
                suggestionListBox.Height = Math.Min(150, suggestions.Count * 25);

                suggestionListBox.Visible = true;
                suggestionListBox.BringToFront();
            }
            else
            {
                suggestionListBox.Visible = false;
            }
        }


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

            string isbn = ISBN.Texts.Trim(); // use Texts for ArthanTextBox
            if (isbn.Length < 10) return;

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
                        // ✅ Assign text safely and force UI refresh to commit value
                        Invoke((Action)(() =>
                        {
                            BookTitle.Texts = book["title"]?.ToString() ?? "";
                            BookTitle.Text = BookTitle.Texts;
                            BookTitle.Refresh();

                            Author.Texts = book["authors"]?.First?.ToString() ?? "";
                            Author.Text = Author.Texts;
                            Author.Refresh();

                            Publisher.Texts = book["publisher"]?.ToString() ?? "";
                            Publisher.Text = Publisher.Texts;
                            Publisher.Refresh();

                            Published.Texts = book["publishedDate"]?.ToString() ?? "";
                            Published.Text = Published.Texts;
                            Published.Refresh();

                            Category.Texts = book["categories"]?.First?.ToString() ?? "";
                            Category.Text = Category.Texts;
                            Category.Refresh();

                            txtDesc.Texts = book["description"]?.ToString() ?? "";
                            txtDesc.Text = txtDesc.Texts;
                            txtDesc.Refresh();
                        }));


                        // ✅ Load book thumbnail safely
                        string thumbnail = book["imageLinks"]?["thumbnail"]?.ToString();
                        if (!string.IsNullOrEmpty(thumbnail))
                        {
                            try
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
                            catch
                            {
                                picCover.BackgroundImage = null;
                            }
                        }
                        else
                        {
                            picCover.BackgroundImage = null;
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
                txtDesc.Texts = "";
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
        txtDesc
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
            using (SqlConnection con = new SqlConnection("  Data Source=(LocalDB)\\MSSQLLocalDB;\r\nInitial Catalog=LibraryDB;\r\nIntegrated Security=True;\r\nEncrypt=True;\r\nTrust Server Certificate=True;\r\n"))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(@"INSERT INTO BooksAcq 
(BookTitle, Author, ISBN, Publisher, Source, Quantity, Published, Category, BookType) 
VALUES (@BookTitle, @Author, @ISBN, @Publisher, @Source, @Quantity, @Published, @Category, @BookType)", con);

                // Your parameters...
                cmd.Parameters.AddWithValue("@BookTitle", BookTitle.Texts);
                cmd.Parameters.AddWithValue("@Author", Author.Texts);
                cmd.Parameters.AddWithValue("@ISBN", ISBN.Texts);
                cmd.Parameters.AddWithValue("@Publisher", Publisher.Texts);
                cmd.Parameters.AddWithValue("@Source", Source.Text);
                cmd.Parameters.AddWithValue("@Quantity", Quantity.Text);
                cmd.Parameters.AddWithValue("@Published", Published.Texts);
                cmd.Parameters.AddWithValue("@Category", Category.Texts);

                // Detect and add BookType
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
                MessageBox.Show($"{typeOfBook} added successfully!");

                GlobalEvents.RaiseBooksDataChanged();
            }

            LoadBooksGrid();

            // clear all fields
            BookTitle.Texts = "";
            Author.Texts = "";
            ISBN.Texts = "";
            Publisher.Texts = "";


            //Quantity.Texts = "";---------------------------------------------
            Published.Texts = "";
            Category.Texts = "";
            txtDesc.Texts = "";
            picCover.BackgroundImage = null;


            var dashboardForm = Application.OpenForms["Form1"] as Form1;
            if (dashboardForm != null)
            {
                dashboardForm.UpdateTotalBooksLabel();
            }



        }



        // All METHOD HERE ---------------------------------------------------------
        private void DataGridTotalBooks_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (DataGridTotalBooks.Columns[e.ColumnIndex].Name == "BookID")
            {
                MessageBox.Show("BookID cannot be edited.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = true; // prevent editing
            }
        }





        private void kryptonButton6_Click(object sender, EventArgs e)
        {//NOT ALREADY APPLY -------------------------------------------------------------------
            SqlConnection con = new SqlConnection(" Data Source=(LocalDB)\\MSSQLLocalDB;\r\nInitial Catalog=LibraryDB;\r\nIntegrated Security=True;\r\nEncrypt=True;\r\nTrust Server Certificate=True;\r\n");
            con.Open();
            //use WHERE to specify what record to UPDATE                                                                                              
            SqlCommand cmd = new SqlCommand("UPDATE BooksAcq SET BookTitle = @BookTitle, Author = @Author, ISBN = @ISBN, Publisher = @Publisher, Source = @Source, Quantity = @Quantity, Published = @Published, Category = @Category WHERE BookID = @BookID", con);

            // this will be used in WHERE clause
            cmd.Parameters.AddWithValue("@BookTitle", BookTitle.Text);
            cmd.Parameters.AddWithValue("@Author", Author.Text);
            cmd.Parameters.AddWithValue("@ISBN", ISBN.Text);
            cmd.Parameters.AddWithValue("@Publisher", Publisher.Text);
            cmd.Parameters.AddWithValue("@Source", Source.Text);
            cmd.Parameters.AddWithValue("@Quantity", Quantity.Text);
            cmd.Parameters.AddWithValue("@Published", Published.Text);
            cmd.Parameters.AddWithValue("@Category", Category.Text);

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
            (BookTitle, Author, ISBN, Publisher, Source, Quantity, Published, Category, BookType)
            VALUES 
            (@BookTitle, @Author, @ISBN, @Publisher, @Source, @Quantity, @Published, @Category, @BookType)", con);

                cmd.Parameters.AddWithValue("@BookTitle", BookTitle.Text);
                cmd.Parameters.AddWithValue("@Author", Author.Text);
                cmd.Parameters.AddWithValue("@ISBN", ISBN.Text);
                cmd.Parameters.AddWithValue("@Publisher", Publisher.Text);
                cmd.Parameters.AddWithValue("@Source", Source.Text);
                cmd.Parameters.AddWithValue("@Quantity", Quantity.Text);
                cmd.Parameters.AddWithValue("@Published", Published.Text);
                cmd.Parameters.AddWithValue("@Category", Category.Text);

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
        }


        private void ArchiveBookFromRow(DataGridViewRow row)
        {
            using (SqlConnection con = new SqlConnection(
                "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True;"))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand(@"
            INSERT INTO BooksArchive 
            (BookID, BookTitle, Author, ISBN, Publisher, Source, Quantity, Published, Category, ArchivedDate, BookType)
            VALUES 
            (@BookID, @BookTitle, @Author, @ISBN, @Publisher, @Source, @Quantity, @Published, @Category, @ArchivedDate, @BookType)", con))
                {
                    cmd.Parameters.AddWithValue("@BookID", row.Cells["BookID"].Value ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@BookTitle", row.Cells["BookTitle"].Value ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Author", row.Cells["Author"].Value ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ISBN", row.Cells["ISBN"].Value ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Publisher", row.Cells["Publisher"].Value ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Source", row.Cells["Source"].Value ?? DBNull.Value);

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
            //output the datagriddddddddddddddd
            using (SqlConnection con = new SqlConnection("  Data Source=(LocalDB)\\MSSQLLocalDB;\r\nInitial Catalog=LibraryDB;\r\nIntegrated Security=True;\r\nEncrypt=True;\r\nTrust Server Certificate=True;\r\n"))
            {
                string query = "SELECT * FROM BooksAcq";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                DataGridTotalBooks.DataSource = dt;

                // Scroll to top
                if (DataGridTotalBooks.Rows.Count > 0)
                {
                    DataGridTotalBooks.FirstDisplayedScrollingRowIndex = 0;
                    DataGridTotalBooks.ClearSelection(); // Optional
                }


                // Button For ARCHIVEEEEEEE
                if (!DataGridTotalBooks.Columns.Contains("Action"))
                {
                    DataGridViewButtonColumn btn = new DataGridViewButtonColumn();
                    btn.HeaderText = "Action";         // Column header name
                    btn.Name = "Action";               // Internal name
                    btn.Text = "Archive";              // Button text
                    btn.UseColumnTextForButtonValue = true; // So it shows text in every row
                    DataGridTotalBooks.Columns.Add(btn);    // Add to DataGridView
                }


                // Button For UPDATEEEEE
                if (!DataGridTotalBooks.Columns.Contains("Update"))
                {
                    DataGridViewButtonColumn updateButton = new DataGridViewButtonColumn();
                    updateButton.Name = "Update";
                    updateButton.HeaderText = "Actions";
                    updateButton.Text = "Update";
                    updateButton.UseColumnTextForButtonValue = true;
                    DataGridTotalBooks.Columns.Add(updateButton);
                }

                // ✅ Auto layout and scaling
                DataGridTotalBooks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                DataGridTotalBooks.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                DataGridTotalBooks.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

                // ✅ Responsive resizing
                DataGridTotalBooks.Dock = DockStyle.Fill;
                // (If you have other controls in the same panel, use Anchors instead:)
                // IssueBooksDataGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

                // 🎨 Bonus — Clean, user-friendly visual settings
                DataGridTotalBooks.RowHeadersVisible = false;
                DataGridTotalBooks.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                DataGridTotalBooks.MultiSelect = false;
                DataGridTotalBooks.ReadOnly = true;
                DataGridTotalBooks.AllowUserToResizeRows = false;
                DataGridTotalBooks.AllowUserToResizeColumns = false;

                // Optional: center column headers
                DataGridTotalBooks.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;




            }
        }

        private void DataGridTotalBooks_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            // TO ENABLE EDITING IN DATAGRID FOR ROWWW
            DataGridTotalBooks.ReadOnly = false;
            DataGridTotalBooks.AllowUserToAddRows = false; // optional
            DataGridTotalBooks.Columns["Update"].ReadOnly = true; // keep button read-only

            // Check if the column is BookID
            if (DataGridTotalBooks.Columns[e.ColumnIndex].Name == "BookID")
            {
                MessageBox.Show("BookID cannot be edited.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }



            // TO ENABLE EDITING IN DATAGRID FOR ROWWW




            if (e.RowIndex >= 0 && DataGridTotalBooks.Columns[e.ColumnIndex].Name == "Action")
            {
                DataGridViewRow selectedRow = DataGridTotalBooks.Rows[e.RowIndex];

                // Archive it
                ArchiveBookFromRow(selectedRow);

                // Optionally delete it from BooksAcq
                string bookID = selectedRow.Cells["BookID"].Value.ToString();
                DeleteFromBooksAcq(bookID);

                // Refresh grid
                LoadBooksGrid();
            }





            // Allow Editing in DATAGRID ROW (UPDATE FUNCTIONNNNNN)

            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            if (DataGridTotalBooks.Columns[e.ColumnIndex].Name == "Update")
            {
                // Get the current row
                DataGridViewRow row = DataGridTotalBooks.Rows[e.RowIndex];

                // Example: retrieve the data

                string bookTitle = row.Cells["BookTitle"].Value.ToString();
                string author = row.Cells["Author"].Value.ToString();
                string isbn = row.Cells["ISBN"].Value.ToString();
                string publisher = row.Cells["Publisher"].Value.ToString();
                string source = row.Cells["Source"].Value.ToString();

                string quantity = row.Cells["Quantity"].Value.ToString();


                string published = row.Cells["Published"].Value.ToString();
                string category = row.Cells["Category"].Value.ToString();

                // Update to database
                using (SqlConnection con = new SqlConnection("   Data Source=(LocalDB)\\MSSQLLocalDB;\r\nInitial Catalog=LibraryDB;\r\nIntegrated Security=True;\r\nEncrypt=True;\r\nTrust Server Certificate=True;\r\n"))
                {
                    int bookID = Convert.ToInt32(DataGridTotalBooks.CurrentRow.Cells["BookID"].Value);

                    con.Open();

                    SqlCommand cmd = new SqlCommand("UPDATE BooksAcq SET BookTitle = @BookTitle, Author = @Author, ISBN = @ISBN, Publisher = @Publisher, Source = @Source, Quantity = @Quantity, Published = @Published, Category = @Category WHERE BookID = @BookID", con);

                    cmd.Parameters.AddWithValue("@BookTitle", bookTitle);
                    cmd.Parameters.AddWithValue("@Author", author);
                    cmd.Parameters.AddWithValue("@ISBN", isbn);
                    cmd.Parameters.AddWithValue("@Publisher", publisher);
                    cmd.Parameters.AddWithValue("@Source", source);
                    cmd.Parameters.AddWithValue("@Quantity", quantity);
                    cmd.Parameters.AddWithValue("@Published", published);
                    cmd.Parameters.AddWithValue("@Category", category);


                    cmd.Parameters.AddWithValue("@BookID", bookID); // Make sure bookID has a value

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Book updated successfully!");

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
                        txtDesc.Texts = book["description"]?.ToString() ?? "";

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
        txtDesc
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
    } //END OF MAIN METHOD

}
