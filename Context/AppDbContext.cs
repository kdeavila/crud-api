using crud_api.Entities;
using Microsoft.EntityFrameworkCore;

namespace crud_api.Context;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Employee> Employees {get; set; }
    public DbSet<Profile> Profiles {get; set; }
}