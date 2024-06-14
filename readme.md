C# .net Part

<hr>
<p>This is CRUD demo full-stack application built using React,ASP.net(C#)
SQL server. </p>
<p>Frontend: React (https://github.com/IvanENERGY/REACT-ASPNETC-MSSQLSERVER-demoCRUD-frontend)</p>
<p>Backend: SQL server & API written in C# (This Repo)</p>
<h1>&#128310;Guide For Creating ASP API in C# (w/SQL Server) </h1>
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

 <h1>&#128640;Deploy to Local Area Network (via IIS; such that the API is accessible by every devices on LAN) </h1>
 <h2>Setup</h2>
 <ol>
  <li>Install the .Net 8 hosting bundle  </li>
  <p> [WebApplication1/Deployment-readme-screenshot/16.png]  </p>
   <img width="1349" alt="16" src="https://github.com/IvanENERGY/REACT-ASPNETC-MSSQLSERVER-demoCRUD-backend/assets/90034836/78d99869-339a-44e2-ac96-a620fdb212b2">
 <li>Publish the web application </li>
   <p>[WebApplication1/Deployment-readme-screenshot/1.png]</p>
 <img width="530" alt="1" src="https://github.com/IvanENERGY/REACT-ASPNETC-MSSQLSERVER-demoCRUD-backend/assets/90034836/ff2e6fac-46cd-42d2-a701-bfad5cb18021">
 <li>Click "Add a publish profile"</li>
   <p>[WebApplication1/Deployment-readme-screenshot/2.png]</p>
 <img width="1043" alt="2" src="https://github.com/IvanENERGY/REACT-ASPNETC-MSSQLSERVER-demoCRUD-backend/assets/90034836/e10ca84a-2a10-4b84-95dc-6325eecd983d">
 <li>Choose Folder</li>
   <p>[WebApplication1/Deployment-readme-screenshot/3.png]</p>
 <img width="638" alt="3" src="https://github.com/IvanENERGY/REACT-ASPNETC-MSSQLSERVER-demoCRUD-backend/assets/90034836/52430af3-6485-415e-a046-3eb87b9c40d9">
 <li>Use default path</li>
    <p>[WebApplication1/Deployment-readme-screenshot/4.png]</p>
  <img width="592" alt="4" src="https://github.com/IvanENERGY/REACT-ASPNETC-MSSQLSERVER-demoCRUD-backend/assets/90034836/8d6f9119-4814-4a33-b785-ade0940b675e">
 <li>Configure the publish settings</li>
    <p>[WebApplication1/Deployment-readme-screenshot/5.png]</p>
  <img width="1206" alt="5" src="https://github.com/IvanENERGY/REACT-ASPNETC-MSSQLSERVER-demoCRUD-backend/assets/90034836/7d76b86e-f2d5-4106-8c58-79603e6eea02">
   <li>Click publish</li>
    <p>[WebApplication1/Deployment-readme-screenshot/6.png]</p>
  <img width="770" alt="6" src="https://github.com/IvanENERGY/REACT-ASPNETC-MSSQLSERVER-demoCRUD-backend/assets/90034836/ecf08b41-1728-4bc3-a7e4-7da740fd1bdd">
  <li>Click open folder->Copy all files within the published folder</li>
    <p>[WebApplication1/Deployment-readme-screenshot/7.png]</p>
  <img width="1169" alt="7" src="https://github.com/IvanENERGY/REACT-ASPNETC-MSSQLSERVER-demoCRUD-backend/assets/90034836/4895b5c3-b139-441d-b109-5ac6b60f5558">
  <li>Go to the server machine, locate the wwwroot folder, create a new folder for hosting the api </li>
    <p>[WebApplication1/Deployment-readme-screenshot/8.png]</p>
  <img width="709" alt="8" src="https://github.com/IvanENERGY/REACT-ASPNETC-MSSQLSERVER-demoCRUD-backend/assets/90034836/ef7e9379-683d-4623-a578-0abc01790f7c">
  <li>Copy all the files to the api folder </li>
    <p>[WebApplication1/Deployment-readme-screenshot/9.png]</p>
  <img width="748" alt="9" src="https://github.com/IvanENERGY/REACT-ASPNETC-MSSQLSERVER-demoCRUD-backend/assets/90034836/81ab5b3b-868f-4913-93c7-0524365baee2">
  <li>Create sites on IIS ; assign the directory and port number </li>
    <p>[WebApplication1/Deployment-readme-screenshot/10.png]</p>
  <img width="987" alt="10" src="https://github.com/IvanENERGY/REACT-ASPNETC-MSSQLSERVER-demoCRUD-backend/assets/90034836/84adcf6e-dbda-4c20-aca0-b1b0230a07d6">
 </ol>
 <p>The page should be working on &lt;localip>:&lt;port></p>

 <h2>IF any error exist: </h2>

 <p>Open "Event Viewer" -> choose Window Logs->Application</p>
   <p>[WebApplication1/Deployment-readme-screenshot/12.png]</p>
 <img width="916" alt="12" src="https://github.com/IvanENERGY/REACT-ASPNETC-MSSQLSERVER-demoCRUD-backend/assets/90034836/cb1acfed-676f-43e7-9383-23676c448273">
 <h3>&#10060;Common err 1 : File Storage path not configured appropriately </h3>
   <p>[WebApplication1/Deployment-readme-screenshot/13.png]</p>
 <img width="794" alt="13" src="https://github.com/IvanENERGY/REACT-ASPNETC-MSSQLSERVER-demoCRUD-backend/assets/90034836/1ff51f38-1c7f-4f5e-86fa-8b2289dfe1aa">
 <p>Solution: Since our api use folder path for storage, we need to add those paths to the publish folder as well  </p>
 <p>For example,we need a folder called "Photos" in our program</p>
    <p>[WebApplication1/Deployment-readme-screenshot/14.png]</p>
  <img width="1102" alt="14" src="https://github.com/IvanENERGY/REACT-ASPNETC-MSSQLSERVER-demoCRUD-backend/assets/90034836/9abc980b-95e5-4f74-b8dc-74a87963b4bf">
   <p>We need to add the folder to the published folder inside wwwroot </p>
       <p>[WebApplication1/Deployment-readme-screenshot/15.png]</p>
     <img width="557" alt="15" src="https://github.com/IvanENERGY/REACT-ASPNETC-MSSQLSERVER-demoCRUD-backend/assets/90034836/8cada850-dd17-407c-af55-2dbedaf724a1">
  <h3>&#10060;Common err 2 :SQL server connect unsuccessful </h3>
   <p>[WebApplication1/Deployment-readme-screenshot/17.png]</p>
 <img width="1342" alt="17" src="https://github.com/IvanENERGY/REACT-ASPNETC-MSSQLSERVER-demoCRUD-backend/assets/90034836/a62f8d33-1025-4d6e-a554-25b6424e2e1a">
 <p>Solution: Add the corresponding user in SQL database; In this case, it is "IIS APPPOOL\employeeapi"</p>
 <p>Add the user to both SQL Server Login User & DatabaseUser; Assign db-owner schema for the user </p>
    <p>[WebApplication1/Deployment-readme-screenshot/18.png]</p><img width="345" alt="18" src="https://github.com/IvanENERGY/REACT-ASPNETC-MSSQLSERVER-demoCRUD-backend/assets/90034836/ae1ac19f-a15a-4a96-a795-00bb08b5a3c3">
    <p>[WebApplication1/Deployment-readme-screenshot/19.png]</p><img width="528" alt="19" src="https://github.com/IvanENERGY/REACT-ASPNETC-MSSQLSERVER-demoCRUD-backend/assets/90034836/97cbf286-3c1b-4950-adec-ad5c9a9a6475">
    <p>[WebApplication1/Deployment-readme-screenshot/20.png]</p><img width="200" alt="20" src="https://github.com/IvanENERGY/REACT-ASPNETC-MSSQLSERVER-demoCRUD-backend/assets/90034836/316b152f-7151-4aab-976d-67ecf833146b">
    <p>[WebApplication1/Deployment-readme-screenshot/21.png]</p><img width="527" alt="21" src="https://github.com/IvanENERGY/REACT-ASPNETC-MSSQLSERVER-demoCRUD-backend/assets/90034836/ee674508-3442-4440-87d3-1f22cea5639c">
   <p>[WebApplication1/Deployment-readme-screenshot/22.png]</p> <img width="527" alt="22" src="https://github.com/IvanENERGY/REACT-ASPNETC-MSSQLSERVER-demoCRUD-backend/assets/90034836/2ba106d5-b5c3-4de9-8720-5ec5c9c8f320">
    <p>[WebApplication1/Deployment-readme-screenshot/23.png]</p><img width="512" alt="23" src="https://github.com/IvanENERGY/REACT-ASPNETC-MSSQLSERVER-demoCRUD-backend/assets/90034836/15b41198-233a-4f2d-a437-b6363d4d7675">


 <h2>For other devices on LAN to access API, new firewall rules might need to be added </h2>
 <ol>
 <li>Go to firewall->advanced setting-> add new incoming rules </li>
    <p>[WebApplication1/Deployment-readme-screenshot/24.png]</p><img width="1071" alt="24" src="https://github.com/IvanENERGY/REACT-ASPNETC-MSSQLSERVER-demoCRUD-backend/assets/90034836/b66f4de7-99f2-4759-aee9-d48ba0aff7d8">
    <p>[WebApplication1/Deployment-readme-screenshot/25.png]</p><img width="536" alt="25" src="https://github.com/IvanENERGY/REACT-ASPNETC-MSSQLSERVER-demoCRUD-backend/assets/90034836/6abe9368-493b-4080-8f14-cb3f980e94da">
    <p>[WebApplication1/Deployment-readme-screenshot/26.png]</p><img width="535" alt="26" src="https://github.com/IvanENERGY/REACT-ASPNETC-MSSQLSERVER-demoCRUD-backend/assets/90034836/b13a9b8c-4d2e-4cea-8d2a-4c17e24ba950">
    <p>[WebApplication1/Deployment-readme-screenshot/27.png]</p><img width="535" alt="27" src="https://github.com/IvanENERGY/REACT-ASPNETC-MSSQLSERVER-demoCRUD-backend/assets/90034836/60231d74-a305-4371-a5af-f0793a09bfa8">
    <p>[WebApplication1/Deployment-readme-screenshot/28.png]</p><img width="533" alt="28" src="https://github.com/IvanENERGY/REACT-ASPNETC-MSSQLSERVER-demoCRUD-backend/assets/90034836/4ed49a52-74f0-444d-8ac9-431705ccee0b">
    <p>[WebApplication1/Deployment-readme-screenshot/29.png]</p><img width="533" alt="29" src="https://github.com/IvanENERGY/REACT-ASPNETC-MSSQLSERVER-demoCRUD-backend/assets/90034836/9528eb3f-425f-4319-936c-72cbd93695ff">


  



 </ol>