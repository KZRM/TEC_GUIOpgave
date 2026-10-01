using System;
using System.Collections.Generic;

namespace Feedr.Data;

public partial class OrderStatusType
{
    public int OrderStatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public virtual ICollection<RestaurantOrder> RestaurantOrders { get; set; } = new List<RestaurantOrder>();
}
