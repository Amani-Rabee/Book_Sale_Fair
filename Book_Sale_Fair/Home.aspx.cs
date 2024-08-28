using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI.WebControls;
using Book_Sale_Fair.Model;

namespace Book_Sale_Fair
{
    public partial class Home : System.Web.UI.Page
    {
        private static string ConnectionString = "Data Source=AMANI;Initial Catalog=Sample2;Integrated Security=True;";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadCategories();
                LoadBooks();
            }
        }

        private void LoadCategories()
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT CategoryID, CategoryName FROM Categories", conn);
                SqlDataReader reader = cmd.ExecuteReader();
                ddlCategoryFilter.DataSource = reader;
                ddlCategoryFilter.DataTextField = "CategoryName";
                ddlCategoryFilter.DataValueField = "CategoryID";
                ddlCategoryFilter.DataBind();
                ddlCategoryFilter.Items.Insert(0, new ListItem("All Categories", "0"));
            }
        }

        private void LoadBooks(string search = "", int categoryId = 0)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Books WHERE (@Search = '' OR Title LIKE '%' + @Search + '%') AND (@CategoryID = 0 OR CategoryID = @CategoryID)", conn);
                cmd.Parameters.AddWithValue("@Search", search);
                cmd.Parameters.AddWithValue("@CategoryID", categoryId);
                SqlDataReader reader = cmd.ExecuteReader();
                rptBooks.DataSource = reader;
                rptBooks.DataBind();
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadBooks(txtSearch.Text.Trim(), int.Parse(ddlCategoryFilter.SelectedValue));
        }

        protected void ddlCategoryFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadBooks(txtSearch.Text.Trim(), int.Parse(ddlCategoryFilter.SelectedValue));
        }

        protected void rptBooks_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "AddToCart")
            {
                if (AuthHelper.IsAuthenticated()&&AuthHelper.GetUserRole()=="Customer")
                {
                    int bookId = Convert.ToInt32(e.CommandArgument);
                    string userName = AuthHelper.GetLoggedInUserInfo().UserName;
                    AddToCart(userName, bookId);

                    System.Diagnostics.Debug.WriteLine($"Book ID {bookId} added to cart for user {userName}");
                } else
                {
                    Response.Redirect("~/Account/SignIn.aspx");
                }
            }
        }

        private void AddToCart(string userName, int bookId)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                // Check if a cart exists for this user
                SqlCommand checkCartCmd = new SqlCommand("SELECT CartID FROM Cart WHERE UserName = @UserName", conn);
                checkCartCmd.Parameters.AddWithValue("@UserName", userName);
                object cartIdObj = checkCartCmd.ExecuteScalar();

                int cartId;
                if (cartIdObj != null)
                {
                    cartId = Convert.ToInt32(cartIdObj);
                }
                else
                {
                    // Create a new cart for the user
                    SqlCommand createCartCmd = new SqlCommand("INSERT INTO Cart (UserName) OUTPUT INSERTED.CartID VALUES (@UserName)", conn);
                    createCartCmd.Parameters.AddWithValue("@UserName", userName);
                    cartId = (int)createCartCmd.ExecuteScalar();
                }

                // Add the book to the CartItems table
                SqlCommand addItemCmd = new SqlCommand("IF EXISTS (SELECT * FROM CartItems WHERE CartID = @CartID AND BookID = @BookID) " +
                                                        "BEGIN " +
                                                        "    UPDATE CartItems SET Quantity = Quantity + 1 WHERE CartID = @CartID AND BookID = @BookID " +
                                                        "END " +
                                                        "ELSE " +
                                                        "BEGIN " +
                                                        "    INSERT INTO CartItems (CartID, BookID, Quantity, Price) " +
                                                        "    SELECT @CartID, @BookID, 1, Price FROM Books WHERE BookID = @BookID " +
                                                        "END", conn);

                addItemCmd.Parameters.AddWithValue("@CartID", cartId);
                addItemCmd.Parameters.AddWithValue("@BookID", bookId);
                addItemCmd.ExecuteNonQuery();
            }
        }
    }
}
