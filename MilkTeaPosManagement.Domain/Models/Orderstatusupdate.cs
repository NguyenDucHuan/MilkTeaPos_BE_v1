using System;
using System.Collections.Generic;

namespace MilkTeaPosManagement.Domain.Models;

public partial class Orderstatusupdate
{
    public int OrderStatusUpdateId { get; set; }

    public string? OrderStatus { get; set; }

    public int? OrderId { get; set; }

    public int? AccountId { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Account? Account { get; set; }

    public virtual Order? Order { get; set; }
}
