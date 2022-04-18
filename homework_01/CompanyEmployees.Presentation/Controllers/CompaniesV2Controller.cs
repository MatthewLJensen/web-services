using Microsoft.AspNetCore.Mvc;
using Service.Contracts;

//Example V2 Company controller

[Route("api/companies")]
[ApiController]
public class CompaniesV2Controller : ControllerBase
{
    private readonly IServiceManager _service;
    public CompaniesV2Controller(IServiceManager service) => _service = service;
    [HttpGet]
    public async Task<IActionResult> GetCompanies([FromQuery] CompanyParameters companyParameters)
    {
        var linkParams = new CompanyLinkParameters(companyParameters, HttpContext);
        var pagedResult = await _service.CompanyService.GetAllCompaniesAsync(companyParameters, linkParams, trackChanges: false);

        return Ok(pagedResult);
    }
}