CREATE TABLE [dbo].[BalanceChangeType] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (250) NULL,
    [Multiplier]  INT            DEFAULT ((1)) NOT NULL,
    [IsDeleted]   BIT            DEFAULT ((0)) NOT NULL,
    [CompanyId]   INT            CONSTRAINT [DF_BalanceChangeType_CompanyId] DEFAULT ((1)) NOT NULL,
    [ValueAsDays] BIT            CONSTRAINT [DF_BalanceChangeType_ValueAsDays] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_BalanceChangeType] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_BalanceChangeType_Company] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Company] ([Id])
);





