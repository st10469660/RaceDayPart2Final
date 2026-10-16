using RaceDayPart2Final.models;
using System.Data.SqlClient;

namespace RaceDayPart2Final.backend
{
    public class Event
    {
        // change to your own server name
        string connectionString = "Server=localhost;Database=RaceDaysDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public string message = string.Empty;

        // ---------------- GET ALL EVENTS ----------------
        public List<EventModel> GetAllEvents()
        {
            List<EventModel> events = new List<EventModel>();

            try
            {
                using (SqlConnection connect = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Events WHERE IsActive=1";

                    connect.Open();

                    SqlCommand run_query = new SqlCommand(query, connect);
                    SqlDataReader read = run_query.ExecuteReader();

                    while (read.Read())
                    {
                        events.Add(new EventModel
                        {
                            EventID = Convert.ToInt32(read["EventID"]),
                            EventName = read["EventName"].ToString(),
                            Description = read["Description"] == DBNull.Value ? "" : read["Description"].ToString(),
                            Date = Convert.ToDateTime(read["Date"]),
                            Location = read["Location"].ToString(),
                            Distance = Convert.ToDecimal(read["Distance"]),
                            EventType = read["EventType"].ToString(),
                            MaxParticipants = read["MaxParticipants"] == DBNull.Value ? (int?)null : Convert.ToInt32(read["MaxParticipants"]),
                            EntryFee = read["EntryFee"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(read["EntryFee"]),
                            IsActive = Convert.ToBoolean(read["IsActive"])
                        });
                    }

                    connect.Close();
                }
            }
            catch (Exception error)
            {
                message = error.Message;
            }

            return events;
        }

        // ---------------- GET EVENT BY ID ----------------
        public EventModel GetEventById(int eventId)
        {
            EventModel ev = new EventModel();

            try
            {
                using (SqlConnection connect = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Events WHERE EventID=" + eventId;

                    connect.Open();

                    SqlCommand run_query = new SqlCommand(query, connect);
                    SqlDataReader read = run_query.ExecuteReader();

                    while (read.Read())
                    {
                        ev.EventID = Convert.ToInt32(read["EventID"]);
                        ev.EventName = read["EventName"].ToString();
                        ev.Description = read["Description"] == DBNull.Value ? "" : read["Description"].ToString();
                        ev.Date = Convert.ToDateTime(read["Date"]);
                        ev.Location = read["Location"].ToString();
                        ev.Distance = Convert.ToDecimal(read["Distance"]);
                        ev.EventType = read["EventType"].ToString();
                        ev.MaxParticipants = read["MaxParticipants"] == DBNull.Value ? (int?)null : Convert.ToInt32(read["MaxParticipants"]);
                        ev.EntryFee = read["EntryFee"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(read["EntryFee"]);
                        ev.IsActive = Convert.ToBoolean(read["IsActive"]);
                    }

                    connect.Close();
                }
            }
            catch (Exception error)
            {
                message = error.Message;
            }

            return ev;
        }

        // ---------------- CREATE EVENT ----------------
        public string CreateEvent(int organiserUserId, string eventName, string description, DateTime date,
                                  string location, decimal distance, string eventType,
                                  int maxParticipants, decimal entryFee)
        {
            try
            {
                using (SqlConnection connect = new SqlConnection(connectionString))
                {
                    connect.Open();

                    // find the OrganiserID from the UserID
                    string orgQuery = "SELECT OrganiserID FROM Organisers WHERE UserID=" + organiserUserId;
                    SqlCommand orgCmd = new SqlCommand(orgQuery, connect);
                    object orgIdObj = orgCmd.ExecuteScalar();

                    if (orgIdObj == null)
                    {
                        return "403: Not an organiser";
                    }

                    int organiserId = Convert.ToInt32(orgIdObj);

                    string query = "INSERT INTO Events (OrganiserID, EventName, Description, Date, Location, Distance, EventType, MaxParticipants, EntryFee, IsActive, IsPublished) " +
                                   "VALUES(" + organiserId + ",'" + eventName + "','" + description + "','" + date.ToString("yyyy-MM-dd HH:mm:ss") +
                                   "','" + location + "'," + distance + ",'" + eventType + "'," + maxParticipants + "," + entryFee + ",1,0)";

                    SqlCommand run_query = new SqlCommand(query, connect);
                    int result = run_query.ExecuteNonQuery();

                    if (result > 0)
                    {
                        message = "Event created successfully";
                    }
                    else
                    {
                        message = "Failed to create event";
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

        // ---------------- UPDATE EVENT ----------------
        public string UpdateEvent(int eventId, int organiserUserId, string eventName, string description, DateTime date,
                                  string location, decimal distance, string eventType,
                                  int maxParticipants, decimal entryFee)
        {
            try
            {
                using (SqlConnection connect = new SqlConnection(connectionString))
                {
                    connect.Open();

                    // check ownership
                    string ownerQuery = "SELECT o.UserID FROM Events e " +
                                        "INNER JOIN Organisers o ON e.OrganiserID=o.OrganiserID WHERE e.EventID=" + eventId;
                    SqlCommand ownerCmd = new SqlCommand(ownerQuery, connect);
                    object ownerObj = ownerCmd.ExecuteScalar();

                    if (ownerObj == null || Convert.ToInt32(ownerObj) != organiserUserId)
                    {
                        return "403: Not event owner";
                    }

                    string query = "UPDATE Events SET EventName='" + eventName + "', Description='" + description +
                                   "', Date='" + date.ToString("yyyy-MM-dd HH:mm:ss") + "', Location='" + location +
                                   "', Distance=" + distance + ", EventType='" + eventType +
                                   "', MaxParticipants=" + maxParticipants + ", EntryFee=" + entryFee +
                                   ", UpdatedAt=GETDATE() WHERE EventID=" + eventId;

                    SqlCommand run_query = new SqlCommand(query, connect);
                    int result = run_query.ExecuteNonQuery();

                    if (result > 0)
                    {
                        message = "Event updated successfully";
                    }
                    else
                    {
                        message = "Update failed";
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

        // ---------------- DELETE EVENT (soft delete) ----------------
        public string DeleteEvent(int eventId, int organiserUserId)
        {
            try
            {
                using (SqlConnection connect = new SqlConnection(connectionString))
                {
                    connect.Open();

                    // check ownership
                    string ownerQuery = "SELECT o.UserID FROM Events e " +
                                        "INNER JOIN Organisers o ON e.OrganiserID=o.OrganiserID WHERE e.EventID=" + eventId;
                    SqlCommand ownerCmd = new SqlCommand(ownerQuery, connect);
                    object ownerObj = ownerCmd.ExecuteScalar();

                    if (ownerObj == null || Convert.ToInt32(ownerObj) != organiserUserId)
                    {
                        return "403: Not event owner";
                    }

                    string query = "UPDATE Events SET IsActive=0, UpdatedAt=GETDATE() WHERE EventID=" + eventId;
                    SqlCommand run_query = new SqlCommand(query, connect);
                    int result = run_query.ExecuteNonQuery();

                    if (result > 0)
                    {
                        message = "Event deleted successfully";
                    }
                    else
                    {
                        message = "Delete failed";
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
    }
}


