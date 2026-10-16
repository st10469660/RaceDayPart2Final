using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace RaceDayPart2Final.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrolmentsController : ControllerBase
    {
        string connectionString = @"Data source=(localdb)\RaceDays;Database=RaceDaysDB;";

        // POST /api/enrolments/event/{eventId}
        // participantUserId = the UserID, categoryId = the chosen category
        [HttpPost("event/{eventId}")]
        public string EnrolParticipant(int eventId, int participantUserId, int categoryId)
        {
            string message = string.Empty;
            try
            {
                using (SqlConnection connect = new SqlConnection(connectionString))
                {
                    connect.Open();

                    // find ParticipantID
                    string pQuery = "SELECT ParticipantID FROM Participants WHERE UserID=" + participantUserId;
                    SqlCommand pCmd = new SqlCommand(pQuery, connect);
                    object pObj = pCmd.ExecuteScalar();
                    if (pObj == null) return "403: Not a participant";
                    int participantId = Convert.ToInt32(pObj);

                    // check not already enrolled
                    string dupQuery = "SELECT COUNT(*) FROM Enrolments WHERE EventID=" + eventId + " AND ParticipantID=" + participantId;
                    SqlCommand dupCmd = new SqlCommand(dupQuery, connect);
                    int dup = Convert.ToInt32(dupCmd.ExecuteScalar());
                    if (dup > 0) return "409: Already enrolled";

                    string query = "INSERT INTO Enrolments (EventID, ParticipantID, CategoryID, Status, EntryFeePaid) " +
                                   "VALUES(" + eventId + "," + participantId + "," + categoryId + ",'Pending',0)";
                    SqlCommand cmd = new SqlCommand(query, connect);
                    int result = cmd.ExecuteNonQuery();
                    message = result > 0 ? "Enrolment submitted" : "Enrolment failed";
                    connect.Close();
                }
            }
            catch (Exception error) { message = error.Message; }
            return message;
        }

        // GET /api/enrolments/event/{eventId} (organiser view)
        [HttpGet("event/{eventId}")]
        public List<string> GetEnrolmentsForEvent(int eventId)
        {
            List<string> list = new List<string>();
            try
            {
                using (SqlConnection connect = new SqlConnection(connectionString))
                {
                    string query = "SELECT e.EnrolmentID, u.FullName, e.Status, e.EntryFeePaid " +
                                   "FROM Enrolments e " +
                                   "INNER JOIN Participants p ON e.ParticipantID=p.ParticipantID " +
                                   "INNER JOIN Users u ON p.UserID=u.UserID " +
                                   "WHERE e.EventID=" + eventId;
                    connect.Open();
                    SqlCommand cmd = new SqlCommand(query, connect);
                    SqlDataReader read = cmd.ExecuteReader();
                    while (read.Read())
                    {
                        list.Add("EnrolmentID: " + read["EnrolmentID"] + " | " + read["FullName"] +
                                 " | Status: " + read["Status"] + " | Paid: " + read["EntryFeePaid"]);
                    }
                    connect.Close();
                }
            }
            catch (Exception error) { }
            return list;
        }

        // GET /api/enrolments/participant/{participantUserId}
        [HttpGet("participant/{participantUserId}")]
        public List<string> GetEnrolmentsForParticipant(int participantUserId)
        {
            List<string> list = new List<string>();
            try
            {
                using (SqlConnection connect = new SqlConnection(connectionString))
                {
                    string query = "SELECT e.EnrolmentID, ev.EventName, e.Status " +
                                   "FROM Enrolments e " +
                                   "INNER JOIN Participants p ON e.ParticipantID=p.ParticipantID " +
                                   "INNER JOIN Events ev ON e.EventID=ev.EventID " +
                                   "WHERE p.UserID=" + participantUserId;
                    connect.Open();
                    SqlCommand cmd = new SqlCommand(query, connect);
                    SqlDataReader read = cmd.ExecuteReader();
                    while (read.Read())
                    {
                        list.Add("EnrolmentID: " + read["EnrolmentID"] + " | Event: " + read["EventName"] + " | Status: " + read["Status"]);
                    }
                    connect.Close();
                }
            }
            catch (Exception error) { }
            return list;
        }

        // PUT /api/enrolments/{id}/status
        [HttpPut("{id}/status")]
        public string UpdateEnrolmentStatus(int id, string status, string notes)
        {
            string message = string.Empty;
            try
            {
                using (SqlConnection connect = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Enrolments SET Status='" + status + "', Notes='" + notes +
                                   "', UpdatedAt=GETDATE() WHERE EnrolmentID=" + id;
                    connect.Open();
                    SqlCommand cmd = new SqlCommand(query, connect);
                    int result = cmd.ExecuteNonQuery();
                    message = result > 0 ? "Enrolment status updated" : "Update failed";
                    connect.Close();
                }
            }
            catch (Exception error) { message = error.Message; }
            return message;
        }
    }
}
