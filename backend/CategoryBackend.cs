using RaceDayPart2Final.models;
using System.Data.SqlClient;

namespace RaceDayPart2Final.backend
{
    public class CategoryBackend
    {
        string connectionString = "Server=localhost;Database=RaceDaysDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public string message = string.Empty;

        public string Age { get; private set; }

        public List<CategoryModel> GetCategoriesForEvent(int eventId)
        {
            return GetCategoriesForEvent(eventId, Age);
        }

        // GET ALL CATEGORIES FOR AN EVENT
        public List<CategoryModel> GetCategoriesForEvent(int eventId, string age)
        {
            List<CategoryModel> list = new List<CategoryModel>();

            try
            {
                using (SqlConnection connect = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Categories WHERE EventID=" + eventId + " AND IsActive=1";

                    connect.Open();

                    SqlCommand run_query = new SqlCommand(query, connect);
                    SqlDataReader read = run_query.ExecuteReader();

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
            catch (Exception error)
            {
                message = error.Message;
            }

            return list;
        }

        //  CREATE CATEGORY 
        public string CreateCategory(int eventId, int organiserUserId, string categoryName, decimal distance,
                                     string age, string startTime, decimal entryFee, int sortOrder)
        {
            try
            {
                using (SqlConnection connect = new SqlConnection(connectionString))
                {
                    connect.Open();

                    // find OrganiserID
                    string orgQuery = "SELECT OrganiserID FROM Organisers WHERE UserID=" + organiserUserId;
                    SqlCommand orgCmd = new SqlCommand(orgQuery, connect);
                    object orgIdObj = orgCmd.ExecuteScalar();

                    if (orgIdObj == null)
                    {
                        return "403: Not an organiser";
                    }

                    int organiserId = Convert.ToInt32(orgIdObj);

                    string query = "INSERT INTO Categories (EventID, OrganiserID, CategoryName, Distance, Age, StartTime, EntryFee, SortOrder) " +
                                   "VALUES(" + eventId + "," + organiserId + ",'" + categoryName + "'," + distance +
                                   ",'" + age + "','" + startTime + "'," + entryFee + "," + sortOrder + ")";

                    SqlCommand run_query = new SqlCommand(query, connect);
                    int result = run_query.ExecuteNonQuery();

                    if (result > 0)
                    {
                        message = "Category created successfully";
                    }
                    else
                    {
                        message = "Failed to create category";
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

        // UPDATE CATEGORY 
        public string UpdateCategory(int categoryId, string categoryName, decimal distance, string age,
                                     string startTime, decimal entryFee, int sortOrder)
        {
            try
            {
                using (SqlConnection connect = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Categories SET CategoryName='" + categoryName + "', Distance=" + distance +
                                   ", Age='" + age + "', StartTime='" + startTime + "', EntryFee=" + entryFee +
                                   ", SortOrder=" + sortOrder + ", UpdatedAt=GETDATE() WHERE CategoryID=" + categoryId;

                    connect.Open();

                    SqlCommand run_query = new SqlCommand(query, connect);
                    int result = run_query.ExecuteNonQuery();

                    if (result > 0)
                    {
                        message = "Category updated successfully";
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

        //  DELETE CATEGORY (soft delete)
        public string DeleteCategory(int categoryId)
        {
            try
            {
                using (SqlConnection connect = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Categories SET IsActive=0, UpdatedAt=GETDATE() WHERE CategoryID=" + categoryId;

                    connect.Open();

                    SqlCommand run_query = new SqlCommand(query, connect);
                    int result = run_query.ExecuteNonQuery();

                    if (result > 0)
                    {
                        message = "Category deleted successfully";
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
    