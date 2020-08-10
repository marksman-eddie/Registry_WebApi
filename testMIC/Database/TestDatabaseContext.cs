using System;
using Microsoft.EntityFrameworkCore;
using testMIC.Database.Model;

namespace testMIC.Database
{
    public class TestDatabaseContext:DbContext
    {
        public TestDatabaseContext(DbContextOptions<TestDatabaseContext> options):base(options)
        {
        }

        public DbSet<Appointment> Appointment { get; set; }
        public DbSet<Event> Event { get; set; }
        public DbSet<Patient> Patient { get; set; }
        public DbSet<Service> Service { get; set; }

    }
}
