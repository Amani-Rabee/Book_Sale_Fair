using System;
using System.Web.UI;
using Book_Sale_Fair.Model;

namespace Book_Sale_Fair
{
    public partial class VerifyEmail : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void VerifyButton_Click(object sender, EventArgs e)
        {
            string email = EmailTextBox.Text.Trim();
            string code = VerificationCodeTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(code))
            {
                DisplayError("Email and verification code are required.");
                return;
            }

            if (AuthHelper.VerifyCode(email, code))
            {
                DisplaySuccess("Email verified successfully!");
                // Redirect to the login page or dashboard
                Response.Redirect("SignIn.aspx");
            }
            else
            {
                DisplayError("Invalid verification code.");
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
