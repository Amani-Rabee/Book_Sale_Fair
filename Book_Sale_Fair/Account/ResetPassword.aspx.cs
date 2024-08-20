using System;
using Book_Sale_Fair.Model;

namespace Book_Sale_Fair
{
    public partial class ResetPassword : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Validate token and email on page load
            var email = Request.QueryString["email"];
            var token = Request.QueryString["token"];

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            {
                MessageLabel.Text = "Invalid or expired link.";
                return;
            }

            if (!AuthHelper.ValidatePasswordResetToken(email, token))
            {
                MessageLabel.Text = "Invalid or expired link.";
                return;
            }
        }

        protected void ResetPasswordButton_Click(object sender, EventArgs e)
        {
            var email = Request.QueryString["email"];
            var token = Request.QueryString["token"];
            var newPassword = NewPasswordTextBox.Text;

            if (AuthHelper.ResetPassword(email, token, newPassword))
            {
                MessageLabel.Text = "Password has been reset successfully.";
            }
            else
            {
                MessageLabel.Text = "Failed to reset password.";
            }
        }
    }
}
