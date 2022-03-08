using Contracts;

public class CompanyEmployeeChecker : ICompanyEmployeeChecker
{
    private IRepositoryManager _repository;
    public CompanyEmployeeChecker(IRepositoryManager repository)
    {
        _repository = repository;
    }

    public async Task CheckIfCompanyExists(Guid companyId, bool trackChanges)
    {
        var company = await _repository.Company.GetCompanyAsync(companyId,
        trackChanges);
        if (company is null)
            throw new CompanyNotFoundException(companyId);
    }

    public async Task<Employee> GetEmployeeForCompanyAndCheckIfItExists
    (Guid companyId, Guid id, bool trackChanges)
    {
        var employeeDb = await _repository.Employee.GetEmployeeAsync(companyId, id,
        trackChanges);
        if (employeeDb is null)
            throw new EmployeeNotFoundException(id);
        return employeeDb;
    }

    public async Task<Company> GetCompanyAndCheckIfItExists(Guid id, bool trackChanges)
    {
        var company = await _repository.Company.GetCompanyAsync(id, trackChanges);
        if (company is null)
            throw new CompanyNotFoundException(id);
        return company;
    }
}