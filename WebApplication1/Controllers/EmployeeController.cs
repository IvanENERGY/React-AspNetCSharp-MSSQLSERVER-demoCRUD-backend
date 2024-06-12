using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using WebApplication1.Models;
using System.Runtime.Intrinsics.Arm;


namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase

    { //to read the connection string from config file, we need dependency inject
        private readonly IConfiguration _configuration;
        //to use physical path of the photo, we need dependency inject
        private readonly IWebHostEnvironment _env;  
        public EmployeeController(IConfiguration configuration,IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env; 
        }
        //api for get all department details
        [HttpGet]
        public JsonResult Get()
        { //stored pro/query/entity framework
            string query = @"
                select EmployeeId,EmployeeName,Department,
                convert(varchar(10),DateOfJoining,120) as DateOfJoining,PhotoFileName
                from
                dbo.Employee
                ";
            DataTable dataTable = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppConn");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    dataTable.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return new JsonResult(dataTable);
        }
        //api for creating
        [HttpPost]
        public JsonResult Post(Employee emp)
        { //stored pro/query/entity framework
            string query = @"
                insert into dbo.Employee
                (EmployeeName,Department,DateOfJoining,PhotoFileName)
                values(@EmployeeName,@Department,@DateOfJoining,@PhotoFileName)
                ";
            DataTable dataTable = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppConn");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@EmployeeName", emp.EmployeeName);
                    myCommand.Parameters.AddWithValue("@Department", emp.Department);
                    myCommand.Parameters.AddWithValue("@DateOfJoining", emp.DateOfJoining);
                    myCommand.Parameters.AddWithValue("@PhotoFileName", emp.PhotoFileName);
                    myReader = myCommand.ExecuteReader();
                    dataTable.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return new JsonResult("Added successfully");
        }


        [HttpPut]
        public JsonResult Put(Employee emp)
        { //stored pro/query/entity framework
            string query = @"
                update dbo.Employee
                set EmployeeName= @EmployeeName,
                Department=@Department,
                DateOfJoining=@DateOfJoining,
                PhotoFileName=@PhotoFileName
                where EmployeeId=@EmployeeId
                ";
            DataTable dataTable = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppConn");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@EmployeeName", emp.EmployeeName);
                    myCommand.Parameters.AddWithValue("@Department", emp.Department);
                    myCommand.Parameters.AddWithValue("@DateOfJoining", emp.DateOfJoining);
                    myCommand.Parameters.AddWithValue("@PhotoFileName", emp.PhotoFileName);
                    myCommand.Parameters.AddWithValue("@EmployeeId", emp.EmployeeId);
                    myReader = myCommand.ExecuteReader();
                    dataTable.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return new JsonResult("Update successfully");
        }
        [HttpDelete("{id}")]
        public JsonResult Delete(int id)
        { //stored pro/query/entity framework
            string query = @"
                        delete from dbo.Employee
                        where EmployeeId=@EmployeeId
                        ";
            DataTable dataTable = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppConn");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@EmployeeId", id);
                    myReader = myCommand.ExecuteReader();
                    dataTable.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return new JsonResult("delete successfully");
        }

        [Route("SaveFile")]
        [HttpPost]
        public JsonResult SaveFile()
        {
            try
            {
                var httpRequest = Request.Form;
                var postedFile = httpRequest.Files[0];
                string filename = postedFile.FileName;
                var physicalPath = _env.ContentRootPath + "/Photos/" + filename;

                using (var stream= new FileStream(physicalPath,FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }
                return new JsonResult(filename);
            }
            catch (Exception ex)
            {
                return new JsonResult("annoymous.png");
            }
        }



    }
}
