using System;
using System.Collections.Generic;

namespace Feedr.Data;

public partial class Review
{
    public int ReviewId { get; set; }

    public int UserId { get; set; }

    public int RestaurantId { get; set; }

    public int Rating { get; set; }

    public string? ReviewDescription { get; set; }

    public virtual Restaurant Restaurant { get; set; } = null!;

    public virtual AppUser User { get; set; } = null!;
}
