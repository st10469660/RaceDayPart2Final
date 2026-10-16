using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDayPart2Final.models;
using System.Data.SqlClient;

namespace RaceDayPart2Final.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        string connectionString = @"Data source=(localdb)\RaceDays;Database=RaceDaysDB;";

        // GET /api/users/profile/{userId}
        [HttpGet("profile/{userId}")]
        public UserModel GetProfile(int userId)
        {
            UserModel user = new UserModel();
            try
            {
                using (SqlConnection connect = new SqlConnection(connectionString))
                {
                    string query = "SELECT UserID, FullName, EmailAddress, PhoneNumber, UserRole, ProfilePictureUrl " +
                                   "FROM Users WHERE UserID=" + userId;
                    connect.Open();
                    SqlCommand cmd = new SqlCommand(query, connect);
                    SqlDataReader read = cmd.ExecuteReader();

                    while (read.Read())
                    {
                        user.UserID = Convert.ToInt32(read["UserID"]);
                        user.FullName = read["FullName"].ToString();
                        user.EmailAddress = read["EmailAddress"].ToString();
                        user.PhoneNumber = read["PhoneNumber"].ToString();
                        user.UserRole = read["UserRole"].ToString();
                        user.ProfilePictureUrl = read["ProfilePictureUrl"] == DBNull.Value ? null : read["ProfilePictureUrl"].ToString();
                    }
                    connect.Close();
                }
            }
            catch (Exception error) { /* swallow per your style */ }
            return user;
        }

        // PUT /api/users/profile/{userId}
        [HttpPut("profile/{userId}")]
        public string UpdateProfile(int userId, string fullName, string phone, string email)
        {
            string message = string.Empty;
            try
            {
                using (SqlConnection connect = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Users SET FullName='" + fullName + "', PhoneNumber='" + phone +
                                   "', EmailAddress='" + email + "', UpdatedAt=GETDATE() WHERE UserID=" + userId;
                    connect.Open();
                    SqlCommand cmd = new SqlCommand(query, connect);
                    int result = cmd.ExecuteNonQuery();

                    message = result > 0 ? "Profile updated successfully" : "Update failed";
                    connect.Close();
                }
            }
            catch (Exception error) { message = error.Message; }
            return message;
        }

        // PUT /api/users/password/{userId}
        [HttpPut("password/{userId}")]
        public string ChangePassword(int userId, string currentPassword, string newPassword)
        {
            string message = string.Empty;
            try
            {
                using (SqlConnection connect = new SqlConnection(connectionString))
                {
                    connect.Open();
                    string getQuery = "SELECT Password FROM Users WHERE UserID=" + userId;
                    SqlCommand getCmd = new SqlCommand(getQuery, connect);
                    string storedHash = getCmd.ExecuteScalar().ToString();

                    if (!BCrypt.Net.BCrypt.Verify(currentPassword, storedHash))
                    {
                        return "Current password incorrect";
                    }

                    string newHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                    string updateQuery = "UPDATE Users SET Password='" + newHash + "', UpdatedAt=GETDATE() WHERE UserID=" + userId;
                    SqlCommand updCmd = new SqlCommand(updateQuery, connect);
                    updCmd.ExecuteNonQuery();

                    message = "Password updated";
                    connect.Close();
                }
            }
            catch (Exception error) { message = error.Message; }
            return message;
        }
    }
}