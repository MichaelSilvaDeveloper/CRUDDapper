using CRUDDapper.API.Entities;
using CRUDDapper.API.Models;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace CRUDDapper.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly string _connectionString;
        public StudentsController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DataBase");
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                const string sql = "SELECT * FROM Students";
                var student = await sqlConnection.QueryAsync<Student>(sql);
                return Ok(student);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var parameters = new
            {
                id
            };

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                const string sql = "SELECT * FROM Students WHERE Id = @id";
                var student = await sqlConnection.QuerySingleOrDefaultAsync<Student>(sql, parameters);

                if(student is null)
                {
                    return NotFound();
                }

                return Ok(student);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post(StudentInputModel studentInputModel)
        {
            var student = new Student(studentInputModel.FullName, studentInputModel.SchoolClass);

            var parameters = new
            {
                student.FullName,
                //student.BirthDate,
                student.SchoolClass,
                //student.IsActive
            };

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                const string sql = "INSERT INTO Students OUTPUT INSERTED.Id VALUES (@FullName, @SchoolClass)";

                var id = await sqlConnection.ExecuteScalarAsync<int>(sql, parameters);

                return Ok(id);
            }

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, StudentInputModel studentInputModel)
        {      
            var parameters = new
            {
                id,
                studentInputModel.FullName,
                //studentInputModel.BirthDate,
                studentInputModel.SchoolClass,
            };

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                const string sql = "UPDATE Students SET FullName = @FullName, SchoolClass = @SchoolClass WHERE Id = @id";

                await sqlConnection.ExecuteAsync(sql, parameters);

                return NoContent();
            }

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var parameters = new
            {
                id
            };

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                const string sql = "Delete from Students WHERE Id = @id";

                await sqlConnection.ExecuteAsync(sql, parameters);

                return NoContent();
            }
        }
    }
}