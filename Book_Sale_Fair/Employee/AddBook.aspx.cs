using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI;
using Book_Sale_Fair.Model;
using DevExpress.Web;

namespace Book_Sale_Fair.Employee
{
    public partial class AddBook : Page
    {
        public static string ConnectionString = "Data Source=AMANI;Initial Catalog=Sample2;Integrated Security=True;";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthHelper.IsAuthenticated() || AuthHelper.GetUserRole() != "Employee")
            {
                Response.Redirect("~/Account/SignIn.aspx");
            }

            if (!IsPostBack)
            {
                string csrfToken = GenerateCsrfToken();
                csrfTokenField.Value = csrfToken;
                Session["CsrfToken"] = csrfToken;

                BindGrid();
                UpdateTotalBooks();
                PopulateCategories();
            }
        }

        protected void AddBookButton_Click(object sender, EventArgs e)
        {
            string storedToken = Session["CsrfToken"] as string;
            if (csrfTokenField.Value != storedToken)
            {
                DisplayError("Invalid request. Please try again.");
                return;
            }

            try
            {
                string title = TitleTextBox.Text.Trim();
                string author = AuthorTextBox.Text.Trim();
                string description = DescriptionMemo.Text.Trim();
                string priceText = PriceSpinEdit.Text.Trim();
                string categoryIDText = CategoryComboBox.SelectedItem?.Value.ToString();
                string stockQuantityText = StockQuantitySpinEdit.Text.Trim();

                if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(author) || string.IsNullOrWhiteSpace(priceText) || string.IsNullOrWhiteSpace(categoryIDText) || string.IsNullOrWhiteSpace(stockQuantityText))
                {
                    DisplayError("All fields are required.");
                    return;
                }

                decimal price;
                int categoryID;
                int stockQuantity;

                if (!decimal.TryParse(priceText, out price))
                {
                    DisplayError("Invalid price format.");
                    return;
                }

                if (!int.TryParse(categoryIDText, out categoryID))
                {
                    DisplayError("Invalid category ID.");
                    return;
                }

                if (!int.TryParse(stockQuantityText, out stockQuantity))
                {
                    DisplayError("Invalid stock quantity.");
                    return;
                }

                string imageUrl = string.Empty;
                if (BookImageUpload.HasFile)
                {
                    string fileName = Path.GetFileName(BookImageUpload.PostedFile.FileName);
                    string fileExtension = Path.GetExtension(fileName);
                    string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };

                    if (Array.Exists(allowedExtensions, ext => ext.Equals(fileExtension, StringComparison.OrdinalIgnoreCase)))
                    {
                        string uploadPath = Server.MapPath("~/BookImages/");
                        if (!Directory.Exists(uploadPath))
                        {
                            Directory.CreateDirectory(uploadPath);
                        }

                        string uniqueFileName = Guid.NewGuid() + fileExtension;
                        string filePath = Path.Combine(uploadPath, uniqueFileName);
                        BookImageUpload.PostedFile.SaveAs(filePath);
                        imageUrl = "/BookImages/" + uniqueFileName;
                    }
                    else
                    {
                        DisplayError("Only image files (.jpg, .jpeg, .png, .gif) are allowed.");
                        return;
                    }
                }

                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    string query = "INSERT INTO Books (Title, Author, Description, Price, CategoryID, StockQuantity, ImageUrl) VALUES (@Title, @Author, @Description, @Price, @CategoryID, @StockQuantity, @ImageUrl)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Title", title);
                        command.Parameters.AddWithValue("@Author", author);
                        command.Parameters.AddWithValue("@Description", description);
                        command.Parameters.AddWithValue("@Price", price);
                        command.Parameters.AddWithValue("@CategoryID", categoryID);
                        command.Parameters.AddWithValue("@StockQuantity", stockQuantity);
                        command.Parameters.AddWithValue("@ImageUrl", imageUrl);

                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }

                DisplaySuccess("Book added successfully.");
                ClearForm();
                BindGrid();
                UpdateTotalBooks();
            }
            catch (Exception ex)
            {
                DisplayError("An error occurred: " + ex.Message);
            }
        }
        protected void BooksGridView_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                int bookID = Convert.ToInt32(e.Keys["BookID"]);
                string title = e.NewValues["Title"].ToString();
                string author = e.NewValues["Author"].ToString();
                string description = e.NewValues["Description"].ToString();
                decimal price = Convert.ToDecimal(e.NewValues["Price"]);
                int categoryID = Convert.ToInt32(e.NewValues["CategoryID"]);
                int stockQuantity = Convert.ToInt32(e.NewValues["StockQuantity"]);
                

                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    string query = "UPDATE Books SET Title = @Title, Author = @Author, Description = @Description, Price = @Price, CategoryID = @CategoryID, StockQuantity = @StockQuantity WHERE BookID = @BookID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Title", title);
                        command.Parameters.AddWithValue("@Author", author);
                        command.Parameters.AddWithValue("@Description", description);
                        command.Parameters.AddWithValue("@Price", price);
                        command.Parameters.AddWithValue("@CategoryID", categoryID);
                        command.Parameters.AddWithValue("@StockQuantity", stockQuantity);
                        command.Parameters.AddWithValue("@BookID", bookID);

                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }

                BooksGridView.CancelEdit();
                BindGrid();
                UpdateTotalBooks();
            }
            catch (Exception ex)
            {
                DisplayError("An error occurred: " + ex.Message);
            }
        }

        protected void SearchTextBox_TextChanged(object sender, EventArgs e)
        {
            BindGrid();
        }

        protected void BooksGridView_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                int bookID = Convert.ToInt32(e.Keys["BookID"]);

                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    string query = "DELETE FROM Books WHERE BookID = @BookID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@BookID", bookID);

                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }

                BindGrid();
                UpdateTotalBooks();
            }
            catch (Exception ex)
            {
                DisplayError("An error occurred: " + ex.Message);
            }

            e.Cancel = true;
        }

        private void PopulateCategories()
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = "SELECT CategoryID, CategoryName FROM Categories";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    CategoryComboBox.DataSource = reader;
                    CategoryComboBox.TextField = "CategoryName";
                    CategoryComboBox.ValueField = "CategoryID";
                    CategoryComboBox.DataBind();
                }
            }
        }

        private void BindGrid()
        {
            string searchText = SearchTextBox.Text.Trim();
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = "SELECT BookID, Title, Author, Description, Price, CategoryID, StockQuantity, ImageUrl FROM Books WHERE Title LIKE @SearchText OR Author LIKE @SearchText";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SearchText", "%" + searchText + "%");

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    BooksGridView.DataSource = dataTable;
                    BooksGridView.DataBind();
                }
            }
        }

        private void UpdateTotalBooks()
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = "SELECT COUNT(*) FROM Books";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    int totalBooks = (int)command.ExecuteScalar();
                    TotalBooksLabel.Text = "Total Books: " + totalBooks;
                }
            }
        }

        private void ClearForm()
        {
            TitleTextBox.Text = string.Empty;
            AuthorTextBox.Text = string.Empty;
            DescriptionMemo.Text = string.Empty;
            PriceSpinEdit.Text = string.Empty;
            CategoryComboBox.SelectedIndex = -1;
            StockQuantitySpinEdit.Text = string.Empty;
            BookImageUpload.Attributes.Clear();
        }

        private void DisplayError(string message)
        {
            ErrorMessageLabel.Text = message;
        }

        private void DisplaySuccess(string message)
        {
            SuccessMessageLabel.Text = message;
        }

        private string GenerateCsrfToken()
        {
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] tokenBytes = new byte[32];
                rng.GetBytes(tokenBytes);
                return Convert.ToBase64String(tokenBytes);
            }
        }
    }
}
