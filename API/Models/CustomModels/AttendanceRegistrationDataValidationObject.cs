using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceWebApi.Models.CustomModels
{
    public class AttendanceRegistrationDataValidationObject
    {
        public bool valid { get; set; }
        public string error { get; set; }
    }
}
