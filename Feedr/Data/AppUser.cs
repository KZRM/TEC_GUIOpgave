using System;
using System.Collections.Generic;

namespace Feedr.Data;

public partial class AppUser
{
    public int UserId { get; set; }

    public string Name { get; set; } = null!;

    public int? AddressId { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public bool IsPremium { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public bool IsAdmin { get; set; }

    public virtual Address? Address { get; set; }

    public virtual ICollection<RestaurantOrder> RestaurantOrders { get; set; } = new List<RestaurantOrder>();

    public virtual ICollection<Restaurant> Restaurants { get; set; } = new List<Restaurant>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
