using System;
using System.Collections.Generic;

namespace AttendanceWebApi.Models
{
    public partial class Users
    {
        public Users()
        {
            AttendanceLog = new HashSet<AttendanceLog>();
            CoursesAssignments = new HashSet<CoursesAssignments>();
            Enrollments = new HashSet<Enrollments>();
            UserRoles = new HashSet<UserRoles>();
        }

        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public ICollection<AttendanceLog> AttendanceLog { get; set; }
        public ICollection<CoursesAssignments> CoursesAssignments { get; set; }
        public ICollection<Enrollments> Enrollments { get; set; }
        public ICollection<UserRoles> UserRoles { get; set; }
    }
}
