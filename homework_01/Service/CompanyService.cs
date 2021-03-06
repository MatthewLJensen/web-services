using AutoMapper;
using Contracts;
using Service.Contracts;
using Shared.DataTransferObjects;
using static Shared.DataTransferObjects.CompanyDto;

namespace Service
{
    internal sealed class CompanyService : ICompanyService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly ICompanyLinks _companyLinks;

        public CompanyService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper, ICompanyLinks companyLinks)

        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _companyLinks = companyLinks;
        }
        public async Task<(LinkResponse linkResponse, MetaData metaData)> GetAllCompaniesAsync(CompanyParameters companyParameters, CompanyLinkParameters linkParameters, bool trackChanges)
        {
            var companiesWithMetaData = await _repository.Company.GetAllCompaniesAsync(linkParameters.CompanyParameters, trackChanges);

            var companiesDto = _mapper.Map<IEnumerable<CompanyDto>>(companiesWithMetaData);

            var links = _companyLinks.TryGenerateLinks(companiesDto,

            linkParameters.CompanyParameters.Fields, linkParameters.Context);
            return (linkResponse: links, metaData: companiesWithMetaData.MetaData);
        }
        public async Task<CompanyDto> GetCompanyAsync(Guid id, bool trackChanges)
        {
            var company = await _repository.CompanyEmployeeChecker.GetCompanyAndCheckIfItExists(id, trackChanges);

            var companyDto = _mapper.Map<CompanyDto>(company);
            return companyDto;
        }

        public async Task<CompanyDto> CreateCompanyAsync(CompanyForCreationDto company)
        {
            var companyEntity = _mapper.Map<Company>(company);
            _repository.Company.CreateCompany(companyEntity);
            await _repository.SaveAsync();
            var companyToReturn = _mapper.Map<CompanyDto>(companyEntity);
            return companyToReturn;
        }

        public async Task<IEnumerable<CompanyDto>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges)
        {
            if (ids is null)
                throw new IdParametersBadRequestException();
            var companyEntities = await _repository.Company.GetByIdsAsync(ids, trackChanges);
            if (ids.Count() != companyEntities.Count())
                throw new CollectionByIdsBadRequestException();
            var companiesToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
            return companiesToReturn;
        }

        public async Task<(IEnumerable<CompanyDto> companies, string ids)>
        CreateCompanyCollectionAsync
        (IEnumerable<CompanyForCreationDto> companyCollection)
        {
            if (companyCollection is null)
                throw new CompanyCollectionBadRequest();
            var companyEntities = _mapper.Map<IEnumerable<Company>>(companyCollection);
            foreach (var company in companyEntities)
            {
                _repository.Company.CreateCompany(company);
            }
            await _repository.SaveAsync();
            var companyCollectionToReturn =
            _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
            var ids = string.Join(",", companyCollectionToReturn.Select(c => c.Id));
            return (companies: companyCollectionToReturn, ids: ids);
        }

        public async Task DeleteCompanyAsync(Guid companyId, bool trackChanges)
        {
            var company = await _repository.CompanyEmployeeChecker.GetCompanyAndCheckIfItExists(companyId, trackChanges);

            _repository.Company.DeleteCompany(company);
            await _repository.SaveAsync();
        }

        public async Task UpdateCompanyAsync(Guid companyId,
         CompanyForUpdateDto companyForUpdate, bool trackChanges)
        {
            var company = await _repository.CompanyEmployeeChecker.GetCompanyAndCheckIfItExists(companyId, trackChanges);

            _mapper.Map(companyForUpdate, company);
            await _repository.SaveAsync();
        }

        public async Task<(CompanyForUpdateDto companyToPatch, Company companyEntity)> GetCompanyForPatchAsync(
        Guid companyId, bool trackChanges)
        {
            var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges);
            if (company is null)
                throw new CompanyNotFoundException(companyId);
            var companyToPatch = _mapper.Map<CompanyForUpdateDto>(company);
            return (companyToPatch, company);
        }
        public async Task SaveChangesForPatchAsync(CompanyForUpdateDto companyToPatch, Company companyEntity)
        {
            _mapper.Map(companyToPatch, companyEntity);
            await _repository.SaveAsync();
        }

    }
}