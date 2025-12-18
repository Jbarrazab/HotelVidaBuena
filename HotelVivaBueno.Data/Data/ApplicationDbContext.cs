using HotelVivaBueno.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace HotelVivaBueno.Data.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure interactions
            modelBuilder.Entity<Department>()
                .HasMany(d => d.Employees)
                .WithOne(e => e.Department)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.Email)
                .IsUnique();

            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.DocumentNumber)
                .IsUnique();

            // Store Enums as Strings for readability (Optional, but strict "Clean" usually prefers int. 
            // However, Supabase/Admin ease often benefits from string. I'll stick to default INT for simplicity unless requested otherwise).
            
            // Seed
            modelBuilder.Entity<Department>().HasData(
                new Department { Id = 1, Name = "Recursos Humanos", Description = "Gestión de personal" },
                new Department { Id = 2, Name = "Tecnología", Description = "Sistemas y desarrollo" },
                new Department { Id = 3, Name = "Operaciones", Description = "Logística y mantenimiento" }
            );
        }
    }
}
