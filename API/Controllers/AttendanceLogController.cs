using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AttendanceWebApi.Models;
using AttendanceWebApi.Helpers;
using Microsoft.Extensions.Configuration;
using AttendanceWebApi.Models.CustomModels;

namespace AttendanceWebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/AttendanceLog")]
    public class AttendanceLogController : Controller
    {
        private readonly attWebApiContext _context;
        private readonly IConfiguration _configuration;
        private readonly string _connString;

        public AttendanceLogController(attWebApiContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _connString = _configuration.GetSection("ConnectionStrings").GetSection(_configuration.GetSection("ConnectionStringName").Value).Value;
        }
                
        public IEnumerable<AttendanceLog> Get()
        {
            return _context.AttendanceLog.ToList();
        }
                
        [HttpGet("{id}", Name = "GetLog")]
        public IActionResult GetById(int id)
        {
            var log = _context.AttendanceLog.SingleOrDefault(x => x.Id == id);
            if (log == default(AttendanceLog))
            {
                return NotFound();
            }
            return new ObjectResult(log);
        }
                
        [HttpGet("GetStudent/{studentId}")]
        public IEnumerable<AttendanceLog> GetStudentAttendance(int studentId)
        {
            return _context.AttendanceLog.Where(x => x.StudentId == studentId);            
        }
                
        [HttpGet("GetCourse/{courseId}")]
        public IEnumerable<AttendanceLog> GetCourseAttendance(int courseId)
        {
            return _context.AttendanceLog.Where(x => x.CourseId == courseId);
        }
                
        [HttpGet("GetAcademicTerm/{academicTermId}")]
        public IEnumerable<AttendanceLog> GetAcademicTermAttendance(int academicTermId)
        {
            return _context.AttendanceLog.Where(x => x.AcademicTermId == academicTermId);
        }

        [HttpGet("GetEnrollmentAttendanceLogs/{enrollmentId}")]
        public IActionResult GetEnrollmentAttendance(int enrollmentId)
        {
            var enrollment = _context.Enrollments.SingleOrDefault(x => x.Id == enrollmentId);
            if (enrollment != default(Enrollments))
            {
                var logs = LogHelper.GetAttendanceLogsForEnrollment(enrollmentId, _context);
                return new ObjectResult(logs);
            }
            else
            {
                return NotFound();
            }
        }
                        
        [HttpPost]
        public IActionResult Create([FromBody] AttendanceRegistrationData data)
        {
            var dataValidation = LogHelper.ValidateRegistrationData(data, this._context, this._configuration);
            if (data == null)
            {
                return BadRequest();
            }
            else if (!dataValidation.valid)
            {
                if (!string.IsNullOrWhiteSpace(dataValidation.error))
                {
                    return Content($"Error: {dataValidation.error}");
                }
                return Content("Error: Invalid data");
            }

            data.Attendance.Longitude = data.GeoLon;
            data.Attendance.Latitude = data.GeoLat;
            data.Attendance.Timestamp = data.SessionStartTimestamp;

            // Remove existing log in same date
            var existingLogs = LogHelper.GetDuplicatedLog(data.Attendance, _context);
            if (existingLogs.Count() > 0)
            {
                _context.AttendanceLog.RemoveRange(existingLogs);
                _context.SaveChanges();
            }
            
            var log = data.Attendance;
            log.Id = AdoHelper.GetNextId("AttendanceLog", _connString);
            _context.AttendanceLog.Add(log);
            _context.SaveChanges();
            return CreatedAtRoute("GetLog", new { id = log.Id }, log);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] AttendanceLog log)
        {
            if (log == null || log.Id != id)
            {
                return BadRequest();
            }
            
            var thisLog = _context.AttendanceLog.SingleOrDefault(x => x.Id == id);
            if (thisLog == default(AttendanceLog))
            {
                return NotFound();
            }
            
            thisLog.StudentId = log.StudentId;
            thisLog.CourseId = log.CourseId;
            thisLog.AcademicTermId = log.AcademicTermId;
            thisLog.Date = log.Date;
            thisLog.AttendanceTypeId = log.AttendanceTypeId;

            _context.AttendanceLog.Update(thisLog);
            _context.SaveChanges();
            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var log = _context.AttendanceLog.SingleOrDefault(x => x.Id == id);
            if (log == default(AttendanceLog))
            {
                return NotFound();
            }

            _context.AttendanceLog.Remove(log);
            _context.SaveChanges();
            return new NoContentResult();
        }
                
    }
}