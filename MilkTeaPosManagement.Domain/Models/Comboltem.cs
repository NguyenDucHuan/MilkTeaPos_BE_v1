using System;
using System.Collections.Generic;

namespace MilkTeaPosManagement.Domain.Models;

public partial class Comboltem
{
    public int ComboltemId { get; set; }

    public int? Combod { get; set; }

    public int? ProductId { get; set; }

    public int? Quantity { get; set; }

    public int? MasterId { get; set; }

    public virtual Product? Product { get; set; }
}
