using System;
using System.Collections.Generic;

namespace MilkTeaPosManagement.Domain.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string? ProductName { get; set; }

    public int? CategoryId { get; set; }

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public decimal? Prize { get; set; }

    public string? ProductType { get; set; }

    public int? ParentId { get; set; }

    public string? SizeId { get; set; }

    public bool? ToppingAllowed { get; set; }

    public DateTime? CreateAt { get; set; }

    public int? CreateBy { get; set; }

    public DateTime? UpdateAt { get; set; }

    public int? UpdateBy { get; set; }

    public DateTime? DisableAt { get; set; }

    public int? DisableBy { get; set; }

    public bool? Status { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<Comboltem> Comboltems { get; set; } = new List<Comboltem>();

    public virtual ICollection<Orderitem> Orderitems { get; set; } = new List<Orderitem>();

    public virtual ICollection<Toppingforproduct> ToppingforproductProducts { get; set; } = new List<Toppingforproduct>();

    public virtual ICollection<Toppingforproduct> ToppingforproductToppings { get; set; } = new List<Toppingforproduct>();
}
