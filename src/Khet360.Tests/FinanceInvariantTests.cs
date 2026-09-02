using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Khet360.Application.Interfaces;
using Khet360.Domain.Entities;
using Khet360.Infrastructure.Persistence;
using Khet360.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using FluentAssertions;

namespace Khet360.Tests;

public class FinanceInvariantTests
{
    private TenantDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<TenantDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var mockUserContext = new Mock<ITenantUserContext>();
        return new TenantDbContext(options, mockUserContext.Object);
    }

    [Fact]
    public async Task VerifyInvariants_Should_Return_Balanced_When_All_Transactions_Balance()
    {
        // Arrange
        var db = GetDbContext();
        var service = new FinanceVerificationService(db);

        var tx = new FinancialTransaction
        {
            Id = Guid.NewGuid(),
            Description = "Balanced Transaction",
            Entries = new List<FinancialEntry>
            {
                new() { Id = Guid.NewGuid(), AccountCode = "1001", Debit = 100.00m, Credit = 0 },
                new() { Id = Guid.NewGuid(), AccountCode = "2001", Debit = 0, Credit = 100.00m }
            }
        };

        db.FinancialTransactions.Add(tx);
        await db.SaveChangesAsync();

        // Act
        var result = await service.VerifyInvariantsAsync();

        // Assert
        result.IsBalanced.Should().BeTrue();
        result.Violations.Should().BeEmpty();
    }

    [Fact]
    public async Task VerifyInvariants_Should_Report_Violation_When_Transaction_Is_Unbalanced()
    {
        // Arrange
        var db = GetDbContext();
        var service = new FinanceVerificationService(db);

        var txId = Guid.NewGuid();
        var tx = new FinancialTransaction
        {
            Id = txId,
            Description = "Unbalanced Transaction",
            Entries = new List<FinancialEntry>
            {
                new() { Id = Guid.NewGuid(), AccountCode = "1001", Debit = 100.00m, Credit = 0 },
                new() { Id = Guid.NewGuid(), AccountCode = "2001", Debit = 0, Credit = 80.00m }
            }
        };

        db.FinancialTransactions.Add(tx);
        await db.SaveChangesAsync();

        // Act
        var result = await service.VerifyInvariantsAsync();

        // Assert
        result.IsBalanced.Should().BeFalse();
        result.Violations.Should().ContainSingle();
        result.Violations[0].TransactionId.Should().Be(txId);
        result.Violations[0].Variance.Should().Be(20.00m);
    }

    [Fact]
    public async Task VerifyInvariants_Should_Handle_Multiple_Transactions_With_Some_Violations()
    {
        // Arrange
        var db = GetDbContext();
        var service = new FinanceVerificationService(db);

        var tx1 = new FinancialTransaction
        {
            Id = Guid.NewGuid(),
            Entries = new List<FinancialEntry>
            {
                new() { Debit = 50, Credit = 0 },
                new() { Debit = 0, Credit = 50 }
            }
        };

        var tx2 = new FinancialTransaction
        {
            Id = Guid.NewGuid(),
            Entries = new List<FinancialEntry>
            {
                new() { Debit = 100, Credit = 0 },
                new() { Debit = 0, Credit = 90 }
            }
        };

        db.FinancialTransactions.AddRange(tx1, tx2);
        await db.SaveChangesAsync();

        // Act
        var result = await service.VerifyInvariantsAsync();

        // Assert
        result.IsBalanced.Should().BeFalse();
        result.Violations.Should().ContainSingle();
        result.Violations[0].TransactionId.Should().Be(tx2.Id);
    }
}
