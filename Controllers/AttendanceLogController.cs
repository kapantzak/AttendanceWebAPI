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

        /// <summary>
        /// Get all attendance logs
        /// </summary>
        /// <returns></returns>
        public IEnumerable<AttendanceLog> Get()
        {
            return _context.AttendanceLog.ToList();
        }

        /// <summary>
        /// Get attendance log by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Get student's attendance logs
        /// </summary>
        /// <param name="studentId"></param>
        /// <returns></returns>
        [HttpGet("GetStudent/{studentId}")]
        public IEnumerable<AttendanceLog> GetStudentAttendance(int studentId)
        {
            return _context.AttendanceLog.Where(x => x.StudentId == studentId);            
        }

        /// <summary>
        /// Get attendance logs for specific course
        /// </summary>
        /// <param name="courseId"></param>
        /// <returns></returns>
        [HttpGet("GetCourse/{courseId}")]
        public IEnumerable<AttendanceLog> GetCourseAttendance(int courseId)
        {
            return _context.AttendanceLog.Where(x => x.CourseId == courseId);
        }

        /// <summary>
        /// Get attendance logs for specific academic term
        /// </summary>
        /// <param name="academicTermId"></param>
        /// <returns></returns>
        [HttpGet("GetAcademicTerm/{academicTermId}")]
        public IEnumerable<AttendanceLog> GetAcademicTermAttendance(int academicTermId)
        {
            return _context.AttendanceLog.Where(x => x.AcademicTermId == academicTermId);
        }

        /// <summary>
        /// Create new log and return record
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Create([FromBody] AttendanceLog log)
        {
            if (log == null || !this.ValidateNewAttendanceLog(log))
            {
                return BadRequest();
            }
            log.Id = AdoHelper.GetNextId("AttendanceLog", _connString);
            _context.AttendanceLog.Add(log);
            _context.SaveChanges();
            return CreatedAtRoute("GetLog", new { id = log.Id }, log);
        }

        /// <summary>
        /// Try to register attendance
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult RegisterAttendance([FromBody] AttendanceRegistrationData data)
        {
            if (data == null)
            {
                return BadRequest();
            }
            else if (!this.ValidateRegistrationData(data))
            {
                return Content("Invalid data");
            }
            var log = data.Attendance;
            log.Id = AdoHelper.GetNextId("AttendanceLog", _connString);
            _context.AttendanceLog.Add(log);
            _context.SaveChanges();
            return CreatedAtRoute("GetLog", new { id = log.Id }, log);
        }

        /// <summary>
        /// Update attendance log
        /// </summary>
        /// <param name="id"></param>
        /// <param name="log"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Delete specific attendance log record
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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


        // HELPERS =================================================================== //

        /// <summary>
        /// Validate user defined attendance log record
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        private bool ValidateNewAttendanceLog(AttendanceLog log)
        {            
            if (
                log.Id != 0 && _context.AttendanceLog.Any(x=>x.Id == log.Id)
                || !_context.Users.Any(x => x.Id == log.StudentId)
                || !_context.Courses.Any(x => x.Id == log.CourseId)
                || !_context.AcademicTerms.Any(x => x.Id == log.AcademicTermId)
                || !_context.AttendanceTypes.Any(x => x.Id == log.AttendanceTypeId)
                )
            {
                return false;
            }
            return true;
        }

        private bool ValidateRegistrationData(AttendanceRegistrationData data)
        {
            var valid = true;
            if (!this.ValidateNewAttendanceLog(data.Attendance))
            {
                valid = false;
            }
            else
            {
                // Validate location and timestamp
                if (data.CoursesAssignmentID != null)
                {
                    // Classroom's coordinates
                    var classRoom = (from c in _context.ClassRooms
                                     join ca in _context.CoursesAssignments on c.Id equals ca.ClassRoomId
                                     where ca.Id == data.CoursesAssignmentID
                                     select c).SingleOrDefault();

                    if (classRoom != default(ClassRooms) && classRoom.Longitude != null && classRoom.Latitude != null && classRoom.RangeInMeters != null)
                    {
                        double thisLon = (double)classRoom.Longitude;
                        double thisLat = (double)classRoom.Latitude;
                        var distanceBuffer = (double)classRoom.RangeInMeters;
                                                
                        var minutesBuffer = _configuration.GetSection("App").GetSection("RegisterAttendance").GetSection("TimeBufferInMinutes").Value;
                        var distance = this.distanceInMeters(thisLat, thisLon, data.GeoLat, data.GeoLon);

                        if (distance > distanceBuffer || TimeSpan.FromTicks(data.Timestamp - data.SessionStartTimestamp).TotalMinutes > Convert.ToInt32(minutesBuffer))
                        {
                            valid = false;
                        }
                    }
                    else
                    {
                        valid = false;
                    }
                }
                else
                {
                    valid = false;
                }
                
            }
            return valid;
        }

        /// <summary>
        /// Get distance in meters between to coordinates
        /// [https://www.geodatasource.com/developers/c-sharp]
        /// </summary>
        /// <param name="lat1"></param>
        /// <param name="lon1"></param>
        /// <param name="lat2"></param>
        /// <param name="lon2"></param>
        /// <returns></returns>
        private double distanceInMeters(double lat1, double lon1, double lat2, double lon2)
        {
            double theta = lon1 - lon2;
            double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
            dist = Math.Acos(dist);
            dist = rad2deg(dist);
            dist = dist * 60 * 1.1515;
            dist = dist * 1609.344;            
            return (dist);
        }

        /// <summary>
        /// Convert decimal degrees to radians
        /// </summary>
        /// <param name="deg"></param>
        /// <returns></returns>
        private double deg2rad(double deg) => (deg * Math.PI / 180.0);

        /// <summary>
        /// Convert radians to decimal degrees
        /// </summary>
        /// <param name="rad"></param>
        /// <returns></returns>
        private double rad2deg(double rad) => (rad / Math.PI * 180.0);
    }
}