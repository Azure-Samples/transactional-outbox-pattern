using System;
using System.Threading;
using System.Threading.Tasks;
using Contacts.Application.Commands;
using Contacts.Application.Mapper;
using Contacts.Application.Models;
using Contacts.Application.Queries;
using Contacts.Application.Queries.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Contacts.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ContactsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ContactsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ReadContactQueryResponse>> GetContact(Guid id,
        CancellationToken cancellationToken,
        [FromHeader(Name = "ETag")] string etag = null)
    {
        var res = await _mediator.Send(new ReadContactQuery { Id = id, Etag = etag },
            cancellationToken);
        var output = res.ToDto();
        Response.Headers.Add("ETag", res.Etag);
        return Ok(output);
    }

    [HttpGet]
    public async Task<ActionResult<ReadAllContactsQueryResponse>> GetAllContacts(
        CancellationToken cancellationToken,
        [FromHeader(Name = "x-continuation-token")]
        string continuationToken = null, [FromQuery(Name = "pageSize")] int pageSize = 10)
    {
        var res = await _mediator.Send(new ReadAllContactsQuery
            { PageSize = pageSize, ContinuationToken = continuationToken }, cancellationToken);
        var output = res.ToDto();
        Response.Headers.Add("X-Continuation-Token", res.ContinuationToken);
        return Ok(output);
    }

    [HttpPost]
    public async Task<ActionResult> CreateContact([FromBody] CreateContactDto dto,
        CancellationToken cancellationToken)
    {
        var cmd = dto.ToCommand();
        var res = await _mediator.Send(cmd, cancellationToken);
        return CreatedAtAction(nameof(GetContact), new { id = res.Id }, null);
    }

    [HttpPost("{id:guid}/update-name")]
    public async Task<ActionResult> UpdateContactName(Guid id, [FromBody] UpdateContactNameDto dto,
        CancellationToken cancellationToken,
        [FromHeader(Name = "ETag")] string etag = null)
    {
        var cmd = dto.ToCommand();

        cmd.Id = id;
        cmd.Etag = etag;

        var res = await _mediator.Send(cmd, cancellationToken);
        Response.Headers.Add("ETag", res.Etag);
        return NoContent();
    }

    [HttpPost("{id:guid}/update-description")]
    public async Task<ActionResult> UpdateContactDescription(Guid id, [FromBody] UpdateContactDescriptionDto dto,
        CancellationToken cancellationToken,
        [FromHeader(Name = "ETag")] string etag = null)
    {
        var cmd = dto.ToCommand();

        cmd.Id = id;
        cmd.Etag = etag;

        var res = await _mediator.Send(cmd, cancellationToken);
        Response.Headers.Add("ETag", res.Etag);
        return NoContent();
    }

    [HttpPost("{id:guid}/update-email")]
    public async Task<ActionResult> UpdateContactEmail(Guid id, [FromBody] UpdateContactEmailDto dto,
        CancellationToken cancellationToken,
        [FromHeader(Name = "ETag")] string etag = null)
    {
        var cmd = dto.ToCommand();

        cmd.Id = id;
        cmd.Etag = etag;

        var res = await _mediator.Send(cmd, cancellationToken);
        Response.Headers.Add("ETag", res.Etag);
        return NoContent();
    }

    [HttpPost("{id:guid}/update-company")]
    public async Task<ActionResult> UpdateContactCompany(Guid id, [FromBody] UpdateContactCompanyDto dto,
        CancellationToken cancellationToken,
        [FromHeader(Name = "ETag")] string etag = null)
    {
        var cmd = dto.ToCommand();

        cmd.Id = id;
        cmd.Etag = etag;

        var res = await _mediator.Send(cmd, cancellationToken);
        Response.Headers.Add("ETag", res.Etag);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteContact(Guid id,
        CancellationToken cancellationToken,
        [FromHeader(Name = "ETag")] string etag = null)
    {
        var res = await _mediator.Send(new DeleteContactCommand { Id = id, Etag = etag },
            cancellationToken);
        Response.Headers.Add("ETag", res.Etag);
        return Ok();
    }
}