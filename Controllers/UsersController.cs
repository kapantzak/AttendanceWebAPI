using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AttendanceWebApi.Helpers;
using AttendanceWebApi.Models;
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

    }
}