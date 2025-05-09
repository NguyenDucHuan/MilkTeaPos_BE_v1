using System;
using System.Collections.Generic;

namespace MilkTeaPosManagement.Domain.Models;

public partial class Cashflow
{
    public int CashFlowId { get; set; }

    public DateOnly? Date { get; set; }

    public decimal? CashIn { get; set; }

    public decimal? CashOut { get; set; }

    public decimal? NetCash { get; set; }

    public decimal? CashBalance { get; set; }

    public int? UserId { get; set; }

    public string? FlowType { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? Status { get; set; }

    public virtual Account? User { get; set; }
}
