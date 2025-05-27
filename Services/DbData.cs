using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;

namespace ClinetraSolutions.Services
{
    public class DbData
    {
        public static string jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
        public static string jsonString = File.ReadAllText(jsonPath);
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
        public static bool PostCourse(IFormFile courseImage, string courseType, string courseDetails, string courseName, string coursePrice, string studentType = null)
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
                string query = $"INSERT INTO Courses(Category,CourseName,CourseDetails,Image,Price,StudentType) VALUES(@Category,@CourseName,@CourseDetails,@Image,@coursePrice,@StudentType)";
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Category", courseType);
                    cmd.Parameters.AddWithValue("@CourseName", courseName);
                    cmd.Parameters.AddWithValue("@CourseDetails", courseDetails);
                    cmd.Parameters.AddWithValue("@Image", imageData);
                    cmd.Parameters.AddWithValue("@coursePrice", coursePrice);
                    if (string.IsNullOrEmpty(studentType))
                    {
                        cmd.Parameters.AddWithValue("@StudentType", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@StudentType", studentType);
                    }
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
        public static DataTable GetCourse(string Category)
        {
            string query = $"SELECT C.Id as Id,CS.seatId as SeatId, Category, CourseName, CourseDetails, Price, Image, StudentType, BatchStartDate, AvailableSeats FROM Courses C inner join CourseSeats CS on C.id=CS.CourseId where Category= '{Category}'";
            return GetDBTable(query, Category);
        }
        public static DataTable GetAllCourses()
        {
            string query = "SELECT Id, Category, CourseName, CourseDetails, Price, Image, StudentType FROM Courses";
            return GetDBTable(query, "AllCourses");
        }
        public static DataTable GetAllSeats()
        {
            string query = "SELECT C.Id as CourseId,CS.seatId, Category, CourseName, BatchStartDate, AvailableSeats FROM Courses C inner join CourseSeats CS on C.id=CS.CourseId";
            return GetDBTable(query, "AllSeats");
        }
        public static DataTable GetCourseDetails(string category, string course)
        {
            string query = $"SELECT Id, CourseName, courseDetails, image, Price FROM Courses where Category='{category}' and CourseName like '%{course}%'";
            return GetDBTable(query, tableName: category.Replace(" ",""));
        }
        public static DataTable GetPharmaCourse(string studentType)
        {
            string query = $"SELECT Id, CourseName, courseDetails, image, Price, StudentType from Courses where Category='Pharma' and StudentType='{studentType}'";
            return GetDBTable(query, tableName: "pharma");
        }
        public static DataTable GetEnquiryData()
        {
            string query = "SELECT Id, Name, Mobile, Qualification, Comments FROM EnquiryForm";
            return GetDBTable(query, "EnquiryForm");
        }
        public static DataTable GetStudentsData()
        {
            string query = "SELECT * FROM StudentEnrollment";
            return GetDBTable(query, "StudentsData");
        }
        public static DataTable GetDBTable(string query, string tableName)
        {
            MySqlConnection connection = null;
            DataTable dt = new DataTable(tableName);
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
        public static bool UpdatePrice(string id, string price, string coursetype)
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
                string query = $"UPDATE {coursetype} SET Price = @Price WHERE Id = @Id";
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Price", price);
                    cmd.Parameters.AddWithValue("@Id", id);
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
        public static bool EnrollAStudent(string id, string name, string category, string courses, string duration, string fee, string discount, string PaidFees, string BalanceFees)
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
                string query = $"INSERT INTO StudentEnrollment(Id, Name, Category, Courses, Duration, Fee, Discount, PaidFees, BalanceFees, JoiningDate, CourseEndDate) VALUES (@Id, @Name, @Category, @Courses, @Duration, @Fee, @Discount, @PaidFees, @BalanceFees, @JoiningDate, @CourseEndDate)";
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    DateTime date = DateTime.Now.Date;
                    DateTime date1 = date.AddMonths(Convert.ToInt32(duration));
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Category", category);
                    cmd.Parameters.AddWithValue("@Courses", courses);
                    cmd.Parameters.AddWithValue("@Duration", duration);
                    cmd.Parameters.AddWithValue("@Fee", fee);
                    cmd.Parameters.AddWithValue("@Discount", discount);
                    cmd.Parameters.AddWithValue("@PaidFees", PaidFees);
                    cmd.Parameters.AddWithValue("@BalanceFees", BalanceFees);
                    cmd.Parameters.AddWithValue("@JoiningDate", DateTime.Now.Date);
                    cmd.Parameters.AddWithValue("@CourseEndDate", DateTime.Now.Date.AddMonths(Convert.ToInt32(duration)));
                    cmd.ExecuteNonQuery();
                    flag = true;
                }
            }
            catch (Exception ex)
            {
                res = "Database error, contact your developer.";
                throw;
            }
            finally
            {
                con?.Close();
            }
            return flag;
        }
        public static bool SetNewBatch(string courseId, string batchStartDate)
        {
            bool flag = false;
            JObject db = ReadJSONData();
            MySqlConnection con = null;
            string conn = db["ConnectionStrings"]["MySqlConnection"].ToString();
            try
            {
                con = new MySqlConnection(conn);
                con.Open();
                string query = "INSERT INTO courseseats(CourseId,BatchStartDate) values(@courseId,@BatchStartDate)";
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@CourseId", courseId);
                    cmd.Parameters.AddWithValue("@BatchStartDate", batchStartDate);
                    cmd.ExecuteNonQuery();
                    flag = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                throw;
            }
            finally
            {
                con?.Close();
            }
            return flag;
        }
        public static bool UpdateBatch(string seatId, string batchStartDate)
        {
            bool flag = false;
            JObject db = ReadJSONData();
            MySqlConnection con = null;
            string conn = db["ConnectionStrings"]["MySqlConnection"].ToString();
            try
            {
                con = new MySqlConnection(conn);
                con.Open();
                string query = "UPDATE courseseats SET BatchStartDate=@BatchStartDate WHERE SeatId=@SeatId";

                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@SeatId", seatId);
                    cmd.Parameters.AddWithValue("@BatchStartDate", batchStartDate);
                    cmd.ExecuteNonQuery();
                    flag = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
            finally
            {
                con?.Close();
            }
            return flag;
        }
        public static bool UpdateCourseSeat(string seatId)
        {
            bool flag = false;
            JObject db = ReadJSONData();
            MySqlConnection con = null;
            string conn = db["ConnectionStrings"]["MySqlConnection"].ToString();
            try
            {
                con = new MySqlConnection(conn);
                con.Open();
                string query = "UPDATE CourseSeats SET AvailableSeats = AvailableSeats - 1 WHERE seatId = @seatId";
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@SeatId", seatId);
                    cmd.ExecuteNonQuery();
                    flag = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
            finally
            {
                con?.Close();
            }
            return flag;
        }
        public static DataTable GetACourseAvail(string seatId)
        {
            string query = $"Select SeatId, CourseId, BatchStartDate, AvailableSeats from CourseSeats where SeatId={seatId}";
            return GetDBTable(query, "CourseAvail");
        }
    }
}