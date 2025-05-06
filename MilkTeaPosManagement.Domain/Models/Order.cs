using System;
using System.Collections.Generic;

namespace MilkTeaPosManagement.Domain.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public DateTime? CreateAt { get; set; }

    public decimal? TotalAmount { get; set; }

    public string? Note { get; set; }

    public int? StaffId { get; set; }

    public virtual ICollection<Orderitem> Orderitems { get; set; } = new List<Orderitem>();

    public virtual ICollection<Orderstatusupdate> Orderstatusupdates { get; set; } = new List<Orderstatusupdate>();

    public virtual Account? Staff { get; set; }

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public virtual ICollection<Voucherusage> Voucherusages { get; set; } = new List<Voucherusage>();
}
