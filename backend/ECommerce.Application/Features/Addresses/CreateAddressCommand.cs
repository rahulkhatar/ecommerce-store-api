using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace ECommerce.Application.Features.Addresses;

public record CreateAddressCommand(Guid UserId, CreateAddressDto Dto) : IRequest<AddressDto>;

public class CreateAddressCommandValidator : AbstractValidator<CreateAddressCommand>
{
    public CreateAddressCommandValidator() => RuleFor(x => x.Dto).SetValidator(new CreateAddressDtoValidator());
}

public class CreateAddressCommandHandler(IAddressRepository addressRepository)
    : IRequestHandler<CreateAddressCommand, AddressDto>
{
    public async Task<AddressDto> Handle(CreateAddressCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        var address = new Address
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            AddressType = dto.AddressType,
            FullName = dto.FullName,
            PhoneNumber = dto.PhoneNumber,
            StreetAddress = dto.StreetAddress,
            City = dto.City,
            StateProvince = dto.StateProvince,
            PostalCode = dto.PostalCode,
            Country = dto.Country,
            IsDefaultAddress = dto.IsDefaultAddress,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false,
        };

        await addressRepository.AddAsync(address, cancellationToken);
        await addressRepository.SaveChangesAsync(cancellationToken);

        return new AddressDto(address.Id, address.AddressType, address.FullName, address.PhoneNumber,
            address.StreetAddress, address.City, address.StateProvince, address.PostalCode, address.Country,
            address.IsDefaultAddress ?? false);
    }
}
