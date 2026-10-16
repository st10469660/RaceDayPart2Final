using Microsoft.AspNetCore.Mvc;
using RaceDayPart2Final.models;
using System.Data.SqlClient;

namespace RaceDayPart2Final.backend
{
    public class ResultBackend
    {
        string connectionString = "Server=localhost;Database=RaceDaysDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public string message = string.Empty;

        // ---------------- CAPTURE RESULT ----------------
        public string CaptureResult(int enrolmentId, TimeOnly finishTime, int finishPosition,
                                    TimeOnly gunTime, TimeOnly chipTime, int overallFinishers, int categoryPosition)
        {
            try
            {
                using (SqlConnection connect = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO Results (EnrolmentID, FinishTime, FinishPosition, GunTime, ChipTime, OverallFinishers, CategoryPosition, IsProvisional) " +
                                   "VALUES(" + enrolmentId + ",'" + finishTime + "'," + finishPosition +
                                   ",'" + gunTime + "','" + chipTime + "'," + overallFinishers + "," + categoryPosition + ",0)";

                    connect.Open();

                    SqlCommand run_query = new SqlCommand(query, connect);
                    int result = run_query.ExecuteNonQuery();

                    if (result > 0)
                    {
                        message = "Result captured successfully";
                    }
                    else
                    {
                        message = "Failed to capture result";
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

        // ---------------- GET RESULTS FOR AN EVENT ----------------
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

                    SqlCommand run_query = new SqlCommand(query, connect);
                    SqlDataReader read = run_query.ExecuteReader();

                    while (read.Read())
                    {
                        list.Add("Pos " + read["FinishPosition"] +
                                 " | " + read["FullName"] +
                                 " | Time: " + read["FinishTime"]);
                    }

                    connect.Close();
                }
            }
            catch (Exception error)
            {
                message = error.Message;
            }

            return list;
        }

        // ---------------- GET RESULTS FOR A PARTICIPANT ----------------
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

                    SqlCommand run_query = new SqlCommand(query, connect);
                    SqlDataReader read = run_query.ExecuteReader();

                    while (read.Read())
                    {
                        list.Add("Event: " + read["EventName"] +
                                 " | Time: " + read["FinishTime"] +
                                 " | Position: " + read["FinishPosition"]);
                    }

                    connect.Close();
                }
            }
            catch (Exception error)
            {
                message = error.Message;
            }

            return list;
        }

        public string CaptureResult(int enrolmentId, string finishTime, int finishPosition, string gunTime, string chipTime, int overallFinishers, int categoryPosition)
        {
            throw new NotImplementedException();
        }
    }
}
    