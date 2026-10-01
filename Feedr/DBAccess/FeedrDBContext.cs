using System;
using System.Collections.Generic;
using Feedr.Data;
using Microsoft.EntityFrameworkCore;

namespace Feedr.DBAccess;

public partial class FeedrDBContext : DbContext
{
    public FeedrDBContext(DbContextOptions<FeedrDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<AppUser> AppUsers { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<OrderItemModification> OrderItemModifications { get; set; }

    public virtual DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }

    public virtual DbSet<OrderStatusType> OrderStatusTypes { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Restaurant> Restaurants { get; set; }

    public virtual DbSet<RestaurantOrder> RestaurantOrders { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(entity =>
        {
            entity.ToTable("Address");

            entity.Property(e => e.AddressId).HasColumnName("AddressID");
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.HouseNumber).HasMaxLength(20);
            entity.Property(e => e.PostalCode).HasMaxLength(20);
            entity.Property(e => e.Street).HasMaxLength(200);
        });

        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.ToTable("AppUser");

            entity.HasIndex(e => e.Username, "UQ_AppUser_Username").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.AddressId).HasColumnName("AddressID");
            entity.Property(e => e.Email).HasMaxLength(254);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(30);
            entity.Property(e => e.Username).HasMaxLength(50);

            entity.HasOne(d => d.Address).WithMany(p => p.AppUsers)
                .HasForeignKey(d => d.AddressId)
                .HasConstraintName("FK__AppUser__Address__4CA06362");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.ToTable("OrderItem");

            entity.Property(e => e.OrderItemId).HasColumnName("OrderItemID");
            entity.Property(e => e.BasePriceAtOrder).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.FinalPriceAtOrder).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.RestaurantOrderId).HasColumnName("RestaurantOrderID");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderItem__Produ__619B8048");

            entity.HasOne(d => d.RestaurantOrder).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.RestaurantOrderId)
                .HasConstraintName("FK__OrderItem__Resta__60A75C0F");
        });

        modelBuilder.Entity<OrderItemModification>(entity =>
        {
            entity.HasKey(e => e.ModificationId);

            entity.ToTable("OrderItemModification");

            entity.Property(e => e.ModificationId).HasColumnName("ModificationID");
            entity.Property(e => e.Modification).HasMaxLength(200);
            entity.Property(e => e.OrderItemId).HasColumnName("OrderItemID");
            entity.Property(e => e.PriceChange).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.OrderItem).WithMany(p => p.OrderItemModifications)
                .HasForeignKey(d => d.OrderItemId)
                .HasConstraintName("FK__OrderItem__Order__66603565");
        });

        modelBuilder.Entity<OrderStatusHistory>(entity =>
        {
            entity.HasKey(e => e.HistoryId);

            entity.ToTable("OrderStatusHistory");

            entity.HasIndex(e => new { e.CustomerId, e.OrderId }, "IX_History_Customer_Order");

            entity.Property(e => e.HistoryId).HasColumnName("HistoryID");
            entity.Property(e => e.ChangedAtUtc).HasDefaultValueSql("(sysutcdatetime())", "DF_History_ChangedAt");
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.NewStatusId).HasColumnName("NewStatusID");
            entity.Property(e => e.OldStatusId).HasColumnName("OldStatusID");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
        });

        modelBuilder.Entity<OrderStatusType>(entity =>
        {
            entity.HasKey(e => e.OrderStatusId);

            entity.ToTable("OrderStatusType");

            entity.HasIndex(e => e.StatusName, "UQ_OrderStatusType_Name").IsUnique();

            entity.Property(e => e.OrderStatusId)
                .ValueGeneratedNever()
                .HasColumnName("OrderStatusID");
            entity.Property(e => e.StatusName).HasMaxLength(30);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Product");

            entity.HasIndex(e => new { e.RestaurantId, e.Price }, "IX_Product_Restaurant_Price");

            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.RestaurantId).HasColumnName("RestaurantID");

            entity.HasOne(d => d.Restaurant).WithMany(p => p.Products)
                .HasForeignKey(d => d.RestaurantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Product__Restaur__5441852A");
        });

        modelBuilder.Entity<Restaurant>(entity =>
        {
            entity.ToTable("Restaurant");

            entity.Property(e => e.RestaurantId).HasColumnName("RestaurantID");
            entity.Property(e => e.AddressId).HasColumnName("AddressID");
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.OwnerId).HasColumnName("OwnerID");

            entity.HasOne(d => d.Address).WithMany(p => p.Restaurants)
                .HasForeignKey(d => d.AddressId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Restauran__Addre__5070F446");

            entity.HasOne(d => d.Owner).WithMany(p => p.Restaurants)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Restauran__Owner__5165187F");
        });

        modelBuilder.Entity<RestaurantOrder>(entity =>
        {
            entity.HasKey(e => e.OrderId);

            entity.ToTable("RestaurantOrder", tb => tb.HasTrigger("TR_RestaurantOrder_StatusHistory"));

            entity.HasIndex(e => e.OrderingCustomerId, "IX_Order_Customer");

            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.CreatedAtUtc).HasDefaultValueSql("(sysutcdatetime())", "DF_RestaurantOrder_CreatedAtUtc");
            entity.Property(e => e.OrderStatusId).HasColumnName("OrderStatusID");
            entity.Property(e => e.OrderingCustomerId).HasColumnName("OrderingCustomerID");
            entity.Property(e => e.RestaurantId).HasColumnName("RestaurantID");

            entity.HasOne(d => d.OrderStatus).WithMany(p => p.RestaurantOrders)
                .HasForeignKey(d => d.OrderStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Restauran__Order__5CD6CB2B");

            entity.HasOne(d => d.OrderingCustomer).WithMany(p => p.RestaurantOrders)
                .HasForeignKey(d => d.OrderingCustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Restauran__Order__5AEE82B9");

            entity.HasOne(d => d.Restaurant).WithMany(p => p.RestaurantOrders)
                .HasForeignKey(d => d.RestaurantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Restauran__Resta__5BE2A6F2");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.ToTable("Review");

            entity.Property(e => e.ReviewId).HasColumnName("ReviewID");
            entity.Property(e => e.RestaurantId).HasColumnName("RestaurantID");
            entity.Property(e => e.ReviewDescription).HasMaxLength(2000);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Restaurant).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.RestaurantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Review__Restaura__6B24EA82");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Review__UserID__6A30C649");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
