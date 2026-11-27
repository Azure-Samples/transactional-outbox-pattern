using System.Collections.Generic;
using Contacts.Application.Mapper;
using Contacts.Application.Models;
using Contacts.Application.Queries.Responses;
using Xunit;

namespace Contacts.Tests;

public class ContactsMapperTest
{
    [Fact]
    public void ReadContactResponse_MapsNestedNameAndCompany()
    {
        var response = new ReadContactQueryResponse("id-1", "etag", "desc", "a@b.c",
            new Name("John", "Doe"),
            new Company("Contoso", "Main St", "1", "12345", "Berlin", "DE"));

        var dto = response.ToDto();

        Assert.Equal("id-1", dto.Id);
        Assert.Equal("desc", dto.Description);
        Assert.Equal("a@b.c", dto.Email);
        Assert.Equal("John", dto.Name.FirstName);
        Assert.Equal("Doe", dto.Name.LastName);
        Assert.Equal("Contoso", dto.Company.CompanyName);
        Assert.Equal("Main St", dto.Company.Street);
        Assert.Equal("1", dto.Company.HouseNumber);
        Assert.Equal("12345", dto.Company.PostalCode);
        Assert.Equal("Berlin", dto.Company.City);
        Assert.Equal("DE", dto.Company.Country);
    }

    [Fact]
    public void ReadContactResponse_WithoutNameOrCompany_MapsToNull()
    {
        var dto = new ReadContactQueryResponse("id-1", "etag", null, "a@b.c", null, null).ToDto();

        Assert.Null(dto.Name);
        Assert.Null(dto.Company);
    }

    [Fact]
    public void ReadAllContactsResponse_MapsItems()
    {
        var response = new ReadAllContactsQueryResponse(true, "token",
            new List<ContactsListModel> { new("id-1", "etag", "a@b.c", "John Doe", "Contoso") });

        var dto = response.ToDto();

        Assert.True(dto.HasMore);
        var item = Assert.Single(dto.Items);
        Assert.Equal("id-1", item.Id);
        Assert.Equal("a@b.c", item.Email);
        Assert.Equal("John Doe", item.FullName);
        Assert.Equal("Contoso", item.CompanyName);
    }

    [Fact]
    public void ReadAllContactsResponse_WithNullItems_MapsToEmptyList()
    {
        var dto = new ReadAllContactsQueryResponse(false, null, null).ToDto();

        Assert.NotNull(dto.Items);
        Assert.Empty(dto.Items);
    }

    [Fact]
    public void CreateContactDto_MapsAllFields()
    {
        var cmd = new CreateContactDto("John", "Doe", "a@b.c", "desc", "Contoso").ToCommand();

        Assert.Equal("John", cmd.FirstName);
        Assert.Equal("Doe", cmd.LastName);
        Assert.Equal("a@b.c", cmd.Email);
        Assert.Equal("desc", cmd.Description);
        Assert.Equal("Contoso", cmd.CompanyName);
    }

    [Fact]
    public void UpdateContactCompanyDto_MapsAllFields()
    {
        var cmd = new UpdateContactCompanyDto("Contoso", "Main St", "1", "12345", "Berlin", "DE").ToCommand();

        Assert.Equal("Contoso", cmd.CompanyName);
        Assert.Equal("Main St", cmd.Street);
        Assert.Equal("1", cmd.HouseNumber);
        Assert.Equal("12345", cmd.PostalCode);
        Assert.Equal("Berlin", cmd.City);
        Assert.Equal("DE", cmd.Country);
    }
}
