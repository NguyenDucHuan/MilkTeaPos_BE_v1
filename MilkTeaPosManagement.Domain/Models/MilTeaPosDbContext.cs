using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace MilkTeaPosManagement.Domain.Models;

public partial class MilTeaPosDbContext : DbContext
{
    public MilTeaPosDbContext()
    {
    }

    public MilTeaPosDbContext(DbContextOptions<MilTeaPosDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Comboltem> Comboltems { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Orderitem> Orderitems { get; set; }

    public virtual DbSet<Orderstatusupdate> Orderstatusupdates { get; set; }

    public virtual DbSet<Paymentmethod> Paymentmethods { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Toppingforproduct> Toppingforproducts { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<Voucher> Vouchers { get; set; }

    public virtual DbSet<Voucherusage> Voucherusages { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;port=3306;database=milktea_pos_db;uid=root;pwd=123456", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.41-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PRIMARY");

            entity.ToTable("accounts");

            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("Created_at");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.ImageUrl).HasMaxLength(255);
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("Password_hash");
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Role).HasColumnType("enum('Staff','Manager')");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("Updated_at");
            entity.Property(e => e.Username).HasMaxLength(100);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PRIMARY");

            entity.ToTable("categories");

            entity.Property(e => e.CategoryName).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.ImageUrl).HasMaxLength(255);
        });

        modelBuilder.Entity<Comboltem>(entity =>
        {
            entity.HasKey(e => e.ComboltemId).HasName("PRIMARY");

            entity.ToTable("comboltems");

            entity.HasIndex(e => e.ProductId, "ProductID");

            entity.Property(e => e.MasterId).HasColumnName("MasterID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");

            entity.HasOne(d => d.Product).WithMany(p => p.Comboltems)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("comboltems_ibfk_1");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PRIMARY");

            entity.ToTable("orders");

            entity.HasIndex(e => e.StaffId, "StaffID");

            entity.Property(e => e.CreateAt)
                .HasColumnType("datetime")
                .HasColumnName("Create_at");
            entity.Property(e => e.Note).HasColumnType("text");
            entity.Property(e => e.StaffId).HasColumnName("StaffID");
            entity.Property(e => e.TotalAmount).HasPrecision(10, 2);

            entity.HasOne(d => d.Staff).WithMany(p => p.Orders)
                .HasForeignKey(d => d.StaffId)
                .HasConstraintName("orders_ibfk_1");
        });

        modelBuilder.Entity<Orderitem>(entity =>
        {
            entity.HasKey(e => e.OrderItemId).HasName("PRIMARY");

            entity.ToTable("orderitems");

            entity.HasIndex(e => e.OrderId, "OrderId");

            entity.HasIndex(e => e.ProductId, "ProductId");

            entity.Property(e => e.MasterId).HasColumnName("MasterID");
            entity.Property(e => e.Price).HasPrecision(10, 2);

            entity.HasOne(d => d.Order).WithMany(p => p.Orderitems)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("orderitems_ibfk_2");

            entity.HasOne(d => d.Product).WithMany(p => p.Orderitems)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("orderitems_ibfk_1");
        });

        modelBuilder.Entity<Orderstatusupdate>(entity =>
        {
            entity.HasKey(e => e.OrderStatusUpdateId).HasName("PRIMARY");

            entity.ToTable("orderstatusupdates");

            entity.HasIndex(e => e.AccountId, "AccountId");

            entity.HasIndex(e => e.OrderId, "OrderId");

            entity.Property(e => e.OrderStatus).HasColumnType("enum('Pending','Shipped','Delivered','Success','Cancelled')");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("Updated_at");

            entity.HasOne(d => d.Account).WithMany(p => p.Orderstatusupdates)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("orderstatusupdates_ibfk_2");

            entity.HasOne(d => d.Order).WithMany(p => p.Orderstatusupdates)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("orderstatusupdates_ibfk_1");
        });

        modelBuilder.Entity<Paymentmethod>(entity =>
        {
            entity.HasKey(e => e.PaymentMethodId).HasName("PRIMARY");

            entity.ToTable("paymentmethods");

            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.MethodName).HasMaxLength(20);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PRIMARY");

            entity.ToTable("products");

            entity.HasIndex(e => e.CategoryId, "CategoryId");

            entity.Property(e => e.CreateAt)
                .HasColumnType("datetime")
                .HasColumnName("Create_at");
            entity.Property(e => e.CreateBy).HasColumnName("Create_by");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.DisableAt)
                .HasColumnType("datetime")
                .HasColumnName("Disable_at");
            entity.Property(e => e.DisableBy).HasColumnName("Disable_by");
            entity.Property(e => e.ImageUrl)
                .HasColumnType("text")
                .HasColumnName("ImageURL");
            entity.Property(e => e.ParentId).HasColumnName("ParentID");
            entity.Property(e => e.Prize).HasPrecision(10, 2);
            entity.Property(e => e.ProductName).HasMaxLength(100);
            entity.Property(e => e.ProductType).HasColumnType("enum('MaterProduct','SingleProduct','Extra','Combo')");
            entity.Property(e => e.SizeId).HasColumnType("enum('Parent','Small','Medium','Large')");
            entity.Property(e => e.UpdateAt)
                .HasColumnType("datetime")
                .HasColumnName("Update_at");
            entity.Property(e => e.UpdateBy).HasColumnName("Update_by");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("products_ibfk_1");
        });

        modelBuilder.Entity<Toppingforproduct>(entity =>
        {
            entity.HasKey(e => new { e.ProductId, e.ToppingId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("toppingforproduct");

            entity.HasIndex(e => e.ToppingId, "ToppingId");

            entity.Property(e => e.Quantity).HasDefaultValueSql("'1'");

            entity.HasOne(d => d.Product).WithMany(p => p.ToppingforproductProducts)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("toppingforproduct_ibfk_1");

            entity.HasOne(d => d.Topping).WithMany(p => p.ToppingforproductToppings)
                .HasForeignKey(d => d.ToppingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("toppingforproduct_ibfk_2");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PRIMARY");

            entity.ToTable("transactions");

            entity.HasIndex(e => e.OrderId, "OrderId");

            entity.HasIndex(e => e.PaymentMethodId, "PaymentMethodId");

            entity.HasIndex(e => e.StaffId, "StaffId");

            entity.Property(e => e.Amount).HasPrecision(10, 2);
            entity.Property(e => e.AmountPaid).HasPrecision(10, 2);
            entity.Property(e => e.ChangeGiven).HasPrecision(10, 2);
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("Created_at");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.TransactionDate).HasColumnType("datetime");
            entity.Property(e => e.TransactionType).HasMaxLength(20);
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("Updated_at");

            entity.HasOne(d => d.Order).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("transactions_ibfk_1");

            entity.HasOne(d => d.PaymentMethod).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.PaymentMethodId)
                .HasConstraintName("transactions_ibfk_2");

            entity.HasOne(d => d.Staff).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.StaffId)
                .HasConstraintName("transactions_ibfk_3");
        });

        modelBuilder.Entity<Voucher>(entity =>
        {
            entity.HasKey(e => e.VoucherId).HasName("PRIMARY");

            entity.ToTable("vouchers");

            entity.HasIndex(e => e.VoucherCode, "VoucherCode").IsUnique();

            entity.Property(e => e.CreateAt)
                .HasColumnType("datetime")
                .HasColumnName("Create_at");
            entity.Property(e => e.CreateBy).HasColumnName("Create_by");
            entity.Property(e => e.DisableAt)
                .HasColumnType("datetime")
                .HasColumnName("Disable_at");
            entity.Property(e => e.DisableBy).HasColumnName("Disable_by");
            entity.Property(e => e.DiscountAmount).HasPrecision(10, 2);
            entity.Property(e => e.DiscountType).HasColumnType("enum('Amount','Percentage')");
            entity.Property(e => e.ExpirationDate).HasColumnType("datetime");
            entity.Property(e => e.MinimumOrderAmount)
                .HasPrecision(10, 2)
                .HasDefaultValueSql("'0.00'");
            entity.Property(e => e.UpdateAt)
                .HasColumnType("datetime")
                .HasColumnName("Update_at");
            entity.Property(e => e.UpdateBy).HasColumnName("Update_by");
            entity.Property(e => e.VoucherCode).HasMaxLength(50);
        });

        modelBuilder.Entity<Voucherusage>(entity =>
        {
            entity.HasKey(e => e.VoucherUsageId).HasName("PRIMARY");

            entity.ToTable("voucherusages");

            entity.HasIndex(e => e.OrderId, "OrderId");

            entity.HasIndex(e => e.VoucherId, "VoucherId");

            entity.Property(e => e.AmountUsed).HasPrecision(10, 2);
            entity.Property(e => e.UsedAt)
                .HasColumnType("datetime")
                .HasColumnName("Used_at");

            entity.HasOne(d => d.Order).WithMany(p => p.Voucherusages)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("voucherusages_ibfk_2");

            entity.HasOne(d => d.Voucher).WithMany(p => p.Voucherusages)
                .HasForeignKey(d => d.VoucherId)
                .HasConstraintName("voucherusages_ibfk_1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
