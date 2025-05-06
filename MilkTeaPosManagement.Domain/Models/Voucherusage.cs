using System;
using System.Collections.Generic;

namespace MilkTeaPosManagement.Domain.Models;

public partial class Voucherusage
{
    public int VoucherUsageId { get; set; }

    public int? VoucherId { get; set; }

    public int? OrderId { get; set; }

    public decimal? AmountUsed { get; set; }

    public DateTime? UsedAt { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Voucher? Voucher { get; set; }
}
