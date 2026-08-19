using System;
using System.Collections.Generic;
using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Persistence;

public partial class ECommerceDbContext : DbContext
{
    public ECommerceDbContext(DbContextOptions<ECommerceDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<AiknowledgeBase> AiknowledgeBases { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<ChatHistory> ChatHistories { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductImage> ProductImages { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Shipment> Shipments { get; set; }

    public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Wishlist> Wishlists { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Addresse__3214EC077E0D7282");

            entity.HasIndex(e => e.UserId, "IX_Addresses_UserId").HasFilter("([IsDeleted]=(0))");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.AddressType).HasMaxLength(50);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.Country).HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.FullName).HasMaxLength(255);
            entity.Property(e => e.IsDefaultAddress).HasDefaultValue(false);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.PostalCode).HasMaxLength(20);
            entity.Property(e => e.StateProvince).HasMaxLength(100);
            entity.Property(e => e.StreetAddress).HasMaxLength(255);

            entity.HasOne(d => d.User).WithMany(p => p.Addresses)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Addresses_User");
        });

        modelBuilder.Entity<AiknowledgeBase>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AIKnowle__3214EC076870762C");

            entity.ToTable("AIKnowledgeBase");

            entity.HasIndex(e => e.Category, "IX_AIKnowledgeBase_Category");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Category).HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.SourceType).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(255);
            // AIKnowledgeBase has no IsDeleted column - BaseEntity adds the
            // property in C#, but nothing should try to persist it.
            entity.Ignore(e => e.IsDeleted);

            entity.HasOne(d => d.Product).WithMany(p => p.AiknowledgeBases)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_AIKnowledgeBase_Product");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Categori__3214EC07703BC546");

            entity.HasIndex(e => e.ParentCategoryId, "IX_Categories_ParentCategoryId").HasFilter("([IsDeleted]=(0))");

            entity.HasIndex(e => e.Slug, "IX_Categories_Slug").HasFilter("([IsDeleted]=(0))");

            entity.HasIndex(e => e.Slug, "UQ__Categori__BC7B5FB6279B6FAD").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.DisplayOrder).HasDefaultValue(0);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Slug).HasMaxLength(200);

            entity.HasOne(d => d.ParentCategory).WithMany(p => p.InverseParentCategory)
                .HasForeignKey(d => d.ParentCategoryId)
                .HasConstraintName("FK_Categories_ParentCategory");
        });

        modelBuilder.Entity<ChatHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ChatHist__3214EC072831E8D7");

            entity.ToTable("ChatHistory");

            entity.HasIndex(e => e.CustomerId, "IX_ChatHistory_CustomerId");

            entity.HasIndex(e => e.SessionId, "IX_ChatHistory_SessionId");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.MessageRole).HasMaxLength(50);
            // ChatHistory has no UpdatedAt/IsDeleted columns (append-only log).
            entity.Ignore(e => e.UpdatedAt);
            entity.Ignore(e => e.IsDeleted);

            entity.HasOne(d => d.Customer).WithMany(p => p.ChatHistories)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChatHistory_Customer");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Customer__3214EC07D7D014EF");

            entity.HasIndex(e => e.UserId, "IX_Customers_UserId").HasFilter("([IsDeleted]=(0))");

            entity.HasIndex(e => e.UserId, "UQ__Customer__1788CC4D5451C216").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CompanyName).HasMaxLength(255);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.LoyaltyPoints).HasDefaultValue(0);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.TotalSpending)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.User).WithOne(p => p.Customer)
                .HasForeignKey<Customer>(d => d.UserId)
                .HasConstraintName("FK_Customers_User");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Orders__3214EC0746B7ED6C");

            entity.HasIndex(e => e.CreatedAt, "IX_Orders_CreatedAt").HasFilter("([IsDeleted]=(0))");

            entity.HasIndex(e => e.CustomerId, "IX_Orders_CustomerId").HasFilter("([IsDeleted]=(0))");

            entity.HasIndex(e => e.OrderStatus, "IX_Orders_OrderStatus").HasFilter("([IsDeleted]=(0))");

            entity.HasIndex(e => e.OrderNumber, "UQ__Orders__CAC5E7430D3EF7F3").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.CurrencyCode)
                .HasMaxLength(3)
                .HasDefaultValue("USD");
            entity.Property(e => e.DiscountAmount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.OrderNumber).HasMaxLength(50);
            entity.Property(e => e.OrderStatus)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");
            entity.Property(e => e.ShippingCost)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TaxAmount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.BillingAddress).WithMany(p => p.OrderBillingAddresses)
                .HasForeignKey(d => d.BillingAddressId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_BillingAddress");

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_Customer");

            entity.HasOne(d => d.ShippingAddress).WithMany(p => p.OrderShippingAddresses)
                .HasForeignKey(d => d.ShippingAddressId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_ShippingAddress");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OrderIte__3214EC074A58E657");

            entity.HasIndex(e => e.OrderId, "IX_OrderItems_OrderId");

            entity.HasIndex(e => e.ProductId, "IX_OrderItems_ProductId");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.DiscountPercentage)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(5, 2)");
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");
            // OrderItems has no audit columns at all (immutable line items,
            // cascade-deleted with the Order).
            entity.Ignore(e => e.CreatedAt);
            entity.Ignore(e => e.UpdatedAt);
            entity.Ignore(e => e.IsDeleted);

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK_OrderItems_Order");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderItems_Product");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Payments__3214EC07A7525ECA");

            entity.HasIndex(e => e.OrderId, "IX_Payments_OrderId");

            entity.HasIndex(e => e.PaymentStatus, "IX_Payments_PaymentStatus");

            entity.HasIndex(e => e.TransactionId, "IX_Payments_TransactionId");

            entity.HasIndex(e => e.TransactionId, "UQ__Payments__55433A6A69E57D4E").IsUnique();

            entity.HasIndex(e => e.OrderId, "UQ__Payments__C3905BCE4A13BDA4").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.PaymentGateway).HasMaxLength(50);
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");
            entity.Property(e => e.RefundAmount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TransactionId).HasMaxLength(255);

            entity.HasOne(d => d.Order).WithOne(p => p.Payment)
                .HasForeignKey<Payment>(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Payments_Order");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Products__3214EC0724297208");

            entity.HasIndex(e => e.CategoryId, "IX_Products_CategoryId").HasFilter("([IsDeleted]=(0))");

            entity.HasIndex(e => e.IsActive, "IX_Products_IsActive").HasFilter("([IsDeleted]=(0))");

            entity.HasIndex(e => e.Rating, "IX_Products_Rating").HasFilter("([IsDeleted]=(0) AND [IsActive]=(1))");

            entity.HasIndex(e => e.Slug, "IX_Products_Slug").HasFilter("([IsDeleted]=(0))");

            entity.HasIndex(e => e.Slug, "UQ__Products__BC7B5FB621985DE0").IsUnique();

            entity.HasIndex(e => e.Sku, "UQ__Products__CA1FD3C557FF95D6").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.DiscountPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.IsFeatured).HasDefaultValue(false);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Rating)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(3, 2)");
            entity.Property(e => e.ReviewCount).HasDefaultValue(0);
            entity.Property(e => e.SalesCount).HasDefaultValue(0);
            entity.Property(e => e.ShortDescription).HasMaxLength(500);
            entity.Property(e => e.Sku).HasMaxLength(100);
            entity.Property(e => e.Slug).HasMaxLength(255);
            entity.Property(e => e.Vendor).HasMaxLength(255);
            entity.Property(e => e.ViewCount).HasDefaultValue(0);

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Products_Category");
        });

        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProductI__3214EC079FCEFE4C");

            entity.HasIndex(e => e.ProductId, "IX_ProductImages_ProductId");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.AltText).HasMaxLength(255);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.DisplayOrder).HasDefaultValue(0);
            entity.Property(e => e.IsMainImage).HasDefaultValue(false);
            // ProductImages only has CreatedAt - no UpdatedAt/IsDeleted columns.
            entity.Ignore(e => e.UpdatedAt);
            entity.Ignore(e => e.IsDeleted);

            entity.HasOne(d => d.Product).WithMany(p => p.ProductImages)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_ProductImages_Product");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Reviews__3214EC07700B2A9E");

            entity.HasIndex(e => e.CustomerId, "IX_Reviews_CustomerId");

            entity.HasIndex(e => e.ProductId, "IX_Reviews_ProductId").HasFilter("([Status]='Approved' AND [IsDeleted]=(0))");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Helpful).HasDefaultValue(0);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.IsVerifiedPurchase).HasDefaultValue(false);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.Unhelpful).HasDefaultValue(0);

            entity.HasOne(d => d.Customer).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reviews_Customer");

            entity.HasOne(d => d.Order).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK_Reviews_Order");

            entity.HasOne(d => d.Product).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reviews_Product");
        });

        modelBuilder.Entity<Shipment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Shipment__3214EC07FADE1C8E");

            entity.HasIndex(e => e.OrderId, "IX_Shipments_OrderId");

            entity.HasIndex(e => e.ShipmentStatus, "IX_Shipments_ShipmentStatus");

            entity.HasIndex(e => e.TrackingNumber, "IX_Shipments_TrackingNumber");

            entity.HasIndex(e => e.TrackingNumber, "UQ__Shipment__784DB3D9B86A1FFF").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CarrierName).HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Dimensions).HasMaxLength(100);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.ShipmentStatus)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");
            entity.Property(e => e.TrackingNumber).HasMaxLength(100);
            entity.Property(e => e.Weight).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Order).WithMany(p => p.Shipments)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Shipments_Order");
        });

        modelBuilder.Entity<ShoppingCart>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Shopping__3214EC0748DE3415");

            entity.ToTable("ShoppingCart");

            entity.HasIndex(e => e.CustomerId, "IX_ShoppingCart_CustomerId");

            entity.HasIndex(e => new { e.CustomerId, e.ProductId }, "UQ_ShoppingCart_CustomerProduct").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.AddedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            // ShoppingCart uses AddedAt instead of CreatedAt - no CreatedAt column.
            entity.Ignore(e => e.CreatedAt);

            entity.HasOne(d => d.Customer).WithMany(p => p.ShoppingCarts)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_ShoppingCart_Customer");

            entity.HasOne(d => d.Product).WithMany(p => p.ShoppingCarts)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ShoppingCart_Product");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC0747C57B14");

            entity.HasIndex(e => e.Email, "IX_Users_Email").HasFilter("([IsDeleted]=(0))");

            entity.HasIndex(e => e.Role, "IX_Users_Role").HasFilter("([IsDeleted]=(0))");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D1053433EA0165").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.IsEmailVerified).HasDefaultValue(false);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .HasDefaultValue("Customer");
        });

        modelBuilder.Entity<Wishlist>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Wishlist__3214EC072F186E26");

            entity.ToTable("Wishlist");

            entity.HasIndex(e => e.CustomerId, "IX_Wishlist_CustomerId").HasFilter("([IsDeleted]=(0))");

            entity.HasIndex(e => new { e.CustomerId, e.ProductId }, "UQ_Wishlist_CustomerProduct").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.AddedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            // Wishlist uses AddedAt instead of CreatedAt, and has no UpdatedAt column.
            entity.Ignore(e => e.CreatedAt);
            entity.Ignore(e => e.UpdatedAt);

            entity.HasOne(d => d.Customer).WithMany(p => p.Wishlists)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_Wishlist_Customer");

            entity.HasOne(d => d.Product).WithMany(p => p.Wishlists)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Wishlist_Product");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
