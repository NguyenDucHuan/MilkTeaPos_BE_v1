using System;
using System.Collections.Generic;

namespace MilkTeaPosManagement.Domain.Models;

public partial class Account
{
    public int AccountId { get; set; }

    public string? Username { get; set; }

    public string? FullName { get; set; }

    public string? PasswordHash { get; set; }

    public string? Email { get; set; }

    public string? ImageUrl { get; set; }

    public string? Phone { get; set; }

    public string? Role { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? Status { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Orderstatusupdate> Orderstatusupdates { get; set; } = new List<Orderstatusupdate>();
}
