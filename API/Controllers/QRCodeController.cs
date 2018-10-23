using System;
using System.Collections.Generic;
using System.DrawingCore;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AttendanceWebApi.Helpers;
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
        /// Get QRCode for specific CourseAssignmentId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]        
        public string Get(int id)
        {
            var courceAssignment = _context.CoursesAssignments.SingleOrDefault(x => x.Id == id);
            if (courceAssignment != null)
            {
                
                // Try create new lecture (One per day && courseAssignmentId)
                var now = DateTime.Now;
                var today = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
                int? lectureLogId = null;
                var existingLecture = _context.LecturesLog.Where(x => x.CourseAssignmentId == id && x.Date == today);
                if (existingLecture.Count() == 0)
                {
                    try
                    {
                        var newLecture = new LecturesLog();
                        newLecture.Id = AdoHelper.GetNextId("LecturesLog", _connString);
                        newLecture.CourseAssignmentId = id;
                        newLecture.Date = today;
                        _context.LecturesLog.Add(newLecture);
                        _context.SaveChanges();
                        lectureLogId = newLecture.Id;
                    }
                    catch
                    {
                        return "Error";
                    }
                }
                else
                {
                    lectureLogId = existingLecture.FirstOrDefault().Id;
                }

                // Create QR code
                var data = new QrCodeData()
                {
                    CourseAssignmentId = id,
                    LectureLogId = lectureLogId,
                    CourseId = courceAssignment.CourseId,
                    AcademicTermId = courceAssignment.AcademicTermId,
                    Date = DateTime.Now.ToString("yyyy-MM-dd"),
                    SessionStartTimestamp = DateTime.Now.Ticks
                };
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(Newtonsoft.Json.JsonConvert.SerializeObject(data), QRCodeGenerator.ECCLevel.Q);
                Base64QRCode qrCode = new Base64QRCode(qrCodeData);
                string qrCodeImageAsBase64 = qrCode.GetGraphic(20);

                return qrCodeImageAsBase64;
            }
            return "Error";
        }

    }
}