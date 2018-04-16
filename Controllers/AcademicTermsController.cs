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
    [Route("api/AcademicTerms")]
    public class AcademicTermsController : Controller
    {
        private readonly attWebApiContext _context;

        public AcademicTermsController(attWebApiContext context)
        {
            _context = context;
        }

        public IEnumerable<AcademicTerms> Get()
        {
            return _context.AcademicTerms.ToList();
        }
    }
}