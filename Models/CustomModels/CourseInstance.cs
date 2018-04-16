using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceWebApi.Models.CustomModels
{
    public class CourseInstance
    {
        public int? Id { get; set; }
        public string Title { get; set; }
        public string Descr { get; set; }
        public bool? IsActive { get; set; }
        public int? CourseAssignmentId { get; set; }
    }
}
