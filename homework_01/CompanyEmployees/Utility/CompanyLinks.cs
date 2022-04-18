using Entities.Models;
using Microsoft.Net.Http.Headers;
using Shared.DataTransferObjects;

public class CompanyLinks : ICompanyLinks
{
    private readonly LinkGenerator _linkGenerator;
    private readonly IDataShaper<CompanyDto> _dataShaper;
    public CompanyLinks(LinkGenerator linkGenerator, IDataShaper<CompanyDto> dataShaper)
    {
        _linkGenerator = linkGenerator;
        _dataShaper = dataShaper;
    }

    public LinkResponse TryGenerateLinks(IEnumerable<CompanyDto> companiesDto, string fields, HttpContext httpContext)
    {
        var shapedCompanies = ShapeData(companiesDto, fields);
        if (ShouldGenerateLinks(httpContext))
            return ReturnLinkedCompanies(companiesDto, fields, httpContext, shapedCompanies);
        return ReturnShapedCompanies(shapedCompanies);
    }

    private List<Entity> ShapeData(IEnumerable<CompanyDto> companiesDto, string fields) => _dataShaper.ShapeData(companiesDto, fields)
        .Select(e => e.Entity)
        .ToList();

    private bool ShouldGenerateLinks(HttpContext httpContext)
    {
        var mediaType = (MediaTypeHeaderValue)httpContext.Items["AcceptHeaderMediaType"];
        return mediaType.SubTypeWithoutSuffix.EndsWith("hateoas",
        StringComparison.InvariantCultureIgnoreCase);
    }
    private LinkResponse ReturnShapedCompanies(List<Entity> shapedCompanies) =>
    new LinkResponse { ShapedEntities = shapedCompanies };


    private LinkResponse ReturnLinkedCompanies(IEnumerable<CompanyDto> companiesDto, string fields, HttpContext httpContext, List<Entity> shapedCompanies)
    {
        var companyDtoList = companiesDto.ToList();
        for (var index = 0; index < companyDtoList.Count(); index++)
        {
            var companyLinks = CreateLinksForCompany(httpContext, companyDtoList[index].Id, fields);
            shapedCompanies[index].Add("Links", companyLinks);
        }
        var companyCollection = new LinkCollectionWrapper<Entity>(shapedCompanies);
        var linkedCompanies = CreateLinksForCompanies(httpContext, companyCollection);
        return new LinkResponse { HasLinks = true, LinkedEntities = linkedCompanies };
    }

    private List<Link> CreateLinksForCompany(HttpContext httpContext, Guid id, string fields = "")
    {
        var links = new List<Link>{
            new Link(_linkGenerator.GetUriByAction(httpContext,"GetCompany",values: new { id, fields }),"self","GET"),
            new Link(_linkGenerator.GetUriByAction(httpContext,"DeleteCompany", values: new { id }),"delete_company","DELETE"),
            new Link(_linkGenerator.GetUriByAction(httpContext,"UpdateCompany", values: new { id }),"update_company","PUT"),
            new Link(_linkGenerator.GetUriByAction(httpContext,"PartiallyUpdateCompany", values: new { id }),"partially_update_company","PATCH"),
        };
        return links;
    }
    private LinkCollectionWrapper<Entity> CreateLinksForCompanies(HttpContext httpContext, LinkCollectionWrapper<Entity> companiesWrapper)
    {
        companiesWrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(httpContext,"GetCompany", values: new { }), "self","GET"));
        return companiesWrapper;
    }

}