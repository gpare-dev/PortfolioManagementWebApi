using Microsoft.EntityFrameworkCore;
using WebApi.Controllers;
using WebApi.Filters;
using WebApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<LogFilter>();
builder.Services.AddDbContext<PortfolioManagementContext>(opt => opt.UseInMemoryDatabase("PortfolioManagement"), contextLifetime: ServiceLifetime.Singleton, optionsLifetime: ServiceLifetime.Singleton);
builder.Services.AddSingleton<IPortfolioManagementController, PortfolioManagementController>();

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();

app.UseCors(builder =>
       builder.WithOrigins("https://localhost:7237")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
