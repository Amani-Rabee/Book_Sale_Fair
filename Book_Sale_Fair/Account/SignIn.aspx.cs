using System;
using DevExpress.Web;
using Book_Sale_Fair.Model;
using System.Threading.Tasks;

namespace Book_Sale_Fair
{
    public partial class SignInModule : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected async void SignInButton_Click(object sender, EventArgs e)
        {
            // Trim and sanitize user input for the sign-in form
            string userName = UserNameTextBox.Text.Trim();
            string password = PasswordButtonEdit.Text.Trim();

            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
            {
                DisplayError("Username and password are required.");
                return;
            }

            try
            {
                // Check if the user's account is locked
                bool isAccountLocked = await Task.Run(() => AuthHelper.IsAccountLocked(userName));
                if (isAccountLocked)
                {
                    DisplayError("Your account is locked. Please contact support for assistance.");
                    return;
                }

                // Perform user authentication asynchronously using the correct SignIn method
                bool isAuthenticated = await Task.Run(() => AuthHelper.SignIn(userName, password));

                if (isAuthenticated)
                {
                    // Redirect to the home page or dashboard after successful sign-in
                    Response.Redirect("HomePage.aspx");
                }
                else
                {
                    DisplayError("Invalid username or password. Please try again.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception in SignInButton_Click: {ex.Message}\n{ex.StackTrace}");
                DisplayError("An error occurred during sign-in. Please try again.");
            }
        }

        protected void RegisterLink_Click(object sender, EventArgs e)
        {
            // Redirect to the registration page
            Response.Redirect("~/Register.aspx");
        }
        private void DisplayError(string message)
        {
            ErrorMessageLabel.Text = message;
            ErrorMessageLabel.Visible = true;
            SuccessMessageLabel.Visible = false;
        }

    }
}
