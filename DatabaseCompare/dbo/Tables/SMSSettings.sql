CREATE TABLE [dbo].[SMSSettings] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [CompanyId]   INT            NOT NULL,
    [UserName]    NVARCHAR (128) NOT NULL,
    [Password]    NVARCHAR (128) NOT NULL,
    [AccountId]   INT            NOT NULL,
    [AccountName] NVARCHAR (128) NOT NULL,
    [DisplyName]  NVARCHAR (128) CONSTRAINT [DF__SMSSettin__Displ__498EEC8D] DEFAULT (N'GetWild') NOT NULL,
    [Balance]     INT            CONSTRAINT [DF__SMSSettin__Balan__4A8310C6] DEFAULT ((0)) NOT NULL,
    [Active]      BIT            CONSTRAINT [DF__SMSSettin__Activ__4B7734FF] DEFAULT ((1)) NOT NULL,
    [URL]         NVARCHAR (512) NULL,
    CONSTRAINT [PK_SMSSettings_1] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_SMSSettings_Company] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Company] ([Id])
);

