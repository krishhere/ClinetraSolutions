using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;

namespace ClinetraSolutions.Services
{
    public class DbData
    {
        public static string pathDirectory = Directory.GetCurrentDirectory();
        public static string jsonPath = Path.Combine(pathDirectory, "appsettings.json");
        public static string jsonString = File.ReadAllText(jsonPath);
        //public static dynamic jsonData = JsonSerializer.Deserialize<dynamic>(jsonString);
        public static JObject ReadJSONData()
        {
            JObject jObject;
            using (StreamReader file = System.IO.File.OpenText(jsonPath))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                jObject = (JObject)JToken.ReadFrom(reader);
            }
            return jObject;
        }
        public static bool PostData(string query)
        {
            bool flag = false;
            string res = string.Empty;
            JObject db = ReadJSONData();
            MySqlConnection con = null;
            string conn = db["ConnectionStrings"]["MySqlConnection"].ToString();
            try
            {
                con = new MySqlConnection(conn);
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.ExecuteNonQuery();
                    flag = true;
                }
            }
            catch (Exception ex)
            {
                res = "Database error, contact your developer.";
            }
            finally
            {
                con?.Close();
            }
            return flag;
        }
        public static bool PostCourse(IFormFile courseImage, string courseType, string courseDetails, string courseName)
        {
            bool flag = false;
            string res = string.Empty;
            JObject db = ReadJSONData();
            MySqlConnection con = null;
            string conn = db["ConnectionStrings"]["MySqlConnection"].ToString();
            try
            {
                con = new MySqlConnection(conn);
                con.Open();
                byte[] imageData;
                using (var memoryStream = new MemoryStream())
                {
                    courseImage.CopyToAsync(memoryStream);
                    imageData = memoryStream.ToArray();
                }
                string query = $"INSERT INTO {courseType}(CourseName, CourseDetails, Image) VALUES (@CourseName, @CourseDetails, @Image)";

                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@CourseName", courseName);
                    cmd.Parameters.AddWithValue("@CourseDetails", courseDetails);
                    cmd.Parameters.AddWithValue("@Image", imageData);
                    cmd.ExecuteNonQuery();
                    flag = true;
                }
            }
            catch (Exception ex)
            {
                res = "Database error, contact your developer.";
            }
            finally
            {
                con?.Close();
            }
            return flag;
        }
        public static DataTable GetData(string tableName)
        {
            string query = $"SELECT Id, CourseName, courseDetails, image FROM {tableName}";
            return GetDBTable(query);
        }
        public static DataTable GetDBTable(string query)
        {
            MySqlConnection connection = null;
            DataTable dt = new DataTable();
            JObject db = ReadJSONData();
            string conn = db["ConnectionStrings"]["MySqlConnection"].ToString();
            try
            {
                connection = new MySqlConnection(conn);
                connection.Open();
                MySqlCommand cmd = connection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                adapter.Fill(dt);
            }
            catch (Exception ex)
            {
                //ScriptManager.RegisterStartupScript(this, this.GetType(), "popup", "alert('Technical issues. please try later');", true);
            }
            finally
            {
                connection.Close();
            }
            return dt;
        }
    }
}