using APBD9.Data;
using APBD9.Services;
using Microsoft.EntityFrameworkCore;

namespace APBD9;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        
        builder.Services.AddEndpointsApiExplorer(); // Wymagane dla AddSwaggerGen
        builder.Services.AddSwaggerGen();
        
        builder.Services.AddDbContext<AppDbContext>(opt =>
        {
            opt.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
        });
        
        builder.Services.AddScoped<IDbService, DbService>();
        

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthorization();
        
        


        app.MapControllers();
        
        
        
        app.Run();
    }
}