using System;
using System.Collections.Generic;

namespace Feedr.Data;

public partial class Product
{
    public int ProductId { get; set; }

    public int RestaurantId { get; set; }

    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual Restaurant Restaurant { get; set; } = null!;
}
