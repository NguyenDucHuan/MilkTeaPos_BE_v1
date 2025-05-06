using System;
using System.Collections.Generic;

namespace MilkTeaPosManagement.Domain.Models;

public partial class Toppingforproduct
{
    public int ProductId { get; set; }

    public int ToppingId { get; set; }

    public int? Quantity { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual Product Topping { get; set; } = null!;
}
