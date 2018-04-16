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
    [Route("api/AcademicTermTypes")]
    public class AcademicTermTypesController : Controller
    {
        private readonly attWebApiContext _context;

        public AcademicTermTypesController(attWebApiContext context)
        {
            _context = context;
        }

        public IEnumerable<AcademicTermTypes> Get()
        {
            return _context.AcademicTermTypes.ToList();
        }
    }
}