using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Auth_Service.Data;

namespace Auth_Service.Data
{
    public static class ServiceMapper
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(ConnectionStringUtil.GetConnectionString(), new MySqlServerVersion(new Version(8, 0, 26)))
            );
        }
    }
}
