using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using AttendanceWebApi.Helpers;
using AttendanceWebApi.Models;
using AttendanceWebApi.Models.CustomModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace AttendanceWebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/Users")]
    public class UsersController : Controller
    {
        private readonly attWebApiContext _context;
        private readonly IConfiguration _configuration;
        private readonly string _connString;

        public UsersController(attWebApiContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _connString = _configuration.GetSection("ConnectionStrings").GetSection(_configuration.GetSection("ConnectionStringName").Value).Value;
        }

        /// <summary>
        /// Get list of all users
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Users> Get()
        {
            return _context.Users;
        }

        /// <summary>
        /// Get user by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var user = _context.Users.SingleOrDefault(x => x.Id == id);
            if (user == default(Users))
            {
                return NotFound();
            }
            return new ObjectResult(user);
        }

        /// <summary>
        /// Get list of users that are students (roleId = 3)
        /// </summary>
        /// <returns></returns>
        [HttpGet("Students")]
        public IEnumerable<Users> GetStudents()
        {
            return this.GetUsersByRoleId(3);
        }

        /// <summary>
        /// Get list of students for specific professor and the current academic term
        /// </summary>
        /// <param name="professorId"></param>        
        /// <returns></returns>
        [HttpGet("ProfessorsStudents/{professorId}")]
        public IEnumerable<Users> GetProfessorsStudents(int professorId)
        {
            var academicTermId = this.GetCurrentAcademicTermId();
            if (academicTermId != null)
            {
                return this.GetStudentsByProfessorId(professorId, (int)academicTermId);
            }
            return null;
        }

        /// <summary>
        /// Get professor's assigned data (courses and students)
        /// </summary>
        /// <param name="professorId"></param>        
        /// <returns></returns>
        [HttpGet("ProfessorsAssignedData/{professorId}")]
        public IActionResult GetProfessorsAssignedData(int professorId)
        {
            var academicTermId = this.GetCurrentAcademicTermId();
            if (academicTermId != null)
            {
                var data = this.ProfessorsAssignedData(professorId, (int)academicTermId);
                return new ObjectResult(data);
            }
            return NotFound();
        }

        /// <summary>
        /// Get list of users that are professors (roleId = 2)
        /// </summary>
        /// <returns></returns>
        [HttpGet("Professors")]
        public IEnumerable<Users> GetProfessors()
        {
            return this.GetUsersByRoleId(2);
        }

        /// <summary>
        /// Create new user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Create([FromBody] Users user)
        {
            if (user == null)
            {
                return BadRequest();
            }
            user.Id = AdoHelper.GetNextId("Users", _connString);
            _context.Users.Add(user);
            _context.SaveChanges();
            return CreatedAtRoute("GetById", new { id = user.Id }, user);
        }

        /// <summary>
        /// Update user record by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Users user)
        {
            if (user == null || user.Id != id)
            {
                return BadRequest();
            }

            var usr = _context.Users.SingleOrDefault(x => x.Id == id);
            if (usr == default(Users))
            {
                return NotFound();
            }

            if (!string.IsNullOrWhiteSpace(user.Username))
            {
                usr.Username = user.Username;
            }
            
            if (!string.IsNullOrWhiteSpace(usr.Password))
            {
                usr.Password = user.Password;
            }
            
            usr.FirstName = user.FirstName;
            usr.LastName = user.LastName;

            _context.Users.Update(usr);
            _context.SaveChanges();
            return new NoContentResult();
        }

        /// <summary>
        /// Delete user by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var usr = _context.Users.SingleOrDefault(x => x.Id == id);
            if (usr == default(Users))
            {
                return NotFound();
            }

            _context.Users.Remove(usr);
            _context.SaveChanges();
            return new NoContentResult();
        }

        // HELPERS ============================================================================================ //

        /// <summary>
        /// Get list of users by role id
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        private IEnumerable<Users> GetUsersByRoleId(int roleId) => from u in _context.Users
                                                                   join ur in _context.UserRoles on u.Id equals ur.UserId
                                                                   join r in _context.Roles on ur.RoleId equals r.Id
                                                                   where r.Id == roleId
                                                                   select u;

        /// <summary>
        /// Get the current academic term id
        /// </summary>
        /// <returns></returns>
        private int? GetCurrentAcademicTermId()
        {
            var query = @"select a.ID from AcademicTerms a
                        where GETDATE() between a.StartDate and a.EndDate";
            var cmd = new SqlCommand(query);
            var sdt = AdoHelper.GetDataTable(cmd, _connString);
            if (!sdt.ErrorFound && sdt.DataTable.Rows.Count > 0)
            {
                var termId = sdt.DataTable.Rows[0]["ID"].ToString();
                int id;
                if (int.TryParse(termId, out id))
                {
                    return id;
                }
                return null;
            }
            return null;
        }

        /// <summary>
        /// Get list of students for specific professor and academic term
        /// </summary>
        /// <param name="professorId"></param>
        /// <param name="academicTermId"></param>
        /// <returns></returns>
        private IEnumerable<Users> GetStudentsByProfessorId(int professorId, int academicTermId) => (from ca in _context.CoursesAssignments
                                                                                                     join e in _context.Enrollments on ca.CourseId equals e.CourseId
                                                                                                     join u in _context.Users on e.StudentId equals u.Id
                                                                                                     where ca.ProfessorId == professorId && e.AcademicTermId == academicTermId
                                                                                                     select u).Distinct();
        
        /// <summary>
        /// Get list of assigned courses and enrolled students for specific professor and academic term
        /// </summary>
        /// <param name="professorId"></param>
        /// <param name="academicTermId"></param>
        /// <returns></returns>
        private object ProfessorsAssignedData(int professorId, int academicTermId) => new
        {
            ProfessorID = professorId,
            Courses = (from ca in _context.CoursesAssignments
                       join e in _context.Enrollments on ca.CourseId equals e.CourseId
                       join c in _context.Courses on e.CourseId equals c.Id
                       where ca.ProfessorId == professorId && ca.AcademicTermId == academicTermId
                       group c by c.Id into g
                       select new
                       {
                           Id = g.Key,
                           _context.Courses.SingleOrDefault(x => x.Id == g.Key).Title,
                           Students = (from se in _context.Enrollments
                                       join su in _context.Users on se.StudentId equals su.Id
                                       where se.CourseId == g.Key && se.AcademicTermId == academicTermId
                                       select su).ToList()
                       }).ToList()
        };

    }
}