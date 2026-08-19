using ECommerce.API.Extensions;
using ECommerce.Application.Features.Addresses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/users/me/addresses")]
[Authorize]
public class AddressesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<AddressDto>>> GetMyAddresses()
        => Ok(await mediator.Send(new GetMyAddressesQuery(User.GetUserId())));

    [HttpPost]
    public async Task<ActionResult<AddressDto>> CreateAddress(CreateAddressDto dto)
        => Ok(await mediator.Send(new CreateAddressCommand(User.GetUserId(), dto)));
}
