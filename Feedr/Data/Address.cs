using System;
using System.Collections.Generic;

namespace Feedr.Data;

public partial class Address
{
    public int AddressId { get; set; }

    public string Street { get; set; } = null!;

    public string HouseNumber { get; set; } = null!;

    public string PostalCode { get; set; } = null!;

    public string City { get; set; } = null!;

    public virtual ICollection<AppUser> AppUsers { get; set; } = new List<AppUser>();

    public virtual ICollection<Restaurant> Restaurants { get; set; } = new List<Restaurant>();
}
