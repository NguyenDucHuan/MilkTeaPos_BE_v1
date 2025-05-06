using System;
using System.Collections.Generic;

namespace MilkTeaPosManagement.Domain.Models;

public partial class Transaction
{
    public int TransactionId { get; set; }

    public DateTime? TransactionDate { get; set; }

    public decimal? Amount { get; set; }

    public decimal? AmountPaid { get; set; }

    public decimal? ChangeGiven { get; set; }

    public string? TransactionType { get; set; }

    public string? Description { get; set; }

    public int? OrderId { get; set; }

    public int? PaymentMethodId { get; set; }

    public int? StaffId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? Status { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Paymentmethod? PaymentMethod { get; set; }

    public virtual Account? Staff { get; set; }
}
