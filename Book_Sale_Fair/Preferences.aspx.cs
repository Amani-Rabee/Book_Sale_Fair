using Book_Sale_Fair.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Services;
using System.Web.UI;

public partial class Preferences : System.Web.UI.Page
{
    private string _username;

    protected void Page_Load(object sender, EventArgs e)
    {
        // Retrieve the username from the query string
        _username = Request.QueryString["username"];

        if (string.IsNullOrEmpty(_username))
        {
            // Handle the case where the username is not provided
            Response.Redirect("SignIn.aspx");
            return;
        }

        if (!IsPostBack)
        {
            
            LoadCategories();
            LoadPreferences();
        }
    }

    protected void LoadCategories()
    {
        string connectionString = "Data Source=AMANI;Initial Catalog=Sample2;Integrated Security=True;";
        string query = "SELECT CategoryID, CategoryName FROM Categories";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            List<Category> categories = new List<Category>();

            while (reader.Read())
            {
                categories.Add(new Category
                {
                    CategoryID = Convert.ToInt32(reader["CategoryID"]),
                    CategoryName = reader["CategoryName"].ToString()
                });
            }

        //    rptCategories.DataSource = categories;
        //    rptCategories.DataBind();
        }
    }


    private void LoadPreferences()
    {
        List<int> selectedCategories = GetUserSelectedCategoriesFromDatabase();

        foreach (int categoryId in selectedCategories)
        {
            string script = String.Format(
                "$(document).ready(function() {{ $('.card[data-category-id=\"{0}\"]').addClass('selected'); }});",
                categoryId
            );

            ScriptManager.RegisterStartupScript(
                this,
                this.GetType(),
                String.Format("SelectCategory{0}", categoryId),
                script,
                true
            );
        }
    }

    private List<int> GetUserSelectedCategoriesFromDatabase()
    {
        List<int> selectedCategories = new List<int>();

        string connectionString = "Data Source=AMANI;Initial Catalog=Sample2;Integrated Security=True;";
        string query = "SELECT CategoryID FROM UserPreferences WHERE UserName = @UserName";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserName", _username); // Use the username from query string

            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                selectedCategories.Add(Convert.ToInt32(reader["CategoryID"]));
            }
        }

        return selectedCategories;
    }

    [WebMethod]
    public static bool SavePreferences(List<int> selectedCategories, string username)
    {


        if (selectedCategories == null || !selectedCategories.Any() || string.IsNullOrEmpty(username))
        {
            return false;
        }

        try
        {
            string connectionString = "Data Source=AMANI;Initial Catalog=Sample2;Integrated Security=True;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Start a transaction
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Delete existing preferences
                        string deleteQuery = "DELETE FROM UserPreferences WHERE UserName = @UserName";
                        using (SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection, transaction))
                        {
                            deleteCommand.Parameters.AddWithValue("@UserName", username);
                            int rowsAffected = deleteCommand.ExecuteNonQuery();
                        }

                        // Insert new preferences
                        string insertQuery = "INSERT INTO UserPreferences (UserName, CategoryID) VALUES (@UserName, @CategoryID)";
                        foreach (int categoryId in selectedCategories)
                        {
                            using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection, transaction))
                            {
                                insertCommand.Parameters.AddWithValue("@UserName", username);
                                insertCommand.Parameters.AddWithValue("@CategoryID", categoryId);
                                int rowsInserted = insertCommand.ExecuteNonQuery();
                            }
                        }

                        // Update HasSetPreferences flag
                        string updateQuery = "UPDATE Users SET HasSetPreferences = 1 WHERE UserName = @UserName";
                        using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection, transaction))
                        {
                            updateCommand.Parameters.AddWithValue("@UserName", username);
                            int rowsUpdated = updateCommand.ExecuteNonQuery();
                        }

                        // Commit the transaction
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            // Log the exception
            System.Diagnostics.Debug.WriteLine("Exception caught in SavePreferences");
            System.Diagnostics.Debug.WriteLine("Error: {0}", ex.Message);
            System.Diagnostics.Debug.WriteLine("StackTrace: {0}", ex.StackTrace);
            return false;
        }

    }
    public string Username
    {
        get { return _username; }
    }

    class Category
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
    }
}