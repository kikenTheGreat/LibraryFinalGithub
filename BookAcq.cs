using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Library_Final
{
    public partial class BookAcq : Form
    {
        public BookAcq()
        {
            InitializeComponent();
        }

        private void kryptonButton5_Click(object sender, EventArgs e)
        {
            Dashboard dashboard = new Dashboard();
            dashboard.ShowDialog();
            this.Close();
        }

        private void kryptonButton1_Click(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");
            con.Open();

            SqlCommand cmd = new SqlCommand("INSERT INTO BooksTotal (BookID, BookTitle, Author, ISBN, Publisher, Source, Quantity, Published, Category) " +
                                "VALUES (@BookID, @BookTitle, @Author, @ISBN, @Publisher, @Source, @Quantity, @Published, @Category)", con);



            cmd.Parameters.AddWithValue("@BookID", BookID.Text);
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
            con.Close();

            BookID.Text = " ";      // what will display after inserting
            BookTitle.Text = " ";
            Author.Text = " ";
            ISBN.Text = " ";
            Publisher.Text = " ";
            Source.Text = " ";
            Quantity.Text = " ";
            Published.Text = " ";
            Category.Text = " ";

        }

        private void kryptonComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void BookAcq_Load(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");
            con.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM BooksTotal", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            DataGridTotalBooks.DataSource = dt;
        }

        private void kryptonButton6_Click(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");
            con.Open();
            //use WHERE to specify what record to UPDATE                                                                                              
            SqlCommand cmd = new SqlCommand("UPDATE BooksTotal SET BookTitle = @BookTitle, Author = @Author, ISBN = @ISBN, Publisher = @Publisher, Source = @Source, Quantity = @Quantity, Published = @Published, Category = @Category WHERE BookID = @BookID", con);

            cmd.Parameters.AddWithValue("@BookID", BookID.Text);  // this will be used in WHERE clause
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
            con.Close();

            // Clear the textboxes after update
            BookID.Text = "";
            BookTitle.Text = "";
            Author.Text = "";
            ISBN.Text = "";
            Publisher.Text = "";
            Source.Text = "";
            Quantity.Text = "";
            Published.Text = "";
            Category.Text = "";

        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");
            con.Open();

            string keyword = "%" + SearchButton.Text + "%"; // This is your search textbox

            SqlCommand cmd = new SqlCommand(@"SELECT * FROM BooksTotal WHERE 
    BookID LIKE @keyword OR 
    BookTitle LIKE @keyword OR 
    Author LIKE @keyword OR 
    ISBN LIKE @keyword OR 
    Publisher LIKE @keyword OR 
    Source LIKE @keyword OR 
    Quantity LIKE @keyword OR 
    Published LIKE @keyword OR 
    Category LIKE @keyword", con);

            cmd.Parameters.AddWithValue("@keyword", keyword);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            DataGridTotalBooks.DataSource = dt;
            con.Close();







        }

        private void SearchBtn_Click(object sender, EventArgs e)
        {

        }
    }
}
