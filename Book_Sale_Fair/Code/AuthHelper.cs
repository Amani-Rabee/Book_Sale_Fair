using System;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Book_Sale_Fair.Model
{
    public static class AuthHelper
    {
        private static readonly int MaxInvalidAttempts = 5;
        private static readonly int LockoutDurationMinutes = 15;
        public static string ConnectionString = "Data Source=AMANI;Initial Catalog=Sample2;Integrated Security=True;";
        //
        //sign in related methods
        //
        public static bool SignIn(string userName, string password)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                var hashPass = HashPassword(password);
                System.Diagnostics.Debug.WriteLine($"Hash: {hashPass}");

                var query = @"
SELECT UserName, FirstName, LastName, Email, InvalidLoginAttempts, LockoutEndDate, PasswordHash, Role
FROM Users
WHERE UserName = @UserName";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserName", userName);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int invalidAttempts = reader.GetInt32(reader.GetOrdinal("InvalidLoginAttempts"));
                            DateTime? lockoutEndDate = reader.IsDBNull(reader.GetOrdinal("LockoutEndDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("LockoutEndDate"));
                            string userRole = reader["Role"].ToString();

                            // Check if the account is locked
                            if (lockoutEndDate.HasValue && lockoutEndDate.Value > DateTime.Now)
                            {
                                return false; // Account is still locked
                            }

                            // Check if password is correct
                            if (hashPass == reader["PasswordHash"].ToString())
                            {
                                // Reset invalid login attempts and lockout
                                ResetInvalidLoginAttempts(userName);

                                // Check if HttpContext is available
                                if (HttpContext.Current != null)
                                {
                                    HttpContext.Current.Session["User"] = new ApplicationUser
                                    {
                                        UserName = reader["UserName"].ToString(),
                                        FirstName = reader["FirstName"].ToString(),
                                        LastName = reader["LastName"].ToString(),
                                        Email = reader["Email"].ToString(),
                                        Role = userRole // Store the role here
                                    };

                                    HttpContext.Current.Session["UserRole"] = userRole; // Store the role separately

                                    return true;
                                }
                                else
                                {
                                    // Handle the null context scenario
                                    throw new InvalidOperationException("HttpContext is not available.");
                                }
                            }
                            else
                            {
                                // Increment invalid login attempts
                                IncrementInvalidLoginAttempts(userName);
                                return false;
                            }
                        }
                    }
                }
            }
            return false;
        }

        private static void IncrementInvalidLoginAttempts(string userName)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                var query = @"
                UPDATE Users
                SET InvalidLoginAttempts = InvalidLoginAttempts + 1,
                    LockoutEndDate = CASE
                        WHEN InvalidLoginAttempts >= @MaxInvalidAttempts
                        THEN DATEADD(MINUTE, @LockoutDuration, GETDATE())
                        ELSE LockoutEndDate
                    END
                WHERE UserName = @UserName";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserName", userName);
                    command.Parameters.AddWithValue("@MaxInvalidAttempts", MaxInvalidAttempts);
                    command.Parameters.AddWithValue("@LockoutDuration", LockoutDurationMinutes);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        private static void ResetInvalidLoginAttempts(string userName)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                var query = @"
                UPDATE Users
                SET InvalidLoginAttempts = 0,
                    LockoutEndDate = NULL
                WHERE UserName = @UserName";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserName", userName);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        public static void GeneratePasswordResetToken(string email)
        {
            var token = Guid.NewGuid().ToString();
            var expiry = DateTime.Now.AddHours(1);

            using (var connection = new SqlConnection(ConnectionString))
            {
                var query = @"
                UPDATE Users
                SET PasswordResetToken = @Token, PasswordResetTokenExpiry = @Expiry
                WHERE Email = @Email";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Token", token);
                    command.Parameters.AddWithValue("@Expiry", expiry);
                    command.Parameters.AddWithValue("@Email", email);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }

            SendPasswordResetEmail(email, token);
        }
        public static bool ValidatePasswordResetToken(string email, string token)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                var query = @"
                SELECT COUNT(*)
                FROM Users
                WHERE Email = @Email
                AND PasswordResetToken = @Token
                AND PasswordResetTokenExpiry > GETDATE()";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Token", token);

                    connection.Open();
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }
        public static bool ResetPassword(string email, string token, string newPassword)
        {
            if (!ValidatePasswordResetToken(email, token))
                return false;

            using (var connection = new SqlConnection(ConnectionString))
            {
                var query = @"
                UPDATE Users
                SET PasswordHash = @PasswordHash,
                    PasswordResetToken = NULL,
                    PasswordResetTokenExpiry = NULL
                WHERE Email = @Email";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@PasswordHash", HashPassword(newPassword));

                    connection.Open();
                    var rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }
        private static void SendPasswordResetEmail(string email, string token)
        {
            var resetLink = $"https://yourwebsite.com/ResetPassword.aspx?email={HttpUtility.UrlEncode(email)}&token={HttpUtility.UrlEncode(token)}";

            var mail = new MailMessage
            {
                From = new MailAddress("amani.rabee2018@gmail.com", "Book Sale Fair", Encoding.UTF8),
                Subject = "Password Reset",
                SubjectEncoding = Encoding.UTF8,
                Body = $"Please reset your password using the following link: <a href='{resetLink}'>Reset Password</a>",
                BodyEncoding = Encoding.UTF8,
                IsBodyHtml = true,
                Priority = MailPriority.High,
            };

            mail.To.Add(email);

            var smtpClient = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                Credentials = new NetworkCredential("amani.rabee2018@gmail.com", ""),
            };

            try
            {
                smtpClient.Send(mail);
                System.Diagnostics.Debug.WriteLine("Password reset email sent successfully.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error sending email: {ex.Message}");
                throw;
            }
        }
        public static bool IsAccountLocked(string userName)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                string query = @"
            SELECT LockoutEndDate
            FROM Users
            WHERE UserName = @UserName";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserName", userName);
                    connection.Open();
                    var lockoutEndDate = command.ExecuteScalar() as DateTime?;

                    if (lockoutEndDate.HasValue && lockoutEndDate.Value > DateTime.Now)
                    {
                        return true; // Account is locked
                    }
                }
            }
            return false; // Account is not locked
        }
        //
        //register related methods
        //
        public static bool RegisterUser(string userName, string firstName, string lastName, string email, string password)
        {
            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    var checkQuery = "SELECT COUNT(*) FROM Users WHERE UserName = @UserName OR Email = @Email";
                    using (var checkCommand = new SqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = userName;
                        checkCommand.Parameters.Add("@Email", SqlDbType.NVarChar).Value = email;

                        var userCount = (int)checkCommand.ExecuteScalar();
                        if (userCount > 0)
                        {
                            return false; // User already exists
                        }
                    }

                    var insertQuery = "INSERT INTO Users (UserName, FirstName, LastName, Email, PasswordHash) VALUES (@UserName, @FirstName, @LastName, @Email, @PasswordHash)";
                    using (var insertCommand = new SqlCommand(insertQuery, connection))
                    {
                        insertCommand.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = userName;
                        insertCommand.Parameters.Add("@FirstName", SqlDbType.NVarChar).Value = firstName;
                        insertCommand.Parameters.Add("@LastName", SqlDbType.NVarChar).Value = lastName;
                        insertCommand.Parameters.Add("@Email", SqlDbType.NVarChar).Value = email;
                        insertCommand.Parameters.Add("@PasswordHash", SqlDbType.NVarChar).Value = HashPassword(password);

                        var rowsAffected = insertCommand.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error: " + ex.Message);
                return false;
            }
        }
        public static void SaveVerificationToken(string email, string token)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                var query = "UPDATE Users SET VerificationToken = @Token WHERE Email = @Email";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Token", token);
                    command.Parameters.AddWithValue("@Email", email);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        public static void SendVerificationEmail(string email, string token)
        {
            var mail = new MailMessage
            {
                From = new MailAddress("amani.rabee2018@gmail.com", "Book Sale Fair", Encoding.UTF8),
                Subject = "Email Verification",
                SubjectEncoding = Encoding.UTF8,
                Body = $"Please verify your email using this code: {token}",
                BodyEncoding = Encoding.UTF8,
                IsBodyHtml = true,
                Priority = MailPriority.High,
            };

            mail.To.Add(email);

            var smtpClient = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                Credentials = new NetworkCredential("amani.rabee2018@gmail.com", ""),
            };

            try
            {
                smtpClient.Send(mail);
                System.Diagnostics.Debug.WriteLine("Verification email sent successfully.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error sending email: {ex.Message}");
                throw;
            }
        }
        public static bool VerifyCode(string email, string code)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                var query = "SELECT COUNT(*) FROM Users WHERE Email = @Email AND VerificationToken = @Code";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Code", code);
                    connection.Open();
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }
        public static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
        public static bool IsRegistered(string userNameOrEmail)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                var query = "SELECT COUNT(*) FROM Users WHERE UserName = @UserNameOrEmail OR Email = @UserNameOrEmail";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserNameOrEmail", userNameOrEmail);

                    connection.Open();
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        public static void SignOut()
        {
            HttpContext.Current.Session["User"] = null;
        }
        public static bool IsAuthenticated()
        {
            return GetLoggedInUserInfo() != null;
        }
        public static ApplicationUser GetLoggedInUserInfo()
        {
            var user = HttpContext.Current.Session["User"] as ApplicationUser;
            System.Diagnostics.Debug.WriteLine(user != null ? $"User found: {user.UserName}" : "User not found in session.");
            return user;
        }

        public static bool HasSetPreferences(string userName)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                var query = "SELECT HasSetPreferences FROM Users WHERE UserName = @UserName";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserName", userName);

                    connection.Open();
                    var result = command.ExecuteScalar();
                    return result != null && Convert.ToBoolean(result);
                }
            }
        }

        public static string GetUserRole()
        {
            // Assuming the user role is stored in the session
            return HttpContext.Current.Session["UserRole"] as string;
        }





    }

    public class ApplicationUser
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string AvatarUrl { get; set; }
        public string Role { get; set; }
    }
}
