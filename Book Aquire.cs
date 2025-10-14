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

namespace LibraryCGC
{
    public partial class Book_Aquire : Form

    {
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

            isbnTimer.Stop();
            isbnTimer.Start(); // restart timer every time they type
         
        }


        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void Book_Aquire_Load(object sender, EventArgs e)
        {
            LoadBooksGrid();
        }

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private void arthanButton1_Load(object sender, EventArgs e)
        {

        }

        private void arthanButton1_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True"))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(@"INSERT INTO BooksAcq 
        (BookTitle, Author, ISBN, Publisher, Source, Quantity, Published, Category) 
        VALUES (@BookTitle, @Author, @ISBN, @Publisher, @Source, @Quantity, @Published, @Category)", con);



                




                // ✅ Add this block BEFORE cmd.Parameters.AddWithValue(...)

                // 🔄 Force ArthanTextBoxes to sync their visual text into actual .Text
                BookTitle.Text = BookTitle.Texts;
                Author.Text = Author.Texts;
                Publisher.Text = Publisher.Texts;
                Quantity.Text = Quantity.Texts;
                Published.Text = Published.Texts;
                Category.Text = Category.Texts;
                txtDesc.Text = txtDesc.Texts;
                ISBN.Text = ISBN.Texts;

                // ✅ Now use .Text (not .Texts) when saving
                cmd.Parameters.AddWithValue("@BookTitle", BookTitle.Text);
                cmd.Parameters.AddWithValue("@Author", Author.Text);
                cmd.Parameters.AddWithValue("@ISBN", ISBN.Text);
                cmd.Parameters.AddWithValue("@Publisher", Publisher.Text);
                cmd.Parameters.AddWithValue("@Source", Source.Text);
                cmd.Parameters.AddWithValue("@Quantity", Quantity.Text);
                cmd.Parameters.AddWithValue("@Published", Published.Text);
                cmd.Parameters.AddWithValue("@Category", Category.Text);

                cmd.ExecuteNonQuery();
                MessageBox.Show("Book added successfully!");
            }

            LoadBooksGrid();

            // clear all fields
            BookTitle.Texts = "";
            Author.Texts = "";
            ISBN.Texts = "";
            Publisher.Texts = "";
            Source.Text = "";
            Quantity.Texts = "";
            Published.Texts = "";
            Category.Texts = "";
            txtDesc.Texts = "";
            picCover.BackgroundImage = null;
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
            SqlConnection con = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");
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
            Source.Text = "";
            Quantity.Text = "";
            Published.Text = "";
            Category.Text = "";

        }


        private void ArchivedButton_Click(object sender, EventArgs e)
        {
            //not already apply thisssssssssssssssssssssssssssssssssssssss
            // archive function ----------------------

            SqlConnection con = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");
            con.Open();

            SqlCommand cmd = new SqlCommand("INSERT INTO BooksArchive (BookTitle, Author, ISBN, Publisher, Source, Quantity, Published, Category) " +
                "VALUES ( @BookTitle, @Author, @ISBN, @Publisher, @Source, @Quantity, @Published, @Category)", con);


            cmd.Parameters.AddWithValue("@BookTitle", BookTitle.Text);
            cmd.Parameters.AddWithValue("@Author", Author.Text);
            cmd.Parameters.AddWithValue("@ISBN", ISBN.Text);
            cmd.Parameters.AddWithValue("@Publisher", Publisher.Text);
            cmd.Parameters.AddWithValue("@Source", Source.Text);
            cmd.Parameters.AddWithValue("@Quantity", Quantity.Text);
            cmd.Parameters.AddWithValue("@Published", Published.Text);
            cmd.Parameters.AddWithValue("@Category", Category.Text);

            cmd.ExecuteNonQuery();
            MessageBox.Show("Book archived successfully!");

            LoadBooksGrid(); // Optional: reload if showing archived books

            con.Close();

            // Clear fields

            BookTitle.Text = "";
            Author.Text = "";
            ISBN.Text = "";
            Publisher.Text = "";
            Source.Text = "";
            Quantity.Text = "";
            Published.Text = "";
            Category.Text = "";

        }

        private void ArchiveBookFromRow(DataGridViewRow row)
        {//not already apply thissssssssssssssssssssssssssssssssssssss
            using (SqlConnection con = new SqlConnection(
    "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True"))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand(@"
        INSERT INTO BooksArchive 
        (BookID, BookTitle, Author, ISBN, Publisher, Source, Quantity, Published, Category, ArchivedDate)
        VALUES 
        (@BookID, @BookTitle, @Author, @ISBN, @Publisher, @Source, @Quantity, @Published, @Category, @ArchivedDate)", con))
                {
                    cmd.Parameters.AddWithValue("@BookID", row.Cells["BookID"].Value ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@BookTitle", row.Cells["BookTitle"].Value ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Author", row.Cells["Author"].Value ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ISBN", row.Cells["ISBN"].Value ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Publisher", row.Cells["Publisher"].Value ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Source", row.Cells["Source"].Value ?? DBNull.Value);

                    // Convert Quantity from string to int (handle nulls safely)
                    if (int.TryParse(row.Cells["Quantity"].Value?.ToString(), out int qty))
                        cmd.Parameters.AddWithValue("@Quantity", qty);
                    else
                        cmd.Parameters.AddWithValue("@Quantity", DBNull.Value);

                    cmd.Parameters.AddWithValue("@Published", row.Cells["Published"].Value ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Category", row.Cells["Category"].Value ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ArchivedDate", DateTime.Now);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Book archived successfully!");
                }
            }

        }

        private void DeleteFromBooksAcq(string bookID) //delete after archive
        {
            //not already apply thisssssssssssssssssss
            SqlConnection con = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");
            con.Open();

            SqlCommand cmd = new SqlCommand("DELETE FROM BooksAcq WHERE BookID = @BookID", con);
            cmd.Parameters.AddWithValue("@BookID", bookID);
            cmd.ExecuteNonQuery();

            con.Close();
        }


        private void LoadBooksGrid()
        {
            //output the datagriddddddddddddddd
            using (SqlConnection con = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True"))
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
                using (SqlConnection con = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True"))
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
            // Hide this form and show Form1 (Home)
            Form1 homeForm = new Form1();
            homeForm.ShowDialog();
            this.Hide();

        }


        private void arthanPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void kryptonButton1_Load(object sender, EventArgs e)
        {


        }

        private void picCover_Click(object sender, EventArgs e)
        {
            picCover.SizeMode = PictureBoxSizeMode.Zoom; // or CenterImage
        }
    } //END OF MAIN METHOD

}
