﻿using System;
using System.Collections.Generic;

namespace MilkTeaPosManagement.Domain.Models;

public partial class Orderitem
{
    public int OrderItemId { get; set; }

    public int? Quantity { get; set; }

    public decimal? Price { get; set; }

    public int? MasterId { get; set; }

    public int? ProductId { get; set; }

    public int? OrderId { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Product? Product { get; set; }
}
