using Microsoft.AspNetCore.Http;
using Shared.DataTransferObjects;

public interface ICompanyLinks
{
    LinkResponse TryGenerateLinks(IEnumerable<CompanyDto> companiesDto, string fields, HttpContext httpContext);
}