using System.ComponentModel.DataAnnotations;

namespace MilkTeaPosManagement.Api.Models.OrderModels
{
    public class OrderRequest
    {
        public string? Note { get; set; }
        public string? VoucherCode { get; set; }
        //public int StaffId { get; set; }
    }
}
