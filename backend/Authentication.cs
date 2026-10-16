using System.Data.SqlClient;

namespace RaceDayPart2Final.backend
{
    public class Authentication
    {
        // Change this to your own server name
        string connectionString = @"Data source=(localdb)\RaceDays;Database=RaceDaysDB;";

        public string message = string.Empty;

        // ---------------- REGISTER ----------------
        public string RegisterUserInfo(string fullName, string email, string phone, string passwordHashed, string userRole)
        {
            try
            {
                using (SqlConnection connect = new SqlConnection(connectionString))
                {
                    // 1. Insert into Users
                    string query = "INSERT INTO Users (FullName, EmailAddress, PhoneNumber, Password, UserRole, IsActive, EmailConfirmed) " +
                                   "VALUES('" + fullName + "','" + email + "','" + phone + "','" + passwordHashed + "','" + userRole + "',1,1)";

                    connect.Open();
                    SqlCommand run_query = new SqlCommand(query, connect);
                    int result = run_query.ExecuteNonQuery();

                    if (result > 0)
                    {
                        message = "Thank you " + fullName + ", you have been registered successfully as an " + userRole;
                    }
                    else
                    {
                        message = "Registration failed.";
                        return message;
                    }

                    // 2. Get the UserID we just inserted
                    string getUserQuery = "SELECT UserID FROM Users WHERE EmailAddress='" + email + "'";
                    SqlCommand getUserCmd = new SqlCommand(getUserQuery, connect);
                    int newUserId = Convert.ToInt32(getUserCmd.ExecuteScalar());

                    // 3. Insert into the correct child table based on role
                    if (userRole == "Organiser")
                    {
                        string query2 = "INSERT INTO Organisers (UserID, ContactNumber) VALUES(" + newUserId + ",'" + phone + "')";
                        SqlCommand storeId = new SqlCommand(query2, connect);
                        storeId.ExecuteNonQuery();
                    }
                    else if (userRole == "Participant")
                    {
                        // We don't have DOB at registration in this simple version,
                        // so use a default for now (user can update profile later)
                        string query3 = "INSERT INTO Participants (UserID, DateOfBirth, ContactNumber) VALUES(" + newUserId + ", '2000-01-01', '" + phone + "')";
                        SqlCommand store_Id = new SqlCommand(query3, connect);
                        store_Id.ExecuteNonQuery();
                    }

                    connect.Close();
                }
            }
            catch (Exception error)
            {
                message = error.Message;
            }

            return message;
        }

        // ---------------- LOGIN ----------------
        public string LogInInfo(string email, string passwordPlain)
        {
            string msg = string.Empty;

            try
            {
                using (SqlConnection connector = new SqlConnection(connectionString))
                {
                    connector.Open();
                    string query = "SELECT Password, UserRole FROM Users WHERE EmailAddress='" + email + "'";
                    SqlCommand run_query = new SqlCommand(query, connector);
                    SqlDataReader read = run_query.ExecuteReader();

                    bool found = false;
                    while (read.Read())
                    {
                        found = true;
                        string storedHash = read["Password"].ToString();
                        string role = read["UserRole"].ToString();

                        // verify typed password against stored hash from DB
                        if (BCrypt.Net.BCrypt.Verify(passwordPlain, storedHash))
                        {
                            msg = "Logged in successfully... Role: " + role;
                        }
                        else
                        {
                            msg = "Login Unsuccessful... Incorrect password";
                        }
                    }

                    if (!found)
                    {
                        msg = "Login Unsuccessful... User not found";
                    }

                    connector.Close();
                }
            }
            catch (Exception error)
            {
                msg = error.Message;
            }
            return msg;
        }
    }
}
