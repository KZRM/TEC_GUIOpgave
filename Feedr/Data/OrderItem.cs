using System;
using System.Collections.Generic;

namespace Feedr.Data;

public partial class OrderItem
{
    public int OrderItemId { get; set; }

    public int RestaurantOrderId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal BasePriceAtOrder { get; set; }

    public decimal FinalPriceAtOrder { get; set; }

    public virtual ICollection<OrderItemModification> OrderItemModifications { get; set; } = new List<OrderItemModification>();

    public virtual Product Product { get; set; } = null!;

    public virtual RestaurantOrder RestaurantOrder { get; set; } = null!;
}
