using Contracts;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Tests;

public class CompanyRepositoryTests
{
    [Fact]
    public void
    GetAllCompaniesAsync_ReturnsListOfCompanies_WithASingleCompany()
    {

        var cp = new CompanyParameters();
        // Arrange
        var mockRepo = new Mock<ICompanyRepository>();
        mockRepo.Setup(repo => (repo.GetAllCompaniesAsync(cp, false)))
            .Returns(Task.FromResult(GetCompanies()));
        // Act
        var result = mockRepo.Object.GetAllCompaniesAsync(cp, false)
        .GetAwaiter()
        .GetResult().ToList();
        // Assert
        Assert.IsAssignableFrom<List<Company>>(result);
        Assert.Single(result);
    }
    public PagedList<Company> GetCompanies()
    {
        var rval = new PagedList<Company>(new List<Company>(), 1, 1, 1);
        rval.Add(new Company
        {
            Id = Guid.NewGuid(),
            Name = "Test Company",
            Country = "United States",
            Address = "908 Woodrow Way"
        });
        return rval;
    }
}