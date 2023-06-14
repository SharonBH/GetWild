CREATE TABLE [dbo].[SubscriptionType] (
    [Id]           INT             IDENTITY (0, 1) NOT NULL,
    [Name]         NVARCHAR (255)  NOT NULL,
    [Description]  NVARCHAR (1024) NOT NULL,
    [NumClasses]   INT             NOT NULL,
    [PeriodMonths] INT             NOT NULL,
    [Price]        FLOAT (53)      NOT NULL,
    [IsDeleted]    BIT             DEFAULT ((0)) NOT NULL,
    [StudioId]     INT             NOT NULL,
    CONSTRAINT [PK_SubscriptionType] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_SubscriptionType_Studio] FOREIGN KEY ([StudioId]) REFERENCES [dbo].[Studio] ([Id])
);



