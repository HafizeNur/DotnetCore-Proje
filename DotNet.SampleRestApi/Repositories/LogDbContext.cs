using DotNet.SampleRestApi.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNet.SampleRestApi.Repositories
{
    //Veri tabanı işlemleri için DbContexten bir context sınıfı türetmeliyiz.
    public class LogDbContext : DbContext
    {
        public LogDbContext(DbContextOptions<LogDbContext>options)
            :base(options)
        {

        }
        //Veri tabanı için olusturdugumuz classlarımızı tablo olarak burada tanıtıyoruz.
       
        public DbSet<Log> Logs { get; set; }
        
    }
}
