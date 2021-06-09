using System;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<AppUser> AppUsers { get; set; }

        internal object ToList()
        {
            throw new NotImplementedException();
        }
    }
}