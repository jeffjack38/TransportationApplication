using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TransportationApplication.SharedModels;

namespace TransportationApplication.UserService.Data
{
    //UserDbContext inherit from IdentityDbContext<User>, predefined tables from Identity
    public class UserDbContext : IdentityDbContext<User>
    {
        //Contructor to accept DbContextOptions to configure DbContext setting (db provider SQL Server)
        public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options)
        {
        }

        //method to configure how entities should be mapped to the database using ModelBuilder
        protected override void OnModelCreating(ModelBuilder builder)
        {
            //calls base method from IdentityDbContext to configure ASP.NET Identity's built-in tables
            base.OnModelCreating(builder);

            //seed roles for Dispatcher, Driver, Customer.. Normalized/
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole {  Name = "Disatcher", NormalizedName = "DISPATCHER" },
                new IdentityRole {  Name = "Driver", NormalizedName = "DRIVER" },
                new IdentityRole {  Name = "Customer", NormalizedName = "CUSTOMER"}
                );
        }
    }
}
