IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [Banners] (
    [Id] bigint NOT NULL IDENTITY,
    [Title] nvarchar(150) NOT NULL,
    [Subtitle] nvarchar(255) NOT NULL,
    [ImageUrl] nvarchar(500) NOT NULL,
    [ImagePublicId] nvarchar(255) NOT NULL,
    [LinkUrl] nvarchar(500) NULL,
    [Position] nvarchar(20) NOT NULL,
    [DisplayOrder] int NOT NULL,
    [IsActive] bit NOT NULL,
    [StartDate] datetime2 NULL,
    [EndDate] datetime2 NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Banners] PRIMARY KEY ([Id])
);

CREATE TABLE [Categories] (
    [Id] bigint NOT NULL IDENTITY,
    [Name] nvarchar(150) NOT NULL,
    [ParentCategoryId] bigint NULL,
    [Description] nvarchar(1000) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    [IsDeleted] bit NOT NULL,
    [DeletedAt] datetime2 NULL,
    CONSTRAINT [PK_Categories] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Categories_Categories_ParentCategoryId] FOREIGN KEY ([ParentCategoryId]) REFERENCES [Categories] ([Id])
);

CREATE TABLE [Languages] (
    [Id] int NOT NULL IDENTITY,
    [Code] nvarchar(max) NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [IsActive] bit NOT NULL,
    [IsDefault] bit NOT NULL,
    CONSTRAINT [PK_Languages] PRIMARY KEY ([Id])
);

CREATE TABLE [PromoCodes] (
    [Id] bigint NOT NULL IDENTITY,
    [Code] nvarchar(50) NOT NULL,
    [DiscountType] int NOT NULL,
    [DiscountValue] decimal(18,2) NOT NULL,
    [MinOrderAmount] decimal(18,2) NOT NULL,
    [ExpiryDate] datetime2 NOT NULL,
    [UsageLimit] int NOT NULL,
    [TimesUsed] int NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    [IsDeleted] bit NOT NULL,
    [DeletedAt] datetime2 NULL,
    CONSTRAINT [PK_PromoCodes] PRIMARY KEY ([Id])
);

CREATE TABLE [RevokedTokens] (
    [Id] bigint NOT NULL IDENTITY,
    [Jti] nvarchar(128) NOT NULL,
    [RevokedAt] datetime2 NOT NULL,
    [Reason] nvarchar(500) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_RevokedTokens] PRIMARY KEY ([Id])
);

CREATE TABLE [Roles] (
    [Id] bigint NOT NULL IDENTITY,
    [Name] nvarchar(50) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY ([Id])
);

CREATE TABLE [ShippingMethods] (
    [Id] bigint NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [Description] nvarchar(500) NOT NULL,
    [BaseCost] decimal(18,2) NOT NULL,
    [EstimatedDays] int NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    [IsDeleted] bit NOT NULL,
    [DeletedAt] datetime2 NULL,
    CONSTRAINT [PK_ShippingMethods] PRIMARY KEY ([Id])
);

CREATE TABLE [Users] (
    [Id] bigint NOT NULL IDENTITY,
    [Username] nvarchar(50) NOT NULL,
    [Email] nvarchar(256) NOT NULL,
    [PhoneNumber] nvarchar(20) NOT NULL,
    [PasswordHash] nvarchar(max) NOT NULL,
    [FullName] nvarchar(150) NOT NULL,
    [EmailConfirmed] bit NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    [IsDeleted] bit NOT NULL,
    [DeletedAt] datetime2 NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);

CREATE TABLE [HomepageSections] (
    [Id] bigint NOT NULL IDENTITY,
    [BannerId] bigint NULL,
    [Title] nvarchar(150) NOT NULL,
    [SubTitle] nvarchar(255) NULL,
    [Type] nvarchar(30) NOT NULL,
    [DisplayOrder] int NOT NULL,
    [IsActive] bit NOT NULL,
    [CategoryId] bigint NULL,
    [MaxItems] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_HomepageSections] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_HomepageSections_Banners_BannerId] FOREIGN KEY ([BannerId]) REFERENCES [Banners] ([Id]),
    CONSTRAINT [FK_HomepageSections_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([Id])
);

CREATE TABLE [ResourceStrings] (
    [Id] int NOT NULL IDENTITY,
    [Key] nvarchar(150) NOT NULL,
    [LanguageId] int NOT NULL,
    [Value] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_ResourceStrings] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ResourceStrings_Languages_LanguageId] FOREIGN KEY ([LanguageId]) REFERENCES [Languages] ([Id])
);

CREATE TABLE [Addresses] (
    [Id] bigint NOT NULL IDENTITY,
    [UserId] bigint NOT NULL,
    [Label] nvarchar(50) NOT NULL,
    [Street] nvarchar(256) NOT NULL,
    [City] nvarchar(100) NOT NULL,
    [State] nvarchar(100) NOT NULL,
    [PostalCode] nvarchar(20) NOT NULL,
    [Country] nvarchar(100) NOT NULL,
    [IsDefault] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Addresses] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Addresses_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id])
);

CREATE TABLE [Carts] (
    [Id] bigint NOT NULL IDENTITY,
    [UserId] bigint NULL,
    [GuestSessionId] nvarchar(512) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Carts] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Carts_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id])
);

CREATE TABLE [DeviceTokens] (
    [Id] int NOT NULL IDENTITY,
    [UserId] bigint NOT NULL,
    [Token] nvarchar(255) NOT NULL,
    [Platform] nvarchar(20) NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_DeviceTokens] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_DeviceTokens_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id])
);

CREATE TABLE [EmailConfirmations] (
    [Id] bigint NOT NULL IDENTITY,
    [UserId] bigint NOT NULL,
    [Token] nvarchar(256) NOT NULL,
    [ExpiresAt] datetime2 NOT NULL,
    [IsUsed] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_EmailConfirmations] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_EmailConfirmations_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id])
);

CREATE TABLE [NewsletterSubscriptions] (
    [Id] int NOT NULL IDENTITY,
    [UserId] bigint NULL,
    [Email] nvarchar(255) NOT NULL,
    [IsSubscribed] bit NOT NULL,
    [SubscribedAt] datetime2 NOT NULL,
    [UnsubscribedAt] datetime2 NULL,
    CONSTRAINT [PK_NewsletterSubscriptions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_NewsletterSubscriptions_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id])
);

CREATE TABLE [Notifications] (
    [Id] bigint NOT NULL IDENTITY,
    [UserId] bigint NOT NULL,
    [Type] int NOT NULL,
    [Message] nvarchar(2000) NOT NULL,
    [IsRead] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Notifications] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Notifications_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id])
);

CREATE TABLE [Orders] (
    [Id] bigint NOT NULL IDENTITY,
    [UserId] bigint NULL,
    [GuestFullName] nvarchar(150) NULL,
    [GuestEmail] nvarchar(256) NULL,
    [GuestPhoneNumber] nvarchar(20) NULL,
    [ShippingFullName] nvarchar(150) NOT NULL,
    [ShippingPhone] nvarchar(20) NOT NULL,
    [ShippingStreet] nvarchar(256) NOT NULL,
    [ShippingCity] nvarchar(100) NOT NULL,
    [ShippingState] nvarchar(100) NOT NULL,
    [ShippingPostalCode] nvarchar(20) NOT NULL,
    [ShippingCountry] nvarchar(100) NOT NULL,
    [ShippingMethodId] bigint NOT NULL,
    [PromoCodeId] bigint NULL,
    [OrderDate] datetime2 NOT NULL,
    [Status] int NOT NULL,
    [SubTotal] decimal(18,2) NOT NULL,
    [DiscountAmount] decimal(18,2) NOT NULL,
    [ShippingCost] decimal(18,2) NOT NULL,
    [TotalAmount] decimal(18,2) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    [IsDeleted] bit NOT NULL,
    [DeletedAt] datetime2 NULL,
    CONSTRAINT [PK_Orders] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Orders_PromoCodes_PromoCodeId] FOREIGN KEY ([PromoCodeId]) REFERENCES [PromoCodes] ([Id]),
    CONSTRAINT [FK_Orders_ShippingMethods_ShippingMethodId] FOREIGN KEY ([ShippingMethodId]) REFERENCES [ShippingMethods] ([Id]),
    CONSTRAINT [FK_Orders_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id])
);

CREATE TABLE [PasswordResetTokens] (
    [Id] bigint NOT NULL IDENTITY,
    [UserId] bigint NOT NULL,
    [Token] nvarchar(256) NOT NULL,
    [ExpiresAt] datetime2 NOT NULL,
    [IsUsed] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_PasswordResetTokens] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PasswordResetTokens_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id])
);

CREATE TABLE [PaymentMethods] (
    [Id] bigint NOT NULL IDENTITY,
    [UserId] bigint NOT NULL,
    [Provider] nvarchar(50) NOT NULL,
    [Token] nvarchar(512) NOT NULL,
    [Last4Digits] nvarchar(4) NOT NULL,
    [ExpiryDate] datetime2 NOT NULL,
    [IsDefault] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_PaymentMethods] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PaymentMethods_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id])
);

CREATE TABLE [Referrals] (
    [Id] int NOT NULL IDENTITY,
    [ReferrerUserId] bigint NOT NULL,
    [ReferredUserId] bigint NOT NULL,
    [ReferralCode] nvarchar(50) NOT NULL,
    [Status] nvarchar(20) NOT NULL,
    [RewardPoints] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Referrals] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Referrals_Users_ReferredUserId] FOREIGN KEY ([ReferredUserId]) REFERENCES [Users] ([Id]),
    CONSTRAINT [FK_Referrals_Users_ReferrerUserId] FOREIGN KEY ([ReferrerUserId]) REFERENCES [Users] ([Id])
);

CREATE TABLE [RefreshTokens] (
    [Id] bigint NOT NULL IDENTITY,
    [UserId] bigint NOT NULL,
    [Token] nvarchar(256) NOT NULL,
    [Expires] datetime2 NOT NULL,
    [IsRevoked] bit NOT NULL,
    [RevokedAt] datetime2 NULL,
    [CreatedByIp] nvarchar(45) NOT NULL,
    [ReplacedByToken] nvarchar(256) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_RefreshTokens] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_RefreshTokens_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id])
);

CREATE TABLE [SellerApplications] (
    [Id] bigint NOT NULL IDENTITY,
    [UserId] bigint NOT NULL,
    [StoreName] nvarchar(150) NOT NULL,
    [StoreDescription] nvarchar(2000) NOT NULL,
    [Status] int NOT NULL,
    [RejectionReason] nvarchar(max) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_SellerApplications] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_SellerApplications_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id])
);

CREATE TABLE [SellerProfiles] (
    [Id] bigint NOT NULL IDENTITY,
    [UserId] bigint NOT NULL,
    [StoreName] nvarchar(150) NOT NULL,
    [StoreDescription] nvarchar(2000) NOT NULL,
    [IsApproved] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    [IsDeleted] bit NOT NULL,
    [DeletedAt] datetime2 NULL,
    CONSTRAINT [PK_SellerProfiles] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_SellerProfiles_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id])
);

CREATE TABLE [UserRoles] (
    [UserId] bigint NOT NULL,
    [RoleId] bigint NOT NULL,
    CONSTRAINT [PK_UserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_UserRoles_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]),
    CONSTRAINT [FK_UserRoles_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id])
);

CREATE TABLE [Wallets] (
    [Id] bigint NOT NULL IDENTITY,
    [UserId] bigint NOT NULL,
    [Balance] decimal(18,2) NOT NULL,
    [Currency] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    [IsDeleted] bit NOT NULL,
    [DeletedAt] datetime2 NULL,
    CONSTRAINT [PK_Wallets] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Wallets_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id])
);

CREATE TABLE [LoyaltyTransactions] (
    [Id] bigint NOT NULL IDENTITY,
    [UserId] bigint NOT NULL,
    [OrderId] bigint NULL,
    [Points] int NOT NULL,
    [Type] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_LoyaltyTransactions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_LoyaltyTransactions_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([Id]),
    CONSTRAINT [FK_LoyaltyTransactions_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id])
);

CREATE TABLE [OrderStatusHistories] (
    [Id] bigint NOT NULL IDENTITY,
    [OrderId] bigint NOT NULL,
    [Status] nvarchar(50) NOT NULL,
    [Notes] nvarchar(1000) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_OrderStatusHistories] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_OrderStatusHistories_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([Id])
);

CREATE TABLE [Payments] (
    [Id] bigint NOT NULL IDENTITY,
    [OrderId] bigint NOT NULL,
    [Method] int NOT NULL,
    [Provider] nvarchar(50) NOT NULL,
    [TransactionId] nvarchar(512) NOT NULL,
    [Currency] nvarchar(max) NOT NULL,
    [Amount] decimal(18,2) NOT NULL,
    [Status] int NOT NULL,
    [PaidAt] datetime2 NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    [IsDeleted] bit NOT NULL,
    [DeletedAt] datetime2 NULL,
    CONSTRAINT [PK_Payments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Payments_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([Id])
);

CREATE TABLE [Payouts] (
    [Id] bigint NOT NULL IDENTITY,
    [SellerProfileId] bigint NOT NULL,
    [Amount] decimal(18,2) NOT NULL,
    [Status] int NOT NULL,
    [PayoutDate] datetime2 NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Payouts] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Payouts_SellerProfiles_SellerProfileId] FOREIGN KEY ([SellerProfileId]) REFERENCES [SellerProfiles] ([Id])
);

CREATE TABLE [Products] (
    [Id] bigint NOT NULL IDENTITY,
    [SellerProfileId] bigint NOT NULL,
    [CategoryId] bigint NOT NULL,
    [Name] nvarchar(256) NOT NULL,
    [Description] nvarchar(4000) NOT NULL,
    [Price] decimal(18,2) NOT NULL,
    [StockQuantity] int NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    [IsDeleted] bit NOT NULL,
    [DeletedAt] datetime2 NULL,
    CONSTRAINT [PK_Products] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Products_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([Id]),
    CONSTRAINT [FK_Products_SellerProfiles_SellerProfileId] FOREIGN KEY ([SellerProfileId]) REFERENCES [SellerProfiles] ([Id])
);

CREATE TABLE [Shipments] (
    [Id] bigint NOT NULL IDENTITY,
    [OrderId] bigint NOT NULL,
    [SellerProfileId] bigint NOT NULL,
    [Carrier] nvarchar(100) NOT NULL,
    [TrackingNumber] nvarchar(512) NOT NULL,
    [Status] int NOT NULL,
    [ShippedDate] datetime2 NULL,
    [EstimatedDeliveryDate] datetime2 NULL,
    [DeliveredDate] datetime2 NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    [IsDeleted] bit NOT NULL,
    [DeletedAt] datetime2 NULL,
    CONSTRAINT [PK_Shipments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Shipments_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([Id]),
    CONSTRAINT [FK_Shipments_SellerProfiles_SellerProfileId] FOREIGN KEY ([SellerProfileId]) REFERENCES [SellerProfiles] ([Id])
);

CREATE TABLE [WalletTransactions] (
    [Id] bigint NOT NULL IDENTITY,
    [WalletId] bigint NOT NULL,
    [Type] int NOT NULL,
    [Amount] decimal(18,2) NOT NULL,
    [BalanceAfter] decimal(18,2) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [OrderId] bigint NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    [IsDeleted] bit NOT NULL,
    [DeletedAt] datetime2 NULL,
    CONSTRAINT [PK_WalletTransactions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_WalletTransactions_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([Id]),
    CONSTRAINT [FK_WalletTransactions_Wallets_WalletId] FOREIGN KEY ([WalletId]) REFERENCES [Wallets] ([Id])
);

CREATE TABLE [CartItems] (
    [Id] bigint NOT NULL IDENTITY,
    [CartId] bigint NOT NULL,
    [ProductId] bigint NOT NULL,
    [Quantity] int NOT NULL,
    [UnitPriceSnapshot] decimal(18,2) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_CartItems] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_CartItems_Carts_CartId] FOREIGN KEY ([CartId]) REFERENCES [Carts] ([Id]),
    CONSTRAINT [FK_CartItems_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id])
);

CREATE TABLE [OrderItems] (
    [Id] bigint NOT NULL IDENTITY,
    [OrderId] bigint NOT NULL,
    [ProductId] bigint NOT NULL,
    [Quantity] int NOT NULL,
    [UnitPrice] decimal(18,2) NOT NULL,
    [Discount] decimal(18,2) NOT NULL,
    [Subtotal] decimal(18,2) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_OrderItems] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_OrderItems_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([Id]),
    CONSTRAINT [FK_OrderItems_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id])
);

CREATE TABLE [ProductImages] (
    [Id] bigint NOT NULL IDENTITY,
    [ProductId] bigint NOT NULL,
    [ImageUrl] nvarchar(2048) NOT NULL,
    [ImagePublicId] nvarchar(512) NOT NULL,
    [DisplayOrder] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_ProductImages] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ProductImages_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id])
);

CREATE TABLE [Reviews] (
    [Id] bigint NOT NULL IDENTITY,
    [UserId] bigint NOT NULL,
    [ProductId] bigint NOT NULL,
    [Rating] int NOT NULL,
    [Comment] nvarchar(2000) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    [IsDeleted] bit NOT NULL,
    [DeletedAt] datetime2 NULL,
    CONSTRAINT [PK_Reviews] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Reviews_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]),
    CONSTRAINT [FK_Reviews_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id])
);

CREATE TABLE [Wishlists] (
    [Id] bigint NOT NULL IDENTITY,
    [UserId] bigint NOT NULL,
    [ProductId] bigint NOT NULL,
    [AddedAt] datetime2 NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Wishlists] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Wishlists_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]),
    CONSTRAINT [FK_Wishlists_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id])
);

CREATE TABLE [ShipmentItems] (
    [Id] bigint NOT NULL IDENTITY,
    [ShipmentId] bigint NOT NULL,
    [OrderItemId] bigint NOT NULL,
    [Quantity] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_ShipmentItems] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ShipmentItems_OrderItems_OrderItemId] FOREIGN KEY ([OrderItemId]) REFERENCES [OrderItems] ([Id]),
    CONSTRAINT [FK_ShipmentItems_Shipments_ShipmentId] FOREIGN KEY ([ShipmentId]) REFERENCES [Shipments] ([Id])
);

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'Name', N'UpdatedAt') AND [object_id] = OBJECT_ID(N'[Roles]'))
    SET IDENTITY_INSERT [Roles] ON;
INSERT INTO [Roles] ([Id], [CreatedAt], [Name], [UpdatedAt])
VALUES (CAST(1 AS bigint), '0001-01-01T00:00:00.0000000', N'Admin', '0001-01-01T00:00:00.0000000'),
(CAST(2 AS bigint), '0001-01-01T00:00:00.0000000', N'Seller', '0001-01-01T00:00:00.0000000'),
(CAST(3 AS bigint), '0001-01-01T00:00:00.0000000', N'Customer', '0001-01-01T00:00:00.0000000');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'Name', N'UpdatedAt') AND [object_id] = OBJECT_ID(N'[Roles]'))
    SET IDENTITY_INSERT [Roles] OFF;

CREATE INDEX [IX_Addresses_UserId] ON [Addresses] ([UserId]);

CREATE INDEX [IX_Banners_IsActive_Position_DisplayOrder] ON [Banners] ([IsActive], [Position], [DisplayOrder]);

CREATE INDEX [IX_CartItems_CartId] ON [CartItems] ([CartId]);

CREATE INDEX [IX_CartItems_ProductId] ON [CartItems] ([ProductId]);

CREATE UNIQUE INDEX [IX_Carts_UserId] ON [Carts] ([UserId]) WHERE [UserId] IS NOT NULL;

CREATE INDEX [IX_Categories_ParentCategoryId] ON [Categories] ([ParentCategoryId]);

CREATE UNIQUE INDEX [IX_DeviceTokens_Token] ON [DeviceTokens] ([Token]);

CREATE INDEX [IX_DeviceTokens_UserId] ON [DeviceTokens] ([UserId]);

CREATE UNIQUE INDEX [IX_EmailConfirmations_Token] ON [EmailConfirmations] ([Token]);

CREATE INDEX [IX_EmailConfirmations_UserId] ON [EmailConfirmations] ([UserId]);

CREATE INDEX [IX_HomepageSections_BannerId] ON [HomepageSections] ([BannerId]);

CREATE INDEX [IX_HomepageSections_CategoryId] ON [HomepageSections] ([CategoryId]);

CREATE INDEX [IX_HomepageSections_IsActive_DisplayOrder] ON [HomepageSections] ([IsActive], [DisplayOrder]);

CREATE INDEX [IX_LoyaltyTransactions_OrderId] ON [LoyaltyTransactions] ([OrderId]);

CREATE INDEX [IX_LoyaltyTransactions_UserId] ON [LoyaltyTransactions] ([UserId]);

CREATE UNIQUE INDEX [IX_NewsletterSubscriptions_Email] ON [NewsletterSubscriptions] ([Email]);

CREATE INDEX [IX_NewsletterSubscriptions_UserId] ON [NewsletterSubscriptions] ([UserId]);

CREATE INDEX [IX_Notifications_UserId] ON [Notifications] ([UserId]);

CREATE INDEX [IX_OrderItems_OrderId] ON [OrderItems] ([OrderId]);

CREATE INDEX [IX_OrderItems_ProductId] ON [OrderItems] ([ProductId]);

CREATE INDEX [IX_Orders_PromoCodeId] ON [Orders] ([PromoCodeId]);

CREATE INDEX [IX_Orders_ShippingMethodId] ON [Orders] ([ShippingMethodId]);

CREATE INDEX [IX_Orders_UserId] ON [Orders] ([UserId]);

CREATE INDEX [IX_OrderStatusHistories_OrderId] ON [OrderStatusHistories] ([OrderId]);

CREATE UNIQUE INDEX [IX_PasswordResetTokens_Token] ON [PasswordResetTokens] ([Token]);

CREATE INDEX [IX_PasswordResetTokens_UserId] ON [PasswordResetTokens] ([UserId]);

CREATE INDEX [IX_PaymentMethods_UserId] ON [PaymentMethods] ([UserId]);

CREATE UNIQUE INDEX [IX_Payments_OrderId] ON [Payments] ([OrderId]);

CREATE INDEX [IX_Payouts_SellerProfileId] ON [Payouts] ([SellerProfileId]);

CREATE INDEX [IX_ProductImages_ProductId] ON [ProductImages] ([ProductId]);

CREATE INDEX [IX_Products_CategoryId] ON [Products] ([CategoryId]);

CREATE INDEX [IX_Products_SellerProfileId] ON [Products] ([SellerProfileId]);

CREATE UNIQUE INDEX [IX_PromoCodes_Code] ON [PromoCodes] ([Code]);

CREATE UNIQUE INDEX [IX_Referrals_ReferralCode] ON [Referrals] ([ReferralCode]);

CREATE INDEX [IX_Referrals_ReferredUserId] ON [Referrals] ([ReferredUserId]);

CREATE INDEX [IX_Referrals_ReferrerUserId] ON [Referrals] ([ReferrerUserId]);

CREATE UNIQUE INDEX [IX_RefreshTokens_Token] ON [RefreshTokens] ([Token]);

CREATE INDEX [IX_RefreshTokens_UserId] ON [RefreshTokens] ([UserId]);

CREATE UNIQUE INDEX [IX_ResourceStrings_Key_LanguageId] ON [ResourceStrings] ([Key], [LanguageId]);

CREATE INDEX [IX_ResourceStrings_LanguageId] ON [ResourceStrings] ([LanguageId]);

CREATE INDEX [IX_Reviews_ProductId] ON [Reviews] ([ProductId]);

CREATE UNIQUE INDEX [IX_Reviews_UserId_ProductId] ON [Reviews] ([UserId], [ProductId]);

CREATE UNIQUE INDEX [IX_RevokedTokens_Jti] ON [RevokedTokens] ([Jti]);

CREATE UNIQUE INDEX [IX_Roles_Name] ON [Roles] ([Name]);

CREATE UNIQUE INDEX [IX_SellerApplications_UserId] ON [SellerApplications] ([UserId]) WHERE [Status] = 1;

CREATE UNIQUE INDEX [IX_SellerProfiles_UserId] ON [SellerProfiles] ([UserId]);

CREATE INDEX [IX_ShipmentItems_OrderItemId] ON [ShipmentItems] ([OrderItemId]);

CREATE INDEX [IX_ShipmentItems_ShipmentId] ON [ShipmentItems] ([ShipmentId]);

CREATE INDEX [IX_Shipments_OrderId] ON [Shipments] ([OrderId]);

CREATE INDEX [IX_Shipments_SellerProfileId] ON [Shipments] ([SellerProfileId]);

CREATE INDEX [IX_UserRoles_RoleId] ON [UserRoles] ([RoleId]);

CREATE UNIQUE INDEX [IX_Users_Email] ON [Users] ([Email]);

CREATE UNIQUE INDEX [IX_Users_PhoneNumber] ON [Users] ([PhoneNumber]) WHERE [PhoneNumber] != '';

CREATE UNIQUE INDEX [IX_Users_Username] ON [Users] ([Username]);

CREATE UNIQUE INDEX [IX_Wallets_UserId] ON [Wallets] ([UserId]);

CREATE INDEX [IX_WalletTransactions_OrderId] ON [WalletTransactions] ([OrderId]);

CREATE INDEX [IX_WalletTransactions_WalletId] ON [WalletTransactions] ([WalletId]);

CREATE INDEX [IX_Wishlists_ProductId] ON [Wishlists] ([ProductId]);

CREATE UNIQUE INDEX [IX_Wishlists_UserId_ProductId] ON [Wishlists] ([UserId], [ProductId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260922085543_InitialSchema', N'9.0.0');

COMMIT;
GO

