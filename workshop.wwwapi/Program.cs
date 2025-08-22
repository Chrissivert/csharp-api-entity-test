using Microsoft.EntityFrameworkCore;
using workshop.wwwapi.Data;
using workshop.wwwapi.Endpoints;
using workshop.wwwapi.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IRepository, Repository>();

// Retrieve connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnectionString");
Console.WriteLine("In Program.cs");
Console.WriteLine($"Runtime connection string: {connectionString}");

// Configure DbContext
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Configure endpoints
app.ConfigurePatientEndpoint();
app.MapControllers();

app.Run();

public partial class Program { }
