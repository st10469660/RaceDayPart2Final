using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDayPart2Final.models;
using System.Data.SqlClient;

namespace RaceDayPart2Final.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        string connectionString = @"Data source=(localdb)\RaceDays;Database=RaceDaysDB;";

        // GET /api/events
        [HttpGet]
        public List<EventModel> GetAllEvents()
        {
            List<EventModel> events = new List<EventModel>();
            try
            {
                using (SqlConnection connect = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Events WHERE IsActive=1";
                    connect.Open();
                    SqlCommand cmd = new SqlCommand(query, connect);
                    SqlDataReader read = cmd.ExecuteReader();
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
            catch (Exception error) { }
            return events;
        }

        // GET /api/events/{id}
        [HttpGet("{id}")]
        public EventModel GetEventById(int id)
        {
            EventModel ev = new EventModel();
            try
            {
                using (SqlConnection connect = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Events WHERE EventID=" + id;
                    connect.Open();
                    SqlCommand cmd = new SqlCommand(query, connect);
                    SqlDataReader read = cmd.ExecuteReader();
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
            catch (Exception error) { }
            return ev;
        }

        // POST /api/events (Organiser only)
        [HttpPost]
        public string CreateEvent(int organiserUserId, string eventName, string description, DateTime date,
                                  string location, decimal distance, string eventType,
                                  int maxParticipants, decimal entryFee)
        {
            string message = string.Empty;
            try
            {
                using (SqlConnection connect = new SqlConnection(connectionString))
                {
                    connect.Open();

                    // find OrganiserID from UserID
                    string orgQuery = "SELECT OrganiserID FROM Organisers WHERE UserID=" + organiserUserId;
                    SqlCommand orgCmd = new SqlCommand(orgQuery, connect);
                    object orgIdObj = orgCmd.ExecuteScalar();
                    if (orgIdObj == null) return "403: Not an organiser";
                    int organiserId = Convert.ToInt32(orgIdObj);

                    string query = "INSERT INTO Events (OrganiserID, EventName, Description, Date, Location, Distance, EventType, MaxParticipants, EntryFee, IsActive, IsPublished) " +
                                   "VALUES(" + organiserId + ",'" + eventName + "','" + description + "','" + date.ToString("yyyy-MM-dd HH:mm:ss") +
                                   "','" + location + "'," + distance + ",'" + eventType + "'," + maxParticipants + "," + entryFee + ",1,0)";
                    SqlCommand cmd = new SqlCommand(query, connect);
                    int result = cmd.ExecuteNonQuery();
                    message = result > 0 ? "Event created successfully" : "Failed to create event";
                    connect.Close();
                }
            }
            catch (Exception error) { message = error.Message; }
            return message;
        }

        // PUT /api/events/{id}
        [HttpPut("{id}")]
        public string UpdateEvent(int id, int organiserUserId, string eventName, string description, DateTime date,
                                  string location, decimal distance, string eventType, int maxParticipants, decimal entryFee)
        {
            string message = string.Empty;
            try
            {
                using (SqlConnection connect = new SqlConnection(connectionString))
                {
                    connect.Open();

                    string ownerQuery = "SELECT o.UserID FROM Events e " +
                                        "INNER JOIN Organisers o ON e.OrganiserID=o.OrganiserID WHERE e.EventID=" + id;
                    SqlCommand ownerCmd = new SqlCommand(ownerQuery, connect);
                    object ownerObj = ownerCmd.ExecuteScalar();
                    if (ownerObj == null || Convert.ToInt32(ownerObj) != organiserUserId)
                        return "403: Not event owner";

                    string query = "UPDATE Events SET EventName='" + eventName + "', Description='" + description +
                                   "', Date='" + date.ToString("yyyy-MM-dd HH:mm:ss") + "', Location='" + location +
                                   "', Distance=" + distance + ", EventType='" + eventType +
                                   "', MaxParticipants=" + maxParticipants + ", EntryFee=" + entryFee +
                                   ", UpdatedAt=GETDATE() WHERE EventID=" + id;
                    SqlCommand cmd = new SqlCommand(query, connect);
                    int result = cmd.ExecuteNonQuery();
                    message = result > 0 ? "Event updated successfully" : "Update failed";
                    connect.Close();
                }
            }
            catch (Exception error) { message = error.Message; }
            return message;
        }

        // DELETE /api/events/{id} (soft delete)
        [HttpDelete("{id}")]
        public string DeleteEvent(int id, int organiserUserId)
        {
            string message = string.Empty;
            try
            {
                using (SqlConnection connect = new SqlConnection(connectionString))
                {
                    connect.Open();

                    string ownerQuery = "SELECT o.UserID FROM Events e " +
                                        "INNER JOIN Organisers o ON e.OrganiserID=o.OrganiserID WHERE e.EventID=" + id;
                    SqlCommand ownerCmd = new SqlCommand(ownerQuery, connect);
                    object ownerObj = ownerCmd.ExecuteScalar();
                    if (ownerObj == null || Convert.ToInt32(ownerObj) != organiserUserId)
                        return "403: Not event owner";

                    string query = "UPDATE Events SET IsActive=0, UpdatedAt=GETDATE() WHERE EventID=" + id;
                    SqlCommand cmd = new SqlCommand(query, connect);
                    int result = cmd.ExecuteNonQuery();
                    message = result > 0 ? "Event deleted successfully" : "Delete failed";
                    connect.Close();
                }
            }
            catch (Exception error) { message = error.Message; }
            return message;
        }
    }
}

