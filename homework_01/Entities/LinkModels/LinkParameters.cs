using Microsoft.AspNetCore.Http;
public record EmployeeLinkParameters(EmployeeParameters EmployeeParameters, HttpContext Context);
public record CompanyLinkParameters(CompanyParameters CompanyParameters, HttpContext Context);