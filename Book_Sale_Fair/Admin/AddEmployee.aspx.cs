using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI;
using Book_Sale_Fair.Model;
using DevExpress.Web;

namespace Book_Sale_Fair.Admin
{
    public partial class AddEmployee : Page
    {
        public static string ConnectionString = "Data Source=AMANI;Initial Catalog=Sample2;Integrated Security=True;";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthHelper.IsAuthenticated() || AuthHelper.GetUserRole() != "Admin")
            {
                Response.Redirect("~/Account/SignIn.aspx");
            }

            if (!IsPostBack)
            {
                string csrfToken = GenerateCsrfToken();
                csrfTokenField.Value = csrfToken;
                Session["CsrfToken"] = csrfToken;

                BindGrid();
                UpdateTotalEmployees();
            }
        }

        protected void AddEmployeeButton_Click(object sender, EventArgs e)
        {
            string storedToken = Session["CsrfToken"] as string;
            if (csrfTokenField.Value != storedToken)
            {
                DisplayError("Invalid request. Please try again.");
                return;
            }

            try
            {
                string userName = UserNameTextBox.Text.Trim();
                string firstName = FirstNameTextBox.Text.Trim();
                string lastName = LastNameTextBox.Text.Trim();
                string email = EmailTextBox.Text.Trim();
                string password = PasswordTextBox.Text.Trim();
                string role = RoleComboBox.SelectedItem?.Value.ToString();

                if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(role))
                {
                    DisplayError("All fields are required.");
                    return;
                }

                if (!IsValidEmail(email))
                {
                    DisplayError("Invalid email address.");
                    return;
                }

                if (password.Length < 8 || !HasSpecialCharacter(password))
                {
                    DisplayError("Password must be at least 8 characters long and contain a special character.");
                    return;
                }

                string hashedPassword = HashPassword(password);

                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    string query = "INSERT INTO Users (UserName, FirstName, LastName, Email, PasswordHash, Role) VALUES (@UserName, @FirstName, @LastName, @Email, @PasswordHash, @Role)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserName", userName);
                        cmd.Parameters.AddWithValue("@FirstName", firstName);
                        cmd.Parameters.AddWithValue("@LastName", lastName);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@PasswordHash", hashedPassword);
                        cmd.Parameters.AddWithValue("@Role", role);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                ClearFields();
                BindGrid();
                UpdateTotalEmployees();
                DisplaySuccess("Employee added successfully.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in AddEmployeeButton_Click: {ex.Message}\n{ex.StackTrace}");
                DisplayError("An error occurred while adding the employee. Please try again.");
            }
        }

        private string GenerateCsrfToken()
        {
            byte[] tokenBytes = new byte[32];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(tokenBytes);
            }
            return Convert.ToBase64String(tokenBytes);
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var mailAddress = new System.Net.Mail.MailAddress(email);
                return mailAddress.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool HasSpecialCharacter(string input)
        {
            return input.Any(ch => !char.IsLetterOrDigit(ch));
        }

        private string HashPassword(string password)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, 16, 10000, HashAlgorithmName.SHA256))
            {
                byte[] hash = pbkdf2.GetBytes(20);
                byte[] salt = pbkdf2.Salt;
                byte[] hashBytes = new byte[36];
                Array.Copy(salt, 0, hashBytes, 0, 16);
                Array.Copy(hash, 0, hashBytes, 16, 20);
                return Convert.ToBase64String(hashBytes);
            }
        }

        private void DisplayError(string message)
        {
            ErrorMessageLabel.Text = message;
            ErrorMessageLabel.Visible = true;
            SuccessMessageLabel.Visible = false;
        }

        private void DisplaySuccess(string message)
        {
            SuccessMessageLabel.Text = message;
            SuccessMessageLabel.Visible = true;
            ErrorMessageLabel.Visible = false;
        }

        private void ClearFields()
        {
            UserNameTextBox.Text = string.Empty;
            FirstNameTextBox.Text = string.Empty;
            LastNameTextBox.Text = string.Empty;
            EmailTextBox.Text = string.Empty;
            PasswordTextBox.Text = string.Empty;
            RoleComboBox.SelectedIndex = -1;
        }
        protected void SearchTextBox_TextChanged(object sender, EventArgs e)
        {
            BindGrid();
        }

        private void BindGrid()
        {
            try
            {
                string searchText = SearchTextBox.Text.Trim();

                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    string query = "SELECT * FROM Users WHERE Role IN ('Admin', 'Employee') AND (UserName LIKE @SearchText OR FirstName LIKE @SearchText OR LastName LIKE @SearchText OR Email LIKE @SearchText)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SearchText", "%" + searchText + "%");

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        EmployeesGridView.DataSource = dt;
                        EmployeesGridView.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessageLabel.Text = "An error occurred while loading employee data. Please try again.";
            }
        }

        protected void EmployeesGridView_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                string userName = e.Keys["UserName"].ToString();

                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    string query = "DELETE FROM Users WHERE UserName = @UserName";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserName", userName);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                e.Cancel = true;

                BindGrid();
                UpdateTotalEmployees();
            }
            catch (Exception ex)
            {
                ErrorMessageLabel.Text = "An error occurred while deleting the employee. Please try again.";
            }
        }

        private void UpdateTotalEmployees()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    string query = "SELECT COUNT(*) FROM Users WHERE Role IN ('Admin', 'Employee')";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();
                        int count = (int)cmd.ExecuteScalar();
                        TotalEmployeesLabel.Text = $"Total Employees: {count}";
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessageLabel.Text = "An error occurred while updating the total number of employees. Please try again.";
            }
        }


    }
}
