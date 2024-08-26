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
            HideUnusedContent();
            UpdateUserInfo();
            AddHomeAndOrdersMenuItems();
        }

        protected void HideUnusedContent()
        {
            var isAuthenticated = AuthHelper.IsAuthenticated();
            var isAdmin = false; // Placeholder for admin check
            var isEmployee = false; // Placeholder for employee check

            if (isAuthenticated)
            {
                var loggedInUser = AuthHelper.GetLoggedInUserInfo();
                if (loggedInUser != null)
                {
                    // Logic based on user roles
                }
            }

            // Hide or show content based on roles
            if (isAuthenticated && !isAdmin)
            {
                // Example: Hide Admin-specific content
            }

            if (isAuthenticated && !isEmployee)
            {
                // Example: Hide Employee-specific content
            }
        }

        protected bool HasContent(Control contentPlaceHolder)
        {
            return contentPlaceHolder?.Controls.Count > 0;
        }

        protected void UpdateUserMenuItemsVisible()
        {
            var isAuthenticated = AuthHelper.IsAuthenticated();
            RightAreaMenu.Items.FindByName("SignInItem").Visible = !isAuthenticated;
            RightAreaMenu.Items.FindByName("RegisterItem").Visible = !isAuthenticated;
            RightAreaMenu.Items.FindByName("MyAccountItem").Visible = isAuthenticated;
            RightAreaMenu.Items.FindByName("SignOutItem").Visible = isAuthenticated;
        }

        protected void UpdateUserInfo()
        {
            if (AuthHelper.IsAuthenticated())
            {
                var userInfo = AuthHelper.GetLoggedInUserInfo();
                if (userInfo != null)
                {
                    var accountMenuItem = (MenuItem)RightAreaMenu.Items.FindByName("MyAccountItem");
                    if (accountMenuItem != null)
                    {
                        var userInfoPanel = (Control)accountMenuItem.FindControl("UserInfoPanel");
                        if (userInfoPanel != null)
                        {
                            var userNameLabel = (ASPxLabel)userInfoPanel.FindControl("UserNameLabel");
                            var emailLabel = (ASPxLabel)userInfoPanel.FindControl("EmailLabel");
                            var avatarImage = (ASPxImage)userInfoPanel.FindControl("AvatarImage");

                            if (userNameLabel != null) userNameLabel.Text = $"{userInfo.FirstName} {userInfo.LastName}";
                            if (emailLabel != null) emailLabel.Text = userInfo.Email;
                            if (avatarImage != null) avatarImage.ImageUrl = userInfo.AvatarUrl;
                        }
                    }
                }
            }
        }

        protected void AddHomeAndOrdersMenuItems()
        {
            var isAuthenticated = AuthHelper.IsAuthenticated();
            if (isAuthenticated)
            {
                var homeMenuItem = new MenuItem("Home", null, "~/Content/Images/home.svg", "~/Home.aspx");
                var ordersMenuItem = new MenuItem("Orders", null, "~/Content/Images/orders.svg", "~/Orders.aspx");

                var accountMenuItem = (MenuItem)RightAreaMenu.Items.FindByName("AccountItem");
                if (accountMenuItem != null)
                {
                    var signOutItemIndex = accountMenuItem.Items.IndexOf(accountMenuItem.Items.FindByName("SignOutItem"));

                    if (!accountMenuItem.Items.Contains(homeMenuItem))
                    {
                        accountMenuItem.Items.Insert(signOutItemIndex, homeMenuItem);
                    }

                    if (!accountMenuItem.Items.Contains(ordersMenuItem))
                    {
                        accountMenuItem.Items.Insert(signOutItemIndex + 1, ordersMenuItem);
                    }
                }
            }
        }

        protected void RightAreaMenu_ItemClick(object source, DevExpress.Web.MenuItemEventArgs e)
        {
            if (e.Item.Name == "SignOutItem")
            {
                AuthHelper.SignOut();
                Response.Redirect("~/");
            }
        }

        protected void ApplicationMenu_ItemDataBound(object source, MenuItemEventArgs e)
        {
            e.Item.Image.Url = string.Format("Content/Images/{0}.svg", e.Item.Text);
            e.Item.Image.UrlSelected = string.Format("Content/Images/{0}-white.svg", e.Item.Text);
        }
    }
}
