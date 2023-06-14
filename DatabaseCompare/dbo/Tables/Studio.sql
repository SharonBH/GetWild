CREATE TABLE [dbo].[Studio] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (256) NOT NULL,
    [Address]     NVARCHAR (512) NULL,
    [ManagerName] NVARCHAR (256) NULL,
    [PhoneNumber] NVARCHAR (64)  NULL,
    [Email]       NVARCHAR (256) NOT NULL,
    [StartDate]   DATETIME       CONSTRAINT [DF_Studio_StartDate] DEFAULT (getdate()) NOT NULL,
    [Active]      BIT            CONSTRAINT [DF_Studio_Active] DEFAULT ((1)) NOT NULL,
    [CompanyId]   INT            NOT NULL,
    [IsDeleted]   BIT            CONSTRAINT [DF_Studio_IsDeleted] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Studio] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Studio_Company] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Company] ([Id])
);

