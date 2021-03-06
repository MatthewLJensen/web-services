using Contracts;
using Microsoft.EntityFrameworkCore;
using Repository.Extensions;

internal sealed class CompanyRepository : RepositoryBase<Company>, ICompanyRepository
{
    public CompanyRepository(RepositoryContext repositoryContext) : base(repositoryContext)
    {
    }

    public async Task<PagedList<Company>> GetAllCompaniesAsync(CompanyParameters companyParameters, bool trackChanges)
    {
        var companies = await FindAll(trackChanges)
        .OrderBy(c => c.Name)
        .Sort(companyParameters.OrderBy)
        .ToListAsync();

        return PagedList<Company>
            .ToPagedList(companies, companyParameters.PageNumber, companyParameters.PageSize);
    }


    public async Task<Company?> GetCompanyAsync(Guid companyId, bool trackChanges) =>
    await FindByCondition(c => c.Id.Equals(companyId), trackChanges)
    .SingleOrDefaultAsync();

    public void CreateCompany(Company company) => Create(company);

    public async Task<IEnumerable<Company>> GetByIdsAsync(IEnumerable<Guid> ids, bool
    trackChanges) =>
    await FindByCondition(x => ids.Contains(x.Id), trackChanges)
    .ToListAsync();

    public void DeleteCompany(Company company) => Delete(company);
}