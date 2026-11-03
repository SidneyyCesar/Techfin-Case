using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Techfin.Case.Application.Interfaces;
using Techfin.Case.Application.UseCases.LoginUser;
using Techfin.Case.Application.UseCases.RefreshToken;
using Techfin.Case.Application.UseCases.RegisterUser;
using Techfin.Case.Domain.Repositories;
using Techfin.Case.Infra.Data;
using Techfin.Case.Infra.Repositories;
using Techfin.Case.Infra.Security;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Users DI
builder.Services.AddSingleton<IDbConnectionFactory, SqliteConnectionFactory>();

builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IPasswordVerify, PasswordVerify>();

builder.Services.AddScoped<RegisterUserHandler>();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

builder.Services.AddScoped<IJwtProvider, JwtProvider>();

builder.Services.AddScoped<LoginUserHandler>();
builder.Services.AddScoped<RefreshTokenHandler>();


var jwtSettings =
    builder.Configuration
        .GetSection("JwtSettings")
        .Get<JwtSettings>()!;

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            jwtSettings.SecretKey))
            };
    });

builder.Services.AddAuthorization();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    _ = app.UseSwagger();
    _ = app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
