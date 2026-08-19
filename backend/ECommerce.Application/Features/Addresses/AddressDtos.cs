namespace ECommerce.Application.Features.Addresses;

public record AddressDto(
    Guid Id, string AddressType, string FullName, string PhoneNumber,
    string StreetAddress, string City, string StateProvince, string PostalCode, string Country, bool IsDefaultAddress);

public record CreateAddressDto(
    string AddressType, string FullName, string PhoneNumber,
    string StreetAddress, string City, string StateProvince, string PostalCode, string Country, bool IsDefaultAddress);
