using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace UOStudio.Web.Data
{
    public static class Module
    {
        public static void AddDatabase(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsBuilder)
        {
            services.AddDbContextFactory<UOStudioDbContext>(optionsBuilder);
        }
    }
}
