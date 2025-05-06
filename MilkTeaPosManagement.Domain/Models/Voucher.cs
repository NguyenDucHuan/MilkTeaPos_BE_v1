using System;
using System.Collections.Generic;

namespace MilkTeaPosManagement.Domain.Models;

public partial class Voucher
{
    public int VoucherId { get; set; }

    public string? VoucherCode { get; set; }

    public decimal? DiscountAmount { get; set; }

    public string? DiscountType { get; set; }

    public DateTime? ExpirationDate { get; set; }

    public decimal? MinimumOrderAmount { get; set; }

    public bool? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Voucherusage> Voucherusages { get; set; } = new List<Voucherusage>();
}
