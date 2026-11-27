using System.Collections.Generic;
using System.Linq;
using Contacts.Application.Commands;
using Contacts.Application.Models;
using Contacts.Application.Queries.Responses;

namespace Contacts.Application.Mapper;

public static class ContactsMapper
{
    public static CreateContactCommand ToCommand(this CreateContactDto dto) => new()
    {
        FirstName = dto.FirstName,
        LastName = dto.LastName,
        Email = dto.Email,
        Description = dto.Description,
        CompanyName = dto.CompanyName
    };

    public static UpdateContactNameCommand ToCommand(this UpdateContactNameDto dto) => new()
    {
        FirstName = dto.FirstName,
        LastName = dto.LastName
    };

    public static UpdateContactDescriptionCommand ToCommand(this UpdateContactDescriptionDto dto) => new()
    {
        Description = dto.Description
    };

    public static UpdateContactEmailCommand ToCommand(this UpdateContactEmailDto dto) => new()
    {
        Email = dto.Email
    };

    public static UpdateContactCompanyCommand ToCommand(this UpdateContactCompanyDto dto) => new()
    {
        CompanyName = dto.CompanyName,
        Street = dto.Street,
        HouseNumber = dto.HouseNumber,
        PostalCode = dto.PostalCode,
        City = dto.City,
        Country = dto.Country
    };

    public static ReadContactDto ToDto(this ReadContactQueryResponse response) => new()
    {
        Id = response.Id,
        Description = response.Description,
        Email = response.Email,
        Name = response.Name?.ToDto(),
        Company = response.Company?.ToDto()
    };

    public static NameDto ToDto(this Name name) => new()
    {
        FirstName = name.FirstName,
        LastName = name.LastName
    };

    public static CompanyDto ToDto(this Company company) => new()
    {
        CompanyName = company.CompanyName,
        Street = company.Street,
        HouseNumber = company.HouseNumber,
        PostalCode = company.PostalCode,
        City = company.City,
        Country = company.Country
    };

    public static ReadAllContactsDto ToDto(this ReadAllContactsQueryResponse response) => new()
    {
        HasMore = response.HasMore,
        // Matches the previous AutoMapper behavior: a null source list maps to an empty list.
        Items = response.Items?.Select(item => item.ToDto()).ToList() ?? new List<ContactsListItemDto>()
    };

    public static ContactsListItemDto ToDto(this ContactsListModel model) => new()
    {
        Id = model.Id,
        Email = model.Email,
        FullName = model.FullName,
        CompanyName = model.CompanyName
    };
}
