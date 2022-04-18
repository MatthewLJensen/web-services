using Shared.DataTransferObjects;

namespace Service.Contracts
{
    public interface ICompanyService
    {
        Task<(LinkResponse linkResponse, MetaData metaData)> GetAllCompaniesAsync(CompanyParameters companyParameters, CompanyLinkParameters linkParameters, bool trackChanges);
        Task<CompanyDto> GetCompanyAsync(Guid companyId, bool trackChanges);
        Task<CompanyDto> CreateCompanyAsync(CompanyForCreationDto company);
        Task<IEnumerable<CompanyDto>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges);

        Task<(IEnumerable<CompanyDto> companies, string ids)>CreateCompanyCollectionAsync(IEnumerable<CompanyForCreationDto> companyCollection);
        Task DeleteCompanyAsync(Guid companyId, bool trackChanges);
        Task UpdateCompanyAsync(Guid companyid, CompanyForUpdateDto companyForUpdate, bool trackChanges);

        Task<(CompanyForUpdateDto companyToPatch, Company companyEntity)> GetCompanyForPatchAsync(Guid companyId, bool trackChanges);
        Task SaveChangesForPatchAsync(CompanyForUpdateDto companyToPatch, Company companyEntity);
    }
}