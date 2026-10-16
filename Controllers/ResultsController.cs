using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace RaceDayPart2Final.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResultsController : ControllerBase
    {
        string connectionString = @"Data source=(localdb)\RaceDays;Database=RaceDaysDB;";

        // POST /api/results
        [HttpPost]
        public string CaptureResult(int enrolmentId, TimeOnly finishTime, int finishPosition,
                                    TimeOnly gunTime, TimeOnly chipTime, int overallFinishers, int categoryPosition)
        {
            string message = string.Empty;
            try
            {
                using (SqlConnection connect = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO Results (EnrolmentID, FinishTime, FinishPosition, GunTime, ChipTime, OverallFinishers, CategoryPosition, IsProvisional) " +
                                   "VALUES(" + enrolmentId + ",'" + finishTime + "'," + finishPosition + ",'" + gunTime + "','" + chipTime + "'," +
                                   overallFinishers + "," + categoryPosition + ",0)";
                    connect.Open();
                    SqlCommand cmd = new SqlCommand(query, connect);
                    int result = cmd.ExecuteNonQuery();
                    message = result > 0 ? "Result captured successfully" : "Failed to capture result";
                    connect.Close();
                }
            }
            catch (Exception error) { message = error.Message; }
            return message;
        }

        // GET /api/results/event/{eventId}
        [HttpGet("event/{eventId}")]
        public List<string> GetResultsForEvent(int eventId)
        {
            List<string> list = new List<string>();
            try
            {
                using (SqlConnection connect = new SqlConnection(connectionString))
                {
                    string query = "SELECT r.ResultID, u.FullName, r.FinishTime, r.FinishPosition " +
                                   "FROM Results r " +
                                   "INNER JOIN Enrolments e ON r.EnrolmentID=e.EnrolmentID " +
                                   "INNER JOIN Participants p ON e.ParticipantID=p.ParticipantID " +
                                   "INNER JOIN Users u ON p.UserID=u.UserID " +
                                   "WHERE e.EventID=" + eventId;
                    connect.Open();
                    SqlCommand cmd = new SqlCommand(query, connect);
                    SqlDataReader read = cmd.ExecuteReader();
                    while (read.Read())
                    {
                        list.Add("Pos " + read["FinishPosition"] + " | " + read["FullName"] +
                                 " | Time: " + read["FinishTime"]);
                    }
                    connect.Close();
                }
            }
            catch (Exception error) { }
            return list;
        }

        // GET /api/results/participant/{participantUserId}
        [HttpGet("participant/{participantUserId}")]
        public List<string> GetResultsForParticipant(int participantUserId)
        {
            List<string> list = new List<string>();
            try
            {
                using (SqlConnection connect = new SqlConnection(connectionString))
                {
                    string query = "SELECT ev.EventName, r.FinishTime, r.FinishPosition " +
                                   "FROM Results r " +
                                   "INNER JOIN Enrolments e ON r.EnrolmentID=e.EnrolmentID " +
                                   "INNER JOIN Participants p ON e.ParticipantID=p.ParticipantID " +
                                   "INNER JOIN Events ev ON e.EventID=ev.EventID " +
                                   "WHERE p.UserID=" + participantUserId;
                    connect.Open();
                    SqlCommand cmd = new SqlCommand(query, connect);
                    SqlDataReader read = cmd.ExecuteReader();
                    while (read.Read())
                    {
                        list.Add("Event: " + read["EventName"] + " | Time: " + read["FinishTime"] +
                                 " | Position: " + read["FinishPosition"]);
                    }
                    connect.Close();
                }
            }
            catch (Exception error) { }
            return list;
        }
    }
}

