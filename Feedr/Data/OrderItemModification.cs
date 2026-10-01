using System;
using System.Collections.Generic;

namespace Feedr.Data;

public partial class OrderItemModification
{
    public int ModificationId { get; set; }

    public int OrderItemId { get; set; }

    public string Modification { get; set; } = null!;

    public decimal PriceChange { get; set; }

    public virtual OrderItem OrderItem { get; set; } = null!;
}
