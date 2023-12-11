using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordGenKeyAPP.Models
{
    public class DBContext : DbContext
    {
        public DbSet<UserEntity> Users { get; set; }
        public DBContext(DbContextOptions options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("SecurePasswordDB");
        }
    }
}
