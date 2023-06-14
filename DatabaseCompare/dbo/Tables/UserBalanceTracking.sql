CREATE TABLE [dbo].[UserBalanceTracking] (
    [Id]             INT            IDENTITY (1, 1) NOT NULL,
    [SubscriptionId] INT            NOT NULL,
    [Date]           DATETIME       DEFAULT (getdate()) NOT NULL,
    [ChangeTypeId]   INT            NOT NULL,
    [Value]          INT            NOT NULL,
    [Balance]        INT            NOT NULL,
    [UserUpdated]    NVARCHAR (128) NOT NULL,
    [Note]           NVARCHAR (512) NULL,
    [OldValue]       NVARCHAR (50)  NULL,
    [NewValue]       NVARCHAR (50)  NULL,
    CONSTRAINT [PK_UserBalanceTracking] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UserBalanceTracking_AspNetUsers] FOREIGN KEY ([UserUpdated]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserBalanceTracking_BalanceChangeType] FOREIGN KEY ([ChangeTypeId]) REFERENCES [dbo].[BalanceChangeType] ([Id]),
    CONSTRAINT [FK_UserBalanceTracking_UserSubscription] FOREIGN KEY ([SubscriptionId]) REFERENCES [dbo].[UserSubscription] ([Id]) ON DELETE CASCADE
);





