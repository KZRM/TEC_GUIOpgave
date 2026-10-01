using System;
using System.Collections.Generic;

namespace Feedr.Data;

public partial class OrderStatusHistory
{
    public int HistoryId { get; set; }

    public int OrderId { get; set; }

    public int CustomerId { get; set; }

    public int OldStatusId { get; set; }

    public int NewStatusId { get; set; }

    public DateTime ChangedAtUtc { get; set; }
}
