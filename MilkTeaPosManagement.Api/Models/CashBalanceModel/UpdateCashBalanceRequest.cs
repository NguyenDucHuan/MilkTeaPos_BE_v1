using MilkTeaPosManagement.Api.Constants;
using System.ComponentModel.DataAnnotations;

namespace MilkTeaPosManagement.Api.Models.CashBalanceModel
{
    public class UpdateCashBalanceRequest
    {
        [Required]
        public decimal Amount { get; set; }

        [Required]
        [RegularExpression($"^({TransactionTypeConstant.CASH_IN}|{TransactionTypeConstant.CASH_OUT})$", ErrorMessage = "Type must be either 'CashIn' or 'CashOut'")]
        public string Type { get; set; } = TransactionTypeConstant.CASH_IN;

        [Required]
        public string Description { get; set; } = string.Empty;
    }
}
