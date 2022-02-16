using Shared.DataTransferObjects;
using System.ComponentModel.DataAnnotations;

public abstract record CompanyForManipulationDto
{

    public Guid Id { get; init; }

    [Required(ErrorMessage = "Company name is a required field.")]
    [MaxLength(60, ErrorMessage = "Maximum length for the Name is 60 characters.")]
    public string? Name { get; init; }
    public string? FullAddress { get; init; }

    public record CompanyForCreationDto(string Name, string Address, string Country,
    IEnumerable<EmployeeForCreationDto> Employees);

    public record CompanyForUpdateDto(string Name, string Address, string Country,
    IEnumerable<EmployeeForCreationDto> Employees);
}