CREATE TABLE [dbo].[MSGTypes] (
    [Id]            INT            NOT NULL,
    [Name]          NVARCHAR (256) NOT NULL,
    [Message]       NVARCHAR (MAX) NOT NULL,
    [Active]        BIT            DEFAULT ((1)) NOT NULL,
    [AutoSend]      BIT            DEFAULT ((0)) NOT NULL,
    [TimeBefore]    INT            DEFAULT ((0)) NULL,
    [CompanyId]     INT            NOT NULL,
    [MessageTypeId] INT            CONSTRAINT [DF_MSGTypes_MessageTypeId] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_MSGTypes] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_MSGTypes_Company] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Company] ([Id])
);





