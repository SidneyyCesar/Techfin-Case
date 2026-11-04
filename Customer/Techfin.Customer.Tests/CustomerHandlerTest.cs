using Microsoft.Extensions.Caching.Memory;
using Moq;
using System.Net;
using Techfin.Customer.Application.DTOs.Requests;
using Techfin.Customer.Application.Services;
using Techfin.Customer.Domain.Repositories;

namespace Techfin.Customer.Tests;

public class CustomerHandlerTest
{
    [Fact]
    public async Task Should_Create_Customer()
    {
        var repositoryMock = new Mock<ICustomerRepository>();
        var memoryCacheMock = new Mock<IMemoryCache>();

        var handler = new CustomerService(repositoryMock.Object, memoryCacheMock.Object);

        var result = await handler.IncludeAsync(new CustomerInsertRequest()
        {
            Nome = "Techfin ERP",
            CPF = "12345678900",
            ValorLimite = 1000.00m
        });


        Assert.True(result.Success);
        Assert.True(result.StatusCode == HttpStatusCode.OK);
        Assert.Equal("Cliente cadastrado com sucesso", result.Message);
    }
}