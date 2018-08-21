using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DotNet.SampleRestApi.Entities;
using DotNet.SampleRestApi.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotNet.SampleRestApi.Controllers
{

    [Route("api")]
    public class MainController : ControllerBase
    {
        private readonly LogDbContext _logDbContext;


        public MainController(LogDbContext logDbContext)
        {

            _logDbContext = logDbContext;
        }

        [HttpGet]
        public JsonResult Get(string startDate, string endDate)
        {
            var result = Validate(startDate, endDate);

            LogData(startDate, endDate);
            CalculateDate a = new CalculateDate();
            int resultDay = a.calculate(startDate, endDate);

            if (result == "ok")
            {
                
                var response = new
                {
                    day = resultDay
                };
               
                            

                return new JsonResult(response);
            }
            else
            {
                return new JsonResult(result);
            }
        }

       
        private void LogData(string startDate, string endDate)
        {
            DateTime today = DateTime.Today;
            var log = new Log()
            {
                StartDate = Convert.ToDateTime(startDate),
                EndDate = Convert.ToDateTime(endDate),
                ErrorMessage = Validate(startDate, endDate)
            };

            string strPath = Environment.GetFolderPath(
                         System.Environment.SpecialFolder.DesktopDirectory);
            //C de result.txt olusturdugumuzda içine loglarımızı yazıyor
            FileStream fs = new FileStream("C:\\result.txt", FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            sw.BaseStream.Seek(0, SeekOrigin.End);
            sw.WriteLine(today.ToString("dd/MM/yyyy") + "  " + log.StartDate + "   " + log.EndDate + "   " + log.ErrorMessage);

            sw.Flush();
            sw.Close();
        }
        
        private string Validate(string startDate, string endDate)
        {
            DateTime dDate;

            var startDateIsValid = DateTime.TryParseExact(startDate, "dd.MM.yyyy", null, DateTimeStyles.None, out dDate);

            var endDateIsValid = DateTime.TryParseExact(endDate, "dd.MM.yyyy", null, DateTimeStyles.None, out dDate);

            if (!startDateIsValid)
            {
                return "Start Date not valid!";
            }
            if (!endDateIsValid)
            {
                return "End Date not valid!";
            }

            if (Convert.ToDateTime(startDate) >= Convert.ToDateTime(endDate))
            {
                return "Start Date must be less than End Date!";
            }

            return "ok";
        }


    }
}