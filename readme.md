C# .net Part

<hr>
<h1>This is readme.md  --- Creating ASP API in C# (w/SQL Server) </h1>
<p>SQL table creation script & inital data insert script & stored procedures script are placed inside <i>WebApplication1/howIDoDbpart folder </i></p>

<h1>1. Create new Project ->ASP.net Core Web API</h1>

<h1>2. Modify progam.cs</h1>

<h2>before builder.build</h2>
<pre>
//CORS
builder.Services.AddCors(c =>
{
    c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});
</pre>

&#9989;for JSON :Install Nuget Package - Microsoft.AspNetCore.Mvc.NewtonsoftJson

<pre>
builder.Services.AddControllersWithViews().AddNewtonsoftJson(options =>
options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore)
.AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());
</pre>

<h2>Before app.run</h2>

<pre>
//for FileStorage in Server 
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Photos")),
    RequestPath = "/Photos"

});
</pre>

<h1>3. Modify appsettings.json</h1>

<pre>
"ConnectionStrings": {
"EmployeeAppConn": "Data Source=IvanPC2023;Initial Catalog=test_db;Integrated Security=True"
},//obtain by connect with server explorer
</pre>

<h1>4. Create Models folder</h1>

Employee.cs
<pre>
namespace WebApplication1.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; } 
        public string EmployeeName { get; set; }    
        public string Department { get; set; }
        public string DateOfJoining { get; set; }
        public string PhotoFileName {  get; set; }
    }
}
</pre>

<h1>5. Create Controllers folder  </h1>

&#9989;for Sql:Install Nuget Package - System.Data.SqlClient

EmployeeController.cs

 <pre>

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

    {
        /*****Dependency Injection**********/
        private readonly IConfiguration _configuration;//to read the connection string from config file, we need dependency inject

        private readonly IWebHostEnvironment _env;//to use physical path of the photo, we need dependency inject
        public EmployeeController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }
        /*************/



        //READ
        //GET /api/Employee   Get all Employees
        [HttpGet]
        public JsonResult Get()
        {
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
                /*using (SqlCommand myCommand = new SqlCommand("sp_GetAllEmployee", myCon))*/
                /*The stored proc sp_GetAllEmployee: 
                 * Input  -Nothing
                 * Output -SelectResult
                 */
                {
                   /*myCommand.CommandType = CommandType.StoredProcedure;*/
                    myReader = myCommand.ExecuteReader();
                    dataTable.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return new JsonResult(dataTable);
        }

        //READ ONE
        //GET /api/Employee/:id   Get one Employee
        [HttpGet("{id}")]
        public JsonResult Get(int id)
        {
            string query = @"
                select EmployeeId,EmployeeName,Department,
                convert(varchar(10),DateOfJoining,120) as DateOfJoining,PhotoFileName
                from
                dbo.Employee
                where EmployeeId=@id
                ";
            DataTable dataTable = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppConn");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                /*using (SqlCommand myCommand = new SqlCommand("sp_GetOneEmployee", myCon))*/
                
                /*The stored proc sp_GetOneEmployee: 
                    * Input  -id
                    * Output -SelectResult
                */
                {
                /*  myCommand.CommandType = CommandType.StoredProcedure;*/
                    myCommand.Parameters.AddWithValue("@id", id);
                    myReader = myCommand.ExecuteReader();
                    dataTable.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return new JsonResult(dataTable);
        }


        //CREATE
        //POST /api/Employee   Create one Employee
        [HttpPost]
        public JsonResult Post(Employee emp)
        { 
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
                //using (SqlCommand myCommand = new SqlCommand("sp_CreateEmployee", myCon))

                /*The stored proc sp_CreateEmployee: 
                     * Input  -@EmployeeName,@Department,@DateOfJoining,@PhotoFileName
                     * Output -SelectResult
                */
                {
                //  myCommand.CommandType = CommandType.StoredProcedure;
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
            return new JsonResult(dataTable);// or new JsonResult("Added successfully")
        }

        //UPDATE
        //PUT /api/Employee   Update one Employee
        [HttpPut]
        public JsonResult Put(Employee emp)
        { 
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
                //using (SqlCommand myCommand = new SqlCommand("sp_UpdateEmployee", myCon))
                /*The stored proc sp_UpdateEmployee: 
                     * Input  -@EmployeeName,@Department,@DateOfJoining,@PhotoFileName,@EmployeeId
                     * Output -SelectResult
                */
                {
                    //myCommand.CommandType = CommandType.StoredProcedure;
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
            return new JsonResult(dataTable); //or new JsonResult("Update successfully")
        }

        //DELETE
        //DELETE /api/Employee/:id   Delete one Employee
        [HttpDelete("{id}")]
        public JsonResult Delete(int id)
        { 
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
                //using (SqlCommand myCommand = new SqlCommand("sp_DeleteEmployee", myCon))
                /*The stored proc sp_DeleteEmployee: 
                     * Input  -@EmployeeId
                     * Output -SelectResult
                */
                {
                    //myCommand.CommandType = CommandType.StoredProcedure;
                    myCommand.Parameters.AddWithValue("@EmployeeId", id);
                    myReader = myCommand.ExecuteReader();
                    dataTable.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return new JsonResult(dataTable);//"Delete successfully"
        }

        // POST /api/Employee/saveFile
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



 </pre>

 <h1>Deploy to Local Area Network (via IIS; such that the API is accessible by every devices on LAN) </h1>
 <h2>Setup</h2>
 <ol>
  <li>Install the .Net 8 hosting bundle  </li>
   [SolutionItem/screenshot/16.png]
 <li>Publish the web application </li>
 [SolutionItem/screenshot/1.png]
 <li>Click "Add a publish profile"</li>
 [SolutionItem/screenshot/2.png]
 <li>Choose Folder</li>
 [SolutionItem/screenshot/3.png]
 <li>Use default path</li>
  [SolutionItem/screenshot/4.png]
 <li>Configure the publish settings</li>
  [SolutionItem/screenshot/5.png]
   <li>Click publish</li>
  [SolutionItem/screenshot/6.png]
  <li>Click open folder->Copy all files within the published folder</li>
  [SolutionItem/screenshot/7.png]
  <li>Go to the server machine, locate the wwwroot folder </li>
  [SolutionItem/screenshot/8.png]
  <li>Copy all the files to the wwwroot folder </li>
  [SolutionItem/screenshot/9.png]
  <li>Create sites on IIS ; assign the diretory and port number </li>
  [SolutionItem/screenshot/10.png]

 </ol>
 <p>The page should be working on <localip>:<port></p>

 <h2>IF any error exist: </h2>
 <li>Open "Event Viewer" -> choose Window Logs->Application</li>
 [SolutionItem/screenshot/12.png]
 <li>Common err 1 : File Storage path not configured appropriately </li>
 [SolutionItem/screenshot/13.png]
 <p>Solution: Since our api use folder path for storage, we need to add those paths to the publish folder as well  </p>
 <p>For example,we need a folder called "Photos" in our program</p>
  [SolutionItem/screenshot/14.png]
   <p>We need to add the folder to the published folder inside wwwroot </p>
     [SolutionItem/screenshot/15.png]
  <li>Common err 2 :SQL server connect unsuccessful </li>
 [SolutionItem/screenshot/17.png]
 <p>Solution: Add the corresponding user in SQL database; In this case, it is "IIS APPPOOL\employeeapi"</p>
 <p>Add the user to both SQL Server Login User & DatabaseUser; Assign db-owner schema for the user </p>
  [SolutionItem/screenshot/18.png]
  [SolutionItem/screenshot/19.png]
  [SolutionItem/screenshot/20.png]
  [SolutionItem/screenshot/21.png]
 [SolutionItem/screenshot/22.png]
  [SolutionItem/screenshot/23.png]

 </ol>