using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using Book_Sale_Fair.Model;

namespace Book_Sale_Fair
{
    public partial class Cart : System.Web.UI.Page
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
                LoadCartItems();
                UpdateTotalPrice();
            }
        }

        private void LoadCartItems()
        {
            var userName = AuthHelper.GetLoggedInUserInfo().UserName;
            
            
                using (var conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(@"SELECT ci.CartItemID, ci.BookID, b.Title, b.Author, b.Price, ci.Quantity, b.ImageUrl
                                                 FROM CartItems ci
                                                 JOIN Books b ON ci.BookID = b.BookID
                                                 JOIN Cart c ON ci.CartID = c.CartID
                                                 WHERE c.UserName = @UserName", conn);
                    cmd.Parameters.AddWithValue("@UserName", userName);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    gvCart.DataSource = dt;
                    gvCart.DataBind();
                }
            
        }

        protected void btnRemove_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            int cartItemId = Convert.ToInt32(btn.CommandArgument);

            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM CartItems WHERE CartItemID = @CartItemID", conn);
                cmd.Parameters.AddWithValue("@CartItemID", cartItemId);
                cmd.ExecuteNonQuery();
            }

            LoadCartItems();
            UpdateTotalPrice();
        }

        protected void btnMinus_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            int cartItemId = Convert.ToInt32(btn.CommandArgument);

            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                // Get current quantity and stock quantity
                SqlCommand getQtyCmd = new SqlCommand(@"SELECT Quantity, b.StockQuantity
                                                FROM CartItems ci
                                                JOIN Books b ON ci.BookID = b.BookID
                                                WHERE ci.CartItemID = @CartItemID", conn);
                getQtyCmd.Parameters.AddWithValue("@CartItemID", cartItemId);
                SqlDataReader reader = getQtyCmd.ExecuteReader();

                int currentQuantity = 0;
                int stockQuantity = 0;

                if (reader.Read())
                {
                    currentQuantity = reader.GetInt32(0);
                    stockQuantity = reader.GetInt32(1);
                }
                reader.Close();

                if (currentQuantity > 1)
                {
                    if (currentQuantity - 1 <= stockQuantity) // Ensure not to exceed stock
                    {
                        SqlCommand updateQtyCmd = new SqlCommand(@"UPDATE CartItems
                                                          SET Quantity = Quantity - 1
                                                          WHERE CartItemID = @CartItemID", conn);
                        updateQtyCmd.Parameters.AddWithValue("@CartItemID", cartItemId);
                        updateQtyCmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    // Remove the item from the cart if quantity is 1
                    SqlCommand removeCmd = new SqlCommand(@"DELETE FROM CartItems WHERE CartItemID = @CartItemID", conn);
                    removeCmd.Parameters.AddWithValue("@CartItemID", cartItemId);
                    removeCmd.ExecuteNonQuery();
                }
            }

            LoadCartItems();
            UpdateTotalPrice();
        }

        protected void btnPlus_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            int cartItemId = Convert.ToInt32(btn.CommandArgument);

            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                // Get current quantity and stock quantity
                SqlCommand getQtyCmd = new SqlCommand(@"SELECT Quantity, b.StockQuantity
                                                FROM CartItems ci
                                                JOIN Books b ON ci.BookID = b.BookID
                                                WHERE ci.CartItemID = @CartItemID", conn);
                getQtyCmd.Parameters.AddWithValue("@CartItemID", cartItemId);
                SqlDataReader reader = getQtyCmd.ExecuteReader();

                int currentQuantity = 0;
                int stockQuantity = 0;

                if (reader.Read())
                {
                    currentQuantity = reader.GetInt32(0);
                    stockQuantity = reader.GetInt32(1);
                }
                reader.Close();

                if (currentQuantity < stockQuantity) // Ensure not to exceed stock
                {
                    SqlCommand updateQtyCmd = new SqlCommand(@"UPDATE CartItems
                                                      SET Quantity = Quantity + 1
                                                      WHERE CartItemID = @CartItemID", conn);
                    updateQtyCmd.Parameters.AddWithValue("@CartItemID", cartItemId);
                    updateQtyCmd.ExecuteNonQuery();
                }
            }

            LoadCartItems();
            UpdateTotalPrice();
        }


        protected void btnRemoveAll_Click(object sender, EventArgs e)
        {
            var userName = AuthHelper.GetLoggedInUserInfo().UserName;

            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(@"DELETE ci
                                                 FROM CartItems ci
                                                 JOIN Cart c ON ci.CartID = c.CartID
                                                 WHERE c.UserName = @UserName", conn);
                cmd.Parameters.AddWithValue("@UserName", userName);
                cmd.ExecuteNonQuery();
            }

            LoadCartItems();
            UpdateTotalPrice();
        }

        protected void btnCheckout_Click(object sender, EventArgs e)
        {
            var userName = AuthHelper.GetLoggedInUserInfo().UserName;

            // Retrieve selected items from the cart
            DataTable selectedItems = new DataTable();
            foreach (GridViewRow row in gvCart.Rows)
            {
                CheckBox chkSelect = (CheckBox)row.FindControl("chkSelect");
                if (chkSelect != null && chkSelect.Checked)
                {
                    int cartItemId = Convert.ToInt32(((Button)row.FindControl("btnMinus")).CommandArgument);

                    // Retrieve item details
                    using (var conn = new SqlConnection(ConnectionString))
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand(@"SELECT ci.CartItemID, ci.BookID, ci.Quantity, b.Price, b.Title
                                                 FROM CartItems ci
                                                 JOIN Books b ON ci.BookID = b.BookID
                                                 WHERE ci.CartItemID = @CartItemID", conn);
                        cmd.Parameters.AddWithValue("@CartItemID", cartItemId);
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(selectedItems);
                    }
                }
            }

            if (selectedItems.Rows.Count > 0)
            {
                // Create a new order
                int orderId;
                using (var conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    SqlTransaction transaction = conn.BeginTransaction();

                    try
                    {
                        // Create Order
                        SqlCommand createOrderCmd = new SqlCommand(@"INSERT INTO Orders (UserName, OrderDate, Status)
                                                            VALUES (@UserName, GETDATE(), 'Pending');
                                                            SELECT SCOPE_IDENTITY();", conn, transaction);
                        createOrderCmd.Parameters.AddWithValue("@UserName", userName);
                        orderId = Convert.ToInt32(createOrderCmd.ExecuteScalar());

                        // Add Order Items
                        foreach (DataRow row in selectedItems.Rows)
                        {
                            int bookID = Convert.ToInt32(row["BookID"]);
                            int quantity = Convert.ToInt32(row["Quantity"]);

                            SqlCommand addOrderItemCmd = new SqlCommand(@"INSERT INTO OrderItems (OrderID, BookID, Quantity)
                                                                 VALUES (@OrderID, @BookID, @Quantity)", conn, transaction);
                            addOrderItemCmd.Parameters.AddWithValue("@OrderID", orderId);
                            addOrderItemCmd.Parameters.AddWithValue("@BookID", bookID);
                            addOrderItemCmd.Parameters.AddWithValue("@Quantity", quantity);
                            addOrderItemCmd.ExecuteNonQuery();
                        }

                        // Commit transaction
                        transaction.Commit();
                    }
                    catch
                    {
                        // Rollback transaction on error
                        transaction.Rollback();
                        throw;
                    }

                    // Clear Cart
                    SqlCommand clearCartCmd = new SqlCommand(@"DELETE ci
                                                      FROM CartItems ci
                                                      JOIN Cart c ON ci.CartID = c.CartID
                                                      WHERE c.UserName = @UserName", conn);
                    clearCartCmd.Parameters.AddWithValue("@UserName", userName);
                    clearCartCmd.ExecuteNonQuery();
                }

                // Show confirmation alert with order details
                string orderDetails = $"Order ID: {orderId}\nTotal Items: {selectedItems.Rows.Count}";
                ScriptManager.RegisterStartupScript(this, GetType(), "OrderConfirmation", $"alert('Order placed successfully!\\n{orderDetails}');", true);

                // Reload the cart to reflect changes
                LoadCartItems();
                UpdateTotalPrice();
            }
            else
            {
                // No items selected
                ScriptManager.RegisterStartupScript(this, GetType(), "NoItemsSelected", "alert('No items selected for checkout.');", true);
            }
        }

        private void UpdateTotalPrice()
        {
            decimal totalPrice = 0;

            foreach (GridViewRow row in gvCart.Rows)
            {
                CheckBox chkSelect = (CheckBox)row.FindControl("chkSelect");
                if (chkSelect != null && chkSelect.Checked)
                {
                    Label lblPrice = (Label)row.FindControl("lblPrice");
                    Label lblQuantity = (Label)row.FindControl("lblQuantity");

                    if (lblPrice != null && lblQuantity != null)
                    {
                        decimal price;
                        int quantity;

                        if (decimal.TryParse(lblPrice.Text.Replace("$", ""), out price) && int.TryParse(lblQuantity.Text, out quantity))
                        {
                            totalPrice += price * quantity;
                        }
                    }
                }
            }

            lblTotalPrice.Text = $"Total Price: {totalPrice:C}";
        }
    }
}
