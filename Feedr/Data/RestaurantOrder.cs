using System;
using System.Collections.Generic;

namespace Feedr.Data;

public partial class RestaurantOrder
{
    public int OrderId { get; set; }

    public int OrderingCustomerId { get; set; }

    public int RestaurantId { get; set; }

    public int OrderStatusId { get; set; }

    public int? DistanceToDelivery { get; set; }

    public DateTime? CreatedAtUtc { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual OrderStatusType OrderStatus { get; set; } = null!;

    public virtual AppUser OrderingCustomer { get; set; } = null!;

    public virtual Restaurant Restaurant { get; set; } = null!;
}
