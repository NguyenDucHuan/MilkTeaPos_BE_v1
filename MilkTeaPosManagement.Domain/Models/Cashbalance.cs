using System;
using System.Collections.Generic;

namespace MilkTeaPosManagement.Domain.Models;

public partial class Cashbalance
{
    public int BalanceId { get; set; }

    public decimal? Amount { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
