using System;
using System.Collections.Generic;

namespace Feedr.Data;

public partial class Restaurant
{
    public int RestaurantId { get; set; }

    public string Name { get; set; } = null!;

    public int AddressId { get; set; }

    public int OwnerId { get; set; }

    public virtual Address Address { get; set; } = null!;

    public virtual AppUser Owner { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual ICollection<RestaurantOrder> RestaurantOrders { get; set; } = new List<RestaurantOrder>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
