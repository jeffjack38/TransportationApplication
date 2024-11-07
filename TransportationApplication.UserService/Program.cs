using Microsoft.AspNetCore.Identity;
using TransportationApplication.SharedModels;
using Microsoft.Extensions.DependencyInjection;
using TransportationApplication.UserService.Data;
using Microsoft.EntityFrameworkCore;
using TransportationApplication.UserService.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//DbContext connections string
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("UserDbContext")));

//IdentityRoles
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<UserDbContext>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//Register UserServices
builder.Services.AddScoped<IUserServices, UserServices>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Seed Admin Role and User on Startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<User>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    await SeedAdminUserAsync(userManager, roleManager);
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


//method for seeding Admin user and role
async Task SeedAdminUserAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
{
    string adminRole = "Admin";
    string adminEmail = "admin@example.com";
    string adminPassword = "Admin@123";

    //check if the admin role exists if not create it
    if (!await roleManager.RoleExistsAsync(adminRole))
    {
        await roleManager.CreateAsync(new IdentityRole(adminRole));
    }

    //check if admin user already exists
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        //create the Admin user
        adminUser = new User
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            FullName = "AdminUser"
        };

        //create the user with the specified password
        var result = await userManager.CreateAsync(adminUser, adminPassword);

        //if success, assing Admin role to user
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, adminRole);
        }
    }
}