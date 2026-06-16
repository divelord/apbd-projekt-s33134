using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SystemUznawaniaPrzychodow.Data;
using SystemUznawaniaPrzychodow.DTOs;
using SystemUznawaniaPrzychodow.Entities;
using SystemUznawaniaPrzychodow.Exceptions;
using SystemUznawaniaPrzychodow.Services;

namespace SystemUznawaniaPrzychodow.Tests.UnitTests;

public class ContractServiceTest
{
    private AppDbContext GetInMemoryDbContext()
    {
        var opt = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(x =>
                x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        return new AppDbContext(opt);
    }

    [Fact]
    public async Task ProcessPaymentAsync_AmountCoversTotalPrice_ShouldMarkContractAsSigned()
    {
        var dbContext = GetInMemoryDbContext();
        var service = new ContractService(dbContext);

        var testClient = new IndividualClient
        {
            ClientId = 1,
            Address = "test",
            Email = "test",
            PhoneNumber = "test",
            FirstName = "test",
            LastName = "test",
            Pesel = "test"
        };

        await dbContext.IndividualClients.AddAsync(testClient);
        await dbContext.SaveChangesAsync();

        var testContract = new Contract
        {
            ContractId = 1,
            ClientId = 1,
            Price = 5000.0m,
            IsSigned = false,
            Deadline = DateOnly.FromDateTime(DateTime.Now.AddDays(5))
        };

        await dbContext.Contracts.AddAsync(testContract);
        await dbContext.SaveChangesAsync();

        var paymentDto = new CreatePaymentDto
        {
            ClientId = 1,
            Amount = 5000.0m,
        };

        await service.ProcessPaymentAsync(contractId: testContract.ContractId, dto: paymentDto);

        var contract = await dbContext.Contracts.FindAsync(testContract.ContractId);

        Assert.NotNull(contract);
        Assert.True(contract.IsSigned);
    }

    [Theory]
    [InlineData(2, false)]
    [InlineData(3, true)]
    [InlineData(15, true)]
    [InlineData(30, true)]
    [InlineData(31, false)]
    public async Task CreateContractAsync_ValidateContractSignTime(
        int days, bool isSucceed)
    {
        var dbContext = GetInMemoryDbContext();
        var service = new ContractService(dbContext);

        var testSoftware = new Software
        {
            SoftwareId = 1,
            SoftwareName = "Software",
            Description = "Software Description",
            Version = "Software Version",
            Category = "Software",
            AnnualPrice = 1000.0m
        };

        await dbContext.AddAsync(testSoftware);
        await dbContext.SaveChangesAsync();

        var testClient = new IndividualClient
        {
            ClientId = 1,
            Address = "test",
            Email = "test",
            PhoneNumber = "test",
            FirstName = "test",
            LastName = "test",
            Pesel = "test"
        };

        await dbContext.IndividualClients.AddAsync(testClient);
        await dbContext.SaveChangesAsync();

        var dateFrom = DateOnly.FromDateTime(DateTime.Now);
        var dateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(days));

        var dto = new CreateContractDto
        {
            ClientId = 1,
            SoftwareId = 1,
            DateFrom = dateFrom,
            DateTo = dateTo,
            AdditionalSupportYears = 1
        };

        if (isSucceed)
        {
            await service.CreateContractAsync(dto);

            var isContractCreated = await dbContext.Contracts
                .AnyAsync(x =>
                    x.ClientId == dto.ClientId && x.SoftwareId == dto.SoftwareId);

            Assert.True(isContractCreated);
        }
        else
        {
            await Assert.ThrowsAsync<BadRequestException>(() =>
                service.CreateContractAsync(dto));
        }
    }
}