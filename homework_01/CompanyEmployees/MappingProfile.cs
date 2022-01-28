﻿using AutoMapper;
using Shared.DataTransferObjects;
using static Shared.DataTransferObjects.CompanyDto;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Company, CompanyDto>()
        .ForMember(c => c.FullAddress,
        opt => opt.MapFrom(x => string.Join(' ', x.Address, x.Country)));
        
        CreateMap<Employee, EmployeeDto>();

        CreateMap<CompanyForCreationDto, Company>();
    }


}