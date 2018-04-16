using System;
using System.Collections.Generic;

namespace AttendanceWebApi.Models
{
    public partial class AcademicTermTypes
    {
        public AcademicTermTypes()
        {
            AcademicTerms = new HashSet<AcademicTerms>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Descr { get; set; }

        public ICollection<AcademicTerms> AcademicTerms { get; set; }
    }
}
