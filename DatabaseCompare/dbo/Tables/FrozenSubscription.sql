CREATE TABLE [dbo].[FrozenSubscription] (
    [Id]             INT            IDENTITY (1, 1) NOT NULL,
    [SubscriptionId] INT            NOT NULL,
    [UserCreated]    NVARCHAR (128) NOT NULL,
    [Date]           DATETIME       NOT NULL,
    [FreezeToDate]   DATETIME       NULL,
    [Note]           NVARCHAR (512) NULL,
    [IsDeleted]      BIT            CONSTRAINT [DF__FrozenSub__IsDel__373B3228] DEFAULT ((0)) NOT NULL,
    [DateFinished]   DATETIME       NULL,
    [UserFinished]   NVARCHAR (128) NULL,
    CONSTRAINT [PK_FrozenSubscription] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK__FrozenSub__Subsc__44EA3301] FOREIGN KEY ([SubscriptionId]) REFERENCES [dbo].[UserSubscription] ([Id]),
    CONSTRAINT [FK_FrozenSubscription_AspNetUsers] FOREIGN KEY ([UserCreated]) REFERENCES [dbo].[AspNetUsers] ([Id]),
    CONSTRAINT [FK_FrozenSubscription_AspNetUsers1] FOREIGN KEY ([UserFinished]) REFERENCES [dbo].[AspNetUsers] ([Id])
);

