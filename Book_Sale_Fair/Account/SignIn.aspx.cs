using System;
using DevExpress.Web;
using Book_Sale_Fair.Model;
using System.Threading.Tasks;
using System.Web;

namespace Book_Sale_Fair
{
    public partial class SignInModule : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected  void SignInButton_Click(object sender, EventArgs e)
        {
            string userName = UserNameTextBox.Text.Trim();
            string password = PasswordButtonEdit.Text.Trim();

            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
            {
                DisplayError("Username and password are required.");
                return;
            }

            try
            {
                // Check if account is locked
                bool isAccountLocked = AuthHelper.IsAccountLocked(userName);
                if (isAccountLocked)
                {
                    DisplayError("Your account is locked. Please contact support for assistance.");
                    return;
                }

                // Attempt to sign in
                bool isAuthenticated =  AuthHelper.SignIn(userName, password);

                if (isAuthenticated)
                {
                    var userInfo = AuthHelper.GetLoggedInUserInfo();
                    if (userInfo != null)
                    {
                        // Set session information
                        HttpContext.Current.Session["User"] = new ApplicationUser
                        {
                            UserName = userInfo.UserName,
                            FirstName = userInfo.FirstName,
                            LastName = userInfo.LastName,
                            Email = userInfo.Email
                        };

                        // Check if user has set preferences
                        bool hasSetPreferences =  AuthHelper.HasSetPreferences(userInfo.UserName);
                        if (!hasSetPreferences)
                        {
                            Response.Redirect($"~/Preferences.aspx?username={userInfo.UserName}");
                        }
                        else
                        {
                            Response.Redirect("~/Home.aspx");
                        }
                    }
                }
                else
                {
                    DisplayError("Invalid username or password. Please try again.");
                }
            }
            catch (Exception ex)
            {
                // Log exception and display error message
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
