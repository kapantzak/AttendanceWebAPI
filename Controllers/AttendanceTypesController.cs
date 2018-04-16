using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AttendanceWebApi.Models;

namespace AttendanceWebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/AttendanceTypes")]
    public class AttendanceTypesController : Controller
    {
        private readonly attWebApiContext _context;

        public AttendanceTypesController(attWebApiContext context)
        {
            _context = context;
        }

        public IEnumerable<AttendanceTypes> Get()
        {
            return _context.AttendanceTypes.ToList();
        }
    }
}