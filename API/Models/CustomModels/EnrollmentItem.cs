using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceWebApi.Models.CustomModels
{
    public class EnrollmentItem
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int StudentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public bool? IsActive { get; set; }
        public int? LecturesMinNum { get; set; }
        public int? LecturesTargetNum { get; set; }
        public int? LecturesActualNum { get; set; }
        public List<AttendanceLog> Logs { get; set; }
    }
}