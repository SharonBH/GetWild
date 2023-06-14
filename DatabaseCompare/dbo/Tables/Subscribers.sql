CREATE TABLE [dbo].[Subscribers] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [CompanyId]   INT            NOT NULL,
    [Name]        NVARCHAR (256) NULL,
    [Email]       NVARCHAR (256) NULL,
    [PhoneNumber] NVARCHAR (64)  NULL,
    [Date]        DATETIME       CONSTRAINT [DF__Subscriber__Date__22AA2996] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_Subscribers] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Subscribers_Company] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Company] ([Id])
);

