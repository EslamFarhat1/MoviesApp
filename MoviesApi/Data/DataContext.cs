using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Entities;

namespace MoviesApi.Data
{
    public class DataContext:IdentityDbContext<ApplicationUser,ApplicationRole,string>
    {
        public DataContext(DbContextOptions<DataContext> options):base(options)
        {
            
        }
    }
}