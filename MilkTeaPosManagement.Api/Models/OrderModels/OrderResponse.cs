using MilkTeaPosManagement.Domain.Models;

namespace MilkTeaPosManagement.Api.Models.OrderModels
{
    public class OrderResponse
    {
        public int OrderId { get; set; }

        public DateTime? CreateAt { get; set; }

        public decimal? TotalAmount { get; set; }

        public string? Note { get; set; }

        public int? StaffId { get; set; }

        public virtual ICollection<Orderitem> Orderitems { get; set; }

        public virtual ICollection<Orderstatusupdate> Orderstatusupdates { get; set; }

        public virtual Account? Staff { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; }

        public virtual Voucher Voucher { get; set; }
    }
}
