using Techfin.Customer.Application.UseCases;
using Techfin.Customer.Domain.Repositories;
using Techfin.Customer.Infra.Data;
using Techfin.Customer.Infra.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Users DI
builder.Services.AddSingleton<IDbConnectionFactory, SqliteConnectionFactory>();

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

builder.Services.AddScoped<CustomerHandler>();

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
