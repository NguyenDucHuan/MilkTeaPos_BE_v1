using System.Runtime.InteropServices;

namespace MilkTeaPosManagement.Api.Models.TransactionModels
{
    public class TransactionResponse
    {
        public int TransactionId { get; set; }

        public DateTime? TransactionDate { get; set; }

        public decimal? Amount { get; set; }

        public decimal? AmountPaid { get; set; }

        public decimal? ChangeGiven { get; set; }

        public decimal? BeforeCashBalance { get; set; }

        public decimal? AfterCashBalance { get; set; }

        public string? TransactionType { get; set; }

        public string? Description { get; set; }

        public int? OrderId { get; set; }

        public int? PaymentMethodId { get; set; }

        public int? StaffId { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool? Status { get; set; }

        public string? PaymentLink { get; set; }
    }

    public class BalanceResponse
    {
        public decimal? Amount { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public decimal? CashInTotal { get; set; }
        public decimal? CashOutTotal { get; set; }
        public decimal? OpeningBalance { get; set; }
        public decimal? ClosingBalance { get; set; }
        public List<TransactionResponse>? Transactions { get; set; }
    }
}
