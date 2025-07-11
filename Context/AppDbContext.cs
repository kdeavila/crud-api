using crud_api.Entities;
using Microsoft.EntityFrameworkCore;

namespace crud_api.Context;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Profile> Profiles { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Profile>(table =>
        {
            table.HasKey(col => col.Id);
            table.Property(col => col.Id).UseIdentityColumn().ValueGeneratedOnAdd();
            table.Property(col => col.Name).HasMaxLength(50);
            table.ToTable("Profile");
            table.HasData(
                new Profile { Id = 1, Name = "Programador Junior" },
                new Profile { Id = 2, Name = "Programador Senior" },
                new Profile { Id = 3, Name = "Analista de Sistemas" },
                new Profile { Id = 4, Name = "Analista de Datos" }
            );
        });

        modelBuilder.Entity<Employee>(table =>
        {
            table.HasKey(col => col.Id);
            table.Property(col => col.Id).UseIdentityColumn().ValueGeneratedOnAdd();
            table.Property(col => col.FullName).HasMaxLength(50);
            table.HasOne(col => col.ProfileReference)
                .WithMany(p => p.EmployeesReference)
                .HasForeignKey(col => col.IdProfile)
                .OnDelete(DeleteBehavior.Restrict);
            table.ToTable("Employee");
        });
        
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
    }
}