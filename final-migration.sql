BEGIN TRANSACTION;
ALTER TABLE [Transactions] DROP CONSTRAINT [FK_Transactions_Orders_OrderId];

ALTER TABLE [Transactions] DROP CONSTRAINT [FK_Transactions_Users_UserId];

ALTER TABLE [Transactions] DROP CONSTRAINT [PK_Transactions];

EXEC sp_rename N'[Transactions]', N'LoyaltyTransactions', 'OBJECT';

EXEC sp_rename N'[LoyaltyTransactions].[IX_Transactions_UserId]', N'IX_LoyaltyTransactions_UserId', 'INDEX';

EXEC sp_rename N'[LoyaltyTransactions].[IX_Transactions_OrderId]', N'IX_LoyaltyTransactions_OrderId', 'INDEX';

ALTER TABLE [LoyaltyTransactions] ADD CONSTRAINT [PK_LoyaltyTransactions] PRIMARY KEY ([Id]);

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

CREATE TABLE [Languages] (
    [Id] int NOT NULL IDENTITY,
    [Code] nvarchar(max) NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [IsActive] bit NOT NULL,
    [IsDefault] bit NOT NULL,
    CONSTRAINT [PK_Languages] PRIMARY KEY ([Id])
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

CREATE TABLE [ResourceStrings] (
    [Id] int NOT NULL IDENTITY,
    [Key] nvarchar(150) NOT NULL,
    [LanguageId] int NOT NULL,
    [Value] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_ResourceStrings] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ResourceStrings_Languages_LanguageId] FOREIGN KEY ([LanguageId]) REFERENCES [Languages] ([Id])
);

CREATE UNIQUE INDEX [IX_DeviceTokens_Token] ON [DeviceTokens] ([Token]);

CREATE INDEX [IX_DeviceTokens_UserId] ON [DeviceTokens] ([UserId]);

CREATE UNIQUE INDEX [IX_NewsletterSubscriptions_Email] ON [NewsletterSubscriptions] ([Email]);

CREATE INDEX [IX_NewsletterSubscriptions_UserId] ON [NewsletterSubscriptions] ([UserId]);

CREATE UNIQUE INDEX [IX_Referrals_ReferralCode] ON [Referrals] ([ReferralCode]);

CREATE INDEX [IX_Referrals_ReferredUserId] ON [Referrals] ([ReferredUserId]);

CREATE INDEX [IX_Referrals_ReferrerUserId] ON [Referrals] ([ReferrerUserId]);

CREATE UNIQUE INDEX [IX_ResourceStrings_Key_LanguageId] ON [ResourceStrings] ([Key], [LanguageId]);

CREATE INDEX [IX_ResourceStrings_LanguageId] ON [ResourceStrings] ([LanguageId]);

ALTER TABLE [LoyaltyTransactions] ADD CONSTRAINT [FK_LoyaltyTransactions_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([Id]);

ALTER TABLE [LoyaltyTransactions] ADD CONSTRAINT [FK_LoyaltyTransactions_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260922081736_SyncModel', N'9.0.0');

COMMIT;
GO

