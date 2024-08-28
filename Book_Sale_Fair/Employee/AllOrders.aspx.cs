using Book_Sale_Fair.Employee;
using Book_Sale_Fair.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Book_Sale_Fair.Employee
{
    public partial class AllOrders : System.Web.UI.Page
    {
        private static string ConnectionString = "Data Source=AMANI;Initial Catalog=Sample2;Integrated Security=True;MultipleActiveResultSets=True;";

        protected void Page_Load(object sender, EventArgs e)
        {
            // Check if the user is authenticated and authorized as an employee
            if (AuthHelper.GetLoggedInUserInfo() == null || !(AuthHelper.GetUserRole() == "Employee"))
            {
                // Redirect to the sign-in page
                Response.Redirect("~/account/SignIn.aspx");
                return; // Ensure no further processing occurs
            }

            if (!IsPostBack)
            {
                LoadAllOrders();
            }
        }

        private void LoadAllOrders()
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(@"SELECT OrderID, OrderDate, UserName, Status 
                                                  FROM Orders", conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                gvAllOrders.DataSource = dt;
                gvAllOrders.DataBind();
            }
        }

        protected void gvAllOrders_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Find the buttons in the current row
                Button btnApprove = (Button)e.Row.FindControl("btnApprove");
                Button btnReject = (Button)e.Row.FindControl("btnReject");

                // Get the status of the order from the DataRowView
                string status = DataBinder.Eval(e.Row.DataItem, "Status").ToString();

                // Hide buttons based on the status
                if (status != "Pending")
                {
                    if (btnApprove != null) btnApprove.Visible = false;
                    if (btnReject != null) btnReject.Visible = false;
                }
            }
        }

        private void UpdateOrderStatus(int orderId, string status)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("UPDATE Orders SET Status = @Status WHERE OrderID = @OrderID", conn);
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@OrderID", orderId);
                cmd.ExecuteNonQuery();

                // If the order is accepted, decrease the quantity of the books
                if (status == "Accepted")
                {
                    // Create a new list to store the book updates
                    var bookUpdates = new List<Tuple<int, int>>();

                    // Read all order items into the list first
                    SqlCommand getOrderItemsCmd = new SqlCommand(@"SELECT BookID, Quantity 
                                                          FROM OrderItems 
                                                          WHERE OrderID = @OrderID", conn);
                    getOrderItemsCmd.Parameters.AddWithValue("@OrderID", orderId);

                    using (SqlDataReader reader = getOrderItemsCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int bookId = reader.GetInt32(0);
                            int quantity = reader.GetInt32(1);
                            bookUpdates.Add(new Tuple<int, int>(bookId, quantity));
                        }
                    }

                    // Now update the stock quantities
                    foreach (var update in bookUpdates)
                    {
                        SqlCommand updateStockCmd = new SqlCommand(@"UPDATE Books 
                                                             SET StockQuantity = StockQuantity - @Quantity 
                                                             WHERE BookID = @BookID", conn);
                        updateStockCmd.Parameters.AddWithValue("@Quantity", update.Item2);
                        updateStockCmd.Parameters.AddWithValue("@BookID", update.Item1);
                        updateStockCmd.ExecuteNonQuery();
                    }
                }
            }

            LoadAllOrders();
        }
        protected void gvAllOrders_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ApproveOrder" || e.CommandName == "RejectOrder")
            {
                int orderId = Convert.ToInt32(e.CommandArgument);
                string newStatus = e.CommandName == "ApproveOrder" ? "Accepted" : "Rejected";

                // Update the order status
                UpdateOrderStatus(orderId, newStatus);
            }
            else if (e.CommandName == "ViewOrder")
            {
                int orderId = Convert.ToInt32(e.CommandArgument);
                Response.Redirect($"~/OrderDetails.aspx?OrderID={orderId}");
            }
        }


    }
}
