using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using Book_Sale_Fair.Model;

namespace Book_Sale_Fair
{
    public partial class RegisterModule : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Generate CSRF token and store it in a hidden field to pervent the CSRF attacks 
                string csrfToken = GenerateCsrfToken();
                csrfTokenField.Value = csrfToken;
                Session["CsrfToken"] = csrfToken;
            }
        }

        protected async void RegisterButton_Click(object sender, EventArgs e)
        {
            // Validate the CSRF token to ensure the request is legitimate
            string storedToken = Session["CsrfToken"] as string;
            if (csrfTokenField.Value != storedToken)
            {
                DisplayError("Invalid request. Please try again.");
                return;
            }
            // Trim and sanitize user input for the registration form
            string userName = RegisterUserNameTextBox.Text.Trim();
            string firstName = FirstNameTextBox.Text.Trim();
            string lastName = LastNameTextBox.Text.Trim();
            string email = EmailTextBox.Text.Trim();
            string password = PasswordButtonEdit.Text.Trim();
            string confirmPassword = ConfirmPasswordButtonEdit.Text.Trim();

            //validations 
            if (password != confirmPassword)
            {
                DisplayError("Passwords do not match.");
                return;
            }

            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(email) ||string.IsNullOrWhiteSpace(password))
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

            

            try
            {
                // Perform user registration asynchronously
                bool registrationSuccess = await Task.Run(() => AuthHelper.RegisterUser(userName, firstName, lastName, email, password));

                if (registrationSuccess)
                {
                    // Generate a 6-digit verification code
                    string verificationCode = new Random().Next(100000, 999999).ToString();

                    // Save the verification token and send the email asynchronously
                    await Task.Run(() => AuthHelper.SaveVerificationToken(email, verificationCode));
                    //await Task.Run(() => AuthHelper.SendVerificationEmail(email, verificationCode));

                    //DisplaySuccess("Registration successful! Please check your email for the verification code.");
                    //Response.Redirect("VerifyEmail.aspx");
                    RegisterUserNameTextBox.Text = string.Empty;
                    FirstNameTextBox.Text = string.Empty;
                    LastNameTextBox.Text = string.Empty;
                    EmailTextBox.Text = string.Empty;
                    PasswordButtonEdit.Text = string.Empty;
                    ConfirmPasswordButtonEdit.Text = string.Empty;

                    // Redirect to the login page with the registered username pre-filled
                    Response.Redirect($"SignIn.aspx?username={Server.UrlEncode(userName)}");
                }
                else
                {
                    DisplayError("Registration failed. User might already exist.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception in RegisterButton_Click: {ex.Message}\n{ex.StackTrace}");
                DisplayError("An error occurred during registration. Please try again.");
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
    }
}
