using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AttendanceWebApi.Models;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using AttendanceWebApi.Helpers;
using System.Data;
using AttendanceWebApi.Models.CustomModels;

namespace AttendanceWebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/Courses")]
    public class CoursesController : Controller
    {
        private readonly attWebApiContext _context;
        private readonly IConfiguration _configuration;
        private readonly string _connString;

        public CoursesController(attWebApiContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _connString = _configuration.GetSection("ConnectionStrings").GetSection(_configuration.GetSection("ConnectionStringName").Value).Value;
        }

        public IEnumerable<Courses> Get()
        {
            return _context.Courses.ToList();
        }

        /// <summary>
        /// Get all active courses for specific professor and academic term
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("GetCurrent")]
        public IEnumerable<CourseInstance> GetCurrent([FromBody] RequestObject req)
        {
            var courses = new List<CourseInstance>();
            var query = @"DECLARE @ProfessorId INT;
                        SET @ProfessorId = @Set_ProfessorId;
                        SELECT c.ID
	                        ,c.Title
	                        ,c.Descr
	                        ,c.IsActive
                            ,ca.ID AS CourseAssignmentId
                        FROM Courses c
                        JOIN CoursesAssignments ca ON c.ID = ca.CourseID
                        JOIN AcademicTerms at ON at.ID = ca.AcademicTermID
                        WHERE ca.ProfessorID = @ProfessorId
                        AND c.IsActive = 1
                        AND GETDATE() BETWEEN at.StartDate AND at.EndDate";
            var cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("@Set_ProfessorId", req.Id);
            var sdt = AdoHelper.GetDataTable(cmd, _connString);
            if (!sdt.ErrorFound)
            {
                foreach (DataRow dr in sdt.DataTable.Rows)
                {
                    var c = new CourseInstance();
                    int id;
                    if (int.TryParse(dr["ID"].ToString(), out id))
                    {
                        c.Id = id;
                    }
                    c.Title = dr["Title"].ToString();
                    c.Descr = dr["Descr"].ToString();
                    c.IsActive = dr["IsActive"].ToString() == "1";

                    int caid;
                    if (int.TryParse(dr["CourseAssignmentId"].ToString(), out caid))
                    {
                        c.CourseAssignmentId = caid;
                    }
                    courses.Add(c);
                }
            }
            return courses;            
        }
    }
}