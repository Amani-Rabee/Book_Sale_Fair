using System;
using System.Web.UI;
using Book_Sale_Fair.Model;
using DevExpress.Web;

namespace Book_Sale_Fair
{
    public partial class Root : MasterPage
    {
        public bool EnableBackButton { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Page.Header.Title))
                Page.Header.Title += " - ";
            Page.Header.Title = Page.Header.Title + "Book Sale Fair";

            Page.Header.DataBind();
            UpdateUserMenuItemsVisible();
            UpdateUserInfo();
        }

        protected void UpdateUserMenuItemsVisible()
        {
            var isAuthenticated = AuthHelper.IsAuthenticated();
            var userRole = AuthHelper.GetUserRole();

            // Show/hide common items
            RightAreaMenu.Items.FindByName("SignInItem").Visible = !isAuthenticated;
            RightAreaMenu.Items.FindByName("RegisterItem").Visible = !isAuthenticated;
            RightAreaMenu.Items.FindByName("MyAccountItem").Visible = isAuthenticated;
            RightAreaMenu.Items.FindByName("SignOutItem").Visible = isAuthenticated;

            // Show/hide menu items based on role
            LeftAreaMenu.Items.FindByName("OrdersMenuItem").Visible = isAuthenticated && userRole == "Customer";
            LeftAreaMenu.Items.FindByName("CartMenuItem").Visible = isAuthenticated && userRole == "Customer";

            LeftAreaMenu.Items.FindByName("AddEmployeeMenuItem").Visible = isAuthenticated && userRole == "Admin";

            LeftAreaMenu.Items.FindByName("AllOrdersMenuItem").Visible = isAuthenticated && userRole == "Employee";
            LeftAreaMenu.Items.FindByName("AddBookMenuItem").Visible = isAuthenticated && userRole == "Employee";
        }

        protected void UpdateUserInfo()
        {
            if (AuthHelper.IsAuthenticated())
            {
                var userInfo = AuthHelper.GetLoggedInUserInfo();
                if (userInfo != null)
                {
                    var userNameLabel = (ASPxLabel)FindControlRecursive(RightAreaMenu, "UserNameLabel");
                    var emailLabel = (ASPxLabel)FindControlRecursive(RightAreaMenu, "EmailLabel");
                    var avatarImage = (ASPxImage)FindControlRecursive(RightAreaMenu, "AvatarImage");

                    if (userNameLabel != null) userNameLabel.Text = $"{userInfo.FirstName} {userInfo.LastName}";
                    if (emailLabel != null) emailLabel.Text = userInfo.Email;
                    if (avatarImage != null) avatarImage.ImageUrl = userInfo.AvatarUrl ?? "Content/Images/user.svg";
                }
            }
        }

        private Control FindControlRecursive(Control root, string id)
        {
            if (root.ID == id) return root;
            foreach (Control control in root.Controls)
            {
                var foundControl = FindControlRecursive(control, id);
                if (foundControl != null) return foundControl;
            }
            return null;
        }
        protected void RightAreaMenu_ItemClick(object source, MenuItemEventArgs e)
        {
            if (e.Item.Name == "SignOutItem")
            {
                AuthHelper.SignOut();
                Response.Redirect("~/Home.aspx");
            }
        }
    }
}