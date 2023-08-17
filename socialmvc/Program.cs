using Microsoft.EntityFrameworkCore;
using socialmvc.Data;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using socialmvc.Interfaces;
using socialmvc.Repository;
using socialmvc.Helpers;
using socialmvc.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container. 
builder.Services.AddControllersWithViews();



//add the repository and interface services
builder.Services.AddScoped<IClubRepository, ClubRepository>();
builder.Services.AddScoped<IRaceRepository, RaceRepository>();
builder.Services.AddScoped<IPhotoService, PhotoService>();

//cloudinary
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));


builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("MySqlConnStr"),
        new MySqlServerVersion(new Version(10, 4, 28)),//phpmyadmin version 10.4.28
        mySqlOptions =>
        {
            mySqlOptions.EnableRetryOnFailure();
        });
});

var app = builder.Build();

//seeder
if(args.Length ==1 && args[0].ToLower() =="seeddata")
{
    Seed.SeedData(app);
}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

