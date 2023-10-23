using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
// Commented out the JWT-related namespaces
// using Microsoft.IdentityModel.Tokens;
// using System.Text;
// using System.IdentityModel.Tokens.Jwt;
// using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuration
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

builder.Services.AddSingleton<IConfiguration>(configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();


// Enable CORS (before UseAuthorization)
app.UseCors(builder =>
{
    builder
        //.WithOrigins("http://localhost:4200") // Replace with your Angular app's URL
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod();

});

// Commented out the JWT-related lines
// app.UseAuthentication(); // Add this line to enable authentication
// app.UseAuthorization();
// ...
// app.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer(options =>
//     {
//         options.TokenValidationParameters = tokenValidationParameters;
//     });

app.MapControllers();

app.Run();
