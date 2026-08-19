using System;
using System.Collections.Generic;

namespace ECommerce.Domain.Entities;

public class Address : BaseEntity
{
    public Guid UserId { get; set; }

    public string AddressType { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string StreetAddress { get; set; } = null!;

    public string City { get; set; } = null!;

    public string StateProvince { get; set; } = null!;

    public string PostalCode { get; set; } = null!;

    public string Country { get; set; } = null!;

    public bool? IsDefaultAddress { get; set; }
    public virtual ICollection<Order> OrderBillingAddresses { get; set; } = new List<Order>();

    public virtual ICollection<Order> OrderShippingAddresses { get; set; } = new List<Order>();

    public virtual User User { get; set; } = null!;
}
