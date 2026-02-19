using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gridLevel2LL.Data
{
    public class GridDbContextFactory : IDesignTimeDbContextFactory<GridDbContext>
    {
        public GridDbContext CreateDbContext(string[] args)
        {
            


            var optionsBuilder = new DbContextOptionsBuilder<GridDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=SpreadsheetDb;Trusted_Connection=True;TrustServerCertificate=True;");

            return new GridDbContext(optionsBuilder.Options);
        }
    }
}
