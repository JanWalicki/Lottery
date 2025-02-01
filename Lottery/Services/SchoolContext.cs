using Lottery.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery.Services
{
    public class SchoolContext : DbContext
    {
        public DbSet<Class> Classes { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<LuckyNumber> LuckyNumbers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=Database3.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Class>()
                .HasMany(c => c.Students)
                .WithOne()
                .HasForeignKey(s => s.ClassId);

            modelBuilder.Entity<Student>()
                .Property(s => s.LastPicked)
                .HasDefaultValue((byte)0);

            modelBuilder.Entity<Student>()
                .Property(s => s.Present)
                .HasDefaultValue(true);
        }
    }
}
