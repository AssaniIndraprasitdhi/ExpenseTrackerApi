using System.Text;
using DotNetEnv;
using ExpenseTrackerApi.Data;
using ExpenseTrackerApi.Helpers;
using ExpenseTrackerApi.Interfaces;
using ExpenseTrackerApi.Middleware;
using ExpenseTrackerApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

var connString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
builder.Services.AddDbContext<AppDbContext>(opts =>
    opts.UseMySql(connString, ServerVersion.AutoDetect(connString))
);

var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET")!;
var jwtExpiresDays = int.Parse(Environment.GetEnvironmentVariable("JWT_EXPIRES_IN_DAYS")!);
builder.Services.Configure<JwtSettings>(opts =>
{
    opts.Secret = jwtSecret;
    opts.ExpiresInDay = jwtExpiresDays;
});

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();

var key = Encoding.ASCII.GetBytes(jwtSecret);
builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
        };
    });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<JwtMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
