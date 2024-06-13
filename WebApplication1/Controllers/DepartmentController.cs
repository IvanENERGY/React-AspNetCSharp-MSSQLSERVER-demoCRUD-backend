using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using System.Data;
using System.Data.SqlClient;
using System.Xml.Linq;
using WebApplication1.Models;
namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase

    { //to read the connection string from config file, we need dependency inject
        private readonly IConfiguration _configuration;
        public DepartmentController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET:  /api/department   : get all department details
        [HttpGet]
        public JsonResult Get()
        { //stored pro/query/entity framework
            string query = @"
                select DepartmentId,DepartmentName from dbo.Department
                ";
            DataTable dataTable = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppConn");
            SqlDataReader myReader;
            using (SqlConnection myCon= new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                /* using (SqlCommand myCommand = new SqlCommand(query, myCon))*/
                using (SqlCommand myCommand = new SqlCommand("sp_GetAllDepartment", myCon))
                {
                    myReader= myCommand.ExecuteReader();
                    dataTable.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return new JsonResult(dataTable);   
        }
        //api for creating
        [HttpPost]
        public JsonResult Post(Department dep)
        { //stored pro/query/entity framework
            string query = @"
                insert into dbo.Department
                values(@DepartmentName)
                ";
            DataTable dataTable = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppConn");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@DepartmentName", dep.DepartmentName);
                    myReader = myCommand.ExecuteReader();
                    dataTable.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }



            }
            return new JsonResult("Added successfully");
        }


        [HttpPut]
        public JsonResult Put(Department dep)
        { //stored pro/query/entity framework
            string query = @"
                update dbo.Department
                set DepartmentName= @DepartmentName
                where DepartmentId=@DepartmentId
                ";
            DataTable dataTable = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppConn");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@DepartmentId", dep.DepartmentId);
                    myCommand.Parameters.AddWithValue("@DepartmentName", dep.DepartmentName);
                    myReader = myCommand.ExecuteReader();
                    dataTable.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return new JsonResult("Update successfully");
        }
        [HttpDelete ("{id}")]
            public JsonResult Delete(int id)
            { //stored pro/query/entity framework
                string query = @"
                        delete from dbo.Department
                        where DepartmentId=@DepartmentId
                        ";
                DataTable dataTable = new DataTable();
                string sqlDataSource = _configuration.GetConnectionString("EmployeeAppConn");
                SqlDataReader myReader;
                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@DepartmentId", id);
                        myReader = myCommand.ExecuteReader();
                        dataTable.Load(myReader);
                        myReader.Close();
                        myCon.Close();
                    }


            }
                return new JsonResult("delete successfully");
            }




    }
    
}

