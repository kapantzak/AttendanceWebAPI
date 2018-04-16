using System;
using System.Collections.Generic;
using System.DrawingCore;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AttendanceWebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using QRCoder;

namespace AttendanceWebApi.Controllers
{
    [Authorize]
    //[Produces("application/json")]
    [Route("api/QRCode")]
    public class QRCodeController : Controller
    {

        private readonly attWebApiContext _context;
        private readonly IConfiguration _configuration;
        private readonly string _connString;

        public QRCodeController(attWebApiContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _connString = _configuration.GetSection("ConnectionStrings").GetSection(_configuration.GetSection("ConnectionStringName").Value).Value;
        }

        /// <summary>
        /// Get QRCode in svg markup for specific CourseAssignmentId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]        
        public string Get(int id)
        {
            var data = new QrCodeData()
            {
                CourseAssignmentId = id,
                Date = DateTime.Now,
                SessionStartTimestamp = DateTime.Now.Ticks
            };
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(Newtonsoft.Json.JsonConvert.SerializeObject(data), QRCodeGenerator.ECCLevel.Q);
            Base64QRCode qrCode = new Base64QRCode(qrCodeData);
            string qrCodeImageAsBase64 = qrCode.GetGraphic(20);
            return qrCodeImageAsBase64;            
        }

    }
}