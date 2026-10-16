using System.Data.SqlClient;

namespace RaceDayPart2Final.backend
{
    public class EnrolmentsBackend
    {
        string connectionString = "Server=localhost;Database=RaceDaysDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public string message = string.Empty;

        // ---------------- ENROL A PARTICIPANT ----------------
        public string EnrolParticipant(int eventId, int participantUserId, int categoryId)
        {
            try
            {
                using (SqlConnection connect = new SqlConnection(connectionString))
                {
                    connect.Open();

                    // find ParticipantID from UserID
                    string pQuery = "SELECT ParticipantID FROM Participants WHERE UserID=" + participantUserId;
                    SqlCommand pCmd = new SqlCommand(pQuery, connect);
                    object pObj = pCmd.ExecuteScalar();

                    if (pObj == null)
                    {
                        return "403: Not a participant";
                    }

                    int participantId = Convert.ToInt32(pObj);

                    // check duplicate
                    string dupQuery = "SELECT COUNT(*) FROM Enrolments WHERE EventID=" + eventId + " AND ParticipantID=" + participantId;
                    SqlCommand dupCmd = new SqlCommand(dupQuery, connect);
                    int dup = Convert.ToInt32(dupCmd.ExecuteScalar());

                    if (dup > 0)
                    {
                        return "409: Already enrolled";
                    }

                    // insert
                    string query = "INSERT INTO Enrolments (EventID, ParticipantID, CategoryID, Status, EntryFeePaid) " +
                                   "VALUES(" + eventId + "," + participantId + "," + categoryId + ",'Pending',0)";

                    SqlCommand run_query = new SqlCommand(query, connect);
                    int result = run_query.ExecuteNonQuery();

                    if (result > 0)
                    {
                        message = "Enrolment submitted";
                    }
                    else
                    {
                        message = "Enrolment failed";
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

        // ---------------- GET ENROLMENTS FOR AN EVENT (organiser view) ----------------
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

                    SqlCommand run_query = new SqlCommand(query, connect);
                    SqlDataReader read = run_query.ExecuteReader();

                    while (read.Read())
                    {
                        list.Add("EnrolmentID: " + read["EnrolmentID"] +
                                 " | " + read["FullName"] +
                                 " | Status: " + read["Status"] +
                                 " | Paid: " + read["EntryFeePaid"]);
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

        // ---------------- GET ENROLMENTS FOR A PARTICIPANT ----------------
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

                    SqlCommand run_query = new SqlCommand(query, connect);
                    SqlDataReader read = run_query.ExecuteReader();

                    while (read.Read())
                    {
                        list.Add("EnrolmentID: " + read["EnrolmentID"] +
                                 " | Event: " + read["EventName"] +
                                 " | Status: " + read["Status"]);
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

        // ---------------- UPDATE ENROLMENT STATUS ----------------
        public string UpdateEnrolmentStatus(int enrolmentId, string status, string notes)
        {
            try
            {
                using (SqlConnection connect = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Enrolments SET Status='" + status + "', Notes='" + notes +
                                   "', UpdatedAt=GETDATE() WHERE EnrolmentID=" + enrolmentId;

                    connect.Open();

                    SqlCommand run_query = new SqlCommand(query, connect);
                    int result = run_query.ExecuteNonQuery();

                    if (result > 0)
                    {
                        message = "Enrolment status updated";
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
    }
}
    