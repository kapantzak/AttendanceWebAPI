using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AttendanceWebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using AttendanceWebApi.Helpers;
using AttendanceWebApi.Models.CustomModels;

namespace AttendanceWebApi.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/Enrollments")]
    public class EnrollmentsController : Controller
    {
        private readonly attWebApiContext _context;
        private readonly IConfiguration _configuration;
        private readonly string _connString;

        public EnrollmentsController(attWebApiContext context, IConfiguration configuration)
        {            
            _context = context;
            _configuration = configuration;
            _connString = _configuration.GetSection("ConnectionStrings").GetSection(_configuration.GetSection("ConnectionStringName").Value).Value;
        }

        /// <summary>
        /// Get all enrollments
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Enrollments> Get()
        {
            return _context.Enrollments.ToList();
        }

        /// <summary>
        /// Get enrollment by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}", Name = "GetEnrollment")]
        public IActionResult GetById(int id)
        {
            var enrollment = _context.Enrollments.SingleOrDefault(x => x.Id == id);
            if (enrollment == default(Enrollments))
            {
                return NotFound();
            }
            return new ObjectResult(enrollment);
        }

        /// <summary>
        /// Get all enrollments of specific user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetByUserId/{id}")]
        public IActionResult GetByUserId(int id)
        {
            var enrollments = from e in _context.Enrollments
                              join c in _context.Courses on e.CourseId equals c.Id
                              join ca in _context.CoursesAssignments on new { e.CourseId, e.AcademicTermId } equals new { ca.CourseId, ca.AcademicTermId }
                              join u in _context.Users on e.StudentId equals u.Id
                              where u.Id == id
                              select new EnrollmentItem
                              {
                                  Id = e.Id,
                                  StartDate = e.StartDate,
                                  EndDate = e.EndDate,
                                  StudentId = e.StudentId,
                                  FirstName = u.FirstName,
                                  LastName = u.LastName,
                                  CourseId = c.Id,
                                  CourseName = c.Title,
                                  IsActive = c.IsActive,
                                  LecturesMinNum = ca.LecturesMinNum,
                                  LecturesTargetNum = ca.LecturesTargetNum,
                                  LecturesActualNum = _context.LecturesLog.Where(l=>l.CourseAssignmentId == ca.Id).Count(),
                                  Logs = _context.AttendanceLog
                                                .Where(a => a.StudentId == id
                                                    && a.CourseId == c.Id
                                                    && a.AcademicTermId == e.AcademicTermId
                                                    && a.AttendanceTypeId == 0)
                                                .OrderByDescending(x => x.Date).ToList()
                              };
            if (enrollments == null)
            {
                return NotFound();
            }
            return new ObjectResult(enrollments);
        }

        /// <summary>
        /// Create new enrollment and return record
        /// </summary>
        /// <param name="enrollment"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Create([FromBody] Enrollments enrollment)
        {
            if (enrollment == null)
            {
                return BadRequest();
            }
            enrollment.Id = AdoHelper.GetNextId("Enrollments", _connString);
            _context.Enrollments.Add(enrollment);            
            _context.SaveChanges();
            return CreatedAtRoute("GetEnrollment", new { id = enrollment.Id }, enrollment);
        }

        /// <summary>
        /// Update enrollment record by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="enrollment"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Enrollments enrollment)
        {
            if (enrollment == null || enrollment.Id != id)
            {
                return BadRequest();
            }

            var enr = _context.Enrollments.SingleOrDefault(x => x.Id == id);
            if (enr == default(Enrollments))
            {
                return NotFound();
            }

            enr.StudentId = enrollment.StudentId;
            enr.CourseId = enrollment.CourseId;
            enr.AcademicTermId = enrollment.AcademicTermId;
            enr.StartDate = enrollment.StartDate;
            enr.EndDate = enrollment.EndDate;
            enr.Notes = enrollment.Notes;

            _context.Enrollments.Update(enr);
            _context.SaveChanges();
            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var enr = _context.Enrollments.SingleOrDefault(x => x.Id == id);
            if (enr == default(Enrollments))
            {
                return NotFound();
            }

            _context.Enrollments.Remove(enr);
            _context.SaveChanges();
            return new NoContentResult();
        }
    }
}