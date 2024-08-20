using System;
using Book_Sale_Fair.Model;

namespace Book_Sale_Fair
{
    public partial class ForgotPassword : System.Web.UI.Page
    {
        protected void SendResetLinkButton_Click(object sender, EventArgs e)
        {
            var email = EmailTextBox.Text;

            if (!AuthHelper.IsRegistered(email))
            {
                MessageLabel.Text = "No account found with that email address.";
                return;
            }

            AuthHelper.GeneratePasswordResetToken(email);
            MessageLabel.Text = "Password reset link has been sent to your email.";
        }
    }
}