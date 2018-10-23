using AttendanceWebApi.Models;
using AttendanceWebApi.Models.CustomModels;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceWebApi.Helpers
{
    public static class LogHelper
    {
        
        public static bool ValidateNewAttendanceLog(AttendanceLog log, attWebApiContext context)
        {
            if (
                log.Id != 0 && context.AttendanceLog.Any(x => x.Id == log.Id)
                || !context.Users.Any(x => x.Id == log.StudentId)
                || !context.Courses.Any(x => x.Id == log.CourseId)
                || !context.AcademicTerms.Any(x => x.Id == log.AcademicTermId)
                || !context.AttendanceTypes.Any(x => x.Id == log.AttendanceTypeId)
                )
            {
                return false;
            }
            return true;
        }

        public static AttendanceRegistrationDataValidationObject ValidateRegistrationData(AttendanceRegistrationData data, attWebApiContext context, IConfiguration configuration)
        {            
            var valid = true;
            var error = string.Empty;
            if (!LogHelper.ValidateNewAttendanceLog(data.Attendance, context))
            {
                valid = false;
            }
            else
            {
                // Validate location and timestamp
                if (data.CoursesAssignmentID != null)
                {
                    // Classroom's coordinates
                    var classRoom = (from c in context.ClassRooms
                                     join ca in context.CoursesAssignments on c.Id equals ca.ClassRoomId
                                     where ca.Id == data.CoursesAssignmentID
                                     select c).SingleOrDefault();

                    if (classRoom != default(ClassRooms) && classRoom.Longitude != null && classRoom.Latitude != null && classRoom.RangeInMeters != null)
                    {
                        double thisLon = (double)classRoom.Longitude;
                        double thisLat = (double)classRoom.Latitude;
                        var distanceBuffer = (double)classRoom.RangeInMeters;

                        var minutesBuffer = configuration.GetSection("App").GetSection("RegisterAttendance").GetSection("TimeBufferInMinutes").Value;
                        var distance = LogHelper.DistanceInMeters(thisLat, thisLon, data.GeoLat, data.GeoLon);

                        var enrollments = GetEnrollmentsForUserId(data.Attendance.StudentId, context);

                        if (distance > distanceBuffer)
                        {
                            valid = false;
                            error = "Your device is detected too far from the classroom!";
                        }
                        else if (TimeSpan.FromTicks(DateTime.Now.Ticks - data.SessionStartTimestamp).TotalMinutes > Convert.ToInt32(minutesBuffer))
                        {
                            valid = false;
                            error = $"It's been over {minutesBuffer} minutes since the beginning of the course";
                        }
                        else if (!enrollments.Any(e=>e.CourseId == data.Attendance.CourseId))
                        {
                            valid = false;
                            error = $"You are not enrolled to {context.Courses.SingleOrDefault(c=>c.Id == data.Attendance.CourseId).Title}";
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
            return new AttendanceRegistrationDataValidationObject {
                valid = valid,
                error = error
            };
        }

        private static IEnumerable<Enrollments> GetEnrollmentsForUserId(int userId, attWebApiContext context)
        {
            return from e in context.Enrollments
                   join c in context.Courses on e.CourseId equals c.Id
                   join ca in context.CoursesAssignments on new { e.CourseId, e.AcademicTermId } equals new { ca.CourseId, ca.AcademicTermId }
                   join u in context.Users on e.StudentId equals u.Id
                   where u.Id == userId
                   select e;
        }

        public static IEnumerable<AttendanceLog> GetDuplicatedLog(AttendanceLog newLog, attWebApiContext context)
        {
            return context.AttendanceLog.Where(a => a.StudentId == newLog.StudentId && a.CourseId == newLog.CourseId && a.AcademicTermId == newLog.AcademicTermId && a.Date == newLog.Date);            
        }

        public static EnrollmentAttendanceLogs GetAttendanceLogsForEnrollment(int enrollmentId, attWebApiContext context)
        {
            var logs = (from l in context.AttendanceLog
                        join e in context.Enrollments on new { l.StudentId, l.CourseId, l.AcademicTermId } equals new { e.StudentId, e.CourseId, e.AcademicTermId }                        
                        where e.Id == enrollmentId
                        select l).ToList();
            var courseAssignment = (from ca in context.CoursesAssignments
                                    join c in context.Courses on ca.CourseId equals c.Id                                    
                                    join e in context.Enrollments on new { ca.CourseId, ca.AcademicTermId } equals new { e.CourseId, e.AcademicTermId }
                                    where e.Id == enrollmentId
                                    select new {
                                        ca.Id,
                                        ca.LecturesMinNum,
                                        ca.LecturesTargetNum,
                                        CourseTitle = c.Title
                                    }).SingleOrDefault();            
            var enrollmentLogs = new EnrollmentAttendanceLogs
            {
                Logs = logs,                
                CourseTitle = courseAssignment.CourseTitle,
                LecturesMinNum = courseAssignment.LecturesMinNum,
                LecturesTargetNum = courseAssignment.LecturesTargetNum,
                LecturesActualNum = context.LecturesLog.Where(l => l.CourseAssignmentId == courseAssignment.Id).Count(),
                Date = DateTime.Now
            };
            return enrollmentLogs;
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
        public static double DistanceInMeters(double lat1, double lon1, double lat2, double lon2)
        {
            double theta = lon1 - lon2;
            double dist = Math.Sin(LogHelper.Deg2rad(lat1)) * Math.Sin(LogHelper.Deg2rad(lat2)) + Math.Cos(LogHelper.Deg2rad(lat1)) * Math.Cos(LogHelper.Deg2rad(lat2)) * Math.Cos(LogHelper.Deg2rad(theta));
            dist = Math.Acos(dist);
            dist = LogHelper.Rad2deg(dist);
            dist = dist * 60 * 1.1515;
            dist = dist * 1609.344;
            return (dist);
        }

        /// <summary>
        /// Convert decimal degrees to radians
        /// </summary>
        /// <param name="deg"></param>
        /// <returns></returns>
        public static double Deg2rad(double deg) => (deg * Math.PI / 180.0);

        /// <summary>
        /// Convert radians to decimal degrees
        /// </summary>
        /// <param name="rad"></param>
        /// <returns></returns>
        public static double Rad2deg(double rad) => (rad / Math.PI * 180.0);
    }
}
