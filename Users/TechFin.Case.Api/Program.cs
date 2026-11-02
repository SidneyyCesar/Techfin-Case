using Techfin.Case.Application.Interfaces;
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

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    _ = app.UseSwagger();
    _ = app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
