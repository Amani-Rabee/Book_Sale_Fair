using Book_Sale_Fair.Model;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace Book_Sale_Fair
{
    public partial class OrderDetails : System.Web.UI.Page
    {
        private static string ConnectionString = "Data Source=AMANI;Initial Catalog=Sample2;Integrated Security=True;";

        protected void Page_Load(object sender, EventArgs e)
        {
            // Check if the user is authenticated
            if (AuthHelper.GetLoggedInUserInfo() == null)
            {
                // Redirect to the sign-in page
                Response.Redirect("~/account/SignIn.aspx");
                return; // Ensure no further processing occurs
            }
            if (!IsPostBack)
            {
                LoadOrderDetails();
                LoadBooksDropdown();
            }
        }

        private void LoadOrderDetails()
        {
            // Assume we get OrderID from query string
            int orderId = Convert.ToInt32(Request.QueryString["OrderID"]);

            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(@"SELECT o.OrderID, o.OrderDate, o.Status, oi.OrderItemID, oi.Quantity, b.BookID, b.Title, b.Author, b.Price
                                                  FROM Orders o
                                                  JOIN OrderItems oi ON o.OrderID = oi.OrderID
                                                  JOIN Books b ON oi.BookID = b.BookID
                                                  WHERE o.OrderID = @OrderID", conn);
                cmd.Parameters.AddWithValue("@OrderID", orderId);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    lblOrderID.Text += dt.Rows[0]["OrderID"].ToString();
                    lblOrderDate.Text += Convert.ToDateTime(dt.Rows[0]["OrderDate"]).ToString("MM/dd/yyyy");
                    lblOrderStatus.Text += dt.Rows[0]["Status"].ToString();

                    gvOrderItems.DataSource = dt;
                    gvOrderItems.DataBind();

                    UpdateTotalPrice(dt);
                }
            }
        }

        private void LoadBooksDropdown()
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT BookID, Title FROM Books", conn);
                SqlDataReader reader = cmd.ExecuteReader();
                ddlBooks.DataSource = reader;
                ddlBooks.DataTextField = "Title";
                ddlBooks.DataValueField = "BookID";
                ddlBooks.DataBind();
            }
        }

        protected void btnIncreaseQuantity_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            int orderItemId = Convert.ToInt32(btn.CommandArgument);

            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("UPDATE OrderItems SET Quantity = Quantity + 1 WHERE OrderItemID = @OrderItemID", conn);
                cmd.Parameters.AddWithValue("@OrderItemID", orderItemId);
                cmd.ExecuteNonQuery();
            }

            LoadOrderDetails();
        }

        protected void btnDecreaseQuantity_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            int orderItemId = Convert.ToInt32(btn.CommandArgument);

            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("UPDATE OrderItems SET Quantity = Quantity - 1 WHERE OrderItemID = @OrderItemID", conn);
                cmd.Parameters.AddWithValue("@OrderItemID", orderItemId);
                cmd.ExecuteNonQuery();
            }

            LoadOrderDetails();
        }

        protected void btnRemoveItem_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            int orderItemId = Convert.ToInt32(btn.CommandArgument);

            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM OrderItems WHERE OrderItemID = @OrderItemID", conn);
                cmd.Parameters.AddWithValue("@OrderItemID", orderItemId);
                cmd.ExecuteNonQuery();
            }

            LoadOrderDetails();
        }

        protected void btnAddBook_Click(object sender, EventArgs e)
        {
            int bookId = Convert.ToInt32(ddlBooks.SelectedValue);
            int orderId = Convert.ToInt32(Request.QueryString["OrderID"]);

            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO OrderItems (OrderID, BookID, Quantity) VALUES (@OrderID, @BookID, 1)", conn);
                cmd.Parameters.AddWithValue("@OrderID", orderId);
                cmd.Parameters.AddWithValue("@BookID", bookId);
                cmd.ExecuteNonQuery();
            }

            LoadOrderDetails();
        }

        private void UpdateTotalPrice(DataTable orderItems)
        {
            decimal totalPrice = 0;

            foreach (DataRow row in orderItems.Rows)
            {
                decimal price = Convert.ToDecimal(row["Price"]);
                int quantity = Convert.ToInt32(row["Quantity"]);
                totalPrice += price * quantity;
            }

            lblTotalPrice.Text = $"Total Price: {totalPrice:C}";
        }

        protected void btnSaveChanges_Click(object sender, EventArgs e)
        {
            
            LoadOrderDetails();
        }
    }
}
