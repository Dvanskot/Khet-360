using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Khet360.Application.Interfaces;
using Khet360.Api.Attributes;

namespace Khet360.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpPost("individual")]
    [HasPermission("Customers.Create")]
    public async Task<IActionResult> CreateIndividual([FromBody] CreateIndividualRequest request)
    {
        var id = await _customerService.CreateIndividualAsync(request);
        return CreatedAtAction(nameof(GetById), new { id }, new { Id = id });
    }

    [HttpPost("organisation")]
    [HasPermission("Customers.Create")]
    public async Task<IActionResult> CreateOrganisation([FromBody] CreateOrganisationRequest request)
    {
        var id = await _customerService.CreateOrganisationAsync(request);
        return CreatedAtAction(nameof(GetById), new { id }, new { Id = id });
    }

    [HttpGet("{id}")]
    [HasPermission("Customers.Read")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var customer = await _customerService.GetCustomerByIdAsync(id);
        if (customer == null) return NotFound();
        return Ok(customer);
    }

    [HttpPut("{id}")]
    [HasPermission("Customers.Edit")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCustomerRequest request)
    {
        if (id != request.Id) return BadRequest("ID mismatch");
        await _customerService.UpdateCustomerAsync(request);
        return NoContent();
    }

    [HttpPost("search")]
    [HasPermission("Customers.Read")]
    public async Task<IActionResult> Search([FromBody] CustomerSearchFilter filter)
    {
        var results = await _customerService.SearchCustomersAsync(filter);
        return Ok(results);
    }
}
