using Shared.DataTransferObjects;
namespace Contracts
{
    public interface ICompanyEmployeeChecker
    { 
        Task<Company> GetCompanyAndCheckIfItExists(Guid id, bool trackChanges);
        Task CheckIfCompanyExists(Guid companyId, bool trackChanges);
        Task<Employee> GetEmployeeForCompanyAndCheckIfItExists(Guid companyId, Guid id, bool trackChanges);

    }
}
