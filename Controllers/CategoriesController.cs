using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDayPart2Final.models;
using System.Data.SqlClient;

namespace RaceDayPart2Final.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        string connectionString = @"Data source=(localdb)\RaceDays;Database=RaceDaysDB;";

        // GET /api/categories/event/{eventId}
        [HttpGet("event/{eventId}")]
        public List<CategoryModel> GetCategoriesForEvent(int eventId)
        {
            List<CategoryModel> list = new List<CategoryModel>();
            try
            {
                using (SqlConnection connect = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Categories WHERE EventID=" + eventId + " AND IsActive=1";
                    connect.Open();
                    SqlCommand cmd = new SqlCommand(query, connect);
                    SqlDataReader read = cmd.ExecuteReader();
                    while (read.Read())
                    {
                        list.Add(new CategoryModel
                        {
                            CategoryID = Convert.ToInt32(read["CategoryID"]),
                            EventID = Convert.ToInt32(read["EventID"]),
                            CategoryName = read["CategoryName"].ToString(),
                            Distance = read["Distance"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(read["Distance"]),
                            Age = read["Age"] == DBNull.Value ? "" : read["Age"].ToString(),
                            StartTime = read["StartTime"] == DBNull.Value ? "" : read["StartTime"].ToString(),
                            EntryFee = read["EntryFee"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(read["EntryFee"]),
                            SortOrder = read["SortOrder"] == DBNull.Value ? 0 : Convert.ToInt32(read["SortOrder"])
                        });
                    }
                    connect.Close();
                }
            }
            catch (Exception error) { }
            return list;
        }

        // POST /api/categories/event/{eventId}
        [HttpPost("event/{eventId}")]
        public string CreateCategory(int eventId, int organiserUserId, string categoryName, decimal distance,
                                     string age, string startTime, decimal entryFee, int sortOrder)
        {
            string message = string.Empty;
            try
            {
                using (SqlConnection connect = new SqlConnection(connectionString))
                {
                    connect.Open();
                    string orgQuery = "SELECT OrganiserID FROM Organisers WHERE UserID=" + organiserUserId;
                    SqlCommand orgCmd = new SqlCommand(orgQuery, connect);
                    object orgIdObj = orgCmd.ExecuteScalar();
                    if (orgIdObj == null) return "403: Not an organiser";
                    int organiserId = Convert.ToInt32(orgIdObj);

                    string query = "INSERT INTO Categories (EventID, OrganiserID, CategoryName, Distance, Age, StartTime, EntryFee, SortOrder) " +
                                   "VALUES(" + eventId + "," + organiserId + ",'" + categoryName + "'," + distance + ",'" + age + "','" + startTime + "'," + entryFee + "," + sortOrder + ")";
                    SqlCommand cmd = new SqlCommand(query, connect);
                    int result = cmd.ExecuteNonQuery();
                    message = result > 0 ? "Category created successfully" : "Failed to create category";
                    connect.Close();
                }
            }
            catch (Exception error) { message = error.Message; }
            return message;
        }

        // PUT /api/categories/{id}
        [HttpPut("{id}")]
        public string UpdateCategory(int id, string categoryName, decimal distance, string age, string startTime, decimal entryFee, int sortOrder)
        {
            string message = string.Empty;
            try
            {
                using (SqlConnection connect = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Categories SET CategoryName='" + categoryName + "', Distance=" + distance +
                                   ", Age='" + age + "', StartTime='" + startTime + "', EntryFee=" + entryFee +
                                   ", SortOrder=" + sortOrder + ", UpdatedAt=GETDATE() WHERE CategoryID=" + id;
                    connect.Open();
                    SqlCommand cmd = new SqlCommand(query, connect);
                    int result = cmd.ExecuteNonQuery();
                    message = result > 0 ? "Category updated successfully" : "Update failed";
                    connect.Close();
                }
            }
            catch (Exception error) { message = error.Message; }
            return message;
        }

        // DELETE /api/categories/{id}
        [HttpDelete("{id}")]
        public string DeleteCategory(int id)
        {
            string message = string.Empty;
            try
            {
                using (SqlConnection connect = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Categories SET IsActive=0, UpdatedAt=GETDATE() WHERE CategoryID=" + id;
                    connect.Open();
                    SqlCommand cmd = new SqlCommand(query, connect);
                    int result = cmd.ExecuteNonQuery();
                    message = result > 0 ? "Category deleted successfully" : "Delete failed";
                    connect.Close();
                }
            }
            catch (Exception error) { message = error.Message; }
            return message;
        }
    }
}
 
