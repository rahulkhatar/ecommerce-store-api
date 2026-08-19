---
name: database-design
description: "MSSQL schema design conventions for the ECommerce platform - naming rules, audit columns, the full 15-table core schema already defined in database/migrations/001_InitialSchema.sql, RAG tables, indexing and migration strategy. Load before creating migrations, EF configs, or reviewing schema changes."
---

# Database Design & Migrations Guide

## MSSQL Schema Design Principles

### 1. Naming Conventions
```sql
-- Tables: PascalCase (e.g., Products, OrderItems)
-- Columns: PascalCase (e.g., CreatedAt, ProductId)
-- Stored Procedures: sp_ActionObject (e.g., sp_CreateOrder)
-- Functions: fn_DescriptiveActionObject
-- Indexes: IX_TableName_ColumnName
-- Foreign Keys: FK_Table1_Table2
-- Constraints: CK_TableName_Description
```

### 2. Base Entity Template
All tables should have audit columns:
```sql
Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
UpdatedAt DATETIME2 NULL,
IsDeleted BIT DEFAULT 0
```

## Core Tables Design

### 1. Users Table (Authentication)
```sql
CREATE TABLE [dbo].[Users]
(
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [Email] NVARCHAR(255) UNIQUE NOT NULL,
    [PasswordHash] NVARCHAR(MAX) NOT NULL,
    [FirstName] NVARCHAR(100) NOT NULL,
    [LastName] NVARCHAR(100) NOT NULL,
    [PhoneNumber] NVARCHAR(20) NULL,
    [ProfilePictureUrl] NVARCHAR(MAX) NULL,
    [Role] NVARCHAR(50) NOT NULL DEFAULT 'Customer', -- Admin, Vendor, Customer
    [IsEmailVerified] BIT DEFAULT 0,
    [EmailVerificationToken] NVARCHAR(MAX) NULL,
    [EmailVerificationTokenExpiry] DATETIME2 NULL,
    [PasswordResetToken] NVARCHAR(MAX) NULL,
    [PasswordResetTokenExpiry] DATETIME2 NULL,
    [LastLoginAt] DATETIME2 NULL,
    [CreatedAt] DATETIME2 DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NULL,
    [IsDeleted] BIT DEFAULT 0,
    
    CONSTRAINT CK_Users_Email CHECK (Email LIKE '%@%.%'),
    CONSTRAINT CK_Users_Role CHECK (Role IN ('Admin', 'Vendor', 'Customer'))
);

CREATE NONCLUSTERED INDEX IX_Users_Email ON [dbo].[Users] ([Email]);
CREATE NONCLUSTERED INDEX IX_Users_Role ON [dbo].[Users] ([Role]) WHERE [IsDeleted] = 0;
```

### 2. Categories Table
```sql
CREATE TABLE [dbo].[Categories]
(
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [Name] NVARCHAR(200) NOT NULL,
    [Slug] NVARCHAR(200) UNIQUE NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [ImageUrl] NVARCHAR(MAX) NULL,
    [ParentCategoryId] UNIQUEIDENTIFIER NULL,
    [DisplayOrder] INT DEFAULT 0,
    [IsActive] BIT DEFAULT 1,
    [CreatedAt] DATETIME2 DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NULL,
    [IsDeleted] BIT DEFAULT 0,
    
    CONSTRAINT FK_Categories_ParentCategory 
        FOREIGN KEY ([ParentCategoryId]) 
        REFERENCES [dbo].[Categories] ([Id])
);

CREATE NONCLUSTERED INDEX IX_Categories_Slug ON [dbo].[Categories] ([Slug]);
CREATE NONCLUSTERED INDEX IX_Categories_ParentCategoryId ON [dbo].[Categories] ([ParentCategoryId]) 
    WHERE [IsDeleted] = 0;
```

### 3. Products Table
```sql
CREATE TABLE [dbo].[Products]
(
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [Name] NVARCHAR(255) NOT NULL,
    [Slug] NVARCHAR(255) UNIQUE NOT NULL,
    [Description] NVARCHAR(MAX) NOT NULL,
    [ShortDescription] NVARCHAR(500) NULL,
    [CategoryId] UNIQUEIDENTIFIER NOT NULL,
    [Price] DECIMAL(18, 2) NOT NULL,
    [DiscountPrice] DECIMAL(18, 2) NULL,
    [StockQuantity] INT NOT NULL DEFAULT 0,
    [Sku] NVARCHAR(100) UNIQUE NOT NULL,
    [ImageUrl] NVARCHAR(MAX) NULL,
    [Vendor] NVARCHAR(255) NULL,
    [Rating] DECIMAL(3, 2) DEFAULT 0,
    [ReviewCount] INT DEFAULT 0,
    [ViewCount] INT DEFAULT 0,
    [SalesCount] INT DEFAULT 0,
    [IsActive] BIT DEFAULT 1,
    [IsFeatured] BIT DEFAULT 0,
    [CreatedAt] DATETIME2 DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NULL,
    [IsDeleted] BIT DEFAULT 0,
    
    CONSTRAINT FK_Products_Category 
        FOREIGN KEY ([CategoryId]) 
        REFERENCES [dbo].[Categories] ([Id]),
    CONSTRAINT CK_Products_Price CHECK ([Price] >= 0),
    CONSTRAINT CK_Products_Discount CHECK ([DiscountPrice] IS NULL OR [DiscountPrice] < [Price])
);

CREATE NONCLUSTERED INDEX IX_Products_CategoryId ON [dbo].[Products] ([CategoryId]) 
    WHERE [IsDeleted] = 0;
CREATE NONCLUSTERED INDEX IX_Products_Slug ON [dbo].[Products] ([Slug]);
CREATE NONCLUSTERED INDEX IX_Products_IsActive ON [dbo].[Products] ([IsActive]) 
    WHERE [IsDeleted] = 0;
CREATE NONCLUSTERED INDEX IX_Products_Rating ON [dbo].[Products] ([Rating]) 
    WHERE [IsDeleted] = 0 AND [IsActive] = 1;

-- Full-text index for search
CREATE FULLTEXT CATALOG ft_Products;
CREATE FULLTEXT INDEX ON [dbo].[Products]([Name], [Description])
    KEY INDEX PK_Products ON ft_Products;
```

### 4. Product Images Table
```sql
CREATE TABLE [dbo].[ProductImages]
(
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [ProductId] UNIQUEIDENTIFIER NOT NULL,
    [ImageUrl] NVARCHAR(MAX) NOT NULL,
    [AltText] NVARCHAR(255) NULL,
    [DisplayOrder] INT DEFAULT 0,
    [IsMainImage] BIT DEFAULT 0,
    [CreatedAt] DATETIME2 DEFAULT GETUTCDATE(),
    
    CONSTRAINT FK_ProductImages_Product 
        FOREIGN KEY ([ProductId]) 
        REFERENCES [dbo].[Products] ([Id]) ON DELETE CASCADE
);

CREATE NONCLUSTERED INDEX IX_ProductImages_ProductId ON [dbo].[ProductImages] ([ProductId]);
```

### 5. Customers Table
```sql
CREATE TABLE [dbo].[Customers]
(
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [UserId] UNIQUEIDENTIFIER NOT NULL UNIQUE,
    [PhoneNumber] NVARCHAR(20) NULL,
    [DateOfBirth] DATE NULL,
    [Gender] NVARCHAR(10) NULL,
    [CompanyName] NVARCHAR(255) NULL,
    [TotalSpending] DECIMAL(18, 2) DEFAULT 0,
    [LoyaltyPoints] INT DEFAULT 0,
    [CreatedAt] DATETIME2 DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NULL,
    [IsDeleted] BIT DEFAULT 0,
    
    CONSTRAINT FK_Customers_User 
        FOREIGN KEY ([UserId]) 
        REFERENCES [dbo].[Users] ([Id]) ON DELETE CASCADE
);

CREATE NONCLUSTERED INDEX IX_Customers_UserId ON [dbo].[Customers] ([UserId]);
```

### 6. Addresses Table
```sql
CREATE TABLE [dbo].[Addresses]
(
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [AddressType] NVARCHAR(50) NOT NULL, -- Home, Office, Other
    [FullName] NVARCHAR(255) NOT NULL,
    [PhoneNumber] NVARCHAR(20) NOT NULL,
    [StreetAddress] NVARCHAR(255) NOT NULL,
    [City] NVARCHAR(100) NOT NULL,
    [StateProvince] NVARCHAR(100) NOT NULL,
    [PostalCode] NVARCHAR(20) NOT NULL,
    [Country] NVARCHAR(100) NOT NULL,
    [IsDefaultAddress] BIT DEFAULT 0,
    [CreatedAt] DATETIME2 DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NULL,
    [IsDeleted] BIT DEFAULT 0,
    
    CONSTRAINT FK_Addresses_User 
        FOREIGN KEY ([UserId]) 
        REFERENCES [dbo].[Users] ([Id]) ON DELETE CASCADE
);

CREATE NONCLUSTERED INDEX IX_Addresses_UserId ON [dbo].[Addresses] ([UserId]) 
    WHERE [IsDeleted] = 0;
```

### 7. Orders Table
```sql
CREATE TABLE [dbo].[Orders]
(
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [OrderNumber] NVARCHAR(50) UNIQUE NOT NULL,
    [CustomerId] UNIQUEIDENTIFIER NOT NULL,
    [ShippingAddressId] UNIQUEIDENTIFIER NOT NULL,
    [BillingAddressId] UNIQUEIDENTIFIER NOT NULL,
    [OrderStatus] NVARCHAR(50) NOT NULL, -- Pending, Confirmed, Processing, Shipped, Delivered, Cancelled
    [TotalAmount] DECIMAL(18, 2) NOT NULL,
    [ShippingCost] DECIMAL(18, 2) DEFAULT 0,
    [TaxAmount] DECIMAL(18, 2) DEFAULT 0,
    [DiscountAmount] DECIMAL(18, 2) DEFAULT 0,
    [Notes] NVARCHAR(MAX) NULL,
    [CurrencyCode] NVARCHAR(3) DEFAULT 'USD',
    [CreatedAt] DATETIME2 DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NULL,
    [IsDeleted] BIT DEFAULT 0,
    
    CONSTRAINT FK_Orders_Customer 
        FOREIGN KEY ([CustomerId]) 
        REFERENCES [dbo].[Customers] ([Id]),
    CONSTRAINT FK_Orders_ShippingAddress 
        FOREIGN KEY ([ShippingAddressId]) 
        REFERENCES [dbo].[Addresses] ([Id]),
    CONSTRAINT FK_Orders_BillingAddress 
        FOREIGN KEY ([BillingAddressId]) 
        REFERENCES [dbo].[Addresses] ([Id]),
    CONSTRAINT CK_Orders_Status CHECK (
        OrderStatus IN ('Pending', 'Confirmed', 'Processing', 'Shipped', 'Delivered', 'Cancelled')
    ),
    CONSTRAINT CK_Orders_Amount CHECK ([TotalAmount] >= 0)
);

CREATE NONCLUSTERED INDEX IX_Orders_CustomerId ON [dbo].[Orders] ([CustomerId]) 
    WHERE [IsDeleted] = 0;
CREATE NONCLUSTERED INDEX IX_Orders_OrderStatus ON [dbo].[Orders] ([OrderStatus]) 
    WHERE [IsDeleted] = 0;
CREATE NONCLUSTERED INDEX IX_Orders_CreatedAt ON [dbo].[Orders] ([CreatedAt]) 
    WHERE [IsDeleted] = 0;
```

### 8. Order Items Table
```sql
CREATE TABLE [dbo].[OrderItems]
(
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [OrderId] UNIQUEIDENTIFIER NOT NULL,
    [ProductId] UNIQUEIDENTIFIER NOT NULL,
    [Quantity] INT NOT NULL,
    [UnitPrice] DECIMAL(18, 2) NOT NULL,
    [DiscountPercentage] DECIMAL(5, 2) DEFAULT 0,
    [TotalPrice] DECIMAL(18, 2) NOT NULL,
    
    CONSTRAINT FK_OrderItems_Order 
        FOREIGN KEY ([OrderId]) 
        REFERENCES [dbo].[Orders] ([Id]) ON DELETE CASCADE,
    CONSTRAINT FK_OrderItems_Product 
        FOREIGN KEY ([ProductId]) 
        REFERENCES [dbo].[Products] ([Id]),
    CONSTRAINT CK_OrderItems_Quantity CHECK ([Quantity] > 0)
);

CREATE NONCLUSTERED INDEX IX_OrderItems_OrderId ON [dbo].[OrderItems] ([OrderId]);
CREATE NONCLUSTERED INDEX IX_OrderItems_ProductId ON [dbo].[OrderItems] ([ProductId]);
```

### 9. Payments Table
```sql
CREATE TABLE [dbo].[Payments]
(
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [OrderId] UNIQUEIDENTIFIER NOT NULL,
    [TransactionId] NVARCHAR(255) UNIQUE NULL,
    [Amount] DECIMAL(18, 2) NOT NULL,
    [PaymentMethod] NVARCHAR(50) NOT NULL, -- CreditCard, DebitCard, Wallet, Paypal, etc
    [PaymentGateway] NVARCHAR(50) NOT NULL, -- Stripe, Razorpay, etc
    [PaymentStatus] NVARCHAR(50) NOT NULL DEFAULT 'Pending', -- Pending, Completed, Failed, Refunded
    [ProcessedAt] DATETIME2 NULL,
    [RefundAmount] DECIMAL(18, 2) DEFAULT 0,
    [RefundedAt] DATETIME2 NULL,
    [FailureReason] NVARCHAR(MAX) NULL,
    [CreatedAt] DATETIME2 DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NULL,
    [IsDeleted] BIT DEFAULT 0,
    
    CONSTRAINT FK_Payments_Order 
        FOREIGN KEY ([OrderId]) 
        REFERENCES [dbo].[Orders] ([Id]),
    CONSTRAINT CK_Payments_Amount CHECK ([Amount] > 0)
);

CREATE NONCLUSTERED INDEX IX_Payments_OrderId ON [dbo].[Payments] ([OrderId]);
CREATE NONCLUSTERED INDEX IX_Payments_TransactionId ON [dbo].[Payments] ([TransactionId]);
CREATE NONCLUSTERED INDEX IX_Payments_PaymentStatus ON [dbo].[Payments] ([PaymentStatus]);
```

### 10. Shipments Table
```sql
CREATE TABLE [dbo].[Shipments]
(
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [OrderId] UNIQUEIDENTIFIER NOT NULL,
    [TrackingNumber] NVARCHAR(100) UNIQUE NOT NULL,
    [CarrierName] NVARCHAR(100) NOT NULL, -- FedEx, UPS, DHL, etc
    [ShipmentStatus] NVARCHAR(50) NOT NULL, -- Pending, Picked, Dispatched, InTransit, Delivered, Failed
    [ShippedAt] DATETIME2 NULL,
    [EstimatedDeliveryAt] DATETIME2 NULL,
    [DeliveredAt] DATETIME2 NULL,
    [Weight] DECIMAL(10, 2) NULL,
    [Dimensions] NVARCHAR(100) NULL,
    [Notes] NVARCHAR(MAX) NULL,
    [CreatedAt] DATETIME2 DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NULL,
    [IsDeleted] BIT DEFAULT 0,
    
    CONSTRAINT FK_Shipments_Order 
        FOREIGN KEY ([OrderId]) 
        REFERENCES [dbo].[Orders] ([Id])
);

CREATE NONCLUSTERED INDEX IX_Shipments_OrderId ON [dbo].[Shipments] ([OrderId]);
CREATE NONCLUSTERED INDEX IX_Shipments_TrackingNumber ON [dbo].[Shipments] ([TrackingNumber]);
CREATE NONCLUSTERED INDEX IX_Shipments_ShipmentStatus ON [dbo].[Shipments] ([ShipmentStatus]);
```

### 11. Reviews Table
```sql
CREATE TABLE [dbo].[Reviews]
(
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [ProductId] UNIQUEIDENTIFIER NOT NULL,
    [CustomerId] UNIQUEIDENTIFIER NOT NULL,
    [OrderId] UNIQUEIDENTIFIER NULL,
    [Rating] INT NOT NULL, -- 1-5
    [Title] NVARCHAR(255) NOT NULL,
    [Content] NVARCHAR(MAX) NOT NULL,
    [Helpful] INT DEFAULT 0,
    [Unhelpful] INT DEFAULT 0,
    [ImageUrl] NVARCHAR(MAX) NULL,
    [IsVerifiedPurchase] BIT DEFAULT 0,
    [Status] NVARCHAR(50) DEFAULT 'Pending', -- Pending, Approved, Rejected
    [CreatedAt] DATETIME2 DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NULL,
    [IsDeleted] BIT DEFAULT 0,
    
    CONSTRAINT FK_Reviews_Product 
        FOREIGN KEY ([ProductId]) 
        REFERENCES [dbo].[Products] ([Id]),
    CONSTRAINT FK_Reviews_Customer 
        FOREIGN KEY ([CustomerId]) 
        REFERENCES [dbo].[Customers] ([Id]),
    CONSTRAINT FK_Reviews_Order 
        FOREIGN KEY ([OrderId]) 
        REFERENCES [dbo].[Orders] ([Id]),
    CONSTRAINT CK_Reviews_Rating CHECK ([Rating] BETWEEN 1 AND 5)
);

CREATE NONCLUSTERED INDEX IX_Reviews_ProductId ON [dbo].[Reviews] ([ProductId]) 
    WHERE [Status] = 'Approved' AND [IsDeleted] = 0;
CREATE NONCLUSTERED INDEX IX_Reviews_CustomerId ON [dbo].[Reviews] ([CustomerId]);
```

### 12. Wishlist Table
```sql
CREATE TABLE [dbo].[Wishlist]
(
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [CustomerId] UNIQUEIDENTIFIER NOT NULL,
    [ProductId] UNIQUEIDENTIFIER NOT NULL,
    [AddedAt] DATETIME2 DEFAULT GETUTCDATE(),
    [IsDeleted] BIT DEFAULT 0,
    
    CONSTRAINT FK_Wishlist_Customer 
        FOREIGN KEY ([CustomerId]) 
        REFERENCES [dbo].[Customers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT FK_Wishlist_Product 
        FOREIGN KEY ([ProductId]) 
        REFERENCES [dbo].[Products] ([Id]),
    CONSTRAINT UQ_Wishlist_CustomerProduct UNIQUE ([CustomerId], [ProductId])
);

CREATE NONCLUSTERED INDEX IX_Wishlist_CustomerId ON [dbo].[Wishlist] ([CustomerId]) 
    WHERE [IsDeleted] = 0;
```

### 13. Cart Table
```sql
CREATE TABLE [dbo].[ShoppingCart]
(
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [CustomerId] UNIQUEIDENTIFIER NOT NULL,
    [ProductId] UNIQUEIDENTIFIER NOT NULL,
    [Quantity] INT NOT NULL,
    [AddedAt] DATETIME2 DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NULL,
    [IsDeleted] BIT DEFAULT 0,
    
    CONSTRAINT FK_ShoppingCart_Customer 
        FOREIGN KEY ([CustomerId]) 
        REFERENCES [dbo].[Customers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT FK_ShoppingCart_Product 
        FOREIGN KEY ([ProductId]) 
        REFERENCES [dbo].[Products] ([Id]),
    CONSTRAINT CK_ShoppingCart_Quantity CHECK ([Quantity] > 0)
);

CREATE NONCLUSTERED INDEX IX_ShoppingCart_CustomerId ON [dbo].[ShoppingCart] ([CustomerId]);
```

## RAG Pipeline Tables

### AI Knowledge Base Table
```sql
CREATE TABLE [dbo].[AIKnowledgeBase]
(
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [ProductId] UNIQUEIDENTIFIER NULL,
    [Title] NVARCHAR(255) NOT NULL,
    [Content] NVARCHAR(MAX) NOT NULL,
    [EmbeddingVector] VARBINARY(MAX) NULL, -- Store OpenAI embeddings
    [Category] NVARCHAR(100) NOT NULL,
    [SourceType] NVARCHAR(50) NOT NULL, -- Product, FAQ, Documentation
    [CreatedAt] DATETIME2 DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NULL,
    
    CONSTRAINT FK_AIKnowledgeBase_Product 
        FOREIGN KEY ([ProductId]) 
        REFERENCES [dbo].[Products] ([Id])
);

CREATE NONCLUSTERED INDEX IX_AIKnowledgeBase_Category ON [dbo].[AIKnowledgeBase] ([Category]);
```

### Chat History Table
```sql
CREATE TABLE [dbo].[ChatHistory]
(
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [CustomerId] UNIQUEIDENTIFIER NOT NULL,
    [MessageRole] NVARCHAR(50) NOT NULL, -- User, Assistant
    [MessageContent] NVARCHAR(MAX) NOT NULL,
    [ResponseTime] INT NULL,
    [TokensUsed] INT NULL,
    [SessionId] UNIQUEIDENTIFIER NOT NULL,
    [CreatedAt] DATETIME2 DEFAULT GETUTCDATE(),
    
    CONSTRAINT FK_ChatHistory_Customer 
        FOREIGN KEY ([CustomerId]) 
        REFERENCES [dbo].[Customers] ([Id])
);

CREATE NONCLUSTERED INDEX IX_ChatHistory_SessionId ON [dbo].[ChatHistory] ([SessionId]);
CREATE NONCLUSTERED INDEX IX_ChatHistory_CustomerId ON [dbo].[ChatHistory] ([CustomerId]);
```

## Migration Strategy

### Migration Numbering
```
001_InitialSchema.sql       - All base tables
002_AddProducts.sql         - Product-related tables
003_AddOrders.sql          - Order tables
004_AddPayments.sql        - Payment tables
005_AddShipments.sql       - Shipping tables
006_AddReviews.sql         - Review tables
007_AddAITables.sql        - AI/RAG tables
```

### Seed Data Strategy
```sql
-- AdminUser.sql
INSERT INTO [dbo].[Users] 
    (Email, PasswordHash, FirstName, LastName, Role, IsEmailVerified)
VALUES
    ('admin@ecommerce.com', 'hashed_password_here', 'Admin', 'User', 'Admin', 1);

-- Categories and Products seeding
-- Sample Orders with different statuses
-- Sample Users for testing
```

## Best Practices

1. **Indexes**: Create on Foreign Keys, commonly filtered columns, and join columns
2. **Partitioning**: Consider for Orders and Payments tables (by date)
3. **Archival**: Keep historical data in archive tables
4. **Backup**: Daily backups before 2 AM UTC
5. **Monitoring**: Track query performance, unused indexes
6. **Security**: Column-level encryption for sensitive data
