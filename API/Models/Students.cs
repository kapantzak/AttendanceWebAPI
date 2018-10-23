using System;
using System.Collections.Generic;

namespace AttendanceWebApi.Models
{
    public partial class Students
    {
        public Students()
        {
            AttendanceLog = new HashSet<AttendanceLog>();
            Enrollments = new HashSet<Enrollments>();
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? UserId { get; set; }

        public ICollection<AttendanceLog> AttendanceLog { get; set; }
        public ICollection<Enrollments> Enrollments { get; set; }
    }
}
