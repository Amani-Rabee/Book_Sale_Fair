using Book_Sale_Fair.Model;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace Book_Sale_Fair
{
    public partial class Orders : System.Web.UI.Page
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
                LoadOrders();
            }
        }

        private void LoadOrders()
        {
            var userName = AuthHelper.GetLoggedInUserInfo().UserName;

            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(@"SELECT OrderID, OrderDate, Status 
                                                  FROM Orders 
                                                  WHERE UserName = @UserName", conn);
                cmd.Parameters.AddWithValue("@UserName", userName);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                gvOrders.DataSource = dt;
                gvOrders.DataBind();
            }
        }

        protected void gvOrders_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ViewOrder")
            {
                int orderId = Convert.ToInt32(e.CommandArgument);
                Response.Redirect($"OrderDetails.aspx?OrderID={orderId}");
            }
        }
    }
}
