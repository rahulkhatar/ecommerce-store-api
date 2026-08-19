using ECommerce.Domain.Interfaces;
using MediatR;

namespace ECommerce.Application.Features.Addresses;

public record GetMyAddressesQuery(Guid UserId) : IRequest<List<AddressDto>>;

public class GetMyAddressesQueryHandler(IAddressRepository addressRepository)
    : IRequestHandler<GetMyAddressesQuery, List<AddressDto>>
{
    public async Task<List<AddressDto>> Handle(GetMyAddressesQuery request, CancellationToken cancellationToken)
    {
        var addresses = await addressRepository.GetByUserAsync(request.UserId, cancellationToken);
        return addresses.Select(a => new AddressDto(
            a.Id, a.AddressType, a.FullName, a.PhoneNumber, a.StreetAddress, a.City,
            a.StateProvince, a.PostalCode, a.Country, a.IsDefaultAddress ?? false)).ToList();
    }
}
