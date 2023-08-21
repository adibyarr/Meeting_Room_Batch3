using MeetingRoomWebApp.AutoGen;
using Microsoft.EntityFrameworkCore;

public static class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<MeetingRoomDbContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("MeetingRoomDatabase") ?? throw new InvalidOperationException("Connection string 'MeetingRoomDbContext' not found.")));

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        builder.Services.AddDistributedMemoryCache();

        builder.Services.AddSession();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/User/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.UseSession();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=User}/{action=Index}/{id?}");

        app.Run();
    }
}