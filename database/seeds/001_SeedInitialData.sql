-- ECommerce Database Seed Data
-- Creates: Admin User, Sample Categories, Products, and Test Customers

USE ECommerceDB;
GO

-- ============================================
-- 1. CREATE ADMIN USER
-- ============================================
DECLARE @AdminUserId UNIQUEIDENTIFIER = NEWID();

INSERT INTO [dbo].[Users] 
(Id, Email, PasswordHash, FirstName, LastName, Role, IsEmailVerified, CreatedAt)
VALUES
(
    @AdminUserId,
    'admin@ecommerce.com',
    -- Password: Admin@123456 (bcrypt hash - replace with actual bcrypt hash in production)
    '$2a$11$YourBcryptHashHere',
    'Admin',
    'User',
    'Admin',
    1,
    GETUTCDATE()
);

-- Create Customer profile for admin (optional)
INSERT INTO [dbo].[Customers] (UserId, LoyaltyPoints)
VALUES (@AdminUserId, 0);

PRINT 'Admin user created successfully!';
GO

-- ============================================
-- 2. CREATE SAMPLE CATEGORIES
-- ============================================
DECLARE @ElectronicsId UNIQUEIDENTIFIER = NEWID();
DECLARE @ClothingId UNIQUEIDENTIFIER = NEWID();
DECLARE @BooksId UNIQUEIDENTIFIER = NEWID();
DECLARE @HomeId UNIQUEIDENTIFIER = NEWID();
DECLARE @SportsId UNIQUEIDENTIFIER = NEWID();

INSERT INTO [dbo].[Categories] (Id, Name, Slug, Description, DisplayOrder, IsActive)
VALUES
(
    @ElectronicsId,
    'Electronics',
    'electronics',
    'Electronic devices and gadgets',
    1,
    1
),
(
    @ClothingId,
    'Clothing',
    'clothing',
    'Apparel and fashion items',
    2,
    1
),
(
    @BooksId,
    'Books',
    'books',
    'Books and e-books',
    3,
    1
),
(
    @HomeId,
    'Home & Garden',
    'home-garden',
    'Home improvement and garden products',
    4,
    1
),
(
    @SportsId,
    'Sports & Outdoors',
    'sports-outdoors',
    'Sports equipment and outdoor gear',
    5,
    1
);

PRINT 'Sample categories created!';
GO

-- ============================================
-- 3. CREATE SAMPLE PRODUCTS
-- ============================================

-- Get category IDs
DECLARE @ElectronicsId UNIQUEIDENTIFIER = (SELECT Id FROM [dbo].[Categories] WHERE Slug = 'electronics');
DECLARE @ClothingId UNIQUEIDENTIFIER = (SELECT Id FROM [dbo].[Categories] WHERE Slug = 'clothing');

-- Electronics Products
INSERT INTO [dbo].[Products] 
(Name, Slug, Description, ShortDescription, CategoryId, Price, StockQuantity, Sku, IsActive, IsFeatured)
VALUES
(
    'Wireless Bluetooth Headphones',
    'wireless-bluetooth-headphones',
    'High-quality wireless headphones with active noise cancellation, 30-hour battery life, and premium sound quality. Perfect for music lovers and professionals.',
    'Premium wireless headphones with ANC',
    @ElectronicsId,
    129.99,
    50,
    'SKU-BT-001',
    1,
    1
),
(
    'USB-C Fast Charging Cable',
    'usb-c-fast-charging-cable',
    'Durable USB-C cable supporting fast charging and data transfer. Compatible with all USB-C devices.',
    'Fast charging USB-C cable',
    @ElectronicsId,
    12.99,
    200,
    'SKU-CB-001',
    1,
    0
),
(
    '4K Webcam',
    '4k-webcam',
    'Crystal clear 4K resolution webcam with auto-focus, built-in microphone, and wide-angle lens. Perfect for streaming and video conferencing.',
    '4K HD webcam for streaming',
    @ElectronicsId,
    89.99,
    75,
    'SKU-WC-001',
    1,
    1
),
(
    'Portable Power Bank 20000mAh',
    'portable-power-bank',
    'High capacity power bank with fast charging support, dual USB ports, and LED display. Charges multiple devices simultaneously.',
    '20000mAh power bank',
    @ElectronicsId,
    39.99,
    100,
    'SKU-PB-001',
    1,
    0
),
(
    'Wireless Mouse',
    'wireless-mouse',
    'Ergonomic wireless mouse with precision tracking, 18-month battery life, and reliable 2.4GHz connection.',
    'Ergonomic wireless mouse',
    @ElectronicsId,
    24.99,
    150,
    'SKU-MS-001',
    1,
    1
);

-- Clothing Products
INSERT INTO [dbo].[Products] 
(Name, Slug, Description, ShortDescription, CategoryId, Price, DiscountPrice, StockQuantity, Sku, IsActive, IsFeatured)
VALUES
(
    'Premium Cotton T-Shirt',
    'premium-cotton-tshirt',
    'Comfortable 100% organic cotton t-shirt with modern design. Available in multiple colors and sizes.',
    'Organic cotton t-shirt',
    @ClothingId,
    29.99,
    24.99,
    200,
    'SKU-TS-001',
    1,
    1
),
(
    'Casual Denim Jeans',
    'casual-denim-jeans',
    'Classic denim jeans with modern fit, durable fabric, and timeless style. Suitable for casual and semi-formal occasions.',
    'Classic blue denim jeans',
    @ClothingId,
    59.99,
    49.99,
    120,
    'SKU-DN-001',
    1,
    1
),
(
    'Winter Wool Sweater',
    'winter-wool-sweater',
    'Cozy wool sweater perfect for cold weather. Breathable, warm, and stylish.',
    'Warm wool winter sweater',
    @ClothingId,
    79.99,
    NULL,
    80,
    'SKU-SW-001',
    1,
    0
);

PRINT 'Sample products created!';
GO

-- ============================================
-- 4. CREATE SAMPLE CUSTOMERS
-- ============================================
DECLARE @Customer1UserId UNIQUEIDENTIFIER = NEWID();
DECLARE @Customer2UserId UNIQUEIDENTIFIER = NEWID();
DECLARE @Customer3UserId UNIQUEIDENTIFIER = NEWID();

-- Customer 1
INSERT INTO [dbo].[Users] (Id, Email, PasswordHash, FirstName, LastName, Role, IsEmailVerified)
VALUES
(
    @Customer1UserId,
    'john.doe@example.com',
    '$2a$11$CustomerHashHere1',
    'John',
    'Doe',
    'Customer',
    1
);

INSERT INTO [dbo].[Customers] (UserId, PhoneNumber, TotalSpending, LoyaltyPoints)
VALUES
(
    @Customer1UserId,
    '+1-234-567-8900',
    299.99,
    500
);

-- Customer 2
INSERT INTO [dbo].[Users] (Id, Email, PasswordHash, FirstName, LastName, Role, IsEmailVerified)
VALUES
(
    @Customer2UserId,
    'jane.smith@example.com',
    '$2a$11$CustomerHashHere2',
    'Jane',
    'Smith',
    'Customer',
    1
);

INSERT INTO [dbo].[Customers] (UserId, PhoneNumber, TotalSpending, LoyaltyPoints)
VALUES
(
    @Customer2UserId,
    '+1-234-567-8901',
    599.99,
    1000
);

-- Customer 3
INSERT INTO [dbo].[Users] (Id, Email, PasswordHash, FirstName, LastName, Role, IsEmailVerified)
VALUES
(
    @Customer3UserId,
    'michael.johnson@example.com',
    '$2a$11$CustomerHashHere3',
    'Michael',
    'Johnson',
    'Customer',
    1
);

INSERT INTO [dbo].[Customers] (UserId, PhoneNumber, TotalSpending, LoyaltyPoints)
VALUES
(
    @Customer3UserId,
    '+1-234-567-8902',
    1200.00,
    2000
);

PRINT 'Sample customers created!';
GO

-- ============================================
-- 5. CREATE SAMPLE ADDRESSES
-- ============================================
DECLARE @Customer1Id UNIQUEIDENTIFIER = (SELECT Id FROM [dbo].[Customers] WHERE UserId = (SELECT Id FROM [dbo].[Users] WHERE Email = 'john.doe@example.com'));
DECLARE @Customer1UserId UNIQUEIDENTIFIER = (SELECT UserId FROM [dbo].[Customers] WHERE Id = @Customer1Id);

INSERT INTO [dbo].[Addresses] 
(UserId, AddressType, FullName, PhoneNumber, StreetAddress, City, StateProvince, PostalCode, Country, IsDefaultAddress)
VALUES
(
    @Customer1UserId,
    'Home',
    'John Doe',
    '+1-234-567-8900',
    '123 Main Street',
    'Springfield',
    'IL',
    '62701',
    'United States',
    1
),
(
    @Customer1UserId,
    'Office',
    'John Doe',
    '+1-234-567-8900',
    '456 Business Ave',
    'Springfield',
    'IL',
    '62702',
    'United States',
    0
);

PRINT 'Sample addresses created!';
GO

-- ============================================
-- 6. CREATE SAMPLE ORDERS
-- ============================================
DECLARE @Customer1Id UNIQUEIDENTIFIER = (SELECT Id FROM [dbo].[Customers] WHERE UserId = (SELECT Id FROM [dbo].[Users] WHERE Email = 'john.doe@example.com'));
DECLARE @Product1Id UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM [dbo].[Products] WHERE Slug = 'wireless-bluetooth-headphones');
DECLARE @AddressId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM [dbo].[Addresses] WHERE UserId = (SELECT UserId FROM [dbo].[Customers] WHERE Id = @Customer1Id));
DECLARE @OrderId UNIQUEIDENTIFIER = NEWID();

INSERT INTO [dbo].[Orders] 
(Id, OrderNumber, CustomerId, ShippingAddressId, BillingAddressId, OrderStatus, TotalAmount, ShippingCost, TaxAmount, CurrencyCode)
VALUES
(
    @OrderId,
    'ORD-' + FORMAT(GETUTCDATE(), 'yyyyMMdd') + '-001',
    @Customer1Id,
    @AddressId,
    @AddressId,
    'Delivered',
    139.99,
    10.00,
    9.99,
    'USD'
);

-- Add order items
INSERT INTO [dbo].[OrderItems] (OrderId, ProductId, Quantity, UnitPrice, TotalPrice)
VALUES
(
    @OrderId,
    @Product1Id,
    1,
    129.99,
    129.99
);

-- Add payment
INSERT INTO [dbo].[Payments] 
(OrderId, TransactionId, Amount, PaymentMethod, PaymentGateway, PaymentStatus, ProcessedAt)
VALUES
(
    @OrderId,
    'TXN-' + CONVERT(VARCHAR(36), NEWID()),
    139.99,
    'CreditCard',
    'Stripe',
    'Completed',
    GETUTCDATE()
);

-- Add shipment
INSERT INTO [dbo].[Shipments] 
(OrderId, TrackingNumber, CarrierName, ShipmentStatus, ShippedAt, DeliveredAt)
VALUES
(
    @OrderId,
    'TRACK-' + CONVERT(VARCHAR(36), NEWID()),
    'FedEx',
    'Delivered',
    DATEADD(DAY, -5, GETUTCDATE()),
    DATEADD(DAY, -2, GETUTCDATE())
);

PRINT 'Sample orders created!';
GO

-- ============================================
-- 7. CREATE SAMPLE REVIEWS
-- ============================================
DECLARE @Product1Id UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM [dbo].[Products] WHERE Slug = 'wireless-bluetooth-headphones');
DECLARE @Customer1Id UNIQUEIDENTIFIER = (SELECT Id FROM [dbo].[Customers] WHERE UserId = (SELECT Id FROM [dbo].[Users] WHERE Email = 'john.doe@example.com'));

INSERT INTO [dbo].[Reviews] 
(ProductId, CustomerId, Rating, Title, Content, IsVerifiedPurchase, Status)
VALUES
(
    @Product1Id,
    @Customer1Id,
    5,
    'Excellent Product!',
    'These headphones are amazing! Great sound quality, comfortable to wear, and the battery lasts forever. Highly recommended!',
    1,
    'Approved'
),
(
    @Product1Id,
    @Customer1Id,
    4,
    'Very Good',
    'Good sound quality and comfortable. Battery life is as advertised. Minor issue with the noise cancellation toggle.',
    1,
    'Approved'
);

-- Update product rating
UPDATE [dbo].[Products]
SET Rating = 4.5, ReviewCount = 2
WHERE Id = @Product1Id;

PRINT 'Sample reviews created!';
GO

-- ============================================
-- 8. ADD INITIAL AI KNOWLEDGE BASE
-- ============================================
DECLARE @Product1Id UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM [dbo].[Products] WHERE Slug = 'wireless-bluetooth-headphones');

INSERT INTO [dbo].[AIKnowledgeBase] 
(ProductId, Title, Content, Category, SourceType)
VALUES
(
    @Product1Id,
    'Wireless Headphones Setup Guide',
    'To pair your wireless headphones: 1. Turn on Bluetooth mode on your device. 2. Press and hold the power button on headphones for 3 seconds. 3. Select device from Bluetooth menu. 4. Connection will be established within 10 seconds.',
    'Product Setup',
    'Product'
),
(
    @Product1Id,
    'Battery Optimization Tips',
    'To maximize battery life: 1. Turn off noise cancellation when not needed. 2. Lower volume to moderate levels. 3. Charge fully before first use. 4. Use power saving mode on devices.',
    'Product Care',
    'Documentation'
),
(
    NULL,
    'General Return Policy',
    'We offer 30-day returns on all products. Items must be unused and in original packaging. Refunds are processed within 5-7 business days.',
    'Policies',
    'FAQ'
),
(
    NULL,
    'Shipping Information',
    'We offer free shipping on orders over $50. Standard shipping takes 5-7 business days. Express shipping available for 2-3 day delivery.',
    'Shipping',
    'FAQ'
);

PRINT 'AI knowledge base populated!';
GO

-- ============================================
-- Verify Seed Data
-- ============================================
PRINT '===== SEED DATA SUMMARY =====';
PRINT 'Total Users: ' + CAST((SELECT COUNT(*) FROM [dbo].[Users]) AS NVARCHAR(10));
PRINT 'Total Categories: ' + CAST((SELECT COUNT(*) FROM [dbo].[Categories]) AS NVARCHAR(10));
PRINT 'Total Products: ' + CAST((SELECT COUNT(*) FROM [dbo].[Products]) AS NVARCHAR(10));
PRINT 'Total Customers: ' + CAST((SELECT COUNT(*) FROM [dbo].[Customers]) AS NVARCHAR(10));
PRINT 'Total Orders: ' + CAST((SELECT COUNT(*) FROM [dbo].[Orders]) AS NVARCHAR(10));
PRINT 'Total Reviews: ' + CAST((SELECT COUNT(*) FROM [dbo].[Reviews]) AS NVARCHAR(10));
PRINT 'Knowledge Base Items: ' + CAST((SELECT COUNT(*) FROM [dbo].[AIKnowledgeBase]) AS NVARCHAR(10));
PRINT '=============================';
PRINT 'Seed data successfully created!';
GO
