using System;
using System.Collections.Generic;

namespace AttendanceWebApi.Models
{
    public partial class AttendanceTypes
    {
        public AttendanceTypes()
        {
            AttendanceLog = new HashSet<AttendanceLog>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Descr { get; set; }

        public ICollection<AttendanceLog> AttendanceLog { get; set; }
    }
}
