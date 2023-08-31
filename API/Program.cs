
using api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using api.email;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<StudentSupportXbcadContext>(
    options => { options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")); }
    );
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureSwaggerGen(setup =>
{
    setup.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "SupportAPI",
        Version = "v1"
    });
});

builder.Configuration.AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();


builder.Services.Configure<adminmail>(builder.Configuration.GetSection("adminmail"));
builder.Services.Configure<usermail>(builder.Configuration.GetSection("usermail"));
builder.Services.AddTransient<IMailService, MailService>();




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
