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

            SqlCommand cmd = new SqlCommand("INSERT INTO BooksAcq (BookID, BookTitle, Author, ISBN, Publisher, Source, Quantity, Published, Category) " +
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

            //output the datagrid 
            SqlConnection con = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");
            con.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM BooksAcq", con);
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
            SqlCommand cmd = new SqlCommand("UPDATE BooksAcq SET BookTitle = @BookTitle, Author = @Author, ISBN = @ISBN, Publisher = @Publisher, Source = @Source, Quantity = @Quantity, Published = @Published, Category = @Category WHERE BookID = @BookID", con);

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

            string query = "SELECT * FROM BooksAcq WHERE 1=1";  // Safe base
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;

            if (!string.IsNullOrWhiteSpace(BookID.Text))
            {
                query += " AND BookID LIKE @BookID";
                cmd.Parameters.AddWithValue("@BookID", "%" + BookID.Text + "%");
            }
            if (!string.IsNullOrWhiteSpace(BookTitle.Text))
            {
                query += " AND BookTitle LIKE @BookTitle";
                cmd.Parameters.AddWithValue("@BookTitle", "%" + BookTitle.Text + "%");
            }
            if (!string.IsNullOrWhiteSpace(Author.Text))
            {
                query += " AND Author LIKE @Author";
                cmd.Parameters.AddWithValue("@Author", "%" + Author.Text + "%");
            }
            if (!string.IsNullOrWhiteSpace(ISBN.Text))
            {
                query += " AND ISBN LIKE @ISBN";
                cmd.Parameters.AddWithValue("@ISBN", "%" + ISBN.Text + "%");
            }
            if (!string.IsNullOrWhiteSpace(Publisher.Text))
            {
                query += " AND Publisher LIKE @Publisher";
                cmd.Parameters.AddWithValue("@Publisher", "%" + Publisher.Text + "%");
            }
            if (!string.IsNullOrWhiteSpace(Source.Text))
            {
                query += " AND Source LIKE @Source";
                cmd.Parameters.AddWithValue("@Source", "%" + Source.Text + "%");
            }
            if (!string.IsNullOrWhiteSpace(Quantity.Text))
            {
                query += " AND Quantity LIKE @Quantity";
                cmd.Parameters.AddWithValue("@Quantity", "%" + Quantity.Text + "%");
            }
            if (!string.IsNullOrWhiteSpace(Published.Text))
            {
                query += " AND Published LIKE @Published";
                cmd.Parameters.AddWithValue("@Published", "%" + Published.Text + "%");
            }
            if (!string.IsNullOrWhiteSpace(Category.Text))
            {
                query += " AND Category LIKE @Category";
                cmd.Parameters.AddWithValue("@Category", "%" + Category.Text + "%");
            }

            cmd.CommandText = query;

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            DataGridTotalBooks.DataSource = dt;
            con.Close();







        }

        private void SearchBtn_Click(object sender, EventArgs e)
        {

        }

        private void DataGridTotalBooks_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void BookID_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
